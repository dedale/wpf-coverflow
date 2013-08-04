using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Point2D = System.Windows.Point;
using MediaColor = System.Windows.Media.Color;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    class Cover : ModelVisual3D
    {
        #region Fields
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
        private Geometry3D Tessellate()
        {
            var p0 = new Point3D(-1, -1, 0);
            var p1 = new Point3D(1, -1, 0);
            var p2 = new Point3D(1, 1, 0);
            var p3 = new Point3D(-1, 1, 0);
            var q0 = new Point2D(0, 0);
            var q1 = new Point2D(1, 0);
            var q2 = new Point2D(1, 1);
            var q3 = new Point2D(0, 1);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }
        private Geometry3D TessellateMirror()
        {
            var p0 = new Point3D(-1, -3, 0);
            var p1 = new Point3D(1, -3, 0);
            var p2 = new Point3D(1, -1, 0);
            var p3 = new Point3D(-1, -1, 0);
            var q0 = new Point2D(0, 1);
            var q1 = new Point2D(1, 1);
            var q2 = new Point2D(1, 0);
            var q3 = new Point2D(0, 0);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }
        private ImageSource LoadImageSource(string imagePath)
        {
            Image thumb = Image.FromFile(imagePath);
            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }
        private Material LoadImage(ImageSource imSrc)
        {
            return new DiffuseMaterial(new ImageBrush(imSrc));
        }
        private Material LoadImageMirror(ImageSource imSrc)
        {
            var image = new System.Windows.Controls.Image();
            image.Source = imSrc;
            MediaColor startColor = MediaColor.FromArgb(127, 255, 255, 255);
            MediaColor endColor = MediaColor.FromArgb(127, 255, 255, 255);
            image.OpacityMask = new LinearGradientBrush(startColor, endColor, 90.0);
            var brush = new VisualBrush(image);
            return new DiffuseMaterial(brush);
        }
        private double RotationAngle(int index)
        {
            return Math.Sign(pos - index) * -90;
        }
        private double TranslationX(int index)
        {
            return pos * .2 + Math.Sign(pos - index) * 1.6;
        }
        private double TranslationZ(int index)
        {
            return pos == index ? 1 : 0;
        }
        #endregion
        public Cover(string imagePath, int pos)
        {
            this.pos = pos;

            ImageSource imSrc = LoadImageSource(imagePath);
            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(Tessellate(), LoadImage(imSrc)));
            modelGroup.Children.Add(new GeometryModel3D(TessellateMirror(), LoadImageMirror(imSrc)));

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
            TimeSpan duration = TimeSpan.FromMilliseconds(500);
            var rotateAnimation = new DoubleAnimation(RotationAngle(index), duration);
            var xAnimation = new DoubleAnimation(TranslationX(index), duration);
            var zAnimation = new DoubleAnimation(TranslationZ(index), duration);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);
            translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, xAnimation);
            translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, zAnimation);
        }
    }
}
