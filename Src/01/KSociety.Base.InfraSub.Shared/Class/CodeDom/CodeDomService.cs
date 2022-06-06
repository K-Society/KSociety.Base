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

    //public void AddBaseClass(string[] components)
    //{
    //    try
    //    {

    //        _class = CodeDomHelper.GetClass(components[0]);
    //        _class.IsClass = true;
    //        _class.IsPartial = false;
    //        _class.Comments.AddRange(
    //            CodeDomHelper.GetSummaryComments(
    //                components[1]));
    //        _class.Attributes = MemberAttributes.Public;
    //        //this.mClass.BaseTypes.Add(new CodeTypeReference("NomeClassePadre"));
    //        //this.mClass.BaseTypes.Add(new CodeTypeReference("InterfacciaImplementata"));
    //        _class.BaseTypes.Add(new CodeTypeReference("IRequest"));

    //        if (_codeNamespace != null)
    //        {
    //            _codeNamespace.Types.Add(_class);
    //            //_class.Members.Add(CodeDomHelper.GetMClassName());
    //            _constructor = CodeDomHelper.GetConstructorBase(MemberAttributes.Public, components[1]);

    //            _class.Members.Add(_constructor);
    //        }

    //    }
    //    catch (Exception ex)
    //    {

    //    }

    //}

    public void AddBaseClass(string className, string description, Type decoration)
    {
        try
        {

            _class = CodeDomHelper.GetClass(className);
            _class.IsClass = true;
            _class.IsPartial = false;
            _class.Comments.AddRange(
                CodeDomHelper.GetSummaryComments(description));
            _class.Attributes = MemberAttributes.Public;
            //this.mClass.BaseTypes.Add(new CodeTypeReference("NomeClassePadre"));
            //this.mClass.BaseTypes.Add(new CodeTypeReference("InterfacciaImplementata"));
            _class.BaseTypes.Add(new CodeTypeReference("IRequest"));

            var attr = new CodeAttributeDeclaration(new CodeTypeReference(decoration));
            //var attr = new CodeAttributeDeclaration();
            _class.CustomAttributes.Add(attr);
            _codeNamespace?.Types.Add(_class);
            //_class.Members.Add(CodeDomHelper.GetMClassName());
            //_constructor = CodeDomHelper.GetConstructorBase(MemberAttributes.Public, description);

            //_class.Members.Add(_constructor);

        }
        catch (Exception ex)
        {

        }

    }

    public void AddConstructor(string description, Dictionary<string, Type> parameters = null)
    {
        if (_codeNamespace != null)
        {
            _constructor = CodeDomHelper.GetConstructorBase(MemberAttributes.Public, description);

            if (parameters is not null)
            {
                foreach (var parameter in parameters)
                {
                    _constructor.Parameters.Add(new CodeParameterDeclarationExpression(parameter.Value, parameter.Key));
                    CodeExpression val = CodeDomHelper.GetPlainCode(parameter.Key);
                    _constructor.Statements.Add(CodeDomHelper.GetFieldVariableAssignment("_"+ parameter.Key, val));
                }
            }
            _class.Members.Add(_constructor);
        }
    }

    //public void AddField(string[] components)
    //{

    //}

    public void AddField(string fieldName, Type dataType, string description)
    {
        //_class.Members.Add(CodeDomHelper.GetF(propertyName, propertyField, dataType, description, true, true));

        //CodeExpression val = CodeDomHelper.GetPlainCode(dataType.FullName);
        //if (components[1].ToLower().Contains("string"))
        //{
        //    val = CodeDomHelper.GetPlainCode("string.empty");
        //}
        //if (components[1].ToLower().Contains("datetime"))
        //{
        //    val = CodeDomHelper.GetPlainCode("DateTime.Now");
        //}

        _class.Members.Add(CodeDomHelper.GetFieldVariable(fieldName,
            dataType.FullName, MemberAttributes.Private,
            description, null));

        //_constructor.Statements.Add(CodeDomHelper.GetFieldVariableAssignment(
        //    fieldName, val));
    }

    //public void AddProperty(string[] components)
    //{

    //    Type tipo = Type.GetType(components[2]);

    //    _class.Members.Add(CodeDomHelper.GetProperty(
    //        components[0], components[2], tipo, components[3], true, true));
    //}

    public void AddProperty(string propertyName, string propertyField, Type dataType, string description, Type decoration, int tag)
    {
        _class.Members.Add(CodeDomHelper.GetProperty(propertyName, propertyField, dataType, description, true, true, decoration, tag));
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
                    AddBaseClass(item.Value, item.Description, item.Decoration);
                    break;

                case CodeDomType.Constructor:
                    AddConstructor(item.Description, item.Parameters);
                    break;

                case CodeDomType.Field:
                    AddField(item.Value, item.DataType, item.Description);
                    break;

                case CodeDomType.Property:
                    AddProperty(item.Value, item.Parameter, item.DataType, item.Description, item.Decoration, item.Tag);
                    break;

                default:
                    break;
            }
        }
    }
}
