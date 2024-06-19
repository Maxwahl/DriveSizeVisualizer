using DriveSizeLib.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DriveSizeLib.Model
{
    public class Directory(string path, FileSystemElement? parent) : FileSystemElement(path, parent)
    {
        public IList<FileSystemElement> Children { get; set; } = [];



        public override long Size => Children.Sum(it => it.Size);

        internal void AddChild(FileSystemElement? child)
        {
            lock (this.Children)
            {
                this.Children.Add(child);
            }
        }

        public void SetChildren(List<FileSystemElement> newChildren)
        {
            lock(this.Children)
            {
                Children.Clear();
                foreach (var child in newChildren)
                    Children.Add(child);
            }
        }

        

    }
}
