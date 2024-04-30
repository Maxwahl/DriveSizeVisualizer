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
        public static FileSystemElement AnalyzeDirectory(string path,bool ignoreInaccessible = true,CancellationTokenSource? cts =null)
        {
            if (string.IsNullOrEmpty(path)) 
            {
                throw new ArgumentException($"Argument {nameof(path)} is null or empty");
            }
            var entries = Directory.EnumerateFileSystemEntries(path, "*",
                new EnumerationOptions()
                {
                    IgnoreInaccessible = ignoreInaccessible,
                    RecurseSubdirectories = false,
                    ReturnSpecialDirectories = false
                });
            ConcurrentBag<FileSystemElement> children = new();
            Parallel.ForEach(entries,(entry) => 
            {
                FileAttributes attr = System.IO.File.GetAttributes(entry);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var dir = AnalyzeDirectory(entry, ignoreInaccessible);
                    children.Add(dir);
                }
                else
                {
                    var fileInfo = new System.IO.FileInfo(entry);
                    var file = new LibFile(entry, fileInfo.Extension, fileInfo.Length);
                    children.Add(file);
                }

            });              
            return new LibDir(path, [.. children]);

        } 
    }
}
