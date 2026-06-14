# MyTCGBinder

A Pokémon TCG collection manager built for collectors who want to track every card they own, including quantity and physical variant (Holo, Reverse Holo, Full Art, etc.).

---

## Overview

MyTCGBinder lets users build and manage a digital binder of their Pokémon TCG collection. On first startup, the API syncs the full card catalog from [pokemontcg.io](https://pokemontcg.io) into a local PostgreSQL table (`tcg_cards`). From that point on, all card searches and set listings run entirely against the local database — no external API calls happen during normal use.

Users can search the catalog by name or set, add cards to their collection, and track the exact physical variant of each copy (e.g., a card can exist as both a Normal and a Holo in the same collection). Quantity is tracked per `(card, variant)` pair, and incrementing an existing entry is handled automatically on add.

Authentication uses JWT stored in an HttpOnly cookie. Password recovery is handled via a time-limited email link with a SHA-256-hashed token stored in the database.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| API | ASP.NET Core 10, Clean Architecture, EF Core (Npgsql) |
| Database | PostgreSQL 17 |
| Auth | JWT in HttpOnly cookie (`MyTCGBinder_token`) |
| Email | MailKit (SMTP) |
| Frontend | Angular 19+, Signals, standalone components, pure CSS |
| Infrastructure | Docker, Docker Compose, nginx reverse proxy |

---

## Features

**Collection**
- Add any card from the local catalog to your collection
- Variant is auto-detected from the card's rarity — no manual selection
- Each `(card, variant)` pair tracks its own quantity; adding a duplicate increments it
- Filter collection by set via scrollable chips
- Search within your own collection by card name
- Paginated grid view with quantity and variant badges on each card

**Card Catalog (local)**
- Full pokemontcg.io catalog synced to `tcg_cards` on startup via `SeedTcgCardsJob`
- Seed resumes automatically if interrupted (page offset based on existing count)
- Search by name and/or set, up to 100 results
- Set listing aggregated from the local table (no external call)

**Account Management**
- Register, login, logout
- Password recovery via email with a 15-minute token (SHA-256 hashed in DB, one-time use)
- Account deletion — removes all user data and cards permanently

---

## Project Structure

```
MyTCGBinder/
├── docker-compose.yml
├── .env                          # secrets — gitignored, see Environment section
├── api/
│   ├── Domain/
│   │   ├── Entities/             # User, UserCard, TCGCard, PasswordResetToken
│   │   ├── Interfaces/           # IUserRepository, IUserCardRepository, ITCGCardRepository, IUnitOfWork
│   │   ├── Enums/                # CardVariant
│   │   ├── ValueObjects/         # Email, Password
│   │   └── Exceptions/           # DomainException, NotFoundException, ValidationException, ForbiddenException
│   ├── Application/
│   │   ├── UseCases/             # Auth + Card use cases
│   │   ├── DTOs/                 # Requests and Responses
│   │   └── Interfaces/           # Use case interfaces, IEmailService, ITcgService
│   ├── Infrastructure/
│   │   ├── Data/                 # EF Core Context
│   │   ├── Repositories/         # UserRepository, UserCardRepository, TCGCardRepository
│   │   ├── Jobs/                 # SeedTcgCardsJob (BackgroundService)
│   │   ├── Middlewares/          # ExceptionMiddleware
│   │   └── TcgApi/               # TcgService (HttpClient wrapper, used only by SeedTcgCardsJob)
│   └── Controllers/              # AuthController, CardController, TcgController
└── front/
    └── src/app/
        ├── core/                 # Models, AuthService, CardService, TcgService, guard, interceptor
        ├── features/
        │   ├── auth/             # Login + Register tabs
        │   ├── collection/       # Card grid, card detail modal
        │   ├── profile/          # User info, stats, sign out
        │   └── search/           # Search modal (queries local DB)
        └── shared/
            ├── pokedex-header/   # Mobile-only fixed top bar
            └── bottom-nav/       # Mobile-only bottom nav (Collection · FAB · Profile)
```

---

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET 10 SDK (only for running EF migrations locally)
- Node 22 (only for local frontend development)

### Environment

Create a `.env` file at the repository root:

```env
JWT_KEY=<64-char hex key>
DB_USER=postgres
DB_PASSWORD=<password>

EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USER=you@email.com
EMAIL_PASSWORD=<app password>    # Gmail App Password — no spaces
EMAIL_FROM=you@email.com

APP_BASE_URL=http://localhost:4201

TCG_API_KEY=                     # optional — lower rate limit without key
TcgApi__BaseUrl=https://api.pokemontcg.io/v2/
```

### Running with Docker

```bash
cp .env.example .env   # fill in values
docker compose up --build
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4201 |
| API | http://localhost:5010 |
| Swagger | http://localhost:5010/swagger |

The API applies migrations and starts the card catalog seed automatically on startup. The seed downloads the full pokemontcg.io catalog (≈ 20 000+ cards) in batches of 250. Subsequent startups skip the seed if more than 20 000 cards are already present.

### Running locally

```bash
# API
cd api
dotnet run          # http://localhost:5010

# Frontend (separate terminal)
cd front
npm install && npm start    # http://localhost:4200
```

> Local dev requires `api/appsettings.Development.json` (gitignored) with `Jwt:Key` and `ConnectionStrings:DefaultConnection`.

### EF Migrations

```bash
cd api
export PATH="$PATH:$HOME/.dotnet/tools"
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add <MigrationName>
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update
```

---

## API Reference

All authenticated endpoints require the `MyTCGBinder_token` JWT cookie set by the login response.

### Auth (`/auth`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/auth/register` | — | Create account |
| POST | `/auth/login` | — | Login, sets JWT cookie |
| GET | `/auth/me` | Required | Authenticated user info |
| POST | `/auth/logout` | Required | Clears JWT cookie |
| POST | `/auth/forgot-password` | — | Send password reset email |
| POST | `/auth/reset-password` | — | Reset password via token from email |
| DELETE | `/auth/` | Required | Delete account and all data |

### Collection (`/cards`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/cards?page=&pageSize=&search=` | Required | Paginated collection |
| GET | `/cards/count` | Required | Total card quantity in collection |
| POST | `/cards` | Required | Add card `{ tcgCardId, variant }` — increments if duplicate |
| PATCH | `/cards/{id}/quantity` | Required | `{ action: "increment" \| "decrement" }` |
| DELETE | `/cards/{id}` | Required | Remove card from collection |

### TCG Catalog (`/tcg`)

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/tcg/sets` | Required | All sets (from local DB) |
| GET | `/tcg/search?name=&setId=` | Required | Search cards (local DB, max 100 results) |

---

## Architecture Notes

**Clean Architecture** — dependencies flow inward. Controllers → Application → Domain. Infrastructure implements the interfaces defined in Domain.

**Card data model** — `TCGCard` stores the full catalog. `UserCard` stores only the user's ownership record: `(UserId, TcgCardId, Variant, Quantity)`. Card details (name, image, set, rarity) are never duplicated — they are always read through the `TCGCard` navigation property via EF Core `Include`.

**Seed job** — `SeedTcgCardsJob` is an `IHostedService` that runs once at startup. It first fetches all sets to build a `setId → series` map, then pages through the cards endpoint. If interrupted, it calculates the resume page from the existing row count. The job is registered with `BackgroundServiceExceptionBehavior.Ignore` so a seed failure does not crash the API.

**Rate limiting** — applied at the controller level via ASP.NET Core's built-in rate limiter:

| Policy | Limit |
|--------|-------|
| `login` | 5 req / 1 min |
| `register` | 3 req / 1 min |
| `forgot-password` | 3 req / 5 min |
| `reset-password` | 5 req / 5 min |

**Frontend layout**
- Mobile (< 768 px): fixed top header 56 px + fixed bottom nav 56 px with a central FAB button.
- Desktop (≥ 768 px): fixed left sidebar 220 px (Collection, Profile, Logout). No top header.
- Auth page: full-screen centered card, no nav.

---

## Card Variants

The `variant` field distinguishes physical copies of the same card. It is auto-detected on the frontend from the `rarity` string returned by the catalog — no manual selection required.

| Variant | Detected from rarity |
|---------|----------------------|
| `Normal` | Common, Uncommon, Rare |
| `Holo` | Rare Holo, Double Rare, Prism Star |
| `ReverseHolo` | Reverse Holo |
| `Promo` | Black Star Promos |
| `FullArt` | Rare Ultra, Ultra Rare |
| `IllustrationRare` | Illustration Rare (SV era) |
| `SpecialIllustrationRare` | Special Illustration Rare (SV era) |
| `HyperRare` | Hyper Rare, Rainbow Rare |
| `SecretRare` | Rare Secret |

---

## License

This project is for personal and educational use.
