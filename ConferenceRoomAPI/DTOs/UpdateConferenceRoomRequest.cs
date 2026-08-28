namespace ConferenceRoomAPI.DTOs
{
    public class UpdateConferenceRoomRequest
    {
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public decimal? BaseHourlyRate { get; set; }
        public List<ServiceDto>? AvailableServices { get; set; }
    }
}
