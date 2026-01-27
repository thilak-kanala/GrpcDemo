FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WSTCConsumer/WSTCConsumer.csproj", "WSTCConsumer/"]
RUN dotnet restore "WSTCConsumer/WSTCConsumer.csproj"
COPY . .
WORKDIR "/src/WSTCConsumer"
RUN dotnet build "./WSTCConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WSTCConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WSTCConsumer.dll"]
