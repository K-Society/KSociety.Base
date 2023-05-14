![Logo](https://raw.githubusercontent.com/k-society/KSociety.Base/master/docs/K-Society__Logo_vs-negative.png)

[![build status](https://img.shields.io/github/actions/workflow/status/K-Society/KSociety.Base/build.yml?branch=develop)](https://github.com/K-Society/KSociety.Base/actions/workflows/build.yml?query=branch%3Adevelop) [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.InfraSub.Shared)](https://www.nuget.org/profiles/K-Society)

[KSociety.Base Home](https://github.com/K-Society/KSociety.Base)

# KSociety.Base.App.Utility

KSociety.Base.App.Utility is a library shared among other stack libraries.

## Introduction

The application layer, abstraction on the command handler library. Contains the abstraction of the request handlers.

### KSociety.Base.App.Utility
The application layer, abstraction on the command handler library. Contains the abstraction of the request handlers.

### Example of use
Refer to the following [example repository](https://github.com/K-Society/KSociety.Example).
Refer to the following [readme](https://github.com/K-Society/KSociety.Example/tree/master/docs/KSociety.Example.App.Dto).
Refer to the following [readme](https://github.com/K-Society/KSociety.Example/tree/master/docs/KSociety.Example.App.ReqHdlr).

| GitHub Repository | NuGet | Download |
| ------------- | ------------- | ------------- |
| [KSociety.Base.App.Utility](https://github.com/K-Society/KSociety.Base/tree/master/Src/01/KSociety.Base.App.Utility) | [![NuGet](https://img.shields.io/nuget/v/KSociety.Base.App.Utility)](https://www.nuget.org/packages/KSociety.Base.App.Utility) | ![NuGet](https://img.shields.io/nuget/dt/KSociety.Base.App.Utility) |

## License
The project is under Microsoft Reciprocal License [(MS-RL)](http://www.opensource.org/licenses/MS-RL)

## Dependencies

List of technologies, frameworks and libraries used for implementation:

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) (platform). Note for Visual Studio users: **VS 2022** is required.
- [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/) (Logging abstractions for Microsoft.Extensions.Logging.)
- [protobuf-net](https://github.com/protobuf-net/protobuf-net) (Provides simple access to fast and efficient "Protocol Buffers" serialization from .NET applications)