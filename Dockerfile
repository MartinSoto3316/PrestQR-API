# Imagen de la API de PrestQR para Render (o cualquier hosting con Docker).
# Se construye desde esta carpeta: docker build -t prestqr-api .

# Etapa 1: compilar y publicar la API con el SDK de .NET
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PrestQR API.csproj", "./"]
RUN dotnet restore "PrestQR API.csproj"

COPY . .
RUN dotnet publish "PrestQR API.csproj" -c Release -o /app/publish --no-restore

# Etapa 2: imagen final, solo con el runtime de ASP.NET (más ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Render detecta este puerto automáticamente
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PrestQR API.dll"]
