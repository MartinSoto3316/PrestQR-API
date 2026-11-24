using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using PrestQR_API.Auth;
using PrestQR_API.Data;
using PrestQR_API.Middleware;
using PrestQR_API.Repositories;
using PrestQR_API.Services;

namespace PrestQR_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                // Botón "Authorize" en Swagger: se pega el token del login y se envía en todas las peticiones.
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Token obtenido en POST /api/auth/login"
                };
                document.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] }];
                return Task.CompletedTask;
            }));

            // Autenticación con JWT: cada petición protegida debe traer "Authorization: Bearer {token}".
            var jwt = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>() ?? new JwtOpciones();
            if (Encoding.UTF8.GetByteCount(jwt.Clave) < 32)
            {
                throw new InvalidOperationException(
                    "Falta 'Jwt:Clave' o tiene menos de 32 caracteres. En desarrollo va en appsettings.Development.json; en el hosting, en la variable de entorno Jwt__Clave.");
            }
            builder.Services.Configure<JwtOpciones>(builder.Configuration.GetSection(JwtOpciones.Seccion));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Conserva los nombres cortos de los claims ("sub", "role") en lugar de los de Microsoft.
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwt.Emisor,
                        ValidAudience = jwt.Audiencia,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
                        NameClaimType = ClaimsUsuario.UsuarioId,
                        RoleClaimType = ClaimsUsuario.Rol,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = RespuestasAuth.Eventos();
                });
            builder.Services.AddAuthorization();

            // CORS: permite que la PWA (otro origen) consuma la API desde el navegador.
            // En desarrollo se acepta cualquier puerto de localhost; en producción solo los orígenes configurados.
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.SetIsOriginAllowed(origin => Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.IsLoopback);
                }
                else
                {
                    policy.WithOrigins(builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? []);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            }));

            // Errores → respuestas ProblemDetails (400/404/409)
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Base de datos (Neon / PostgreSQL) con Dapper
            DefaultTypeMap.MatchNamesWithUnderscores = true; // codigo_qr → CodigoQr
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

            var connectionString = builder.Configuration.GetConnectionString("PrestQR")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'ConnectionStrings:PrestQR'. En desarrollo va en appsettings.Development.json; en el hosting, en la variable de entorno ConnectionStrings__PrestQR.");
            builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));

            // Repositorios (acceso a datos)
            builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
            builder.Services.AddScoped<IAlumnoRepository, AlumnoRepository>();
            builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IEstadisticasRepository, EstadisticasRepository>();

            // Servicios (reglas de negocio)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEstadisticasService, EstadisticasService>();
            builder.Services.AddScoped<IEquipoService, EquipoService>();
            builder.Services.AddScoped<IAlumnoService, AlumnoService>();
            builder.Services.AddScoped<IPrestamoService, PrestamoService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                // Página para probar los endpoints: http://localhost:5279/swagger
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "PrestQR API"));
            }

            app.UseHttpsRedirection();

            app.UseCors();

            // Primero se identifica al usuario (token) y después se revisa su rol.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
