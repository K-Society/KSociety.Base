using System;

namespace KSociety.Base.InfraSub.Shared.Class.CodeDom;
public class ClassGenerator
{
    public CodeDomType CodeDomType { get; set; }
    public string Value { get; set; }
    public Type DataType { get; set; }
    public string Parameters { get; set; }
    public string Description { get; set; }

    public ClassGenerator()
    {

    }

    public ClassGenerator(CodeDomType codeDomType, string value, Type dataType, string parameters, string description)
    {
        CodeDomType = codeDomType;
        Value = value;
        DataType = dataType;
        Parameters = parameters;
        Description = description;
        
    }
}
