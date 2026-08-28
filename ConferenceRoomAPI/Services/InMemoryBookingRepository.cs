using ConferenceRoomAPI.Models;

namespace ConferenceRoomAPI.Services
{
    public class InMemoryBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new();
        private int _nextId = 1;

        public Task<Booking> CreateBookingAsync(Booking booking)
        {
            booking.Id = _nextId++;
            _bookings.Add(booking);
            return Task.FromResult(booking);
        }

        public Task<Booking?> GetBookingByIdAsync(int id)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(booking);
        }

        public Task<List<Booking>> GetAllBookingsAsync()
        {
            return Task.FromResult(new List<Booking>(_bookings));
        }

        public Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            var bookings = _bookings.Where(b => b.RoomId == roomId).ToList();
            return Task.FromResult(bookings);
        }

        public Task<List<Booking>> GetBookingsByTimeRangeAsync(int roomId, DateTime startTime, DateTime endTime)
        {
            var bookings = _bookings.Where(b =>
                b.RoomId == roomId &&
                !(b.EndTime <= startTime || b.StartTime >= endTime)
            ).ToList();

            return Task.FromResult(bookings);
        }
    }
}
