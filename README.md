# Elevator Control System API 

This is an example of a Web API using aspnet core with basic error handling and integration tests.

The Elevator Control System API provides a simple interface for managing elevator requests. This API allows multiple dependent teams to integrate and test their interactions with the elevator control system efficiently.

Created by [Paul Henkin](https://www.linkedin.com/in/henkin/) as a coding exercise.

### tl;dr 
- `make run`
- [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) to access the Swagger UI.
---

## Overview

The Elevator API is a RESTful web service that provides functionalities to manage elevator floor requests. The API allows users to request an elevator to a specific floor, view all outstanding floor requests, get the next floor to stop at based on the current direction of the elevator, and remove fulfilled floor requests.

The API is implemented using .NET Core and exposes endpoints that can be accessed through HTTP requests. The solution includes both the API project (`Elevator.WebApi`) and the unit tests project (`Elevator.WebApi.Tests`).

## Requirements

The API is designed to meet the following requirements:

1. Provide endpoints to request an elevator to a specific floor, view all outstanding floor requests, get the next floor to stop at based on the current direction of the elevator, and remove fulfilled floor requests.

2. Support integration testing with a dedicated endpoint to run unit tests and generate test coverage reports.

## Usage

### Build the Solution

To build the Elevator API solution, use the following command:

```bash
make build
```

### Run the API

To run the Elevator API locally, use the following command:

```bash
make run
```

The API will be available at `http://localhost:8080`.

### Run Unit Tests

To run unit tests for the API, use the following command:

```bash
make test
```

This command will run all the unit tests and generate a code coverage report in HTML format. The coverage report will be available in the [`coveragereport` directory](./coveragereport/).

### Deploy the API

To deploy the API to Heroku, use the following command:

```bash
make deploy
```

This command will build the Docker image, tag it with the appropriate name, and push it to the Heroku container registry. Finally, it will release the container to the Heroku app named `elevator-api`.

### Load Testing

To perform load testing using Yandex.Tank, use the following command:

```bash
make load
```

This command will run Yandex.Tank using the load test configuration in the `loadtest` directory.

## API Endpoints

The Elevator API exposes the following endpoints:

### 1. Request an Elevator to a Specific Floor

**Endpoint:** POST `/floor-requests`

**Request Body:**
```
5
```

**Response:**
- 200 OK: The floor request was successfully added.
- 400 Bad Request: If the floor number is not a positive integer.

### 2. View All Outstanding Floor Requests

**Endpoint:** GET `/floor-requests`

**Response:**
- 200 OK: Returns a list of all outstanding floor requests.

### 3. Get Next Floor to Stop At

**Endpoint:** GET `/floor-requests/next`

**Query Parameters:**
- `currentFloor` [int]: The current floor of the elevator.
- `elevatorTravelDirection` [string, optional]: The current direction of the elevator (`"Up"`, `"Down"`, or `"Stationary"`).

**Response:**
- 200 OK: Returns the next floor to stop at based on the current direction of the elevator.
- 204 No Content: If there are no outstanding requests in the specified direction.

### 4. Remove Fulfilled Floor Request

**Endpoint:** DELETE `/floor-requests/{id}`

**Path Parameter:**
- `id` [int]: The ID of the fulfilled floor request to be removed.

**Response:**
- 200 OK: The floor request was successfully removed.
- 404 Not Found: If the specified floor request ID does not exist.

## Assumptions

1. The Elevator API assumes that the input floor numbers are positive integers greater than zero.

2. The Elevator API handles the scenario where there are no outstanding floor requests and returns appropriate responses.

3. If the elevator is stationary, the API returns the closest floor in any direction as the next stop.

4. If there are no outstanding requests in the current direction of travel, the API goes to the nearest opposite direction floor.

5. The API supports integration testing using the provided Makefile and Docker.

6. The API is deployable to Heroku using the provided Makefile.

## Use Cases

The Elevator API is designed to satisfy the following use cases:

1. **Request an Elevator:** Users can request an elevator to a specific floor by providing the floor number. The API ensures that the floor number is a positive integer greater than zero.

2. **View Outstanding Requests:** The API allows users to view all outstanding floor requests.

3. **Get Next Floor to Stop:** Users can get the next floor for the elevator to stop at based on the current direction of travel. If there are no requests in the current direction, the API goes to the nearest opposite direction floor.

4. **Remove Fulfilled Request:** Users can remove a fulfilled floor request by providing the ID of the request.

5. **Unit Testing:** The API supports comprehensive unit testing with code coverage reports.

6. **Integration Testing:** The provided Makefile allows for easy integration testing of the API.

> Future:
> 7. **Containerization and Deployment:** The API >can be containerized using Docker and deployed to Heroku with a single command.

