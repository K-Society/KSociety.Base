using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace KSociety.Base.Infra.Shared.Class
{
    public static class CreateView
    {
        public static OperationBuilder<CreateViewOperation> CreateViewFromSql(
            this MigrationBuilder migrationBuilder,
            string assemblyName,
            string resourceSqlFileName)
        {

            var operation = new CreateViewOperation
            {
                AssemblyName = assemblyName,
                ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
            };

            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<CreateViewOperation>(operation);
        }
    }
}
