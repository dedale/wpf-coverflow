using System.Windows;
using System.IO;
using System.Reflection;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
            var assembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var image = new FileInfo(Path.Combine(assembly.Directory.FullName, "Katy Perry.jpg"));
            for (int i = -1; i < 2; i++)
                visualModel.Children.Add(new Cover(image.FullName, i));
        }
    }
}
