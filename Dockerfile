# Simple Dockerfile for .NET 8.0 application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files for restore
COPY ["EventManagement.csproj", "./"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "EventManagement.csproj"

# Copy source code and build
COPY . .
RUN dotnet publish "EventManagement.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Release

# Start the application
ENTRYPOINT ["dotnet", "EventManagement.dll"]