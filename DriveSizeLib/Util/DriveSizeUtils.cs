using DriveSizeLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeLib.Util
{
    public static class DriveSizeUtils
    {
        public static void Print(FileSystemElement element, int indent = 0)
        {
            string indentString = new('-', 2*indent);
            Console.WriteLine($"{indentString} {element.Path}: {PrettyPrintSize(element.SizeInKB)}");
            if(element is Model.Directory dir)
            {
                if (dir.Children.Length < 20)
                {
                    foreach (var child in dir.Children)
                    {
                        Print(child, indent + 1);
                    }
                }
            }
        }

        private static string PrettyPrintSize(double sizeInKB)
        {
            if (sizeInKB < 1)
            {
                return sizeInKB * 1024.0 + " Bytes";
            }
            else if (sizeInKB < 1024)
            {
                return sizeInKB + " KB";
            }
            else if (sizeInKB < 1024 * 1024)
            {
                return sizeInKB / 1024.0 + " MB";
            }
            else 
                return sizeInKB / (1024 * 1024) +" GB";
        }
    }
}
