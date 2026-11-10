# Build from the solution root: docker build -f MES.Api/Dockerfile -t mes-api:local .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MES.Domain/MES.Domain.csproj MES.Domain/
COPY MES.Application/MES.Application.csproj MES.Application/
COPY MES.Infrastructure/MES.Infrastructure.csproj MES.Infrastructure/
COPY MES.Algorithm/MES.Algorithm.csproj MES.Algorithm/
COPY MES.MLTrainer/MES.MLTrainer.csproj MES.MLTrainer/
COPY MES.Api/MES.Api.csproj MES.Api/
RUN dotnet restore MES.Api/MES.Api.csproj

COPY . .
RUN dotnet publish MES.Api/MES.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MES.Api.dll"]