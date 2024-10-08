# Use the official Node.js image as the base image
FROM node:20

# Set the working directory
WORKDIR /usr/src/app

# Copy the package.json and package-lock.json (if available)
COPY package*.json .

# Install app dependencies
RUN npm install --production

# Copy the rest of the application code
COPY . .

# Override runtime config
ENV API_BASE_URL=http://mylist-server:8080/mylist-api/**

# Expose the port that your app runs on
EXPOSE 3000

# Build the application for production
RUN npm run build

# Define the command to run your app
CMD ["node", ".output/server/index.mjs"]
