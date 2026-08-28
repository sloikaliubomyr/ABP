using ConferenceRoomAPI.DTOs;
using ConferenceRoomAPI.Services;
using Xunit;

namespace ConferenceRoomAPI.Tests
{
    public class ConferenceRoomServiceTests
    {
        private readonly ConferenceRoomService _service;
        private readonly IConferenceRoomRepository _repository;

        public ConferenceRoomServiceTests()
        {
            _repository = new InMemoryConferenceRoomRepository();
            _service = new ConferenceRoomService(_repository);
        }

        [Fact]
        public async Task CreateRoom_ValidRequest_ReturnsRoomWithId()
        {
            var request = new CreateConferenceRoomRequest
            {
                Name = "New Meeting Room",
                Capacity = 30,
                BaseHourlyRate = 1500,
                AvailableServices = new List<ServiceDto>
                {
                    new ServiceDto { Name = "Projector", Price = 500 }
                }
            };

            var result = await _service.CreateRoomAsync(request);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Equal("New Meeting Room", result.Name);
            Assert.Equal(30, result.Capacity);
        }

        [Fact]
        public async Task GetRoom_ExistingRoom_ReturnsRoom()
        {
            var result = await _service.GetRoomAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetRoom_NonExistingRoom_ReturnsNull()
        {
            var result = await _service.GetRoomAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllRooms_ReturnsAllRooms()
        {
            var result = await _service.GetAllRoomsAsync();

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task UpdateRoom_ValidData_ReturnsTrue()
        {
            var request = new UpdateConferenceRoomRequest
            {
                Name = "Updated Room",
                BaseHourlyRate = 2500
            };

            var result = await _service.UpdateRoomAsync(1, request);

            Assert.True(result);
            var room = await _service.GetRoomAsync(1);
            Assert.Equal("Updated Room", room.Name);
            Assert.Equal(2500, room.BaseHourlyRate);
        }

        [Fact]
        public async Task UpdateRoom_NonExistingRoom_ReturnsFalse()
        {
            var request = new UpdateConferenceRoomRequest
            {
                Name = "Updated Room"
            };

            var result = await _service.UpdateRoomAsync(999, request);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_ReturnsTrue()
        {
            var createRequest = new CreateConferenceRoomRequest
            {
                Name = "Room to Delete",
                Capacity = 20,
                BaseHourlyRate = 1000,
                AvailableServices = new List<ServiceDto>()
            };

            var created = await _service.CreateRoomAsync(createRequest);
            var result = await _service.DeleteRoomAsync(created.Id);

            Assert.True(result);
            var deleted = await _service.GetRoomAsync(created.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteRoom_NonExistingRoom_ReturnsFalse()
        {
            var result = await _service.DeleteRoomAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task SearchAvailableRooms_SufficientCapacity_ReturnsRooms()
        {
            var result = await _service.SearchAvailableRoomsAsync(
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 14, 0, 0),
                50);

            Assert.NotEmpty(result);
            Assert.All(result, r => Assert.True(r.Capacity >= 50));
        }

        [Fact]
        public async Task SearchAvailableRooms_InsufficientCapacity_ReturnsEmpty()
        {
            var result = await _service.SearchAvailableRoomsAsync(
                new DateTime(2024, 9, 1, 10, 0, 0),
                new DateTime(2024, 9, 1, 14, 0, 0),
                200);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateRoom_WithMultipleServices_IncludesAllServices()
        {
            var request = new CreateConferenceRoomRequest
            {
                Name = "Luxury Room",
                Capacity = 100,
                BaseHourlyRate = 5000,
                AvailableServices = new List<ServiceDto>
                {
                    new ServiceDto { Name = "Projector", Price = 500 },
                    new ServiceDto { Name = "Sound System", Price = 1000 },
                    new ServiceDto { Name = "Catering", Price = 2000 }
                }
            };

            var result = await _service.CreateRoomAsync(request);

            Assert.NotEmpty(result.AvailableServices);
            Assert.Equal(3, result.AvailableServices.Count);
        }

        [Fact]
        public async Task GetAllRooms_InitialRooms_HaveCorrectNames()
        {
            var rooms = await _service.GetAllRoomsAsync();

            Assert.Contains(rooms, r => r.Name == "Зал А");
            Assert.Contains(rooms, r => r.Name == "Зал B");
            Assert.Contains(rooms, r => r.Name == "Зал C");
        }

        [Fact]
        public async Task UpdateRoom_PartialUpdate_PreservesOtherData()
        {
            var originalRoom = await _service.GetRoomAsync(1);
            var updateRequest = new UpdateConferenceRoomRequest
            {
                BaseHourlyRate = 2500
            };

            await _service.UpdateRoomAsync(1, updateRequest);
            var updatedRoom = await _service.GetRoomAsync(1);

            Assert.Equal(originalRoom.Capacity, updatedRoom.Capacity);
            Assert.Equal(2500, updatedRoom.BaseHourlyRate);
        }
    }
}
