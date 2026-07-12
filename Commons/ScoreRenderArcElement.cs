using System;
using Windows.Foundation;

namespace SEH.Commons
{
    /// <summary>
    /// 弧线渲染元素类
    /// </summary>
    public class ScoreRenderArcElement : ScoreRenderElement
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public int ControlOffset { get; set; } = 15; //弧线高度

        public Point StartPoint => new(StartX, StartY);
        public Point EndPoint => new(EndX, EndY);
        public Point ControlPoint => new((StartX + EndX) / 2, Math.Min(StartY, EndY) - ControlOffset);
    }
}
