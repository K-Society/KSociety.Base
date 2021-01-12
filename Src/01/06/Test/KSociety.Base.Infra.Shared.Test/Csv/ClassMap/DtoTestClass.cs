namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap
{
    public sealed class DtoClassMap : CsvHelper.Configuration.ClassMap<Csv.DtoTestClass>
    {
        public DtoClassMap()
        {
            Map(map => map.Id);
            Map(map => map.ClassTypeId);
            Map(map => map.Name);
            Map(map => map.Ip);
            Map(map => map.Enable);
            Map(map => map.Ahh);
        }
    }
}
