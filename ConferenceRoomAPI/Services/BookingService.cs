using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Models;
using ConferenceRoomAPI.Utilities;

namespace ConferenceRoomAPI.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IConferenceRoomRepository _roomRepository;

        public BookingService(IBookingRepository bookingRepository, IConferenceRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<BookingResponse?> CreateBookingAsync(BookingRequest request)
        {
            var room = await _roomRepository.GetRoomByIdAsync(request.RoomId);
            if (room == null)
                return null;

            if (request.StartTime >= request.EndTime)
                throw new ArgumentException("Start time must be before end time.");

            var conflictingBookings = await _bookingRepository.GetBookingsByTimeRangeAsync(
                request.RoomId,
                request.StartTime,
                request.EndTime
            );

            if (conflictingBookings.Any())
                throw new InvalidOperationException("Room is already booked for the requested time.");

            var booking = new Booking(0, request.RoomId, request.StartTime, request.EndTime, request.SelectedServiceIds);

            var servicesPrices = request.SelectedServiceIds
                .Select(id => room.AvailableServices.FirstOrDefault(s => s.Id == id))
                .Where(s => s != null)
                .Select(s => s!.Price)
                .ToList();

            var priceResult = PricingCalculator.CalculatePrice(
                request.StartTime,
                request.EndTime,
                room.BaseHourlyRate,
                servicesPrices
            );

            booking.TotalPrice = priceResult.TotalPrice;
            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);

            return MapToResponse(createdBooking, priceResult);
        }

        public async Task<BookingResponse?> GetBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
                return null;

            var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId);
            if (room == null)
                return null;

            var servicesPrices = booking.SelectedServiceIds
                .Select(id => room.AvailableServices.FirstOrDefault(s => s.Id == id))
                .Where(s => s != null)
                .Select(s => s!.Price)
                .ToList();

            var priceResult = PricingCalculator.CalculatePrice(
                booking.StartTime,
                booking.EndTime,
                room.BaseHourlyRate,
                servicesPrices
            );

            return MapToResponse(booking, priceResult);
        }

        public async Task<List<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            var responses = new List<BookingResponse>();

            foreach (var booking in bookings)
            {
                var response = await GetBookingAsync(booking.Id);
                if (response != null)
                    responses.Add(response);
            }

            return responses;
        }

        public async Task<List<BookingResponse>> GetRoomBookingsAsync(int roomId)
        {
            var bookings = await _bookingRepository.GetBookingsByRoomIdAsync(roomId);
            var responses = new List<BookingResponse>();

            foreach (var booking in bookings)
            {
                var response = await GetBookingAsync(booking.Id);
                if (response != null)
                    responses.Add(response);
            }

            return responses;
        }

        private static BookingResponse MapToResponse(Booking booking, PricingCalculator.PriceResult priceResult)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                SelectedServiceIds = booking.SelectedServiceIds,
                TotalPrice = booking.TotalPrice,
                PriceBreakdown = new PriceBreakdown
                {
                    BaseRoomPrice = priceResult.BasePrice,
                    Discount = priceResult.Discount,
                    ServicesPrice = priceResult.ServicesPrice,
                    TotalPrice = priceResult.TotalPrice
                }
            };
        }
    }
}
