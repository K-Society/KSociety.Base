namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap
{
    public sealed class TestClassPrivateSetter : CsvHelper.Configuration.ClassMap<Csv.Dto.TestClassPrivateSetter>
    {
        public TestClassPrivateSetter()
        {
            Map(map => map.Id);
            Map(map => map.ClassTypeId);
            Map(map => map.Name);
            Map(map => map.Enable);
            Map(map => map.Ahh);
        }
    }
}
