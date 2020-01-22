using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

/*
https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Syntax-Analysis
https://github.com/dotnet/roslyn/wiki/Roslyn%20Overview
https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/special-issue/csharp-and-visual-basic-use-roslyn-to-write-a-live-code-analyzer-for-your-api

*/

namespace Storage {


  public class StorageClassGenerator {


    /// <summary>
    /// Reads all .cs files in sourceDirectoryString. The .cs file can contain one or several classes. If any class contains
    /// a method, it gets skipped, otherwise StorageClassGenerator generates for each class a new .base.cs file in targetDirectoryString, 
    /// adding all code needed for object oriented data storage. A corresponding.cs file gets created, if it doesn't exist yet, where
    /// more code can get added manually. The generator will not overwrite these changes.
    /// 
    /// <para/>
    /// The following constants can be used in a class to configure its
    /// behaviour:
    /// <para/>
    /// MaxLineLenght: Number of UTF8 bytes needed to store longest class instance in a CVS file
    /// <para/>
    /// AreItemsUpdatable, default true: Can the property values be changed once an instance is created ?
    /// <para/>
    /// AreItemsDeletable, default true: Can an created instance get removed from its StorageDirectory ?
    /// <para/>
    /// IsCompactDuringDispose, default true: When property values get changed or an instance deleted, this activity gets written 
    /// immediately to the CVS file. IsCompactDuringDispose is true, a new CSV file gets written containing only the still existing
    /// instances with their updated values.
    /// </summary>
    /// <param name="sourceDirectoryString">Source directory from where the .cs files get read.</param>
    /// <param name="targetDirectoryString">Target directory where the new .cs files get written.</param>
    /// <param name="context">Name of Context class, which gives static access to all data stored.</param>
    public StorageClassGenerator(string sourceDirectoryString, string targetDirectoryString, string context) {
      try {
        Console.WriteLine("Storage Class Generator");
        Console.WriteLine("**********************");
        Console.WriteLine();

        DirectoryInfo sourceDirectory = findDirectory(sourceDirectoryString, "source directory");
        DirectoryInfo targetDirectory= findDirectory(targetDirectoryString, "target directory");
        Console.WriteLine();

        var compiler = new Compiler();
        foreach (var file in sourceDirectory.GetFiles("*.cs")) {
          if (isModelFile(file, out NamespaceDeclarationSyntax? namespaceDeclaration)) {
            Console.WriteLine($"Parse {file.Name}");
            compiler.Parse(namespaceDeclaration!, file.Name);
          }
        }

        Console.WriteLine("analyze dependencies");
        compiler.AnalyzeDependencies();
        Console.WriteLine();

        Console.WriteLine("write class files");
        compiler.WriteClassFiles(targetDirectory, context);
        Console.WriteLine("");

        if (compiler.Enums.Count>0) {
          Console.WriteLine("write Enums.base.cs");
          compiler.WriteEnumsFile(targetDirectory);
          Console.WriteLine("");
        }

        Console.WriteLine($"write {context}.cs");
        compiler.WriteContextFile(targetDirectory, context);
        Console.WriteLine("");
      } catch (GeneratorException gex) {
        var oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("Error: " + gex.Message);
        Console.WriteLine();
        Console.ForegroundColor = oldForegroundColor;

      } catch (Exception ex) {
        var oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("Exception");
        Console.WriteLine(ex.ToString());
        Console.WriteLine();
        Console.ForegroundColor = oldForegroundColor;
      }
    }


    private DirectoryInfo findDirectory(string directoryString, string directoryName) {
      DirectoryInfo directory;
      try {
        directory = new DirectoryInfo(directoryString);
      } catch (Exception) {
        throw new GeneratorException($"Cannot find {directoryName} {directoryString}.");
      }
      if (!directory.Exists) {
        throw new GeneratorException($"Cannot find {directoryName} {directoryString}.");
      }
      Console.WriteLine($"{directoryName}: {directory.FullName}");
      return directory;
    }


    private bool isModelFile(FileInfo file, out NamespaceDeclarationSyntax? namespaceDeclaration) {
      var tree = CSharpSyntaxTree.ParseText(file.OpenText().ReadToEnd());
      if (!(tree.GetRoot() is CompilationUnitSyntax root)) {
        Console.WriteLine($"{file.Name} skipped, it cannot be converted to a compilation unit.");
        namespaceDeclaration = null;
        return false;
      }
      if (root.Members.Count!=1) {
        Console.WriteLine($"{file.Name} skipped, it has more than 1 namespace declaration in the compilation unit.");
        namespaceDeclaration = null;
        return false;
      }
      namespaceDeclaration = root.Members[0] as NamespaceDeclarationSyntax;
      if (namespaceDeclaration is null) {
        Console.WriteLine($"{file.Name} skipped, compilation unit does not contain just 1 namespace declaration.");
        return false;

      }
      foreach (var member in namespaceDeclaration.Members) {
        if (!(member is ClassDeclarationSyntax classDeclaration)) {
          if (!(member is EnumDeclarationSyntax)) {
            Console.WriteLine($"{file.Name} skipped, namespace does not contain just class and enum declarations.");
            return false;
          }
        } else {
          foreach (var classMember in classDeclaration.Members) {
            if (classMember is MethodDeclarationSyntax methodDeclaration) {
              Console.WriteLine($"{file.Name} skipped, it has a method {methodDeclaration.Identifier.Text}.");
              return false;
            }
          }
        }
      }
      return true;
    }
  }
}
