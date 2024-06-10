using DriveSizeLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Model
{
    public record FileSystemElementUpdate(string Path,string? ParentPath)
    {
        public override string ToString()
        {
            return $"Discovered Directory: {Path}";
        }
    }
    public record FileUpdate(string Path, string? ParentPath,long Size,string Type) : FileSystemElementUpdate(Path,ParentPath)
    {
        public override string ToString()
        {
            return $"Discovered File: {Path}, Size: {DriveSizeUtils.PrettyPrintSize(Size)}, Type: {Type}";
        }
    }
}
