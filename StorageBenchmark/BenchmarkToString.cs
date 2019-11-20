using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Storage;

namespace StorageBenchmark {

  /*
  4k Buffer
|            Method |      Mean |     Error |    StdDev |
|------------------ |----------:|----------:|----------:|
| WriteStaticString |  75.31 ms |  1.609 ms |  3.286 ms |
|          WriteTo1 | 596.46 ms | 20.874 ms | 31.877 ms |
|          WriteTo2 | 192.55 ms |  3.665 ms |  5.017 ms |
|          WriteTo3 | 191.13 ms |  4.567 ms |  6.097 ms |
|          WriteTo4 | 312.49 ms |  6.036 ms |  7.848 ms |
|  WriteToCsvWriter | 926.4  ms | 18.34  ms | 23.85  ms |

  32k Buffer
|            Method |      Mean |     Error |    StdDev |    Median |
|------------------ |----------:|----------:|----------:|----------:|
| WriteStaticString |  38.31 ms |  0.755 ms |  0.927 ms |  38.24 ms |
|          WriteTo1 | 531.44 ms | 10.474 ms | 19.928 ms | 529.53 ms |
|          WriteTo2 | 157.27 ms |  3.854 ms |  4.438 ms | 156.05 ms |
|          WriteTo3 | 157.46 ms |  3.236 ms |  9.285 ms | 153.80 ms |
|          WriteTo4 | 274.52 ms |  5.418 ms | 12.665 ms | 272.23 ms |
|  WriteToCsvWriter | 288.12 ms |  5.698 ms |  9.676 ms | 284.27 ms |

  64k Buffer
|            Method |      Mean |     Error |    StdDev |
|------------------ |----------:|----------:|----------:|
| WriteStaticString |  44.67 ms |  1.432 ms |  1.270 ms |
|          WriteTo1 | 540.44 ms | 10.784 ms | 20.518 ms |
|          WriteTo2 | 165.21 ms |  3.257 ms |  5.956 ms |
|          WriteTo3 | 162.02 ms |  3.193 ms |  4.370 ms |
|          WriteTo4 | 283.55 ms |  5.590 ms | 10.769 ms |
|  WriteToCsvWriter | 144.01 ms |  2.378 ms |  1.985 ms |
  */

  public class BenchmarkToString {
    readonly DirectoryInfo directoryInfo;
    const int iterations = 1000000;
    //const int bufferSize = 1 << 12;
    const int bufferSize = 1 << 16;


    public BenchmarkToString() {
      directoryInfo = new DirectoryInfo("TestFileSpeed");
      if (directoryInfo.Exists) {
        directoryInfo.Delete(recursive: true);
        directoryInfo.Refresh();
      }

      directoryInfo.Create();
      directoryInfo.Refresh();
#if DEBUG
      Console.WriteLine($"Directory: {directoryInfo.FullName}");
#endif
    }


    //[Benchmark]
    //public void WriteStaticString() {
    //  var PathFileName = directoryInfo.FullName + @"\Test.csv";
    //  using (var fileStream = new FileStream(PathFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
    //    using (var streamWriter = new StreamWriter(fileStream)) {
    //      for (int i = 0; i < iterations; i++) {
    //        streamWriter.WriteLine("1;12;123;1234;12345;123456;1234567;12345678;123;");
    //      }
    //    }
    //  }
    //}


    //[Benchmark]
    //public void WriteTo1() {
    //  var PathFileName = directoryInfo.FullName + @"\Test1.csv";
    //  using (var fileStream = new FileStream(PathFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
    //    using (var streamWriter = new StreamWriter(fileStream)) {
    //      for (int i = 0; i < iterations; i++) {
    //        streamWriter.WriteLine($"{i};{i+1};{i+2};{i+3};{i+4};{i+5};{i+6};");
    //      }
    //    }
    //  }
    //}


    //[Benchmark]
    //public void WriteTo2() {
    //  var PathFileName = directoryInfo.FullName + @"\Test2.csv";
    //  using (var fileStream = new FileStream(PathFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
    //    using (var streamWriter = new StreamWriter(fileStream)) {
    //      var lineBuffer = new char[100];
    //      for (int i = 0; i < iterations; i++) {
    //        var index = 0;
    //        lineBuffer.Write2(i, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+1, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+2, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+3, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+4, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+5, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write2(i+6, ref index);
    //        lineBuffer[index++] = ';';
    //        streamWriter.WriteLine(lineBuffer, 0, index);
    //      }
    //    }
    //  }
    //}


    //[Benchmark]
    //public void WriteTo3() {
    //  var PathFileName = directoryInfo.FullName + @"\Test3.csv";
    //  using (var fileStream = new FileStream(PathFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
    //    using (var streamWriter = new StreamWriter(fileStream)) {
    //      var lineBuffer = new char[100];
    //      for (int i = 0; i < iterations; i++) {
    //        var index = 0;
    //        lineBuffer.Write3(i, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+1, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+2, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+3, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+4, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+5, ref index);
    //        lineBuffer[index++] = ';';
    //        lineBuffer.Write3(i+6, ref index);
    //        lineBuffer[index++] = ';';
    //        streamWriter.WriteLine(lineBuffer, 0, index);
    //      }
    //    }
    //  }
    //}




    //[Benchmark]
    //public void WriteTo4() {
    //  var PathFileName = directoryInfo.FullName + @"\Test4.csv";
    //  using (var fileStream = new FileStream(PathFileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
    //    using (var streamWriter = new StreamWriter(fileStream)) {
    //      var lineBuffer = new char[100];
    //      Span<char> span = lineBuffer;
    //      for (int i = 0; i < iterations; i++) {
    //        var ok = i.TryFormat(span, out var charsWritten);
    //        lineBuffer[charsWritten++] = ';';
    //        var span1 = span[charsWritten..];
    //        ok = (i+1).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';
    //        span1 = span1[charsWritten..];
    //        ok = (i+2).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';
    //        span1 = span1[charsWritten..];
    //        ok = (i+3).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';
    //        span1 = span1[charsWritten..];
    //        ok = (i+4).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';
    //        span1 = span1[charsWritten..];
    //        ok = (i+5).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';
    //        span1 = span1[charsWritten..];
    //        ok = (i+6).TryFormat(span1, out charsWritten);
    //        span1[charsWritten++] = ';';

    //        var ca = lineBuffer[..(lineBuffer.Length - span1.Length + charsWritten)];
    //        streamWriter.WriteLine(lineBuffer, 0, lineBuffer.Length - span1.Length + charsWritten);
    //      }
    //    }
    //  }
    //}

    /*
|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
| WriteToCsvWriter | 926.4 ms | 18.34 ms | 23.85 ms | exactly 4k buffer
| WriteToCsvWriter | 580.8 ms | 10.39 ms |  8.67 ms | exactly 8k buffer
| WriteToCsvWriter | 417.3 ms |  8.15 ms | 14.28 ms | exactly 16k buffer
| WriteToCsvWriter | 333.5 ms |  6.57 ms | 11.33 ms | exactly 32k buffer
| WriteToCsvWriter | 288.5 ms |  5.72 ms |  8.20 ms | exactly 64k buffer
| WriteToCsvWriter | 276.5 ms |  5.39 ms |  6.82 ms | exactly 132k buffer

64k Buffer, FileShare.None, 
| WriteToCsvWriter | 287.9 ms |  6.42 ms | 11.24 ms | FileMode.OpenOrCreate, FileAccess.Write, FileOptions.WriteThrough
| WriteToCsvWriter | 145.1 ms |  2.80 ms |  3.33 ms | FileMode.OpenOrCreate, FileAccess.Write
| WriteToCsvWriter | 154.2 ms |  3.19 ms |  5.15 ms | FileMode.Create, FileAccess.Write
| WriteToCsvWriter | 152.7 ms |  2.94 m  |  4.13 ms | FileMode.Create, FileAccess.Write, FileOptions.SequentialScan
| WriteToCsvWriter | 149.6 ms |  2.97 ms |  7.72 ms | FileMode.OpenOrCreate, FileAccess.ReadWrite, FileOptions.SequentialScan

| WriteToCsvWriter | 141.3 ms | 2.04 ms | 1.71 ms | writing full buffers to FileStream
| WriteToCsvWriter | 161.2 ms | 4.40 ms | 4.32 ms | with locks for flushtimer
    */
    [Benchmark]
    public void WriteToCsvWriter() {
      var PathFileName = directoryInfo.FullName + @"\Test5.csv";
      var csvConfig = new CsvConfig(directoryInfo.FullName, bufferSize: 1<<16);
      using (var csvWriter = new CsvWriter(PathFileName, csvConfig, 60, isAsciiOnly: true)) {
        for (int i = 0; i < iterations; i++) {
          csvWriter.Write(i);
          csvWriter.Write(i+1);
          csvWriter.Write(i+2);
          csvWriter.Write(i+3);
          csvWriter.Write(i+4);
          csvWriter.Write(i+5);
          csvWriter.Write(i+6);
          csvWriter.WriteEndOfLine();
        }
      }
    }
  }
}

public static class SpanExtensions {

  public static void Write2(this char[] charArray, int i, ref int index) {
    if (i<0) {
      charArray[index++] = '-';
      i = -i;
    }
    int length;
    if (i<10000) {
      if (i<100) {
        if (i<10) {
          length = 1;
        } else {
          length = 2;
        }
      } else {
        if (i<1000) {
          length = 3;
        } else {
          length = 4;
        }
      }
    } else if (i<100000000) {
      if (i<1000000) {
        if (i<100000) {
          length = 5;
        } else {
          length = 6;
        }
      } else {
        if (i<10000000) {
          length = 7;
        } else {
          length = 8;
        }
      }
    } else if (i<1000000000) {
      length = 9;
    } else {
      length = 10;
    }
    index += length - 1;
    while (i>9) {
      charArray[index--] = (char)((i % 10) + '0');
      i /= 10;
    }
    charArray[index--] = (char)(i + '0');
    index += length + 1;
  }


  public static void Write3(this char[] charArray, int i, ref int index) {
    if (i<0) {
      charArray[index++] = '-';
      i = -i;
    }
    int start = index;

    while (i>9) {
      charArray[index++] = (char)((i % 10) + '0');
      i /= 10;
    }
    charArray[index++] = (char)(i + '0');
    var end = index-1;
    while (end>start) {
      var temp = charArray[end];
      charArray[end--] = charArray[start];
      charArray[start++] = temp;
    }
  }


  public static void Write4(this char[] charArray, int i, ref int index) {
    if (i<0) {
      charArray[index++] = '-';
      i = -i;
    }
    int start = index;

    while (i>9) {
      charArray[index++] = (char)((i % 10) + '0');
      i /= 10;
    }
    charArray[index++] = (char)(i + '0');
    var end = index-1;
    while (end>start) {
      var temp = charArray[end];
      charArray[end--] = charArray[start];
      charArray[start++] = temp;
    }
  }
}
