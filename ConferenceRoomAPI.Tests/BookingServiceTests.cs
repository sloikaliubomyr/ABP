using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Models;
using ConferenceRoomAPI.Services;
using Xunit;

namespace ConferenceRoomAPI.Tests
{
    public class BookingServiceTests
    {
        private readonly BookingService _bookingService;
        private readonly IConferenceRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingServiceTests()
        {
            _roomRepository = new InMemoryConferenceRoomRepository();
            _bookingRepository = new InMemoryBookingRepository();
            _bookingService = new BookingService(_bookingRepository, _roomRepository);
        }

        [Fact]
        public async Task CreateBooking_ValidRequest_ReturnsBookingResponse()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 1, 12, 0, 0),
                SelectedServiceIds = new List<int> { 1 }
            };

            var result = await _bookingService.CreateBookingAsync(request);

            Assert.NotNull(result);
            Assert.Equal(1, result.RoomId);
            Assert.NotEqual(0, result.Id);
            Assert.True(result.TotalPrice > 0);
        }

        [Fact]
        public async Task CreateBooking_NonExistingRoom_ReturnsNull()
        {
            var request = new BookingRequest
            {
                RoomId = 999,
                StartTime = new DateTime(2024, 9, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 1, 12, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var result = await _bookingService.CreateBookingAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBooking_InvalidTimeRange_ThrowsArgumentException()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 1, 12, 0, 0),
                EndTime = new DateTime(2024, 9, 1, 10, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(request));
        }

        [Fact]
        public async Task CreateBooking_ConflictingTime_ThrowsInvalidOperationException()
        {
            var request1 = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 1, 12, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var request2 = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 1, 11, 0, 0),
                EndTime = new DateTime(2024, 9, 1, 13, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            await _bookingService.CreateBookingAsync(request1);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(request2));
        }

        [Fact]
        public async Task CreateBooking_IncludesServicePrices()
        {
            var request = new BookingRequest
            {
                RoomId = 2,
                StartTime = new DateTime(2024, 9, 2, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 2, 12, 0, 0),
                SelectedServiceIds = new List<int> { 1, 2, 3 }
            };

            var result = await _bookingService.CreateBookingAsync(request);

            Assert.True(result.PriceBreakdown.ServicesPrice > 0);
            Assert.Equal(1500, result.PriceBreakdown.ServicesPrice);
        }

        [Fact]
        public async Task GetBooking_ExistingBooking_ReturnsBookingResponse()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 3, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 3, 12, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var created = await _bookingService.CreateBookingAsync(request);
            var result = await _bookingService.GetBookingAsync(created.Id);

            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task GetBooking_NonExistingBooking_ReturnsNull()
        {
            var result = await _bookingService.GetBookingAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllBookings_ReturnsAllBookings()
        {
            var request1 = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 4, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 4, 12, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var request2 = new BookingRequest
            {
                RoomId = 2,
                StartTime = new DateTime(2024, 9, 4, 14, 0, 0),
                EndTime = new DateTime(2024, 9, 4, 16, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            await _bookingService.CreateBookingAsync(request1);
            await _bookingService.CreateBookingAsync(request2);

            var results = await _bookingService.GetAllBookingsAsync();

            Assert.True(results.Count >= 2);
        }

        [Fact]
        public async Task GetRoomBookings_ReturnsBookingsForRoom()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 5, 10, 0, 0),
                EndTime = new DateTime(2024, 9, 5, 12, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            await _bookingService.CreateBookingAsync(request);

            var results = await _bookingService.GetRoomBookingsAsync(1);

            Assert.NotEmpty(results);
            Assert.All(results, b => Assert.Equal(1, b.RoomId));
        }

        [Fact]
        public async Task CreateBooking_PeakHours_CalculatesMarkup()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 6, 12, 0, 0),
                EndTime = new DateTime(2024, 9, 6, 14, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var result = await _bookingService.CreateBookingAsync(request);

            var expectedPrice = 2000m * 2 * 1.15m;
            Assert.Equal(expectedPrice, result.PriceBreakdown.BaseRoomPrice);
            Assert.True(result.PriceBreakdown.Discount < 0);
        }

        [Fact]
        public async Task CreateBooking_EveningHours_CalculatesDiscount()
        {
            var request = new BookingRequest
            {
                RoomId = 1,
                StartTime = new DateTime(2024, 9, 7, 18, 0, 0),
                EndTime = new DateTime(2024, 9, 7, 22, 0, 0),
                SelectedServiceIds = new List<int>()
            };

            var result = await _bookingService.CreateBookingAsync(request);

            var expectedPrice = 2000m * 4 * 0.8m;
            var expectedDiscount = 2000m * 4 * 0.2m;
            Assert.Equal(expectedPrice, result.PriceBreakdown.BaseRoomPrice);
            Assert.Equal(expectedDiscount, result.PriceBreakdown.Discount);
        }
    }
}
