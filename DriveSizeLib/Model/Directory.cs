using DriveSizeLib.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Model
{
    public class Directory : FileSystemElement
    {
        public Directory(string path, FileSystemElement? parent) : base(path, parent)
        {
            Children = new List<FileSystemElement>();
        }

        public List<FileSystemElement> Children { get; set; }

        public override long Size => Children.Sum(it => it.Size);    
    }
}
