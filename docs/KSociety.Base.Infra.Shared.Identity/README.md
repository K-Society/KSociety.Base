![Logo](https://raw.githubusercontent.com/k-society/KSociety.Base/master/docs/K-Society__Logo_vs-negative.png)

[![build status](https://img.shields.io/github/actions/workflow/status/K-Society/KSociety.Base/build.yml?branch=develop)](https://github.com/K-Society/KSociety.Base/actions/workflows/build.yml?query=branch%3Adevelop) [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InfraSub.Shared)](https://www.nuget.org/profiles/K-Society)

[KSociety.Base Home](https://github.com/K-Society/KSociety.Base)

# KSociety.Base.Infra.Shared.Identity

The infrastructure layer, data access layer (DAL).

## Introduction

The infrastructure layer, data access layer (DAL).

### KSociety.Base.Infra.Shared.Identity
The infrastructure layer, data access layer (DAL).
It also contains the implementation of the UnitOfWork.
Supported databases:
1. SqlServer
2. Sqlite
3. MySql
4. PostgreSQL

### Example of use
Refer to the following [example repository](https://github.com/K-Society/KSociety.Example).
Refer to the following [readme](https://github.com/K-Society/KSociety.Example/tree/master/docs/KSociety.Example.Infra.DataAccess).

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Infra.Shared](https://github.com/K-Society/KSociety.Base/tree/master/src/01/KSociety.Base.Infra.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Infra.Shared)](https://www.nuget.org/packages/KSociety.Base.Infra.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Infra.Shared) |

## License
The project is under Microsoft Reciprocal License [(MS-RL)](http://www.opensource.org/licenses/MS-RL)

## Dependencies

List of technologies, frameworks and libraries used for implementation:

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) (platform). Note for Visual Studio users: **VS 2022** is required.
- [Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies) (Lazy loading proxies for Entity Framework Core.)
- [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite) (SQLite database provider for Entity Framework Core.)
- [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json) (JSON configuration provider implementation for Microsoft.Extensions.Configuration.)
- [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) (Console logger provider implementation for Microsoft.Extensions.Logging.)
- [Microsoft.AspNetCore.Identity.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore) (ASP.NET Core Identity provider that uses Entity Framework Core.)
- [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) (Entity Framework Core is a modern object-database mapper for .NET.)
- [Microsoft.EntityFrameworkCore.Relational](https://www.nuget.org/packages?q=Microsoft.EntityFrameworkCore.Relational) (Shared Entity Framework Core components for relational database providers.)
- [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer) (Microsoft SQL Server database provider for Entity Framework Core.)
- [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL) (Npgsql.EntityFrameworkCore.PostgreSQL is the open source EF Core provider for PostgreSQL.)
- [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql) (Pomelo's MySQL database provider for Entity Framework Core.)