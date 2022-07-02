using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace KSociety.Base.Utility.Class.CodeDom.ClassMap
{
    public class CodeDomTypeEnumConverter<T> : DefaultTypeConverter where T : struct
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            //T result;
            if (Enum.TryParse<T>(value.ToString(), out var result))
            {
                return Convert.ToInt32(result).ToString();
            }

            throw new InvalidCastException($"Invalid value to EnumConverter. Type: {typeof(T)} Value: {value}");
        }
    }
}
