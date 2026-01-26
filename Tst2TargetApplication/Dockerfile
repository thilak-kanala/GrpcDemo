FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tst2TargetApplication/Tst2TargetApplication.csproj", "Tst2TargetApplication/"]
RUN dotnet restore "Tst2TargetApplication/Tst2TargetApplication.csproj"
COPY . .
WORKDIR "/src/Tst2TargetApplication"
RUN dotnet build "./Tst2TargetApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tst2TargetApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tst2TargetApplication.dll"]
