
# duende-identityserver-basic ![Logo](https://img.shields.io/badge/.NET-6.0-blue) ![SQLite](https://img.shields.io/badge/SQLite-3.0-lightgrey) ![License](https://img.shields.io/github/license/zeecorleone/duende-identityserver-basic)

## Introduction
Welcome to the **Duende Identity Server Basic** repository! This project provides a basic implementation of the Duende Identity Server using .NET 6 and SQLite. It serves as a starting point for integrating identity management and authorization into your applications.

## Features
- **Duende Identity Server**: Secure your applications with an OAuth 2.0 and OpenID Connect server.
- **.NET 6**: Leverage the latest features and improvements of .NET 6.
- **SQLite**: Simplified database setup with SQLite. Ideal for development and testing.

## Getting Started

### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) ![Download .NET 6](https://img.shields.io/badge/Download-.NET%206-blue)
- [SQLite](https://www.sqlite.org/download.html) ![Download SQLite](https://img.shields.io/badge/Download-SQLite-lightgrey)

### Installation
1. **Clone the repository**
    ```bash
    git clone https://github.com/zeecorleone/duende-identityserver-basic.git
    cd duende-identityserver-basic
    ```

2. **Restore dependencies**
    ```bash
    dotnet restore
    ```

3. **Update the database**
    ```bash
    dotnet ef database update
    ```

4. **Run the application**
    ```bash
    dotnet run
    ```

### Configuration
This project uses SQLite for simplicity, but you can configure it to use any other database by installing the appropriate dependencies and updating the configuration. 

#### For example, to use SQL Server, you would:

1. Install the SQL Server package
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    ```

2. Update the `appsettings.json` with your SQL Server connection string:
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IdentityServerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3. Modify `Program.cs` to use SQL Server:
    ```csharp
    ....
    services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    ```

## Usage
Once the application is running, you can access the Identity Server at `https://localhost:7015`.



## Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss what you would like to change.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

