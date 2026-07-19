using SEH.Models;

namespace SEH.Commons
{
    /// <summary>
    /// 文本渲染元素类
    /// </summary>
    public class ScoreRenderTextElement : ScoreRenderElement
    {
        public string Text { get; set; } = "";
        public double FontSize { get; set; } = 22;

        public Note? Note { get; set; } = null;

        public bool IsBold { get; set; } = false;
    }
}
