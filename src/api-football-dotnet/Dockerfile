# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiowanie plików projektów i przywracanie zależności
COPY ["Road2Senior/Road2Senior.csproj", "Road2Senior/"]
COPY ["Application/Application.csproj","Application/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Database/Database.csproj", "Database/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["IntegrationTests/IntegrationTests.csproj", "IntegrationTests/"]
RUN dotnet restore "Road2Senior/Road2Senior.csproj"

# Reszta źródeł
COPY . .
WORKDIR "/src/Road2Senior"
RUN dotnet build "Road2Senior.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== PUBLISH =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Road2Senior.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== FINAL =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Road2Senior.dll"]
