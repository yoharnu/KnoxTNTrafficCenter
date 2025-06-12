# KnoxTrafficCenter

## Project Overview
KnoxTrafficCenter is an internal web application for monitoring traffic cameras, incidents, construction events, and weather alerts in the Knox County, Tennessee area. The application integrates with the Tennessee Department of Transportation (TDOT) Smartway API to fetch real-time traffic data.

## Technology Stack
- **Framework:** ASP.NET Core 8.0
- **Frontend:** Razor Pages with Bootstrap
- **API:** REST API controllers for alerts and other data
- **External Dependencies:** 
  - TDOT Smartway API
  - Video.js for video streaming
  - Bootstrap Icons

## Project Structure
- **Controllers/** - API endpoints for alerts (incidents, construction, etc.)
- **Models/** - Data models including Camera and TDOT API models
- **Pages/** - Razor pages for the web interface
- **Services/** - Services for interacting with the TDOT API
- **wwwroot/** - Static assets including CSS, JavaScript, and libraries

## Key Components

### Services
- **TDOTAPIService** - Core service that communicates with the TDOT Smartway API
- **TDOTEventService** - Service for handling traffic events

### Models
- **Camera** - Represents traffic cameras with video and image streams
- **CameraGroup** - Groups of cameras organized by location
- **TDOT API Models** - Models for handling TDOT API data (Camera, Event, Location)

### API Endpoints
The application provides REST API endpoints under `/api/alerts` for:
- Incidents
- Construction events
- Weather alerts

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Node.js (for npm packages)

### Getting Started
1. Clone the repository
2. Restore NuGet packages: `dotnet restore`
3. Install npm dependencies: `npm install`
4. Run the application: `dotnet run --project KnoxTrafficCenter`
5. Access the application at `https://localhost:7240` or `http://localhost:5200`

### Configuration
- API settings are loaded from the TDOT config endpoint
- Application settings can be modified in `appsettings.json`
- For local development, use `appsettings.Development.json`

## Additional Notes
- The project uses Swagger for API documentation, available at `/swagger` when running the application
- User secrets are configured for sensitive information (API keys)