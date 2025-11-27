# Build stage for .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copy project files
COPY *.csproj ./
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY DTO/*.csproj DTO/
COPY Infrastructure/*.csproj Infrastructure/

# Restore dependencies
RUN dotnet restore EventManagement.csproj

# Copy all source code
COPY . .

# Clean obj and bin directories that might be cached
RUN rm -rf obj bin
RUN find . -type d -name "obj" -o -name "bin" | xargs rm -rf

# Publish the application
RUN dotnet publish EventManagement.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published application
COPY --from=backend-build /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Release
ENV ASPNETCORE_URLS=http://+:5000

# Start the application
ENTRYPOINT ["dotnet", "EventManagement.dll"]