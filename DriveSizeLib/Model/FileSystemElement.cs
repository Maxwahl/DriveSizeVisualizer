using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DriveSizeLib.Model
{
    public abstract record FileSystemElement(string Path)
    {
        public abstract double SizeInKB { get; }
    }
}
