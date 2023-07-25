# [Paul Henkin](https://www.linkedin.com/in/henkin/) Elevator Control System API 

The Elevator Control System API provides a simple interface for managing elevator requests. This API allows multiple dependent teams to integrate and test their interactions with the elevator control system efficiently.

> TL;DR: 
> 1. `make run`
> 2. [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) to access the Swagger UI.

_talktopaul@gmail.com_

## Table of Contents

- [Introduction](#introduction)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
    - [Building the Solution](#building-the-solution)
    - [Running the API](#running-the-api)
    - [Running the Tests](#running-the-tests)
- [API Endpoints](#api-endpoints)
    - [POST /floor-requests](#post-floor-requests)
    - [GET /floor-requests](#get-floor-requests)
    - [GET /floor-requests/next](#get-floor-requestsnext)
    - [DELETE /floor-requests/{id}](#delete-floor-requestsid)

## Introduction

The Elevator Control System API allows clients to request elevator services and retrieve information about the pending elevator requests. It's designed to provide a minimal set of functionalities to unblock the integration of other teams while allowing flexibility for future extension.

The API supports the following scenarios:

1. A person can request an elevator to be sent to their current floor.
2. A person can request to be brought to a specific floor.
3. An elevator car can request the floors that its current passengers are servicing.
4. An elevator car can request the next floor it needs to service.

The API is built using ASP.NET Core and includes Swagger/OpenAPI documentation for easy exploration and integration.

## Prerequisites

To use the Elevator Control System API, you need the following:

- .NET Core SDK 5.0 or higher
- Make (optional but recommended for easy command execution)

## Getting Started

Follow the steps below to build, run, and test the Elevator Control System API.

### Running the API

To run the API, open a terminal in the root of the `Elevator` solution folder and run the following command:

```bash
make run
```

Alternatively, you can run the following command without using Make:

```bash
dotnet run --project Elevator/Elevator.WebApi/Elevator.WebApi.csproj
```

The API will be available at `http://localhost:8080`.

### Running the Tests

To run the tests for the API, open a terminal in the root of the `Elevator` solution folder and run the following command:

```bash
make test
```

Alternatively, you can run the following command without using Make:

```bash
dotnet test Elevator/Elevator.WebApi.Tests/Elevator.WebApi.Tests.csproj
```

## Assumptions and Corner Cases

While designing the Elevator Control System API, the following assumptions have been made:

1. **Floor Number Validation**: It is assumed that floor numbers must be positive integers. Negative floor numbers and zero are considered invalid inputs and will result in a `400 Bad Request` response when making a request to create a floor request.

2. **Direction of Travel**: The API supports three directions of travel: "Up", "Down", and "Stationary." The elevator is considered stationary when there are no outstanding requests. If an invalid direction is provided in the `GetNextFloorToStop` endpoint, an `InvalidOperationException` will be thrown.

3. **Nearest Floor in Opposite Direction**: When the elevator is stationary, and there are no requests in the current direction of travel, it will find the nearest floor in the opposite direction. If no requests exist in either direction, the API will return `204 No Content`.

4. **Duplicate Requests**: The API does not prevent duplicate floor requests for the same floor. If a floor is requested multiple times, it will be serviced as separate requests, but not added multiple times. 

5. **Concurrent Requests**: The API is designed to handle multiple concurrent requests from different clients. The underlying service should be thread-safe to manage concurrent access to the list of outstanding requests.

6. **Deleting Fulfilled Requests**: When calling the `DELETE` endpoint to remove a fulfilled request, the API will return `200 OK` if the request was successfully deleted and `404 Not Found` if the specified ID does not match any outstanding requests.

7. **Error Handling**: The API includes error handling for various scenarios, such as invalid floor numbers, missing elevator requests, and incorrect endpoint usage. It returns appropriate HTTP status codes and error messages to help clients identify and handle errors.

By keeping these assumptions and corner cases in mind, clients can effectively integrate and test their interactions with the Elevator Control System API, ensuring smooth and reliable elevator operations.

## API Endpoints

The Elevator Control System API provides the following endpoints:

### POST /floor-requests

Creates a new floor request to call an elevator to the specified floor.

#### Request

- Method: POST
- Endpoint: `/floor-requests`
- Body: Integer representing the floor number

#### Response

- 200 OK: The floor request was successfully created.
- 400 Bad Request: If the floor number is not a positive integer.

### GET /floor-requests

Retrieves the list of outstanding elevator floor requests.

#### Request

- Method: GET
- Endpoint: `/floor-requests`

#### Response

- 200 OK: Returns a JSON array containing the list of outstanding elevator floor requests.

### GET /floor-requests/next

Retrieves the next floor the elevator needs to stop at based on the current floor and direction of travel.

#### Request

- Method: GET
- Endpoint: `/floor-requests/next`
- Query Parameters:
    - `currentFloor`: Integer representing the current floor of the elevator.
    - `elevatorTravelDirection` (optional): The current direction of travel for the elevator. (Values: "Up", "Down", or "Stationary")

#### Response

- 200 OK: Returns the next floor the elevator needs to stop at.
- 204 No Content: If there are no more outstanding requests in the specified direction.

### DELETE /floor-requests/{id}

Deletes a fulfilled elevator floor request by its ID.

#### Request

- Method: DELETE
- Endpoint: `/floor-requests/{id}`
- Path Parameter: `id` (Integer) - The ID of the fulfilled request to be deleted.

#### Response

- 200 OK: The request was successfully deleted.
- 404 Not Found: If the specified ID does not match any outstanding requests.

## Swagger/OpenAPI Documentation

The Elevator Control System API includes Swagger documentation that allows easy exploration and testing of the API endpoints. To access the Swagger documentation, run the API and navigate to the following URL in your browser:

```
http://localhost:8080/swagger/index.html
```

The Swagger UI will provide a user-friendly interface to interact with the API endpoints, view request/response examples, and get familiar with the API's capabilities.

That's it! You now have the Elevator Control System API up and running, along with the tools to explore and test its endpoints. Happy elevator controlling! 🛗