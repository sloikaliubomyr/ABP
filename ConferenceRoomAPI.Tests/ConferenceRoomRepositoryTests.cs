using ConferenceRoomAPI.Models;
using ConferenceRoomAPI.Services;
using Xunit;

namespace ConferenceRoomAPI.Tests
{
    public class ConferenceRoomRepositoryTests
    {
        private readonly IConferenceRoomRepository _repository = new InMemoryConferenceRoomRepository();

        [Fact]
        public async Task AddRoom_ValidRoom_ReturnsRoomWithId()
        {
            var room = new ConferenceRoom(0, "Test Room", 50, 2000);

            var result = await _repository.AddRoomAsync(room);

            Assert.NotEqual(0, result.Id);
            Assert.Equal("Test Room", result.Name);
        }

        [Fact]
        public async Task GetRoomById_ExistingRoom_ReturnsRoom()
        {
            var room = await _repository.GetRoomByIdAsync(1);

            Assert.NotNull(room);
            Assert.Equal(1, room.Id);
        }

        [Fact]
        public async Task GetRoomById_NonExistingRoom_ReturnsNull()
        {
            var room = await _repository.GetRoomByIdAsync(999);

            Assert.Null(room);
        }

        [Fact]
        public async Task GetAllRooms_ReturnsAllRooms()
        {
            var rooms = await _repository.GetAllRoomsAsync();

            Assert.NotEmpty(rooms);
            Assert.Equal(3, rooms.Count);
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_ReturnsTrue()
        {
            var newRoom = new ConferenceRoom(0, "Room to Delete", 20, 1000);
            var added = await _repository.AddRoomAsync(newRoom);

            var result = await _repository.DeleteRoomAsync(added.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRoom_NonExistingRoom_ReturnsFalse()
        {
            var result = await _repository.DeleteRoomAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateRoom_ValidData_ReturnsTrue()
        {
            var updatedRoom = new ConferenceRoom { Name = "Updated Room", BaseHourlyRate = 3000 };

            var result = await _repository.UpdateRoomAsync(1, updatedRoom);

            Assert.True(result);
            var room = await _repository.GetRoomByIdAsync(1);
            Assert.Equal("Updated Room", room.Name);
            Assert.Equal(3000, room.BaseHourlyRate);
        }

        [Fact]
        public async Task UpdateRoom_NonExistingRoom_ReturnsFalse()
        {
            var updatedRoom = new ConferenceRoom { Name = "Updated Room" };

            var result = await _repository.UpdateRoomAsync(999, updatedRoom);

            Assert.False(result);
        }

        [Fact]
        public async Task SearchAvailableRooms_SufficientCapacity_ReturnsMatchingRooms()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 14, 0, 0);
            var capacity = 50;

            var rooms = await _repository.SearchAvailableRoomsAsync(startTime, endTime, capacity);

            Assert.NotEmpty(rooms);
            Assert.All(rooms, r => Assert.True(r.Capacity >= capacity));
        }

        [Fact]
        public async Task SearchAvailableRooms_InsufficientCapacity_ReturnsEmptyList()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 14, 0, 0);
            var capacity = 200;

            var rooms = await _repository.SearchAvailableRoomsAsync(startTime, endTime, capacity);

            Assert.Empty(rooms);
        }

        [Fact]
        public async Task SearchAvailableRooms_ExactCapacity_ReturnsRoom()
        {
            var startTime = new DateTime(2024, 9, 1, 10, 0, 0);
            var endTime = new DateTime(2024, 9, 1, 14, 0, 0);
            var capacity = 100;

            var rooms = await _repository.SearchAvailableRoomsAsync(startTime, endTime, capacity);

            Assert.Contains(rooms, r => r.Id == 2 && r.Capacity == 100);
        }

        [Fact]
        public async Task GetAllRooms_InitialRooms_HaveCorrectProperties()
        {
            var rooms = await _repository.GetAllRoomsAsync();

            var hallA = rooms.FirstOrDefault(r => r.Id == 1);
            Assert.NotNull(hallA);
            Assert.Equal("Зал А", hallA.Name);
            Assert.Equal(50, hallA.Capacity);
            Assert.Equal(2000, hallA.BaseHourlyRate);
        }
    }
}
