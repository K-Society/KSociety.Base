using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace KSociety.Base.InfraSub.Test.ClassMap;
public class CodeDomTypeEnumConverter<T> : DefaultTypeConverter where T : struct
{
    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        //T result;
        if (Enum.TryParse<T>(value.ToString(), out var result))
        {
            return Convert.ToInt32(result).ToString();
        }

        throw new InvalidCastException(String.Format("Invalid value to EnumConverter. Type: {0} Value: {1}", typeof(T), value));
    }
}
