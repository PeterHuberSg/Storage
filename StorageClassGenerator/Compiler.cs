/**************************************************************************************

Storage.Compiler
================

Compiles a Model into a data Context

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


// https://csharp-source.net/last-projects: add link to Storage on that website
// http://prevayler.org/ java oject persitence library

namespace Storage {


  /// <summary>
  /// Compiles a Model into a data Context
  /// </summary>
  public class Compiler {

    public const bool IsFullyCommented = true; //set false to create Xxx.cs files where executable code is not commented out

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
            "MaxLineLenght, AreInstancesUpdatable and AreInstancesDeletable, but not";


    public void Parse(NamespaceDeclarationSyntax namespaceDeclaration, string fileName) {
      var newNameSpaceString = namespaceDeclaration.Name.GetText().ToString();
      if (nameSpaceString is null) {
        nameSpaceString = newNameSpaceString;
      } else if (nameSpaceString!=newNameSpaceString) {
        throw new GeneratorException($"{fileName} defines a different namespace 'newNameSpaceString' than the already defined one 'nameSpaceString'.");
      }
      foreach (var namespaceMember in namespaceDeclaration.Members) {
        if (namespaceMember is EnumDeclarationSyntax enumDeclarationSyntax) {
          parseEnum(enumDeclarationSyntax);
          continue;
        }

        if (!(namespaceMember is ClassDeclarationSyntax classDeclaration)) {
          throw new GeneratorException($"{fileName} contains not only class and enum declarations in namespace '{nameSpaceString}'.");
        }
        var className = classDeclaration.Identifier.Text;
        string? classComment = getXmlComment(classDeclaration.GetLeadingTrivia());
        int? maxLineLength = null;
        string? pluralName = className + 's';
        bool areInstancesUpdatable = true;
        bool areInstancesDeletable = true;
        bool isConstructorPrivate = false;
        if (classDeclaration.AttributeLists.Count==0) {
          //use the default values
        } else if (classDeclaration.AttributeLists.Count>1) {
          throw new GeneratorException($"Class {className} should contain at most 1 attribute, i.e. StorageClass attribute, but has '{classDeclaration.AttributeLists.Count}' attributes: '{classDeclaration.AttributeLists}'");

        } else {
          var attributes = classDeclaration.AttributeLists[0].Attributes;
          if (attributes.Count!=1) throw new GeneratorException($"Class {className} should contain at most 1 attribute, i.e. StorageClass attribute, but has '{classDeclaration.AttributeLists.Count}' attributes: '{attributes}'");

          var attribute = attributes[0];
          if (!(attribute.Name is IdentifierNameSyntax attributeName) || attributeName.Identifier.Text!="StorageClass") {
            throw new GeneratorException($"Class {className} should contain only a StorageClass attribute, but has: '{classDeclaration.AttributeLists}'");
          }
          foreach (var argument in attribute.ArgumentList!.Arguments) {
            if (argument.NameColon is null) throw new GeneratorException($"Class {className} Attribute{attribute}: the argument name is missing, like 'areInstancesUpdatable: true'.");

            var name = argument.NameColon.Name.Identifier.Text;
            try {
              var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
              switch (name) {
              case "maxLineLength": maxLineLength = int.Parse(value); break;
              case "pluralName": pluralName = value[1..^1]; break;
              case "areInstancesUpdatable": areInstancesUpdatable = value=="true"; break;
              case "areInstancesDeletable": areInstancesDeletable = value=="true"; break;
              case "isConstructorPrivate": isConstructorPrivate = value=="true"; break;
              default: throw new Exception();
              }
            } catch {
              new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
            }
          }

        }
        var classInfo = new ClassInfo(className, classComment, maxLineLength, pluralName, areInstancesUpdatable, areInstancesDeletable, isConstructorPrivate);
        classes.Add(className, classInfo);
        var isPropertyWithDefaultValueFound = false;
        foreach (var classMember in classDeclaration.Members) {
          //each field has only 1 property
          if (!(classMember is FieldDeclarationSyntax field)) {
            throw new GeneratorException($"Class {className} should contain only properties, but has '{classMember}'.");
          }

          string? propertyComment = getComment(field.GetLeadingTrivia());

          bool isReadOnly = false;
          foreach (var modifierToken in field.Modifiers) {
            if (modifierToken.Text=="const") {
              throw new GeneratorException($"Class {className} should contain only properties, but has const '{classMember}'.");
            } else if (modifierToken.Text=="readonly") {
              isReadOnly = true;
            }
          }
          if (!classInfo.AreInstancesUpdatable) {
            isReadOnly = true; //if a class cannot be updated, all its properties are readonly
          }

          if (!(field.Declaration is VariableDeclarationSyntax variableDeclaration)) {
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
          //      } else if (variableDeclarator.Identifier.Text=="AreInstancesUpdatable") {
          //        if (constValue!=null) {
          //          classInfo.SetAreInstancesUpdatable(bool.Parse(constValue.Token.Text));
          //          continue;
          //        }
          //      } else if (variableDeclarator.Identifier.Text=="AreInstancesDeletable") {
          //        if (constValue!=null) {
          //          classInfo.SetAreInstancesDeletable(bool.Parse(constValue.Token.Text));
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
            bool? isLookupOnly = null;
            if (field.AttributeLists.Count==0) {
              if (isPropertyWithDefaultValueFound && !propertyType.StartsWith("List<")) {
                throw new GeneratorException($"Property {className}.{property.Identifier.Text} should have a " +
                  "StorageProperty(defaultValue: \"xxx\") attribute, because the previous one had one too. Once a " +
                  "property has a default value, all following properties need to have one too.");
              }
              //use the default values
            } else if (field.AttributeLists.Count>1) {
              throw new GeneratorException($"Property {className}.{property.Identifier.Text} should contain at most 1 attribute, i.e. StorageProperty attribute, but has '{field.AttributeLists.Count}' attributes: '{field.AttributeLists}'");

            } else {
              var attributes = field.AttributeLists[0].Attributes;
              if (attributes.Count!=1) throw new GeneratorException($"Property {className}.{property.Identifier.Text} should contain at most 1 attribute, i.e. StorageProperty attribute, but has '{field.AttributeLists.Count}' attributes: '{attributes}'");

              var attribute = attributes[0];
              if (!(attribute.Name is IdentifierNameSyntax attributeName) || attributeName.Identifier.Text!="StorageProperty") {
                throw new GeneratorException($"Property {className}.{property.Identifier.Text} should contain only a StorageProperty attribute, but has: '{classDeclaration.AttributeLists}'");
              }
              foreach (var argument in attribute.ArgumentList!.Arguments) {
                if (argument.NameColon is null) throw new GeneratorException($"Property {className}.{property.Identifier.Text} Attribute{attribute}: the argument name is missing, like 'defaultValue: null'.");

                var name = argument.NameColon.Name.Identifier.Text;
                try {
                  var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
                  switch (name) {
                  case "defaultValue": defaultValue = value[1..^1]; isPropertyWithDefaultValueFound = true; break;
                  case "isLookupOnly": isLookupOnly = bool.Parse(value); break;
                  default: throw new Exception();
                  }
                } catch {
                  new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
                }
                //try {
                //  var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
                //  defaultValue = name switch {
                //    "defaultValue" => value[1..^1],
                //    _ => throw new Exception(),
                //  };
                //} catch {
                //  new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
                //}
              }
              
              ///////////////////////////////////
            }
            classInfo.AddMember(property.Identifier.Text, propertyType, propertyComment, defaultValue, isLookupOnly, isReadOnly);
            //}
          }
        }
      }
    }


    private void parseEnum(EnumDeclarationSyntax enumDeclaration) {
      var enumLeadingComment = getXmlComment(enumDeclaration.GetLeadingTrivia());
      //var enumDeclarationWithLeadingComment = enumDeclaration.ToFullString();
      //var enumDeclarationOnly = removeRegionAndLeadingSimpleComments(enumDeclarationWithLeadingComment);
      var indentation = enumDeclaration.GetLastToken().LeadingTrivia.ToString();
      enums.Add(enumDeclaration.Identifier.Text, 
        new EnumInfo(enumDeclaration.Identifier.Text, enumLeadingComment + indentation + enumDeclaration.ToString()));
    }


    //private string removeRegionAndLeadingSimpleComments(string declaration) {
    //  var pos1 = declaration.IndexOf("///");
    //  if (pos1>=0) {
    //    return addLeadingSpaces(declaration, pos1);
    //  }
    //  var pos2 = declaration.IndexOf("public enum ");
    //  if (pos2>=0) {
    //    return addLeadingSpaces(declaration, pos1);
    //  }
    //  return declaration;
    //}


    private string addLeadingSpaces(string declaration, int pos) {
      pos--;
      while (pos>0) {
        var c = declaration[pos];
        if (c!=' ') {
          break;
        }
        pos--;
      }
      pos++;
      return declaration[pos..];
    }


    private string? getXmlComment(SyntaxTriviaList syntaxTriviaList) {
      string? comment = null;
      var leadingTrivia = syntaxTriviaList.ToString();
      var lines = leadingTrivia.Split(Environment.NewLine);
      foreach (var line in lines) {
        if (line.Contains("///")) {
          comment += line + Environment.NewLine;
        }
      }
      return comment;
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
      foreach (var ci in classes.Values) {
        foreach (var mi in ci.Members.Values) {
          if (mi.MemberType==MemberTypeEnum.Parent) {
            if (classes.TryGetValue(mi.ParentType!, out mi.ParentClassInfo)) {
              ci.ParentsAll.Add(mi.ParentClassInfo);
              topClasses.Remove(ci.ClassName);
              mi.ParentClassInfo.Children.Add(ci);
              if (!mi.IsLookupOnly) {
                ci.ParentsWithList.Add(mi.ParentClassInfo);
                var isfound = false;
                foreach (var parentMember in mi.ParentClassInfo.Members.Values) {
                  if (parentMember.MemberName==ci.PluralName) {
                    isfound = true;
                    parentMember.ChildCount++;
                    break;
                  }
                }
                if (!isfound) {
                  throw new GeneratorException(
                    $"Class {mi.ParentClassInfo.ClassName}: property 'List<{ci.ClassName}> {ci.PluralName}' is missing." + Environment.NewLine +
                    $"Normally if a child class {ci.ClassName} references a parent class {mi.ParentClassInfo.ClassName}, the child class {ci.ClassName} " + Environment.NewLine +
                    $"should be added to {mi.ParentClassInfo.ClassName}.{ci.PluralName}. If no such collection should be generated in parent {mi.ParentClassInfo.ClassName}," + Environment.NewLine +
                    $"then add [StorageProperty(isLookupOnly: true)] to {ci.ClassName}.{mi.MemberName}.");
                }
              }
            } else {
              if (enums.TryGetValue(mi.ParentType!, out mi.EnumInfo)) {
                mi.MemberType = MemberTypeEnum.Enum;
                mi.ToStringFunc = "";
              } else {
                throw new GeneratorException($"{ci} '{mi}': can not find class or enum '{mi.MemberName}'.");
              }
            }

          //List
          } else if (mi.MemberType==MemberTypeEnum.List) {
            if (!classes.TryGetValue(mi.ChildTypeName!, out mi.ChildClassInfo))
              throw new GeneratorException($"{ci} '{mi}': can not find class {mi.ChildTypeName}.");

            bool isFound = false;
            foreach (var childMI in mi.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.Parent && childMI.ParentType==ci.ClassName) {
                isFound = true;
                mi.IsChildReadOnly |= childMI.IsReadOnly;
                if (mi.MemberName!=childMI.ClassInfo.PluralName) {
                  throw new GeneratorException($"{ci} '{mi}': name {mi.MemberName} should be {childMI.ClassInfo.PluralName}.");
                }
                //no break here, because child can have 2 properties belonging to the same Parent
              }
            }
            if (!isFound) {
              //guarantee that there is a property linking to the parent for each child class.
              throw new GeneratorException($"{ci} '{mi}': has a List<{mi.ChildTypeName}>. The corresponding " +
                $"property with type {ci.ClassName} is missing in the class {mi.ChildTypeName}.");
            }

          //Dictionary, SortedList
          } else if (mi.MemberType==MemberTypeEnum.CollectionKeyValue) {
            if (!classes.TryGetValue(mi.ChildTypeName!, out mi.ChildClassInfo))
              throw new GeneratorException($"{ci} '{mi}': can not find class {mi.ChildTypeName}.");

            bool isFound = false;
            bool isKeyFound = false;
            foreach (var childMI in mi.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.Parent && childMI.ParentType==ci.ClassName) {
                isFound = true;
                mi.IsChildReadOnly |= childMI.IsReadOnly;
                if (mi.MemberName!=childMI.ClassInfo.PluralName) {
                  throw new GeneratorException($"{ci} '{mi}': name {mi.MemberName} should be {childMI.ClassInfo.PluralName}.");
                }
                foreach (var childKeyMI in mi.ChildClassInfo.Members.Values) {
                  if (mi.ChildKeyPropertyName==childKeyMI.MemberName) {
                    if (mi.ChildKeyTypeString!=childKeyMI.CsvTypeString) {
                      throw new GeneratorException($"{ci}.{mi.MemberName} {mi.TypeString}: found {childKeyMI.ClassInfo.ClassName}.{childKeyMI.MemberName}, but it has wrong type: {childKeyMI.CsvTypeString}.");
                    }
                    isKeyFound = true;
                    if (mi.CollectionType==CollectionTypeEnum.SortedList) {
                      //memberTypeString = $"SortedList<{keyTypeName}, {itemTypeName}>";
                      mi.TypeString = $"SortedList<{childKeyMI.TypeString}, {childKeyMI.ClassInfo.ClassName}>";
                      mi.ReadOnlyTypeString = $"IReadOnlyDictionary<{childKeyMI.TypeString}, {childKeyMI.ClassInfo.ClassName}>";
                    } else {
                      //Dictionary
                      //memberTypeString = $"Dictionary<{keyTypeName}, {itemTypeName}>";
                      mi.TypeString = $"Dictionary<{childKeyMI.TypeString}, {childKeyMI.ClassInfo.ClassName}>";
                      mi.ReadOnlyTypeString = $"IReadOnlyDictionary<{childKeyMI.TypeString}, {childKeyMI.ClassInfo.ClassName}>";
                    }
                    break;
                  }
                }
              }
            }
            if (!isFound) {
              if (mi.CollectionType==CollectionTypeEnum.SortedList) {
                //guarantee that there is a property linking to the parent for each child class.
                throw new GeneratorException($"{ci} '{mi}': has a SortedList<{mi.ChildTypeName}>. The corresponding " +
                  $"property with type {ci.ClassName} is missing in the class {mi.ChildTypeName}.");
              } else {
                //guarantee that there is a property linking to the parent for each child class.
                throw new GeneratorException($"{ci} '{mi}': has a Dictionary<{mi.ChildTypeName}>. The corresponding " +
                  $"property with type {ci.ClassName} is missing in the class {mi.ChildTypeName}.");
              }
            }
            if (!isKeyFound) {
              if (mi.CollectionType==CollectionTypeEnum.SortedList) {
                //guarantee that there is a property that can be used as key into parent's SortedList
                throw new GeneratorException($"{ci} '{mi}': has a SortedList<{mi.ChildTypeName}>. The corresponding " +
                  $" key property '{mi.ChildKeyPropertyName}' with type {mi.ChildKeyTypeString} is missing in the class {mi.ChildTypeName}.");
              } else {
                //guarantee that there is a property that can be used as key into parent's Dictionary
                throw new GeneratorException($"{ci} '{mi}': has a Dictionary<{mi.ChildTypeName}>. The corresponding " +
                  $" key property '{mi.ChildKeyPropertyName}' with type {mi.ChildKeyTypeString} is missing in the class {mi.ChildTypeName}.");
              }
            }
          }
        }
      }

      //create parent child tree
      foreach (var classInfo in topClasses.Values) {
        addParentChildTree(classInfo);
      }
      foreach (var classInfo in classes.Values) {
        if (!classInfo.IsAddedToParentChildTree) throw new Exception();
      }
    }


    private void addParentChildTree(ClassInfo classInfo) {
      if (!classInfo.IsAddedToParentChildTree && allParentsAreAddedToParentChildTree(classInfo)) {
        classInfo.IsAddedToParentChildTree = true;
        parentChildTree.Add(classInfo);
        foreach (var child in classInfo.Children) {
          addParentChildTree(child);
        }
      }
    }


    private bool allParentsAreAddedToParentChildTree(ClassInfo childClass) {
      foreach (var parentClass in childClass.ParentsAll) {
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
          Console.Write(classInfo.ClassName + ".base.cs");
          if (classInfo.MaxLineLength is null) {
            Console.WriteLine($"  MaxBytes: {classInfo.EstimatedMaxByteSize}, default");
          } else if (classInfo.MaxLineLength<classInfo.EstimatedMaxByteSize) {
            Console.WriteLine($"  MaxBytes: {classInfo.MaxLineLength}, default would be: {classInfo.EstimatedMaxByteSize}");
          } else {
            Console.WriteLine($"  MaxBytes: {classInfo.MaxLineLength}, longer than default");
          }
          classInfo.WriteBaseClassFile(streamWriter, nameSpaceString!, context);
        }

        var fileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.ClassName + ".cs";
        if (!new FileInfo(fileNameAndPath).Exists) {
          using var streamWriter = new StreamWriter(fileNameAndPath);
          Console.WriteLine(classInfo.ClassName + ".cs");
          classInfo.WriteClassFile(streamWriter, nameSpaceString!, context, IsFullyCommented);
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
      streamWriter.WriteLine("//------------------------------------------------------------------------------");
      streamWriter.WriteLine("// <auto-generated>");
      streamWriter.WriteLine("//     This code was generated by StorageClassGenerator");
      streamWriter.WriteLine("//");
      streamWriter.WriteLine("//     Do not change code in this file, it will get lost when the file gets ");
      streamWriter.WriteLine($"//     auto generated again. Write your code into {context}.cs.");
      streamWriter.WriteLine("// </auto-generated>");
      streamWriter.WriteLine("//------------------------------------------------------------------------------");
      streamWriter.WriteLine("#nullable enable");
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Threading;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine($"namespace {nameSpaceString} {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine("  /// <summary>");
      streamWriter.WriteLine($"  /// A part of {context} is static, which gives easy access to all stored data (=context) through {context}.Data. But most functionality is in the");
      streamWriter.WriteLine($"  /// instantiatable part of {context}. Since it is instantiatable, is possible to use different contexts over the lifetime of a program. This ");
      streamWriter.WriteLine($"  /// is helpful for unit testing. Use {context}.Init() to create a new context and dispose it with DisposeData() before creating a new one.");
      streamWriter.WriteLine("  /// </summary>");
      streamWriter.WriteLine($"  public partial class {context}: IDisposable {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region static Part");
      streamWriter.WriteLine("    //      -----------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Provides static root access to the data context");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public static {context} Data {{");
      streamWriter.WriteLine("      get { return data!; }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine($"    private static {context}? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      //streamWriter.WriteLine("    /// <summary>");
      //streamWriter.WriteLine("    /// Constructs the StorageDirectories for all auto generated classes");
      //streamWriter.WriteLine("    /// </summary>");
      //streamWriter.WriteLine("    /// <param name=\"csvConfig\">null: no permanent data storage, not null: info where to store the data</param>");
      //streamWriter.WriteLine("    public static void Init(CsvConfig? csvConfig) {");
      //streamWriter.WriteLine("      if (data!=null) throw new Exception(\"Dispose old data first before initiating new ones.\");");
      //streamWriter.WriteLine();
      //streamWriter.WriteLine($"      data = new {context}(csvConfig);");
      //streamWriter.WriteLine("    }");
      //streamWriter.WriteLine();
      //streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Flushes all data to permanent storage location if permanent data storage is active. Compacts data storage");
      streamWriter.WriteLine("    /// by applying all updates and removing all instances marked as deleted.");
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
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Configuration parameters if data gets stored in .csv files");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public CsvConfig? CsvConfig { get; }");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Is all data initialised");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public bool IsInitialised { get; private set; }");
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
      streamWriter.WriteLine("      if (!IsDisposed) {");
      streamWriter.WriteLine($"        throw new Exception(\"Dispose old {context} before creating a new one.\");");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      isDisposed = 0;");
      streamWriter.WriteLine("      data = this;");
      streamWriter.WriteLine("      IsInitialised = false;");
      streamWriter.WriteLine();
      streamWriter.WriteLine("      onConstruct();");
      streamWriter.WriteLine();
      streamWriter.WriteLine("      CsvConfig = csvConfig;");
      streamWriter.WriteLine("      if (csvConfig==null) {");
      foreach (var classInfo in parentChildTree) {
        streamWriter.WriteLine($"        {classInfo.PluralName} = new StorageDictionary<{classInfo.ClassName}, {context}>(");
        streamWriter.WriteLine("          this,");
        streamWriter.WriteLine($"          {classInfo.ClassName}.SetKey,");
        if (classInfo.AreInstancesDeletable && classInfo.IsDisconnectNeeded) {
          streamWriter.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        } else {
          streamWriter.WriteLine($"          null,");
        }
        streamWriter.WriteLine($"          areInstancesUpdatable: {classInfo.AreInstancesUpdatable.ToString().ToLowerInvariant()},");
        streamWriter.WriteLine($"          areInstancesDeletable: {classInfo.AreInstancesDeletable.ToString().ToLowerInvariant()});");
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
        if (classInfo.ParentsWithList.Count>0) {
          streamWriter.WriteLine($"          {classInfo.ClassName}.Verify,");
        } else {
          streamWriter.WriteLine("          null,");
        }
        if (classInfo.AreInstancesUpdatable) {
          streamWriter.WriteLine($"          {classInfo.ClassName}.Update,");
        } else {
          streamWriter.WriteLine($"          null,");
        }
        streamWriter.WriteLine($"          {classInfo.ClassName}.Write,");
        if (classInfo.AreInstancesDeletable && classInfo.IsDisconnectNeeded) {
          streamWriter.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        } else {
          streamWriter.WriteLine($"          null,");
        }
        streamWriter.WriteLine($"          areInstancesUpdatable: {classInfo.AreInstancesUpdatable.ToString().ToLowerInvariant()},");
        streamWriter.WriteLine($"          areInstancesDeletable: {classInfo.AreInstancesDeletable.ToString().ToLowerInvariant()});");
      }
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      onConstructed();");
      streamWriter.WriteLine("      IsInitialised = true;");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>}");
      streamWriter.WriteLine("    /// Called at beginning of constructor");
      streamWriter.WriteLine("    /// </summary>}");
      streamWriter.WriteLine("    partial void onConstruct();");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>}");
      streamWriter.WriteLine("    /// Called at end of constructor");
      streamWriter.WriteLine("    /// </summary>}");
      streamWriter.WriteLine("    partial void onConstructed();");
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
      streamWriter.WriteLine("    int isDisposed = 1;");
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
      streamWriter.WriteLine("        data = null;");
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
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine(enumInfo.CodeLines);
      }
      streamWriter.WriteLine("}");
    }
  }
}
