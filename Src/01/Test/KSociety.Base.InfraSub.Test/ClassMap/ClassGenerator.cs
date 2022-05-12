using CsvHelper.Configuration;

namespace KSociety.Base.InfraSub.Test.ClassMap;

public sealed class ClassGenerator : ClassMap<KSociety.Base.InfraSub.Shared.Class.CodeDom.ClassGenerator>
{
    public ClassGenerator()
    {
        //var converter = new EnumConverter(typeof(KSociety.Base.InfraSub.Shared.Class.CodeDom.CodeDomType));

        //var propertyMapData = new MemberMapData(null)
        //{
        //    TypeConverterOptions = { EnumIgnoreCase = true },
        //};

        //Map(map => converter.ConvertFromString(map.CodeDomType.ToString(), null, propertyMapData)); //.TypeConverter<KSociety.Base.InfraSub.Shared.Class.CodeDom.CodeDomType>().Convert()
        //.TypeConverter<CodeDomTypeEnumConverter<KSociety.Base.InfraSub.Shared.Class.CodeDom.CodeDomType>>();
        Map(map => map.CodeDomType);
        Map(map => map.Value);
        Map(map => map.DataType).TypeConverter<DataTypeConverter>();
        Map(map => map.Parameters);
        Map(map => map.Description);
    }
}
