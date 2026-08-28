# Conference Room Booking API

A professional ASP.NET Core 8.0 Web API for managing conference room bookings and rental calculations with dynamic pricing based on time slots and additional services.

## Project Overview

This API enables businesses to manage conference room rentals with the following capabilities:
- **Room Management**: Create, read, update, and delete conference rooms with customizable services
- **Availability Search**: Find available rooms based on date, time, and capacity requirements
- **Smart Booking**: Reserve rooms with dynamic pricing calculation based on time of day
- **Price Analytics**: Detailed price breakdowns including services and time-based discounts/markups

## Business Requirements

### Rooms
- Hall A: 50 people, 2,000 UAH/hour
- Hall B: 100 people, 3,500 UAH/hour
- Hall C: 30 people, 1,500 UAH/hour

### Services
- Projector: 500 UAH
- Wi-Fi: 300 UAH
- Sound: 700 UAH

### Dynamic Pricing Rules
- **Standard hours (09:00-18:00)**: Base rate
- **Peak hours (12:00-14:00)**: 15% markup
- **Evening hours (18:00-23:00)**: 20% discount
- **Early morning (06:00-09:00)**: 10% discount

## Technology Stack

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **C# 12** - Modern language features
- **Swagger/OpenAPI** - API documentation
- **In-Memory Storage** - For demo (easily replaceable with database)

## Project Structure

```
ConferenceRoomAPI/
├── Controllers/
│   ├── ConferenceRoomsController.cs    # Room management endpoints
│   └── BookingsController.cs            # Booking management endpoints
├── Models/
│   ├── ConferenceRoom.cs               # Room domain model
│   ├── Booking.cs                      # Booking domain model
│   └── Service.cs                      # Service domain model
├── Services/
│   ├── IConferenceRoomRepository.cs    # Room repository interface
│   ├── IBookingRepository.cs           # Booking repository interface
│   ├── InMemoryConferenceRoomRepository.cs  # Room repository implementation
│   ├── InMemoryBookingRepository.cs    # Booking repository implementation
│   ├── ConferenceRoomService.cs        # Room business logic
│   └── BookingService.cs               # Booking business logic
├── DTOs/
│   ├── CreateConferenceRoomRequest.cs  # Create room DTO
│   ├── UpdateConferenceRoomRequest.cs  # Update room DTO
│   ├── BookingRequest.cs               # Booking request DTO
│   ├── BookingResponse.cs              # Booking response DTO
│   └── ConferenceRoomResponse.cs       # Room response DTO
├── Utilities/
│   └── PricingCalculator.cs            # Pricing calculation logic
└── Program.cs                          # Application setup

```

## API Endpoints

### Conference Rooms

#### Create Room
```
POST /api/conferencerooms
Content-Type: application/json

{
  "name": "Meeting Room A",
  "capacity": 50,
  "baseHourlyRate": 2000,
  "availableServices": [
    { "name": "Projector", "price": 500 },
    { "name": "Wi-Fi", "price": 300 }
  ]
}

Response: 201 Created
{
  "id": 1,
  "name": "Meeting Room A",
  "capacity": 50,
  "baseHourlyRate": 2000,
  "availableServices": [...]
}
```

#### Get Room by ID
```
GET /api/conferencerooms/{id}

Response: 200 OK
{
  "id": 1,
  "name": "Meeting Room A",
  "capacity": 50,
  "baseHourlyRate": 2000,
  "availableServices": [...]
}
```

#### Get All Rooms
```
GET /api/conferencerooms

Response: 200 OK
[{...}, {...}, ...]
```

#### Search Available Rooms
```
GET /api/conferencerooms/search?startTime=2024-09-01T10:00:00&endTime=2024-09-01T14:00:00&capacity=50

Response: 200 OK
[{...}, {...}, ...]
```

#### Update Room
```
PUT /api/conferencerooms/{id}
Content-Type: application/json

{
  "name": "Updated Room Name",
  "baseHourlyRate": 2500
}

Response: 204 No Content
```

#### Delete Room
```
DELETE /api/conferencerooms/{id}

Response: 204 No Content
```

### Bookings

#### Create Booking
```
POST /api/bookings
Content-Type: application/json

{
  "roomId": 1,
  "startTime": "2024-09-01T10:00:00",
  "endTime": "2024-09-01T14:00:00",
  "selectedServiceIds": [1, 2]
}

Response: 201 Created
{
  "id": 1,
  "roomId": 1,
  "startTime": "2024-09-01T10:00:00",
  "endTime": "2024-09-01T14:00:00",
  "selectedServiceIds": [1, 2],
  "totalPrice": 9300,
  "priceBreakdown": {
    "baseRoomPrice": 8600,
    "discount": 400,
    "servicesPrice": 800,
    "totalPrice": 9300
  }
}
```

#### Get Booking by ID
```
GET /api/bookings/{id}

Response: 200 OK
{...}
```

#### Get All Bookings
```
GET /api/bookings

Response: 200 OK
[{...}, {...}, ...]
```

#### Get Room Bookings
```
GET /api/bookings/room/{roomId}

Response: 200 OK
[{...}, {...}, ...]
```

## Key Features

### Clean Code Practices
- **Separation of Concerns**: Clear separation between repositories, services, and controllers
- **Dependency Injection**: Loosely coupled components using DI container
- **SOLID Principles**: 
  - Single Responsibility: Each class has one reason to change
  - Open/Closed: Services are open for extension, closed for modification
  - Dependency Inversion: Depend on abstractions, not concrete implementations
- **Meaningful Names**: Clear, descriptive naming conventions throughout
- **Error Handling**: Comprehensive validation and meaningful error responses

### Scalability
- **Repository Pattern**: Easy to switch from in-memory to database storage
- **Async/Await**: Non-blocking operations for better performance
- **Extensible Pricing**: PricingCalculator can be easily extended with new rules
- **Modular Architecture**: Components can be developed and tested independently

### API Quality
- **Swagger Documentation**: Auto-generated interactive API documentation
- **HTTP Status Codes**: Proper status codes for different scenarios (201, 204, 400, 404, 409)
- **Request Validation**: Input validation at controller level
- **CORS Support**: Cross-origin requests enabled

## Running the Application

### Prerequisites
- .NET 8.0 SDK or later

### Setup
```bash
# Navigate to project directory
cd ConferenceRoomAPI

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

### Access API
- **API Base URL**: `https://localhost:5001/api`
- **Swagger UI**: `https://localhost:5001/`
- **Swagger JSON**: `https://localhost:5001/swagger/v1/swagger.json`

## Development

### Testing Endpoints
Use the included REST client file (`ConferenceRoomAPI.http`) with VS Code REST Client extension, or use Postman/curl:

```bash
# Create a room
curl -X POST https://localhost:5001/api/conferencerooms \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Room","capacity":50,"baseHourlyRate":2000,"availableServices":[]}'

# Get all rooms
curl https://localhost:5001/api/conferencerooms

# Create booking
curl -X POST https://localhost:5001/api/bookings \
  -H "Content-Type: application/json" \
  -d '{"roomId":1,"startTime":"2024-09-01T10:00:00","endTime":"2024-09-01T14:00:00","selectedServiceIds":[1]}'
```

### Project Configuration
- **Language Version**: C# 12
- **Target Framework**: .NET 8.0
- **Nullable Reference Types**: Enabled
- **API Versioning**: v1

## Future Enhancements

1. **Database Integration**: Replace in-memory storage with SQL Server/PostgreSQL
2. **User Authentication**: Add JWT-based authentication
3. **Payment Integration**: Process payments for bookings
4. **Email Notifications**: Send confirmation emails
5. **Analytics Dashboard**: Business intelligence and reporting
6. **Rate Limiting**: Implement rate limiting for API protection
7. **Logging**: Structured logging with Serilog
8. **Unit Tests**: Comprehensive test coverage
9. **Caching**: Redis caching for performance
10. **Multi-tenancy**: Support multiple organizations

## Security Considerations

- Input validation on all endpoints
- CORS configured for controlled access
- No sensitive data logged
- SQL injection prevention (repository pattern)
- XSS prevention via JSON serialization
- HTTPS enforcement

## Error Handling

The API returns meaningful error responses:

- **400 Bad Request**: Invalid input data
- **404 Not Found**: Resource not found
- **409 Conflict**: Room already booked for requested time
- **500 Internal Server Error**: Server-side errors

Example error response:
```json
{
  "error": "Room is already booked for the requested time."
}
```

## Version
1.0.0

## License
Commercial - All rights reserved by ABP Company
