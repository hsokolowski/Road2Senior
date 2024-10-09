FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Road2Senior/Road2Senior.csproj", "Road2Senior/"]
COPY ["Api/Api.csproj", "Api/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Database/Database.csproj", "Database/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Road2Senior/Road2Senior.csproj"
COPY . .
WORKDIR "/src/Road2Senior"
RUN dotnet build "Road2Senior.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Road2Senior.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Road2Senior.dll"]
