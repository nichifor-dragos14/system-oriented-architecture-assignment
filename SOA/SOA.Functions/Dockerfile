# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./SOA.Functions/SOA.Functions.csproj
RUN dotnet publish ./SOA.Functions/SOA.Functions.csproj -c Release -o /app/site/wwwroot

# Run on Azure Functions base
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /app/site/wwwroot .
# The Functions image hosts the app; no custom ENTRYPOINT required.
