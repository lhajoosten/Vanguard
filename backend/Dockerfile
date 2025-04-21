# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Web/Vanguard.WebApi/Vanguard.WebApi.csproj", "src/Web/Vanguard.WebApi/"]
COPY ["src/Core/Vanguard.Application/Vanguard.Application.csproj", "src/Core/Vanguard.Application/"]
COPY ["src/Core/Vanguard.Core/Vanguard.Core.csproj", "src/Core/Vanguard.Core/"]
COPY ["src/Core/Vanguard.Domain/Vanguard.Domain.csproj", "src/Core/Vanguard.Domain/"]
COPY ["src/Infrastructure/Vanguard.Infrastructure/Vanguard.Infrastructure.csproj", "src/Infrastructure/Vanguard.Infrastructure/"]
COPY ["src/Infrastructure/Vanguard.Persistence/Vanguard.Persistence.csproj", "src/Infrastructure/Vanguard.Persistence/"]
RUN dotnet restore "./src/Web/Vanguard.WebApi/Vanguard.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Web/Vanguard.WebApi"
RUN dotnet build "./Vanguard.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vanguard.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vanguard.WebApi.dll"]