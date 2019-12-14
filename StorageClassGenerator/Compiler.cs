using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Storage {


  public class Compiler {

    readonly Dictionary<string, ClassInfo> classes;
    readonly List<ClassInfo> parentChildTree;

    public Compiler() {
      classes = new Dictionary<string, ClassInfo>();
      parentChildTree = new List<ClassInfo>();
    }


    string? nameSpaceString;
    const string onlyAcceptableConsts = "should only contain properties and configuration constants for " +
            "MaxLineLenght, AreItemsUpdatable, AreItemsDeletable and IsCompactDuringDispose , but not";


    public void Parse(NamespaceDeclarationSyntax namespaceDeclaration, string fileName) {
      nameSpaceString = namespaceDeclaration.Name.GetText().ToString();
      //if (namespaceDeclaration.Members.Count!=1) {
      //  throw new GeneratorException($"{fileName} contains more than 1 xxx in namespace '{nameSpaceString}'.");
      //}
      foreach (var namespaceMember in namespaceDeclaration.Members) {
        var classDeclaration = namespaceMember as ClassDeclarationSyntax;
        if (classDeclaration is null) {
          throw new GeneratorException($"{fileName} contains not only class declarations in namespace '{nameSpaceString}'.");
        }
        var className = classDeclaration.Identifier.Text;
        string? classComment = getComment(classDeclaration.GetLeadingTrivia());
        var classInfo = new ClassInfo(className, classComment);
        classes.Add(className, classInfo);
        foreach (var classMember in classDeclaration.Members) {
          var field = classMember as FieldDeclarationSyntax;
          if (field is null) {
            throw new GeneratorException($"Class {className} schould contain only properties and configuration constants, but has '{classMember}'.");
          }

          string? propertyComment = getComment(field.GetLeadingTrivia());

          var isConst = false;
          foreach (var modifierToken in field.Modifiers) {
            if (modifierToken.Text=="const") {
              isConst = true;
              break;
            }
          }

          var variableDeclaration = field.Declaration as VariableDeclarationSyntax;
          if (variableDeclaration is null) {
            throw new GeneratorException($"Class {className} {onlyAcceptableConsts} '{field.Declaration}'.");
          }
          var propertyType = variableDeclaration.Type.ToString();
          if (isConst) {
            foreach (var variableDeclarator in variableDeclaration.Variables) {
              var constValue = variableDeclarator.Initializer?.Value as LiteralExpressionSyntax;
              if (constValue!=null) {
                if (variableDeclarator.Identifier.Text=="MaxLineLenght") {
                  if (constValue!=null) {
                    classInfo.SetMaxLineLength(int.Parse(constValue.Token.Text));
                    continue;
                  }
                } else if (variableDeclarator.Identifier.Text=="AreItemsUpdatable") {
                  if (constValue!=null) {
                    classInfo.SetAreItemsUpdatable(bool.Parse(constValue.Token.Text));
                    continue;
                  }
                } else if (variableDeclarator.Identifier.Text=="AreItemsDeletable") {
                  if (constValue!=null) {
                    classInfo.SetAreItemsDeletable(bool.Parse(constValue.Token.Text));
                    continue;
                  }
                } else if (variableDeclarator.Identifier.Text=="IsCompactDuringDispose") {
                  if (constValue!=null) {
                    classInfo.SetIsCompactDuringDispose(bool.Parse(constValue.Token.Text));
                    continue;
                  }
                }
              }
              throw new GeneratorException($"Class {className} {onlyAcceptableConsts} '{classMember}'.");
            }
          } else {
            foreach (var property in variableDeclaration.Variables) {
              classInfo.AddMember(property.Identifier.Text, propertyType, propertyComment);
            }
          }
        }
      }
    }


    private string? getComment(SyntaxTriviaList syntaxTriviaList) {
      string? comment = null;
      var leadingTrivia = syntaxTriviaList.ToString();
      if (leadingTrivia.Contains("///")) {
        var triviaLines = leadingTrivia.Split(Environment.NewLine);
        foreach (var line in triviaLines) {
          if (!string.IsNullOrWhiteSpace(line)) {
            comment += line.TrimStart() + Environment.NewLine;
          }
        }
      }
      return comment;
    }


    public void AnalyzeDependencies() {
      var topClasses = classes.Values.ToDictionary(c=>c.Name);
      foreach (var classInfo in classes.Values) {
        foreach (var memberInfo in classInfo.Members.Values) {
          if (memberInfo.MemberType==MemberTypeEnum.Parent) {
            if (!classes.TryGetValue(memberInfo.Name!, out memberInfo.LinkedClassInfo))
              throw new GeneratorException($"{classInfo} '{memberInfo}': can not find class {memberInfo.Name}.");
            classInfo.Parents.Add(memberInfo.LinkedClassInfo);
            memberInfo.LinkedClassInfo.Children.Add(classInfo);
            topClasses.Remove(classInfo.Name);
          } else if (memberInfo.MemberType==MemberTypeEnum.List) {
            if (!classes.TryGetValue(memberInfo.ChildTypeName!, out memberInfo.LinkedClassInfo))
              throw new GeneratorException($"{classInfo} '{memberInfo}': can not find class {memberInfo.ChildTypeName}.");
            if (!memberInfo.LinkedClassInfo.Members.ContainsKey(classInfo.Name)) {
              //guarantee that there is a property linking to the parent for each child class.
              throw new GeneratorException($"{classInfo} '{memberInfo}': has a List<{memberInfo.ChildTypeName}>. The corresponding " +
                $"property {classInfo.Name} is missing in the class {memberInfo.ChildTypeName}.");
            }
          }
        }
      };

      //create parent child tree
      foreach (var classInfo in topClasses.Values) {
        addPparentChildTree(classInfo);
      }
      foreach (var classInfo in classes.Values) {
        if (!classInfo.IsAddedToParentChildTree) throw new Exception();
      }
    }


    private void addPparentChildTree(ClassInfo classInfo) {
      if (!classInfo.IsAddedToParentChildTree && allParentsAreAddedToParentChildTree(classInfo)) {
        classInfo.IsAddedToParentChildTree = true;
        parentChildTree.Add(classInfo);
        foreach (var child in classInfo.Children) {
          addPparentChildTree(child);
        }
      }
    }


    private bool allParentsAreAddedToParentChildTree(ClassInfo childClass) {
      foreach (var parentClass in childClass.Parents) {
        if (!parentClass.IsAddedToParentChildTree) return false;

        //if (!allParentsAreAddedToParentChildTree(parentClass)) return false;
      }
      return true;
    }


    public void WriteContentToConsole() {
      foreach (var classInfo in classes.Values) {
        Console.WriteLine(classInfo);
        foreach (var memberInfo in classInfo.Members.Values) {
          Console.WriteLine("  " + memberInfo);
        }
        Console.WriteLine();
      }
    }


    public void WriteCodeFiles(DirectoryInfo targetDirectory, string context) {
      foreach (var classInfo in classes.Values) {
        var baseFileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.Name + ".base.cs";
        try {
          File.Delete(baseFileNameAndPath);
        } catch {
          throw new GeneratorException($"Cannot delete file {baseFileNameAndPath}.");
        }
        using (var streamWriter = new StreamWriter(baseFileNameAndPath)) {
          Console.WriteLine(classInfo.Name + ".base.cs");
          classInfo.WriteBaseCodeFile(streamWriter, nameSpaceString!, context);
        }

        var fileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.Name + ".cs";
        if (!new FileInfo(fileNameAndPath).Exists) {
          using var streamWriter = new StreamWriter(fileNameAndPath);
          Console.WriteLine(classInfo.Name + ".cs");
          classInfo.WriteCodeFile(streamWriter, nameSpaceString!);
        }
      }
    }


    public void WriteContextFile(DirectoryInfo targetDirectory, string context) {
      var fileNameAndPath = targetDirectory!.FullName + '\\' + context + ".base.cs";
      try {
        File.Delete(fileNameAndPath);
      } catch {
        throw new GeneratorException($"Cannot delete file {fileNameAndPath}.");
      }
      using var streamWriter = new StreamWriter(fileNameAndPath);
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Threading;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine($"namespace {nameSpaceString} {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine("  /// <summary>");
      streamWriter.WriteLine($"  /// A part of {context} is static, which gives easy access to all stored data (=context) through {context}.Data. But most functionality is in the");
      streamWriter.WriteLine($"  /// instantiatable part of {context}. Since it is instantiatable, is is possible to use different contexts over the lifetime of a program. This ");
      streamWriter.WriteLine($"  /// is helpful for unti testing. Use {context}.Init() to create a new context and dispose it with DisposeData() before creating a new one.");
      streamWriter.WriteLine("  /// </summary>");
      streamWriter.WriteLine($"  public partial class {context}: IDisposable {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region static Part");
      streamWriter.WriteLine("    //      -----------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Provides static root access to the data context");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public static {context}? Data {{");
      streamWriter.WriteLine("      get { return data; }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine($"    private static {context}? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Constructs the StorageDirectories for all auto generated classes");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    /// <param name=\"csvConfig\">null: no permanent data storage, not null: info where to store the data</param>");
      streamWriter.WriteLine("    public static void Init(CsvConfig? csvConfig) {");
      streamWriter.WriteLine("      if (data!=null) throw new Exception();");
      streamWriter.WriteLine();
      streamWriter.WriteLine($"      data = new {context}(csvConfig);");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Flushes all data to permanent storage location if permanent data storage is active. Compacts data storage");
      streamWriter.WriteLine("    /// by applying all updates and removing all instances marked as deleted if isCompactDuringDispose==true.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public static void DisposeData() {");
      streamWriter.WriteLine("      var dataLocal = Interlocked.Exchange(ref data, null);");
      streamWriter.WriteLine("      dataLocal?.Dispose();");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Properties");
      streamWriter.WriteLine("    //      ----------");
      foreach (var classInfo in classes.Values.OrderBy(ci => ci.Name)) {
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Directory of all {classInfo.Name}s");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public StorageDictionary<{classInfo.Name}, {context}> {classInfo.Name}s {{ get; private set; }}");
      }
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Events");
      streamWriter.WriteLine("    //      ------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Constructors");
      streamWriter.WriteLine("    //      ------------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Creates a new data context. If csvConfig is null, the data is only stored in RAM, but gets lost once ");
      streamWriter.WriteLine("    /// program terminates. With csvConfig defined, existing data gets read at startup, changes immediately");
      streamWriter.WriteLine("    /// when written and Dispose() ensures by flushing that all data is permanently stored.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public {context}(CsvConfig? csvConfig) {{");
      streamWriter.WriteLine("      if (csvConfig==null) {");
      foreach (var classInfo in parentChildTree) {
        streamWriter.WriteLine($"        {classInfo.Name}s = new StorageDictionary<{classInfo.Name}, {context}>(");
        streamWriter.WriteLine("          this,");
        streamWriter.WriteLine($"          {classInfo.Name}.SetKey,");
        streamWriter.WriteLine($"          {classInfo.Name}.Disconnect,");
        streamWriter.WriteLine($"          areItemsUpdatable: {classInfo.AreItemsUpdatable},");
        streamWriter.WriteLine($"          areItemsDeletable: {classInfo.AreItemsDeletable});");
      }
      streamWriter.WriteLine("      } else {");
      foreach (var classInfo in parentChildTree) {
        streamWriter.WriteLine($"        {classInfo.Name}s = new StorageDictionaryCSV<{classInfo.Name}, {context}>(");
        streamWriter.WriteLine("          this,");
        streamWriter.WriteLine("          csvConfig!,");
        streamWriter.WriteLine($"          {classInfo.Name}.MaxLineLength,");
        streamWriter.WriteLine($"          {classInfo.Name}.Headers,");
        streamWriter.WriteLine($"          {classInfo.Name}.SetKey,");
        streamWriter.WriteLine($"          {classInfo.Name}.Create,");
        if (classInfo.Parents.Count>0) {
          streamWriter.WriteLine($"          {classInfo.Name}.Verify,");
        } else {
          streamWriter.WriteLine("          null,");
        }
        streamWriter.WriteLine($"          {classInfo.Name}.Update,");
        streamWriter.WriteLine($"          {classInfo.Name}.Write,");
        streamWriter.WriteLine($"          {classInfo.Name}.Disconnect,");
        streamWriter.WriteLine($"          areItemsUpdatable: {classInfo.AreItemsUpdatable},");
        streamWriter.WriteLine($"          areItemsDeletable: {classInfo.AreItemsDeletable},");
        streamWriter.WriteLine($"          isCompactDuringDispose: {classInfo.IsCompactDuringDispose});");
      }
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region IDisposable Support");
      streamWriter.WriteLine("    //      -------------------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Is {context}.Data already disposed ?");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public bool IsDisposed {");
      streamWriter.WriteLine("      get { return isDisposed==1; }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    int isDisposed = 0;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    protected virtual void Dispose(bool disposing) {");
      streamWriter.WriteLine("      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously");
      streamWriter.WriteLine("      if (wasDisposed==1) return; // already disposed");
      streamWriter.WriteLine();
      streamWriter.WriteLine("      if (disposing) {");
      streamWriter.WriteLine("        SampleDetails.Dispose();");
      streamWriter.WriteLine("        Samples.Dispose();");
      streamWriter.WriteLine("        SampleMasters.Dispose();");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    public void Dispose() {");
      streamWriter.WriteLine("      Dispose(true);");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Methods");
      streamWriter.WriteLine("    //      -------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine("  }");
      streamWriter.WriteLine("}");
      streamWriter.WriteLine();
    }
  }
}
