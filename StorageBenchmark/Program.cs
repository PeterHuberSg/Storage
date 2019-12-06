using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//https://benchmarkdotnet.org

namespace StorageBenchmark {
  class Program {
    static void Main(string[] args) {
#if DEBUG
      Console.WriteLine("Debug Tests");
      Console.WriteLine();
      //new Benchmarks().WriteStaticString();

      /* TestToString*/
      var benchmarkToString = new BenchmarkToString();
      //benchmarkToString.WriteStaticString();
      //benchmarkToString.WriteStringWithParams();
      //benchmarkToString.WriteBufferIf();
      //benchmarkToString.WriteBufferReverse();
      //benchmarkToString.WriteSpan();
      benchmarkToString.WriteToCsvWriter();

      /* BenchmarkFromString*/
      //var benchmarkFromString = new BenchmarkFromString();
      //benchmarkFromString.ReadString();
      //benchmarkFromString.ReadFileStream();
      //benchmarkFromString.ReadCSVReader();


#endif
#if RELEASE
      //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
      //var summary = BenchmarkRunner.Run<BenchmarkFromString>();
      var summary = BenchmarkRunner.Run<BenchmarkToString>();
#endif
      Console.WriteLine();
      Console.WriteLine("press any key to exit");
      Console.ReadKey();
    }
  }


  //public class Benchmarks {

  //  TestToString2 testToString2;

  //  public Benchmarks() {
  //    testToString2 = new TestToString2();
  //  }

  //  [Benchmark]
  //  public void WriteStaticString2() => testToString2.WriteStaticString();

  //}


  //public class Benchmarks {
  //  private const int N = 10000;
  //  private readonly byte[] data;

  //  private readonly SHA256 sha256 = SHA256.Create();
  //  private readonly MD5 md5 = MD5.Create();

  //  public Benchmarks() {
  //    data = new byte[N];
  //    new Random(42).NextBytes(data);
  //  }

  //  [Benchmark]
  //  public byte[] Sha256() => sha256.ComputeHash(data);

  //  [Benchmark]
  //  public byte[] Md5() => md5.ComputeHash(data);
  //}

}
