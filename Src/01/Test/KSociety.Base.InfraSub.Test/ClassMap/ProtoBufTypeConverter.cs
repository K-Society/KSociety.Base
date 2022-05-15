using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace KSociety.Base.InfraSub.Test.ClassMap;
public class ProtoBufTypeConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        Type output = null;
        switch (text)
        {
            case "ProtoBuf.ProtoContractAttribute":
                output = typeof(ProtoBuf.ProtoContractAttribute);
                break;

            case "ProtoBuf.ProtoMemberAttribute":
                output = typeof(ProtoBuf.ProtoMemberAttribute);
                break;
        }

        return output;

        //return Assembly.GetType(text, );
    }

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        //var test = (typeof(value)).FullName;
        return ((Type)value).FullName;
    }
}
