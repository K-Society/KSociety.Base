﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
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

        private static bool ShouldUseConstructorParameters(Type type)
        {
            return true;
        }

        private static ConstructorInfo GetConstructor(GetConstructorArgs args)
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