using DriveSizeLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Directory = System.IO.Directory;
using LibFile = DriveSizeLib.Model.File;
using  LibDir = DriveSizeLib.Model.Directory;
using File = DriveSizeLib.Model.File;
using System.Collections.Concurrent;

namespace DriveSizeLib.Logic
{
    public static class DriveSizeAnalyzer
    {
        public static async Task<Model.Directory?> AnalyzeDirectory(string path, Model.Directory? parent, CancellationToken cts,bool ignoreInaccessible = true,bool computeParallel = false,IProgress<FileSystemElementUpdate>? progress = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"Argument {nameof(path)} is null or empty");
            }
            if (cts.IsCancellationRequested)
                return null;
            var entries = Directory.GetFileSystemEntries(path, "*",
                new EnumerationOptions()
                {
                    IgnoreInaccessible = ignoreInaccessible,
                    RecurseSubdirectories = false,
                    ReturnSpecialDirectories = false,
                    BufferSize = 4096,
                });
            var directory = new LibDir(path, parent);
            progress?.Report(new FileSystemElementUpdate(path, parent?.Path ?? ""));
            if (!cts.IsCancellationRequested)
            {
                if(!computeParallel)
                {
                    foreach (var entry in entries)
                    {
                        await DiscoverDirectory(path, entry, directory, cts, ignoreInaccessible,computeParallel, progress);
                    }
                }
                else
                {
                    Parallel.ForEach(entries, async (entry) => await DiscoverDirectory(path, entry, directory, cts, ignoreInaccessible, computeParallel, progress));
                }
                

            }
            return directory;
        }



        private static async Task DiscoverDirectory(string path, string entry, LibDir directory, CancellationToken cts, bool ignoreInaccessible,bool computeParallel, IProgress<FileSystemElementUpdate>? progress)
        {
            FileAttributes attr = System.IO.File.GetAttributes(entry);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {

                if (!cts.IsCancellationRequested)
                {
                    var child = await AnalyzeDirectory(entry, directory, cts, ignoreInaccessible,computeParallel, progress);
                        directory.AddChild(child);                    
                }
            }
            else
            {
                var fileInfo = new FileInfo(entry);
                progress?.Report(
                    new FileUpdate(entry, path, fileInfo.Length, fileInfo.Extension)
                    );
                var file = new Model.File(entry, directory, fileInfo.Extension, fileInfo.Length);
                directory.AddChild(file);
            }
        }
    }
}
