﻿/**************************************************************************************

Storage.MemberInfo
==================

Some info for each class defined in data model

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.IO;


namespace Storage {


  /// <summary>
  /// Some info for each class defined in data model
  /// </summary>
  public class MemberInfo {
    public readonly string MemberName;
    public readonly string LowerMemberName;
    public MemberTypeEnum MemberType;
    public CollectionTypeEnum CollectionType;
    public readonly ClassInfo ClassInfo;
    public string? CsvTypeString;
    public string TypeString;
    public string? ReadOnlyTypeString; //used by Dictionary and SortedList: IReadOnlyDictionary<DateTime, DictionaryChild>
    public readonly bool IsNullable;
    public readonly bool IsReadOnly;
    public readonly string? Comment;
    public readonly string? PrecissionComment;
    public readonly string? Rounding;
    public readonly string? DefaultValue;
    public readonly bool IsLookupOnly = false;
    public readonly int MaxStorageSize;
    public readonly string? ChildTypeName; //used by List, Dictionary and SortedList
    public readonly string? LowerChildTypeName; //used by List, Dictionary and SortedList
    public readonly string? ChildKeyPropertyName; //used by Dictionary and SortedList
    public readonly string? ChildKeyTypeString; //used by Dictionary and SortedList
    public readonly string? ParentType;//is only different from TypeString when IsNullable
    public readonly string? LowerParentType;
    public readonly string? CsvReaderRead;
    public readonly string? CsvWriterWrite;
    public readonly string? NoValue; //used to fill NoClass with a obviously bad value
    public string? ToStringFunc;

    public ClassInfo? ChildClassInfo;
    public bool IsChildReadOnly;
    public ClassInfo? ParentClassInfo; //not really used
    public EnumInfo? EnumInfo;
    public int ChildCount = 0;


    public MemberInfo(
      string name, 
      string csvTypeString, 
      MemberTypeEnum memberType, 
      ClassInfo classInfo, 
      bool isNullable,
      bool isReadOnly,
      string? comment, 
      string? defaultValue) 
    {
      if (CollectionType>CollectionTypeEnum.Undefined) throw new Exception();

      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      CsvTypeString = csvTypeString;
      MemberType = memberType;
      ClassInfo = classInfo;
      IsNullable = isNullable;
      IsReadOnly = isReadOnly;
      Comment = comment;
      DefaultValue = defaultValue;
      switch (memberType) {
      case MemberTypeEnum.Date:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234\t".Length;
        CsvWriterWrite = "WriteDate";
        if (isNullable) {
          CsvReaderRead = "ReadDateNull()";
          NoValue = "null";
          ToStringFunc = "?.ToShortDateString()";
        } else {
          CsvReaderRead = "ReadDate()";
          NoValue = "DateTime.MinValue.Date";
          ToStringFunc = ".ToShortDateString()";
        }
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
        PrecissionComment = "Stores less than 24 hours with second precision.";
        Rounding = ".Round(Rounding.Seconds)";
        break;

      case MemberTypeEnum.DateMinutes:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234 23:59\t".Length;
        CsvReaderRead = "ReadDateSeconds()";//can also be used for minutes
        CsvWriterWrite = "WriteDateMinutes";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with minute preclusion.";
        Rounding = ".Round(Rounding.Minutes)";
        break;

      case MemberTypeEnum.DateSeconds:
        TypeString = "DateTime";
        MaxStorageSize = "12.12.1234 23:59:59\t".Length;
        CsvReaderRead = "ReadDateSeconds()";
        CsvWriterWrite = "WriteDateSeconds";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with seconds precision.";
        Rounding = ".Round(Rounding.Seconds)";
        break;

      case MemberTypeEnum.DateTime: 
        TypeString = "DateTime";
        MaxStorageSize = (long.MaxValue.ToString()+"\t").Length;
        CsvReaderRead = "ReadDateTimeTicks()";
        CsvWriterWrite = "WriteDateTimeTicks";
        NoValue = "DateTime.MinValue";
        ToStringFunc = "";
        PrecissionComment = "Stores date and time with tick precision.";
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
        PrecissionComment = "Stores date and time with maximum precision.";
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

      case MemberTypeEnum.Decimal4:
        TypeString = "decimal";
        MaxStorageSize = "-12345.6789\t".Length;//reasonable limit, but could be as long as decimal.MinValue
        if (isNullable) {
          CsvReaderRead = "ReadDecimalNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadDecimal()";
          NoValue = "Decimal.MinValue";
        }
        CsvWriterWrite = "WriteDecimal4";
        ToStringFunc = "";
        PrecissionComment = "Stores decimal with 4 digits after comma.";
        Rounding = ".Round(4)";
        break;

      case MemberTypeEnum.Decimal5:
        TypeString = "decimal";
        MaxStorageSize = "-1234.56789\t".Length;//reasonable limit, but could be as long as decimal.MinValue
        if (isNullable) {
          CsvReaderRead = "ReadDecimalNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadDecimal()";
          NoValue = "Decimal.MinValue";
        }
        CsvWriterWrite = "WriteDecimal5";
        ToStringFunc = "";
        PrecissionComment = "Stores decimal with 5 digits after comma.";
        Rounding = ".Round(5)";
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
        MaxStorageSize = 150;//reasonable limit, but could be much longer. CsvWriter checks if it writes longer strings and corrects this number for CsvReader
        if (isNullable) {
          CsvReaderRead = "ReadStringNull()";
          NoValue = "null";
        } else {
          CsvReaderRead = "ReadString()";
          NoValue = $"\"No{name}\"";
        }
        CsvWriterWrite = "Write";
        ToStringFunc = "";
        break;

      case MemberTypeEnum.List:
        throw new Exception("List uses its own constructor.");
      case MemberTypeEnum.CollectionKeyValue:
        throw new Exception("Dictionary and SortedList uses its own constructor.");
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
    public MemberInfo(
      string name, 
      ClassInfo classInfo, 
      string listType, 
      string childType, 
      string? comment, 
      string? defaultValue) 
    {
      MemberType = MemberTypeEnum.List;
      CollectionType = CollectionTypeEnum.List;
      MaxStorageSize = 0;//a reference is only stored in the child, not the parent
      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      ClassInfo = classInfo;
      ChildTypeName = childType;
      LowerChildTypeName = childType[0..1].ToLowerInvariant() + childType[1..];
      IsNullable = false;
      IsReadOnly = false; //List properties are IReadOnlyList, but not need to mark them with ReadOnly
      TypeString = listType;
      CsvReaderRead = null;
      CsvWriterWrite = null;
      Comment = comment;
      DefaultValue = defaultValue;
    }


    /// <summary>
    /// constructor for CollectionKeyValue, i.e. Dictionary&lt;TKey, TValue> or SortedList&lt;TKey, TValue>
    /// </summary>
    public MemberInfo(
      string name, 
      ClassInfo classInfo, 
      string memberTypeString,
      CollectionTypeEnum collectionTypeEnum,
      string childType,
      string childKeyPropertyName, 
      string keyTypeString, 
      string? comment, 
      string? 
      defaultValue) 
    {
      MemberType = MemberTypeEnum.CollectionKeyValue;
      CollectionType = collectionTypeEnum;
      MaxStorageSize = 0;//a reference is only stored in the child, not the parent
      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      ClassInfo = classInfo;
      ChildTypeName = childType;
      LowerChildTypeName = childType[0..1].ToLowerInvariant() + childType[1..];
      ChildKeyPropertyName = childKeyPropertyName;
      ChildKeyTypeString = keyTypeString;
      IsNullable = false;
      IsReadOnly = false; //Collection properties are IReadOnlyXXX, but not need to mark them with ReadOnly
      TypeString = memberTypeString; //will be overwritten in Compiler.AnalyzeDependencies()
      CsvReaderRead = null;
      CsvWriterWrite = null;
      Comment = comment;
      DefaultValue = defaultValue;
    }


    /// <summary>
    /// constructor for Parent
    /// </summary>
    public MemberInfo(
      string name, 
      ClassInfo classInfo, 
      string memberTypeString, 
      bool isNullable,
      bool isReadOnly,
      string? comment, 
      string? defaultValue,
      bool isLookupOnly) 
    {
      MemberType = MemberTypeEnum.Parent;
      MemberName = name;
      LowerMemberName = name[0..1].ToLowerInvariant() + name[1..];
      ClassInfo = classInfo;
      IsNullable = isNullable;
      IsReadOnly = isReadOnly;
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
      IsLookupOnly = isLookupOnly;
    }


    public override string ToString() {
      string isNullableString = IsNullable ? "?" : "";
      if (MemberType==MemberTypeEnum.List) {
        return $"List<{ChildTypeName}> {MemberName}";
      }else if (MemberType==MemberTypeEnum.CollectionKeyValue) {
        return $"{MemberName} {TypeString}";
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
      } else if (MemberType==MemberTypeEnum.CollectionKeyValue) {
        //streamWriter.WriteLine($"    public IReadOnly{TypeString} {MemberName} => {LowerMemberName};");
        streamWriter.WriteLine($"    public {ReadOnlyTypeString} {MemberName} => {LowerMemberName};");
        streamWriter.WriteLine($"    readonly {TypeString} {LowerMemberName};");

      } else {
        if (IsReadOnly) {
          streamWriter.WriteLine($"    public {TypeString} {MemberName} {{ get; }}");
        } else {
          streamWriter.WriteLine($"    public {TypeString} {MemberName} {{ get; private set; }}");
        }
      }
    }
  }
}
