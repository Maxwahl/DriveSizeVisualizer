using DriveSizeLib.Model;
using System;
using System.Diagnostics;
namespace DriveSizeVisualizer.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var path = args[0];
            Stopwatch stopwatch = Stopwatch.StartNew();
            CancellationTokenSource cts = new CancellationTokenSource();
            DriveSizeLib.Model.Directory dir =null;
            IProgress<FileSystemElementUpdate> progressReport = new Progress<FileSystemElementUpdate>(OnProgressReported);

            await Task.Run(() =>
            {
                DriveSizeLib.Logic.DriveSizeAnalyzer.AnalyzeDirectory(path, null, cts.Token,progress:progressReport);
            });
            stopwatch.Stop();
            Console.WriteLine($"analysis took: {stopwatch.ElapsedMilliseconds} ms");
        }
        public static  void OnProgressReported(FileSystemElementUpdate update)
        {
            Console.WriteLine(update);
        }
    }
}
