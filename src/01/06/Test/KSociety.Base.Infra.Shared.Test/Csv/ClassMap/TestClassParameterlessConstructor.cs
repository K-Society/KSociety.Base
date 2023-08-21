// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv.ClassMap;

public sealed class TestClassParameterlessConstructor : CsvHelper.Configuration.ClassMap<Csv.Dto.TestClassParameterlessConstructor>
{
    public TestClassParameterlessConstructor()
    {
        this.Map(map => map.Id);
        this.Map(map => map.ClassTypeId);
        this.Map(map => map.Name);
        this.Map(map => map.Ip);
        this.Map(map => map.Enable);
        this.Map(map => map.Ahh);
    }
}
