# Elevator Control API

## Overview

This API is being developed by a new team tasked with supplying an interface to be used by multiple other teams integrating with an elevator control system.

The goal is to unblock dependent teams by providing a simple API that enables testing their integrations. The initial focus is on the interface rather than complete business logic. 

The API needs to support:

- Requesting an elevator to a floor
- Requesting a floor destination
- Elevator requesting floors to service
- Elevator requesting next floor to stop at

## Assumptions
- There is a single elevator, and a single set of floor requests, such that when a rider requests the elevator, it will light up on the single elevator's button bank. 

## Endpoints

### POST /floor-requests

Used to request an elevator be sent to the specified floor.

**Request Body**

```json
{
  "floorNumber": 5
}
```

**Response**  

- 200 OK if request accepted
- 400 Bad Request if invalid floor

### GET /floor-requests

Used by elevator to retrieve outstanding pickup requests.

**Response**

```json 
[
  5, 
  3,
  10
]
```

## Getting Started

To run the API service:

1. Clone the git repository 
2. Run `make run`

This will build the .NET project and execute `dotnet run` to start the service on port 8080.

The API can then be accessed from localhost:8080.

## Example Requests

Request elevator to floor 5:

```
POST /floor-requests
{
  "floorNumber": 5
} 
```

Get outstanding requests:

```
GET /floor-requests
```

## Development Approach

- Makefile for builds and execution
- Feature branches for development
- xUnit for unit testing
- Parameterized requests for easy testing
- Descriptive commits and PR reviews

This allows moving quickly while maintaining code quality.

## Next Steps

The initial focus is the API interface, but capabilities can be added incrementally:

- Persistent storage of requests
- Additional logic to optimize routing  
- Authentication and access controls
- Admin functionality

Let me know if you need any clarification or have additional requirements!