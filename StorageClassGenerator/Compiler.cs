﻿/**************************************************************************************

Storage.Compiler
================

Compiles a Model into a DataContext

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
  /// Compiles a Model into a DataContext
  /// </summary>
  public class Compiler {

    //public const bool IsFullyCommented = true; //set false to create Xxx.cs files where executable code is not commented out
    public const bool IsFullyCommented = false; 

    readonly Dictionary<string, ClassInfo> classes;
    readonly List<ClassInfo> parentChildTree;
    readonly Dictionary<string, EnumInfo> enums;
    public IReadOnlyDictionary<string, EnumInfo> Enums { get { return enums; } }
    bool isUsingDictionary = false;


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
        string? pluralName = className + 's';
        bool areInstancesUpdatable = true;
        bool areInstancesDeletable = true;
        bool isConstructorPrivate = false;
        bool isGenerateReaderWriter = false;
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
            var isException = false;
            try {
              var value = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
              switch (name) {
              case "pluralName": pluralName = value[1..^1]; break;
              case "areInstancesUpdatable": areInstancesUpdatable = value=="true"; break;
              case "areInstancesDeletable": areInstancesDeletable = value=="true"; break;
              case "isConstructorPrivate": isConstructorPrivate = value=="true"; break;
              case "isGenerateReaderWriter": isGenerateReaderWriter = value=="true"; break;
              default:
                isException = true;
                throw new GeneratorException($"Class {className} Attribute{attribute}: Illegal argument name {name}. It " +
                  "can only be: pluralName, areInstancesUpdatable, areInstancesDeletable, isConstructorPrivate or " +
                  "isGenerateReaderWriter.");
              }
            } catch {
              if (isException) {
                throw;
              }
              throw new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
            }
          }

        }
        var classInfo = new ClassInfo(className, classComment, pluralName, areInstancesUpdatable, 
          areInstancesDeletable, isConstructorPrivate, isGenerateReaderWriter);
        classes.Add(className, classInfo);
        var isPropertyWithDefaultValueFound = false;
        foreach (var classMember in classDeclaration.Members) {
          //each field has only 1 property
          if (!(classMember is FieldDeclarationSyntax field)) {
            throw new GeneratorException($"Class {className} should contain only properties and these properties should " + 
              $"not contain get and set, but has '{classMember}'.");
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
          foreach (var property in variableDeclaration.Variables) {
            string? defaultValue = null;
            bool? isLookupOnly = null;
            bool needsDictionary = false;
            bool isParentOneChild = false;
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
                  case "needsDictionary": 
                    needsDictionary = bool.Parse(value);
                    if (needsDictionary) isUsingDictionary = true;
                    break;
                  case "isParentOneChild": isParentOneChild = bool.Parse(value); break;
                  default: throw new Exception();
                  }
                } catch {
                  new GeneratorException($"Class {className} Attribute{attribute}: Something wrong with assigning a value to argument {name}.");
                }
              }
            }
            if ((isLookupOnly??false) && isParentOneChild) {
              throw new GeneratorException($"Property {className}.{property.Identifier.Text} cannot have " + 
                "isLookupOnly: true and isParentOneChild: true in its StorageProperty attribute.");
            }
            classInfo.AddMember(property.Identifier.Text, propertyType, propertyComment, defaultValue, isLookupOnly, 
              needsDictionary, isParentOneChild, isReadOnly);
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
          var isFound = false;
          switch (mi.MemberType) {
          case MemberTypeEnum.LinkToParent:
            //                -------------
            if (classes.TryGetValue(mi.ParentTypeString!, out mi.ParentClassInfo)) {
              ci.ParentsAll.Add(mi.ParentClassInfo);
              topClasses.Remove(ci.ClassName);
              mi.ParentClassInfo.Children.Add(ci);
              if (mi.IsLookupOnly) {
                if (mi.ParentClassInfo.AreInstancesDeletable) {
                  throw new GeneratorException($"{ci.ClassName}.{mi.MemberName}: cannot use the deletable instances of class " +
                    $"{mi.ParentClassInfo.ClassName} as lookup.");
                }
              } else { 
                foreach (var parentMember in mi.ParentClassInfo.Members.Values) {
                  if (parentMember.MemberName==ci.PluralName) {
                    isFound = true;
                    //ci.ParentsWithList.Add(mi.ParentClassInfo);
                    mi.ParentMemberInfo = parentMember;
                    mi.ParentMethodName = ci.PluralName;
                    parentMember.ChildCount++;
                    break;
                  } else if (parentMember.ChildTypeName==mi.ClassInfo.ClassName) {
                    if (parentMember.MemberType!=MemberTypeEnum.ParentOneChild) {
                      throw new GeneratorException($"{ci.ClassName}.{mi.MemberName}: links to {mi.TypeString}.{parentMember.MemberName}, " +
                        "which should have StoragePropertyAttribute(isParentOneChild: true).");
                    }
                    isFound = true;
                    mi.ParentMemberInfo = parentMember;
                    mi.ParentMethodName = parentMember.MemberName;
                    break;
                  }
                }
                if (!isFound) {
                  throw new GeneratorException(
                    $"Property {mi.MemberName} from child class {ci.ClassName} links to parent {mi.ParentClassInfo.ClassName}." + 
                    $"But the parent does not have a property which links to the child. Add a collection (list, dictionary or sortedList) " + 
                    $"to the parent if many children are allowed or a property with the [StorageProperty(isParentOneChild: true)] attribute " + 
                    $"if only 1 child is allowed.");
                }
                if (!mi.ClassInfo.AreInstancesDeletable && mi.ParentClassInfo.AreInstancesDeletable) {
                  //todo: Compiler.AnalyzeDependencies() Add tests if child is at least updatable, parent property not readonly and nullable
                  throw new GeneratorException($"Child {mi.ClassInfo.ClassName} does not support deletion. Therefore, the " + 
                    $"parent {mi.ParentClassInfo.ClassName} can neither support deletion, because it can not delete its children.");
                }
              }

            } else if (enums.TryGetValue(mi.ParentTypeString!, out mi.EnumInfo)) {
              mi.MemberType = MemberTypeEnum.Enum;
              mi.ToStringFunc = "";
            } else {
              throw new GeneratorException($"{ci.ClassName}.{mi.MemberName}: cannot find '{mi.ParentTypeString}'. Should this be a data type " +
                "defined by Storage, a user defined enum or a link to a user defined class ?");
            }
            break;

          case MemberTypeEnum.ParentOneChild:
            //                --------------
            if (!classes.TryGetValue(mi.ChildTypeName!, out mi.ChildClassInfo))
              throw new GeneratorException($"{ci} '{mi}': cannot find class {mi.ChildTypeName}.");

            foreach (var childMI in mi.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.LinkToParent && childMI.ParentTypeString==ci.ClassName) {
                isFound = true;
                mi.ChildMemberInfo = childMI;
                mi.IsChildReadOnly |= childMI.IsReadOnly;
                break;
              }
            }
            if (!isFound) {
              //guarantee that there is a property linking to the parent for each child class.
              throw new GeneratorException($"{ci} '{mi}' is a property which links to 0 or 1 child. A corresponding " +
                $"property with type {ci.ClassName} is missing in the class {mi.ChildTypeName}.");
            }
            break;

          case MemberTypeEnum.ParentMultipleChildrenList:
            //                --------------------------
            if (!classes.TryGetValue(mi.ChildTypeName!, out mi.ChildClassInfo))
              throw new GeneratorException($"{ci} '{mi}': can not find class {mi.ChildTypeName}.");

            foreach (var childMI in mi.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.LinkToParent && childMI.ParentTypeString==ci.ClassName) {
                isFound = true;
                mi.IsChildReadOnly |= childMI.IsReadOnly;
                if (mi.MemberName!=childMI.ClassInfo.PluralName) {
                  throw new GeneratorException($"{ci} '{mi}': name {mi.MemberName} should be {childMI.ClassInfo.PluralName}.");
                }
                //no break here, because child can have 2 properties belonging to the same Parent
                //mi.ChildMemberInfo = childMI; not done here, because of multiple children
              }
            }
            if (!isFound) {
              //guarantee that there is a property linking to the parent for each child class.
              throw new GeneratorException($"{ci} '{mi}': has a List<{mi.ChildTypeName}>. The corresponding " +
                $"property with type {ci.ClassName} is missing in the class {mi.ChildTypeName}.");
            }
            break;

          case MemberTypeEnum.ParentMultipleChildrenDictionary:
          case MemberTypeEnum.ParentMultipleChildrenSortedList:
            //Dictionary, SortedList
            if (!classes.TryGetValue(mi.ChildTypeName!, out mi.ChildClassInfo))
              throw new GeneratorException($"{ci} '{mi}': cannot find class {mi.ChildTypeName}.");

            bool isKeyFound = false;
            foreach (var childMI in mi.ChildClassInfo.Members.Values) {
              if (childMI.MemberType==MemberTypeEnum.LinkToParent && childMI.ParentTypeString==ci.ClassName) {
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
                    mi.ChildMemberInfo = childMI;
                    if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenSortedList) {
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
                if (isKeyFound) break;
              }
            }
            if (!isFound) {
              if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenSortedList) {
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
              if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenSortedList) {
                //guarantee that there is a property that can be used as key into parent's SortedList
                throw new GeneratorException($"{ci} '{mi}': has a SortedList<{mi.ChildTypeName}>. The corresponding " +
                  $" key property '{mi.ChildKeyPropertyName}' with type {mi.ChildKeyTypeString} is missing in the class {mi.ChildTypeName}.");
              } else {
                //guarantee that there is a property that can be used as key into parent's Dictionary
                throw new GeneratorException($"{ci} '{mi}': has a Dictionary<{mi.ChildTypeName}>. The corresponding " +
                  $" key property '{mi.ChildKeyPropertyName}' with type {mi.ChildKeyTypeString} is missing in the class {mi.ChildTypeName}.");
              }
            }
            break;
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
          classInfo.EstimatedMaxByteSize = Math.Max(classInfo.EstimatedMaxByteSize, classInfo.HeaderLength);
          Console.WriteLine($"  Estimated bytes to store 1 instance: {classInfo.EstimatedMaxByteSize}, default");
          classInfo.WriteBaseClassFile(streamWriter, nameSpaceString!, context);
        }

        var fileNameAndPath = targetDirectory!.FullName + '\\' + classInfo.ClassName + ".cs";
        if (!new FileInfo(fileNameAndPath).Exists) {
          using var streamWriter = new StreamWriter(fileNameAndPath);
          Console.WriteLine(classInfo.ClassName + ".cs");
          classInfo.WriteClassFile(streamWriter, nameSpaceString!, IsFullyCommented);
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
      using var sw = new StreamWriter(fileNameAndPath);
      sw.WriteLine("//------------------------------------------------------------------------------");
      sw.WriteLine("// <auto-generated>");
      sw.WriteLine("//     This code was generated by StorageClassGenerator");
      sw.WriteLine("//");
      sw.WriteLine("//     Do not change code in this file, it will get lost when the file gets ");
      sw.WriteLine($"//     auto generated again. Write your code into {context}.cs.");
      sw.WriteLine("// </auto-generated>");
      sw.WriteLine("//------------------------------------------------------------------------------");
      sw.WriteLine("#nullable enable");
      sw.WriteLine("using System;");
      if (isUsingDictionary) {
        sw.WriteLine("using System.Collections.Generic;");
      }
      sw.WriteLine("using System.Threading;");
      sw.WriteLine("using Storage;");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine($"namespace {nameSpaceString} {{");
      sw.WriteLine();
      sw.WriteLine("  /// <summary>");
      sw.WriteLine($"  /// A part of {context} is static, which gives easy access to all stored data (=context) through {context}.Data. But most functionality is in the");
      sw.WriteLine($"  /// instantiatable part of {context}. Since it is instantiatable, is possible to use different contexts over the lifetime of a program. This ");
      sw.WriteLine($"  /// is helpful for unit testing. Use {context}.Init() to create a new context and dispose it with DisposeData() before creating a new one.");
      sw.WriteLine("  /// </summary>");
      sw.WriteLine($"  public partial class {context}: DataContextBase {{");
      sw.WriteLine();
      sw.WriteLine("    #region static Part");
      sw.WriteLine("    //      -----------");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>");
      sw.WriteLine("    /// Provides static root access to the data context");
      sw.WriteLine("    /// </summary>");
      sw.WriteLine($"    public static {context} Data {{");
      sw.WriteLine("      get { return data!; }");
      sw.WriteLine("    }");
      sw.WriteLine($"    private static {context}? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    /// <summary>");
      sw.WriteLine("    /// Flushes all data to permanent storage location if permanent data storage is active. Compacts data storage");
      sw.WriteLine("    /// by applying all updates and removing all instances marked as deleted.");
      sw.WriteLine("    /// </summary>");
      sw.WriteLine("    public static void DisposeData() {");
      sw.WriteLine("      var dataLocal = Interlocked.Exchange(ref data, null);");
      sw.WriteLine("      dataLocal?.Dispose();");
      sw.WriteLine("    }");
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    #region Properties");
      sw.WriteLine("    //      ----------");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>");
      sw.WriteLine("    /// Configuration parameters if data gets stored in .csv files");
      sw.WriteLine("    /// </summary>");
      sw.WriteLine("    public CsvConfig? CsvConfig { get; }");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>");
      sw.WriteLine("    /// Is all data initialised");
      sw.WriteLine("    /// </summary>");
      sw.WriteLine("    public bool IsInitialised { get; private set; }");
      foreach (var classInfo in classes.Values.OrderBy(ci => ci.ClassName)) {
        sw.WriteLine();
        sw.WriteLine("    /// <summary>");
        sw.WriteLine($"    /// Directory of all {classInfo.PluralName}");
        sw.WriteLine("    /// </summary>");
        sw.WriteLine($"    public DataStore<{classInfo.ClassName}> {classInfo.PluralName} {{ get; private set; }}");
        foreach (var mi in classInfo.Members.Values.OrderBy(mi => mi.MemberName)) {
          if (mi.NeedsDictionary) {
            sw.WriteLine();
            sw.WriteLine("    /// <summary>");
            sw.WriteLine($"    /// Directory of all {classInfo.PluralName} by {mi.MemberName}");
            sw.WriteLine("    /// </summary>");
            sw.WriteLine($"    public Dictionary<{mi.TypeString.Replace("?", "")}, {classInfo.ClassName}> {classInfo.PluralName}By{mi.MemberName} {{ get; private set; }}");
          }
        }
      }
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    #region Events");
      sw.WriteLine("    //      ------");
      sw.WriteLine();
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    #region Constructors");
      sw.WriteLine("    //      ------------");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>");
      sw.WriteLine("    /// Creates a new DataContext. If csvConfig is null, the data is only stored in RAM and gets lost once the ");
      sw.WriteLine("    /// program terminates. With csvConfig defined, existing data gets read at startup, changes get immediately");
      sw.WriteLine("    /// written and Dispose() ensures by flushing that all data is permanently stored.");
      sw.WriteLine("    /// </summary>");
      sw.WriteLine($"    public {context}(CsvConfig? csvConfig): base(DataStoresCount: {classes.Count}) {{");
      sw.WriteLine("      data = this;");
      sw.WriteLine("      IsInitialised = false;");
      sw.WriteLine();
      sw.WriteLine("      string? backupResult = null;");
      sw.WriteLine("      if (csvConfig!=null) {");
      sw.WriteLine("        backupResult = Csv.Backup(csvConfig, DateTime.Now);");
      sw.WriteLine("      }");
      sw.WriteLine();
      sw.WriteLine("      CsvConfig = csvConfig;");
      sw.WriteLine("      onConstructing(backupResult);");
      sw.WriteLine();

      foreach (var classInfo in classes.Values.OrderBy(ci => ci.ClassName)) {
        foreach (var mi in classInfo.Members.Values.OrderBy(mi => mi.MemberName)) {
          if (mi.NeedsDictionary) {
            sw.WriteLine($"      {classInfo.PluralName}By{mi.MemberName} = new Dictionary<{mi.TypeString.Replace("?", "")}, {classInfo.ClassName}>();");
         }
        }
      }

      sw.WriteLine("      var storeKey = 0;");
      sw.WriteLine("      if (csvConfig==null) {");
      foreach (var classInfo in parentChildTree) {
        sw.WriteLine($"        {classInfo.PluralName} = new DataStore<{classInfo.ClassName}>(");
        sw.WriteLine("          this,");
        sw.WriteLine("          storeKey,");
        sw.WriteLine($"          {classInfo.ClassName}.SetKey,");
        sw.WriteLine($"          {classInfo.ClassName}.RollbackItemStore,");
        if (classInfo.AreInstancesUpdatable) {
          sw.WriteLine($"          {classInfo.ClassName}.RollbackItemUpdate,");
        } else {
          sw.WriteLine($"          null,");
        }
        if (classInfo.AreInstancesDeletable) {
          sw.WriteLine($"          {classInfo.ClassName}.RollbackItemRemove,");
        } else {
          sw.WriteLine($"          null,");
        }
        if (classInfo.AreInstancesDeletable && classInfo.IsDisconnectNeeded) {
          sw.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        } else {
          sw.WriteLine($"          null,");
        }
        sw.WriteLine($"          areInstancesUpdatable: {classInfo.AreInstancesUpdatable.ToString().ToLowerInvariant()},");
        sw.WriteLine($"          areInstancesDeletable: {classInfo.AreInstancesDeletable.ToString().ToLowerInvariant()});");
        sw.WriteLine($"        DataStores[storeKey++] = {classInfo.PluralName};");
        sw.WriteLine($"        on{classInfo.PluralName}Filled();");
      }
      sw.WriteLine("      } else {");
      foreach (var classInfo in parentChildTree) {
        sw.WriteLine($"        {classInfo.PluralName} = new DataStoreCSV<{classInfo.ClassName}>(");
        sw.WriteLine("          this,");
        sw.WriteLine("          storeKey,");
        sw.WriteLine("          csvConfig!,");
        sw.WriteLine($"          {classInfo.ClassName}.EstimatedLineLength,");
        sw.WriteLine($"          {classInfo.ClassName}.Headers,");
        sw.WriteLine($"          {classInfo.ClassName}.SetKey,");
        sw.WriteLine($"          {classInfo.ClassName}.Create,");
        //if (classInfo.ParentsWithList.Count>0) {
        if (classInfo.ParentsAll.Count>0) {
          sw.WriteLine($"          {classInfo.ClassName}.Verify,");
        } else {
          sw.WriteLine("          null,");
        }
        if (classInfo.AreInstancesUpdatable) {
          sw.WriteLine($"          {classInfo.ClassName}.Update,");
        } else {
          sw.WriteLine($"          null,");
        }
        sw.WriteLine($"          {classInfo.ClassName}.Write,");
        sw.WriteLine($"          {classInfo.ClassName}.RollbackItemStore,");
        if (classInfo.AreInstancesUpdatable) {
          sw.WriteLine($"          {classInfo.ClassName}.RollbackItemUpdate,");
        } else {
          sw.WriteLine($"          null,");
        }
        if (classInfo.AreInstancesDeletable) {
          sw.WriteLine($"          {classInfo.ClassName}.RollbackItemRemove,");
        } else {
          sw.WriteLine($"          null,");
        }
        if (classInfo.AreInstancesDeletable && classInfo.IsDisconnectNeeded) {
          sw.WriteLine($"          {classInfo.ClassName}.Disconnect,");
        } else {
          sw.WriteLine($"          null,");
        }
        sw.WriteLine($"          areInstancesUpdatable: {classInfo.AreInstancesUpdatable.ToString().ToLowerInvariant()},");
        sw.WriteLine($"          areInstancesDeletable: {classInfo.AreInstancesDeletable.ToString().ToLowerInvariant()});");
        sw.WriteLine($"        DataStores[storeKey++] = {classInfo.PluralName};");
        sw.WriteLine($"        on{classInfo.PluralName}Filled();");
      }
      sw.WriteLine("      }");
      sw.WriteLine("      onConstructed();");
      sw.WriteLine("      IsInitialised = true;");
      sw.WriteLine("    }");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>}");
      sw.WriteLine("    /// Called at beginning of constructor");
      sw.WriteLine("    /// </summary>}");
      sw.WriteLine("    partial void onConstructing(string? backupResult);");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>}");
      sw.WriteLine("    /// Called at end of constructor");
      sw.WriteLine("    /// </summary>}");
      sw.WriteLine("    partial void onConstructed();");
      foreach (var classInfo in parentChildTree) {
        sw.WriteLine();
        sw.WriteLine("    /// <summary>}");
        sw.WriteLine($"    /// Called once the data for {classInfo.PluralName} is read.");
        sw.WriteLine("    /// </summary>}");
        sw.WriteLine($"    partial void on{classInfo.PluralName}Filled();");
      }
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    #region IDisposable Support");
      sw.WriteLine("    //      -------------------");
      sw.WriteLine();
      sw.WriteLine("    protected override void Dispose(bool disposing) {");
      sw.WriteLine("      if (disposing) {");
      sw.WriteLine("        onDispose();");
      foreach (var classInfo in ((IEnumerable<ClassInfo>)parentChildTree).Reverse()) {
        sw.WriteLine($"        {classInfo.PluralName}.Dispose();");
        sw.WriteLine($"        {classInfo.PluralName} = null!;");
        foreach (var mi in classInfo.Members.Values.OrderBy(mi => mi.MemberName)) {
          if (mi.NeedsDictionary) {
            sw.WriteLine($"        {classInfo.PluralName}By{mi.MemberName} = null!;");
          }
        }
      }
      sw.WriteLine("        data = null;");
      sw.WriteLine("      }");
      sw.WriteLine("      base.Dispose(disposing);");
      sw.WriteLine("    }");
      sw.WriteLine();
      sw.WriteLine("    /// <summary>}");
      sw.WriteLine("    /// Called before storageDirectories get disposed.");
      sw.WriteLine("    /// </summary>}");
      sw.WriteLine("    partial void onDispose();");
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("    #region Methods");
      sw.WriteLine("    //      -------");
      sw.WriteLine();
      sw.WriteLine("    #endregion");
      sw.WriteLine();
      sw.WriteLine("  }");
      sw.WriteLine("}");
      sw.WriteLine();
    }


    internal void WriteEnumsFile(DirectoryInfo targetDirectory) {
      var baseFileNameAndPath = targetDirectory!.FullName + '\\' + "Enums.base.cs";
      try {
        File.Delete(baseFileNameAndPath);
      } catch {
        throw new GeneratorException($"Cannot delete file {baseFileNameAndPath}.");
      }
      using var sw = new StreamWriter(baseFileNameAndPath);
      sw.WriteLine("using System;");
      sw.WriteLine("using System.Collections.Generic;");
      sw.WriteLine("using Storage;");
      sw.WriteLine();
      sw.WriteLine();
      sw.WriteLine("namespace " + nameSpaceString + " {");
      foreach (var enumInfo in enums.Values) {
        sw.WriteLine();
        sw.WriteLine();
        sw.WriteLine(enumInfo.CodeLines);
      }
      sw.WriteLine("}");
    }
  }
}
