using ConferenceRoomAPI.Models;

namespace ConferenceRoomAPI.Services
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<List<Booking>> GetBookingsByTimeRangeAsync(int roomId, DateTime startTime, DateTime endTime);
    }
}
