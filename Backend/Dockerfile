# Use the official .NET 9 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the rest of the source code
COPY . .

RUN dotnet restore WebApi/WebApi.csproj

# Build the application
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /app/publish

# Use the official .NET 9 runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the default port (adjust if needed)
EXPOSE 8080

# Set environment variables (optional, can be overridden)
# ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebApi.dll"]