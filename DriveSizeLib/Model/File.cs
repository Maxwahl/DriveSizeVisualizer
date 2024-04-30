using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Model
{
    public record File(string Path, string Type,long Size) : FileSystemElement(Path)
    {
        public override double SizeInKB { 
            get
            {
                return Size / 1024.0;
            }
        }
    }
}
