namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap;

public sealed class TestClassParameterlessConstructor : CsvHelper.Configuration.ClassMap<Csv.Dto.TestClassParameterlessConstructor>
{
    public TestClassParameterlessConstructor()
    {
        Map(map => map.Id);
        Map(map => map.ClassTypeId);
        Map(map => map.Name);
        Map(map => map.Ip);
        Map(map => map.Enable);
        Map(map => map.Ahh);
    }
}