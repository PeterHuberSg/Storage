using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Storage {
  public class ClassInfo {
    public readonly string ClassName;
    public readonly string LowerClassName;
    public readonly string? ClassComment;
    public readonly int? MaxLineLength;
    public readonly string PluralName;
    public readonly string LowerPluralName;
    public readonly bool AreItemsUpdatable;
    public readonly bool AreItemsDeletable;
    public readonly bool IsCompactDuringDispose;
    public readonly Dictionary<string, MemberInfo> Members;
    public readonly HashSet<ClassInfo> Parents;
    public readonly List<ClassInfo> Children;

    public bool IsAddedToParentChildTree;


    public ClassInfo(
      string name, 
      string? classComment, 
      int maxLineLength, 
      string pluralName, 
      bool areItemsUpdatable, 
      bool areItemsDeletable, 
      bool isCompactDuringDispose)
    {
      ClassName = name;
      LowerClassName = name[0..1].ToLowerInvariant() + name[1..];
      ClassComment = classComment;
      MaxLineLength = maxLineLength;
      PluralName = pluralName;
      LowerPluralName = pluralName[0..1].ToLowerInvariant() + pluralName[1..];
      AreItemsUpdatable = areItemsUpdatable;
      AreItemsDeletable = areItemsDeletable;
      IsCompactDuringDispose = isCompactDuringDispose;
      Members = new Dictionary<string, MemberInfo>();
      Parents = new HashSet<ClassInfo>();
      Children = new List<ClassInfo>();
      IsAddedToParentChildTree = false;
    }


    public void AddMember(string name, string memberTypeString, string? propertyComment, string ? defaultValue) {
      var isNullable = memberTypeString[^1]=='?';
      if (isNullable) {
        memberTypeString = memberTypeString[..^1];
      }
      switch (memberTypeString.ToLowerInvariant()) {
      case "date": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Date, this, isNullable, propertyComment, defaultValue)); break;
      case "time": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Time, this, isNullable, propertyComment, defaultValue)); break;
      case "dateminutes": Members.Add(name, new MemberInfo(name, MemberTypeEnum.DateMinutes, this, isNullable, propertyComment, defaultValue)); break;
      case "dateseconds": Members.Add(name, new MemberInfo(name, MemberTypeEnum.DateSeconds, this, isNullable, propertyComment, defaultValue)); break;
      case "datetime": Members.Add(name, new MemberInfo(name, MemberTypeEnum.DateTime, this, isNullable, propertyComment, defaultValue)); break;
      case "decimal": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Decimal, this, isNullable, propertyComment, defaultValue)); break;
      case "decimal2": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Decimal2, this, isNullable, propertyComment, defaultValue)); break;
      case "bool": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Bool, this, isNullable, propertyComment, defaultValue)); break;
      case "int": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Int, this, isNullable, propertyComment, defaultValue)); break;
      case "string": Members.Add(name, new MemberInfo(name, MemberTypeEnum.String, this, isNullable, propertyComment, defaultValue)); break;
      default:
        if (memberTypeString.Contains("<")) {
          if (memberTypeString.StartsWith("List<") && memberTypeString.EndsWith(">")) {
            if (isNullable) throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {memberTypeString} cannot be nullable.");

            Members.Add/*List*/(name, new MemberInfo(name, this, memberTypeString, memberTypeString[5..^1], propertyComment, defaultValue));
            break;
          }
          throw new GeneratorException($"Class '{ClassName}'.Property '{name}': {memberTypeString} not support. Should it be a List<> ?");
        }
        Members.Add/*Parent*/(name, new MemberInfo(name, this, memberTypeString, isNullable, propertyComment, defaultValue));
        break;
      }
    }


    //internal void SetMaxLineLength(int maxLineLength) {
    //  MaxLineLength = maxLineLength;
    //}


    //internal void SetAreItemsUpdatable(bool areItemsUpdatable) {
    //  AreItemsUpdatable = areItemsUpdatable.ToString().ToLowerInvariant();
    //}


    //internal void SetAreItemsDeletable(bool areItemsDeletable) {
    //  AreItemsDeletable  = areItemsDeletable.ToString().ToLowerInvariant();
    //}


    //internal void SetIsCompactDuringDispose(bool isCompactDuringDispose) {
    //  IsCompactDuringDispose = isCompactDuringDispose.ToString().ToLowerInvariant();
    //}


    public override string ToString() {
      return $"Class {ClassName}";
    }


    internal void WriteClassFile(StreamWriter streamWriter, string nameSpace) {
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
      streamWriter.WriteLine("    //partial void onConstruct() {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    #region Methods");
      streamWriter.WriteLine("    //      -------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called before storing gets executed");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    //partial void onStore() {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called after all properties are updated, but before the HasChanged event gets raised");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    //partial void onUpdate() {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Called before any remove activities get executed");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    //partial void onRemove() {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Updates returnString with additional info for a short description.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    //partial void onToShortString(ref string returnString) {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Updates returnString with additional info for a short description.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    //partial void onToString(ref string returnString) {");
      streamWriter.WriteLine("    //}");
      streamWriter.WriteLine("    #endregion");
      streamWriter.WriteLine("  }");
      streamWriter.WriteLine("}");
    }


    public void WriteBaseClassFile(StreamWriter streamWriter, string nameSpace, string context) {
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
      streamWriter.WriteLine("    //------------------------------------------------------------------------------");
      streamWriter.WriteLine("    //     This code was generated by StorageClassGenerator.");
      streamWriter.WriteLine("    //");
      streamWriter.WriteLine("    //     Changes to this file will be lost if the code is regenerated.");
      streamWriter.WriteLine("    //------------------------------------------------------------------------------");
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
      var lines = new List<string> {
        "\"Key\""
      };
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
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
        if (mi.MemberType!=MemberTypeEnum.List) {
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
      streamWriter.WriteLine($"    /// Content of {ClassName} has changed. Gets only raised for changes occuring after loading {context}.Data with previously stored data.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public event Action<{ClassName}>? HasChanged;");
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
      streamWriter.Write($"    public {ClassName}(");
      writeParameters(streamWriter, isConstructor: true);
      streamWriter.WriteLine("      Key = Storage.Storage.NoKey;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        //} else if(mi.MemberType==MemberTypeEnum.Decimal2) {
        //  if (mi.IsNullable) {
        //    streamWriter.WriteLine($"      {mi.MemberName} = {mi.LowerMemberName} is null ? (decimal?)null : Math.Round({mi.LowerMemberName}.Value, 2);");
        //  } else {
        //    streamWriter.WriteLine($"      {mi.MemberName} = Math.Round({mi.LowerMemberName}, 2);");
        //  }
        } else {
          streamWriter.WriteLine($"      {mi.MemberName} = {mi.LowerMemberName}{mi.Rounding};");
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
      if (Parents.Count>0) {
        streamWriter.Write("context");
      } else {
        streamWriter.Write("_");
      }
      streamWriter.WriteLine(") {");
      streamWriter.WriteLine("      Key = key;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          if (mi.ChildCount>1) {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new HashSet<{mi.ChildTypeName}>();");
          } else {
            streamWriter.WriteLine($"      {mi.LowerMemberName} = new List<{mi.ChildTypeName}>();");
          }
        } else if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      var {mi.LowerMemberName}Key = csvReader.ReadIntNull();");
            streamWriter.WriteLine($"      if ({mi.LowerMemberName}Key.HasValue) {{");
            streamWriter.WriteLine($"        if (context.{mi.ParentClassInfo!.PluralName}.TryGetValue({mi.LowerMemberName}Key.Value, " + 
              $"out var {mi.LowerMemberName})) {{");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.LowerMemberName};");
            streamWriter.WriteLine($"          {mi.MemberName}.AddTo{PluralName}(this);");
            streamWriter.WriteLine("        } else {");
            streamWriter.WriteLine($"          {mi.MemberName} = {mi.ParentType}.No{mi.ParentType};");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if (context.{mi.ParentClassInfo!.PluralName}.TryGetValue(csvReader.ReadInt(), out var {mi.LowerMemberName})) {{");
            streamWriter.WriteLine($"        {mi.MemberName} = {mi.LowerMemberName};");
            streamWriter.WriteLine($"        {mi.MemberName}.AddTo{PluralName}(this);");
            streamWriter.WriteLine("      } else {");
            streamWriter.WriteLine($"        {mi.MemberName} = {mi.ParentType}.No{mi.ParentType};");
            streamWriter.WriteLine("      }");
          }
        } else if (mi.MemberType==MemberTypeEnum.Enum) {
          streamWriter.WriteLine($"      {mi.MemberName} = ({mi.EnumInfo!.Name})csvReader.{mi.CsvReaderRead};");
        } else {
          streamWriter.WriteLine($"      {mi.MemberName} = csvReader.{mi.CsvReaderRead};");
        }
      }
      streamWriter.WriteLine("    }");
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
      foreach (var parent in Parents) {
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
      streamWriter.WriteLine($"        throw new Exception($\"{ClassName} '{this}' can not be stored in {context}.Data, " +
        $"key is {{Key}} greater equal 0.\");");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      onStore();");
      streamWriter.WriteLine($"      {context}.Data.{PluralName}.Add(this);");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
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
      streamWriter.WriteLine($"    internal const int MaxLineLength = {MaxLineLength};");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Write()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Write {ClassName} to CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static void Write({ClassName} {LowerClassName}, CsvWriter csvWriter) {{");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({LowerClassName}.{mi.MemberName} is null) {{");
            streamWriter.WriteLine("        csvWriter.Write(\"\");");
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
        } else if (mi.MemberType!=MemberTypeEnum.List) {
          streamWriter.WriteLine($"      csvWriter.{mi.CsvWriterWrite}({LowerClassName}.{mi.MemberName});");
        }
      }
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Public Update()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Updates {ClassName} with the provided values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write("    public void Update(");
      writeParameters(streamWriter, isConstructor: false);
      streamWriter.WriteLine("      var isChangeDetected = false;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
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
      if (Parents.Count>0) {
        streamWriter.Write("context");
      } else {
        streamWriter.Write("_");
      }
      streamWriter.WriteLine(") {");
      foreach (var mi in Members.Values) {
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
        } else if (mi.MemberType!=MemberTypeEnum.List) {
          streamWriter.WriteLine($"      {LowerClassName}.{mi.MemberName} = csvReader.{mi.CsvReaderRead};");
        }
      }
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region AddTo() and RemoveFrom()
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Add {mi.LowerChildTypeName} to {mi.ChildClassInfo!.PluralName}.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void AddTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
          streamWriter.WriteLine($"      {mi.ChildClassInfo!.LowerPluralName}.Add({mi.LowerChildTypeName});");
          streamWriter.WriteLine($"      OnAddedTo{mi.ChildClassInfo!.PluralName}({mi.LowerChildTypeName});");
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine($"    partial void OnAddedTo{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Removes {mi.LowerChildTypeName} from {mi.ChildClassInfo!.PluralName}.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void RemoveFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");

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
            streamWriter.WriteLine($"        {mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName}));");
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
            streamWriter.WriteLine($"        {mi.ChildClassInfo!.LowerPluralName}.Remove({mi.LowerChildTypeName}));");
            streamWriter.WriteLine("#endif");
          }
          streamWriter.WriteLine($"      OnRemovedFrom{mi.ChildClassInfo!.PluralName}({mi.LowerChildTypeName});");
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine($"    partial void OnRemovedFrom{mi.ChildClassInfo!.PluralName}({mi.ChildTypeName} {mi.LowerChildTypeName});");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
        }
      }
      #endregion

      #region Remove()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.Write($"    /// Removes {ClassName} from {context}.Data.{PluralName}");
      writeRemoveComent(streamWriter, isCapitaliseFirstLetter: false);
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
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.Write($"    /// ");
      writeRemoveComent(streamWriter, isCapitaliseFirstLetter: true);
      streamWriter.WriteLine(".");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static void Disconnect({ClassName} {LowerClassName}) {{");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {LowerClassName}.{mi.ChildClassInfo!.PluralName}) {{");

          //var childClassMember = mi.ChildClassInfo!.Members[Name];
          //if (childClassMember.IsNullable) {
          //  streamWriter.WriteLine($"        {mi.LowerChildTypeName}.Remove{Name}(" + LowerName + ");");
          //} else {
          //  streamWriter.WriteLine("         if (sampleDetail.Key>=0) {");
          //  streamWriter.WriteLine("           sampleDetail.Remove();");
          //  streamWriter.WriteLine("         }");
          //}

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

      #region ToShortString()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine("    /// Returns property values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public string ToShortString() {");
      streamWriter.WriteLine("      var returnString =");
      lines.Clear();
      lines.Add("        $\"{Key.ToKeyString()}");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
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
        if (mi.MemberType==MemberTypeEnum.List) {
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
      streamWriter.WriteLine("}");
      #endregion
    }


    private void writeParameters(StreamWriter streamWriter, bool isConstructor) {
      var parts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          var part = $"{mi.TypeString} {mi.LowerMemberName}";
          if (isConstructor && mi.DefaultValue!=null) {
            part += $" = {mi.DefaultValue}";
          }
          parts.Add(part);
        }
      }
      if (isConstructor) {
        parts.Add("bool isStoring = true");
      }
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
        if (isNewLines) {
        }
      }
      streamWriter.Write(")");
      if (isNewLines) {
        streamWriter.WriteLine();
        streamWriter.Write("   ");
      }
      streamWriter.WriteLine(" {");
    }


    private void writeRemoveComent(StreamWriter streamWriter, bool isCapitaliseFirstLetter) {
      var lineParts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
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
      } else {
        System.Diagnostics.Debugger.Break();
      }

    }
  }
}
