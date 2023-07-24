using Microsoft.AspNetCore.Mvc;

namespace Elevator.WebApi.Controllers;

[Route("floor-requests")]
[ApiController]
public class FloorRequestsController : ControllerBase
{
    private readonly IFloorRequestService _floorRequestService;

    public FloorRequestsController(IFloorRequestService floorRequestService)
    {
        _floorRequestService = floorRequestService;
    }

    // POST: /floor-requests
    [HttpPost]
    public async Task<IActionResult> RequestElevator([FromBody] int floorNumber)
    {
        if (floorNumber <= 0)
        {
            return BadRequest("Invalid floor number. Floor number must be a positive integer.");
        }
            
        // Add the floor request to the list of outstanding requests
        await _floorRequestService.AddFloorRequestAsync(floorNumber);

        return Ok();
    }

    // GET: /floor-requests
    [HttpGet]
    public async Task<IActionResult> GetOutstandingRequests()
    {
        // Return the list of outstanding requests to the elevator car
        return Ok(await _floorRequestService.GetOutstandingRequestsAsync());
    }

    // GET: /floor-requests/next
    [HttpGet("next")]
    public async Task<IActionResult> GetNextFloorToStopAsync(
        int currentFloor, 
        ElevatorTravelDirection elevatorTravelDirection = ElevatorTravelDirection.Stationary)
    {
        int? nextFloor = await _floorRequestService.GetNextFloorToStopAsync(currentFloor, elevatorTravelDirection);

        if (nextFloor == null)
        {
            // No further requests in the specified direction; empty response
            return NoContent();
        }

        return Ok(nextFloor);
    }

    // DELETE: /floor-requests/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFulfilledRequest(int id)
    {
        bool isDeleted = await _floorRequestService.RemoveFloorRequestAsync(id);

        if (isDeleted)
        {
            return Ok();
        }

        return NotFound();
    }
}