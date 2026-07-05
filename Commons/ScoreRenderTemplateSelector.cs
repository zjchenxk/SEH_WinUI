using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SEH.Commons
{
    public partial class ScoreRenderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextDataTemplate { get; set; }
        public DataTemplate? DotDataTemplate { get; set; }
        public DataTemplate? LineDataTemplate { get; set; }
        public DataTemplate? ArcDataTemplate { get; set; }
        public DataTemplate? FermataDataTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item)
        {
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null) return null;

            if (item is ScoreRenderTextElement) return TextDataTemplate;
            if (item is ScoreRenderDotElement) return DotDataTemplate;
            if (item is ScoreRenderLineElement) return LineDataTemplate;
            if (item is ScoreRenderArcElement) return ArcDataTemplate;
            if (item is ScoreRenderFermataElement) return FermataDataTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
