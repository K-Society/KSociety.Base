// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap;

public sealed class TestClass : CsvHelper.Configuration.ClassMap<Csv.Dto.TestClass>
{
    public TestClass()
    {
        this.Map(map => map.Id).Name("id");
        this.Map(map => map.ClassTypeId).Name("classTypeId");
        this.Map(map => map.Name).Name("name");
        this.Map(map => map.Ip).Name("ip");
        this.Map(map => map.Enable).Name("enable");
        this.Map(map => map.Ahh).Name("ahh");
    }
}
