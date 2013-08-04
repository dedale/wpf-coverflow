using System.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public partial class TestWindow : Window
    {
        #region Fields
        private int index;
        private readonly List<Cover> coverList = new List<Cover>();
        #endregion
        private void RotateCover(int pos)
        {
            coverList[pos].Animate(index);
        }
        private void UpdateIndex(int newIndex)
        {
            if (index != newIndex)
            {
                int oldIndex = index;
                index = newIndex;
                RotateCover(oldIndex);
                RotateCover(index);
                camera.Position = new Point3D(.2 * index, camera.Position.Y, camera.Position.Z);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int newIndex = index;
            switch (e.Key)
            {
                case Key.Right:
                    if (newIndex < coverList.Count - 1)
                        newIndex++;
                    break;
                case Key.Left:
                    if (newIndex > 0)
                        newIndex--;
                    break;
            }
            UpdateIndex(newIndex);
        }
        public TestWindow()
        {
            InitializeComponent();
            var assembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var image = new FileInfo(Path.Combine(assembly.Directory.FullName, "Katy Perry.jpg"));
            for (int i = 0; i < 10; i++)
            {
                var cover = new Cover(image.FullName, i);
                coverList.Add(cover);
                visualModel.Children.Add(cover);
            }
        }
    }
}
