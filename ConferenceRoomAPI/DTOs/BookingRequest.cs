namespace ConferenceRoomAPI.DTOs
{
    public class BookingRequest
    {
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new();
    }
}
