# Use the official .NET SDK as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Use the official .NET SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY . .
RUN dotnet restore "./PactProviderTests/PactProviderTests/PactProviderTests.csproj"
RUN dotnet restore "./dotnet-6-crud-api/WebApi.csproj"

RUN dotnet build "./dotnet-6-crud-api/WebApi.csproj"
RUN dotnet build "./PactProviderTests/PactProviderTests/PactProviderTests.csproj"

ENTRYPOINT ["dotnet", "test", "./PactProviderTests/PactProviderTests/PactProviderTests.csproj"]
