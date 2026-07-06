using SEH.Models;

namespace SEH.Commons
{
    /// <summary>
    /// 线条渲染元素类
    /// </summary>
    public class ScoreRenderLineElement : ScoreRenderElement
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsVertical { get; set; }
        public bool IsDashed { get; set; } = false;

        public double X1 => 0;
        public double Y1 => 0;
        public double X2 => IsVertical ? 0 : Width;
        public double Y2 => IsVertical ? Height : 0;
        public double Thickness => IsVertical ? Width : Height;

        public Note? Note { get; set; } = null;
    }
}
