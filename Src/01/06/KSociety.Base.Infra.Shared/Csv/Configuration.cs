using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class Configuration
    {
        //Entity class no public parameterless constructor
        public static CsvConfiguration CsvConfiguration => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            PrepareHeaderForMatch = (header, index) => header.ToLower(),
            MemberTypes = MemberTypes.None,
            //PrepareHeaderForMatch = header => header.ToLower(),
            //HasHeaderRecord = true,
            //GetConstructor = type => type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First(),
            GetConstructor = type => type.GetConstructors().First()
            //ShouldUseConstructorParameters = type => true
            //IncludePrivateMembers = true,
        };
    }
}
