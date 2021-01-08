using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class Configuration
    {
        public static CsvConfiguration CsvConfiguration => new(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            PrepareHeaderForMatch = (header, index) => header.ToLower(),
            MemberTypes = MemberTypes.Properties,
            GetConstructor = type => type.GetConstructors().First()
        };
    }
}
