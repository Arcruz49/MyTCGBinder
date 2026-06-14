# MyTCGBinder

A Pokémon TCG collection manager. On first startup the API syncs the full card catalog from [pokemontcg.io](https://pokemontcg.io) into a local PostgreSQL table — all searches and set listings run against the local database, with no live API calls during normal use.

## Stack

| Layer | Technology |
|-------|-----------|
| API | ASP.NET Core 10, C#, Clean Architecture |
| Database | PostgreSQL 17 via EF Core (Npgsql) |
| Auth | JWT in HttpOnly cookie (`MyTCGBinder_token`) |
| Frontend | Angular 19+, Signals, standalone components |
| Infrastructure | Docker + docker-compose |

## Running with Docker

```bash
# 1. Create and fill in the environment file
cp .env.example .env   # set JWT_KEY, DB_USER, DB_PASSWORD, etc.

# 2. Start everything
docker compose up --build
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4201 |
| API | http://localhost:5010 |
| Swagger | http://localhost:5010/swagger |

## Running locally

**Prerequisites:** .NET 10 SDK, Node 22, PostgreSQL running locally.

```bash
# API
cd api
dotnet run          # http://localhost:5010

# Frontend (separate terminal)
cd front
npm install
npm start           # http://localhost:4200
```

Migrations are applied automatically on API startup.

### EF Migrations

```bash
cd api
export PATH="$PATH:$HOME/.dotnet/tools"
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add <name>
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update
```

> Requires `appsettings.Development.json` with `Jwt:Key` and the local connection string (gitignored).

## Environment variables

Create a `.env` file at the project root (gitignored):

```env
JWT_KEY=<64-char hex key>
DB_USER=postgres
DB_PASSWORD=<password>
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USER=you@email.com
EMAIL_PASSWORD=<app password>
EMAIL_FROM=you@email.com
APP_BASE_URL=http://localhost:4201
TCG_API_KEY=                        # optional — lower rate limit without a key
TcgApi__BaseUrl=https://api.pokemontcg.io/v2/
```

## Project structure

```
MyTCGBinder/
├── api/                  # ASP.NET Core 10
│   ├── Domain/           # Entities (User, UserCard, TCGCard), Value Objects, enums, repository interfaces
│   ├── Application/      # Use cases, DTOs, service interfaces
│   ├── Infrastructure/
│   │   ├── Data/         # EF Core Context
│   │   ├── Repositories/ # UserRepository, UserCardRepository, TCGCardRepository
│   │   ├── Jobs/         # SeedTcgCardsJob (populates tcg_cards on startup)
│   │   └── ...           # EmailService, ExceptionMiddleware
│   └── Controllers/      # AuthController, CardController, TcgController
├── front/                # Angular 19+
│   └── src/app/
│       ├── core/         # Services, guards, interceptors, models
│       ├── features/     # auth, collection, profile, search
│       └── shared/       # PokedexHeader, BottomNav
└── docker-compose.yml
```

## API endpoints

### Auth (`/auth`)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/auth/register` | Create account |
| POST | `/auth/login` | Login (sets JWT cookie) |
| GET | `/auth/me` | Authenticated user info |
| POST | `/auth/logout` | Logout (clears cookie) |
| POST | `/auth/forgot-password` | Send password reset email |
| POST | `/auth/reset-password` | Reset password via token |
| DELETE | `/auth/` | Delete account and all data |

### Collection (`/cards`) — requires auth
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/cards?page=&pageSize=&search=` | List collection (paginated) |
| GET | `/cards/count` | Total cards in collection |
| POST | `/cards` | Add card (`tcgCardId`, `variant`) |
| PATCH | `/cards/{id}/quantity` | Increment or decrement quantity |
| DELETE | `/cards/{id}` | Remove card |

### TCG (`/tcg`) — requires auth
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/tcg/sets` | List all sets (from local DB) |
| GET | `/tcg/search?name=&setId=` | Search cards (from local DB, up to 100 results) |

## Card variants

The `variant` field distinguishes physical copies of the same card. It is auto-detected from the `rarity` field returned by pokemontcg.io — no manual selection required.

| Variant | Maps from |
|---------|-----------|
| `Normal` | Common, Uncommon, Rare |
| `Holo` | Rare Holo, Double Rare, Trainer Gallery Holo |
| `ReverseHolo` | Reverse Holo |
| `Promo` | Black Star Promos |
| `FullArt` | Rare Ultra, Ultra Rare |
| `IllustrationRare` | Illustration Rare (SV era) |
| `SpecialIllustrationRare` | Special Illustration Rare (SV era) |
| `HyperRare` | Hyper Rare, Rainbow Rare |
| `SecretRare` | Rare Secret |
