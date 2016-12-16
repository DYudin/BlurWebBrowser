using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BlurWebBrowser.Services.Abstract
{
    public interface IImageProcessingService
    {
        Bitmap BlurImage(byte[] bytes, int blurLevel);
             

        //public static void DrawPoly(Bitmap img, FaceAnnotation face)
        //{
        //    Graphics grp = Graphics.FromImage(img);
        //    List<Point> points = new List<Point>();

        //    foreach (Vertex vertex in face.FdBoundingPoly.Vertices)
        //    {
        //        points.Add(new Point((int)vertex.X, (int)vertex.Y));
        //    }
        //    grp.DrawPolygon(Pens.Red, points.ToArray());
        //}
    }
}