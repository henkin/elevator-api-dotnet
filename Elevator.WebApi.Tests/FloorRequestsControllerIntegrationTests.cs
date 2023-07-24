using System.Net.Http.Json;
using System.Text.Json;
using Elevator.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Elevator.WebApi.Tests;

public class FloorRequestsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task TestGetNextFloorToStop_WithOutstandingRequests()
    {
        // Arrange
        int floorNumber = 5;
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
            
        await PostFloorRequest(client, floorNumber);

        // Act
        var floorRequests = await GetFloorRequests(client);

        // Assert
        floorRequests.Should().BeEquivalentTo(new int[] { floorNumber });
    }

    [Fact]
    public async Task TestGetNextFloorToStop_WithNoOutstandingRequests()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var floorRequests = await GetFloorRequests(client);

        // Assert
        floorRequests.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task PostFloorRequest_ValidFloorNumber_Returns200(int floorNumber)
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/floor-requests", floorNumber);

        // Assert
        response.Should().Be200Ok();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task PostFloorRequest_InvalidFloorNumber_Returns400(int floorNumber)
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/floor-requests", floorNumber);

        // Assert
        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task GetOutstandingRequests_WithNoRequests_ReturnsEmptyArray()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/floor-requests");

        // Assert 
        response.Should().Be200Ok();
        var floorRequests = await response.Content.ReadFromJsonAsync<int[]>();
        floorRequests.Should().BeEmpty();
    }

    [Theory]
    [InlineData(ElevatorTravelDirection.Up, new int[] { }, 5, null)] // No requests, next floor is empty
    [InlineData(ElevatorTravelDirection.Down, new int[] { }, 0, null)] // No requests, next floor is empty
    [InlineData(ElevatorTravelDirection.Up, new[] { 5, 10, 15 }, 10, 15)] // Valid next floor is 15
    [InlineData(ElevatorTravelDirection.Down, new[] { 5, 10, 15 }, 9, 5)] // Valid next floor is 5
    [InlineData(ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, 10, 10)] // Elevator is stationary, next floor is the closest one
    [InlineData(ElevatorTravelDirection.Stationary, new int[] { }, 5, null)] // Elevator is stationary, no matching requests, next floor is null

    public async Task GetNextFloorToStopConditions(
        ElevatorTravelDirection direction,
        int[] requestedFloors,
        int currentFloor,
        int? nextFloor)
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        foreach (var floor in requestedFloors)
        {
            await PostFloorRequest(client, floor);
        }
        
        // Act
        var response = await client.GetAsync($"/floor-requests/next?currentFloor={currentFloor}&elevatorTravelDirection={direction}");

        // Assert
        if (nextFloor != null)
        {
            response.Should().Be200Ok();
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Be(nextFloor.ToString());
        }
        else
        {
            response.Should().Be204NoContent();
        }
    }

    [Fact]
    public async Task DeleteRequest_ValidId_Returns200()
    {
        // Arrange
        int requestId = 1;
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        await PostFloorRequest(client, requestId);
        
        // Act
        var response = await client.DeleteAsync($"/floor-requests/{requestId}");

        // Assert
        response.Should().Be200Ok();
    }

    [Fact]
    public async Task DeleteRequest_InvalidId_Returns404()
    {
        // Arrange
        int requestId = 999;
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/floor-requests/{requestId}");

        // Assert
        response.Should().Be404NotFound();
    }
    
    /// <summary>
    /// GetFloorRequests is a helper method that sends a GET request to the /floor-requests endpoint.
    /// It's a convenient way to get the list of outstanding requests from the elevator car.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private static async Task<int[]?> GetFloorRequests(HttpClient client)
    {
        var response = await client.GetAsync("/floor-requests");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var outstandingRequests = JsonSerializer.Deserialize<int[]>(responseBody);
        return outstandingRequests;
    }
    
    /// <summary>
    /// PostFloorRequest is a helper method that sends a POST request to the /floor-requests endpoint.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="floorNumber"></param>
    private async Task PostFloorRequest(HttpClient client, int floorNumber)
    {
        await client.PostAsync("/floor-requests",
            new StringContent(floorNumber.ToString(), System.Text.Encoding.Default, "application/json"));
    }
}