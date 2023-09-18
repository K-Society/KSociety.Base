// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Abstraction.Class
{
    public class DatabaseOptions
    {
        public DatabaseEngine DatabaseEngine { get; set; }
        public string? ConnectionString { get; set; }
        public bool Logging { get; set; }
        public string? MigrationsAssembly { get; set; }
        public bool LazyLoading { get; set; }

        public DatabaseOptions()
        {

        }
    }
}
