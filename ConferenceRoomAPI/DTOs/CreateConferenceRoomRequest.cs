namespace ConferenceRoomAPI.DTOs
{
    public class CreateConferenceRoomRequest
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal BaseHourlyRate { get; set; }
        public List<ServiceDto> AvailableServices { get; set; } = new();
    }

    public class ServiceDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
