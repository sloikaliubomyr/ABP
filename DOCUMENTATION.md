# Conference Room Booking API - Technical Documentation

## Executive Summary

The Conference Room Booking API is a modern, scalable ASP.NET Core solution designed to manage conference room rentals with dynamic pricing. The application follows SOLID principles and clean code practices, ensuring maintainability and extensibility.

## Business Objectives

### Primary Goals
1. Provide a reliable API for booking conference rooms
2. Calculate accurate rental prices based on time and services
3. Enable easy room search by availability and capacity
4. Support business analytics and reporting

### Key Metrics
- Room utilization rates
- Revenue by room and time slot
- Service popularity
- Pricing strategy effectiveness

## Architecture Overview

### Layered Architecture

```
┌─────────────────────────────────────────┐
│         API Controllers                 │
│  (ConferenceRoomsController,            │
│   BookingsController)                   │
└────────────────┬────────────────────────┘
                 │
┌────────────────┴────────────────────────┐
│      Business Logic Services            │
│  (ConferenceRoomService,                │
│   BookingService)                       │
└────────────────┬────────────────────────┘
                 │
┌────────────────┴────────────────────────┐
│    Repository Pattern (Data Access)     │
│  (IConferenceRoomRepository,            │
│   IBookingRepository)                   │
└────────────────┬────────────────────────┘
                 │
┌────────────────┴────────────────────────┐
│         Data Storage                    │
│  (In-Memory | Database)                 │
└─────────────────────────────────────────┘
```

## Technical Design Decisions

### 1. Repository Pattern
**Decision**: Use Repository pattern with interfaces

**Rationale**:
- Abstracts data access logic from business logic
- Easy to swap implementations (in-memory ↔ database)
- Improves testability through mocking
- Follows DIP (Dependency Inversion Principle)

**Implementation**:
```csharp
public interface IConferenceRoomRepository
{
    Task<ConferenceRoom> AddRoomAsync(ConferenceRoom room);
    Task<ConferenceRoom?> GetRoomByIdAsync(int id);
    // ... other methods
}
```

### 2. Service Layer Pattern
**Decision**: Separate business logic into service classes

**Rationale**:
- Controllers focus solely on HTTP concerns
- Business logic is reusable and testable
- Clear separation of concerns
- Easier to maintain and extend

**Implementation**:
```csharp
public class ConferenceRoomService
{
    private readonly IConferenceRoomRepository _repository;
    
    // Business logic for room operations
}
```

### 3. Pricing Calculator
**Decision**: Dedicated utility class for price calculation

**Rationale**:
- Complex pricing logic isolated from business logic
- Easy to test independently
- Extensible for new pricing rules
- Single Responsibility Principle

**Features**:
- Time-slot detection (standard, peak, evening, early morning)
- Dynamic multiplier calculation
- Service price aggregation
- Discount/markup calculation

```csharp
public static PriceResult CalculatePrice(
    DateTime startTime,
    DateTime endTime,
    decimal baseHourlyRate,
    List<decimal> servicesPrices)
{
    // Complex pricing logic
}
```

### 4. Async/Await Pattern
**Decision**: All I/O operations use async/await

**Rationale**:
- Non-blocking operations
- Better scalability under load
- Aligns with modern ASP.NET Core best practices
- Improves overall application responsiveness

### 5. Dependency Injection
**Decision**: Use built-in ASP.NET Core DI container

**Rationale**:
- No external dependencies
- Integrated with ASP.NET Core
- Loose coupling between components
- Easier testing with mock implementations

**Configuration**:
```csharp
builder.Services.AddSingleton<IConferenceRoomRepository, InMemoryConferenceRoomRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
builder.Services.AddScoped<ConferenceRoomService>();
builder.Services.AddScoped<BookingService>();
```

## Core Business Logic

### Pricing Algorithm

#### Time-Based Multipliers
```
06:00-09:00  → 0.90x (10% discount)
09:00-12:00  → 1.00x (standard)
12:00-14:00  → 1.15x (15% markup)
14:00-18:00  → 1.00x (standard)
18:00-23:00  → 0.80x (20% discount)
```

#### Price Calculation Formula
```
Booking Price = (Base Hourly Rate × Duration × Time Multiplier) + Services Price

Example:
- Room: Hall A (2000 UAH/hour)
- Duration: 4 hours (10:00-14:00)
- Time Slot: Includes peak hours (12:00-14:00)
- Services: Projector (500 UAH) + Wi-Fi (300 UAH)
- Result: (2000 × 4 × 1.15) + 800 = 9,800 + 800 = 10,600 UAH
```

### Room Availability Logic
```csharp
public async Task<List<ConferenceRoom>> SearchAvailableRoomsAsync(
    DateTime startTime,
    DateTime endTime,
    int capacity)
{
    // Returns rooms that:
    // 1. Have sufficient capacity
    // 2. Are not booked during the requested period
    // 3. Meet service requirements
}
```

### Booking Conflict Detection
```csharp
public async Task<List<Booking>> GetBookingsByTimeRangeAsync(
    int roomId,
    DateTime startTime,
    DateTime endTime)
{
    // Detects conflicts using interval overlap logic
    // !(b.EndTime <= startTime || b.StartTime >= endTime)
}
```

## Data Models

### ConferenceRoom
```csharp
public class ConferenceRoom
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public decimal BaseHourlyRate { get; set; }
    public List<Service> AvailableServices { get; set; }
}
```

### Booking
```csharp
public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<int> SelectedServiceIds { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Service
```csharp
public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

## API Response Formats

### Success Response (Booking)
```json
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

### Error Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "traceId": "0HN1GFRQQ40FM:00000001"
}
```

## Security Considerations

### Input Validation
- Required field validation
- Range checks (capacity > 0, price > 0)
- DateTime validation (start < end)
- SQL injection prevention via parameterized queries (when DB is used)

### Error Handling
- No sensitive data in error messages
- Proper HTTP status codes
- Meaningful error descriptions for clients
- Logging of security-related events

### CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
```

## Scalability Strategy

### Horizontal Scaling
- Stateless design enables multiple instances
- In-memory storage is suitable for dev/test
- Database implementation supports clustering

### Performance Optimization
- Async operations prevent thread pool exhaustion
- Entity framework query optimization (when DB is used)
- Caching strategy can be added (Redis)
- Pagination for large result sets

### Database Migration Path
```
Current: In-Memory Storage
↓
Future: SQL Server/PostgreSQL
- Replace InMemoryConferenceRoomRepository with DbContext
- Add Entity Framework Core
- Implement migrations
- Add connection pooling
```

## Testing Strategy

### Unit Tests
- Service layer testing with mock repositories
- Price calculation validation
- Booking conflict detection

### Integration Tests
- End-to-end API tests
- Repository implementations
- Database integration (when applicable)

### Test Coverage Areas
1. Room CRUD operations
2. Booking creation and validation
3. Price calculation accuracy
4. Time-slot detection
5. Conflict detection
6. Edge cases (midnight bookings, DST, etc.)

## Monitoring and Analytics

### Metrics to Track
```
1. Room Utilization
   - Occupancy rate per room
   - Peak hours utilization
   - Revenue per room

2. Booking Analytics
   - Bookings per day
   - Average booking duration
   - Service popularity

3. Financial Metrics
   - Total revenue
   - Revenue by time slot
   - Discount impact
   - Service contribution

4. Performance Metrics
   - API response times
   - Booking creation time
   - Search query performance
```

### Logging Recommendations
```csharp
// Log booking creation
_logger.LogInformation(
    "Booking created: RoomId={roomId}, StartTime={startTime}, Price={price}",
    booking.RoomId, booking.StartTime, booking.TotalPrice);

// Log pricing calculation
_logger.LogDebug(
    "Price calculated: Base={base}, Discount={discount}, Services={services}",
    priceResult.BasePrice, priceResult.Discount, priceResult.ServicesPrice);
```

## Deployment Considerations

### Environment Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish .
ENTRYPOINT ["dotnet", "ConferenceRoomAPI.dll"]
```

## Maintenance and Updates

### Code Review Checklist
- [ ] Follows SOLID principles
- [ ] No code duplication (DRY)
- [ ] Meaningful variable/method names
- [ ] Error handling present
- [ ] Security validation implemented
- [ ] Async/await used for I/O

### Version Management
- Semantic versioning (MAJOR.MINOR.PATCH)
- API versioning strategy
- Breaking change management
- Deprecation policy

## Known Limitations

1. **In-Memory Storage**: Data lost on application restart
2. **Single-Threaded Pricing**: Complex multi-room calculations may need optimization
3. **No Authentication**: Public API endpoints
4. **No Audit Trail**: Booking modifications not tracked
5. **No Rate Limiting**: No protection against abuse

## Future Roadmap

### Phase 1 (v1.1)
- Add unit tests
- Implement logging
- Add rate limiting

### Phase 2 (v1.2)
- Database integration (EF Core)
- User authentication (JWT)
- Booking modification/cancellation

### Phase 3 (v2.0)
- Payment integration
- Multi-tenancy support
- Advanced reporting
- Email notifications

## References

- [Clean Code - Robert C. Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Dependency Injection in .NET](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)

---
**Document Version**: 1.0.0
**Last Updated**: 2026-08-28
**Author**: ABP Company Development Team
