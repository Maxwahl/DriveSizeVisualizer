using DriveSizeLib.Model;

namespace DriveSizeLib.Util
{
    public static class DriveSizeUtils
    {
        public static void Print(FileSystemElement element, int indent = 0)
        {
            string indentString = new('-', 2*indent);
            Console.WriteLine($"{indentString} {element.Path}: {PrettyPrintSize(element.Size)}");
            if(element is Model.Directory dir)
            {
                if (dir.Children.Count() < 20)
                {
                    foreach (var child in dir.Children)
                    {
                        Print(child, indent + 1);
                    }
                }
            }
        }

        public static string PrettyPrintSize(long size)
        {
            if (size < 1024)
            {
                return $"{(size):F2} Bytes";
            }
            else if (size < 1024 * 1024)
            {
                return $"{(size / 1024.0):F2} KB";
            }
            else if (size < 1024 * 1024 *1024)
            {
                return $"{(size / (1024.0*1024.0)):F2} MB";
            }
            else 
                return $"{(size / (1024 * 1024 *1024.0)):F2} GB";
        }

        public static List<string> GetAllFileTypes(FileSystemElement element)
        {
            if(element is Model.Directory dir)
            {
                return dir.Children.SelectMany(it => GetAllFileTypes(it)).Distinct().ToList();
            }
            else
            {
                var file = element as Model.File;
                return [file.Type];
            }
        }
    }
}
