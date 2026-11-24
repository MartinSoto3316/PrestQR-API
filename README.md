# PrestQR API

API REST de **PrestQR**, el sistema de préstamo y devolución de equipos escolares mediante códigos QR.
La consume la app móvil (PWA) del repositorio **[PrestQR-PWA](https://github.com/MartinSoto3316/PrestQR-PWA)**, donde también está la documentación del proyecto (`docs/`).

[![CircleCI](https://dl.circleci.com/status-badge/img/gh/MartinSoto3316/PrestQR-API/tree/main.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/MartinSoto3316/PrestQR-API/tree/main)

- **Producción:** https://prestqr.onrender.com (la primera petición puede tardar ~1 min: el plan gratuito de Render apaga el servicio tras 15 min sin uso)
- **Swagger (solo en desarrollo):** http://localhost:5279/swagger

> **¿Solo quieres probar la app?** No necesitas este repositorio: la API ya está desplegada y la app móvil la usa por defecto. Sigue el *Inicio rápido* de [PrestQR-PWA](https://github.com/MartinSoto3316/PrestQR-PWA#inicio-rápido).
> Las instrucciones de abajo son para **desarrollar la API** en tu computadora.

## Tecnologías

| Herramienta | Versión | Uso |
|---|---|---|
| .NET SDK / ASP.NET Core | 10.0 | API REST, inyección de dependencias, autorización por roles |
| Dapper | 2.1.89 | Consultas SQL y mapeo a objetos |
| Npgsql | 10.0.3 | Conexión a PostgreSQL |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.13 | Autenticación con JWT |
| BCrypt.Net-Next | 4.2.0 | Hash de contraseñas |
| Microsoft.AspNetCore.OpenApi + Swashbuckle.AspNetCore.SwaggerUI | 10.0.13 / 10.3.1 | Documentación y prueba de endpoints |
| PostgreSQL (Neon en producción) | 16 o superior | Base de datos |
| Docker | 24 o superior | Imagen para Render |
| CircleCI | config 2.1 | Integración y despliegue continuos |

## Estructura

```
Controllers/     Endpoints HTTP y autorización por rol
Services/        Lógica de aplicación y validaciones
Repositories/    Consultas SQL con Dapper
Models/          Entidades y DTOs
Auth/            Opciones y claims del JWT
Middleware/      Traducción de errores a respuestas ProblemDetails (400/401/404/409)
Exceptions/      Excepciones de negocio
Data/            Configuración de Dapper (DateOnly)
database/        schema.sql (tablas, restricciones, funciones) y seed.sql (datos de prueba)
scripts/         generar-hash.fsx (hash BCrypt para crear usuarios)
.circleci/       Pipeline: build → imagen Docker → despliegue en Render
```

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL 16 o superior: local, en Docker o una base gratuita en [Neon](https://neon.tech)
- Git
- Docker (opcional, para levantar PostgreSQL o probar la imagen)

## Ejecución local (para desarrollar la API)

### 1. Clonar el repositorio

```bash
git clone https://github.com/MartinSoto3316/PrestQR-API.git
cd PrestQR-API
```

### 2. Crear la base de datos

Si no tienes PostgreSQL, puedes levantarlo con Docker:

```bash
docker run -d --name prestqr-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=prestqr -p 5432:5432 postgres:16
```

Crea las tablas, restricciones y funciones, y después carga los datos de prueba:

```bash
psql "postgresql://postgres:postgres@localhost:5432/prestqr" -f database/schema.sql
psql "postgresql://postgres:postgres@localhost:5432/prestqr" -f database/seed.sql
```

> Sin `psql` instalado: abre cada archivo en pgAdmin, DBeaver o el SQL Editor de Neon y ejecútalo completo (primero `schema.sql`, luego `seed.sql`).

### 3. Configurar la API

Copia el archivo de ejemplo y completa tus valores:

```bash
cp appsettings.Development.example.json appsettings.Development.json
```

| Clave | Descripción |
|---|---|
| `ConnectionStrings:PrestQR` | Cadena de conexión a PostgreSQL. En Neon agrega `SSL Mode=Require`. |
| `Jwt:Clave` | Clave para firmar los tokens. **Mínimo 32 caracteres**; si falta, la API no arranca. |

`appsettings.Development.json` está en `.gitignore`: **nunca se sube al repositorio**.
El resto de la configuración (emisor, audiencia, expiración de 480 min) ya viene en `appsettings.json`.

### 4. Ejecutar

```bash
dotnet run --launch-profile http
```

- API: http://localhost:5279
- Swagger: http://localhost:5279/swagger

### 5. Probar

1. En Swagger, `POST /api/auth/login` con un usuario de prueba de `database/seed.sql`.
2. Copia el `token` de la respuesta, presiona **Authorize** y pégalo.
3. Prueba, por ejemplo, `GET /api/equipos/qr/LAP-001` (disponible) y `GET /api/equipos/qr/CAM-001` (prestado).

También puedes usar el archivo `PrestQR API.http` desde Visual Studio o VS Code.

## Configuración en producción (Render)

En Render las claves se definen como **variables de entorno** (`:` se escribe `__`):

| Variable | Obligatoria | Ejemplo |
|---|---|---|
| `ConnectionStrings__PrestQR` | Sí | `Host=<host>.neon.tech;Database=PrestQR;Username=<usuario>;Password=<contraseña>;SSL Mode=Require` |
| `Jwt__Clave` | Sí | Cadena aleatoria de 32 caracteres o más |
| `Cors__OrigenesPermitidos__0` | Sí | URL pública de la PWA, por ejemplo `https://prestqr-pwa.onrender.com` |
| `Jwt__MinutosExpiracion` | No | `480` (valor por defecto) |
| `ASPNETCORE_ENVIRONMENT` | No | `Production` (valor por defecto en el contenedor) |

### Docker

```bash
docker build -t prestqr-api .
docker run -p 8080:8080 -e ConnectionStrings__PrestQR="<cadena>" -e Jwt__Clave="<clave>" prestqr-api
```

La API queda en http://localhost:8080.

## Integración y despliegue continuos

`.circleci/config.yml` se ejecuta en cada push:

1. **build:** `dotnet restore` y `dotnet build` en Release.
2. **docker-build:** verifica que la imagen Docker se construye.
3. **deploy** (solo `main`): llama al *deploy hook* de Render (`RENDER_DEPLOY_HOOK_URL`, configurado en CircleCI).

## Flujo de trabajo con Git

- `main`: versión estable y desplegada.
- Ramas de trabajo (`feature/...`, ramas personales): se fusionan a `main` mediante pull request revisado por otro integrante.
- Mensajes de commit con *Conventional Commits*: `feat:`, `fix:`, `docs:`, `refactor:`, `chore:`.

## Equipo

Martín Ricardo Soto Gastelo · Rodrigo Gutiérrez Morquecho · Norberto Carrillo Leyva
Universidad Tecnológica del Sur de Sonora · Desarrollo móvil integral
