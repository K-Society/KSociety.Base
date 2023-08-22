// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

    public static class CreateView
    {
        public static OperationBuilder<CreateViewOperation> CreateViewFromSql(
            this MigrationBuilder migrationBuilder,
            string assemblyName,
            string resourceSqlFileName)
        {

            var operation = new CreateViewOperation
            {
                AssemblyName = assemblyName, ResourceSqlFileName = @"." + resourceSqlFileName + @".sql"
            };

            migrationBuilder.Operations.Add(operation);

            return new OperationBuilder<CreateViewOperation>(operation);
        }
    }
}