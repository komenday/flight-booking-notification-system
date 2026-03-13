# 📧 FBNS - Flight Booking Notification System

A lightweight notification microservice for processing flight reservation events and sending transactional emails. Designed to work seamlessly with FBS (Flight Booking System) through webhook-based event delivery.

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Microservice-green)](https://microservices.io/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Webhook Endpoints](#-webhook-endpoints)
- [Email Templates](#-email-templates)
- [Configuration](#-configuration)
- [Email Providers](#-email-providers)
- [Security](#-security)
- [Development](#-development)
- [Deployment](#-deployment)
- [Monitoring](#-monitoring)
- [Troubleshooting](#-troubleshooting)
- [License](#-license)

## ✨ Features

### Core Functionality
- **Webhook Listener** - Receives reservation events from FBS
- **Email Notifications** - Sends transactional emails for reservation events
- **Template Engine** - Dynamic HTML email generation
- **Event Logging** - File-based notification history
- **API Key Authentication** - Secure webhook endpoints
- **Retry Logic** - Automatic retry for failed email sends

### Supported Events
- ✉️ **Reservation Created** - Welcome email with 10-minute expiration warning
- ✅ **Reservation Confirmed** - Payment confirmation email
- ❌ **Reservation Cancelled** - Cancellation confirmation
- ⏰ **Reservation Expired** - Expiration notification

### Technical Features
- **Webhook-Based** - Event-driven architecture with FBS
- **Multiple Email Providers** - Support for Mailtrap, SendGrid, Gmail SMTP
- **Health Checks** - Service health monitoring
- **Structured Logging** - Serilog with file and console outputs
- **CORS Support** - Configured for FBS integration
- **Global Exception Handling** - Graceful error management

## 🏗️ Architecture

### Microservice Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    FBS (Main System)                        │
│                                                             │
│  Reservation Created ──────────────┐                        │
│  Reservation Confirmed ────────────┤                        │
│  Reservation Cancelled ────────────┤ HTTP POST              │
│  Reservation Expired ──────────────┘                        │
└────────────────────────────────────┬────────────────────────┘
                                     │
                    ┌────────────────▼──────────────────┐
                    │      Webhook Middleware           │
                    │    (API Key Validation)           │
                    └────────────────┬──────────────────┘
                                     │
                    ┌────────────────▼──────────────────┐
                    │    FBNS Webhook Controller        │
                    │  (Event Type Routing)             │
                    └────────────────┬──────────────────┘
                                     │
                    ┌────────────────▼──────────────────┐
                    │   Notification Service            │
                    │  (Business Logic)                 │
                    └────────────────┬──────────────────┘
                                     │
                    ┌────────────────▼──────────────────┐
                    │    Email Service                  │
                    │  (Mailtrap)        │
                    └────────────────┬──────────────────┘
                                     │
                    ┌────────────────▼──────────────────┐
                    │   File Notification Logger        │
                    │  (Audit Trail)                    │
                    └───────────────────────────────────┘
```

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│        FBNS.API (Presentation)          │
│  Controllers, Middleware, Startup       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     FBNS.Application (Use Cases)        │
│  Services, DTOs, Interfaces             │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│   FBNS.Infrastructure (External I/O)    │
│  Email Services, File Logging           │
└─────────────────────────────────────────┘
```

## 🛠️ Technologies

### Core Stack
- **.NET 10.0** - Latest LTS framework
- **ASP.NET Core 10.0** - Web API framework
- **Serilog** - Structured logging

### Email Providers (Choose One)
- **Mailtrap API** - Development/testing

### Tools & Libraries
- **Polly** - Retry policies and resilience
- **HttpClient** - HTTP API client (for Mailtrap/SendGrid)

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Email provider account (choose one):
  - [Mailtrap](https://mailtrap.io/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/komenday/flight-booking-notification-system.git
   cd flight-booking-notification-system
   ```

2. **Configure email provider**

   **Option A: Mailtrap**
   ```json
   {
     "MailtrapApi": {
       "ApiToken": "your-api-token-here",
       "InboxId": "your-inbox-id-here",
       "FromEmail": "noreply@flightbooking.com",
       "FromName": "Flight Booking System"
     }
   }
   ```

3. **Configure webhook authentication**
   
   Update `appsettings.json`:
   ```json
   {
     "Webhook": {
       "ApiKey": "your-secure-api-key-here"
     }
   }
   ```

   ⚠️ **Important**: This API key must match the one configured in FBS!

4. **Run the application**
   ```bash
   cd src/FBNS.API
   dotnet run
   ```

5. **Verify service is running**
   - Health check: https://localhost:5002/health
   - Swagger UI: https://localhost:5002/swagger

### Quick Test

Send a test webhook:
```bash
curl -X POST https://localhost:5002/webhooks/reservation-created \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key-here" \
  -d '{
    "eventId": "123e4567-e89b-12d3-a456-426614174000",
    "occurredAt": "2024-02-26T10:00:00Z",
    "reservationId": "456e7890-e89b-12d3-a456-426614174111",
    "flightId": "789e0123-e89b-12d3-a456-426614174222",
    "flightNumber": "AA1234",
    "seatNumber": "12A",
    "passengerFirstName": "John",
    "passengerLastName": "Doe",
    "passengerEmail": "john.doe@example.com",
    "expiresAt": "2024-02-26T10:15:00Z"
  }'
```

## 📁 Project Structure

```
src/
├── FBNS.API/                          # Presentation Layer
│   ├── Controllers/
│   │   └── WebhooksController.cs     # Webhook endpoints
│   ├── Middleware/
│   │   ├── WebhookAuthenticationMiddleware.cs
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── appsettings.json               # Configuration
│   └── Program.cs                     # Application entry point
│
├── FBNS.Application/                  # Application Layer
│   ├── Services/
│   │   ├── INotificationService.cs
│   │   ├── NotificationService.cs
│   │   ├── IEmailService.cs
│   │   ├── IFileNotificationLogger.cs
│   │   └── NotificationLog.cs
│   └── Events/
│       ├── ReservationCreatedEvent.cs
│       ├── ReservationConfirmedEvent.cs
│       ├── ReservationCancelledEvent.cs
│       └── ReservationExpiredEvent.cs
│
└── FBNS.Infrastructure/               # Infrastructure Layer
    ├── Email/
    │   ├── MailtrapApiEmailService.cs # Mailtrap implementation
    │   ├── SendGridEmailService.cs    # SendGrid implementation
    │   ├── GmailSmtpEmailService.cs   # Gmail implementation
    │   └── FileNotificationLogger.cs  # Audit logging
    └── DependencyInjection.cs         # Service registration
```

## 🔌 Webhook Endpoints

### Authentication

All webhook endpoints require API key authentication via `X-API-Key` header.

### Endpoints

| Method | Endpoint | Description | Event Type |
|--------|----------|-------------|------------|
| `POST` | `/webhooks/reservation-created` | New reservation created | Sends welcome email |
| `POST` | `/webhooks/reservation-confirmed` | Reservation confirmed | Sends confirmation email |
| `POST` | `/webhooks/reservation-cancelled` | Reservation cancelled | Sends cancellation email |
| `POST` | `/webhooks/reservation-expired` | Reservation expired | Sends expiration notice |

### Request Format

All endpoints accept the same DTO structure:

```json
{
  "eventId": "guid",
  "occurredAt": "datetime",
  "reservationId": "guid",
  "flightId": "guid",
  "flightNumber": "string",
  "seatNumber": "string",
  "passengerFirstName": "string",
  "passengerLastName": "string",
  "passengerEmail": "string",
  "expiresAt": "datetime"  // Optional, only for created events
}
```

### Response Format

**Success (200 OK):**
```json
{
  "message": "Notification processed successfully"
}
```

**Unauthorized (401):**
```json
{
  "error": "Unauthorized",
  "message": "Invalid or missing API key"
}
```

**Bad Request (400):**
```json
{
  "error": "Bad Request",
  "message": "Invalid event data"
}
```

## 📧 Email Templates

### Reservation Created

**Subject:** ✈️ Reservation Created - Confirm Within 10 Minutes

```
Hello John Doe,

Your flight reservation has been created successfully!

Flight Details:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✈️  Flight:        AA1234
💺  Seat:          12A
🆔  Reservation:   456e7890...
⏰  Expires:       Feb 26, 2024 10:15 AM

⚠️  IMPORTANT: Your reservation will expire in 10 minutes.

Please confirm your reservation as soon as possible.

Thank you for choosing Flight Booking System!
```

### Reservation Confirmed

**Subject:** ✅ Reservation Confirmed - Flight AA1234

```
Hello John Doe,

Great news! Your reservation has been confirmed.

Your booking is now complete and your seat is reserved.

Flight Details:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✈️  Flight:        AA1234
💺  Seat:          12A
🆔  Reservation:   456e7890...

Have a great flight! ✈️
```

### Reservation Cancelled

**Subject:** ❌ Reservation Cancelled - Flight AA1234

```
Hello John Doe,

Your reservation has been cancelled as requested.

Cancelled Booking:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✈️  Flight:        AA1234
💺  Seat:          12A
🆔  Reservation:   456e7890...

You can make a new reservation at any time.
```

### Reservation Expired

**Subject:** ⏰ Reservation Expired - Flight AA1234

```
Hello John Doe,

Unfortunately, your reservation has expired.

The 10-minute confirmation window has passed.

Expired Booking:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✈️  Flight:        AA1234
💺  Seat:          12A
🆔  Reservation:   456e7890...

Don't worry - you can make a new reservation for the same flight.
```

## ⚙️ Configuration

### Complete appsettings.json Example

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  
  "MailtrapApi": {
    "ApiToken": "your-mailtrap-api-token",
    "InboxId": "your-inbox-id",
    "FromEmail": "noreply@flightbooking.com",
    "FromName": "Flight Booking System"
  },
  
  "FileLogger": {
    "LogFilePath": "logs/notifications.log"
  },
  
  "Webhook": {
    "ApiKey": "your-secure-api-key"
  },
  
  "Cors": {
    "AllowedOrigins": {
      "FBS": "https://localhost:5001"
    }
  },
  
  "AllowedHosts": "*"
}
```

### Environment-Specific Configuration

**Development:**
```json
{
  "MailtrapApi": {
    "ApiToken": "test-token",
    "InboxId": "123456"
  }
}
```

**Production:**
```json
{
  "SendGrid": {
    "ApiKey": "SG.production-key",
    "FromEmail": "noreply@flightbooking.com"
  }
}
```

## 📬 Email Providers

### Mailtrap

**Features:**
- ✅ Unlimited test emails
- ✅ Email preview in web UI
- ✅ No actual emails sent
- ✅ Perfect for development

**Setup:**
1. Create account at [mailtrap.io](https://mailtrap.io)
2. Get API token from Settings → API Tokens
3. Get Inbox ID from inbox URL
4. Configure in `appsettings.json`

**Configuration:**
```json
{
  "MailtrapApi": {
    "ApiToken": "your-token-here",
    "InboxId": "123456",
    "FromEmail": "noreply@flightbooking.com",
    "FromName": "Flight Booking System"
  }
}
```

## 🔒 Security

### API Key Authentication

**Middleware:** `WebhookAuthenticationMiddleware`

Validates `X-API-Key` header on all webhook requests:

```csharp
if (request.Headers["X-API-Key"] != expectedApiKey)
{
    context.Response.StatusCode = 401;
    return;
}
```

**Best Practices:**
- ✅ Use strong, random API keys (32+ characters)
- ✅ Store in environment variables in production
- ✅ Rotate keys periodically
- ✅ Never commit keys to source control

### CORS Configuration

Restricts requests to configured FBS origin:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFBS", policy =>
    {
        policy.WithOrigins("https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### Secrets Management

**Development:**
```bash
dotnet user-secrets init
dotnet user-secrets set "Webhook:ApiKey" "your-key-here"
dotnet user-secrets set "MailtrapApi:ApiToken" "your-token-here"
```

**Production:**
- Use Azure Key Vault
- Use AWS Secrets Manager
- Use environment variables

## 👨‍💻 Development

### Running Locally

```bash
# Run with hot reload
dotnet watch run --project src/FBNS.API

# Run with specific environment
dotnet run --project src/FBNS.API --environment Development

# Run on specific port
dotnet run --project src/FBNS.API --urls "https://localhost:5002"
```

### Testing Webhooks

Use tools like:
- **Postman** - API testing tool
- **curl** - Command line
- **REST Client** - VS Code extension

Example curl command:
```bash
curl -X POST https://localhost:5002/webhooks/reservation-created \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-key" \
  -d @test-event.json
```

### Viewing Logs

**Console logs:**
```bash
dotnet run  # See logs in terminal
```

**File logs:**
```bash
# View notification history
cat logs/notifications.log

# Watch logs in real-time
tail -f logs/notifications.log
```

**Log format:**
```
[2024-02-26 10:15:23] SUCCESS | To: john@example.com | Subject: Reservation Created | Provider: Mailtrap
[2024-02-26 10:20:45] FAILED | To: jane@example.com | Subject: Reservation Confirmed | Error: Timeout
```

## 🚢 Deployment

### Docker

**Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/FBNS.API/FBNS.API.csproj", "FBNS.API/"]
RUN dotnet restore "FBNS.API/FBNS.API.csproj"
COPY . .
WORKDIR "/src/FBNS.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FBNS.API.dll"]
```

**Build and run:**
```bash
docker build -t fbns:latest .
docker run -d -p 5002:80 \
  -e Webhook__ApiKey="your-key" \
  -e MailtrapApi__ApiToken="your-token" \
  fbns:latest
```

### Azure App Service

```bash
az webapp up \
  --name fbns-app \
  --resource-group your-rg \
  --runtime "DOTNETCORE:10.0"
```

### Environment Variables

Required:
- `Webhook__ApiKey` - API key for authentication
- `MailtrapApi__ApiToken` - Email provider token
- `MailtrapApi__InboxId` - Email provider inbox

Optional:
- `ASPNETCORE_ENVIRONMENT` - Environment name
- `Cors__AllowedOrigins__FBS` - FBS origin URL

## 📊 Monitoring

### Health Checks

**Endpoint:** `GET /health`

**Response:**
```json
{
  "status": "healthy",
  "service": "FBNS"
}
```

### Metrics to Monitor

- **Request Rate** - Webhooks/minute
- **Error Rate** - Failed notifications percentage
- **Email Delivery Time** - Average time to send
- **API Key Failures** - Unauthorized requests count

### Application Insights (Azure)

```csharp
// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

## 🔧 Troubleshooting

### Common Issues

#### 1. 401 Unauthorized
**Cause:** API key mismatch between FBS and FBNS

**Solution:**
```bash
# Check FBNS key
cat src/FBNS.API/appsettings.json | grep ApiKey

# Check FBS key
cat src/FBS.API/appsettings.json | grep ApiKey

# They must match!
```

#### 2. Email Not Sending
**Cause:** Invalid email provider credentials

**Solution:**
- Check API token/password is correct
- Verify sender email is verified
- Check logs for detailed error

#### 3. Webhook Not Received
**Cause:** CORS or network issue

**Solution:**
- Check FBS URL in CORS config
- Verify FBNS is running on correct port
- Check firewall settings

### Debug Mode

Enable detailed logging:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

## 📄 License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file.

## 📞 Contact

- **Project Link**: [https://github.com/komenday/flight-booking-notification-system](https://github.com/komenday/flight-booking-notification-system)
- **Issues**: [https://github.com/komenday/flight-booking-notification-system/issues](https://github.com/komenday/flight-booking-notification-system/issues)

## 🔗 Related Projects

- **FBS** - Flight Booking System (Main Application)
- [FBS Repository](https://github.com/komenday/flight-booking-system)

---

Built with ❤️ for reliable transactional email delivery.
