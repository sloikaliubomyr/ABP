using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingsController(BookingService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] BookingRequest request)
        {
            if (request.RoomId <= 0)
                return BadRequest("Invalid room ID.");

            if (request.StartTime >= request.EndTime)
                return BadRequest("Start time must be before end time.");

            try
            {
                var booking = await _service.CreateBookingAsync(request);
                if (booking == null)
                    return NotFound("Room not found.");

                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResponse>> GetBooking(int id)
        {
            var booking = await _service.GetBookingAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found.");

            return Ok(booking);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BookingResponse>>> GetAllBookings()
        {
            var bookings = await _service.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("room/{roomId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BookingResponse>>> GetRoomBookings(int roomId)
        {
            if (roomId <= 0)
                return BadRequest("Invalid room ID.");

            var bookings = await _service.GetRoomBookingsAsync(roomId);
            return Ok(bookings);
        }
    }
}
