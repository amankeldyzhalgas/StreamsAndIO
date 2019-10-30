using System;
using System.Configuration;
using static StreamsDemo.StreamsExtension;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = ConfigurationManager.AppSettings["sourceFilePath"];

            var destination = ConfigurationManager.AppSettings["destinationFilePath"];

            //Console.WriteLine($"ByteCopy() done. Total bytes: {ByByteCopy(source, destination)}");//9404-true

            //Console.WriteLine($"InMemoryByteCopy() done. Total bytes: {InMemoryByByteCopy(source, destination)}");//9400-false

            //Console.WriteLine($"BlockCopy()() done. Total bytes: {ByBlockCopy(source, destination)}");//9404-true

            //Console.WriteLine($"InMemoryBlockCopy() done. Total bytes: {InMemoryByBlockCopy(source, destination)}");//9400-false

            //Console.WriteLine($"BufferedCopy() done. Total bytes: {BufferedCopy(source, destination)}");//9404-true

            ////Console.WriteLine($"ByLineCopy() done. Total bytes: {ByLineCopy(source, destination)}");//78-false

            Console.WriteLine(IsContentEquals(source, destination));

            Console.ReadKey();
        }
    }
}
