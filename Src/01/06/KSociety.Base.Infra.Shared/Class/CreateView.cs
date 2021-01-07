using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace KSociety.Base.Infra.Shared.Class
{
    public static class CreateView
    {
        //public static MigrationBuilder CreateViewFromSql(
        //    this MigrationBuilder migrationBuilder,
        //    string assemblyName,
        //    string resourceSqlFileName)
        //{
        //    //if (migrationBuilder.IsSqlServer())
        //    //{
        //        migrationBuilder.Operations.Add(
        //            new CreateViewOperation
        //            {
        //                AssemblyName = assemblyName, 
        //                ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
        //            });

        //        return migrationBuilder;
        //    //}

        //    //migrationBuilder..Operations.Add(
        //    //    new CreateViewOperation
        //    //    {
        //    //        AssemblyName = assemblyName,
        //    //        ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
        //    //    });

        //    //return migrationBuilder;

        //    //switch (migrationBuilder.ActiveProvider)
        //    //{
        //    //    case "Microsoft.EntityFrameworkCore.SqlServer":
        //    //        return migrationBuilder
        //    //            .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");

        //    //    case "Microsoft.EntityFrameworkCore.Sqlite":
        //    //        return migrationBuilder
        //    //            .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");

        //    //    case "Npgsql.EntityFrameworkCore.PostgreSQL":
        //    //        return migrationBuilder
        //    //            .Sql($"CREATE USER {name} WITH PASSWORD '{password}';");
        //    //}
        //}

        public static OperationBuilder<CreateViewOperation> CreateViewFromSql(
            this MigrationBuilder migrationBuilder,
            string assemblyName,
            string resourceSqlFileName)
        {
            //if (migrationBuilder.IsSqlServer())
            //{

            var operation = new CreateViewOperation
            {
                AssemblyName = assemblyName, ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
            };

            migrationBuilder.Operations.Add(operation);

            //return migrationBuilder;

            return new OperationBuilder<CreateViewOperation>(operation);

            //}

            //migrationBuilder..Operations.Add(
            //    new CreateViewOperation
            //    {
            //        AssemblyName = assemblyName,
            //        ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
            //    });

            //return migrationBuilder;

            //switch (migrationBuilder.ActiveProvider)
            //{
            //    case "Microsoft.EntityFrameworkCore.SqlServer":
            //        return migrationBuilder
            //            .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");

            //    case "Microsoft.EntityFrameworkCore.Sqlite":
            //        return migrationBuilder
            //            .Sql($"CREATE USER {name} WITH PASSWORD = '{password}';");

            //    case "Npgsql.EntityFrameworkCore.PostgreSQL":
            //        return migrationBuilder
            //            .Sql($"CREATE USER {name} WITH PASSWORD '{password}';");
            //}
        }
    }
}
