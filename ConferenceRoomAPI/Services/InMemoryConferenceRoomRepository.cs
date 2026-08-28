using ConferenceRoomAPI.Models;

namespace ConferenceRoomAPI.Services
{
    public class InMemoryConferenceRoomRepository : IConferenceRoomRepository
    {
        private readonly List<ConferenceRoom> _rooms = new();
        private int _nextId = 1;

        public InMemoryConferenceRoomRepository()
        {
            InitializeDefaultRooms();
        }

        private void InitializeDefaultRooms()
        {
            var hallA = new ConferenceRoom(1, "Зал А", 50, 2000)
            {
                AvailableServices = new List<Service>
                {
                    new Service(1, "Проєктор", 500),
                    new Service(2, "Wi-Fi", 300)
                }
            };

            var hallB = new ConferenceRoom(2, "Зал B", 100, 3500)
            {
                AvailableServices = new List<Service>
                {
                    new Service(1, "Проєктор", 500),
                    new Service(2, "Wi-Fi", 300),
                    new Service(3, "Звук", 700)
                }
            };

            var hallC = new ConferenceRoom(3, "Зал C", 30, 1500)
            {
                AvailableServices = new List<Service>
                {
                    new Service(2, "Wi-Fi", 300)
                }
            };

            _rooms.Add(hallA);
            _rooms.Add(hallB);
            _rooms.Add(hallC);
            _nextId = 4;
        }

        public Task<ConferenceRoom> AddRoomAsync(ConferenceRoom room)
        {
            room.Id = _nextId++;
            _rooms.Add(room);
            return Task.FromResult(room);
        }

        public Task<ConferenceRoom?> GetRoomByIdAsync(int id)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(room);
        }

        public Task<List<ConferenceRoom>> GetAllRoomsAsync()
        {
            return Task.FromResult(new List<ConferenceRoom>(_rooms));
        }

        public Task<bool> UpdateRoomAsync(int id, ConferenceRoom updatedRoom)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
                return Task.FromResult(false);

            if (!string.IsNullOrEmpty(updatedRoom.Name))
                room.Name = updatedRoom.Name;
            if (updatedRoom.Capacity > 0)
                room.Capacity = updatedRoom.Capacity;
            if (updatedRoom.BaseHourlyRate > 0)
                room.BaseHourlyRate = updatedRoom.BaseHourlyRate;
            if (updatedRoom.AvailableServices?.Count > 0)
                room.AvailableServices = updatedRoom.AvailableServices;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteRoomAsync(int id)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
                return Task.FromResult(false);

            _rooms.Remove(room);
            return Task.FromResult(true);
        }

        public Task<List<ConferenceRoom>> SearchAvailableRoomsAsync(DateTime startTime, DateTime endTime, int capacity)
        {
            var availableRooms = _rooms
                .Where(r => r.Capacity >= capacity)
                .ToList();

            return Task.FromResult(availableRooms);
        }
    }
}
