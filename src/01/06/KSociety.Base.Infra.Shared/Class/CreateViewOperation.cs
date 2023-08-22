// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using Microsoft.EntityFrameworkCore.Migrations.Operations;

    public class CreateViewOperation : MigrationOperation
    {
        public string AssemblyName { get; set; }

        public string ResourceSqlFileName { get; set; }
    }
}