namespace Elevator.WebApi.Controllers;

public interface IFloorRequestService
{
    /// <summary>
    /// Adds a floor request to the list of outstanding requests.
    /// If the floorNumber is already in the list, do nothing.
    /// </summary>
    /// <param name="floorNumber">Floor to stop at.</param>
    /// <param name="cancellationToken">(Optional) cancellation token for more complex implementations.</param>
    /// <exception cref="ArgumentException">If floorNumber is 0 or negative.</exception>
    Task AddFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the list of outstanding floor requests.
    /// </summary>
    /// <param name="cancellationToken">(Optional) cancellation token for more complex implementations.</param>
    /// <returns></returns>
    Task<List<int>> GetOutstandingRequestsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a floor request from the list of outstanding requests.
    /// If the floorNumber is not in the list, return false.
    /// If the floorNumber is in the list, remove it and return true.
    /// </summary>
    /// <param name="floorNumber">Floor number to remove.</param>
    /// <param name="cancellationToken">(optional) cancellation token for more complex implementations.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If floorNumber is 0 or negative.</exception>
    Task<bool> RemoveFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next floor to stop at, based on the current floor and direction of travel.
    /// If the outstandingRequests list is empty, return null.
    /// If currentFloor is a floor in outstandingRequests, skip it, and return the next one in the direction of travel.
    /// </summary>
    /// <param name="currentFloor">Current floor of the elevator.</param>
    /// <param name="currentElevatorTravelDirection">The direction the elevator is traveling.</param>
    /// <param name="cancellationToken">(optional) cancellation token for more complex implementations.</param>
    /// <returns>The next floor to stop</returns>
    Task<int?> GetNextFloorToStopAsync(
        int currentFloor, 
        ElevatorTravelDirection currentElevatorTravelDirection,
        CancellationToken cancellationToken = default);
}