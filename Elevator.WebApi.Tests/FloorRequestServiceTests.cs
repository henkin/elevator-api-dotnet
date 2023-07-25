using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elevator.WebApi.Controllers;
using Xunit;

namespace Elevator.WebApi.Tests;

public class FloorRequestServiceTests
{
    private readonly FloorRequestService _floorRequestService;

    public FloorRequestServiceTests()
    {
        _floorRequestService = new FloorRequestService();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public async Task AddFloorRequestAsync_ValidFloorNumber_AddsToOutstandingRequests(int floorNumber)
    {
        // Act
        await _floorRequestService.AddFloorRequestAsync(floorNumber);

        // Assert
        var outstandingRequestsAsync = await _floorRequestService.GetOutstandingRequestsAsync();
        outstandingRequestsAsync.Should().Contain(floorNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task AddFloorRequestAsync_InvalidFloorNumber_ThrowsArgumentException(int floorNumber)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _floorRequestService.AddFloorRequestAsync(floorNumber));
    }

    [Fact]
    public async Task GetOutstandingRequestsAsync_ReturnsOutstandingRequests()
    {
        // Arrange
        var requests = new List<int> { 5, 10, 15 };
        foreach (var floor in requests)
        {
            await _floorRequestService.AddFloorRequestAsync(floor);
        }

        // Act
        var outstandingRequests = await _floorRequestService.GetOutstandingRequestsAsync();

        // Assert
        outstandingRequests.Should().BeEquivalentTo(requests);
    }

    public static IEnumerable<object[]> TestData =>
        new List<object[]>
        {
            new object[] { "Moves to next floor in the same direction", ElevatorTravelDirection.Up, new[] { 5, 10, 15 }, 10, 15 },
            new object[] { "Moves to next floor in the same direction", ElevatorTravelDirection.Up, new[] { 5, 10, 15 }, 12, 15 },
            new object[] { "No more requests in the same direction, goes to nearest opposite direction floor", ElevatorTravelDirection.Up, new[] { 5, 10, 15 }, 20, 15 },
            new object[] { "Moves to next floor in the opposite direction", ElevatorTravelDirection.Down, new[] { 5, 10, 15 }, 10, 5 },
            new object[] { "Moves to next floor in the opposite direction", ElevatorTravelDirection.Down, new[] { 5, 10, 15 }, 8, 5 },
            new object[] { "No more requests in the opposite direction, goes to nearest same direction floor", ElevatorTravelDirection.Down, new[] { 5, 10, 15 }, 1, 5 },
            new object[] { "Moves to the closest floor in any direction when stationary", ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, 10, 10 },
            new object[] { "Moves to the closest floor in any direction when stationary", ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, 8, 10 },
            new object[] { "Moves to the closest floor in any direction when stationary", ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, 12, 10 },
            new object[] { "Current floor is zero, goes to nearest floor in any direction when stationary", ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, 0, 5 },
            new object[] { "Current floor is negative, goes to nearest floor in any direction when stationary", ElevatorTravelDirection.Stationary, new[] { 5, 10, 15 }, -1, 5 },
        };

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task GetNextFloorToStopAsync(string testName, ElevatorTravelDirection direction, IEnumerable<int> requestedFloors, int currentFloor, int? expectedNextFloor)
    {
        // Arrange
        await SetFloorRequestsAsync(requestedFloors);

        // Act
        var result = await _floorRequestService.GetNextFloorToStopAsync(currentFloor, direction);

        // Assert
        result.Should().Be(expectedNextFloor, because: testName);
    }
    
    private async Task SetFloorRequestsAsync(IEnumerable<int> floorNumbers)
    {
        foreach (var floor in floorNumbers)
        {
            await _floorRequestService.AddFloorRequestAsync(floor);
        }
    }
    
    [Fact]
    public async Task RemoveFloorRequestAsync_ValidRequest_ReturnsTrueAndRemovesRequest()
    {
        // Arrange
        int floorNumber = 5;
        await _floorRequestService.AddFloorRequestAsync(floorNumber);

        // Act
        bool isDeleted = await _floorRequestService.RemoveFloorRequestAsync(floorNumber);

        // Assert
        isDeleted.Should().BeTrue();
        (await _floorRequestService.GetOutstandingRequestsAsync()).Should().NotContain(floorNumber);
    }

    [Fact]
    public async Task RemoveFloorRequestAsync_InvalidRequest_ReturnsFalse()
    {
        // Arrange
        int floorNumber = 5;

        // Act
        bool isDeleted = await _floorRequestService.RemoveFloorRequestAsync(floorNumber);

        // Assert
        isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveFloorRequestAsync_InvalidFloorNumber_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _floorRequestService.RemoveFloorRequestAsync(0));
    }
}