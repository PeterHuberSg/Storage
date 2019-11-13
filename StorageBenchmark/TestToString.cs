using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace StorageBenchmark {


  public class TestToString {
    readonly DirectoryInfo directoryInfo;
    const int iterations = 1000000;
    const int bufferSize = 2 << 12;


    public TestToString() {
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


    [Benchmark]
    public void WriteStaticString() {
      var PathFileName = directoryInfo.FullName + @"\Test.csv";
      using (var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
        using (var streamWriter = new StreamWriter(fileStream)) {
          for (int i = 0; i < iterations; i++) {
            streamWriter.WriteLine("1;12;123;1234;12345;123456;1234567;12345678;123;");
          }
        }
      }
    }


    [Benchmark]
    public void WriteTo1() {
      var PathFileName = directoryInfo.FullName + @"\Test1.csv";
      using (var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
        using (var streamWriter = new StreamWriter(fileStream)) {
          for (int i = 0; i < iterations; i++) {
            streamWriter.WriteLine($"{i};{i+1};{i+2};{i+3};{i+4};{i+5};{i+6};");
          }
        }
      }
    }


    [Benchmark]
    public void WriteTo2() {
      var PathFileName = directoryInfo.FullName + @"\Test1.csv";
      using (var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
        using (var streamWriter = new StreamWriter(fileStream)) {
          var lineBuffer = new char[100];
          for (int i = 0; i < iterations; i++) {
            var index = 0;
            lineBuffer.Write2(i, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+1, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+2, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+3, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+4, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+5, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write2(i+6, ref index);
            lineBuffer[index++] = ';';
            streamWriter.WriteLine(lineBuffer, 0, index);
          }
        }
      }
    }


    [Benchmark]
    public void WriteTo3() {
      var PathFileName = directoryInfo.FullName + @"\Test1.csv";
      using (var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
        using (var streamWriter = new StreamWriter(fileStream)) {
          var lineBuffer = new char[100];
          for (int i = 0; i < iterations; i++) {
            var index = 0;
            lineBuffer.Write3(i, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+1, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+2, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+3, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+4, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+5, ref index);
            lineBuffer[index++] = ';';
            lineBuffer.Write3(i+6, ref index);
            lineBuffer[index++] = ';';
            streamWriter.WriteLine(lineBuffer, 0, index);
          }
        }
      }
    }




    [Benchmark]
    public void WriteTo4() {
      var PathFileName = directoryInfo.FullName + @"\Test1.csv";
      using (var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, bufferSize, FileOptions.SequentialScan)) {
        using (var streamWriter = new StreamWriter(fileStream)) {
          var lineBuffer = new char[100];
          Span<char> span = lineBuffer;
          for (int i = 0; i < iterations; i++) {
            var ok = i.TryFormat(span, out var charsWritten);
            lineBuffer[charsWritten++] = ';';
            var span1 = span[charsWritten..];
            ok = (i+1).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';
            span1 = span1[charsWritten..];
            ok = (i+2).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';
            span1 = span1[charsWritten..];
            ok = (i+3).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';
            span1 = span1[charsWritten..];
            ok = (i+4).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';
            span1 = span1[charsWritten..];
            ok = (i+5).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';
            span1 = span1[charsWritten..];
            ok = (i+6).TryFormat(span1, out charsWritten);
            span1[charsWritten++] = ';';

            var ca = lineBuffer[..(lineBuffer.Length - span1.Length + charsWritten)];
            streamWriter.WriteLine(lineBuffer, 0, lineBuffer.Length - span1.Length + charsWritten);
          }
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
