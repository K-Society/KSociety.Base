using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

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
        Map(map => map.Parameter);
        Map(map => map.Description);
        Map(map => map.Decoration).TypeConverter<ProtoBufTypeConverter>();
        Map(map => map.Tag).Default(0);
        Map(map => map.Parameters).Convert(row =>
        {
            var dictionary = new Dictionary<string, Type>();
            try
            {
                var columnValue = row.Row.GetField<string>("Parameters");
                var list = columnValue?.Split(',').ToList() ?? new List<string>();
                foreach (var parameter in list)
                {
                    var parameterArray = parameter.Split(' ');
                    if (parameterArray.Length == 2)
                    {
                        dictionary.Add(parameterArray[1], Type.GetType(parameterArray[0]));
                    }

                }

                //return dictionary;
            }
            catch (Exception ex)
            {
                ;
            }

            return dictionary;
        });
    }
}
