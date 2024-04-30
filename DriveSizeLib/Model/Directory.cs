using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Model
{
    public record Directory(string Path, FileSystemElement[] Children) : FileSystemElement(Path)
    {
        private double? _size;
        public override double SizeInKB { get
            {
                _size??= Children.Sum(it => it.SizeInKB);
                return _size.Value;
            }
        }
    }
}
