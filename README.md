# MyList

MyList allows you to create and edit your own lists (to-do lists, movie lists, book lists, music lists, etc.)  

<img src="screenshots/MyList.gif" />

## Installation

### Prerequisites

#### Docker setup:

- [Docker](https://www.docker.com/)

#### Manual setup:

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- EF Core CLI tools (`dotnet tool install --global dotnet-ef`)
- [Node.js 20+](https://nodejs.org/en)
- [Java 17+](https://jdk.java.net/)
- [Maven](https://maven.apache.org/)
- [Optional] [Nginx](https://nginx.org/en/)

### Run (Docker)

Running with docker is as simple as running one command.  

1. [Optional] Edit the configurations to your liking (nginx, Dockerfiles, etc.)
2. Run `docker compose up -d --build`

### Run (Manual)

If you don't have docker or wish to run `MyList` manually, follow the steps below.  
You should configure Nginx to proxy the requests if you want to use it on another device (i.e.: other than `localhost`, e.g.: your phone).  

#### Web application

Working directory: `mylist.client`

1. Install dependencies with `npm install --production`
2. Set environment variable `API_BASE_URL=http://localhost:5000/mylist-api/**`
3. Build project with `npm run build`
4. Run with `node .output/server/index.mjs`

You should now be able to view the page at `localhost:3000`.  

#### API

Working directory: `MyList.Server`

1. Edit the `appsettings.json` file
    - Change the `SearchEngineUri` to `http://localhost:8080` (or wherever your [Search Engine](#search-engine) is running)
    - Change the connection string to where your database is located (it's shared with the [Search Engine](#search-engine))
2. Build project with `dotnet publish -c Release -o publish`
3. Copy the fresh database file `mylist.db`* (or make your own with `dotnet ef database update`) to the `publish` folder
4. Enter the `publish` directory with `cd publish`
5. Run with `dotnet MyList.Server.dll`

You should now be able to access the API at `localhost:5000`.  

\* This file is located in the repository's base folder `data`.  

#### Search Engine

Working directory: `mylist.search-engine`

1. Edit the `application.properties` file
    - Change the `spring.datasource.url` to where your database is located (it's shared with the [API](#api))
2. Build the project with `mvn clean package -DskipTests`
3. Enter the `target` directory with `cd target`
4. Run with `java -jar mylist-search-engine-[VERSION].jar`

You should now be able to access the Search Engine API at `localhost:8080`
