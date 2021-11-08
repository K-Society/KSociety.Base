[![Build status](https://ci.appveyor.com/api/projects/status/svxutqmffkucfp0r?svg=true)](https://ci.appveyor.com/project/maniglia/ksociety-base)

# KSociety.Base - Framework for microservices

K-Society Base is a full stack framework for .NET 6 application.

## Introduction

KSociety.Base is a .NET 6.0 framework that can be used to create a clean design by enforcing single responsibility and separation of concerns.
Its advanced features are ideal for Domain Driven Design (DDD), Command Query Responsibilty Segragation (CQRS) and Event Sourcing, is an open sourse 
.NET framework and represents the basic infrastructure. The whole framework is divided into layers listed below.

## Architecture

![Image of Architecture](https://github.com/K-Society/KSociety.Base/blob/experimental/docs/Architecture_view_for_KSociety.Base.png)

### 0. Install
The install utility library.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.InstallAction](https://github.com/K-Society/KSociety.Base/tree/master/Src/00/KSociety.Base.InstallAction) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InstallAction)](https://www.nuget.org/packages/KSociety.Base.InstallAction) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.InstallAction) |

### 1. Presentation
The presentation layer.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Pre.Model](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Model) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Model)](https://www.nuget.org/packages/KSociety.Base.Pre.Model) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Model) |

### 1.1 Presentation - Form
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Pre.Form.View](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Form.View) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Form.View)](https://www.nuget.org/packages/KSociety.Base.Pre.Form.View) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Form.View) |
| [KSociety.Base.Pre.Form.Presenter](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/01/KSociety.Base.Pre.Form.Presenter) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Pre.Form.Presenter)](https://www.nuget.org/packages/KSociety.Base.Pre.Form.Presenter) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Pre.Form.Presenter) |

### 2. Service
The service layer.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Srv.Agent](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Agent) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Agent)](https://www.nuget.org/packages/KSociety.Base.Srv.Agent) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Agent) |
| [KSociety.Base.Srv.Behavior](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Behavior) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Behavior)](https://www.nuget.org/packages/KSociety.Base.Srv.Behavior) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Behavior) |
| [KSociety.Base.Srv.Contract](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Society.Base.Srv.Contract) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Contract)](https://www.nuget.org/packages/KSociety.Base.Srv.Contract) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Contract) |
| [KSociety.Base.Srv.Dto](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Dto) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Dto)](https://www.nuget.org/packages/KSociety.Base.Srv.Dto) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Dto) |
| [KSociety.Base.Srv.Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/02/KSociety.Base.Srv.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Shared)](https://www.nuget.org/packages/KSociety.Base.Srv.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Shared) |

### 2.1 Service - Host
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Srv.Host.Shared](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/02/Host/KSociety.Base.Srv.Host.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Srv.Host.Shared)](https://www.nuget.org/packages/KSociety.Base.Srv.Host.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Srv.Host.Shared) |

### 3. Application
The application layer, abstraction on the command handler library.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.App.Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/03/KSociety.Base.App.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.App.Shared)](https://www.nuget.org/packages/KSociety.Base.App.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.App.Shared) |

### 4. Business
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |

### 5. Domain
The domain layer.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Domain.Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/05/KSociety.Base.Domain.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Domain.Shared)](https://www.nuget.org/packages/KSociety.Base.Domain.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Domain.Shared) |

### 6. Infrastructure
The infrastructure layer.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.Infra.Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/06/KSociety.Base.Infra.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.Infra.Shared)](https://www.nuget.org/packages/KSociety.Base.Infra.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.Infra.Shared) |

### EventBus
The event bus abstraction and [RbbitMQ](https://www.rabbitmq.com/) implementation.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.EventBus](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSociety.BaseEventBus/KSociety.Base.EventBus) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.EventBus)](https://www.nuget.org/packages/KSociety.Base.EventBus) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.EventBus) |
| [KSociety.Base.EventBus.RabbitMQ](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSociety.BaseEventBus/KSociety.Base.EventBusRabbitMQ) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.EventBusRabbitMQ)](https://www.nuget.org/packages/KSociety.Base.EventBusRabbitMQ) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.EventBusRabbitMQ) |

### InfrastructureSub
The generic shared library.
| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.InfraSub.Shared](https://github.com/K-Society/KSociety.Base/tree/develop/Src/01/KSociety.Base.InfraSub.Shared) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InfraSub.Shared)](https://www.nuget.org/packages/KSociety.Base.InfraSub.Shared) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.InfraSub.Shared) |

## License
The project is under Microsoft Reciprocal License [(MS-RL)](http://www.opensource.org/licenses/MS-RL)

## Dependencies

List of technologies, frameworks and libraries used for implementation:

- [.NET Core 5](https://dotnet.microsoft.com/download) (platform). Note for Visual Studio users: **VS 2022** is required.
- [Autofac](https://autofac.org/) (Inversion of Control Container)
- [IdentityServer4](http://docs.identityserver.io) (Authentication and Authorization)
- [Serilog](https://serilog.net/) (structured logging)
- [Quartz.NET](https://www.quartz-scheduler.net/) (background processing)
- [FluentValidation](https://fluentvalidation.net/) (data validation)
- [MediatR](https://github.com/jbogard/MediatR) (mediator implementation)
- [Polly](https://github.com/App-vNext/Polly) (Resilience and transient-fault-handling library)