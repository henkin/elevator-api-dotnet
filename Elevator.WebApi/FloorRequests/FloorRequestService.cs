namespace Elevator.WebApi.Controllers;

/// <summary>
/// Elevator travel direction.
/// </summary>
public enum ElevatorTravelDirection
{
    /// <summary>
    /// Elevator is stationary.
    /// </summary>
    Stationary = 0,
    
    /// <summary>
    /// Up direction.
    /// </summary>
    Up = 1,
    
    /// <summary>
    /// Down direction.
    /// </summary>
    Down = 2,
}

/// <inheritdoc />
public class FloorRequestService : IFloorRequestService
{
    private readonly List<int> _outstandingRequests = new();

    /// <inheritdoc />
    public async Task AddFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default)
    {
        if (floorNumber <= 0)
        {
            throw new ArgumentException("Invalid floor number. Floor number must be a positive integer.",
                nameof(floorNumber));
        }

        if (!_outstandingRequests.Contains(floorNumber))
        {
            _outstandingRequests.Add(floorNumber);
        }
    }

    /// <inheritdoc />
    public async Task<List<int>> GetOutstandingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return _outstandingRequests;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default)
    {
        if (floorNumber <= 0)
        {
            throw new ArgumentException("Invalid floor number. Floor number must be a positive integer.",
                nameof(floorNumber));
        }

        // if floorNumber is not in the list, return false, otherwise remove it and return true
        if (!_outstandingRequests.Contains(floorNumber))
        {
            return false;
        }

        _outstandingRequests.Remove(floorNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<int?> GetNextFloorToStopAsync(
        int currentFloor,
        ElevatorTravelDirection currentElevatorTravelDirection,
        CancellationToken cancellationToken = default)
    {
        if (!_outstandingRequests.Any())
        {
            return null;
        }

        if (currentElevatorTravelDirection == ElevatorTravelDirection.Stationary) 
        {
            return _outstandingRequests.OrderBy(floor => Math.Abs(floor - currentFloor)).FirstOrDefault();
        }
        var requestsInDirection = GetRequestsInDirection(currentFloor, currentElevatorTravelDirection);

        if (requestsInDirection.Any())
        {
            return currentElevatorTravelDirection == ElevatorTravelDirection.Up
                ? requestsInDirection.Min()
                : requestsInDirection.Max();
        }

        var oppositeDirection = GetOppositeDirection(currentElevatorTravelDirection);
        var requestsInOppositeDirection = GetRequestsInDirection(currentFloor, oppositeDirection);

        if (requestsInOppositeDirection.Any())
        {
            return oppositeDirection == ElevatorTravelDirection.Up
                ? requestsInOppositeDirection.Min()
                : requestsInOppositeDirection.Max();
        }

        return null;
    }

    private IEnumerable<int> GetRequestsInDirection(int currentFloor, ElevatorTravelDirection direction)
    {
        return direction == ElevatorTravelDirection.Up
            ? _outstandingRequests.Where(floor => floor > currentFloor)
            : _outstandingRequests.Where(floor => floor < currentFloor);
    }

    private ElevatorTravelDirection GetOppositeDirection(ElevatorTravelDirection direction)
    {
        return direction == ElevatorTravelDirection.Up ? ElevatorTravelDirection.Down : ElevatorTravelDirection.Up;
    }
}
