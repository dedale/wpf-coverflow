using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    internal class Cover : ModelVisual3D, ICover
    {
        #region Constants
        public const double CoverStep = .2;
        public static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(400);
        #endregion
        #region Fields
        private static IThumbnailManager thumbCache;
        private readonly ModelVisual3D visualModel;
        private readonly ImageSource imageSource;
        private readonly Model3DGroup modelGroup;
        private readonly AxisAngleRotation3D rotation;
        private readonly TranslateTransform3D translation;
        private readonly int pos;
        private readonly string imageName;
        #endregion
        #region Private stuff
        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
        private static MeshGeometry3D BuildMesh(Point3D p0, Point3D p1, Point3D p2, Point3D p3,
                                                Point q0, Point q1, Point q2, Point q3)
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
            return 1 - imageSource.Width / imageSource.Height;
        }
        private double RectangleDy()
        {
            if (imageSource.Width > imageSource.Height)
                return 1 - imageSource.Height / imageSource.Width;
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
            var q0 = new Point(0, 0);
            var q1 = new Point(1, 0);
            var q2 = new Point(1, 1);
            var q3 = new Point(0, 1);
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
            var q0 = new Point(0, 1);
            var q1 = new Point(1, 1);
            var q2 = new Point(1, 0);
            var q3 = new Point(0, 0);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }
        private static ImageSource LoadImageSource(ImageInfo info)
        {
            if (thumbCache == null)
                throw new InvalidOperationException("No thumbnail cache.");
            return thumbCache.GetThumbnail(info.Host, info.Path);
        }
        private static Material LoadImage(ImageSource imSrc)
        {
            return new DiffuseMaterial(new ImageBrush(imSrc));
        }
        private static Material LoadImageMirror(ImageSource imSrc)
        {
            var image = new Image { Source = imSrc };
            Color color = Color.FromArgb(127, 255, 255, 255);
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
        public Cover(ImageInfo info, int coverPos, int currentPos, ModelVisual3D model)
        {
            pos = coverPos;
            imageName = new FileInfo(info.Path).Name;
            visualModel = model;

            imageSource = LoadImageSource(info);
            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(Tessellate(), LoadImage(imageSource)));
            modelGroup.Children.Add(new GeometryModel3D(TessellateMirror(), LoadImageMirror(imageSource)));

            rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationAngle(currentPos));
            translation = new TranslateTransform3D(TranslationX(currentPos), 0, TranslationZ(currentPos));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(rotation));
            transformGroup.Children.Add(translation);
            modelGroup.Transform = transformGroup;

            Content = modelGroup;

            visualModel.Children.Add(this);
        }
        public static IThumbnailManager Cache
        {
            set { thumbCache = value; }
        }
        public void Animate(int index, bool animate)
        {
            if (animate || rotation.HasAnimatedProperties)
            {
                var rotateAnimation = new DoubleAnimation(RotationAngle(index), AnimationDuration);
                var xAnimation = new DoubleAnimation(TranslationX(index), AnimationDuration);
                var zAnimation = new DoubleAnimation(TranslationZ(index), AnimationDuration);
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, xAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, zAnimation);
            }
            else
            {
                rotation.Angle = RotationAngle(index);
                translation.OffsetX = TranslationX(index);
                translation.OffsetZ = TranslationZ(index);
            }
        }
        public bool Matches(MeshGeometry3D mesh)
        {
            foreach (GeometryModel3D part in modelGroup.Children)
                if (part.Geometry == mesh)
                    return true;
            return false;
        }
        public void Destroy()
        {
            visualModel.Children.Remove(this);
        }
        public override string ToString()
        {
            return string.Format("{0} {1}", pos, imageName);
        }
    }
}
