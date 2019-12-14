using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Storage {
  public class ClassInfo {
    public readonly string Name;
    public readonly string LowerName;
    public string? ClassComment;
    public int? MaxLineLength;
    public readonly Dictionary<string, MemberInfo> Members;
    public readonly List<ClassInfo> Parents;
    public readonly List<ClassInfo> Children;

    public bool IsAddedToParentChildTree;
    public string AreItemsUpdatable = "true";
    public string AreItemsDeletable = "true";
    public string IsCompactDuringDispose = "true";


    public ClassInfo(string name, string? classComment) {
      Name = name;
      LowerName = name[0..1].ToLowerInvariant() + name[1..];
      ClassComment = classComment;
      Members = new Dictionary<string, MemberInfo>();
      Parents = new List<ClassInfo>();
      Children = new List<ClassInfo>();
      IsAddedToParentChildTree = false;
    }


    public void AddMember(string name, string memberTypeString, string? propertyComment) {
      var isNullable = memberTypeString[^1]=='?';
      if (isNullable) {
        memberTypeString = memberTypeString[..^1];
      }
      switch (memberTypeString.ToLowerInvariant()) {
      case "date": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Date, isNullable, propertyComment)); break;
      case "time": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Time, isNullable, propertyComment)); break;
      case "datetime": Members.Add(name, new MemberInfo(name, MemberTypeEnum.DateTime, isNullable, propertyComment)); break;
      case "decimal": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Decimal, isNullable, propertyComment)); break;
      case "decimal2": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Decimal2, isNullable, propertyComment)); break;
      case "int": Members.Add(name, new MemberInfo(name, MemberTypeEnum.Int, isNullable, propertyComment)); break;
      case "string": Members.Add(name, new MemberInfo(name, MemberTypeEnum.String, isNullable, propertyComment)); break;
      default:
        if (memberTypeString.Contains("<")) {
          if (memberTypeString.StartsWith("List<") && memberTypeString.EndsWith(">")) {
            if (isNullable) throw new GeneratorException($"Class '{Name}'.Property '{name}': {memberTypeString} cannot be nullable.");

            Members.Add(name, new MemberInfo(name, memberTypeString, memberTypeString[5..^1], propertyComment));
            break;
          }
          throw new GeneratorException($"Class '{Name}'.Property '{name}': {memberTypeString} not support. Should it be a List<> ?");
        }
        Members.Add(name, new MemberInfo(name, MemberTypeEnum.Parent, isNullable, propertyComment));
        break;
      }
    }


    internal void SetMaxLineLength(int maxLineLength) {
      MaxLineLength = maxLineLength;
    }


    internal void SetAreItemsUpdatable(bool areItemsUpdatable) {
      AreItemsUpdatable = areItemsUpdatable.ToString().ToLowerInvariant();
    }


    internal void SetAreItemsDeletable(bool areItemsDeletable) {
      AreItemsDeletable  = areItemsDeletable.ToString().ToLowerInvariant();
    }


    internal void SetIsCompactDuringDispose(bool isCompactDuringDispose) {
      IsCompactDuringDispose = isCompactDuringDispose.ToString().ToLowerInvariant();
    }


    public override string ToString() {
      return $"Class {Name}";
    }


    internal void WriteCodeFile(StreamWriter streamWriter, string nameSpace) {
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Collections.Generic;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("namespace " + nameSpace + " {");
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
      streamWriter.WriteLine("  public partial class " + Name + ": IStorage<" + Name + "> {");
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
      streamWriter.WriteLine("    //partial void onCreate() {");
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


    public void WriteBaseCodeFile(StreamWriter streamWriter, string nameSpace, string context) {
      streamWriter.WriteLine("using System;");
      streamWriter.WriteLine("using System.Collections.Generic;");
      streamWriter.WriteLine("using Storage;");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("namespace " + nameSpace + " {");
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
      streamWriter.WriteLine("  public partial class " + Name + ": IStorage<" + Name + "> {");
      streamWriter.WriteLine();

      #region Properties
      //      ----------

      streamWriter.WriteLine("    #region Properties");
      streamWriter.WriteLine("    //      ----------");
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Unique identifier for {Name}. Gets set once {Name} gets added to {context}.Data.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public int Key { get; private set; }");
      streamWriter.WriteLine($"    internal static void SetKey({Name} {LowerName}, int key) {{ " + LowerName + ".Key = key; }");

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
          lines.Add($"\"{mi.Name}\"");
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
      streamWriter.WriteLine("    /// None existing " + Name);
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write($"    internal static {Name} No{Name} = new {Name}(");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          streamWriter.Write(mi.NoValue + ", ");
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
      streamWriter.WriteLine($"    /// Content of {Name} has changed. Gets only raised for changes occuring after loading {context}.Data with previously stored data.");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    public event Action<{Name}>? HasChanged;");
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
      streamWriter.Write($"    /// {Name} Constructor. If isStoring is true, adds {Name} to {context}.Data.{Name}s");
      lines.Clear();
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            lines.Add($"if there is a {mi.LowerParentName} adds {Name} to {mi.LowerParentName}.{Name}s");
          } else {
            lines.Add($"adds {Name} to {mi.LowerParentName}.{Name}s");
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
      streamWriter.Write($"    public {Name}(");
      writeParameters(streamWriter, isWriteIsStoring: true);
      streamWriter.WriteLine("      Key = Storage.Storage.NoKey;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          streamWriter.WriteLine($"      {mi.LowerName} = new List<{mi.ChildTypeName}>();");
        } else {
          streamWriter.WriteLine($"      {mi.Name} = {mi.LowerName};");
        }
      }
      streamWriter.WriteLine("      onCreate();");
      streamWriter.WriteLine();
      streamWriter.WriteLine("      if (isStoring) {");
      streamWriter.WriteLine("        Store();");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onCreate();");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region private constructor
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Constructor for {Name} read from CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write($"    private {Name}(int key, CsvReader csvReader, {context} ");
      if (Parents.Count>0) {
        streamWriter.Write("context");
      } else {
        streamWriter.Write("_");
      }
      streamWriter.WriteLine(") {");
      streamWriter.WriteLine("      Key = key;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          streamWriter.WriteLine($"      {mi.LowerName} = new List<{mi.ChildTypeName}>();");
        } else if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      var {mi.LowerParentName}Key = csvReader.ReadIntNull();");
            streamWriter.WriteLine($"      if ({mi.LowerParentName}Key.HasValue) {{");
            streamWriter.WriteLine($"        if (context.{mi.ParentName}s.TryGetValue({mi.LowerParentName}Key.Value, " + 
              $"out var {mi.LowerParentName})) {{");
            streamWriter.WriteLine($"          {mi.ParentName} = {mi.LowerParentName};");
            streamWriter.WriteLine($"          {mi.ParentName}.AddTo{Name}s(this);");
            streamWriter.WriteLine("        } else {");
            streamWriter.WriteLine($"          {mi.ParentName} = {mi.ParentName}.No{mi.ParentName};");
            streamWriter.WriteLine("        }");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if (context.{mi.ParentName}s.TryGetValue(csvReader.ReadInt(), out var {mi.LowerParentName})) {{");
            streamWriter.WriteLine($"        {mi.ParentName} = {mi.LowerParentName};");
            streamWriter.WriteLine($"        {mi.ParentName}.AddTo{Name}s(this);");
            streamWriter.WriteLine("      } else {");
            streamWriter.WriteLine($"        {mi.ParentName} = {mi.ParentName}.No{mi.ParentName};");
            streamWriter.WriteLine("      }");
          }
        } else {
          streamWriter.WriteLine($"      {mi.Name} = csvReader.{mi.CsvReaderRead};");
        }
      }
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Create()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// New {Name} read from CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static {Name} Create(int key, CsvReader csvReader, {context} context) {{");
      streamWriter.WriteLine($"      return new {Name}(key, csvReader, context);");
      streamWriter.WriteLine("    }");
      #endregion

      #region Verify()
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Verify that {LowerName}.{mi.ParentName} exists");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal static bool Verify({Name} {LowerName}) {{");
          streamWriter.WriteLine($"      return {LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName};");
          streamWriter.WriteLine("    }");
        }
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
      streamWriter.Write($"    /// Adds {Name} to {context}.Data.{Name}s");
      var parts = new List<string>();
      foreach (var parent in Parents) {
        parts.Add($"{parent.Name}.{Name}s");
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
      streamWriter.WriteLine($"        throw new Exception($\"{Name} '{this}' can not be stored in {context}.Data, " +
        $"key is {{Key}} greater equal 0.\");");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      onStore();");
      streamWriter.WriteLine($"      {context}.Data!.{Name}s.Add(this);");
      foreach (var parent in Parents) {
        if (Members[parent.Name].IsNullable) {
          streamWriter.WriteLine($"      if ({parent.Name}!=null) {{");
          streamWriter.WriteLine($"        {parent.Name}.AddTo{Name}s(this);");
          streamWriter.WriteLine("      }");
        } else {
          streamWriter.WriteLine($"      {parent.Name}.AddTo{Name}s(this);");
        }
      }

      streamWriter.WriteLine("    }");
      streamWriter.WriteLine("    partial void onStore();");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Maximal number of UTF8 characters needed to write " + Name + " to CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal const int MaxLineLength = " + MaxLineLength + ";");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Write()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Write {Name} to CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine($"    internal static void Write({Name} {LowerName}, CsvWriter csvWriter) {{");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          if (mi.MemberType==MemberTypeEnum.Parent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName} is null) {{");
              streamWriter.WriteLine("        csvWriter.Write(\"\");");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if ({LowerName}.{mi.ParentName}.Key<0) throw new Exception($\"Cannot write" + 
                $" {LowerName} '{{{LowerName}}}' to CSV File, because {mi.ParentName} is not stored in {context}.Data.{mi.ParentName}s.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"        csvWriter.Write({LowerName}.{mi.ParentName}.Key.ToString());");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName}.Key<0) throw new Exception($\"Cannot write {LowerName} '{{{LowerName}}}'" + 
                $" to CSV File, because {mi.ParentName} is not stored in {context}.Data.{mi.ParentName}s.\");");
              streamWriter.WriteLine();
              streamWriter.WriteLine($"      csvWriter.Write(" + LowerName + "." + mi.ParentName + ".Key.ToString());");
            }
          } else {
            streamWriter.WriteLine($"      csvWriter." + mi.CsvWriterWrite + "(" + LowerName + "." + mi.Name + ");");
          }
        }
      }
      streamWriter.WriteLine("    }");
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      #endregion

      #region Public Update()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.WriteLine($"    /// Updates {Name} with the provided values");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write("    public void Update(");
      writeParameters(streamWriter, isWriteIsStoring: false);
      //var isFirst = true;
      //foreach (var mi in Members.Values) {
      //  if (mi.MemberType!=MemberTypeEnum.List) {
      //    if (isFirst) {
      //      isFirst = false;
      //    } else {
      //      streamWriter.Write(", ");
      //    }
      //    streamWriter.Write($"{mi.TypeString} {mi.LowerName}");
      //  }
      //}
      //streamWriter.WriteLine(") {");
      streamWriter.WriteLine("      var isChangeDetected = false;");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          if (mi.MemberType==MemberTypeEnum.Parent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      if ({mi.Name} is null) {{");
              streamWriter.WriteLine($"        if ({mi.LowerName} is null) {{");
              streamWriter.WriteLine("          //nothing to do");
              streamWriter.WriteLine("        } else {");
              streamWriter.WriteLine($"          {mi.Name} = {mi.LowerName};");
              streamWriter.WriteLine($"          {mi.Name}.AddTo{Name}s(this);");
              streamWriter.WriteLine("          isChangeDetected = true;");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if ({mi.LowerName} is null) {{");
              streamWriter.WriteLine($"          {mi.Name}.RemoveFrom{Name}s(this);");
              streamWriter.WriteLine($"          {mi.Name} = null;");
              streamWriter.WriteLine("          isChangeDetected = true;");
              streamWriter.WriteLine("        } else {");
              streamWriter.WriteLine($"          if ({mi.Name}!={mi.LowerName}) {{");
              streamWriter.WriteLine($"            {mi.Name}.RemoveFrom{Name}s(this);");
              streamWriter.WriteLine($"            {mi.Name} = {mi.LowerName};");
              streamWriter.WriteLine($"            {mi.Name}.AddTo{Name}s(this);");
              streamWriter.WriteLine("            isChangeDetected = true;");
              streamWriter.WriteLine("          }");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if ({mi.Name}!={mi.LowerName}) {{");
              streamWriter.WriteLine($"        {mi.Name}.RemoveFrom{Name}s(this);");
              streamWriter.WriteLine($"        {mi.Name} = {mi.LowerName};");
              streamWriter.WriteLine($"        {mi.Name}.AddTo{Name}s(this);");
              streamWriter.WriteLine("        isChangeDetected = true;");
              streamWriter.WriteLine("      }");
            }
          } else {
            streamWriter.WriteLine($"      if ({mi.Name}!={mi.LowerName}) {{");
            streamWriter.WriteLine($"        {mi.Name} = {mi.LowerName};");
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
      streamWriter.WriteLine($"    /// Updates this {Name} with values from CSV file");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.Write($"    internal static void Update({Name} {LowerName}, CsvReader csvReader, {context} ");
      if (Parents.Count>0) {
        streamWriter.Write("context");
      } else {
        streamWriter.Write("_");
      }
      streamWriter.WriteLine(") {");
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          if (mi.MemberType==MemberTypeEnum.Parent) {
            if (mi.IsNullable) {
              streamWriter.WriteLine($"      var {mi.LowerParentName}Key = csvReader.ReadIntNull();");
              streamWriter.WriteLine($"      {mi.ParentName}? {mi.LowerParentName};");
              streamWriter.WriteLine($"      if ({mi.LowerParentName}Key is null) {{");
              streamWriter.WriteLine($"        {mi.LowerParentName} = null;");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if (!context.{mi.ParentName}s.TryGetValue({mi.LowerParentName}Key.Value, out {mi.LowerParentName})) {{");
              streamWriter.WriteLine($"          {mi.LowerParentName} = {mi.ParentName}.No{mi.ParentName};");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine("      }");
              streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName} is null) {{");
              streamWriter.WriteLine($"        if ({mi.LowerParentName} is null) {{");
              streamWriter.WriteLine("          //nothing to do");
              streamWriter.WriteLine("        } else {");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName} = {mi.LowerParentName};");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName}.AddTo{Name}s({LowerName});");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine("      } else {");
              streamWriter.WriteLine($"        if ({mi.LowerParentName} is null) {{");
              streamWriter.WriteLine($"          if ({LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName}) {{");
              streamWriter.WriteLine($"            {LowerName}.{mi.ParentName}.RemoveFrom{Name}s({LowerName});");
              streamWriter.WriteLine("          }");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName} = null;");
              streamWriter.WriteLine("        } else {");
              streamWriter.WriteLine($"          if ({LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName}) {{");
              streamWriter.WriteLine($"            {LowerName}.{mi.ParentName}.RemoveFrom{Name}s({LowerName});");
              streamWriter.WriteLine("          }");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName} = {mi.LowerParentName};");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName}.AddTo{Name}s({LowerName});");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine("      }");
            } else {
              streamWriter.WriteLine($"      if (!context.{mi.ParentName}s.TryGetValue(csvReader.ReadInt(), out var {mi.LowerParentName})) {{");
              streamWriter.WriteLine($"        {mi.LowerParentName} = {mi.ParentName}.No{mi.ParentName};");
              streamWriter.WriteLine("      }");
              streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName}!={mi.LowerParentName}) {{");
              streamWriter.WriteLine($"        if ({LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName}) {{");
              streamWriter.WriteLine($"          {LowerName}.{mi.ParentName}.RemoveFrom{Name}s({LowerName});");
              streamWriter.WriteLine("        }");
              streamWriter.WriteLine($"        {LowerName}.{mi.ParentName} = {mi.LowerParentName};");
              streamWriter.WriteLine($"        {LowerName}.{mi.ParentName}.AddTo{Name}s({LowerName});");
              streamWriter.WriteLine("      }");
            }
          } else {
            streamWriter.WriteLine($"      {LowerName}.{mi.Name} = csvReader.{mi.CsvReaderRead};");
          }
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
          streamWriter.WriteLine($"    /// Add {mi.LowerChildTypeName} to {mi.ChildTypeName}s.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void AddTo{mi.ChildTypeName}s({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
          streamWriter.WriteLine($"      {mi.LowerChildTypeName}s.Add({mi.LowerChildTypeName});");
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          streamWriter.WriteLine("    /// <summary>");
          streamWriter.WriteLine($"    /// Removes {mi.LowerChildTypeName} from {mi.ChildTypeName}s.");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void RemoveFrom{mi.ChildTypeName}s({mi.ChildTypeName} {mi.LowerChildTypeName}) {{");
          streamWriter.WriteLine("#if DEBUG");
          streamWriter.WriteLine($"      if (!{mi.LowerChildTypeName}s.Remove({mi.LowerChildTypeName})) throw new Exception();");
          streamWriter.WriteLine("#else");
          streamWriter.WriteLine($"        {mi.LowerChildTypeName}s.Remove({mi.LowerChildTypeName}));");
          streamWriter.WriteLine("#endif");
          streamWriter.WriteLine("    }");
          streamWriter.WriteLine();
          streamWriter.WriteLine();
        }
      }
      #endregion

      #region Remove()
      streamWriter.WriteLine("    /// <summary>");
      streamWriter.Write($"    /// Removes {Name} from {context}.Data.{Name}s");
      writeRemoveComent(streamWriter, isCapitaliseFirstLetter: false);
      streamWriter.WriteLine(".");
      streamWriter.WriteLine("    /// </summary>");
      streamWriter.WriteLine("    public void Remove() {");
      streamWriter.WriteLine("      if (Key<0) {");
      streamWriter.WriteLine($"        throw new Exception($\"{Name}.Remove(): {Name} '{this}' is not stored in {context}.Data, key is {{Key}}.\");");
      streamWriter.WriteLine("      }");
      streamWriter.WriteLine("      onRemove();");
      streamWriter.WriteLine($"      {context}.Data!.{Name}s.Remove(Key);");
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
      streamWriter.WriteLine($"    internal static void Disconnect({Name} {LowerName}) {{");
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.List) {
          streamWriter.WriteLine($"      foreach (var {mi.LowerChildTypeName} in {LowerName}.{mi.ChildTypeName}s) {{");
          var childClassMember = mi.LinkedClassInfo!.Members[Name];
          if (childClassMember.IsNullable) {
            streamWriter.WriteLine($"        {mi.LowerChildTypeName}.Remove{Name}(" + LowerName + ");");
          } else {
            streamWriter.WriteLine("         if (sampleDetail.Key>=0) {");
            streamWriter.WriteLine("           sampleDetail.Remove();");
            streamWriter.WriteLine("         }");
          }
          streamWriter.WriteLine("      }");
        }
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
          if (mi.IsNullable) {
            streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName}!=null && {LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName}) {{");
            streamWriter.WriteLine($"        {LowerName}.{mi.ParentName}.RemoveFrom{Name}s({LowerName});");
            streamWriter.WriteLine("      }");
          } else {
            streamWriter.WriteLine($"      if ({LowerName}.{mi.ParentName}!={mi.ParentName}.No{mi.ParentName}) {{");
            streamWriter.WriteLine($"        {LowerName}.{mi.ParentName}.RemoveFrom{Name}s({LowerName});");
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
          streamWriter.WriteLine($"    /// Removes {mi.LowerParentName} from {mi.ParentName}");
          streamWriter.WriteLine("    /// </summary>");
          streamWriter.WriteLine($"    internal void Remove{mi.ParentName}({mi.ParentName} {mi.LowerParentName}) {{");
          streamWriter.WriteLine($"      if ({mi.LowerParentName}!={mi.ParentName}) throw new Exception();");
          streamWriter.WriteLine($"      {mi.ParentName} = null;");
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
          lines.Add($"        $\" {{{mi.Name}{mi.ToStringFunc}}}");
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
          lines.Add($"        $\" {mi.Name}: {{{mi.Name}.Count}}");
        } else {
          lines.Add($"        $\" {mi.Name}: {{{mi.Name}{mi.ToStringFunc}}}");
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


    private void writeParameters(StreamWriter streamWriter, bool isWriteIsStoring) {
      var parts = new List<string>();
      foreach (var mi in Members.Values) {
        if (mi.MemberType!=MemberTypeEnum.List) {
          parts.Add($"{mi.TypeString} {mi.LowerName}");
        }
      }
      if (isWriteIsStoring) {
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
          if (!mi.LinkedClassInfo!.Members[Name].IsNullable) {
            lineParts.Add("deletes all " + mi.ChildTypeName + "s");
          } else {
            lineParts.Add("disconnects all " + mi.ChildTypeName + "s");
          }
        }
      }
      foreach (var mi in Members.Values) {
        if (mi.MemberType==MemberTypeEnum.Parent) {
          lineParts.Add("disconnects " + Name + " from " + mi.ParentName + "");
        }
      }
      if (lineParts.Count>0) {
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
          if (lineParts.Count>3) {
            streamWriter.WriteLine();
            streamWriter.Write("        ");
          }
          streamWriter.Write(linePart);
        }
      } else {
        System.Diagnostics.Debugger.Break();
      }

    }
  }
}
