using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace SEH.Commons
{
    /// <summary>
    /// 连音线形状枚举
    /// </summary>
    public enum TieLineShape
    {
        Full,           //两端带圆角（默认）
        StartStraight,  //始端直线，末端带圆角（用于跨行的第二行开头）
        EndStraight     //始端带圆角，末端直线（用于跨行的第一行末尾）
    }

    /// <summary>
    /// 圆角连音线
    /// </summary>
    public class ScoreRenderTieLineElement : ScoreRenderElement
    {
        public double Width { get; set; }
        public double Height { get; set; } // 线条粗细
        public double HookDepth { get; set; } = 10;
        public TieLineShape Shape { get; set; } = TieLineShape.Full;

        public Geometry PathData
        {
            get
            {
                PathGeometry geometry = new PathGeometry();
                PathFigure figure = new PathFigure();

                if (Shape == TieLineShape.StartStraight)
                {
                    //1.始端是直线：直接从 (0,0) 开始
                    figure.StartPoint = new Point(0, 0);

                    //2.中间水平直线
                    LineSegment line = new LineSegment();
                    line.Point = new Point(Width - HookDepth, 0);
                    figure.Segments.Add(line);

                    //3.末端向下弯曲的圆角
                    QuadraticBezierSegment rightHook = new QuadraticBezierSegment();
                    rightHook.Point1 = new Point(Width, 0);
                    rightHook.Point2 = new Point(Width, HookDepth);
                    figure.Segments.Add(rightHook);
                }
                else if (Shape == TieLineShape.EndStraight)
                {
                    //1.始端带圆角
                    figure.StartPoint = new Point(0, HookDepth);
                    QuadraticBezierSegment leftHook = new QuadraticBezierSegment();
                    leftHook.Point1 = new Point(0, 0);
                    leftHook.Point2 = new Point(HookDepth, 0);
                    figure.Segments.Add(leftHook);

                    //2.中间水平直线，末端直线，直接延伸到 Width
                    LineSegment line = new LineSegment();
                    line.Point = new Point(Width, 0);
                    figure.Segments.Add(line);
                }
                else //Full (默认两端带钩)
                {
                    figure.StartPoint = new Point(0, HookDepth);

                    QuadraticBezierSegment leftHook = new QuadraticBezierSegment();
                    leftHook.Point1 = new Point(0, 0);
                    leftHook.Point2 = new Point(HookDepth, 0);
                    figure.Segments.Add(leftHook);

                    LineSegment line = new LineSegment();
                    line.Point = new Point(Width - HookDepth, 0);
                    figure.Segments.Add(line);

                    QuadraticBezierSegment rightHook = new QuadraticBezierSegment();
                    rightHook.Point1 = new Point(Width, 0);
                    rightHook.Point2 = new Point(Width, HookDepth);
                    figure.Segments.Add(rightHook);
                }

                geometry.Figures.Add(figure);

                return geometry;
            }
        }
    }
}
