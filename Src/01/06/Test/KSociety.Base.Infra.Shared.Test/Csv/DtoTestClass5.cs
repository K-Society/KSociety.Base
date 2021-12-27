using System;

namespace KSociety.Base.Infra.Shared.Test.Csv;

public class DtoTestClass5
{
    public Int32 ClassTypeId { get; private set; }

    public string Parameter { get; private set; }

    public DtoTestClass5(/*Int32 classTypeId*/ string parameter)
    {
        ClassTypeId = 2;
        Parameter = parameter;
    }

    public DtoTestClass5()
    {

    }
}