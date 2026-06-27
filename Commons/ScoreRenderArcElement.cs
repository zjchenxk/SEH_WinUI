using System;
using Windows.Foundation;

namespace SEH.Commons
{
    /// <summary>
    /// 弧线渲染元素类
    /// </summary>
    public class ScoreRenderArcElement : ScoreRenderElement
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public int ControlOffset { get; set; } = 20; //弧线高度

        public Point StartPoint => new(StartX, StartY);
        public Point EndPoint => new(EndX, EndY);
        public Point ControlPoint => new((StartX + EndX) / 2, Math.Min(StartY, EndY) - ControlOffset);
    }
}
