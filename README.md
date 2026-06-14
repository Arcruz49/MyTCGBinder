# MyTCGBinder

Gerenciador de coleção de cartas Pokémon TCG. Permite buscar cartas na API pública [pokemontcg.io](https://pokemontcg.io), adicioná-las à sua coleção com controle de quantidade e variante, e organizar por set.

## Stack

| Camada | Tecnologia |
|--------|-----------|
| API | ASP.NET Core 10, C#, Clean Architecture |
| Banco | PostgreSQL 17 via EF Core (Npgsql) |
| Auth | JWT em cookie HttpOnly (`MyTCGBinder_token`) |
| Frontend | Angular 19+, Signals, componentes standalone |
| Infra | Docker + docker-compose |

## Rodando com Docker

```bash
# 1. Copie e preencha as variáveis de ambiente
cp .env.example .env   # edite JWT_KEY, DB_USER, DB_PASSWORD etc.

# 2. Suba tudo
docker compose up --build
```

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:4201 |
| API | http://localhost:5010 |
| Swagger | http://localhost:5010/swagger |

## Rodando localmente (sem Docker)

**Pré-requisitos:** .NET 10 SDK, Node 22, PostgreSQL rodando localmente.

```bash
# API
cd api
dotnet run          # http://localhost:5010

# Frontend (outro terminal)
cd front
npm install
npm start           # http://localhost:4200
```

A API aplica migrações automaticamente no startup.

### EF Migrations

```bash
cd api
export PATH="$PATH:$HOME/.dotnet/tools"
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add <nome>
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update
```

> Requer `appsettings.Development.json` com `Jwt:Key` e a connection string local (arquivo gitignored).

## Variáveis de ambiente

Crie `.env` na raiz do projeto (gitignored):

```env
JWT_KEY=<chave-hex-64-chars>
DB_USER=postgres
DB_PASSWORD=<senha>
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USER=seu@email.com
EMAIL_PASSWORD=<app-password>
EMAIL_FROM=seu@email.com
APP_BASE_URL=http://localhost:4201
TCG_API_KEY=                        # opcional — sem key tem rate limit menor
TcgApi__BaseUrl=https://api.pokemontcg.io/v2/
```

## Estrutura

```
MyTCGBinder/
├── api/                  # ASP.NET Core 10
│   ├── Domain/           # Entidades, Value Objects, enums, interfaces
│   ├── Application/      # Use cases, DTOs, interfaces de serviços
│   ├── Infrastructure/   # EF Core, repositórios, TcgService, EmailService
│   └── Controllers/      # AuthController, CardController, TcgController
├── front/                # Angular 19+
│   └── src/app/
│       ├── core/         # Services, guards, interceptors, models
│       ├── features/     # auth, collection, profile, search
│       └── shared/       # PokedexHeader, BottomNav
└── docker-compose.yml
```

## API — Endpoints principais

### Auth (`/auth`)
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/auth/register` | Criar conta |
| POST | `/auth/login` | Login (define cookie JWT) |
| GET | `/auth/me` | Dados do usuário autenticado |
| POST | `/auth/logout` | Logout (remove cookie) |
| POST | `/auth/forgot-password` | Envia email de reset |
| POST | `/auth/reset-password` | Redefine senha via token |
| DELETE | `/auth/` | Deleta conta e dados |

### Coleção (`/cards`) — requer autenticação
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/cards?page=&pageSize=&search=` | Lista a coleção paginada |
| GET | `/cards/count` | Total de cartas na coleção |
| POST | `/cards` | Adiciona carta (`tcgCardId`, `variant`) |
| PATCH | `/cards/{id}/quantity` | Incrementa ou decrementa quantidade |
| DELETE | `/cards/{id}` | Remove carta |

### TCG (`/tcg`) — requer autenticação
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/tcg/sets` | Lista todos os sets |
| GET | `/tcg/search?name=&setId=` | Busca cartas na pokemontcg.io |

## Variantes suportadas

O campo `variant` distingue cópias físicas diferentes da mesma carta:

| Valor | Descrição |
|-------|-----------|
| `Normal` | Common, Uncommon, Rare sem brilho |
| `Holo` | Rare Holo, Double Rare, Trainer Gallery Holo |
| `ReverseHolo` | Reverse Holo |
| `Promo` | Black Star Promos |
| `FullArt` | Rare Ultra, Ultra Rare |
| `IllustrationRare` | Illustration Rare (era SV) |
| `SpecialIllustrationRare` | Special Illustration Rare (era SV) |
| `HyperRare` | Hyper Rare, Rainbow Rare |
| `SecretRare` | Rare Secret |

A variante é detectada automaticamente da `rarity` retornada pela pokemontcg.io ao adicionar uma carta.
