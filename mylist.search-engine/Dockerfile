# Stage 1: Build the application
# Use the official Maven image with OpenJDK 17 as the base image
FROM maven:3.8.5-openjdk-17 AS build

# Set the working directory
WORKDIR /app

# Copy the pom.xml and download dependencies
COPY pom.xml .
RUN mvn dependency:go-offline

# Copy the rest of the application code
COPY src ./src

# Package the application using Maven
RUN mvn clean package -DskipTests

# Stage 2: Use runtime image for production
FROM openjdk:17-jdk-slim AS runtime

# Set the working directory
WORKDIR /app

# Copy the target output from the build stage
COPY --from=build /app/target .

# Expose the port your application will run on
EXPOSE 8080

# Run the Spring Boot application
ENTRYPOINT ["java", "-jar", "mylist-search-engine-1.0.0.jar"]
