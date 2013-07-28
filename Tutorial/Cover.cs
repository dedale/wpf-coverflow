using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    class Cover : ModelVisual3D
    {
        #region Fields
        private readonly Model3DGroup modelGroup;
        #endregion
        #region Private stuff
        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
        private Geometry3D Tessellate()
        {
            var p0 = new Point3D(-1, -1, 0);
            var p1 = new Point3D(1, -1, 0);
            var p2 = new Point3D(1, 1, 0);
            var p3 = new Point3D(-1, 1, 0);
            var q0 = new System.Windows.Point(0, 0);
            var q1 = new System.Windows.Point(1, 0);
            var q2 = new System.Windows.Point(1, 1);
            var q3 = new System.Windows.Point(0, 1);

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
        private ImageSource LoadImageSource(string imagePath)
        {
            Image thumb = Image.FromFile(imagePath);
            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }
        private Material LoadImage(ImageSource imSrc)
        {
            return new DiffuseMaterial(new ImageBrush(imSrc));
        }
        #endregion
        public Cover(string imagePath)
        {
            ImageSource imSrc = LoadImageSource(imagePath);
            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(Tessellate(), LoadImage(imSrc)));
            Content = modelGroup;
        }
    }
}
