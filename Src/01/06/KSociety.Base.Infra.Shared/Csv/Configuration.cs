using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class Configuration
    {
        public static CsvConfiguration CsvConfiguration => new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",

            PrepareHeaderForMatch = (header, index) => header.ToLower(),

            GetConstructor = GetConstructor

        };

        public static CsvConfiguration CsvConfigurationWrite => new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",

            PrepareHeaderForMatch = (header, index) => header.ToLower(),

            GetConstructor = GetConstructor

        };

        private static bool ShouldUseConstructorParameters(Type type)
        {
            return true;
            //if (!type.IsValueType)
            //{
            //    return type.HasConstructor();
            //}

            //return false;

            //if (type.Name.Equals("DtoTestClass5"))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            try
            {
                var result = type.GetConstructors().FirstOrDefault();
                
                return result;
            }
            catch (Exception ex)
            {
                ;
            }

            return null;
        }
    }
}
