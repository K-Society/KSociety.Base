namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap;

public sealed class TestClass : CsvHelper.Configuration.ClassMap<Csv.Dto.TestClass>
{
    public TestClass()
    {
        Map(map => map.Id).Name("id");
        Map(map => map.ClassTypeId).Name("classTypeId");
        Map(map => map.Name).Name("name");
        Map(map => map.Ip).Name("ip");
        Map(map => map.Enable).Name("enable");
        Map(map => map.Ahh).Name("ahh");
    }
}