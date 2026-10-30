using Dapper;
using Npgsql;
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
            builder.Services.AddOpenApi();

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

            // Servicios (reglas de negocio)
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
