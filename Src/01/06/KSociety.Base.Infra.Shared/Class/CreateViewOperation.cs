using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KSociety.Base.Infra.Shared.Class
{
    public class CreateViewOperation : MigrationOperation
    {
        public string AssemblyName { get; set; }

        public string ResourceSqlFileName { get; set; }
    }
}