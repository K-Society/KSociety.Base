using System;
using System.CodeDom;
using System.Collections.Generic;

namespace KSociety.Base.InfraSub.Shared.Class.CodeDom;
public class CodeDomService
{
    private readonly CodeNamespace _usingNamespace;
    private CodeNamespace _codeNamespace;
    private CodeTypeDeclaration _class;
    private CodeConstructor _constructor;

    public CodeDomService()
    {
        _usingNamespace = CodeDomHelper.GetNamespace();
    }

    public void GenerateClass(string fileName)
    {
        CodeDomHelper.GenerateCSharpCode(fileName, _usingNamespace, _codeNamespace);
    }

    public void AddCodeNamespace(string codeNamespace)
    {
        _codeNamespace = new CodeNamespace(codeNamespace);
    }

    public void AddUsing(string namespaceToImport)
    {
        _usingNamespace.Imports.Add(CodeDomHelper.GetUsing(namespaceToImport));
    }

    public void AddBaseClass(string[] components)
    {
        try
        {

            _class = CodeDomHelper.GetClass(components[0]);
            _class.IsClass = true;
            _class.IsPartial = false;
            _class.Comments.AddRange(
                CodeDomHelper.GetSummaryComments(
                    components[1]));
            _class.Attributes = MemberAttributes.Public;
            //this.mClass.BaseTypes.Add(new CodeTypeReference("NomeClassePadre"));
            //this.mClass.BaseTypes.Add(new CodeTypeReference("InterfacciaImplementata"));
            _class.BaseTypes.Add(new CodeTypeReference("IRequest"));

            if (_codeNamespace != null)
            {
                _codeNamespace.Types.Add(_class);
                //_class.Members.Add(CodeDomHelper.GetMClassName());
                _constructor = CodeDomHelper.GetConstructorBase(MemberAttributes.Public, components[1]);

                _class.Members.Add(_constructor);
            }

        }
        catch (Exception ex)
        {
            
        }

    }

    public void AddProperty(string[] components)
    {
        
        Type tipo = Type.GetType(components[2]);

        _class.Members.Add(CodeDomHelper.GetProperty(
            components[0], components[1], tipo, components[3], true, true));
    }

    public void Generator(IEnumerable<ClassGenerator> classGeneratorItems)
    {
        foreach (var item in classGeneratorItems)
        {
            switch (item.CodeDomType)
            {
                case CodeDomType.Using:
                    AddUsing(item.Value);
                    break;

                case CodeDomType.Namespace:
                    AddCodeNamespace(item.Value);
                    break;

                case CodeDomType.ClassName:
                    AddBaseClass(new[] { item.Value, item.Value });
                    break;

                case CodeDomType.Property:
                    //Property=Name;mName;System.String;Nome della persona
                    AddProperty(new[] { item.Value, item.Value, "System.Int32", "Test" });
                    break;

                default:
                    break;
            }
        }
    }
}
