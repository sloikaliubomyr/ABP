namespace ConferenceRoomAPI.DTOs
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<int> SelectedServiceIds { get; set; }
        public decimal TotalPrice { get; set; }
        public PriceBreakdown PriceBreakdown { get; set; }
    }

    public class PriceBreakdown
    {
        public decimal BaseRoomPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ServicesPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
