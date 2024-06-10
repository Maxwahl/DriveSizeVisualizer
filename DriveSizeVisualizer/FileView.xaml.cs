using System.Xml.Linq;

namespace DriveSizeVisualizer;

public partial class FileView : ContentView
{
    public static readonly BindableProperty FileProperty = BindableProperty.Create(nameof(File), typeof(DriveSizeLib.Model.File), typeof(FileView), default(DriveSizeLib.Model.File),
        propertyChanged: (b, o, n) =>
        {
            if (b is FileView fV)
            {
                fV.File = n as DriveSizeLib.Model.File ?? null;
            }
        });
    public DriveSizeLib.Model.File File
    {
        get => (DriveSizeLib.Model.File)GetValue(FileView.FileProperty);
        set => SetValue(FileView.FileProperty, value);
    }
    public FileView()
	{
		InitializeComponent();
	}
}