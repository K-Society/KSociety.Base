namespace KSociety.Base.InfraSub.Shared.Class.CodeDom;
public class ClassGenerator
{
    public CodeDomType CodeDomType { get; set; }
    public string Value { get; set; }
    public string Parameters { get; set; }

    public ClassGenerator()
    {

    }

    public ClassGenerator(CodeDomType codeDomType, string value, string parameters)
    {
        CodeDomType = codeDomType;
        Value = value;
        Parameters = parameters;
    }
}
