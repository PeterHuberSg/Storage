using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Storage {

  public enum MemberTypeEnum {
    Undefined = 0,
    Date,
    Time,
    DateTime,
    Decimal,
    Decimal2,
    Int,
    List,
    Parent,
    String,
    Lenght
  }


  public class MemberInfo {
    public readonly string Name;
    public readonly string LowerName;
    public readonly MemberTypeEnum MemberType;
    public readonly bool IsNullable;
    public readonly string? Comment;
    public readonly string? PrecissionComment;
    public readonly string? ChildTypeName;
    public readonly string? LowerChildTypeName;
    public readonly string TypeString;
    public readonly string? ParentName;
    public readonly string? LowerParentName;
    public readonly string? CsvReaderRead;
    public readonly string? CsvWriterWrite;
    public readonly string? NoValue; //used to fill NoClass with a obviously bad value
    public readonly string? ToStringFunc;

    public ClassInfo? LinkedClassInfo;


    public MemberInfo(string name, MemberTypeEnum memberType, bool isNullable, string? comment) {
      if (memberType==MemberTypeEnum.List) throw new Exception();

      Name = name;
      LowerName = name[0..1].ToLowerInvariant() + name[1..];
      MemberType = memberType;
      IsNullable = isNullable;
      Comment = comment;
      switch (memberType) {
      case MemberTypeEnum.Date:
        TypeString = "DateTime";
        CsvReaderRead = "ReadDate()";
        CsvWriterWrite = "WriteDate";
        NoValue = "DateTime.MinValue.Date";
        ToStringFunc = ".ToShortDateString()";
        PrecissionComment = "Stores only dates but no times.";
        break;
      case MemberTypeEnum.Time:
        TypeString = "TimeSpan";
        CsvReaderRead = "ReadTime()";
        CsvWriterWrite = "WriteTime";
        NoValue = "TimeSpan.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores less than 24 hours with second precission.";
        break;
      case MemberTypeEnum.DateTime: 
        TypeString = "DateTime";
        CsvReaderRead = "ReadDateTime()";
        CsvWriterWrite = "WriteDateTime";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with tick precission.";
        break;
      case MemberTypeEnum.Decimal: 
        TypeString = "decimal";
        CsvReaderRead = "ReadDecimal()";
        CsvWriterWrite = "Write";
        NoValue = "Decimal.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with maximum precission.";
        break;
      case MemberTypeEnum.Decimal2:
        TypeString = "decimal";
        CsvReaderRead = "ReadDecimal()";
        CsvWriterWrite = "WriteDecimal2";
        NoValue = "Decimal.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores decimal with 2 digits after comma.";
        break;
      case MemberTypeEnum.Int: 
        TypeString = "int";
        CsvReaderRead = "ReadInt()";
        CsvWriterWrite = "Write";
        NoValue = "int.MinValue";
        ToStringFunc = "";
        break;
      case MemberTypeEnum.Parent:
        ParentName = name;
        LowerParentName = name[0..1].ToLowerInvariant() + name[1..];
        TypeString = name;
        CsvWriterWrite = "Write";
        if (isNullable) {
          CsvReaderRead = "ReadIntNull()";
          NoValue = "null";
          ToStringFunc = "?.ToShortString()";
        } else {
          CsvReaderRead = "ReadInt()";
          NoValue = $"{name}.No{name}";
          ToStringFunc = ".ToShortString()";
        }
        break;
      case MemberTypeEnum.String: 
        TypeString = "string";
        CsvReaderRead = "ReadString()!";
        CsvWriterWrite = "Write";
        NoValue = isNullable ? "null" : $"\"No{name}\"";
        ToStringFunc = "";
        break;
      default:
        throw new NotSupportedException();
      }
      if (isNullable) {
        TypeString += '?';
      }
    }


    /// <summary>
    /// constructor for List
    /// </summary>
    public MemberInfo(string name, string listType, string childType, string? comment) {
      MemberType = MemberTypeEnum.List;
      Name = name;
      LowerName = name[0..1].ToLowerInvariant() + name[1..];
      ChildTypeName = childType;
      LowerChildTypeName = childType[0..1].ToLowerInvariant() + childType[1..];
      IsNullable = false;
      TypeString = listType;
      CsvReaderRead = null;
      CsvWriterWrite = null;
      Comment = comment;
    }


    public override string ToString() {
      string isNullableString = IsNullable ? "?" : "";
      if (MemberType==MemberTypeEnum.List) {
        return $"List<{ChildTypeName}> {Name}";
      }else if (MemberType==MemberTypeEnum.List) {
        return $"{Name}{isNullableString} {Name}";
      } else {
        return $"{MemberType}{isNullableString} {Name}";
      }
    }


    internal void WriteProperty(StreamWriter streamWriter) {
      streamWriter.WriteLine();
      streamWriter.WriteLine();
      bool hasWrittenComment = false;
      if (Comment!=null) {
        var linesArray = Comment.Split(Environment.NewLine);
        foreach (var line in linesArray) {
          if (!string.IsNullOrWhiteSpace(line)) {
            if (PrecissionComment!=null && line.Contains("/// </summary>")) {
              hasWrittenComment = true;
              streamWriter.WriteLine($"    /// {PrecissionComment}");
            }
            streamWriter.WriteLine($"    {line}");
          }
        }
      }
      if (PrecissionComment!=null && !hasWrittenComment) {
        streamWriter.WriteLine($"    /// <summary>");
        streamWriter.WriteLine($"    /// {PrecissionComment}");
        streamWriter.WriteLine($"    ///  </summary>");
      }
      if (MemberType==MemberTypeEnum.List) {
        streamWriter.WriteLine("    public IReadOnly" + TypeString + " " + Name + " { get { return " + LowerName + "; } }");
        streamWriter.WriteLine("    readonly List<" + ChildTypeName + "> " + LowerName + ";");
      } else {
        streamWriter.WriteLine("    public " + TypeString + " " + Name + " { get; private set; }");
      }
    }
  }
}
