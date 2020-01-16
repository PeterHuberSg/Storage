﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Storage {

  public enum MemberTypeEnum {
    Undefined = 0,
    Date,
    Time,
    DateMinutes,
    DateSeconds,
    DateTime,
    Decimal,
    Decimal2,
    Bool,
    Int,
    List,
    Parent,
    String,
    Enum,
    Lenght
  }


  public class MemberInfo {
    public readonly string MemberName;
    public readonly string LowerMemberName;
    public MemberTypeEnum MemberType;
    public readonly ClassInfo ClassInfo;
    public readonly string TypeString;
    public readonly bool IsNullable;
    public readonly string? Comment;
    public readonly string? PrecissionComment;
    public readonly string? Rounding;
    public readonly string? DefaultValue;
    public readonly int MaxStorageSize;
    public readonly string? ChildTypeName;
    public readonly string? LowerChildTypeName;
    public readonly string? ParentType;//is only different from TypeString when IsNullable
    public readonly string? LowerParentType;
    public readonly string? CsvReaderRead;
    public readonly string? CsvWriterWrite;
    public readonly string? NoValue; //used to fill NoClass with a obviously bad value
    public string? ToStringFunc;

    public ClassInfo? ChildClassInfo;
    public ClassInfo? ParentClassInfo; //not really used
    public EnumInfo? EnumInfo;
    public int ChildCount = 0;


    public MemberInfo(string name, MemberTypeEnum memberType, ClassInfo classInfo, bool isNullable, string? comment, string? defaultValue) {
      if (memberType==MemberTypeEnum.List) throw new Exception();

      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      MemberType = memberType;
      ClassInfo = classInfo;
      IsNullable = isNullable;
      Comment = comment;
      DefaultValue = defaultValue;
      switch (memberType) {
      case MemberTypeEnum.Date:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234\t".Length;
        CsvReaderRead = "ReadDate()";
        CsvWriterWrite = "WriteDate";
        NoValue = "DateTime.MinValue.Date";
        ToStringFunc = ".ToShortDateString()";
        PrecissionComment = "Stores only dates but no times.";
        Rounding = ".Floor(Rounding.Days)";
        break;

      case MemberTypeEnum.Time:
        TypeString = "TimeSpan";
        MaxStorageSize = "23:59:59\t".Length;
        CsvReaderRead = "ReadTime()";
        CsvWriterWrite = "WriteTime";
        NoValue = "TimeSpan.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores less than 24 hours with second precission.";
        Rounding = ".Round(Rounding.Seconds)";
        break;

      case MemberTypeEnum.DateMinutes:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234 23:59\t".Length;
        CsvReaderRead = "ReadDateSeconds()";//can also be used for minutes
        CsvWriterWrite = "WriteDateMinutes";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with minute precission.";
        Rounding = ".Round(Rounding.Minutes)";
        break;

      case MemberTypeEnum.DateSeconds:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234 23:59:59\t".Length;
        CsvReaderRead = "ReadDateSeconds()";
        CsvWriterWrite = "WriteDateSeconds";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with seconds precission.";
        Rounding = ".Round(Rounding.Seconds)";
        break;

      case MemberTypeEnum.DateTime: 
        TypeString = "DateTime";
        MaxStorageSize = (long.MaxValue.ToString()+"\t").Length;
        CsvReaderRead = "ReadDateTimeTicks()";
        CsvWriterWrite = "WriteDateTimeTicks";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with tick precission.";
        break;

      case MemberTypeEnum.Decimal: 
        TypeString = "decimal";
        MaxStorageSize = (decimal.MinValue.ToString()+"\t").Length;
        if (isNullable) {
          CsvReaderRead = "ReadDecimalNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadDecimal()";
          NoValue = "Decimal.MinValue";
        }
        CsvWriterWrite = "Write";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with maximum precission.";
        break;

      case MemberTypeEnum.Decimal2:
        TypeString = "decimal";
        MaxStorageSize = "-1234567.89\t".Length;//reasonable limit, but could be as long as decimal.MinValue
        if (isNullable) {
          CsvReaderRead = "ReadDecimalNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadDecimal()";
          NoValue = "Decimal.MinValue";
        }
        CsvWriterWrite = "WriteDecimal2";
        ToStringFunc = "";
        PrecissionComment = "Stores decimal with 2 digits after comma.";
        Rounding = ".Round(2)";
        break;

      case MemberTypeEnum.Bool:
        TypeString = "bool";
        MaxStorageSize = "1\t".Length;
        if (isNullable) {
          CsvReaderRead = "ReadBoolNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadBool()";
          NoValue = "false";
        }
        CsvWriterWrite = "Write";
        ToStringFunc = "";
        break;

      case MemberTypeEnum.Int:
        TypeString = "int";
        MaxStorageSize = "-123456789\t".Length;//reasonable limit, but could be int.MinValue
        if (isNullable) {
          CsvReaderRead = "ReadIntNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadInt()";
          NoValue = "int.MinValue";
        }
        CsvWriterWrite = "Write";
        ToStringFunc = "";
        break;

      case MemberTypeEnum.String: 
        TypeString = "string";
        MaxStorageSize = 150;//reasonable limit, but could be much longer. CsvWriter checks if it writes longer strings and coorects this number for CsvReader
        CsvReaderRead = "ReadString()!";
        CsvWriterWrite = "Write";
        NoValue = isNullable ? "null" : $"\"No{name}\"";
        ToStringFunc = "";
        break;

      case MemberTypeEnum.List:
        throw new Exception("List uses its own constructor.");
      case MemberTypeEnum.Parent:
        throw new Exception("Parent uses its own constructor.");
      case MemberTypeEnum.Enum:
        throw new Exception("Enum needs to get constructed as Parent first and only later memberType gets changed to MemberTypeEnum.Enum.");
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
    public MemberInfo(string name, ClassInfo classInfo, string listType, string childType, string? comment, string? defaultValue) {
      MemberType = MemberTypeEnum.List;
      MaxStorageSize = 0;//a reference is only stored in the child, not the parent
      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      ClassInfo = classInfo;
      ChildTypeName = childType;
      LowerChildTypeName = childType[0..1].ToLowerInvariant() + childType[1..];
      IsNullable = false;
      TypeString = listType;
      CsvReaderRead = null;
      CsvWriterWrite = null;
      Comment = comment;
      DefaultValue = defaultValue;
    }


    /// <summary>
    /// constructor for Parent
    /// </summary>
    public MemberInfo(string name, ClassInfo classInfo, string memberTypeString, bool isNullable, string? comment, string? defaultValue) {
      MemberType = MemberTypeEnum.Parent;
      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      ClassInfo = classInfo;
      IsNullable = isNullable;
      ParentType = memberTypeString;
      LowerParentType = memberTypeString[0..1].ToLowerInvariant() + memberTypeString[1..];
      CsvWriterWrite = "Write";
      if (isNullable) {
        TypeString = memberTypeString + '?';
        CsvReaderRead = "ReadIntNull()";
        NoValue = "null";
        ToStringFunc = "?.ToShortString()";
      } else {
        TypeString = memberTypeString;
        CsvReaderRead = "ReadInt()";
        NoValue = $"{TypeString}.No{TypeString}";
        ToStringFunc = ".ToShortString()";
      }
      Comment = comment;
      DefaultValue = defaultValue;
    }


    public override string ToString() {
      string isNullableString = IsNullable ? "?" : "";
      if (MemberType==MemberTypeEnum.List) {
        return $"List<{ChildTypeName}> {MemberName}";
      }else if (MemberType==MemberTypeEnum.List) {
        return $"{MemberName}{isNullableString} {MemberName}";
      } else {
        return $"{MemberType}{isNullableString} {MemberName}";
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
        streamWriter.WriteLine("    /// <summary>");
        streamWriter.WriteLine($"    /// {PrecissionComment}");
        streamWriter.WriteLine("    ///  </summary>");
      }
      if (MemberType==MemberTypeEnum.List) {
        if (ChildCount<1) {
          throw new Exception();
        } else if (ChildCount==1) {
          streamWriter.WriteLine($"    public IReadOnly{TypeString} {MemberName} => {LowerMemberName};");
          streamWriter.WriteLine($"    readonly List<{ChildTypeName}> {LowerMemberName};");
        } else { 
          streamWriter.WriteLine($"    public ICollection<{ChildTypeName}> {MemberName} => {LowerMemberName};");
          streamWriter.WriteLine($"    readonly HashSet<{ChildTypeName}> {LowerMemberName};");
        }
      } else {
        streamWriter.WriteLine($"    public {TypeString} {MemberName} {{ get; private set; }}");
      }
    }
  }
}
