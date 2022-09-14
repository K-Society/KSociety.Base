[![Build status](https://ci.appveyor.com/api/projects/status/svxutqmffkucfp0r?svg=true)](https://ci.appveyor.com/project/maniglia/ksociety-base)

# KSociety.Base - Framework for microservices

K-Society Base is a full stack framework for .NET 6 application, ideal for implementing microservices.

## Introduction

KSociety.Base is a .NET 6.0 framework that can be used to create a clean design by enforcing single responsibility and separation of concerns.
Its advanced features are ideal for Domain Driven Design (DDD), Command Query Responsibilty Segragation (CQRS) and Event Sourcing, is an open sourse 
.NET framework and represents the basic infrastructure. The whole framework is divided into layers listed below.

## Architecture

![Image of Architecture](https://github.com/K-Society/KSociety.Base/blob/experimental/docs/Architecture_view_for_KSociety.Base.png)

### 0. Install (KSociety.Base.)
The install utility library.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [InstallAction](https://github.com/K-Society/KSociety.Base/tree/master/Src/00/KSociety.Base.InstallAction) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InstallAction)](https://www.nuget.org/packages/KSociety.Base.InstallAction) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.InstallAction) |

### 1. Presentation (KSociety.Base.Pre.)
The presentation layer.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Model](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Model) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Model)](https://www.nuget.org/packages/KSociety.Base.Pre.Model) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Model) |

### 1.1 Presentation - Form (KSociety.Base.Pre.Form.)
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [View](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Form.View) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Form.View)](https://www.nuget.org/packages/KSociety.Base.Pre.Form.View) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Form.View) |
| [Presenter](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Form.Presenter) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Form.Presenter)](https://www.nuget.org/packages/KSociety.Base.Pre.Form.Presenter) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Form.Presenter) |

### 2. Service (KSociety.Base.Srv.)
The service layer.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Agent](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Agent) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Agent)](https://www.nuget.org/packages/KSociety.Base.Srv.Agent) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Agent) |
| [Behavior](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Behavior) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Behavior)](https://www.nuget.org/packages/KSociety.Base.Srv.Behavior) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Behavior) |
| [Contract](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Society.Base.Srv.Contract) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Contract)](https://www.nuget.org/packages/KSociety.Base.Srv.Contract) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Contract) |
| [Dto](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Dto) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Dto)](https://www.nuget.org/packages/KSociety.Base.Srv.Dto) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Dto) |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Shared)](https://www.nuget.org/packages/KSociety.Base.Srv.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Shared) |

### 2.1 Service - Host (KSociety.Base.Srv.Host.)
Contains the autofac modules.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/02/Host/KSociety.Base.Srv.Host.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Host.Shared)](https://www.nuget.org/packages/KSociety.Base.Srv.Host.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Host.Shared) |

### 3. Application (KSociety.Base.App.)
The application layer, abstraction on the command handler library.
Contains the abstraction of the request handlers.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/03/KSociety.Base.App.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.App.Shared)](https://www.nuget.org/packages/KSociety.Base.App.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.App.Shared) |

### 4. Business
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |

### 5. Domain (KSociety.Base.Domain.)
The domain layer.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/05/KSociety.Base.Domain.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Domain.Shared)](https://www.nuget.org/packages/KSociety.Base.Domain.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Domain.Shared) |

### 6. Infrastructure (KSociety.Base.Infra.)
The infrastructure layer, data access layer (DAL).
It also contains the implementation of the UnitOfWork.
Supported databases:
1. SqlServer
2. Sqlite
3. MySql
4. PostgreSQL

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/06/KSociety.Base.Infra.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Infra.Shared)](https://www.nuget.org/packages/KSociety.Base.Infra.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Infra.Shared) |

### EventBus (KSociety.Base.)
The event bus abstraction and [RbbitMQ](https://www.rabbitmq.com/) implementation.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [EventBus](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSocietyBaseEventBus/KSociety.Base.EventBus) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.EventBus)](https://www.nuget.org/packages/KSociety.Base.EventBus) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.EventBus) |
| [EventBusRabbitMQ](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSocietyBaseEventBus/KSociety.Base.EventBusRabbitMQ) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.EventBusRabbitMQ)](https://www.nuget.org/packages/KSociety.Base.EventBusRabbitMQ) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.EventBusRabbitMQ) |

### InfrastructureSub (KSociety.Base.InfraSub.)
The generic shared library.

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSociety.Base.InfraSub.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InfraSub.Shared)](https://www.nuget.org/packages/KSociety.Base.InfraSub.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.InfraSub.Shared) |

## License
The project is under Microsoft Reciprocal License [(MS-RL)](http://www.opensource.org/licenses/MS-RL)

## Dependencies

List of technologies, frameworks and libraries used for implementation:

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) (platform). Note for Visual Studio users: **VS 2022** is required.
- [Autofac](https://autofac.org/) (Inversion of Control Container)
- [AutoMapper](https://automapper.org/) (A convention-based object-object mapper)
- [CsvHelper](https://joshclose.github.io/CsvHelper/) (A .NET library for reading and writing CSV files)
- [Grpc.Net.Client](https://github.com/grpc/grpc-dotnet) (.NET client for gRPC)
- [MediatR](https://github.com/jbogard/MediatR) (mediator implementation)
- [Polly](https://github.com/App-vNext/Polly) (Resilience and transient-fault-handling library)
- [protobuf-net](https://github.com/protobuf-net/protobuf-net) (protobuf-net is a contract based serializer for .NET code)
- [Quartz.NET](https://www.quartz-scheduler.net/) (Quartz Scheduling Framework for .NET)
- [RabbitMQ.Client](https://www.rabbitmq.com/dotnet.html) (The RabbitMQ .NET client is an implementation of an AMQP 0-9-1 client library for C#)
- [Serilog](https://serilog.net/) (structured logging)
