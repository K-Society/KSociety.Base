using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace KSociety.Base.InfraSub.Shared.Class.CodeDom;
public static class CodeDomHelper
{
    /// <summary>
    /// Genera il file C# contenente la classe definita dal codedom composto
    /// nei namespaces passati come parametro
    /// </summary>
    /// <param name="fileName">Nome del file</param>
    /// <param name="usingNamespace">Namespace contenente gli imports</param>
    /// <param name="codeNamespace">Namespace contenente il codice</param>
    public static void GenerateCSharpCode(string fileName,
        CodeNamespace usingNamespace, CodeNamespace codeNamespace)
    {

        //Predispone il provider
        CSharpCodeProvider provider = new();

        //Predispone le opzioni di formattazione della classe
        CodeGeneratorOptions codeOpts = new()
        {
            BracingStyle = "C", 
            IndentString = "\t", 
            VerbatimOrder = true, // Usare true una volta costruito tutto x avere l'ordine giusto
            BlankLinesBetweenMembers = true
        };

        FileInfo fInfo = new(fileName);
        DirectoryInfo dInfo = new(fInfo.DirectoryName);
        if (!dInfo.Exists)
        {
            dInfo.Create();
        }

        //Genera il codedom container
        CodeCompileUnit unit = GenerateCodeUnit(usingNamespace, codeNamespace);

        //Crea uno stream writer per il file di output
        IndentedTextWriter tw = new(
            new StreamWriter(fileName, false), "\t");

        //genera il codice sorgente usando lo stream e le opzioni predisposte
        provider.GenerateCodeFromCompileUnit(unit, tw, codeOpts);
        //Chiude il file di output
        tw.Close();

    }

    /// <summary>
    /// Genera l'oggetto contenitore per il codice
    /// </summary>
    /// <param name="usingNamespace">The p using namespace.</param>
    /// <param name="codeNamespace">The p code namespace.</param>
    /// <returns></returns>
    public static CodeCompileUnit GenerateCodeUnit(
        CodeNamespace usingNamespace, CodeNamespace codeNamespace)
    {
        CodeCompileUnit codeUnit = new();

        codeUnit.Namespaces.Add(usingNamespace);
        codeUnit.Namespaces.Add(codeNamespace);
        return (codeUnit);
    }

    /// <summary>
    /// Genera un namespace con un nome oppure un namespace vuoto
    /// il namespace vuoto serve per generare le clausole using (Imports) per la classe
    /// che devono essere posizionate fuori dal namespace.
    /// </summary>
    /// <param name="theNamespace">The namespace.</param>
    /// <returns></returns>
    public static CodeNamespace GetNamespace(string theNamespace)
    {
        if (theNamespace != null)
        {
            return new CodeNamespace(theNamespace);
        }
        else
        {
            return new CodeNamespace();
        }
    }

    /// <summary>
    /// Genera il namespace vuoto x le using (Imports)
    /// </summary>
    /// <returns></returns>
    public static CodeNamespace GetNamespace()
    {
        return (GetNamespace(null));

    }

    /// <summary>
    /// Genera una clausola Using (Imports)
    /// </summary>
    /// <param name="namespaceToImport">The namespace to import.</param>
    /// <returns>CodeNamespaceImport</returns>
    public static CodeNamespaceImport GetUsing(string namespaceToImport)
    {
        return (new CodeNamespaceImport(namespaceToImport));
    }

    /// <summary>
    /// Genera una classe
    /// </summary>
    /// <param name="className">Name of the p class.</param>
    /// <returns></returns>
    public static CodeTypeDeclaration GetClass(string className)
    {
        return (new CodeTypeDeclaration(className));
    }

    /// <summary>
    /// Genera un costruttore senza parametri con un commento descrittivo e 
    /// il tipo di accesso (private, public ecc.).
    /// </summary>
    /// <param name="access">Tipo di accesso</param>
    /// <param name="description">Descrizione</param>
    /// <returns></returns>
    public static CodeConstructor GetConstructorBase(MemberAttributes access, string description)
    {
        CodeConstructor cst = new() {Attributes = access};
        cst.Comments.AddRange(GetSummaryComments(description));

        return cst;
    }

    public static CodeMemberField GetMClassName()
    {
        CodeMemberField fld = new(typeof(string), "mClassName")
        {
            Attributes = MemberAttributes.Private | MemberAttributes.Static, 
            InitExpression = GetMClassNameExpression()
        };
        fld.Comments.AddRange(GetSummaryComments("Indica il nome della classe predisposta per la gestione delle eccezioni"));
        return fld;
    }

    public static CodeSnippetExpression GetMClassNameExpression()
    {
        return GetPlainCode("System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name");
    }

    /// <summary>
    /// Genera un XML Summary Comment
    /// </summary>
    /// <param name="description">Descrizione da porre nel commento</param>
    /// <returns>
    /// Collezione di commenti contenente il commento
    /// </returns>
    public static CodeCommentStatementCollection GetSummaryComments(string description)
    {
        CodeCommentStatementCollection summary = new()
        {
            new CodeCommentStatement("<summary>", true), 
            new CodeCommentStatement(description, true),
            new CodeCommentStatement("</summary>", true)
        };
        return summary;
    }

    /// <summary>
    /// Genera una property base
    /// </summary>
    /// <param name="pName">Nome della Property</param>
    /// <param name="pField">Nome del campo</param>
    /// <param name="pType">Tipo della property</param>
    /// <param name="pDescription">Descrizione per il commento</param>
    /// <returns></returns>
    public static CodeMemberProperty GetProperty(string pName, string pField, Type pType, string pDescription,
        bool pHasGet, bool pHasSet)
    {
        CodeMemberProperty prop = new CodeMemberProperty();
        prop.Name = pName;
        prop.Comments.AddRange(GetSummaryComments(pDescription));
        prop.Attributes = MemberAttributes.Public;
        prop.Type = new CodeTypeReference(pType);
        prop.HasGet = pHasGet;
        prop.HasSet = pHasSet;

        if (pHasGet)
        {
            prop.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeArgumentReferenceExpression(pField)));
        } // if
        if (pHasSet)
        {
            prop.SetStatements.Add(
                GetFieldVariableAssignment(
                    pField, GetPlainCode("value")));
        } // if
        return prop;

    }

    /// <summary>
    /// Genera il codice per assegnare un valore ad una variabile
    /// </summary>
    /// <param name="pFieldName">Nome del campo</param>
    /// <param name="pValue">Valore in forma di espressione di codice</param>
    /// <returns></returns>
    public static CodeAssignStatement GetFieldVariableAssignment(
        string pFieldName, CodeExpression pValue)
    {
        return (new CodeAssignStatement(new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), pFieldName), pValue));

    }

    /// <summary>
    /// Trasforma una stringa in codice da inserire dove occorre
    /// </summary>
    /// <param name="pCode">Stringa di codice</param>
    /// <returns></returns>
    public static CodeSnippetExpression GetPlainCode(string pCode)
    {
        return (new CodeSnippetExpression(pCode));
    }
}
