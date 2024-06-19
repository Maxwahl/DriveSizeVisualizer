using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Model
{
    public class File : FileSystemElement {
        public string Type { get; set; }
        private long _size =0;

        public File(string entry, Directory curr, string extension, long length) : base(entry,curr)
        {
            Type = extension;
            _size = length;
        }

        public File(string path, FileSystemElement? parent) : base(path, parent)
        {
        }
        public override long Size { get { return _size; }}

    }
}
