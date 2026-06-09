# PropertyCare (e-Kvarovi)

Web application for digital management of fault reports and service interventions
in buildings (offices, residential complexes, warehouses).

Roles: **Reporter** (files fault requests), **Technician** (resolves work orders),
**Administrator** (triage, assignment, facilities management).

## Tech stack

- **Backend:** ASP.NET Core Web API (.NET 10), Clean Architecture
  (Domain / Application / Infrastructure / API), CQRS with MediatR,
  FluentValidation, EF Core (SQL Server), JWT + refresh tokens
- **Frontend:** Angular (NgModules + lazy loading), Angular Material, RxJS
- **Database:** SQL Server (LocalDB in development)

## Project structure

```
backend/    ASP.NET Core solution (PropertyCare.Backend.slnx)
frontend/   Angular application
```

## Running locally

### Backend

```bash
cd backend
dotnet run --project PropertyCare.API --launch-profile https
```

- API: https://localhost:7260 (Swagger UI at /swagger in Development)
- On first run the database is migrated and seeded with demo data automatically.
- Connection string and JWT settings live in `PropertyCare.API/appsettings.Development.json`.

### Frontend

```bash
cd frontend
npm install
ng serve
```

- App: http://localhost:4200
- API base URL is configured in `src/environments/environment.development.ts`.

## Demo logins (seeded)

| Role          | Email                       | Password     |
|---------------|-----------------------------|--------------|
| Administrator | admin@propertycare.ba       | Admin123!    |
| Technician    | technician1@propertycare.ba | Tech123!     |
| Technician    | technician2@propertycare.ba | Tech123!     |
| Reporter      | reporter1@propertycare.ba   | Reporter123! |
| Reporter      | reporter2@propertycare.ba   | Reporter123! |

## Tests

```bash
cd backend
dotnet test
```
