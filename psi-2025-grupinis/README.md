# Filmder – Film Tinder

A movie recommendation app where groups of friends swipe through films together to find something to watch.

## Tech Stack

- ASP.NET Core 9.0
- SQLite with Entity Framework Core
- JWT Authentication
- SignalR for real-time chat

## Setup

Clone and restore dependencies:
```bash
git clone <repository-url>
cd Filmder
dotnet restore
```

Run database migrations:
```bash
dotnet ef database update
```

Start the application:
```bash
dotnet run
```

API runs at `http://localhost:5144`

## API Endpoints

**Auth**
- POST `/register` - Create account
- POST `/login` - Get JWT token

**Groups**
- POST `/api/group/create` - Create group
- GET `/api/group/mine` - List your groups

**Chat**
- WebSocket: `/chatHub`

## Configuration

Set these in `appsettings.Development.json`:
- Connection string for SQLite
- TokenKey (minimum 64 characters)

## Database Models

- AppUser: User accounts
- Movie: Film catalog
- Group: Movie groups
- GroupMember: Group membership
- Message: Chat messages