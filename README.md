# AppsWave Booking Management Service

This project solves a simple booking problem: users need to reserve shared resources such as meeting rooms without two active bookings using the same resource at the same time.

The solution contains a `.NET 10 Web API`, a small React frontend, and xUnit tests. SQLite is used so the project can run without setting up a separate database server.

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

The frontend connects to `http://localhost:5295` by default. If the API uses a different address, set the `VITE_API_URL` environment variable.

### Tests

```powershell
dotnet test .\AppsWave.Tests\AppsWave.Tests.csproj
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

## What was implemented

### Booking shape

The required booking data is `ResourceId`, `UserId`, `StartDateTime`, and `EndDateTime`. I also added an integer `Id`, `Status`, `CreatedAt`, and nullable `CancelledAt` so each booking has an identity and a clear lifecycle.

This comes from page 4, “The shape of a booking”, and page 5, “Create a booking”.

### Time and validation

The API accepts UTC timestamps. It rejects missing resource or user identifiers and rejects a period when the start is greater than or equal to the end.

This follows page 4 and page 5 of the assignment.

### Overlap policy

1. Bookings for different resources do not conflict.
2. Cancelled bookings do not block a resource.
3. Two bookings conflict when their time periods share actual time.
4. A booking may start exactly when another booking ends.

The rule is implemented with a half-open time window. This makes adjacent bookings valid and avoids double-booking an actual shared interval.

This is the overlap requirement from page 5, “Prevent overlapping bookings”.

### Cancellation

Cancellation keeps the booking record and changes its status to `Cancelled`. The cancellation time is stored in `CancelledAt`. Cancelled records remain available for history but are ignored by overlap checks.

This follows page 5, “Cancel a booking”.

### Extension: availability query

This solution chooses the availability-query extension. It returns free hourly slots for a resource by subtracting active bookings from a requested date range. Hourly slots keep the example small and easy to understand; a production design would accept a configurable slot size and would need stronger concurrency protection.

This is the selected extension from page 7, “Extension Task”, option 5.

## Testing

The unit tests cover overlapping periods, adjacent periods, different resources, and cancelled bookings. The overlap rule is kept in a separate service so it can be tested without a database.

## Notes and trade-offs

- Simplicity was prioritized because this is a small internal service.
- SQLite keeps the repository easy to run from a clean checkout.
- `EnsureCreated` is used for the take-home demo; a production service should use reviewed EF Core migrations.
- The current create flow assumes a single application instance. At scale, a database constraint or transaction strategy would be needed to protect against two simultaneous requests booking the same slot.
- If this were developed further, I would add authentication, authorization, pagination, structured error responses, and a stronger concurrency strategy.

## Assignment mapping

- Page 5, item 1: create a booking, implemented by `POST /api/bookings`.
- Page 5, item 2: prevent overlap, implemented by `BookingOverlapCheckerService`.
- Page 5, item 3: retrieve by resource and date range, implemented by `GET /api/bookings/resource/{resourceId}`.
- Page 5, item 4: cancel a booking, implemented by `POST /api/bookings/{id}/cancel`.
- Page 5, item 5: focused frontend, implemented in `frontend`.
- Page 8: source code, README, and unit tests are included.
- Page 9: the repository includes the backend, frontend, tests, and README required for submission.
