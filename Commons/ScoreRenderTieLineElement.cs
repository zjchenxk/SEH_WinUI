using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace SEH.Commons
{
    public class ScoreRenderTieLineElement : ScoreRenderElement
    {
        public double Width { get; set; }
        public double Height { get; set; } //线条粗细

        //控制两端钩向下弯的深度
        public double HookDepth { get; set; } = 10;

        /// <summary>
        /// 动态生成路径几何图形
        /// </summary>
        public Geometry PathData
        {
            get
            {
                PathGeometry geometry = new PathGeometry();
                PathFigure figure = new PathFigure();

                //1.起点在底部
                figure.StartPoint = new Point(0, HookDepth);

                //2.左端向上弯曲的圆角 -> (HookDepth, 0)
                QuadraticBezierSegment leftHook = new QuadraticBezierSegment();
                leftHook.Point1 = new Point(0, 0);
                leftHook.Point2 = new Point(HookDepth, 0);
                figure.Segments.Add(leftHook);

                //3.中间水平直线 -> (Width - HookDepth, 0)
                LineSegment line = new LineSegment();
                line.Point = new Point(Width - HookDepth, 0);
                figure.Segments.Add(line);

                //4.右端向下弯曲的圆角 -> (Width, HookDepth)
                QuadraticBezierSegment rightHook = new QuadraticBezierSegment();
                rightHook.Point1 = new Point(Width, 0);
                rightHook.Point2 = new Point(Width, HookDepth);
                figure.Segments.Add(rightHook);

                geometry.Figures.Add(figure);

                return geometry;
            }
        }
    }
}
