FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files to restore dependencies
COPY *.slnx ./
COPY GameHaven.Api/*.csproj ./GameHaven.Api/
COPY GameHaven.Application/*.csproj ./GameHaven.Application/
COPY GameHaven.Domain/*.csproj ./GameHaven.Domain/
COPY GameHaven.Infrastructure/*.csproj ./GameHaven.Infrastructure/

RUN dotnet restore GameHaven.Api/GameHaven.Api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish GameHaven.Api/GameHaven.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Use polling file watcher for Render / Linux limits if necessary (same as NainOrder)
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GameHaven.Api.dll"]
