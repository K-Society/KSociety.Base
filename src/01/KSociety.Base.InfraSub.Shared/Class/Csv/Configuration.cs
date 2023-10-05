// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public static class Configuration
    {
        public static CsvConfiguration CsvConfiguration => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            PrepareHeaderForMatch = header => header.Header.ToLower(),
            GetConstructor = GetConstructor
        };

        public static CsvConfiguration CsvConfigurationWrite => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            PrepareHeaderForMatch = (header) => header.Header.ToLower(),
            GetConstructor = GetConstructor
        };

        private static bool ShouldUseConstructorParameters(Type _)
        {
            return true;
        }

        #if NETSTANDARD2_0
        private static ConstructorInfo GetConstructor(GetConstructorArgs args)
        #elif NETSTANDARD2_1
        private static ConstructorInfo GetConstructor(GetConstructorArgs args)
        #endif
        {
            try
            {
                var result = args.ClassType.GetConstructors().FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                ;
            }

            return null;
        }
    }
}
