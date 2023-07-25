# Elevator Control System API 
[![actions](https://github.com/henkin/elevator-api-dotnet/actions/workflows/dotnet.yml/badge.svg)]()

This is an example of a Web API using ASP.NET Core with basic error handling and integration tests.

The Elevator Control System API provides a simple interface for managing elevator requests. This API allows multiple dependent teams to integrate and test their interactions with the elevator control system efficiently.

Created by [Paul Henkin](https://www.linkedin.com/in/henkin/) as a coding exercise.

### tl;dr
- `make run`
- [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) to access the Swagger UI.
- Test report will be generated after running `make test` and coverage report will be available in the [`coveragereport` directory](./coveragereport/).

---

# Elevator API - README

## Overview

The Elevator API is a .NET Core web application that provides a RESTful API for managing elevator requests. It allows users to request elevators to their current floor, be brought to a specific floor, and query the next floor an elevator needs to stop at.

## Requirements

To build, run, and test the Elevator API, make sure you have the following prerequisites installed on your machine:

- .NET 5 SDK (or later) - [Download .NET](https://dotnet.microsoft.com/download)
- Make (optional, but recommended for using the provided Makefile) - [Download Make](https://www.gnu.org/software/make/)

## Getting Started

To get started with the Elevator API, follow the steps below:

1. Clone the repository:

```bash
git clone https://github.com/your-username/elevator-api.git
cd elevator-api
```

2. Build and run the API locally using the provided Makefile (recommended):

```bash
# Build the solution
make build

# Run the API locally
make run
```

Alternatively, you can build and run the API manually:

```bash
# Build the solution
dotnet build Elevator.WebApi/Elevator.WebApi.csproj

# Run the API locally
dotnet run --project Elevator.WebApi/Elevator.WebApi.csproj
```

The API will be accessible at `http://localhost:8080`.

## API Endpoints

The Elevator API provides the following endpoints:

### Request an Elevator to Current Floor

- Endpoint: `POST /floor-requests`
- Request Body: The floor number where the elevator is requested. The floor number must be a positive integer.
- Response: Status 200 (OK) if the request is successful, Status 400 (Bad Request) if the floor number is invalid.

### Request to Be Brought to a Specific Floor

- Endpoint: `POST /floor-requests`
- Request Body: The floor number where the user wants to be brought. The floor number must be a positive integer.
- Response: Status 200 (OK) if the request is successful, Status 400 (Bad Request) if the floor number is invalid.

### Get Outstanding Requests

- Endpoint: `GET /floor-requests`
- Response: Status 200 (OK) along with the list of outstanding floor requests.

### Get Next Floor to Stop

- Endpoint: `GET /floor-requests/next`
- Query Parameters:
    - `currentFloor`: The current floor of the elevator (a positive integer).
    - `elevatorTravelDirection`: Optional. The direction of travel for the elevator (0 for stationary, 1 for up, 2 for down).
- Response: Status 200 (OK) with the next floor where the elevator needs to stop, or Status 204 (No Content) if there are no outstanding requests.

### Delete Fulfilled Request

- Endpoint: `DELETE /floor-requests/{id}`
- Path Parameter: The ID of the fulfilled request to be deleted.
- Response: Status 200 (OK) if the request is deleted successfully, Status 404 (Not Found) if the request ID is not found.

## Using the Makefile

The provided Makefile simplifies common tasks related to building, running, testing, and deploying the Elevator API. Here are the available targets and their usage:

- `make build`: Build the Elevator API project.
- `make run`: Run the Elevator API locally on `http://localhost:8080`.
- `make test`: Run unit tests and generate a test coverage report.
- `make deploy`: Build, publish, and deploy the Elevator API to Heroku (requires Docker and Heroku CLI).
- `make load`: Run load tests using Yandex Tank (requires Docker).
- `make clean`: Remove build artifacts and test coverage reports.

## API Assumptions

The Elevator API makes the following assumptions:

- Floor numbers are positive integers, starting from 1.
- Elevator direction is specified as 0 (stationary), 1 (up), or 2 (down).
- The API is stateful and maintains a list of outstanding floor requests.
- An elevator can handle multiple requests at once.

## Use Cases

The Elevator API is designed to meet the following use cases:

1. Office Building - Users request elevators to their current floor, specifying the desired floor they want to be brought to.
2. Elevator Car System - Elevator cars request outstanding floor requests to light up buttons indicating the floors they need to service.
3. Control Center - The control center can query the next floor an elevator needs to stop at based on its current floor and travel direction.

## API Endpoints in `curl`

1. Request an elevator to the 5th floor:
```bash
curl -X POST "http://localhost:8080/floor-requests" -H "Content-Type: application/json" -d "5"
```

2. Request to be brought to the 10th floor:
```bash
curl -X POST "http://localhost:8080/floor-requests" -H "Content-Type: application/json" -d "10"
```

3. Get outstanding floor requests:
```bash
curl -X GET "http://localhost:8080/floor-requests"
```

4. Get next floor to stop for the elevator on the 7th floor, going up:
```bash
curl -X GET "http://localhost:8080/floor-requests/next?currentFloor=7&elevatorTravelDirection=1"
```

## Conclusion

Thanks for coming! I hope you enjoyed this exercise as much as I did. If you have any questions or feedback, please feel free to reach out to me at my [linkedin](https://www.linkedin.com/in/henkin/).