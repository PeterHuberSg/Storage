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
    readonly Dictionary<string, EnumInfo> enums;
    public IReadOnlyDictionary<string, EnumInfo> Enums { get { return enums; } }


    public Compiler() {
      classes = new Dictionary<string, ClassInfo>();
      parentChildTree = new List<ClassInfo>();
      enums = new Dictionary<string, EnumInfo>();
    }


    string? nameSpaceString;
    const string onlyAcceptableConsts = "should only contain properties and configuration constants for " +
            "MaxLineLenght, AreItemsUpdatable, AreItemsDeletable and IsCompactDuringDispose , but not";


    public void Parse(NamespaceDeclarationSyntax namespaceDeclaration, string fileName) {
      var newNameSpaceString = namespaceDeclaration.Name.GetText().ToString();
      if (nameSpaceString is null) {
        nameSpaceString = newNameSpaceString;
      } else if (nameSpaceString!=newNameSpaceString) {
        throw new GeneratorException($"{fileName} defines a different namespace 'newNameSpaceString' than the already defined one 'nameSpaceString'.");
      }
      foreach (var namespaceMember in namespaceDeclaration.Members) {
        var classDeclaration = namespaceMember as ClassDeclarationSyntax;
        if (classDeclaration is null) {
          var enumDeclarationSyntax = namespaceMember as EnumDeclarationSyntax;
          if (enumDeclarationSyntax is null) {
            throw new GeneratorException($"{fileName} contains not only class and enum declarations in namespace '{nameSpaceString}'.");
          }
          parseEnum(enumDeclarationSyntax);
          continue;
        }
        var className = classDeclaration.Identifier.Text;
        string? classComment = getComment(classDeclaration.GetLeadingTrivia());
        int maxLineLength = 0;
        string? pluralName = className + 's';
        bool areItemsUpdatable = true;
        bool areItemsDeletable = true;
        bool isCompactDuringDispose = true;
        if (classDeclaration.AttributeLists.Count==0) {
          //use the default values
        } else if (classDeclaration.AttributeLists.Count>1) {
          throw new GeneratorException($"Class {className} schould contain at most 1 attribute, i.e. StorageClass attribute, but has '{classDeclaration.AttributeLists.Count}' attributes: '{classDeclaration.AttributeLists}'");

        } else {
          var attributes = classDeclaration.AttributeLists[0].Attributes;
          if (attributes.Count!=1) throw new GeneratorException($"Class {className} schould contain at most 1 attribute, i.e. StorageClass attribute, but has '{classDeclaration.AttributeLists.Count}' attributes: '{attributes}'");

          var attribute = attributes[0];
          var attributeName = attribute.Name as IdentifierNameSyntax;
          if (attributeName==null || attributeName.Identifier.Text!="StorageClass") {
            throw new GeneratorException($"Class {className} schould contain only a StorageClass attribute, but has: '{classDeclaration.AttributeLists}'");
          }
          foreach (var argument in attribute.ArgumentList!.Arguments) {
            if (argument.NameColon is null) throw new GeneratorException($"Class {className} Attribute{attribute}: the argument name is missing, like 'areItemsUpdatable: true'.");

            var name = argument.NameColon.Name.Identifier.Text;
            try {
              var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
              switch (name) {
              case "maxLineLength": maxLineLength = int.Parse(value); break;
              case "pluralName": pluralName = value[1..^1]; break;
              case "areItemsUpdatable": areItemsUpdatable = value=="true"; break;
              case "areItemsDeletable": areItemsDeletable = value=="true"; break;
              case "isCompactDuringDispose": isCompactDuringDispose = value=="true"; break;
              default: throw new Exception();
              }
            } catch {
              new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
            }
          }

        }
        var classInfo = new ClassInfo(className, classComment, maxLineLength, pluralName, areItemsUpdatable, areItemsDeletable, isCompactDuringDispose);
        classes.Add(className, classInfo);
        var isPropertyWithDefaultValueFound = false;
        foreach (var classMember in classDeclaration.Members) {
          var field = classMember as FieldDeclarationSyntax; //each field has only 1 property
          if (field is null) {
            throw new GeneratorException($"Class {className} schould contain only properties, but has '{classMember}'.");
          }

          string? propertyComment = getComment(field.GetLeadingTrivia());

          //var isConst = false;
          foreach (var modifierToken in field.Modifiers) {
            if (modifierToken.Text=="const") {
              //isConst = true;
              //break;
              throw new GeneratorException($"Class {className} schould contain only properties, but has const '{classMember}'.");
            }
          }

          var variableDeclaration = field.Declaration as VariableDeclarationSyntax;
          if (variableDeclaration is null) {
            throw new GeneratorException($"Class {className} {onlyAcceptableConsts} '{field.Declaration}'.");
          }
          var propertyType = variableDeclaration.Type.ToString();
          //if (isConst) {
          //  foreach (var variableDeclarator in variableDeclaration.Variables) {
          //    var constValue = variableDeclarator.Initializer?.Value as LiteralExpressionSyntax;
          //    if (constValue!=null) {
          //      if (variableDeclarator.Identifier.Text=="MaxLineLenght") {
          //        if (constValue!=null) {
          //          classInfo.SetMaxLineLength(int.Parse(constValue.Token.Text));
          //          continue;
          //        }
          //      } else if (variableDeclarator.Identifier.Text=="AreItemsUpdatable") {
          //        if (constValue!=null) {
          //          classInfo.SetAreItemsUpdatable(bool.Parse(constValue.Token.Text));
          //          continue;
          //        }
          //      } else if (variableDeclarator.Identifier.Text=="AreItemsDeletable") {
          //        if (constValue!=null) {
          //          classInfo.SetAreItemsDeletable(bool.Parse(constValue.Token.Text));
          //          continue;
          //        }
          //      } else if (variableDeclarator.Identifier.Text=="IsCompactDuringDispose") {
          //        if (constValue!=null) {
          //          classInfo.SetIsCompactDuringDispose(bool.Parse(constValue.Token.Text));
          //          continue;
          //        }
          //      }
          //    }
          //    throw new GeneratorException($"Class {className} {onlyAcceptableConsts} '{classMember}'.");
          //  }
          //} else {
          foreach (var property in variableDeclaration.Variables) {
            //////////////////////////////////
            string? defaultValue = null;
            if (field.AttributeLists.Count==0) {
              if (isPropertyWithDefaultValueFound && !propertyType.StartsWith("List<")) {
                throw new GeneratorException($"Property {className}.{property.Identifier.Text} schould have a " +
                  "StorageProperty(defaultValue: \"xxx\") attribute, because the previous one had one too. Once a " +
                  "property has a deault value, all following properties need to have one too.");
              }
              //use the default values
            } else if (field.AttributeLists.Count>1) {
              throw new GeneratorException($"Property {className}.{property.Identifier.Text} schould contain at most 1 attribute, i.e. StorageProperty attribute, but has '{field.AttributeLists.Count}' attributes: '{field.AttributeLists}'");

            } else {
              var attributes = field.AttributeLists[0].Attributes;
              if (attributes.Count!=1) throw new GeneratorException($"Property {className}.{property.Identifier.Text} schould contain at most 1 attribute, i.e. StorageProperty attribute, but has '{field.AttributeLists.Count}' attributes: '{attributes}'");

              var attribute = attributes[0];
              var attributeName = attribute.Name as IdentifierNameSyntax;
              if (attributeName==null || attributeName.Identifier.Text!="StorageProperty") {
                throw new GeneratorException($"Property {className}.{property.Identifier.Text} schould contain only a StorageProperty attribute, but has: '{classDeclaration.AttributeLists}'");
              }
              foreach (var argument in attribute.ArgumentList!.Arguments) {
                if (argument.NameColon is null) throw new GeneratorException($"Property {className}.{property.Identifier.Text} Attribute{attribute}: the argument name is missing, like 'defaultValue: null'.");

                var name = argument.NameColon.Name.Identifier.Text;
                try {
                  var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
                  switch (name) {
                  case "defaultValue": defaultValue = value[1..^1]; break;
                  default: throw new Exception();
                  }
                } catch {
                  new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
                }
              }
              isPropertyWithDefaultValueFound = true;
              ///////////////////////////////////
            }
            classInfo.AddMember(property.Identifier.Text, propertyType, propertyComment, defaultValue);
            //}
          }
        }
      }
    }


    private void parseEnum(EnumDeclarationSyntax enumDeclaration) {
      enums.Add(enumDeclaration.Identifier.Text, new EnumInfo(enumDeclaration.Identifier.Text, enumDeclaration.ToFullString()));
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
      var topClasses = classes.Values.ToDictionary(c=>c.ClassName);
      foreach (var classInfo in classes.Values) {
        foreach (var memberInfo in classInfo.Members.Values) {
          if (memberInfo.MemberType==MemberTypeEnum.Parent) {
            if (classes.TryGetValue(memberInfo.ParentType!, out memberInfo.ParentClassInfo)) {
              classInfo.Parents.Add(memberInfo.ParentClassInfo);
              memberInfo.ParentClassInfo.Children.Add(classInfo);
              topClasses.Remove(classInfo.ClassName);
              var isfound = false;
              foreach (var parentMember in memberInfo.ParentClassInfo.Members.Values) {
                if (parentMember.MemberName==classInfo.PluralName) {
                  isfound = true;
                  parentMember.ChildCount++;
                  break;
                }
              }
              if (!isfound) {
                throw new GeneratorException($"Class {memberInfo.ParentClassInfo.ClassName}: property 'List<{classInfo.ClassName}> {classInfo.PluralName}' is missing.");
              }
            } else {
              if (enums.TryGetValue(memberInfo.ParentType!, out memberInfo.EnumInfo)) {
                memberInfo.MemberType = MemberTypeEnum.Enum;
                memberInfo.ToStringFunc = "";
              } else {
                throw new GeneratorException($"{classInfo} '{memberInfo}': can not find class or enum {memberInfo.MemberName}.");
              }
            }
          } else if (memberInfo.MemberType==MemberTypeEnum.List) {
            if (!classes.TryGetValue(memberInfo.ChildTypeName!, out memberInfo.ChildClassInfo))
              throw new GeneratorException($"{classInfo} '{memberInfo}': can not find class {memberInfo.ChildTypeName}.");

            bool isFound = false;
            foreach (var childMI in memberInfo.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.Parent && childMI.ParentType==classInfo.ClassName) {
                isFound = true;
                if (memberInfo.MemberName!=childMI.ClassInfo.PluralName) {
                  throw new GeneratorException($"{classInfo} '{memberInfo}': name {memberInfo.MemberName} should be {childMI.ClassInfo.PluralName}.");
                }
              }
            }
            if (!isFound) {
              //guarantee that there is a property linking to the parent for each child class.
              throw new GeneratorException($"{classInfo} '{memberInfo}': has a List<{memberInfo.ChildTypeName}>. The corresponding " +
                $"property with type {classInfo.ClassName} is missing in the class {memberInfo.ChildTypeName}.");
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


    public void WriteClassFiles(DirectoryInfo targetDirectory, string context) {
      foreach (var classInfo in classes.Values) {
        var baseFileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.ClassName + ".base.cs";
        try {
          File.Delete(baseFileNameAndPath);
        } catch {
          throw new GeneratorException($"Cannot delete file {baseFileNameAndPath}.");
        }
        using (var streamWriter = new StreamWriter(baseFileNameAndPath)) {
          Console.WriteLine(classInfo.ClassName + ".base.cs");
          classInfo.WriteBaseClassFile(streamWriter, nameSpaceString!, context);
        }

        var fileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.ClassName + ".cs";
        if (!new FileInfo(fileNameAndPath).Exists) {
          using var streamWriter = new StreamWriter(fileNameAndPath);
          Console.WriteLine(classInfo.ClassName + ".cs");
          classInfo.WriteClassFile(streamWriter, nameSpaceString!);
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
      foreach (var classInfo in classes.Values.OrderBy(ci => ci.ClassName)) {
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Directory of all {classInfo.PluralName}");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public StorageDictionary<{classInfo.ClassName}, {context}> {classInfo.PluralName} {{ get; private set; }}");
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
        streamWriter.WriteLine($"        {classInfo.PluralName} = new StorageDictionary<{classInfo.ClassName}, {context}>(");
        streamWriter.WriteLine("          this,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.SetKey,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        streamWriter.WriteLine($"          areItemsUpdatable: {classInfo.AreItemsUpdatable.ToString().ToLowerInvariant()},");
        streamWriter.WriteLine($"          areItemsDeletable: {classInfo.AreItemsDeletable.ToString().ToLowerInvariant()});");
      }
      streamWriter.WriteLine("      } else {");
      foreach (var classInfo in parentChildTree) {
        streamWriter.WriteLine($"        {classInfo.PluralName} = new StorageDictionaryCSV<{classInfo.ClassName}, {context}>(");
        streamWriter.WriteLine("          this,");
        streamWriter.WriteLine("          csvConfig!,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.MaxLineLength,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.Headers,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.SetKey,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.Create,");
        if (classInfo.Parents.Count>0) {
          streamWriter.WriteLine($"          {classInfo.ClassName}.Verify,");
        } else {
          streamWriter.WriteLine("          null,");
        }
        streamWriter.WriteLine($"          {classInfo.ClassName}.Update,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.Write,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        streamWriter.WriteLine($"          areItemsUpdatable: {classInfo.AreItemsUpdatable.ToString().ToLowerInvariant()},");
        streamWriter.WriteLine($"          areItemsDeletable: {classInfo.AreItemsDeletable.ToString().ToLowerInvariant()},");
        streamWriter.WriteLine($"          isCompactDuringDispose: {classInfo.IsCompactDuringDispose.ToString().ToLowerInvariant()});");
      }
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      onConstruct();");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>}");
      streamWriter.WriteLine("    /// Called at end of constructor");
      streamWriter.WriteLine("    /// </summary>}");
      streamWriter.WriteLine("    partial void onConstruct();");
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
      streamWriter.WriteLine("        onDispose();");
      foreach (var classInfo in ((IEnumerable<ClassInfo>)parentChildTree).Reverse()) {
        streamWriter.WriteLine($"        {classInfo.PluralName}.Dispose();");
      }
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>}");
      streamWriter.WriteLine("    /// Called before storageDirectories get disposed.");
      streamWriter.WriteLine("    /// </summary>}");
      streamWriter.WriteLine("    partial void onDispose();");
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


    internal void WriteEnumsFile(DirectoryInfo targetDirectory) {
      var baseFileNameAndPath = targetDirectory!.FullName + '\\' + "Enums.base.cs";
      try {
        File.Delete(baseFileNameAndPath);
      } catch {
        throw new GeneratorException($"Cannot delete file {baseFileNameAndPath}.");
      }
      using var streamWriter = new StreamWriter(baseFileNameAndPath);
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Collections.Generic;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("namespace " + nameSpaceString + " {");
      foreach (var enumInfo in enums.Values) {
        streamWriter.Write(enumInfo.CodeLines);
      }
      streamWriter.WriteLine("}");
    }
  }
}
