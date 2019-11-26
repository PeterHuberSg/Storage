using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Storage;

namespace StorageBenchmark {


  public class BenchmarkFromString {
    readonly DirectoryInfo directoryInfo;
    const int iterations = 1000000;
    readonly string pathFileName;
    readonly int delimiter = (int)';';
    readonly int newLine0 = Environment.NewLine[0];
    readonly int newLine1 = Environment.NewLine[1];


    public BenchmarkFromString() {
      directoryInfo = new DirectoryInfo("TestFileSpeed");
      if (directoryInfo.Exists) {
        directoryInfo.Delete(recursive: true);
        directoryInfo.Refresh();
      }

      directoryInfo.Create();
      directoryInfo.Refresh();
      pathFileName = directoryInfo.FullName + @"\Test.csv";
      using (var fileStream = new FileStream(pathFileName, FileMode.CreateNew)) {
        using var streamWriter = new StreamWriter(fileStream);
        for (int i = 0; i < iterations; i++) {
          streamWriter.WriteLine($"{i};{i+1};{i+2};{i+3};{i+4};{i+5};{i+6};");
        }
      }
#if DEBUG
      Console.WriteLine($"Directory: {directoryInfo.FullName}");
#endif
    }


    [Benchmark]
    public int ReadString() {
      var total = 0;
      var sw = new Stopwatch();
      sw.Start();
      using (var fileStream = new FileStream(pathFileName, FileMode.Open)) {
        using var streamReader = new StreamReader(fileStream);
        while (!streamReader.EndOfStream) {
          var line = streamReader.ReadLine();
          foreach (var field in line.Split(';', StringSplitOptions.RemoveEmptyEntries)) {
            var i = int.Parse(field);
            total += i;
          }
        }
      }
      sw.Stop();
      return total;
    }


    [Benchmark]
    public int ReadFileStream() {
      var total = 0;
      var esb = new StringBuilder();
      using var fileStream = new FileStream(pathFileName, FileMode.Open);
      var fileLength = fileStream.Length;
      while (true) {
        for (int fieldIndex = 0; fieldIndex < 7; fieldIndex++) {
          var (isEof, i)= fileStream.ReadInt(delimiter, "Test", esb);
          if (isEof) {
            if (fieldIndex==0) {
              return total;
            }

            throw new Exception();
          }
          total += i;
        }
        var newLineByte0 = fileStream.ReadByte();
        if (newLineByte0!=newLine0) throw new Exception();
        var newLineByte1 = fileStream.ReadByte();
        if (newLineByte1!=newLine1) throw new Exception();
      }
    }


    /*
    filestream buffer has the same size as byteArray, FileAccess.Read, FileMode.Open
| Buffer |     Mean |   Error |  StdDev |
|------- |---------:|--------:|--------:|
|     4k | 147.5 ms | 2.83 ms | 3.03 ms |
|     8k | 129.1 ms | 4.53 ms | 3.78 ms |
|    16k | 120.1 ms | 2.40 ms | 4.44 ms |
|    32k | 113.2 ms | 1.63 ms | 1.27 ms |
|    64k | 110.8 ms | 2.21 ms | 2.46 ms |
|   128k | 109.0 ms | 1.26 ms | 1.06 ms |
|   128k | 109.1 ms | 2.14 ms | 2.46 ms | FileStreamBuffer = 4k
|   128k | 110.3 ms | 2.50 ms | 2.45 ms | FileAccess.ReadWrite
|   128k | 109.8 ms | 2.09 ms | 2.15 ms | FileMode.OpenOrCreate
|   128k | 108.7 ms | 1.37 ms | 1.15 ms | FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FileOptions.SequentialScan | FileOptions.WriteThrough
    */

    /* after removing checks for every byte read if there is another byte in the buffer
|         Method |      Mean |     Error |    StdDev |    Median |
|--------------- |----------:|----------:|----------:|----------:|
|     ReadString | 573.33 ms | 24.275 ms | 71.194 ms | 539.47 ms |
| ReadFileStream | 519.38 ms | 10.375 ms | 25.837 ms | 519.64 ms |
|  ReadCSVReader |  99.86 ms |  1.256 ms |  1.175 ms |  99.95 ms |

    */
    [Benchmark]
    public int ReadCSVReader() {
      var total = 0;
      var csvConfig = new CsvConfig(directoryInfo.FullName, ';', bufferSize: 1<<17, reportException: reportException);
      var fileName = csvConfig.DirectoryPath + @"\TestCsvReader.csv";

      using var csvReader = new CsvReader(pathFileName, csvConfig, maxLineLenght: 60);
      do {
        for (int fieldIndex = 0; fieldIndex<7; fieldIndex++) {
          var i = csvReader.ReadInt();
          total += i;
        }
        csvReader.ReadEndOfLine();
      } while (!csvReader.IsEndOfFileReached());
      return total;
    }


    private void reportException(Exception obj) {
      throw new Exception();
    }
  }
}