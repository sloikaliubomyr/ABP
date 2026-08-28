namespace ConferenceRoomAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Booking() { }

        public Booking(int id, int roomId, DateTime startTime, DateTime endTime, List<int> serviceIds)
        {
            Id = id;
            RoomId = roomId;
            StartTime = startTime;
            EndTime = endTime;
            SelectedServiceIds = serviceIds;
        }

        public int GetDurationInHours()
        {
            return (int)Math.Ceiling((EndTime - StartTime).TotalHours);
        }
    }
}
