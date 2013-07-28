using System.Windows;
using System.IO;
using System.Reflection;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
            var assembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var image = new FileInfo(Path.Combine(assembly.Directory.FullName, "Katy Perry.jpg"));
            var cover = new Cover(image.FullName);
            visualModel.Children.Add(cover);
        }
    }
}
