using Microsoft.EntityFrameworkCore.Migrations;

namespace KSociety.Base.Infra.Shared.Class
{
    public static class CreateView
    {
        public static MigrationBuilder CreateViewFromSql(
            this MigrationBuilder migrationBuilder,
            string assemblyName,
            string resourceSqlFileName)
        {
            migrationBuilder.Operations.Add(
                new CreateViewOperation
                {
                    AssemblyName = assemblyName,
                    ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
                });

            return migrationBuilder;
        }
    }
}
