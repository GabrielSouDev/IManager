FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
RUN apk add --no-cache icu-libs krb5-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IManager.Web/IManager.Web.csproj", "IManager.Web/"]
RUN dotnet restore "./IManager.Web/IManager.Web.csproj"
COPY . .
RUN rm -f IManager.Web/appsettings*.json
WORKDIR "/src/IManager.Web"
RUN dotnet build "./IManager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IManager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IManager.Web.dll"]