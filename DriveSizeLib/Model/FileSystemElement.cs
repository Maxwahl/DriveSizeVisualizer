using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DriveSizeLib.Model
{
    public abstract class FileSystemElement
    {
        public string Path { get; set; }
        public FileSystemElement? Parent { get; set; }
        [Sortable]
        public abstract long Size { get; }

        protected FileSystemElement(string path, FileSystemElement? parent)
        {
            Path = path;
            Parent = parent;
        }
        [Sortable]
        public string Name
        {
            get
            {
                if (Parent is null)
                    return Path;
                else
                    return Path.Split('\\')[^1];
            }
        }
        public override string ToString()
        {
            return $"{Path} {Util.DriveSizeUtils.PrettyPrintSize(Size)}";
        }
        public string DisplayString
        {
            get
            {
                return $"{Name}\t\t{Util.DriveSizeUtils.PrettyPrintSize(Size)}";
            }
        }
    }
}
