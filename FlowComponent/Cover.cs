using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using MediaColor = System.Windows.Media.Color;
using Point2D = System.Windows.Point;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    class Cover : ModelVisual3D
    {
        #region Constants
        public const double CoverStep = .2;
        private readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(400);
        #endregion
        #region Fields
        private readonly ImageSource imageSource;
        private readonly Model3DGroup modelGroup;
        private readonly AxisAngleRotation3D rotation;
        private readonly TranslateTransform3D translation;
        private readonly int pos;
        #endregion
        #region Private stuff
        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
        private MeshGeometry3D BuildMesh(Point3D p0, Point3D p1, Point3D p2, Point3D p3,
            Point2D q0, Point2D q1, Point2D q2, Point2D q3)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);

            var normal = CalculateNormal(p0, p1, p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.TextureCoordinates.Add(q3);
            mesh.TextureCoordinates.Add(q2);
            mesh.TextureCoordinates.Add(q1);

            normal = CalculateNormal(p2, p3, p0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.TextureCoordinates.Add(q0);
            mesh.TextureCoordinates.Add(q1);
            mesh.TextureCoordinates.Add(q2);

            mesh.Freeze();
            return mesh;
        }
        private double RectangleDx()
        {
            if (imageSource.Width > imageSource.Height)
                return 0;
            else
                return 1 - imageSource.Width / imageSource.Height;
        }
        private double RectangleDy()
        {
            if (imageSource.Width > imageSource.Height)
                return 1 - imageSource.Height / imageSource.Width;
            else
                return 0;
        }
        private Geometry3D Tessellate()
        {
            double dx = RectangleDx();
            double dy = RectangleDy();
            var p0 = new Point3D(-1 + dx, -1 + dy, 0);
            var p1 = new Point3D(1 - dx, -1 + dy, 0);
            var p2 = new Point3D(1 - dx, 1 - dy, 0);
            var p3 = new Point3D(-1 + dx, 1 - dy, 0);
            var q0 = new Point2D(0, 0);
            var q1 = new Point2D(1, 0);
            var q2 = new Point2D(1, 1);
            var q3 = new Point2D(0, 1);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }
        private Geometry3D TessellateMirror()
        {
            double dx = RectangleDx();
            double dy = RectangleDy();
            var p0 = new Point3D(-1 + dx, -3 + 3 * dy, 0);
            var p1 = new Point3D(1 - dx, -3 + 3 * dy, 0);
            var p2 = new Point3D(1 - dx, -1 + dy, 0);
            var p3 = new Point3D(-1 + dx, -1 + dy, 0);
            var q0 = new Point2D(0, 1);
            var q1 = new Point2D(1, 1);
            var q2 = new Point2D(1, 0);
            var q3 = new Point2D(0, 0);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }
        private ImageSource LoadImageSource(string imagePath)
        {
            var imageFile = new FileInfo(imagePath);
            var thumbnailDir = new DirectoryInfo(Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "tn"));
            if (!thumbnailDir.Exists)
                thumbnailDir.Create();
            var thumbnail = new FileInfo(Path.Combine(thumbnailDir.FullName, imageFile.Name));
            if (!thumbnail.Exists)
            {
                Image source = Image.FromFile(imageFile.FullName);
                int height = source.Height;
                int width = source.Width;
                int factor = (height - 1) / 250 + 1;
                int smallHeight = height / factor;
                int smallWidth = width / factor;
                Image thumb = source.GetThumbnailImage(smallWidth, smallHeight, null, IntPtr.Zero);
                thumb.Save(thumbnail.FullName);
            }
            ImageSource image = new BitmapImage(new Uri(thumbnail.FullName, UriKind.RelativeOrAbsolute));
            image.Freeze();
            return image;
        }
        private Material LoadImage(ImageSource imSrc)
        {
            return new DiffuseMaterial(new ImageBrush(imSrc));
        }
        private Material LoadImageMirror(ImageSource imSrc)
        {
            var image = new System.Windows.Controls.Image { Source = imSrc };
            MediaColor color = MediaColor.FromArgb(127, 255, 255, 255);
            image.OpacityMask = new LinearGradientBrush(color, color, 90.0);
            var brush = new VisualBrush(image);
            return new DiffuseMaterial(brush);
        }
        private double RotationAngle(int index)
        {
            return Math.Sign(pos - index) * -90;
        }
        private double TranslationX(int index)
        {
            return pos * CoverStep + Math.Sign(pos - index) * 1.5;
        }
        private double TranslationZ(int index)
        {
            return pos == index ? 1 : 0;
        }
        #endregion
        public Cover(string imagePath, int pos)
        {
            this.pos = pos;

            imageSource = LoadImageSource(imagePath);
            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(Tessellate(), LoadImage(imageSource)));
            modelGroup.Children.Add(new GeometryModel3D(TessellateMirror(), LoadImageMirror(imageSource)));

            rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationAngle(0));
            translation = new TranslateTransform3D(TranslationX(0), 0, TranslationZ(0));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(rotation));
            transformGroup.Children.Add(translation);
            modelGroup.Transform = transformGroup;

            Content = modelGroup;
        }
        public void Animate(int index)
        {
            var rotateAnimation = new DoubleAnimation(RotationAngle(index), AnimationDuration);
            var xAnimation = new DoubleAnimation(TranslationX(index), AnimationDuration);
            var zAnimation = new DoubleAnimation(TranslationZ(index), AnimationDuration);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);
            translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, xAnimation);
            translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, zAnimation);
        }
        public bool Matches(MeshGeometry3D mesh)
        {
            foreach (GeometryModel3D part in modelGroup.Children)
                if (part.Geometry == mesh)
                    return true;
            return false;
        }
    }
}
