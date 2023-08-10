using Microsoft.AspNetCore.Mvc;

namespace Elevator.WebApi.Controllers;

/// <summary>
/// Floor requests controller for managing elevator floor requests.
/// </summary>
[Route("floor-requests")]
[ApiController]
public class FloorRequestsController : ControllerBase
{
    private readonly IFloorRequestService _floorRequestService;
    private readonly ILogger<FloorRequestsController> _logger;

    /// <summary>
    /// Constructor for the FloorRequestsController.
    /// </summary>
    /// <param name="floorRequestService">The service backing floor requests.</param>
    /// <param name="logger"></param>
    public FloorRequestsController(IFloorRequestService floorRequestService, ILogger<FloorRequestsController> logger)
    {
        _floorRequestService = floorRequestService;
        _logger = logger;
    }

    /// <summary>
    /// Request an elevator to a specific floor.
    /// </summary>
    /// <param name="floorNumber">The floor number where the elevator is requested to stop.</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /floor-requests
    ///     Request Body: 5
    /// 
    /// </remarks>
    /// <response code="200">The floor request was successful.</response>
    /// <response code="400">If the floorNumber is not a positive integer.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestElevator([FromBody] int floorNumber)
    {
        if (floorNumber <= 0)
        {
            _logger.LogWarning("Invalid floor request received for floor number {FloorNumber}", floorNumber);
            return BadRequest("Invalid floor number. Floor number must be a positive integer.");
        }

        _logger.LogInformation("Processing elevator request for floor number {FloorNumber}", floorNumber);

        await _floorRequestService.AddFloorRequestAsync(floorNumber);

        return Ok();
    }

    /// <summary>
    /// Get the list of outstanding floor requests for the elevator.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /floor-requests
    /// 
    /// </remarks>
    /// <response code="200">The list of outstanding floor requests.</response>
    /// <response code="204">No outstanding requests found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetOutstandingRequests()
    {
        // Return the list of outstanding requests to the elevator car
        _logger.LogInformation("Fetching outstanding elevator requests");
        return Ok(await _floorRequestService.GetOutstandingRequestsAsync());
    }

    /// <summary>
    /// Get the next floor where the elevator needs to stop based on its current position and travel direction.
    /// </summary>
    /// <param name="currentFloor">The current floor of the elevator.</param>
    /// <param name="elevatorTravelDirection">The current travel direction of the elevator (optional, default is Stationary).</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /floor-requests/next?currentFloor=10&amp;elevatorTravelDirection=Up
    /// 
    /// </remarks>
    /// <response code="200">The next floor number where the elevator needs to stop.</response>
    /// <response code="204">No matching requests found for the given direction.</response>
    [HttpGet("next")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetNextFloorToStopAsync(
        int currentFloor, 
        ElevatorTravelDirection elevatorTravelDirection = ElevatorTravelDirection.Stationary)
    {
        if (currentFloor <= 0)
        {
            _logger.LogWarning("Invalid current floor provided {CurrentFloor}", currentFloor);
            return BadRequest("Invalid floor number. Floor number must be a positive integer.");
        }

        _logger.LogInformation(
            "Fetching next floor to stop from current floor {CurrentFloor} with travel direction {TravelDirection}",
            currentFloor, 
            elevatorTravelDirection);

        int? nextFloor = await _floorRequestService.GetNextFloorToStopAsync(currentFloor, elevatorTravelDirection);

        if (nextFloor == null)
        {
            // No further requests in the specified direction; empty response
            _logger.LogInformation(
                "No matching requests found for current floor {CurrentFloor} and direction {TravelDirection}",
                currentFloor, 
                elevatorTravelDirection);
            return NoContent();
        }

        return Ok(nextFloor);
    }


    /// <summary>
    /// Delete a fulfilled floor request from the list of outstanding requests.
    /// </summary>
    /// <param name="floorNumber">The ID of the fulfilled floor request to delete.</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /floor-requests/1
    /// 
    /// </remarks>
    /// <response code="200">The floor request was successfully deleted.</response>
    /// <response code="404">If the floor request with the specified ID was not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{floorNumber}")]
    public async Task<IActionResult> DeleteFulfilledRequest(int floorNumber)
    {
        _logger.LogInformation("Attempting to delete fulfilled request for floor number {FloorNumber}", floorNumber);
        bool isDeleted = await _floorRequestService.RemoveFloorRequestAsync(floorNumber);

        if (isDeleted)
        {
            _logger.LogInformation("Successfully deleted request for floor number {FloorNumber}", floorNumber);
            return Ok();
        }

        _logger.LogWarning("Request for floor number {FloorNumber} not found", floorNumber);
        return NotFound();
    }
}