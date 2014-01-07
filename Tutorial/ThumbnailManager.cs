using Ded.Tutorial.Wpf.CoverFlow.FlowComponent;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public class ThumbnailManager : IThumbnailManager
    {
        #region Fields
        private readonly IsolatedStorageFile store;
        #endregion
        private static Image AmazonCut(Image image)
        {
            if (image.Width != image.Height)
                return image;
            var bmp = new Bitmap(image);
            int size = image.Height;
            int white = System.Drawing.Color.FromKnownColor(KnownColor.White).ToArgb();
            int i = 0;
            while (i < size / 2)
            {
                if (bmp.GetPixel(i, i).ToArgb() != white)
                    break;
                if (bmp.GetPixel(i, size - 1 - i).ToArgb() != white)
                    break;
                if (bmp.GetPixel(size - 1 - i, i).ToArgb() != white)
                    break;
                if (bmp.GetPixel(size - 1 - i, size - 1 - i).ToArgb() != white)
                    break;
                i++;
            }
            if (i > 0)
            {
                i += 8;
                var zone = new Rectangle(i, i, size - 2 * i, size - 2 * i);
                return bmp.Clone(zone, System.Drawing.Imaging.PixelFormat.DontCare);
            }
            return bmp;
        }
        private byte[] GetThumbnail(string path)
        {
            using (Image fileImage = Image.FromFile(path))
            using (Image source = AmazonCut(fileImage))
            {
                int height = source.Height;
                int width = source.Width;
                int factor = (height - 1) / 250 + 1;
                int smallHeight = height / factor;
                int smallWidth = width / factor;
                using (Image thumb = source.GetThumbnailImage(smallWidth, smallHeight, null, IntPtr.Zero))
                using (var ms = new MemoryStream())
                {
                    thumb.Save(ms, ImageFormat.Png);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    var result = new byte[ms.Length];
                    ms.Read(result, 0, (int)ms.Length);
                    return result;
                }
            }
        }
        public ThumbnailManager()
        {
            store = IsolatedStorageFile.GetUserStoreForAssembly();
        }
        public ImageSource GetThumbnail(string host, string path)
        {
            string thumbName = Path.GetFileName(path);
            if (store.GetFileNames(thumbName).Length == 0)
            {
                using (var stream = new IsolatedStorageFileStream(thumbName, FileMode.CreateNew, store))
                {
                    byte[] data = GetThumbnail(path);
                    stream.Write(data, 0, data.Length);
                }
            }
            using (var stream = new IsolatedStorageFileStream(thumbName, FileMode.Open, store))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}
