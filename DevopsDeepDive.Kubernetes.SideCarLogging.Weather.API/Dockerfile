# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
VOLUME "/application-logs"
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API/DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API.csproj", "DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API/"]
RUN dotnet restore "./DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API/DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API.csproj"
COPY . .
WORKDIR "/src/DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API"
RUN dotnet build "./DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user and switch to that user & Set file ownership to the non-root user
#RUN useradd -ms /bin/bash appuser && \
    #chown -R appuser:appuser /app && \
    #chmod 775 /app 

#switch to non-root user
#USER appuser

#RUN mkdir -p -v /log/myapp/
#chown www-data:www-data /log/myapp/
#chmod 755 /log/myapp


ENTRYPOINT ["dotnet", "DevopsDeepDive.Kubernetes.SideCarLogging.Weather.API.dll"]