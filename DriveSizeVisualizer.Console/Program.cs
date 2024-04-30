using System;
using System.Diagnostics;
namespace DriveSizeVisualizer.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            Stopwatch stopwatch = Stopwatch.StartNew();
            var dir = DriveSizeLib.Logic.DriveSizeAnalyzer.AnalyzeDirectory(path);
            stopwatch.Stop();
            DriveSizeLib.Util.DriveSizeUtils.Print(dir);
            Console.WriteLine($"analysis took: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
