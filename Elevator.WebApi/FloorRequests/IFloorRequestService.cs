namespace Elevator.WebApi.Controllers;

public interface IFloorRequestService
{
    public Task AddFloorRequest(int floorNumber);
    public Task<List<int>> GetOutstandingRequests();
    public Task<int> GetNextFloorToStop();
}