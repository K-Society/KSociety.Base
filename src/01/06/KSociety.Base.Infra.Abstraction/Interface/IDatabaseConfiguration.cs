// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Abstraction.Interface
{
    using Class;

    /// <summary>
    /// The Database configuration.
    /// </summary>
    public interface IDatabaseConfiguration
    {
        DatabaseEngine DatabaseEngine { get; }
        string? ConnectionString { get; }
        bool Logging { get; }
        string? MigrationsAssembly { get; }
        bool LazyLoading { get; }
        string ToString();
    }
}
