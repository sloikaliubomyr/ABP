using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceRoomsController : ControllerBase
    {
        private readonly ConferenceRoomService _service;

        public ConferenceRoomsController(ConferenceRoomService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConferenceRoomResponse>> CreateRoom([FromBody] CreateConferenceRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Room name is required.");

            if (request.Capacity <= 0)
                return BadRequest("Capacity must be greater than 0.");

            if (request.BaseHourlyRate <= 0)
                return BadRequest("Base hourly rate must be greater than 0.");

            var room = await _service.CreateRoomAsync(request);
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConferenceRoomResponse>> GetRoom(int id)
        {
            var room = await _service.GetRoomAsync(id);
            if (room == null)
                return NotFound($"Room with ID {id} not found.");

            return Ok(room);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ConferenceRoomResponse>>> GetAllRooms()
        {
            var rooms = await _service.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ConferenceRoomResponse>>> SearchAvailableRooms(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] int capacity)
        {
            if (startTime >= endTime)
                return BadRequest("Start time must be before end time.");

            if (capacity <= 0)
                return BadRequest("Capacity must be greater than 0.");

            var rooms = await _service.SearchAvailableRoomsAsync(startTime, endTime, capacity);
            return Ok(rooms);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateConferenceRoomRequest request)
        {
            if (request.Capacity.HasValue && request.Capacity <= 0)
                return BadRequest("Capacity must be greater than 0.");

            if (request.BaseHourlyRate.HasValue && request.BaseHourlyRate <= 0)
                return BadRequest("Base hourly rate must be greater than 0.");

            var updated = await _service.UpdateRoomAsync(id, request);
            if (!updated)
                return NotFound($"Room with ID {id} not found.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var deleted = await _service.DeleteRoomAsync(id);
            if (!deleted)
                return NotFound($"Room with ID {id} not found.");

            return NoContent();
        }
    }
}
