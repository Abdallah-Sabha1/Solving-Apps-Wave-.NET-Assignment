# AppsWave Booking Management Service

This repository contains a small booking service for shared resources such as meeting rooms and equipment. The backend uses `.NET 10 Web API`, `Entity Framework Core`, and SQLite. The frontend is a small React application that exercises the API.

## How to run

### Backend

```powershell
dotnet run --project .\AppsWave\AppsWave.csproj
```

The API creates `appswave.db` on first start. Swagger/OpenAPI is available in development.

### Frontend

```powershell
cd .\frontend
npm install
npm run dev
```

If the API uses a different HTTPS port, update the `API` constant in `frontend/src/main.jsx`.

### Tests

```powershell
dotnet test .\tests\AppsWave.Tests\AppsWave.Tests.csproj
```

## API endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/bookings` | Create a booking |
| GET | `/api/bookings/resource/{resourceId}` | List bookings for a resource; optional `from` and `to` filters |
| POST | `/api/bookings/{id}/cancel` | Cancel a booking without deleting its history |
| GET | `/api/bookings/resource/{resourceId}/availability` | Return free hourly slots for a date range |

Example request:

```json
{
  "resourceId": "room-1",
  "userId": "user-1",
  "startDateTime": "2026-09-01T10:00:00Z",
  "endDateTime": "2026-09-01T11:00:00Z"
}
```

## Decisions from the assignment

### Booking shape

The assignment requires `ResourceId`, `UserId`, `StartDateTime`, and `EndDateTime`. This implementation also stores an integer `Id`, `Status`, `CreatedAt`, and nullable `CancelledAt` for identity and lifecycle history.

Source: PDF page 4, `The shape of a booking`; page 5, `Create a booking`.

### Time and validation

The API accepts UTC timestamps and rejects missing resource or user identifiers and any period where the start is greater than or equal to the end.

Source: PDF page 4, `The shape of a booking`; page 5, `Create a booking`.

### Overlap policy

1. We compare bookings only when they use the same resource.
2. Cancelled bookings do not block a resource.
3. A booking overlaps another booking when their time periods share time.
4. A booking can start exactly when another booking ends.

The rule is implemented with a half-open time window. This makes adjacent bookings valid and avoids double-booking an actual shared interval.

Source: PDF page 5, `Prevent overlapping bookings`.

### Cancellation

Cancellation keeps the booking record and changes its status to `Cancelled`. The cancellation time is stored in `CancelledAt`. Cancelled records remain available for history but are ignored by overlap checks.

Source: PDF page 5, `Cancel a booking`.

### Extension: availability query

This solution chooses the availability-query extension. It returns free hourly slots for a resource by subtracting active bookings from a requested date range. Hourly slots keep the example small and easy to understand; a production design would accept a configurable slot size and would need stronger concurrency protection.

Source: PDF page 7, `Extension Task`, option 5.

## Testing

The unit tests cover overlapping periods, adjacent periods, different resources, and cancelled bookings. The core overlap rule is isolated in `BookingOverlapChecker` so it can be tested without a database.

## Trade-offs and next steps

- Simplicity was prioritized because this is a small internal service.
- SQLite keeps the repository easy to run from a clean checkout.
- `EnsureCreated` is used for the take-home demo; a production service should use reviewed EF Core migrations.
- The current create flow assumes a single application instance. At scale, a database constraint or transaction strategy would be needed to protect against two simultaneous requests booking the same slot.
- The next production improvements would be authentication, authorization, pagination, structured error responses, and a concurrency strategy.

## Assignment mapping

- Page 5, item 1: create a booking, implemented by `POST /api/bookings`.
- Page 5, item 2: prevent overlap, implemented by `BookingOverlapChecker`.
- Page 5, item 3: retrieve by resource and date range, implemented by `GET /api/bookings/resource/{resourceId}`.
- Page 5, item 4: cancel a booking, implemented by `POST /api/bookings/{id}/cancel`.
- Page 5, item 5: focused frontend, implemented in `frontend`.
- Page 8: source code, README, and unit tests are included.
- Page 9: verify the repository from a clean checkout before submission.
