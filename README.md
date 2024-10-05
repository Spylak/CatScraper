# Cat Scraper API

## Description
Please note: This repository is intended for learning purposes to explore .NET minimal APIs concepts, Unit Testing and Docker Development setup.

Cat Scraper API is an ASP.NET Core Web API application that interacts with an external Cat-as-a-Service (CaaS) API to fetch, store, and serve cat images. This application demonstrates various features including image handling, database operations with Entity Framework Core, RESTful API design, and OpenAPI (Swagger) documentation.
~~~~
Key Features:
- Fetch and store cat images from an external API
- Store cat data (image, dimensions, tags) in SQL Server using Entity Framework Core
- Retrieve cat images by ID
- List cats with pagination support
- Filter cats by tags (based on cat temperaments)
- Secure API access using an API key
- OpenAPI (Swagger) documentation for easy API testing and integration

## Prerequisites

- .NET 8 SDK
- Docker
- An API key for the Cat-as-a-Service (CaaS) API
- GNU Make

## Setup and Running the Application

Follow these steps to set up and run the Cat Scraper API:

1. **Clone the Repository**
   ```
   git clone https://github.com/Spylak/CatScraper.git
   cd CatScraper
   ```

2. **Set Up the Database**
   Run the following command to start a SQL Server instance in Docker:
   ```
   make docker-mssql
   ```
   This will start a SQL Server container named `cat-scraper-db` on port 1433.

3. **Update "CatsApi:ApiKey"**
   Open `appsettings.{ENV}.json` and provide your actual CaaS API key.
   Alternatively, you can provide your actual CaaS API key in the Authorize function of swagger since the API project takes into consideration the 'x-api-key' for a user provided API key, which also override the apps API key.~~~~
   ```json
     "CatsApi" : {
       "ApiKey" : ""
     }
   ```

4. **Build and Run the Docker Image**
   ```
   make docker-build
   ```
   
5. **Access the API**
   The API will now be running on `http://localhost:5000`. You can access the Swagger UI at `http://localhost:5000/swagger`.

## API Endpoints

- `POST /api/cats/fetch`: Fetch 25 cat images from CaaS API and save them to the database.
- `GET /api/cats/{id}`: Retrieve a cat by its ID.
- `GET /api/images/{id}`: Retrieve a cat image by its ID.
- `GET /api/cats`: Retrieve cats with paging support (e.g., `/api/cats?page=1&pageSize=10`).
- `GET /api/cats`: Retrieve cats with a specific tag (e.g., `/api/cats?tag=playful&page=1&pageSize=10`).

## Running Tests

To run the unit tests for the application:
You can provide your API key in the ApiTests file if you only want to run the tests project or override the apps API key.
```
make dotnet-test
```

## Additional Commands

- Remove the Docker image:
  ```
  make docker-remove-image
  ```

- Remove the last migration (if not applied to the database):
  ```
  make remove-migration
  ```

## Troubleshooting

- If you encounter issues with database connections, ensure the SQL Server container is running and the connection string is correct.
- Make sure to not have an active service running on 1433 when running tests or setting up docker ms-sql server.

## Contributing
Contributions are welcome! This project is designed for learning and collaboration, so any feedback, suggestions, or pull requests are appreciated.

## License
MIT License

