// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv;
public class DtoTestClass5
{
    public int ClassTypeId { get; private set; }

    public string Parameter { get; private set; }

    public DtoTestClass5(/*Int32 classTypeId*/ string parameter)
    {
        this.ClassTypeId = 2;
        this.Parameter = parameter;
    }

    public DtoTestClass5()
    {

    }
}
