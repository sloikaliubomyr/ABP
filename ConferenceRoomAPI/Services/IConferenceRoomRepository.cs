using ConferenceRoomAPI.Models;

namespace ConferenceRoomAPI.Services
{
    public interface IConferenceRoomRepository
    {
        Task<ConferenceRoom> AddRoomAsync(ConferenceRoom room);
        Task<ConferenceRoom?> GetRoomByIdAsync(int id);
        Task<List<ConferenceRoom>> GetAllRoomsAsync();
        Task<bool> UpdateRoomAsync(int id, ConferenceRoom room);
        Task<bool> DeleteRoomAsync(int id);
        Task<List<ConferenceRoom>> SearchAvailableRoomsAsync(DateTime startTime, DateTime endTime, int capacity);
    }
}
