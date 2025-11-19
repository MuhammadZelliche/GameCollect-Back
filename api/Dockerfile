# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ProjetSecret/*.csproj ./ProjetSecret/
WORKDIR /src/ProjetSecret
RUN dotnet restore

COPY ProjetSecret/ /tmp/ProjetSecret/
WORKDIR /tmp/ProjetSecret

RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "ProjetSecret.dll"]
