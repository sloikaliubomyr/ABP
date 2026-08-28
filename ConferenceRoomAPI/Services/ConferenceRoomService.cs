using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Models;

namespace ConferenceRoomAPI.Services
{
    public class ConferenceRoomService
    {
        private readonly IConferenceRoomRepository _repository;

        public ConferenceRoomService(IConferenceRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConferenceRoomResponse> CreateRoomAsync(CreateConferenceRoomRequest request)
        {
            var room = new ConferenceRoom(0, request.Name, request.Capacity, request.BaseHourlyRate)
            {
                AvailableServices = request.AvailableServices
                    .Select((s, idx) => new Service(idx + 1, s.Name, s.Price))
                    .ToList()
            };

            var createdRoom = await _repository.AddRoomAsync(room);
            return MapToResponse(createdRoom);
        }

        public async Task<ConferenceRoomResponse?> GetRoomAsync(int id)
        {
            var room = await _repository.GetRoomByIdAsync(id);
            return room != null ? MapToResponse(room) : null;
        }

        public async Task<List<ConferenceRoomResponse>> GetAllRoomsAsync()
        {
            var rooms = await _repository.GetAllRoomsAsync();
            return rooms.Select(MapToResponse).ToList();
        }

        public async Task<bool> UpdateRoomAsync(int id, UpdateConferenceRoomRequest request)
        {
            var existingRoom = await _repository.GetRoomByIdAsync(id);
            if (existingRoom == null)
                return false;

            var updatedRoom = new ConferenceRoom
            {
                Name = request.Name ?? existingRoom.Name,
                Capacity = request.Capacity ?? existingRoom.Capacity,
                BaseHourlyRate = request.BaseHourlyRate ?? existingRoom.BaseHourlyRate,
                AvailableServices = request.AvailableServices != null
                    ? request.AvailableServices
                        .Select((s, idx) => new Service(idx + 1, s.Name, s.Price))
                        .ToList()
                    : existingRoom.AvailableServices
            };

            return await _repository.UpdateRoomAsync(id, updatedRoom);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _repository.DeleteRoomAsync(id);
        }

        public async Task<List<ConferenceRoomResponse>> SearchAvailableRoomsAsync(DateTime startTime, DateTime endTime, int capacity)
        {
            var rooms = await _repository.SearchAvailableRoomsAsync(startTime, endTime, capacity);
            return rooms.Select(MapToResponse).ToList();
        }

        private static ConferenceRoomResponse MapToResponse(ConferenceRoom room)
        {
            return new ConferenceRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                BaseHourlyRate = room.BaseHourlyRate,
                AvailableServices = room.AvailableServices
                    .Select(s => new ServiceDto { Name = s.Name, Price = s.Price })
                    .ToList()
            };
        }
    }
}
