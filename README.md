
# StarWarsAPI

This is a simple .NET Core 6.0 web API that implements the **Backends for Frontends (BFF)** pattern to provide an interface to interact with the [Star Wars API (swapi.dev)](https://swapi.dev). It allows users to authenticate, retrieve starship data, and filter starships by manufacturer. Additionally, the API supports pagination and caching for performance improvements.

## Features

- **Authentication**: JWT-based token authentication to secure the API.
- **Starship Retrieval**: Fetches starship data from the Star Wars API.
- **Filtering**: Users can filter starships by manufacturer.
- **Pagination**: Supports pagination for retrieving starship data.
- **Caching**: Implements in-memory caching to reduce API call overhead.
- **Error Handling**: Logs and handles errors gracefully, with meaningful error messages.

## Technologies Used

- **.NET Core 6.0**
- **ASP.NET Core Web API**
- **JWT Authentication**
- **HttpClient Factory**
- **In-Memory Caching**
- **xUnit**: Unit testing framework
- **Moq**: Mocking library for unit tests
- **Newtonsoft.Json**: JSON serialization and deserialization

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A code editor or IDE, such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/).

### Setup

1. **Clone the repository**:

   ```bash
   git clone https://github.com/Gabbs279/StarWarsAPI.git
   cd StarWarsAPI
   ```

2. **Install dependencies**:

   Make sure you are in the root directory of the project and restore dependencies:

   ```bash
   dotnet restore
   ```

3. **Update appsettings.json**:

   Configure your JWT settings in `appsettings.json`:

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "Jwt": {
       "Issuer": "yourIssuer",
       "Audience": "yourAudience",
       "Key": "supersecretlongkey1234567890123456"
     }
   }
   ```

4. **Run the API**:

   You can start the API locally by running the following command:

   ```bash
   dotnet run --project StarWarsAPI
   ```

   The API should now be running at `https://localhost:5001/`.

### Endpoints

#### Authentication

- **POST** `/api/auth/token`: 
  Generates a JWT token for use in subsequent requests.

#### Starship Retrieval

- **GET** `/api/starships`: 
  Retrieves a list of starships. Supports the following query parameters:
  
  - `manufacturer`: Filter starships by manufacturer.
  - `page`: Pagination, defaults to page 1.
  - `limit`: Number of results per page, defaults to 10.

  Example:

  ```bash
  curl -H "Authorization: Bearer {token}" https://localhost:5001/api/starships?manufacturer=Corellian&page=1&limit=5
  ```

### Running Tests

The project uses **xUnit** for unit tests. To run the tests, execute:

```bash
dotnet test
```

This will run all the unit tests located in the `StarWarsAPI.Tests` project.

### Architecture

This project follows the **Backends for Frontends (BFF)** pattern, ensuring that it acts as a secure, specific API tailored for clients interacting with the Star Wars API. Key architectural features include:

- **Controller-Service Pattern**: Controllers handle HTTP requests and delegate business logic to services (e.g., `StarshipService`).
- **HttpClient Factory**: Manages API calls to the external Star Wars API, ensuring resilience and performance.
- **Caching**: Uses in-memory caching to optimize repeated API calls for the same data.
- **Error Handling**: Catches and logs exceptions, returning user-friendly error messages to the client.

### Improvements

- **Rate Limiting**: Implement rate limiting to handle potential API request limitations from the external Star Wars API.
- **Data Persistence**: Add a database to store frequently accessed data or logging information.
- **Deployment**: Containerize the application using Docker and deploy it to cloud platforms like Azure or AWS.

## Contributing

Contributions are welcome! If you'd like to contribute, please fork the repository and make changes as you'd like. Pull requests are warmly welcomed.

### Steps to Contribute:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-branch-name`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature-branch-name`).
5. Create a new Pull Request.

## License

This project is licensed under the MIT License.

---

Happy Coding!
 
