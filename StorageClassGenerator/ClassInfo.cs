/**************************************************************************************

Storage.ClassInfo
=================

Some infos about a class to be generated

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
using Microsoft.CodeAnalysis.CSharp.Syntax;

//////////////////////////////////////////////////////////////////////////////////////
// Todo: Improvements Storage Compiler for stored / not stored parents and children
// When creating a child instance which is not stored yet, it should get added to its parent's children collection only
// if the parent is not stored yet neither.
// When updating a child instance which is not stored yet, it should get added to its parent's children collection  only
// if the parent is not stored yet neither.
// During store a child needs to get added to its parent, unless it is added already
// Should there be an update() just to change the parent ? This is useful when a child was created without parent, but
// the parent gets added before storing.
//
//////////////////////////////////////////////////////////////////////////////////////


namespace Storage {

  /// <summary>
  /// Some infos about a class to be generated
  /// </summary>
  public class ClassInfo {
    public readonly string ClassName;
    public readonly string LowerClassName;
    public readonly string RawName;
    public readonly string LowerRawName;
    public readonly string ReaderName;
    public readonly string WriterName;
    public readonly string? ClassComment;
    public readonly int? MaxLineLength;
    public readonly string PluralName;
    public readonly string LowerPluralName;
    public readonly bool AreInstancesUpdatable;
    public readonly bool AreInstancesDeletable;
    public readonly bool IsConstructorPrivate;
    public readonly bool IsGenerateReaderWriter;
    public bool IsDisconnectNeeded = true;
    public readonly Dictionary<string, MemberInfo> Members;
    public readonly HashSet<ClassInfo> ParentsAll;
    //public readonly HashSet<ClassInfo> ParentsWithList; contains the same like ParentsAll ?
    public readonly List<ClassInfo> Children;

    public int EstimatedMaxByteSize;
    public bool IsAddedToParentChildTree;


    public ClassInfo(
      string name, 
      string? classComment, 
      int? maxLineLength, 
      string pluralName, 
      bool areInstancesUpdatable, 
      bool areInstancesDeletable,
      bool isConstructorPrivate,
      bool isGenerateReaderWriter)
    {
      ClassName = name;
      LowerClassName = name[0..1].ToLowerInvariant() + name[1..];
      RawName = name + "Raw";
      LowerRawName = LowerClassName + "Raw";
      ReaderName = name + "Reader";
      WriterName = name + "Writer";
      ClassComment = classComment;
      MaxLineLength = maxLineLength;
      PluralName = pluralName;
      LowerPluralName = pluralName[0..1].ToLowerInvariant() + pluralName[1..];
      AreInstancesUpdatable = areInstancesUpdatable;
      AreInstancesDeletable = areInstancesDeletable;
      IsConstructorPrivate = isConstructorPrivate;
      IsGenerateReaderWriter = isGenerateReaderWriter;
      Members = new Dictionary<string, MemberInfo>();
      ParentsAll = new HashSet<ClassInfo>();
      //ParentsWithList = new HashSet<ClassInfo>();
      Children = new List<ClassInfo>();
      EstimatedMaxByteSize = 0;
      IsAddedToParentChildTree = false;
    }


    public void AddMember(
      string name, 
      string csvTypeString, 
      string? propertyComment, 
      string ? defaultValue, 
      bool? isLookupOnly,
      bool needsDictionary,
      bool isParentOneChild,
      bool isReadOnly) 
    {
      var isLookUp = false;
      var isNullable = csvTypeString[^1]=='?';
      if (isNullable) {
        csvTypeString = csvTypeString[..^1];
      }
      MemberInfo member;
      var memberType = (csvTypeString.ToLowerInvariant()) switch
      {
        "date" => MemberTypeEnum.Date,
        "time" => MemberTypeEnum.Time,
        "dateminutes" => MemberTypeEnum.DateMinutes,
        "dateseconds" => MemberTypeEnum.DateSeconds,
        "datetime" => MemberTypeEnum.DateTime,
        "decimal" => MemberTypeEnum.Decimal,
        "decimal2" => MemberTypeEnum.Decimal2,
        "decimal4" => MemberTypeEnum.Decimal4,
        "decimal5" => MemberTypeEnum.Decimal5,
        "bool" => MemberTypeEnum.Bool,
        "int" => MemberTypeEnum.Int,
        "string" => MemberTypeEnum.String,
        _ => MemberTypeEnum.Undefined,
      };

      if (memberType!=MemberTypeEnum.Undefined) {
        //simple data type
        //----------------
        if (isLookUp || isParentOneChild) {
          throw new GeneratorException($"{ClassName}.{name} is of type {csvTypeString}. It " +
            "cannot have the attribute 'isLookUp' or 'isParentOneChild', which are only available for links to other classes.");
        }
        member = new MemberInfo(name, csvTypeString, memberType, this, isNullable, isReadOnly, propertyComment,
          defaultValue, needsDictionary);

      } else {
        //links to other classes
        if (needsDictionary) {
          throw new GeneratorException($"{ClassName}.{name} is of type {csvTypeString}. It " +
            "cannot have the attribute 'needsDictionary', which is only available for simple types.");
        }
        if (csvTypeString.Contains("<")) {
          //a parent having a collection for its children
          //=============================================

          if (isLookUp || isParentOneChild) {
            throw new GeneratorException($"{ClassName}.{name} a collection of type {csvTypeString}. One to many relationships " +
              "using collections cannot have the attributes 'isLookUp' or 'isParentOneChild'.");
          }
          if (csvTypeString.StartsWith("List<") && csvTypeString.EndsWith(">")) {
            //List
            //----
            if (isNullable) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} cannot be nullable.");

            member = new MemberInfo(name, this, csvTypeString, csvTypeString[5..^1], propertyComment, defaultValue);

          } else if ((csvTypeString.StartsWith("Dictionary<") || csvTypeString.StartsWith("SortedList<")) &&
            csvTypeString.EndsWith(">")) 
          {
            //Dictionary or SortedList
            //------------------------
            // memberTypeString: SortedList<DateTime /*DateOnly*/, Sample>
            // memberTypeString: Dictionary<DateTime /*DateOnly*/, Sample>
            if (isNullable) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} cannot be nullable.");

            var startCommentPos = csvTypeString.IndexOf("/*");
            if (startCommentPos<0) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} is " +
              "missing a comment '/* */' after the key type, indicating which child-property to use for that key.");
            var endCommentPos = csvTypeString.IndexOf("*/");
            if (endCommentPos<0) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} is " +
              "missing the comment closing '*/'.");
            if (endCommentPos<startCommentPos) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "Closing '*/' must come before '/*'.");
            var childKeyPropertyName = csvTypeString[(startCommentPos+2)..endCommentPos];
            var stringWithoutComment = csvTypeString[..(startCommentPos-1)] + csvTypeString[(endCommentPos+2)..];
            // stringWithoutComment: Dictionary<DateTime, Sample>
            var openBracketPos = stringWithoutComment.IndexOf('<');
            if (openBracketPos<0) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "Opening '<' is missing.");
            var closingBracketPos = stringWithoutComment.IndexOf('>');
            if (closingBracketPos<0) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "Closing '>' is missing.");
            var comaPos = stringWithoutComment.IndexOf(',');
            if (comaPos<0) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "',' is missing.");
            if (comaPos<openBracketPos) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "Coma ',' should come after '<'.");
            if (closingBracketPos<comaPos) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} " +
              "Coma ',' should come before '>'.");
            var keyTypeString = stringWithoutComment[(openBracketPos+1)..(comaPos)].Trim();
            var itemTypeName = stringWithoutComment[(comaPos+1)..(closingBracketPos)].Trim();
            if (csvTypeString.StartsWith("SortedList<")) {
              memberType = MemberTypeEnum.ParentMultipleChildrenSortedList;
            } else {
              memberType = MemberTypeEnum.ParentMultipleChildrenDictionary;
            }
            member = new MemberInfo(
              name,
              this,
              csvTypeString,
              memberType,
              itemTypeName,
              childKeyPropertyName,
              keyTypeString,
              propertyComment,
              defaultValue);

          } else {
            throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} not support. Should " +
              "it be a List<>, SortedList<,> or Dictionary<,> ?");
          }
        } else if (isParentOneChild) {

          //a parent having at most one child
          //=================================
          if (!isNullable) {
            throw new GeneratorException($"{ClassName}.{name} is a parent for at most 1 child. It must be nullable.");
          }
          //if (isReadOnly) {
          //  throw new GeneratorException($"{ClassName}.{name} is a parent for at most 1 child. It cannot be readonly.");
          //}
          member = new MemberInfo(name, this, csvTypeString, propertyComment, defaultValue);
        } else {

          //a child linking to its parent, i.e. a MemberTypeEnum.Parent
          //===========================================================
          member = new MemberInfo(name, this, csvTypeString, isNullable, isReadOnly, propertyComment, defaultValue, isLookupOnly??false);
          isLookUp = true;
        }
      }

      if (!isLookUp && isLookupOnly.HasValue) {
        throw new GeneratorException($"Class '{ClassName}.{name}': Remove [StorageProperty(isLookupOnly: {isLookupOnly.Value})], " +
            "it can only be applied when referencing a parent.");
      }
      Members.Add(member.MemberName, member);
      EstimatedMaxByteSize +=member.MaxStorageSize;
    }


    //internal void SetMaxLineLength(int maxLineLength) {
    //  MaxLineLength = maxLineLength;
    //}


    //internal void SetAreInstancesUpdatable(bool areInstancesUpdatable) {
    //  AreInstancesUpdatable = areInstancesUpdatable.ToString().ToLowerInvariant();
    //}


    //internal void SetAreInstancesDeletable(bool areInstancesDeletable) {
    //  AreInstancesDeletable  = areInstancesDeletable.ToString().ToLowerInvariant();
    //}


    public override string ToString() {
      return $"Class {ClassName}";
    }


    internal void WriteClassFile(StreamWriter streamWriter, string nameSpace, string context, bool isFullyCommented) {
      var cs /*CommentString*/= isFullyCommented ? "//" : "";
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Collections.Generic;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine($"namespace {nameSpace} {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      if (ClassComment!=null) {
        var linesArray = ClassComment.Split(Environment.NewLine);
        foreach (var line in linesArray) {
          if (!string.IsNullOrWhiteSpace(line)) {
            streamWriter.WriteLine($"  {line}");
          }
        }
      }
      streamWriter.WriteLine($"  public partial class {ClassName}: IStorage<{ClassName}> {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Properties");
      streamWriter.WriteLine("    //      ----------");
      streamWriter.WriteLine();
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
      streamWriter.WriteLine("    /// Called once the constructor has filled all the properties");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onConstruct() {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called once the CSV-constructor who reads the data from a CSV file has filled all the properties");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onCsvConstruct({context} context) {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Methods");
      streamWriter.WriteLine("    //      -------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called before storing gets executed");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onStore() {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called before the data gets written to a CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onCsvWrite() {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      if (this.AreInstancesUpdatable) {
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Called after all properties are updated, but before the HasChanged event gets raised");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.Write($"    {cs}partial void onUpdating(");
        if (!writeOnUpdateParameters(streamWriter, updateTypeEnum.Implementation, cs)) {
          throw new GeneratorException($"Method '{ClassName}.onUpdating()': has no parameters. Are all properties readonly ?" +
            " Then add attribute [StorageClass(areInstancesUpdatable: false] and remove readonly from the properties.");
        }
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Called after all properties are updated, but before the HasChanged event gets raised");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    {cs}partial void onUpdated() {{");
        streamWriter.WriteLine($"    {cs}}}");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Called after an update is read from a CSV file");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    {cs}partial void onCsvUpdate() {{");
        streamWriter.WriteLine($"    {cs}}}");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
      }
      if (AreInstancesDeletable) {
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Called before removal gets executed");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    {cs}partial void onRemove() {{");
        streamWriter.WriteLine($"    {cs}}}");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Called after a {mi.LowerChildTypeName} gets added to {mi.ChildClassInfo!.PluralName}.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    {cs}partial void onAddedTo{mi.ChildMemberInfo!.ParentMethodName}({mi.ChildTypeName} {mi.LowerChildTypeName}){{");
          streamWriter.WriteLine($"    {cs}}}");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          if (!mi.IsChildReadOnly) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Called after a {mi.LowerChildTypeName} gets removed from {mi.ChildClassInfo!.PluralName}.");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    {cs}partial void onRemovedFrom{mi.ChildMemberInfo!.ParentMethodName}({mi.ChildTypeName} {mi.LowerChildTypeName}){{");
            streamWriter.WriteLine($"    {cs}}}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
          }
        } else  if (mi.MemberType>MemberTypeEnum.ParentOneChild) { //List, Dictionary or SortedList
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Called after a {mi.LowerChildTypeName} gets added to {mi.ChildClassInfo!.PluralName}.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    {cs}partial void onAddedTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}){{");
          streamWriter.WriteLine($"    {cs}}}");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          if (!mi.IsChildReadOnly) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Called after a {mi.LowerChildTypeName} gets removed from {mi.ChildClassInfo!.PluralName}.");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    {cs}partial void onRemovedFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}){{");
            streamWriter.WriteLine($"    {cs}}}");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
          }
        }
      }
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Updates returnString with additional info for a short description.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onToShortString(ref string returnString) {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Updates returnString with additional info for a short description.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    {cs}partial void onToString(ref string returnString) {{");
      streamWriter.WriteLine($"    {cs}}}");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine("  }");
      streamWriter.WriteLine("}");
    }


    public void WriteBaseClassFile(StreamWriter streamWriter, string nameSpace, string context) {
      streamWriter.WriteLine("//------------------------------------------------------------------------------");
      streamWriter.WriteLine("// <auto-generated>");
      streamWriter.WriteLine("//     This code was generated by StorageClassGenerator");
      streamWriter.WriteLine("//");
      streamWriter.WriteLine("//     Do not change code in this file, it will get lost when the file gets ");
      streamWriter.WriteLine($"//     auto generated again. Write your code into {ClassName}.cs.");
      streamWriter.WriteLine("// </auto-generated>");
      streamWriter.WriteLine("//------------------------------------------------------------------------------");
      streamWriter.WriteLine("#nullable enable");
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Collections.Generic;");
      if (IsGenerateReaderWriter) {
        streamWriter.WriteLine("using System.Diagnostics.CodeAnalysis;");
      }
      streamWriter.WriteLine("using System.Threading;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine($"namespace {nameSpace} {{");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      if (ClassComment!=null) {
        var linesArray = ClassComment.Split(Environment.NewLine);
        foreach (var line in linesArray) {
          if (!string.IsNullOrWhiteSpace(line)) {
            streamWriter.WriteLine($"  {line}");
          }
        }
      }
      streamWriter.WriteLine($"  public partial class {ClassName}: IStorage<{ClassName}> {{");
      streamWriter.WriteLine();
      #region Properties
      //      ----------

      streamWriter.WriteLine("    #region Properties");
      streamWriter.WriteLine("    //      ----------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Unique identifier for {ClassName}. Gets set once {ClassName} gets added to {context}.Data.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public int Key { get; private set; }");
      streamWriter.WriteLine($"    internal static void SetKey({ClassName} {LowerClassName}, int key) {{ {LowerClassName}.Key = key; }}");

      foreach (var mi in Members.Values) {
        mi.WriteProperty(streamWriter);
      }
      streamWriter.WriteLine();
      streamWriter.WriteLine();

      #region Headers
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Headers written to first line in CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write("    internal static readonly string[] Headers = {");
      var lines = new List<string>();
      if (AreInstancesDeletable || AreInstancesUpdatable) {
        lines.Add("\"Key\"");
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType<MemberTypeEnum.ParentOneChild ) {//not List, Dictionary nor SortedList
          lines.Add($"\"{mi.MemberName}\"");
        }
      }
      for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++) {
        var line = lines[lineIndex];
        if (lines.Count>5) {
          streamWriter.WriteLine();
          streamWriter.Write("      ");
        }
        streamWriter.Write(line);
        if (lineIndex+1<lines.Count) {
          streamWriter.Write(", ");
        }
      }
      if (lines.Count>5) {
        streamWriter.WriteLine();
        streamWriter.WriteLine("    };");
      } else {
        streamWriter.WriteLine("};");
      }
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region static NoClass singleton
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// None existing " + ClassName);
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write($"    internal static {ClassName} No{ClassName} = new {ClassName}(");
      foreach (var mi in Members.Values) {
        if (mi.MemberType<=MemberTypeEnum.LinkToParent) {//simple data type, link to parent
          if (mi.MemberType==MemberTypeEnum.Enum) {
            streamWriter.Write(0 + ", ");
          } else {
            streamWriter.Write(mi.NoValue + ", ");
          }
        }
      }
      streamWriter.WriteLine("isStoring: false);");
      #endregion
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Events
      //      ------

      streamWriter.WriteLine("    #region Events");
      streamWriter.WriteLine("    //      ------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      if (AreInstancesUpdatable) {
        streamWriter.WriteLine($"    /// Content of {ClassName} has changed. Gets only raised for changes occurring after loading {context}.Data with previously stored data.");
      } else {
        streamWriter.WriteLine($"    /// This event will never be raised, but is needed to comply with IStorage.");
      }
      streamWriter.WriteLine("    /// </summary>");
      if (!AreInstancesUpdatable) {
        streamWriter.WriteLine("#pragma warning disable 67");
      }
      streamWriter.WriteLine($"    public event Action<{ClassName}>? HasChanged;");
      if (!AreInstancesUpdatable) {
        streamWriter.WriteLine("#pragma warning restore 67");
      }
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Constructors
      //      ------------

      streamWriter.WriteLine("    #region Constructors");
      streamWriter.WriteLine("    //      ------------");
      streamWriter.WriteLine();

      #region Public constructor
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.Write($"    /// {ClassName} Constructor. If isStoring is true, adds {ClassName} to {context}.Data.{PluralName}");
      lines.Clear();
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          if (mi.IsNullable) {
            lines.Add($"if there is a {mi.MemberName} adds {ClassName} to {mi.LowerParentType}.{PluralName}");
          } else {
            lines.Add($"adds {ClassName} to {mi.LowerParentType}.{PluralName}");
          }
        }
      }
      if (lines.Count>0) {
        for (int linesIndex = 0; linesIndex < lines.Count; linesIndex++) {
          var line = lines[linesIndex];
          if (linesIndex+1==lines.Count) {
            streamWriter.WriteLine();
            streamWriter.Write("    /// and " + line);
          } else {
            streamWriter.WriteLine(", ");
            streamWriter.Write("    /// " + line);
          }
        }
      }
      streamWriter.WriteLine(".");
      streamWriter.WriteLine("    /// </summary>");
      if (IsConstructorPrivate) {
        streamWriter.Write($"    private {ClassName}(");
      } else {
        streamWriter.Write($"    public {ClassName}(");
      }
      writeParameters(streamWriter, isConstructor: true);
      streamWriter.WriteLine("      Key = StorageExtensions.NoKey;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
          //nothing to do
        } else if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenList) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        } else if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenDictionary || 
          mi.MemberType==MemberTypeEnum.ParentMultipleChildrenSortedList) 
        {
          streamWriter.WriteLine($"      {mi.LowerMemberName} = new {mi.TypeString}();");
        } else {
          streamWriter.WriteLine($"      {mi.MemberName} = {mi.LowerMemberName}{mi.Rounding};");
          //if (mi.MemberType==MemberTypeEnum.Parent  && !mi.IsLookupOnly) {
          //  if (mi.IsNullable) {
          //    streamWriter.WriteLine($"      if ({mi.MemberName}!=null && {mi.MemberName}.Key<0) {{");
          //    streamWriter.WriteLine($"        {mi.MemberName}.AddTo{PluralName}(this);");
          //    streamWriter.WriteLine("      }");
          //  } else {
          //    streamWriter.WriteLine($"      if ({mi.MemberName}.Key<0 && {mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
          //    streamWriter.WriteLine($"        {mi.MemberName}.AddTo{PluralName}(this);");
          //    streamWriter.WriteLine("      }");
          //  }
          //}
        }
      }
      streamWriter.WriteLine("      onConstruct();");
      streamWriter.WriteLine();
      streamWriter.WriteLine("      if (isStoring) {");
      streamWriter.WriteLine("        Store();");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onConstruct();");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region private constructor
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Constructor for {ClassName} read from CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write($"    private {ClassName}(int key, CsvReader csvReader, {context} ");
      //if (ParentsAll.Count>0) {
      //  streamWriter.Write("context");
      //} else {
      //  streamWriter.Write("_");
      //}
      streamWriter.Write("context");// context always needed for onCsvConstruct
      streamWriter.WriteLine(") {");
      streamWriter.WriteLine("      Key = key;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
          //nothing to do
        } else if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenList) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        } else if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenDictionary ||
          mi.MemberType==MemberTypeEnum.ParentMultipleChildrenSortedList) 
        {
          streamWriter.WriteLine($"      {mi.LowerMemberName} = new {mi.TypeString}();");
        } else if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      var {mi.LowerMemberName}Key = csvReader.ReadIntNull();");
            streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key.HasValue) {{");
            streamWriter.WriteLine($"        if (context.{mi.ParentClassInfo!.PluralName}.TryGetValue({mi.LowerMemberName}Key.Value, " +
              $"out var {mi.LowerMemberName})) {{");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
            streamWriter.WriteLine("        } else {");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.ParentTypeString}.No{mi.ParentTypeString};");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      var {mi.ParentClassInfo!.LowerClassName}Key = csvReader.ReadInt();");
            streamWriter.WriteLine($"      if (context.{mi.ParentClassInfo!.PluralName}." +
              $"TryGetValue({mi.ParentClassInfo!.LowerClassName}Key, out var {mi.LowerMemberName})) {{");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
            streamWriter.WriteLine($"      }} else {{");
            streamWriter.WriteLine($"        throw new Exception($\"Read {mi.ClassInfo.ClassName} from CSV file: Cannot find " +
              $"{mi.MemberName} with key {{{mi.ParentClassInfo!.LowerClassName}Key}}.\" + Environment.NewLine + ");
            streamWriter.WriteLine($"          csvReader.PresentContent);");
            streamWriter.WriteLine($"      }}");
          }
        } else {
          //simple data type
          if (mi.MemberType==MemberTypeEnum.Enum) {
          streamWriter.WriteLine($"      {mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
          } else {
            streamWriter.WriteLine($"      {mi.MemberName} = csvReader.{mi.CsvReaderRead};");
          }
          if (mi.NeedsDictionary) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({mi.MemberName}!=null) {{");
              streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
              streamWriter.WriteLine($"      }}");
            } else {
              streamWriter.WriteLine($"      {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
            }
          }
        }
      }
      //if the parent is a Dictionary or SortedList, the key property must be assigned first (i.e. in the for loop 
      //above), before the child gets added to the parent
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent && !mi.IsLookupOnly) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key.HasValue && {mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
            streamWriter.WriteLine($"        {mi.MemberName}!.AddTo{mi.ParentMethodName}(this);");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
            streamWriter.WriteLine($"        {mi.MemberName}.AddTo{mi.ParentMethodName}(this);");
            streamWriter.WriteLine("      }");
          }
        }
      }
      streamWriter.WriteLine("      onCsvConstruct(context);");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine($"    partial void onCsvConstruct({context} context);");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Create()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// New {ClassName} read from CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static {ClassName} Create(int key, CsvReader csvReader, {context} context) {{");
      streamWriter.WriteLine($"      return new {ClassName}(key, csvReader, context);");
      streamWriter.WriteLine("    }");
      #endregion

      #region Verify()
      var commentLines = new List<string>();
      var codeLines = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          commentLines.Add($"    /// Verify that {LowerClassName}.{mi.MemberName} exists.");
          codeLines.Add($"      if ({LowerClassName}.{mi.MemberName}=={mi.ParentTypeString}.No{mi.ParentTypeString}) return false;");
        }
      }
      if (commentLines.Count>0) {
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        foreach (var commentLine in commentLines) {
          streamWriter.WriteLine(commentLine);
        }
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    internal static bool Verify({ClassName} {LowerClassName}) {{");
        foreach (var codeLine in codeLines) {
          streamWriter.WriteLine(codeLine);
        }
        streamWriter.WriteLine($"      return true;");
        streamWriter.WriteLine("    }");

      }
      #endregion
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Methods
      //      -------

      streamWriter.WriteLine("    #region Methods");
      streamWriter.WriteLine("    //      -------");
      streamWriter.WriteLine();

      #region Store()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.Write($"    /// Adds {ClassName} to {context}.Data.{PluralName}");
      var parts = new List<string>();
      //foreach (var parent in ParentsWithList) {
      foreach (var parent in ParentsAll) {
        parts.Add($"{parent.ClassName}");
      }
      if (parts.Count>0) {
        for (int partIndex = 0; partIndex < parts.Count; partIndex++) {
          if (partIndex+1<parts.Count) {
            streamWriter.Write(", ");
          } else {
            streamWriter.Write(" and ");
          }
          streamWriter.Write(parts[partIndex]);
        }
      }
      streamWriter.WriteLine(". ");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public void Store() {");
      streamWriter.WriteLine("      if (Key>=0) {");
      streamWriter.WriteLine($"        throw new Exception($\"{ClassName} cannot be stored again in {context}.Data, " +
        $"key is {{Key}} greater equal 0.\" + Environment.NewLine + ToString());");
      streamWriter.WriteLine("      }");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent && !mi.IsLookupOnly) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({mi.MemberName}!=null && {mi.MemberName}.Key<0) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{ClassName} cannot be stored in {context}.Data, " +
              $"{mi.MemberName} is not stored yet.\" + Environment.NewLine + ToString());");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({mi.MemberName}.Key<0) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{ClassName} cannot be stored in {context}.Data, " +
              $"{mi.MemberName} is missing or not stored yet.\" + Environment.NewLine + ToString());");
            streamWriter.WriteLine("      }");
          }
        }
      }
      streamWriter.WriteLine("      onStore();");
      streamWriter.WriteLine($"      {context}.Data.{PluralName}.Add(this);");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          if (!mi.IsLookupOnly) {
            if (mi.IsNullable) {
              //streamWriter.WriteLine($"      if ({mi.Name}!=null) {{");
              streamWriter.WriteLine($"      {mi.MemberName}?.AddTo{mi.ParentMethodName}(this);");
              //streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      {mi.MemberName}.AddTo{mi.ParentMethodName}(this);");
            }
          }
        } else if (mi.MemberType<MemberTypeEnum.LinkToParent) {
          //simple data type
          if (mi.NeedsDictionary) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({mi.MemberName}!=null) {{");
              streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
              streamWriter.WriteLine($"      }}");
            } else {
              streamWriter.WriteLine($"      {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
            }
          }
        }
      }
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onStore();");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Maximal number of UTF8 characters needed to write {ClassName} to CSV file");
      streamWriter.WriteLine("    /// </summary>");
      if (MaxLineLength is null) {
        streamWriter.WriteLine($"    public const int MaxLineLength = {EstimatedMaxByteSize};");
      } else {
        streamWriter.WriteLine($"    public const int MaxLineLength = {MaxLineLength};");
      }
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Write()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Write {ClassName} to CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static void Write({ClassName} {LowerClassName}, CsvWriter csvWriter) {{");
      streamWriter.WriteLine($"      {LowerClassName}.onCsvWrite();");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName} is null) {{");
            streamWriter.WriteLine("        csvWriter.WriteNull();");
            streamWriter.WriteLine("      } else {");
            streamWriter.WriteLine($"        if ({LowerClassName}.{mi.MemberName}.Key<0) throw new Exception($\"Cannot write" +
              $" {LowerClassName} '{{{LowerClassName}}}' to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"        csvWriter.Write({LowerClassName}.{mi.MemberName}.Key.ToString());");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}.Key<0) throw new Exception($\"Cannot write {LowerClassName} '{{{LowerClassName}}}'" +
              $" to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
            streamWriter.WriteLine();
            streamWriter.WriteLine($"      csvWriter.Write({LowerClassName}.{mi.MemberName}.Key.ToString());");
          }
        } else if (mi.MemberType==MemberTypeEnum.Enum) {
          streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}((int){LowerClassName}.{mi.MemberName});");
        } else if (mi.MemberType<MemberTypeEnum.Enum) {//simple data type
          streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}({LowerClassName}.{mi.MemberName});");
        }
      }
      //Todo: ClassInfo: change methods like onCsvWrite to onCsvWriting and onCsvWriten, add them also to methods like internal void RemoveParentNullable
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onCsvWrite();");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      if (AreInstancesUpdatable) {
        #region Public Update()
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Updates {ClassName} with the provided values");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.Write("    public void Update(");
        if (!writeParameters(streamWriter, isConstructor: false)) {
          throw new GeneratorException($"Method '{ClassName}.Update()': has no parameters. Are all properties readonly ?" +
            " Then add attribute [StorageClass(areInstancesUpdatable: false] and remove readonly from the properties.");
        }
        //call onUpdating()
        streamWriter.WriteLine("      var isCancelled = false;");
        streamWriter.Write("      onUpdating(");
        if (!writeOnUpdateParameters(streamWriter, updateTypeEnum.Call)) {
          throw new GeneratorException($"Method '{ClassName}.onUpdating()': has no parameters. Are all properties readonly ?" +
            " Then add attribute [StorageClass(areInstancesUpdatable: false] and remove readonly from the properties.");
        }
        streamWriter.WriteLine("      if (isCancelled) return;");
        streamWriter.WriteLine();
        //update code and detect change
        streamWriter.WriteLine("      var isChangeDetected = false;");
        foreach (var mi in Members.Values) {
          if (mi.MemberType<=MemberTypeEnum.LinkToParent && //simple data type
            !mi.IsReadOnly) 
          {
            if (mi.MemberType==MemberTypeEnum.LinkToParent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      if ({mi.MemberName} is null) {{");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine("          //nothing to do");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {mi.MemberName}.AddTo{mi.ParentMethodName}(this);");
                streamWriter.WriteLine("          isChangeDetected = true;");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine($"          {mi.MemberName}.RemoveFrom{mi.ParentMethodName}(this);");
                streamWriter.WriteLine($"          {mi.MemberName} = null;");
                streamWriter.WriteLine("          isChangeDetected = true;");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          if ({mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"            {mi.MemberName}.RemoveFrom{mi.ParentMethodName}(this);");
                streamWriter.WriteLine($"            {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"            {mi.MemberName}.AddTo{mi.ParentMethodName}(this);");
                streamWriter.WriteLine("            isChangeDetected = true;");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"        {mi.MemberName}.RemoveFrom{mi.ParentMethodName}(this);");
                streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"        {mi.MemberName}.AddTo{mi.ParentMethodName}(this);");
                streamWriter.WriteLine("        isChangeDetected = true;");
                streamWriter.WriteLine("      }");
              }
            } else {
              if (mi.Rounding!=null) {
                streamWriter.WriteLine($"      var {mi.LowerMemberName}Rounded = {mi.LowerMemberName}{mi.Rounding};");
                streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}Rounded) {{");
                if (mi.NeedsDictionary) {
                  if (mi.IsNullable) {
                    streamWriter.WriteLine($"        if ({mi.MemberName}!=null) {{");
                    streamWriter.WriteLine($"          {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({mi.MemberName});");
                    streamWriter.WriteLine($"        }}");
                  } else {
                    streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({mi.MemberName});");
                  }
                }
                streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName}Rounded;");
              } else {
                streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}) {{");
                if (mi.NeedsDictionary) {
                  if (mi.IsNullable) {
                    streamWriter.WriteLine($"        if ({mi.MemberName}!=null) {{");
                    streamWriter.WriteLine($"          {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({mi.MemberName});");
                    streamWriter.WriteLine($"        }}");
                  } else {
                    streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({mi.MemberName});");
                  }
                }
                streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName};");
              }
              if (mi.NeedsDictionary) {
                if (mi.IsNullable) {
                  streamWriter.WriteLine($"        if ({mi.MemberName}!=null) {{");
                  streamWriter.WriteLine($"          {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
                  streamWriter.WriteLine($"        }}");
                } else {
                  streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({mi.MemberName}, this);");
                }
              }
              streamWriter.WriteLine("        isChangeDetected = true;");
              streamWriter.WriteLine("      }");
            }
          }
        }
        //call updated()
        streamWriter.WriteLine("      if (isChangeDetected) {");
        streamWriter.WriteLine("        onUpdated();");
        streamWriter.WriteLine("        HasChanged?.Invoke(this);");
        streamWriter.WriteLine("      }");
        streamWriter.WriteLine("    }");
        streamWriter.Write("    partial void onUpdating(");
        if (!writeOnUpdateParameters(streamWriter, updateTypeEnum.Definition)) {
          throw new GeneratorException($"Method '{ClassName}.onUpdating()': has no parameters. Are all properties readonly ?" +
            " Then add attribute [StorageClass(areInstancesUpdatable: false] and remove readonly from the properties.");
        }
        streamWriter.WriteLine("    partial void onUpdated();");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion

        #region Internal Update()
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Updates this {ClassName} with values from CSV file");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.Write($"    internal static void Update({ClassName} {LowerClassName}, CsvReader csvReader, {context} ");
        //if (ParentsWithList.Count>0) {
        if (ParentsAll.Count>0) {
          streamWriter.Write("context");
        } else {
          streamWriter.Write("_");
        }
        streamWriter.WriteLine(") {");
        foreach (var mi in Members.Values) {
          if (mi.IsReadOnly) {
            streamWriter.WriteLine($"      var value = csvReader.{mi.CsvReaderRead};");
            streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=value) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{ClassName}.Update(): Property {mi.MemberName}" +
              $" '{{{LowerClassName}.{mi.MemberName}}}' is \" +");
            streamWriter.WriteLine($"          $\"readonly, the value '{{value}}' read from the CSV file should be the" +
              " same.\" + Environment.NewLine + "); 
            streamWriter.WriteLine($"          {LowerClassName}.ToString());");
            streamWriter.WriteLine("      }");
          } else {
            if (mi.MemberType==MemberTypeEnum.LinkToParent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      var {mi.LowerMemberName}Key = csvReader.ReadIntNull();");
                streamWriter.WriteLine($"      {mi.ParentTypeString}? {mi.LowerMemberName};");
                streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key is null) {{");
                streamWriter.WriteLine($"        {mi.LowerMemberName} = null;");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if (!context.{mi.ParentClassInfo!.PluralName}.TryGetValue({mi.LowerMemberName}Key.Value, out {mi.LowerMemberName})) {{");
                streamWriter.WriteLine($"          {mi.LowerMemberName} = {mi.ParentTypeString}.No{mi.ParentTypeString};");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName} is null) {{");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine("          //nothing to do");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.AddTo{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine($"          if ({LowerClassName}.{mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
                streamWriter.WriteLine($"            {LowerClassName}.{mi.MemberName}.RemoveFrom{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = null;");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          if ({LowerClassName}.{mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
                streamWriter.WriteLine($"            {LowerClassName}.{mi.MemberName}.RemoveFrom{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.AddTo{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if (!context.{mi.ParentClassInfo!.PluralName}.TryGetValue(csvReader.ReadInt(), out var {mi.LowerMemberName})) {{");
                streamWriter.WriteLine($"        {mi.LowerMemberName} = {mi.ParentTypeString}.No{mi.ParentTypeString};");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"        if ({LowerClassName}.{mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.RemoveFrom{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.AddTo{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("      }");
              }
            } else if (mi.MemberType<MemberTypeEnum.LinkToParent) {//simple type
              if (mi.NeedsDictionary) {
                if (mi.IsNullable) {
                  streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null) {{");
                  streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({LowerClassName}.{mi.MemberName});");
                  streamWriter.WriteLine($"      }}");
                } else {
                  streamWriter.WriteLine($"      {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({LowerClassName}.{mi.MemberName});");
                }
              }
              if (mi.MemberType==MemberTypeEnum.Enum) {
                streamWriter.WriteLine($"      {LowerClassName}.{mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
              } else {
                streamWriter.WriteLine($"      {LowerClassName}.{mi.MemberName} = csvReader.{mi.CsvReaderRead};");
              }
              if (mi.NeedsDictionary) {
                if (mi.IsNullable) {
                  streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null) {{");
                  streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({LowerClassName}.{mi.MemberName}, {LowerClassName});");
                  streamWriter.WriteLine($"      }}");
                } else {
                  streamWriter.WriteLine($"      {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Add({LowerClassName}.{mi.MemberName}, {LowerClassName});");
                }
              }
            }
          }
        }
        streamWriter.WriteLine($"      {LowerClassName}.onCsvUpdate();");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("    partial void onCsvUpdate();");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion
      }

      #region AddTo() and RemoveFrom()
      foreach (var mi in Members.Values) {
        if (mi.MemberType>=MemberTypeEnum.ParentOneChild) {//ParentOneChild or ParentMultipleChildren

          if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Add {mi.LowerChildTypeName} to {mi.MemberName}.");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    internal void AddTo{mi.MemberName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
            streamWriter.WriteLine($"      if ({mi.MemberName}!=null) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{mi.ClassInfo.ClassName}.AddTo{mi.MemberName}(): " +
              $"'{{{mi.MemberName}}}' is already assigned to {mi.MemberName}, it is not possible to assign now '{{{mi.LowerChildTypeName}}}'.\");");
            streamWriter.WriteLine($"      }}");
            streamWriter.WriteLine($"      {mi.MemberName} = {mi.LowerChildTypeName};");
            streamWriter.WriteLine($"      onAddedTo{mi.MemberName}({mi.LowerChildTypeName});");
            streamWriter.WriteLine("    }");
            streamWriter.WriteLine($"    partial void onAddedTo{mi.MemberName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
            streamWriter.WriteLine();
            streamWriter.WriteLine();

          } else if (mi.MemberType>MemberTypeEnum.ParentOneChild) {//List, Dictionary or SortedList
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Add {mi.LowerChildTypeName} to {mi.ChildClassInfo!.PluralName}.");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    internal void AddTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
            if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenList) {
              streamWriter.WriteLine($"      {mi.ChildClassInfo!.LowerPluralName}.Add({mi.LowerChildTypeName});");
            } else { //Dictionary or SortedList
              streamWriter.WriteLine($"      {mi.ChildClassInfo!.LowerPluralName}.Add({mi.LowerChildTypeName}.{mi.ChildKeyPropertyName}, {mi.LowerChildTypeName});");
            }
            streamWriter.WriteLine($"      onAddedTo{mi.ChildClassInfo!.PluralName}({mi.LowerChildTypeName});");
            streamWriter.WriteLine("    }");
            streamWriter.WriteLine($"    partial void onAddedTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
          }

          //RemoveFrom
          //----------
          if (mi.ChildClassInfo!.AreInstancesUpdatable || mi.ChildClassInfo.AreInstancesDeletable) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Removes {mi.LowerChildTypeName} from {mi.ClassInfo!.ClassName}.");
            streamWriter.WriteLine("    /// </summary>");

            if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
              streamWriter.WriteLine($"    internal void RemoveFrom{mi.MemberName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
              streamWriter.WriteLine($"#if DEBUG");
              streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerChildTypeName}) {{");
              streamWriter.WriteLine($"        throw new Exception($\"{mi.ClassInfo!.ClassName}.RemoveFrom{mi.MemberName}(): {mi.MemberName} does not link to {mi.LowerChildTypeName} '{{{mi.LowerChildTypeName}}}' but '{{{mi.MemberName}}}'.\");");
              streamWriter.WriteLine($"      }}");
              streamWriter.WriteLine($"#endif");
              streamWriter.WriteLine($"      {mi.MemberName} = null;");
              streamWriter.WriteLine($"      onRemovedFrom{mi.MemberName}({mi.LowerChildTypeName});");
              streamWriter.WriteLine($"    }}");
              streamWriter.WriteLine($"    partial void onRemovedFrom{mi.MemberName}({mi.ChildTypeName} {mi.LowerChildTypeName});");

            } else {//ParentMultipleChildren
              streamWriter.WriteLine($"    internal void RemoveFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
              if (mi.MemberType==MemberTypeEnum.ParentMultipleChildrenList) {
                var linksLines = new List<string>();
                foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
                  if (childMI.MemberType==MemberTypeEnum.LinkToParent && (childMI.ParentTypeString!)==ClassName) {
                    linksLines.Add($"      if ({childMI.ClassInfo.LowerClassName}.{childMI.MemberName}==this ) countLinks++;");
                  }
                }
                if (linksLines.Count<1) throw new Exception();

                if (linksLines.Count==1) {
                  streamWriter.WriteLine("#if DEBUG");
                  streamWriter.WriteLine($"      if (!{mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName})) throw new Exception();");
                  streamWriter.WriteLine("#else");
                  streamWriter.WriteLine($"        {mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName});");
                  streamWriter.WriteLine("#endif");
                } else {
                  streamWriter.WriteLine("      //Execute Remove() only when exactly one property in the child still links to this parent. If");
                  streamWriter.WriteLine("      //no property links here (Count=0), the child should not be in the children collection. If");
                  streamWriter.WriteLine("      //more than 1 child property links here, it cannot yet be removed from the children collection.");
                  streamWriter.WriteLine("      var countLinks = 0;");
                  foreach (var linksLine in linksLines) {
                    streamWriter.WriteLine(linksLine);
                  }
                  streamWriter.WriteLine("      if (countLinks>1) return;");
                  streamWriter.WriteLine("#if DEBUG");
                  streamWriter.WriteLine("      if (countLinks==0) throw new Exception();");
                  streamWriter.WriteLine($"      if (!{mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName})) throw new Exception();");
                  streamWriter.WriteLine("#else");
                  streamWriter.WriteLine($"        {mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName});");
                  streamWriter.WriteLine("#endif");
                }
              } else { //Dictionary or SortedList
                streamWriter.WriteLine("#if DEBUG");
                streamWriter.WriteLine($"      if (!{mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName}.{mi.ChildKeyPropertyName})) throw new Exception();");
                streamWriter.WriteLine("#else");
                streamWriter.WriteLine($"        {mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName}.{mi.ChildKeyPropertyName});");
                streamWriter.WriteLine("#endif");
              }

              streamWriter.WriteLine($"      onRemovedFrom{mi.ChildClassInfo!.PluralName}({mi.LowerChildTypeName});");
              streamWriter.WriteLine("    }");
              streamWriter.WriteLine($"    partial void onRemovedFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine();
          }
        }
      }
      #endregion

      if (AreInstancesDeletable) {
        #region Remove()
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.Write($"    /// Removes {ClassName} from {context}.Data.{PluralName}");
        IsDisconnectNeeded = writeRemoveComment(streamWriter, context, isCapitaliseFirstLetter: false);
        streamWriter.WriteLine(".");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public void Remove() {");
        streamWriter.WriteLine("      if (Key<0) {");
        streamWriter.WriteLine($"        throw new Exception($\"{ClassName}.Remove(): {ClassName} '{this}' is not stored in {context}.Data, key is {{Key}}.\");");
        streamWriter.WriteLine("      }");
        streamWriter.WriteLine("      onRemove();");
        streamWriter.WriteLine($"      {context}.Data.{PluralName}.Remove(Key);");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("    partial void onRemove();");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion

        #region Disconnect()
        if (IsDisconnectNeeded) {
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.Write($"    /// ");
          writeRemoveComment(streamWriter, context, isCapitaliseFirstLetter: true);
          streamWriter.WriteLine(".");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal static void Disconnect({ClassName} {LowerClassName}) {{");
          foreach (var mi in Members.Values) {
            if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
              streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null) {{");
              //streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.Remove();");
              if (mi.ChildMemberInfo!.IsNullable) {
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.Remove();");
              } else {
                streamWriter.WriteLine($"         if ({LowerClassName}.{mi.MemberName}.Key>=0) {{");
                streamWriter.WriteLine($"           {LowerClassName}.{mi.MemberName}.Remove();");
                streamWriter.WriteLine("         }");
              }
              streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName} = null;");
              streamWriter.WriteLine("      }");
           
            } else if (mi.MemberType>MemberTypeEnum.ParentOneChild) { //ParentMultipleChildren
              switch (mi.MemberType) {
              case MemberTypeEnum.ParentMultipleChildrenList:
                streamWriter.WriteLine($"      for (int {mi.LowerChildTypeName}Index = {LowerClassName}.{mi.ChildClassInfo!.PluralName}.Count-1; {mi.LowerChildTypeName}Index>= 0; {mi.LowerChildTypeName}Index--) {{");
                streamWriter.WriteLine($"        var {mi.LowerChildTypeName} = {LowerClassName}.{mi.ChildClassInfo!.PluralName}[{mi.LowerChildTypeName}Index];");
                break;
              case MemberTypeEnum.ParentMultipleChildrenDictionary:
                streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {LowerClassName}.{mi.ChildClassInfo!.PluralName}.Values) {{");
                break;
              case MemberTypeEnum.ParentMultipleChildrenSortedList:
                streamWriter.WriteLine($"      var {mi.ChildClassInfo!.LowerPluralName} = new {mi.ChildTypeName}[{LowerClassName}.{mi.ChildClassInfo!.PluralName}.Count];");
                streamWriter.WriteLine($"      {LowerClassName}.{mi.ChildClassInfo!.LowerPluralName}.Values.CopyTo({mi.ChildClassInfo!.LowerPluralName}, 0);");
                streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {mi.ChildClassInfo!.LowerPluralName}) {{");
                break;
              default: throw new NotSupportedException(); 
              }
              foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
                if (childMI.MemberType==MemberTypeEnum.LinkToParent && (childMI.ParentTypeString!)==ClassName) {
                  if (childMI.IsNullable) {
                    streamWriter.WriteLine($"        {mi.LowerChildTypeName}.Remove{childMI.MemberName}({LowerClassName});");
                  } else {
                    streamWriter.WriteLine($"         if ({mi.LowerChildTypeName}.Key>=0) {{");
                    streamWriter.WriteLine($"           {mi.LowerChildTypeName}.Remove();");
                    streamWriter.WriteLine("         }");
                  }
                }
              }
              streamWriter.WriteLine("      }");

            } else if (mi.MemberType==MemberTypeEnum.LinkToParent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null && {LowerClassName}.{mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.RemoveFrom{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!={mi.ParentTypeString}.No{mi.ParentTypeString}) {{");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.RemoveFrom{mi.ParentMethodName}({LowerClassName});");
                streamWriter.WriteLine("      }");
              }
            } else {
              //simple data type
              if (mi.NeedsDictionary) {
                if (mi.IsNullable) {
                  streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null) {{");
                  streamWriter.WriteLine($"        {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({LowerClassName}.{mi.MemberName});");
                  streamWriter.WriteLine($"      }}");
                } else {
                  streamWriter.WriteLine($"      {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}.Remove({LowerClassName}.{mi.MemberName});");
                }
              }
            }
          }
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
        }
        #endregion

        #region Internal RemoveParent())
        foreach (var mi in Members.Values) {
          if (mi.MemberType==MemberTypeEnum.LinkToParent && mi.IsNullable) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Removes {mi.LowerParentType} from {mi.MemberName}");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    internal void Remove{mi.MemberName}({mi.ParentTypeString} {mi.LowerParentType}) {{");
            streamWriter.WriteLine($"      if ({mi.LowerParentType}!={mi.MemberName}) throw new Exception();");
            streamWriter.WriteLine($"      {mi.MemberName} = null;");
            streamWriter.WriteLine("      HasChanged?.Invoke(this);");
            streamWriter.WriteLine("    }");
            streamWriter.WriteLine();
            streamWriter.WriteLine();
          }
        }
        #endregion
      } else {
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Removing {ClassName} from {context}.Data.{PluralName} is not supported.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public void Remove() {");
        streamWriter.WriteLine("      throw new NotSupportedException(\"StorageClass attribute AreInstancesDeletable is false.\");");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
      }

      #region ToShortString()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Returns property values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public string ToShortString() {");
      streamWriter.WriteLine("      var returnString =");
      lines.Clear();
      lines.Add("        $\"{Key.ToKeyString()}");
      foreach (var mi in Members.Values) {
        if (mi.MemberType<=MemberTypeEnum.LinkToParent) {//simple data type
          lines.Add($"        $\" {{{mi.MemberName}{mi.ToStringFunc}}}");
        }
      }
      for (int linesIndex = 0; linesIndex < lines.Count; linesIndex++) {
        var line = lines[linesIndex];
        streamWriter.Write(line);
        if (linesIndex+1<lines.Count) {
          streamWriter.WriteLine(",\" +");
        } else {
          streamWriter.WriteLine("\";");
        }
      }
      streamWriter.WriteLine("      onToShortString(ref returnString);");
      streamWriter.WriteLine("      return returnString;");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onToShortString(ref string returnString);");
      #endregion

      #region ToString()
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Returns all property names and values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public override string ToString() {");
      streamWriter.WriteLine("      var returnString =");
      lines.Clear();
      lines.Add("        $\"Key: {Key}");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.ParentOneChild) {
          lines.Add($"        $\" {mi.MemberName}: {{{mi.MemberName}?.ToShortString()}}");
        } else if (mi.MemberType>MemberTypeEnum.ParentOneChild) {
          //list, directory or sortedList
          lines.Add($"        $\" {mi.MemberName}: {{{mi.MemberName}.Count}}");
        } else {
          //simple data type and enum
          lines.Add($"        $\" {mi.MemberName}: {{{mi.MemberName}{mi.ToStringFunc}}}");
        }
      }
      for (int linesIndex = 0; linesIndex < lines.Count; linesIndex++) {
        var line = lines[linesIndex];
        streamWriter.Write(line);
        if (linesIndex+1<lines.Count) {
          streamWriter.WriteLine(",\" +");
        } else {
          streamWriter.WriteLine(";\";");
        }
      }
      streamWriter.WriteLine("      onToString(ref returnString);");
      streamWriter.WriteLine("      return returnString;");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onToString(ref string returnString);");
      #endregion
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine("  }");

      if (IsGenerateReaderWriter) {

        #region Raw Class
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine($"  #region {RawName}");
        streamWriter.WriteLine("  //      " + new string('-', RawName.Length));
        streamWriter.WriteLine();
        streamWriter.WriteLine("  /// <summary>");
        streamWriter.WriteLine($"  /// {RawName} is used instead {ClassName} and {context}.Data to read an instance from a CSV file with ");
        streamWriter.WriteLine($"  /// {ReaderName} or write with {WriterName}.");
        streamWriter.WriteLine("  /// </summary>");
        streamWriter.WriteLine($"  public class {RawName} {{");
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Unique identifier for {RawName}.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public int Key { get; set; }");
        foreach (var mi in Members.Values) {
          mi.WriteProperty(streamWriter, isRaw: true);
        }
        if (AreInstancesUpdatable || AreInstancesDeletable) {
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// How was {RawName} marked in CSV file (read, update, delete) ? If not read from CSV file, RawState is new.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine("    public RawStateEnum RawState { get; set; }");
        }

        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Default Constructor.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public {RawName}() {{");
        streamWriter.WriteLine("    }");

        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Constructor, will replace links to parents with the parents' key.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public {RawName}({ClassName} {LowerClassName}) {{");
        streamWriter.WriteLine($"      Key = {LowerClassName}.Key;");
        foreach (var mi in Members.Values) {
          if (mi.MemberType<MemberTypeEnum.LinkToParent) {
            streamWriter.WriteLine($"      {mi.MemberName} = {LowerClassName}.{mi.MemberName};");
          } else if (mi.MemberType==MemberTypeEnum.LinkToParent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      {mi.MemberName}Key = {LowerClassName}.{mi.MemberName}?.Key;");
            } else {
              streamWriter.WriteLine($"      {mi.MemberName}Key = {LowerClassName}.{mi.MemberName}.Key;");
            }
          }
        }
        streamWriter.WriteLine("    }");

        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Returns all property names and values");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public override string ToString() {");
        streamWriter.WriteLine("      var returnString =");
        lines.Clear();
        lines.Add("        $\"Key: {Key}");
        foreach (var mi in Members.Values) {
          if (mi.MemberType==MemberTypeEnum.LinkToParent) {
            lines.Add($"        $\" {mi.MemberName}Key: {{{mi.MemberName}Key}}");
          } else if (mi.MemberType<MemberTypeEnum.LinkToParent) {
            //simple data type and enum
            lines.Add($"        $\" {mi.MemberName}: {{{mi.MemberName}{mi.ToStringFunc}}}");
          }
        }
        for (int linesIndex = 0; linesIndex < lines.Count; linesIndex++) {
          var line = lines[linesIndex];
          streamWriter.Write(line);
          if (linesIndex+1<lines.Count) {
            streamWriter.WriteLine(",\" +");
          } else {
            streamWriter.WriteLine(";\";");
          }
        }
        streamWriter.WriteLine("      return returnString;");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("  }");
        streamWriter.WriteLine("  #endregion");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion

        #region Reader Class
        streamWriter.WriteLine($"  #region {ReaderName}");
        streamWriter.WriteLine("  //      " + new string('-', ReaderName.Length));
        streamWriter.WriteLine();
        streamWriter.WriteLine("  /// <summary>");
        streamWriter.WriteLine($"  /// Reads from a CSV file containing {ClassName} instances. Note that the keys of linked objects will be returned ");
        streamWriter.WriteLine("  /// and not the linked objects themselves, since the data context will not be involved.");
        streamWriter.WriteLine("  /// </summary>");
        streamWriter.WriteLine($"  public class {ReaderName}: IDisposable {{");
        streamWriter.WriteLine();
        streamWriter.WriteLine("    readonly CsvConfig csvConfig;");
        streamWriter.WriteLine("    readonly CsvReader csvReader;");
        if (!AreInstancesDeletable && !AreInstancesUpdatable) {
          streamWriter.WriteLine("    int nextKey = 0;");
        }
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Constructor, will read and verify the {ClassName} header line. You need to dispose {ReaderName} once");
        streamWriter.WriteLine($"    /// you are done, except when you have read the whole file, then {ReaderName}.ReadLine() disposes automatically.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public {ReaderName}(string fileNamePath, CsvConfig csvConfig) {{");
        streamWriter.WriteLine("      this.csvConfig = csvConfig;");
        streamWriter.WriteLine($"      csvReader = new CsvReader(fileNamePath, csvConfig, {ClassName}.MaxLineLength);");
        streamWriter.WriteLine($"      var csvHeaderString = Csv.ToCsvHeaderString({ClassName}.Headers, csvConfig.Delimiter);");
        streamWriter.WriteLine("      var headerLine = csvReader.ReadLine();");
        streamWriter.WriteLine("      if (csvHeaderString!=headerLine) throw new Exception($\"Error reading file {csvReader.FileName}{Environment.NewLine}'\" +");
        streamWriter.WriteLine("        headerLine + \"' should be \" + csvHeaderString + \".\");");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Reads the details of one {ClassName} from the CSV file. Returns false when all lines are");
        streamWriter.WriteLine($"    /// read and disposes the reader.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public bool ReadLine([NotNullWhen(true)] out {RawName}? {LowerRawName}){{");
        streamWriter.WriteLine("      if (csvReader.IsEndOfFileReached()) {");
        streamWriter.WriteLine("        csvReader.Dispose();");
        streamWriter.WriteLine($"        {LowerRawName} = null;");
        streamWriter.WriteLine("        return false;");
        streamWriter.WriteLine("      }");
        streamWriter.WriteLine($"      {LowerRawName} = new {RawName}();");
        if (AreInstancesDeletable || AreInstancesUpdatable) {
          streamWriter.WriteLine($"      var firstLineChar = csvReader.ReadFirstLineChar();");
          streamWriter.WriteLine($"      if (firstLineChar==csvConfig.LineCharAdd) {{");
          streamWriter.WriteLine($"        {LowerRawName}.RawState = RawStateEnum.Read;");
          streamWriter.WriteLine($"      }} else if (firstLineChar==csvConfig.LineCharUpdate) {{");
          streamWriter.WriteLine($"        {LowerRawName}.RawState = RawStateEnum.Updated;");
          streamWriter.WriteLine($"      }} else if (firstLineChar==csvConfig.LineCharDelete) {{");
          streamWriter.WriteLine($"        {LowerRawName}.RawState = RawStateEnum.Deleted;");
          streamWriter.WriteLine($"      }} else {{");
          streamWriter.WriteLine($"        throw new NotSupportedException($\"Illegal first line character '{{firstLineChar}}' found in '{{csvReader.GetPresentContent()}}'.\");");
          streamWriter.WriteLine($"      }}");
          streamWriter.WriteLine($"      {LowerRawName}.Key = csvReader.ReadInt();");
        } else {
          streamWriter.WriteLine($"      {LowerRawName}.Key = nextKey++;");
        }
        foreach (var mi in Members.Values) {
          if (mi.MemberType==MemberTypeEnum.LinkToParent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      {LowerRawName}.{mi.MemberName}Key = csvReader.ReadIntNull();");
            } else {
              streamWriter.WriteLine($"      {LowerRawName}.{mi.MemberName}Key = csvReader.ReadInt();");
            }
          } else if (mi.MemberType==MemberTypeEnum.Enum) {
            streamWriter.WriteLine($"      {LowerRawName}.{mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
          } else if (mi.MemberType<=MemberTypeEnum.LinkToParent) {//simple data type or LinkToParent
            streamWriter.WriteLine($"      {LowerRawName}.{mi.MemberName} = csvReader.{mi.CsvReaderRead};");
          }
        }
        streamWriter.WriteLine("      csvReader.ReadEndOfLine();");
        streamWriter.WriteLine($"      return true;");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    #region IDisposable Support");
        streamWriter.WriteLine("    //      -------------------");
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Executes disposal of {ReaderName} exactly once.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public void Dispose() {");
        streamWriter.WriteLine("      Dispose(true);");
        streamWriter.WriteLine();
        streamWriter.WriteLine("      GC.SuppressFinalize(this);");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Is {ReaderName} already exposed ?");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    protected bool IsDisposed {");
        streamWriter.WriteLine("      get { return isDisposed==1; }");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    int isDisposed = 0;");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Inheritors should call Dispose(false) from their destructor");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    protected void Dispose(bool disposing) {");
        streamWriter.WriteLine("      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously");
        streamWriter.WriteLine("      if (wasDisposed==1) return; // already disposed");
        streamWriter.WriteLine();
        streamWriter.WriteLine("      csvReader.Dispose();");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("    #endregion");
        streamWriter.WriteLine("  }");
        streamWriter.WriteLine("  #endregion");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion

        #region Writer Class
        streamWriter.WriteLine($"  #region {WriterName}");
        streamWriter.WriteLine("  //      " + new string('-', WriterName.Length));
        streamWriter.WriteLine();
        streamWriter.WriteLine("  /// <summary>");
        streamWriter.WriteLine($"  /// Writes a CSV file containing records which can be read back as {ClassName}. Note that the keys of linked objects");
        streamWriter.WriteLine("  /// need to be provided in Write(), since the data context will not be involved.");
        streamWriter.WriteLine("  /// </summary>");
        streamWriter.WriteLine($"  public class {WriterName}: IDisposable {{");
        streamWriter.WriteLine();
        streamWriter.WriteLine("    readonly CsvConfig csvConfig;");
        streamWriter.WriteLine("    readonly CsvWriter csvWriter;");
        if (AreInstancesDeletable || AreInstancesUpdatable) {
          streamWriter.WriteLine("    int lastKey = int.MinValue;");
        } else {
          streamWriter.WriteLine("    int nextKey = 0;");
        }
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Constructor, will write the {ClassName} header line into the CSV file. Dispose {WriterName} once done.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public {WriterName}(string fileNamePath, CsvConfig csvConfig){{");
        streamWriter.WriteLine("      this.csvConfig = csvConfig;");
        streamWriter.WriteLine($"      csvWriter = new CsvWriter(fileNamePath, csvConfig, {ClassName}.MaxLineLength, null, 0);");
        streamWriter.WriteLine($"      var csvHeaderString = Csv.ToCsvHeaderString({ClassName}.Headers, csvConfig.Delimiter);");
        streamWriter.WriteLine("      csvWriter.WriteLine(csvHeaderString);");
        streamWriter.WriteLine("    }");


        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Writes the details of one {RawName} to the CSV file");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine($"    public void Write({RawName} {LowerRawName}){{");
        if (AreInstancesDeletable || AreInstancesUpdatable) {
          streamWriter.WriteLine($"      if ({LowerRawName}.Key<0) {{");
          streamWriter.WriteLine($"        throw new Exception($\"{RawName}'s key {{{LowerRawName}.Key}} needs to be greater equal 0.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine($"      if ({LowerRawName}.Key<=lastKey) {{");
          streamWriter.WriteLine($"        throw new Exception($\"{RawName}'s key {{{LowerRawName}.Key}} must be greater than the last written {ClassName}'s key {{lastKey}}.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine($"      lastKey = {LowerRawName}.Key;");
          streamWriter.WriteLine("      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);");
          streamWriter.WriteLine($"      csvWriter.Write({LowerRawName}.Key);");
        } else {
          streamWriter.WriteLine($"      if ({LowerRawName}.Key!=nextKey) {{");
          streamWriter.WriteLine($"        throw new Exception($\"{RawName}'s key {{{LowerRawName}.Key}} should be {{nextKey}}.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine("      nextKey++;");
          streamWriter.WriteLine("      csvWriter.StartNewLine();");
        }
        foreach (var mi in Members.Values) {
          if (mi.MemberType==MemberTypeEnum.LinkToParent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({LowerRawName}.{mi.MemberName}Key is null) {{");
              streamWriter.WriteLine("        csvWriter.WriteNull();");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if ({LowerRawName}.{mi.MemberName}Key<0) throw new Exception($\"Cannot write" +
                $" {LowerClassName} to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"        csvWriter.Write({LowerRawName}.{mi.MemberName}Key.ToString());");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if ({LowerRawName}.{mi.MemberName}Key<0) throw new Exception($\"Cannot write {LowerClassName}" +
                $" to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"      csvWriter.Write({LowerRawName}.{mi.MemberName}Key.ToString());");
            }
          } else if (mi.MemberType==MemberTypeEnum.Enum) {
            streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}((int){LowerRawName}.{mi.MemberName});");
          } else if (mi.MemberType<MemberTypeEnum.Enum) {//simple data
            streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}({LowerRawName}.{mi.MemberName});");
          }
        }
        streamWriter.WriteLine("      csvWriter.WriteEndOfLine();");
        streamWriter.WriteLine("    }");








        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Writes the details of one {ClassName} to the CSV file");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.Write("    public void Write(");
        writeParameters(streamWriter, isConstructor: true, isWriterWrite: true);
        if (AreInstancesDeletable || AreInstancesUpdatable) {
          streamWriter.WriteLine("      if (key<0) {");
          streamWriter.WriteLine($"        throw new Exception($\"{ClassName}'s key {{key}} needs to be greater equal 0.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine("      if (key<=lastKey) {");
          streamWriter.WriteLine($"        throw new Exception($\"{ClassName}'s key {{key}} must be greater than the last written {ClassName}'s key {{lastKey}}.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine("      lastKey = key;");
          streamWriter.WriteLine("      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);");
          streamWriter.WriteLine("      csvWriter.Write(key);");
        } else {
          streamWriter.WriteLine("      if (key!=nextKey) {");
          streamWriter.WriteLine($"        throw new Exception($\"{ClassName}'s key {{key}} should be {{nextKey}}.\");");
          streamWriter.WriteLine("      }");
          streamWriter.WriteLine("      nextKey++;");
          streamWriter.WriteLine("      csvWriter.StartNewLine();");
        }
        foreach (var mi in Members.Values) {
          if (mi.MemberType==MemberTypeEnum.LinkToParent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key is null) {{");
              streamWriter.WriteLine("        csvWriter.WriteNull();");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if ({mi.LowerMemberName}Key<0) throw new Exception($\"Cannot write" +
                $" {LowerClassName} to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"        csvWriter.Write({mi.LowerMemberName}Key.ToString());");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key<0) throw new Exception($\"Cannot write {LowerClassName}" +
                $" to CSV File, because {mi.MemberName} is not stored in {context}.Data.{mi.ParentClassInfo!.PluralName}.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"      csvWriter.Write({mi.LowerMemberName}Key.ToString());");
            }
          } else if (mi.MemberType==MemberTypeEnum.Enum) {
            streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}((int){mi.LowerMemberName});");
          } else if (mi.MemberType<MemberTypeEnum.Enum) {//simple data
            streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}({mi.LowerMemberName});");
          }
        }
        streamWriter.WriteLine("      csvWriter.WriteEndOfLine();");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    #region IDisposable Support");
        streamWriter.WriteLine("    //      -------------------");
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Executes disposal of {WriterName} exactly once.");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    public void Dispose() {");
        streamWriter.WriteLine("      Dispose(true);");
        streamWriter.WriteLine();
        streamWriter.WriteLine("      GC.SuppressFinalize(this);");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Is {WriterName} already exposed ?");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    protected bool IsDisposed {");
        streamWriter.WriteLine("      get { return isDisposed==1; }");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    int isDisposed = 0;");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine("    /// Inheritors should call Dispose(false) from their destructor");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.WriteLine("    protected void Dispose(bool disposing) {");
        streamWriter.WriteLine("      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously");
        streamWriter.WriteLine("      if (wasDisposed==1) return; // already disposed");
        streamWriter.WriteLine();
        streamWriter.WriteLine("      csvWriter.Dispose();");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("    #endregion");
        streamWriter.WriteLine("  }");
        streamWriter.WriteLine("  #endregion");
        #endregion
      }

      streamWriter.WriteLine("}");
      #endregion
    }


    private bool writeParameters(StreamWriter streamWriter, bool isConstructor, bool isWriterWrite = false) {
      //isConstructor = false: Called by Update()
      if (!isConstructor && isWriterWrite) throw new Exception("Not supported combination.");

      var parts = new List<string>();
      if (isWriterWrite) {
        parts.Add("int key");
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType<=MemberTypeEnum.LinkToParent && //simple data type and LinkToParent
          (isConstructor || !mi.IsReadOnly))
        {
          string part;
          if (isWriterWrite && mi.ParentClassInfo!=null) {
            if (mi.IsNullable) {
              part = $"int? {mi.LowerMemberName}Key";
            } else {
              part = $"int {mi.LowerMemberName}Key";
            }
          } else {
            part = $"{mi.TypeString} {mi.LowerMemberName}";
            if (isConstructor && mi.DefaultValue!=null) {
              part += $" = {mi.DefaultValue}";
            }
          }
          parts.Add(part);
        }
      }
      if (isConstructor && !isWriterWrite) {
        parts.Add("bool isStoring = true");
      }
      if (parts.Count==0) return false;//update should only be created if it has parameters.

      writeCode(streamWriter, parts);
      //var isNewLines = parts.Count>4;
      //for (int partsIndex = 0; partsIndex < parts.Count; partsIndex++) {
      //  if (isNewLines) {
      //    streamWriter.WriteLine();
      //    streamWriter.Write("      ");
      //  }
      //  streamWriter.Write(parts[partsIndex]);
      //  if (partsIndex+1!=parts.Count) {
      //    streamWriter.Write(", ");
      //  }
      //}
      //streamWriter.Write(")");
      //if (isNewLines) {
      //  streamWriter.WriteLine();
      //  streamWriter.Write("   ");
      //}
      //streamWriter.WriteLine(" {");
      return true;
    }


    private void writeCode(StreamWriter streamWriter, List<string> parts, bool isStatement = false) {
      var isNewLines = parts.Count>4;
      for (int partsIndex = 0; partsIndex < parts.Count; partsIndex++) {
        if (isNewLines) {
          streamWriter.WriteLine();
          streamWriter.Write("      ");
        }
        streamWriter.Write(parts[partsIndex]);
        if (partsIndex+1!=parts.Count) {
          streamWriter.Write(", ");
        }
      }
      if (isStatement) {
        streamWriter.Write(");");
      } else {
        streamWriter.Write(")");
        if (isNewLines) {
          streamWriter.WriteLine();
          streamWriter.Write("   ");
        }
        streamWriter.WriteLine(" {");
      }
    }


    enum updateTypeEnum {
      Call,
      Definition,
      Implementation
    }


    private bool writeOnUpdateParameters(StreamWriter streamWriter, updateTypeEnum updateType, string? comment = null) {
      var parts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType<=MemberTypeEnum.LinkToParent && //simple data type and LinkToParent
          (!mi.IsReadOnly)) 
        {
          var part = "";
          if (updateType==updateTypeEnum.Definition || updateType==updateTypeEnum.Implementation) {
            part = $"{mi.TypeString} ";
          }
          part += $"{mi.LowerMemberName}";
          parts.Add(part);
        }
      }
      if (parts.Count==0) return false;//update should only be created if it has parameters.

      var lastPart = "ref ";
      if (updateType==updateTypeEnum.Definition || updateType==updateTypeEnum.Implementation) {
        lastPart += "bool ";
      }
      lastPart += "isCancelled";
      parts.Add(lastPart);

      bool isNewLines;
      if (updateType==updateTypeEnum.Call) {
        isNewLines = parts.Count>7;
      } else {
        isNewLines = parts.Count>4;
      }
      for (int partsIndex = 0; partsIndex < parts.Count; partsIndex++) {
        if (isNewLines) {
          streamWriter.WriteLine();
          if (updateType==updateTypeEnum.Call) {
            streamWriter.Write("        ");
          } else {
            streamWriter.Write("      ");
          }
          if (comment!=null) {
            streamWriter.Write(comment);
          }
        }
        streamWriter.Write(parts[partsIndex]);
        if (partsIndex+1!=parts.Count) {
          streamWriter.Write(", ");
        }
      }
      switch (updateType) {
      case updateTypeEnum.Call:
      case updateTypeEnum.Definition:
        streamWriter.WriteLine(");");
        break;
      case updateTypeEnum.Implementation:
        streamWriter.Write(")");
        if (isNewLines) {
          streamWriter.WriteLine();
          streamWriter.Write("   ");
          if (comment!=null) {
            streamWriter.Write(comment);
          }
        }
        streamWriter.WriteLine("{");
        streamWriter.WriteLine($"   {comment}}}");
        break;
      default:
        throw new NotSupportedException();
      }
      return true;
    }


    private bool writeRemoveComment(StreamWriter streamWriter, string context, bool isCapitaliseFirstLetter) {
      var lineParts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType>=MemberTypeEnum.ParentOneChild) {//parent child relationship
          foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
            if (childMI.MemberType==MemberTypeEnum.LinkToParent && (childMI.ParentTypeString!)==ClassName) {
              if (childMI.IsNullable) {
                lineParts.Add($"disconnects {childMI.ClassInfo.ClassName}.{childMI.MemberName} from {mi.ChildClassInfo!.PluralName}");
              } else {
                lineParts.Add($"deletes any {childMI.ClassInfo.ClassName} where {childMI.ClassInfo.ClassName}.{childMI.MemberName} links to this {ClassName}");
              }
            }
          }
        } else if (mi.MemberType==MemberTypeEnum.LinkToParent) {
          lineParts.Add($"disconnects {ClassName} from {mi.ParentTypeString} because of {mi.MemberName}");
        } else if (mi.NeedsDictionary) {
          lineParts.Add($"removes {ClassName} from {context}.Data.{mi.ClassInfo.PluralName}By{mi.MemberName}");
        }
      }

      if (lineParts.Count>0) {
        //var partsCount = 0;
        if (isCapitaliseFirstLetter) {
          lineParts[0] = lineParts[0][0..1].ToUpperInvariant() + lineParts[0][1..];
        }
        for (int linePartIndex = 0; linePartIndex < lineParts.Count; linePartIndex++) {
          var linePart = lineParts[linePartIndex];
          if (linePartIndex + 1 == lineParts.Count) {
            if (!isCapitaliseFirstLetter || linePartIndex!=0) {
              streamWriter.Write(" and ");
            }
          } else {
            if (!isCapitaliseFirstLetter || linePartIndex!=0) {
              streamWriter.Write(", ");
            }
          }
          //partsCount++;
          //if (partsCount>2) {
          //  partsCount = 0;
          if (linePartIndex>0 || !isCapitaliseFirstLetter) {
            streamWriter.WriteLine();
            streamWriter.Write("    /// ");
          }
          //}
          streamWriter.Write(linePart);
        }
        return true;
      } else {
        return false;
      }

    }
  }
}
