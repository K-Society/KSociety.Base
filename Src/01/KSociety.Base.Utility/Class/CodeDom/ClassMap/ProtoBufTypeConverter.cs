using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace KSociety.Base.Utility.Class.CodeDom.ClassMap
{
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

                case "ProtoBuf.CompatibilityLevelAttribute":
                    output = typeof(ProtoBuf.CompatibilityLevelAttribute);
                    break;
            }

            return output;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((Type)value).FullName;
        }
    }
}