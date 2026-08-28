# Conference Room Booking API - Usage Examples

## Quick Start Guide

### 1. Running the API

```bash
cd ConferenceRoomAPI
dotnet run
```

The API will be available at:
- **Base URL**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger/index.html`

### 2. Initial Rooms

The API comes pre-loaded with three conference rooms:

| ID | Name | Capacity | Base Rate | Services |
|----|------|----------|-----------|----------|
| 1 | Зал А | 50 | 2000 UAH/h | Projector (500), Wi-Fi (300) |
| 2 | Зал B | 100 | 3500 UAH/h | Projector (500), Wi-Fi (300), Sound (700) |
| 3 | Зал C | 30 | 1500 UAH/h | Wi-Fi (300) |

---

## Examples with cURL

### Conference Rooms Management

#### 1. Create a New Conference Room

```bash
curl -X POST "https://localhost:5001/api/conferencerooms" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Executive Suite",
    "capacity": 20,
    "baseHourlyRate": 3000,
    "availableServices": [
      {
        "name": "Projector",
        "price": 500
      },
      {
        "name": "Wi-Fi",
        "price": 300
      },
      {
        "name": "Catering",
        "price": 1000
      }
    ]
  }'
```

**Expected Response (201 Created)**:
```json
{
  "id": 4,
  "name": "Executive Suite",
  "capacity": 20,
  "baseHourlyRate": 3000,
  "availableServices": [
    {
      "name": "Projector",
      "price": 500
    },
    {
      "name": "Wi-Fi",
      "price": 300
    },
    {
      "name": "Catering",
      "price": 1000
    }
  ]
}
```

#### 2. Get Room by ID

```bash
curl "https://localhost:5001/api/conferencerooms/1"
```

**Expected Response (200 OK)**:
```json
{
  "id": 1,
  "name": "Зал А",
  "capacity": 50,
  "baseHourlyRate": 2000,
  "availableServices": [
    {
      "name": "Проєктор",
      "price": 500
    },
    {
      "name": "Wi-Fi",
      "price": 300
    }
  ]
}
```

#### 3. Get All Rooms

```bash
curl "https://localhost:5001/api/conferencerooms"
```

**Expected Response (200 OK)**:
```json
[
  {
    "id": 1,
    "name": "Зал А",
    "capacity": 50,
    "baseHourlyRate": 2000,
    "availableServices": [...]
  },
  {
    "id": 2,
    "name": "Зал B",
    "capacity": 100,
    "baseHourlyRate": 3500,
    "availableServices": [...]
  },
  {
    "id": 3,
    "name": "Зал C",
    "capacity": 30,
    "baseHourlyRate": 1500,
    "availableServices": [...]
  }
]
```

#### 4. Search Available Rooms

Search for rooms available on 2024-09-01 from 10:00 to 14:00 with capacity for 50 people:

```bash
curl "https://localhost:5001/api/conferencerooms/search?startTime=2024-09-01T10:00:00&endTime=2024-09-01T14:00:00&capacity=50"
```

**Expected Response (200 OK)**:
```json
[
  {
    "id": 1,
    "name": "Зал А",
    "capacity": 50,
    "baseHourlyRate": 2000,
    "availableServices": [...]
  },
  {
    "id": 2,
    "name": "Зал B",
    "capacity": 100,
    "baseHourlyRate": 3500,
    "availableServices": [...]
  }
]
```

#### 5. Update Room Information

Update Hall A's base hourly rate to 2500:

```bash
curl -X PUT "https://localhost:5001/api/conferencerooms/1" \
  -H "Content-Type: application/json" \
  -d '{
    "baseHourlyRate": 2500
  }'
```

**Expected Response (204 No Content)**

#### 6. Delete a Room

```bash
curl -X DELETE "https://localhost:5001/api/conferencerooms/4"
```

**Expected Response (204 No Content)**

---

### Booking Management

#### 1. Create a Booking (Standard Hours)

Book Hall A for standard hours (10:00-14:00) with Projector and Wi-Fi:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "startTime": "2024-09-01T10:00:00",
    "endTime": "2024-09-01T14:00:00",
    "selectedServiceIds": [1, 2]
  }'
```

**Expected Response (201 Created)**:
```json
{
  "id": 1,
  "roomId": 1,
  "startTime": "2024-09-01T10:00:00",
  "endTime": "2024-09-01T14:00:00",
  "selectedServiceIds": [1, 2],
  "totalPrice": 9300,
  "priceBreakdown": {
    "baseRoomPrice": 8000,
    "discount": 0,
    "servicesPrice": 800,
    "totalPrice": 9300
  }
}
```

**Price Calculation Breakdown**:
- Duration: 4 hours
- Base Rate: 2000 UAH/hour
- Time Slot: Standard (09:00-18:00) → 1.0x multiplier
- Base Price: 2000 × 4 × 1.0 = 8000 UAH
- Services: Projector (500) + Wi-Fi (300) = 800 UAH
- **Total: 8800 UAH**

#### 2. Create a Booking (Peak Hours with Markup)

Book Hall B for peak hours (12:00-14:00) with 15% markup:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 2,
    "startTime": "2024-09-01T12:00:00",
    "endTime": "2024-09-01T14:00:00",
    "selectedServiceIds": [1, 2, 3]
  }'
```

**Expected Response (201 Created)**:
```json
{
  "id": 2,
  "roomId": 2,
  "startTime": "2024-09-01T12:00:00",
  "endTime": "2024-09-01T14:00:00",
  "selectedServiceIds": [1, 2, 3],
  "totalPrice": 9305,
  "priceBreakdown": {
    "baseRoomPrice": 8050,
    "discount": -700,
    "servicesPrice": 1500,
    "totalPrice": 9550
  }
}
```

**Price Calculation Breakdown**:
- Duration: 2 hours
- Base Rate: 3500 UAH/hour
- Time Slot: Peak (12:00-14:00) → 1.15x multiplier
- Base Price: 3500 × 2 × 1.15 = 8050 UAH
- Services: Projector (500) + Wi-Fi (300) + Sound (700) = 1500 UAH
- **Total: 9550 UAH**

#### 3. Create a Booking (Evening Hours with Discount)

Book Hall C for evening hours (18:00-22:00) with 20% discount:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 3,
    "startTime": "2024-09-01T18:00:00",
    "endTime": "2024-09-01T22:00:00",
    "selectedServiceIds": [2]
  }'
```

**Expected Response (201 Created)**:
```json
{
  "id": 3,
  "roomId": 3,
  "startTime": "2024-09-01T18:00:00",
  "endTime": "2024-09-01T22:00:00",
  "selectedServiceIds": [2],
  "totalPrice": 4900,
  "priceBreakdown": {
    "baseRoomPrice": 4800,
    "discount": 1200,
    "servicesPrice": 300,
    "totalPrice": 5100
  }
}
```

**Price Calculation Breakdown**:
- Duration: 4 hours
- Base Rate: 1500 UAH/hour
- Time Slot: Evening (18:00-23:00) → 0.8x multiplier
- Base Price: 1500 × 4 × 0.8 = 4800 UAH
- Services: Wi-Fi (300) = 300 UAH
- Discount Applied: 1500 × 4 × 0.2 = 1200 UAH
- **Total: 5100 UAH**

#### 4. Create a Booking (Early Morning with Discount)

Book Hall A for early morning (07:00-09:00) with 10% discount:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "startTime": "2024-09-01T07:00:00",
    "endTime": "2024-09-01T09:00:00",
    "selectedServiceIds": []
  }'
```

**Expected Response (201 Created)**:
```json
{
  "id": 4,
  "roomId": 1,
  "startTime": "2024-09-01T07:00:00",
  "endTime": "2024-09-01T09:00:00",
  "selectedServiceIds": [],
  "totalPrice": 3600,
  "priceBreakdown": {
    "baseRoomPrice": 3600,
    "discount": 400,
    "servicesPrice": 0,
    "totalPrice": 3600
  }
}
```

**Price Calculation Breakdown**:
- Duration: 2 hours
- Base Rate: 2000 UAH/hour
- Time Slot: Early Morning (06:00-09:00) → 0.9x multiplier
- Base Price: 2000 × 2 × 0.9 = 3600 UAH
- Discount Applied: 2000 × 2 × 0.1 = 400 UAH
- **Total: 3600 UAH**

#### 5. Get Booking by ID

```bash
curl "https://localhost:5001/api/bookings/1"
```

**Expected Response (200 OK)**:
```json
{
  "id": 1,
  "roomId": 1,
  "startTime": "2024-09-01T10:00:00",
  "endTime": "2024-09-01T14:00:00",
  "selectedServiceIds": [1, 2],
  "totalPrice": 9300,
  "priceBreakdown": {
    "baseRoomPrice": 8000,
    "discount": 0,
    "servicesPrice": 800,
    "totalPrice": 9300
  }
}
```

#### 6. Get All Bookings

```bash
curl "https://localhost:5001/api/bookings"
```

**Expected Response (200 OK)**:
```json
[
  {
    "id": 1,
    "roomId": 1,
    ...
  },
  {
    "id": 2,
    "roomId": 2,
    ...
  }
]
```

#### 7. Get Bookings for Specific Room

Get all bookings for Hall A (Room ID 1):

```bash
curl "https://localhost:5001/api/bookings/room/1"
```

**Expected Response (200 OK)**:
```json
[
  {
    "id": 1,
    "roomId": 1,
    ...
  }
]
```

---

## Error Cases

### 1. Conflict: Room Already Booked

Try to book a room during an already booked time:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "startTime": "2024-09-01T10:00:00",
    "endTime": "2024-09-01T12:00:00",
    "selectedServiceIds": []
  }'
```

**Expected Response (409 Conflict)**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.9",
  "title": "Conflict",
  "status": 409,
  "detail": "Room is already booked for the requested time."
}
```

### 2. Bad Request: Invalid Input

Try to create booking with invalid time range:

```bash
curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "startTime": "2024-09-01T14:00:00",
    "endTime": "2024-09-01T10:00:00",
    "selectedServiceIds": []
  }'
```

**Expected Response (400 Bad Request)**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Start time must be before end time."
}
```

### 3. Not Found: Room Not Found

```bash
curl "https://localhost:5001/api/conferencerooms/999"
```

**Expected Response (404 Not Found)**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Room with ID 999 not found."
}
```

---

## Pricing Examples Summary

### Time Slots Reference
| Time Range | Multiplier | Effect |
|-----------|-----------|---------|
| 06:00-09:00 | 0.90x | 10% discount |
| 09:00-12:00 | 1.00x | Standard rate |
| 12:00-14:00 | 1.15x | 15% markup |
| 14:00-18:00 | 1.00x | Standard rate |
| 18:00-23:00 | 0.80x | 20% discount |

### Practical Scenarios

**Scenario 1: Morning Meeting**
```
Room: Hall A (2000/hour)
Time: 09:00-11:00 (2 hours)
Services: Wi-Fi
Price: (2000 × 2 × 1.0) + 300 = 4300 UAH
```

**Scenario 2: Lunch Presentation**
```
Room: Hall B (3500/hour)
Time: 12:00-13:30 (1.5 hours)
Services: Projector + Sound
Price: (3500 × 1.5 × 1.15) + 1200 = 7,237.50 UAH
```

**Scenario 3: Evening Training**
```
Room: Hall C (1500/hour)
Time: 18:00-21:00 (3 hours)
Services: Wi-Fi + Projector (no projector available)
Price: (1500 × 3 × 0.8) + 300 = 3900 UAH
```

---

## Integration Testing Checklist

- [ ] Create room successfully
- [ ] Retrieve room by ID
- [ ] List all rooms
- [ ] Search available rooms with various criteria
- [ ] Update room details
- [ ] Delete room
- [ ] Book room with different time slots
- [ ] Verify pricing calculations
- [ ] Test booking conflicts
- [ ] Handle invalid input gracefully
- [ ] Verify all HTTP status codes
- [ ] Test with edge cases (midnight, DST, etc.)

---

## Notes

- All timestamps should be in UTC (ISO 8601 format)
- Prices are in Ukrainian Hryvnias (UAH)
- Duration is calculated in full hours (rounded up)
- Services are optional during booking
- Rooms with insufficient capacity are filtered out during search
- Overlapping bookings are detected and rejected

