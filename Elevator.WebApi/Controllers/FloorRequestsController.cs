using Microsoft.AspNetCore.Mvc;

namespace Elevator.WebApi.Controllers
{
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
            await _floorRequestService.AddFloorRequest(floorNumber);

            // Return a successful response
            return Ok();
        }

        // GET: /floor-requests
        [HttpGet]
        public async Task<IActionResult> GetOutstandingRequests()
        {
            // Return the list of outstanding requests to the elevator car
            return Ok(await _floorRequestService.GetOutstandingRequests());
        }

        // // GET: /floor-requests/next
        // [HttpGet("next")]
        // public async Task<IActionResult> GetNextFloorToStop()
        // {
        //     // If there are no outstanding requests, return an empty response
        //     if (outstandingRequests.Count == 0)
        //     {
        //         return NoContent();
        //     }
        //
        //     // Return the next floor to stop at (first request in the list)
        //     int nextFloor = outstandingRequests[0];
        //     return Ok(nextFloor);
        // }

        // // Error handling for unhandled exceptions
        // [Route("/error")]
        // public IActionResult Error()
        // {
        //     return Problem("An unexpected error occurred. Please try again later.", statusCode: 500);
        // }
    }
}
