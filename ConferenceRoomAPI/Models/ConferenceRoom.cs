namespace ConferenceRoomAPI.Models
{
    public class ConferenceRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal BaseHourlyRate { get; set; }
        public List<Service> AvailableServices { get; set; } = new();

        public ConferenceRoom() { }

        public ConferenceRoom(int id, string name, int capacity, decimal baseHourlyRate)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            BaseHourlyRate = baseHourlyRate;
        }
    }
}
