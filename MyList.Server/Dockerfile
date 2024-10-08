# Stage 1: Build the application
# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the .csproj
COPY MyList.Server.csproj .

# Install app dependencies
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Build the application for production
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Use runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Set the working directory
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the application port
EXPOSE 8080

# Run EF migrations before starting the app
CMD ["dotnet", "MyList.Server.dll"]
