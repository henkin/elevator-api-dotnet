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

/// <summary>
/// The backing store for the floor requests, which is injected into the controller.
/// For now, keep it simple and just use a list of integers. 
/// </summary>
public class FloorRequestService : IFloorRequestService
{
    private readonly List<int> _outstandingRequests = new();

    /// <inheritdoc />
    public async Task AddFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default)
    {
        if (floorNumber <= 0)
        {
            throw new ArgumentException("Invalid floor number. Floor number must be a positive integer.", nameof(floorNumber));
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
            throw new ArgumentException("Invalid floor number. Floor number must be a positive integer.", nameof(floorNumber));
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

        // Calculate the next stop based on the current direction of travel
        int nextFloor;
        switch (currentElevatorTravelDirection)
        {
            case ElevatorTravelDirection.Up:
                nextFloor = _outstandingRequests.Where(floor => floor > currentFloor).Min();
                break;
            case ElevatorTravelDirection.Down:
                nextFloor = _outstandingRequests.Where(floor => floor < currentFloor).Max();
                break;
            default:
            {
                // If the elevator is stationary, find the closest floor in any direction
                nextFloor = _outstandingRequests.OrderBy(floor => Math.Abs(floor - currentFloor)).First();
                break;
            }
        }
        
        return nextFloor;
    }
}
