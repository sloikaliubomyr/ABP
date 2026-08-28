using ConferenceRoomAPI.Models;
using ConferenceRoomAPI.Services;
using Xunit;

namespace ConferenceRoomAPI.Tests
{
    public class BookingRepositoryTests
    {
        private readonly IBookingRepository _repository = new InMemoryBookingRepository();

        [Fact]
        public async Task CreateBooking_ValidBooking_ReturnsBookingWithId()
        {
            var booking = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int> { 1 })
            {
                TotalPrice = 4500
            };

            var result = await _repository.CreateBookingAsync(booking);

            Assert.NotEqual(0, result.Id);
            Assert.Equal(1, result.RoomId);
            Assert.Equal(4500, result.TotalPrice);
        }

        [Fact]
        public async Task GetBookingById_ExistingBooking_ReturnsBooking()
        {
            var booking = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int> { 1 })
            {
                TotalPrice = 4500
            };

            var created = await _repository.CreateBookingAsync(booking);
            var result = await _repository.GetBookingByIdAsync(created.Id);

            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task GetBookingById_NonExistingBooking_ReturnsNull()
        {
            var result = await _repository.GetBookingByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllBookings_ReturnsAllBookings()
        {
            var booking1 = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            var booking2 = new Booking(0, 2,
                new DateTime(2024, 9, 1, 14, 0, 0),
                new DateTime(2024, 9, 1, 16, 0, 0),
                new List<int>()) { TotalPrice = 7000 };

            await _repository.CreateBookingAsync(booking1);
            await _repository.CreateBookingAsync(booking2);

            var results = await _repository.GetAllBookingsAsync();

            Assert.True(results.Count >= 2);
        }

        [Fact]
        public async Task GetBookingsByRoomId_MultipleBookings_ReturnsRoomBookings()
        {
            var booking1 = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            var booking2 = new Booking(0, 1,
                new DateTime(2024, 9, 1, 15, 0, 0),
                new DateTime(2024, 9, 1, 17, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            var booking3 = new Booking(0, 2,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 7000 };

            await _repository.CreateBookingAsync(booking1);
            await _repository.CreateBookingAsync(booking2);
            await _repository.CreateBookingAsync(booking3);

            var results = await _repository.GetBookingsByRoomIdAsync(1);

            Assert.All(results, b => Assert.Equal(1, b.RoomId));
            Assert.True(results.Count >= 2);
        }

        [Fact]
        public async Task GetBookingsByTimeRange_ConflictingBookings_ReturnsConflictingBookings()
        {
            var booking = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            await _repository.CreateBookingAsync(booking);

            var conflictingStart = new DateTime(2024, 9, 1, 11, 0, 0);
            var conflictingEnd = new DateTime(2024, 9, 1, 13, 0, 0);

            var results = await _repository.GetBookingsByTimeRangeAsync(1, conflictingStart, conflictingEnd);

            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task GetBookingsByTimeRange_NonConflictingBookings_ReturnsEmptyList()
        {
            var booking = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            await _repository.CreateBookingAsync(booking);

            var nonConflictingStart = new DateTime(2024, 9, 1, 14, 0, 0);
            var nonConflictingEnd = new DateTime(2024, 9, 1, 16, 0, 0);

            var results = await _repository.GetBookingsByTimeRangeAsync(1, nonConflictingStart, nonConflictingEnd);

            Assert.Empty(results);
        }

        [Fact]
        public async Task GetBookingsByTimeRange_AdjacentBookings_NoConflict()
        {
            var booking = new Booking(0, 1,
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 12, 0, 0),
                new List<int>()) { TotalPrice = 4000 };

            await _repository.CreateBookingAsync(booking);

            var adjacentStart = new DateTime(2024, 9, 1, 12, 0, 0);
            var adjacentEnd = new DateTime(2024, 9, 1, 14, 0, 0);

            var results = await _repository.GetBookingsByTimeRangeAsync(1, adjacentStart, adjacentEnd);

            Assert.Empty(results);
        }
    }
}
