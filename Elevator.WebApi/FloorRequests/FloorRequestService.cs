namespace Elevator.WebApi.Controllers;

/// <summary>
/// The backing store for the floor requests, which is injected into the controller.
/// For now, keep it simple and just use a list of integers.
/// </summary>
public class FloorRequestService : IFloorRequestService
{
    private readonly List<int> outstandingRequests = new List<int>();
        
    public async Task AddFloorRequest(int floorNumber)
    {
        outstandingRequests.Add(floorNumber);
    }
        
    public async Task<List<int>> GetOutstandingRequests()
    {
        return outstandingRequests;
    }
        
    public async Task<int> GetNextFloorToStop()
    {
        if (outstandingRequests.Count == 0)
        {
            throw new InvalidOperationException("There are no outstanding requests.");
        }
            
        int nextFloor = outstandingRequests[0];
        outstandingRequests.RemoveAt(0);
        return nextFloor;
    }
}