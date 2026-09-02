# Beatok API

Welcome to **Beatok API**! The backend API for the Beatok competitive beat-making platform.

The frontend application is available in a [separate repository](https://github.com/upyrov/beatok).

## 🛠️ Features

- **REST API**: ASP.NET Core Web API.
- **Authentication**: Firebase Authentication and JWT-based authentication.
- **Real-Time Capabilities**: Real-time communication through SignalR.
- **Database**: PostgreSQL with Entity Framework Core.
- **Background Jobs**: Background and scheduled tasks using Hangfire.
- **Clean Architecture**: Separation of API, Application, Domain, and Infrastructure layers.

## 📦 Getting Started

### Prerequisites

[**.NET 10**](https://dotnet.microsoft.com/) is required to run the backend.

Make sure you have the .NET 10 SDK installed.

A PostgreSQL database is also required.

### Installation

1. Clone the repository:

       git clone https://github.com/upyrov/beatok-api.git
       cd beatok-api

2. Restore the dependencies:

       dotnet restore

3. Configure the application settings and database connection.

4. Apply database migrations:

       dotnet ef database update

5. Start the API:

       dotnet run

The API will be available at the configured local address.

### API Documentation

[**Scalar**](https://scalar.com/) is available for exploring the API endpoints and testing requests.

Once the API is running, open `http://localhost:xxxx/scalar` in your browser.

## 🤝 Contributing

We welcome contributions to Beatok API!

Please refer to our [**CONTRIBUTING.md**](https://github.com/upyrov/beatok-api/blob/main/CONTRIBUTING.md) for detailed instructions on:

- Local development workflow
- Database migrations
- Pull request process
- Reporting bugs and features

## 📜 License

This project is licensed under the terms of the [MIT License](https://github.com/upyrov/beatok-api/blob/main/LICENSE).
