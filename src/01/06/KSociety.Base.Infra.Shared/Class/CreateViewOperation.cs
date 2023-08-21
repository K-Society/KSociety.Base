namespace KSociety.Base.Infra.Shared.Class
{
    using Microsoft.EntityFrameworkCore.Migrations.Operations;

    public class CreateViewOperation : MigrationOperation
    {
        public string AssemblyName { get; set; }

        public string ResourceSqlFileName { get; set; }
    }
}