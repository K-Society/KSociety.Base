using System;
using System.Collections.Generic;

namespace KSociety.Base.InfraSub.Shared.Class.CodeDom;
public class ClassGenerator
{
    public CodeDomType CodeDomType { get; set; }
    public string Value { get; set; }
    public Type DataType { get; set; }
    public string Parameter { get; set; }
    public string Description { get; set; }
    public Type Decoration { get; set; }
    public int Tag { get; set; }
    public Dictionary<string, Type> Parameters { get; set; }

    public ClassGenerator()
    {

    }

    public ClassGenerator(
        CodeDomType codeDomType, 
        string value, 
        Type dataType, 
        string parameter, 
        string description, 
        Type decoration, 
        int tag,
        Dictionary<string, Type> parameters)
    {
        CodeDomType = codeDomType;
        Value = value;
        DataType = dataType;
        Parameter = parameter;
        Description = description;
        Decoration = decoration;
        Tag = tag;
        Parameters = parameters;
    }
}
