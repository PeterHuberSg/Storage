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
    public readonly string WriterName;
    public readonly string? ClassComment;
    public readonly int? MaxLineLength;
    public readonly string PluralName;
    public readonly string LowerPluralName;
    public readonly bool AreInstancesUpdatable;
    public readonly bool AreInstancesDeletable;
    public readonly bool IsConstructorPrivate;
    public bool IsDisconnectNeeded = true;
    public readonly Dictionary<string, MemberInfo> Members;
    public readonly HashSet<ClassInfo> ParentsAll;
    public readonly HashSet<ClassInfo> ParentsWithList;
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
      bool isConstructorPrivate)
    {
      ClassName = name;
      LowerClassName = name[0..1].ToLowerInvariant() + name[1..];
      WriterName = name + "Writer";
      ClassComment = classComment;
      MaxLineLength = maxLineLength;
      PluralName = pluralName;
      LowerPluralName = pluralName[0..1].ToLowerInvariant() + pluralName[1..];
      AreInstancesUpdatable = areInstancesUpdatable;
      AreInstancesDeletable = areInstancesDeletable;
      IsConstructorPrivate = isConstructorPrivate;
      Members = new Dictionary<string, MemberInfo>();
      ParentsAll = new HashSet<ClassInfo>();
      ParentsWithList = new HashSet<ClassInfo>();
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
      bool isReadOnly) 
    {
      var isLookUp = false;
      var isNullable = csvTypeString[^1]=='?';
      if (isNullable) {
        csvTypeString = csvTypeString[..^1];
      }
      MemberInfo member;
      switch (csvTypeString.ToLowerInvariant()) {
      case "date": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Date, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "time": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Time, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "dateminutes": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.DateMinutes, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "dateseconds": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.DateSeconds, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "datetime": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.DateTime, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "decimal": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Decimal, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "decimal2": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Decimal2, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "decimal4": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Decimal4, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "decimal5": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Decimal5, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "bool": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Bool, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "int": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.Int, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      case "string": member = new MemberInfo(name, csvTypeString, MemberTypeEnum.String, this, isNullable, isReadOnly, propertyComment, defaultValue); break;
      default:
        if (csvTypeString.Contains("<")) {
          //a parent having a collection for its children
          if (csvTypeString.StartsWith("List<") && csvTypeString.EndsWith(">")) {
            if (isNullable) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} cannot be nullable.");

            /*List*/member = new MemberInfo(name, this, csvTypeString, csvTypeString[5..^1], propertyComment, defaultValue);
            break;
          } else if ((csvTypeString.StartsWith("Dictionary<") || csvTypeString.StartsWith("SortedList<")) && 
            csvTypeString.EndsWith(">")) 
          {
            if (isNullable) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} cannot be nullable.");

            //Dictionary or SortedList
            // memberTypeString: SortedList<DateTime /*DateOnly*/, Sample>
            // memberTypeString: Dictionary<DateTime /*DateOnly*/, Sample>
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
            CollectionTypeEnum collectionType;
            if (csvTypeString.StartsWith("SortedList<")) {
              collectionType = CollectionTypeEnum.SortedList;
            } else {
              collectionType = CollectionTypeEnum.Dictionary;
            }
            member = new MemberInfo(
              name, 
              this, 
              csvTypeString,
              collectionType,
              itemTypeName,
              childKeyPropertyName, 
              keyTypeString, 
              propertyComment, 
              defaultValue);
            break;
          }

          throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {csvTypeString} not support. Should " +
            "it be a List<>, SortedList<,> or Dictionary<,> ?");
        }
        //a child linking to its parent, i.e. a MemberTypeEnum.Parent
        member = new MemberInfo(name, this, csvTypeString, isNullable, isReadOnly, propertyComment, defaultValue, isLookupOnly??false);
        isLookUp = true;
        break;
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


    internal void WriteClassFile(StreamWriter streamWriter, string nameSpace, bool isFullyCommented) {
      var cs = isFullyCommented ? "//" : "";
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
      streamWriter.WriteLine($"    {cs}partial void onCsvConstruct(DL context) {{");
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
        streamWriter.WriteLine($"    {cs}partial void onUpdate() {{");
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
        if (mi.CollectionType>CollectionTypeEnum.Undefined) { //List, Dictionary or SortedList
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
        if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
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
        if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
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
        if (mi.MemberType==MemberTypeEnum.Parent) {
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
        if (mi.MemberType==MemberTypeEnum.List) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        } else if (mi.MemberType==MemberTypeEnum.CollectionKeyValue) {
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
        if (mi.MemberType==MemberTypeEnum.List) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        } else if (mi.MemberType==MemberTypeEnum.CollectionKeyValue) {
          streamWriter.WriteLine($"      {mi.LowerMemberName} = new {mi.TypeString}();");
        } else if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      var {mi.LowerMemberName}Key = csvReader.ReadIntNull();");
            streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key.HasValue) {{");
            streamWriter.WriteLine($"        if (context.{mi.ParentClassInfo!.PluralName}.TryGetValue({mi.LowerMemberName}Key.Value, " +
              $"out var {mi.LowerMemberName})) {{");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
            streamWriter.WriteLine("        } else {");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.ParentType}.No{mi.ParentType};");
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
        } else if (mi.MemberType==MemberTypeEnum.Enum) {
          streamWriter.WriteLine($"      {mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
        } else {
          streamWriter.WriteLine($"      {mi.MemberName} = csvReader.{mi.CsvReaderRead};");
        }
      }
      //if the parent is a Dictionary or SortedList, the key property must be assigned first (i.e. in the for loop 
      //above), before the child gets added to the parent
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent && !mi.IsLookupOnly) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key.HasValue && {mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
            streamWriter.WriteLine($"        {mi.MemberName}!.AddTo{PluralName}(this);");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
            streamWriter.WriteLine($"        {mi.MemberName}.AddTo{PluralName}(this);");
            streamWriter.WriteLine("      }");
          }
        }
      }
      streamWriter.WriteLine("      onCsvConstruct(context);");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onCsvConstruct(DL context);");
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
        if (mi.MemberType==MemberTypeEnum.Parent) {
          commentLines.Add($"    /// Verify that {LowerClassName}.{mi.MemberName} exists.");
          codeLines.Add($"      if ({LowerClassName}.{mi.MemberName}=={mi.ParentType}.No{mi.ParentType}) return false;");
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
      foreach (var parent in ParentsWithList) {
        parts.Add($"{parent.ClassName}.{PluralName}");
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
      streamWriter.WriteLine($"        throw new Exception($\"{ClassName} can not be stored again in {context}.Data, " +
        $"key is {{Key}} greater equal 0.\" + Environment.NewLine + ToString());");
      streamWriter.WriteLine("      }");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent && !mi.IsLookupOnly) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({mi.MemberName}!=null && {mi.MemberName}.Key<0) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{ClassName} can not be stored in {context}.Data, " +
              $"{mi.MemberName} is not stored yet.\" + Environment.NewLine + ToString());");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({mi.MemberName}.Key<0) {{");
            streamWriter.WriteLine($"        throw new Exception($\"{ClassName} can not be stored in {context}.Data, " +
              $"{mi.MemberName} is missing or not stored yet.\" + Environment.NewLine + ToString());");
            streamWriter.WriteLine("      }");
          }
        }
      }
      streamWriter.WriteLine("      onStore();");
      streamWriter.WriteLine($"      {context}.Data.{PluralName}.Add(this);");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent && !mi.IsLookupOnly) {
          if (mi.IsNullable) {
            //streamWriter.WriteLine($"      if ({mi.Name}!=null) {{");
            streamWriter.WriteLine($"      {mi.MemberName}?.AddTo{PluralName}(this);");
            //streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      {mi.MemberName}.AddTo{PluralName}(this);");
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
        if (mi.MemberType==MemberTypeEnum.Parent) {
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
        } else if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
          streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}({LowerClassName}.{mi.MemberName});");
        }
      }
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
        streamWriter.WriteLine("      var isChangeDetected = false;");
        foreach (var mi in Members.Values) {
          if (mi.CollectionType==CollectionTypeEnum.Undefined && //not List, Dictionary nor SortedList
            !mi.IsReadOnly)
          {
            if (mi.MemberType==MemberTypeEnum.Parent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      if ({mi.MemberName} is null) {{");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine("          //nothing to do");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {mi.MemberName}.AddTo{PluralName}(this);");
                streamWriter.WriteLine("          isChangeDetected = true;");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine($"          {mi.MemberName}.RemoveFrom{PluralName}(this);");
                streamWriter.WriteLine($"          {mi.MemberName} = null;");
                streamWriter.WriteLine("          isChangeDetected = true;");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          if ({mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"            {mi.MemberName}.RemoveFrom{PluralName}(this);");
                streamWriter.WriteLine($"            {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"            {mi.MemberName}.AddTo{PluralName}(this);");
                streamWriter.WriteLine("            isChangeDetected = true;");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"        {mi.MemberName}.RemoveFrom{PluralName}(this);");
                streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"        {mi.MemberName}.AddTo{PluralName}(this);");
                streamWriter.WriteLine("        isChangeDetected = true;");
                streamWriter.WriteLine("      }");
              }
            } else if (mi.Rounding!=null) {
              streamWriter.WriteLine($"      var {mi.LowerMemberName}Rounded = {mi.LowerMemberName}{mi.Rounding};");
              streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}Rounded) {{");
              streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName}Rounded;");
              streamWriter.WriteLine("        isChangeDetected = true;");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if ({mi.MemberName}!={mi.LowerMemberName}) {{");
              streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName};");
              streamWriter.WriteLine("        isChangeDetected = true;");
              streamWriter.WriteLine("      }");
            }
          }
        }
        streamWriter.WriteLine("      if (isChangeDetected) {");
        streamWriter.WriteLine("        onUpdate();");
        streamWriter.WriteLine("        HasChanged?.Invoke(this);");
        streamWriter.WriteLine("      }");
        streamWriter.WriteLine("    }");
        streamWriter.WriteLine("    partial void onUpdate();");
        streamWriter.WriteLine();
        streamWriter.WriteLine();
        #endregion

        #region Internal Update()
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// Updates this {ClassName} with values from CSV file");
        streamWriter.WriteLine("    /// </summary>");
        streamWriter.Write($"    internal static void Update({ClassName} {LowerClassName}, CsvReader csvReader, {context} ");
        if (ParentsWithList.Count>0) {
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
            if (mi.MemberType==MemberTypeEnum.Parent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      var {mi.LowerMemberName}Key = csvReader.ReadIntNull();");
                streamWriter.WriteLine($"      {mi.ParentType}? {mi.LowerMemberName};");
                streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key is null) {{");
                streamWriter.WriteLine($"        {mi.LowerMemberName} = null;");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if (!context.{mi.ParentClassInfo!.PluralName}.TryGetValue({mi.LowerMemberName}Key.Value, out {mi.LowerMemberName})) {{");
                streamWriter.WriteLine($"          {mi.LowerMemberName} = {mi.ParentType}.No{mi.ParentType};");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName} is null) {{");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine("          //nothing to do");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.AddTo{PluralName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      } else {");
                streamWriter.WriteLine($"        if ({mi.LowerMemberName} is null) {{");
                streamWriter.WriteLine($"          if ({LowerClassName}.{mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
                streamWriter.WriteLine($"            {LowerClassName}.{mi.MemberName}.RemoveFrom{PluralName}({LowerClassName});");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = null;");
                streamWriter.WriteLine("        } else {");
                streamWriter.WriteLine($"          if ({LowerClassName}.{mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
                streamWriter.WriteLine($"            {LowerClassName}.{mi.MemberName}.RemoveFrom{PluralName}({LowerClassName});");
                streamWriter.WriteLine("          }");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.AddTo{PluralName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if (!context.{mi.ParentClassInfo!.PluralName}.TryGetValue(csvReader.ReadInt(), out var {mi.LowerMemberName})) {{");
                streamWriter.WriteLine($"        {mi.LowerMemberName} = {mi.ParentType}.No{mi.ParentType};");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!={mi.LowerMemberName}) {{");
                streamWriter.WriteLine($"        if ({LowerClassName}.{mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
                streamWriter.WriteLine($"          {LowerClassName}.{mi.MemberName}.RemoveFrom{PluralName}({LowerClassName});");
                streamWriter.WriteLine("        }");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName} = {mi.LowerMemberName};");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.AddTo{PluralName}({LowerClassName});");
                streamWriter.WriteLine("      }");
              }
            } else if (mi.MemberType==MemberTypeEnum.Enum) {
              streamWriter.WriteLine($"      {LowerClassName}.{mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
            } else if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
              streamWriter.WriteLine($"      {LowerClassName}.{mi.MemberName} = csvReader.{mi.CsvReaderRead};");
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
        if (mi.CollectionType>CollectionTypeEnum.Undefined) {//List, Dictionary or SortedList
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Add {mi.LowerChildTypeName} to {mi.ChildClassInfo!.PluralName}.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void AddTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
          if (mi.MemberType==MemberTypeEnum.List) {
            streamWriter.WriteLine($"      {mi.ChildClassInfo!.LowerPluralName}.Add({mi.LowerChildTypeName});");
          } else { //Dictionary or SortedList
            streamWriter.WriteLine($"      {mi.ChildClassInfo!.LowerPluralName}.Add({mi.LowerChildTypeName}.{mi.ChildKeyPropertyName}, {mi.LowerChildTypeName});");
          }
          streamWriter.WriteLine($"      onAddedTo{mi.ChildClassInfo!.PluralName}({mi.LowerChildTypeName});");
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine($"    partial void onAddedTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          if (mi.ChildClassInfo.AreInstancesUpdatable || mi.ChildClassInfo.AreInstancesDeletable) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Removes {mi.LowerChildTypeName} from {mi.ChildClassInfo!.PluralName}.");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    internal void RemoveFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");

            if (mi.MemberType==MemberTypeEnum.List) {
              var linksLines = new List<string>();
              foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
                if (childMI.MemberType==MemberTypeEnum.Parent && (childMI.ParentType!)==ClassName) {
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
        IsDisconnectNeeded = writeRemoveComent(streamWriter, isCapitaliseFirstLetter: false);
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
          writeRemoveComent(streamWriter, isCapitaliseFirstLetter: true);
          streamWriter.WriteLine(".");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal static void Disconnect({ClassName} {LowerClassName}) {{");
          foreach (var mi in Members.Values) {
            if (mi.CollectionType>CollectionTypeEnum.Undefined) { 
              switch (mi.CollectionType) {
              case CollectionTypeEnum.List:
                streamWriter.WriteLine($"      for (int {mi.LowerChildTypeName}Index = {LowerClassName}.{mi.ChildClassInfo!.PluralName}.Count-1; {mi.LowerChildTypeName}Index>= 0; {mi.LowerChildTypeName}Index--) {{");
                streamWriter.WriteLine($"        var {mi.LowerChildTypeName} = {LowerClassName}.{mi.ChildClassInfo!.PluralName}[{mi.LowerChildTypeName}Index];");
                break;
              case CollectionTypeEnum.Dictionary:
                streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {LowerClassName}.{mi.ChildClassInfo!.PluralName}.Values) {{");
                break;
              case CollectionTypeEnum.SortedList:
                streamWriter.WriteLine($"      var {mi.ChildClassInfo!.LowerPluralName} = new {mi.ChildTypeName}[{LowerClassName}.{mi.ChildClassInfo!.PluralName}.Count];");
                streamWriter.WriteLine($"      {LowerClassName}.{mi.ChildClassInfo!.LowerPluralName}.Values.CopyTo({mi.ChildClassInfo!.LowerPluralName}, 0);");
                streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {mi.ChildClassInfo!.LowerPluralName}) {{");
                break;
              default: throw new NotSupportedException(); 
              }
              foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
                if (childMI.MemberType==MemberTypeEnum.Parent && (childMI.ParentType!)==ClassName) {
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

            } else if (mi.MemberType==MemberTypeEnum.Parent) {
              if (mi.IsNullable) {
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!=null && {LowerClassName}.{mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.RemoveFrom{PluralName}({LowerClassName});");
                streamWriter.WriteLine("      }");
              } else {
                streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName}!={mi.ParentType}.No{mi.ParentType}) {{");
                streamWriter.WriteLine($"        {LowerClassName}.{mi.MemberName}.RemoveFrom{PluralName}({LowerClassName});");
                streamWriter.WriteLine("      }");
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
          if (mi.MemberType==MemberTypeEnum.Parent && mi.IsNullable) {
            streamWriter.WriteLine("    /// <summary>");
            streamWriter.WriteLine($"    /// Removes {mi.LowerParentType} from {mi.MemberName}");
            streamWriter.WriteLine("    /// </summary>");
            streamWriter.WriteLine($"    internal void Remove{mi.MemberName}({mi.ParentType} {mi.LowerParentType}) {{");
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
        if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
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
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region ToString()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Returns all property names and values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public override string ToString() {");
      streamWriter.WriteLine("      var returnString =");
      lines.Clear();
      lines.Add("        $\"Key: {Key}");
      foreach (var mi in Members.Values) {
        if (mi.CollectionType>CollectionTypeEnum.Undefined) {//List, Dictionary or SortedList
          lines.Add($"        $\" {mi.MemberName}: {{{mi.MemberName}.Count}}");
        } else {
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
      streamWriter.WriteLine();
      streamWriter.WriteLine();

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
      }
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Constructor, will write the {ClassName} header line into the CSV file. Dispose {WriterName} once done.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public {WriterName}(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {{");
      streamWriter.WriteLine("      this.csvConfig = csvConfig;");
      streamWriter.WriteLine("      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);");
      streamWriter.WriteLine($"      var csvHeaderString = Csv.ToCsvHeaderString({ClassName}.Headers, csvConfig.Delimiter);");
      streamWriter.WriteLine("      csvWriter.WriteLine(csvHeaderString);");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Writes the details of one {ClassName} to the CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write("    public void Write(");
      writeParameters(streamWriter, isConstructor: true, isWriter: true);
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
        streamWriter.WriteLine("      csvWriter.StartNewLine();");
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
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
        } else if (mi.CollectionType==CollectionTypeEnum.Undefined) {//not List, Dictionary nor SortedList
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

      streamWriter.WriteLine("}");
      #endregion
    }


    private bool writeParameters(StreamWriter streamWriter, bool isConstructor, bool isWriter = false) {
      var parts = new List<string>();
      if (isWriter) {
        if (AreInstancesDeletable || AreInstancesUpdatable) {
          parts.Add("int key");
        }
      }
      foreach (var mi in Members.Values) {
        if (mi.CollectionType==CollectionTypeEnum.Undefined && //not List, Dictionary nor SortedList
          (isConstructor || !mi.IsReadOnly))
        {
          string part;
          if (isWriter && mi.ParentClassInfo!=null) {
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
      if (isConstructor && !isWriter) {
        parts.Add("bool isStoring = true");
      }
      if (parts.Count==0) return false;//update should only be created if it has parameters.

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
      streamWriter.Write(")");
      if (isNewLines) {
        streamWriter.WriteLine();
        streamWriter.Write("   ");
      }
      streamWriter.WriteLine(" {");
      return true;
    }


    private bool writeRemoveComent(StreamWriter streamWriter, bool isCapitaliseFirstLetter) {
      var lineParts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.CollectionType>CollectionTypeEnum.Undefined) {//List, Dictionary or SortedList
          foreach (var childMI in mi.ChildClassInfo!.Members.Values) {
            if (childMI.MemberType==MemberTypeEnum.Parent && (childMI.ParentType!)==ClassName) {
              if (childMI.IsNullable) {
                lineParts.Add($"disconnects {childMI.ClassInfo.ClassName}.{childMI.MemberName} from {mi.ChildClassInfo!.PluralName}");
              } else {
                lineParts.Add($"deletes all {childMI.ClassInfo.ClassName} where {childMI.ClassInfo.ClassName}.{childMI.MemberName} links to this {ClassName}");
              }
            }
          }
        } else if (mi.MemberType==MemberTypeEnum.Parent) {
          lineParts.Add($"disconnects {ClassName} from {mi.ParentType} because of {mi.MemberName}");
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
