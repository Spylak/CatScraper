# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore dependencies
COPY ["CatScraper/CatScraper.csproj", "./"]
RUN dotnet restore "CatScraper.csproj"

COPY ["CatScraper.Tests/CatScraper.Tests.csproj", "./"]
RUN dotnet restore "CatScraper.Tests.csproj"

# Copy the remaining source code
COPY . .
WORKDIR "/src"

# Build the application
RUN dotnet build "CatScraper/CatScraper.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "CatScraper/CatScraper.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://*:5195

# Change user to non-root for security reasons
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Set the entry point for the container
ENTRYPOINT ["dotnet", "CatScraper.dll"]
