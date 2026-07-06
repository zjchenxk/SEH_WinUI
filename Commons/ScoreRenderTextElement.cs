using Microsoft.UI.Text;
using SEH.Models;
using Windows.UI.Text;

namespace SEH.Commons
{
    /// <summary>
    /// 文本渲染元素类
    /// </summary>
    public class ScoreRenderTextElement : ScoreRenderElement
    {
        public string Text { get; set; } = "";
        public double FontSize { get; set; } = 22;
        public bool IsBold { get; set; } = false;

        public Note? NoteSource { get; set; } = null;

        public FontWeight GetFontWeight() => IsBold ? FontWeights.Bold : FontWeights.Normal;
    }
}
