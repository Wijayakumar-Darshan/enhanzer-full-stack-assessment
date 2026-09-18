# Purchase Bill App — Full Stack Developer Assignment

Angular + ASP.NET Core solution for the Enhanzer Full Stack Developer Trainee
technical assessment (Login page + Purchase Bill form).

## Project structure

```
project/
├── backend/PurchaseBillApi/     ASP.NET Core 8 Web API
├── frontend/purchase-bill-app/  Angular 18 app
├── database/CreateDatabase.sql  SQL Server schema (Location_Details, Purchase_Bill_Item)
└── README.md
```

## Architecture overview

- **Login (Task 1):** Angular login form → `POST /api/auth/login` on our backend →
  backend calls the external POS API (`https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke`)
  with `API_Action: GetLoginData`, using the entered email/password. On success, the
  backend issues its own JWT (so the SPA never talks to the external API directly),
  and saves the returned `User_Locations` into the `Location_Details` table, scoped
  to that user.
- **Route protection:** `authGuard` blocks `/purchase-bill` unless a JWT is present;
  an `HttpInterceptor` attaches the JWT to every API call; `[Authorize]` protects
  the backend endpoints.
- **Purchase Bill (Task 2):** Item field is an autocomplete (`Mango, Apple, Banana,
  Orange, Grapes, Kiwi, Strawberry`); Batch dropdown is populated from the
  `Location_Details` rows saved at login. `Total Cost = (Standard Cost × Qty) −
  Discount%`, `Total Selling = Standard Price × Qty`. Adding an item appends it to
  the table and recalculates `Total Items` / `Total Qty` in the summary panel.

## Prerequisites

- Node.js 18+ and npm
- Angular CLI 18 (`npm install -g @angular/cli`)
- .NET 8 SDK
- SQL Server (LocalDB, Express, or full) and SQL Server Management Studio (optional)

## 1. Database setup

Run the script in SQL Server Management Studio or `sqlcmd`:

```bash
sqlcmd -S localhost -i database/CreateDatabase.sql
```

This creates the `PurchaseBillDb` database with `Location_Details` and
`Purchase_Bill_Item` tables.

## 2. Backend setup

```bash
cd backend/PurchaseBillApi
dotnet restore
```

Update `appsettings.json`:
- `ConnectionStrings:DefaultConnection` → point at your SQL Server instance.
- `Jwt:Key` → replace with a long random secret (32+ chars) before any real use.

Run the API:

```bash
dotnet run
```

By default it listens on `https://localhost:7000` (check the console output /
`launchSettings.json` for the exact port) and exposes Swagger at `/swagger` in
Development mode.

### API endpoints

| Method | Route                 | Auth | Purpose                                   |
|--------|-----------------------|------|--------------------------------------------|
| POST   | `/api/auth/login`     | No   | Authenticates via the external POS API, saves locations, returns a JWT |
| GET    | `/api/location`       | Yes  | Returns saved locations for the Batch dropdown |
| GET    | `/api/purchasebill`   | Yes  | Lists saved purchase bill items |
| POST   | `/api/purchasebill`   | Yes  | Adds a new purchase bill item |

## 3. Frontend setup

```bash
cd frontend/purchase-bill-app
npm install
```

Confirm `src/environments/environment.ts` → `apiBaseUrl` matches the port your
backend is running on (e.g. `https://localhost:7000/api`).

```bash
npm start
```

Open `http://localhost:4200`.

## 4. Trying it out

1. Go to `/login`, enter the credentials from the assignment's sample JSON
   (or any valid account on the staging POS system).
2. On success you're redirected to `/purchase-bill`.
3. Add an item (choose from the autocomplete, pick a Batch, enter Qty/Discount)
   and click **Add** — it appears in the table below and the summary panel
   updates its totals.
4. Refreshing the page or navigating directly to `/purchase-bill` without
   logging in redirects back to `/login`.

## Notes on design decisions

- The JWT is stored in `sessionStorage` (cleared when the tab closes) rather
  than `localStorage`, to reduce exposure if the browser is shared.
- The backend re-computes `Total Cost` / `Total Selling` server-side so the API
  remains the source of truth even though the UI also calculates them live for
  instant feedback.
- Locations are scoped per-username in `Location_Details`, so multiple users
  logging in on the same machine don't see each other's batches.
