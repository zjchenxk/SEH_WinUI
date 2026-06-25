using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using SEH.ViewModels;
using Windows.Graphics.Display;

namespace SEH.Views
{
    /// <summary>
    /// 编辑简谱页面
    /// </summary>
    public sealed partial class EditScorePage : Page
    {
        public EditScoreViewModel ViewModel { get; }


        public EditScorePage()
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = App.Services.GetRequiredService<EditScoreViewModel>();

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.Loaded += EditScorePage_Loaded;

            this.DataContext = ViewModel;
        }

        private void EditScorePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // 检查 XamlRoot 是否可用
            if (this.XamlRoot != null)
            {
                // 获取当前系统的缩放比例 (1.0 = 96 DPI, 1.25 = 120 DPI, 1.5 = 144 DPI 等)
                float scaleFactor = (float)this.XamlRoot.RasterizationScale;
                float dpi = 96 * scaleFactor;

                // A4 尺寸 (毫米)
                double widthInMm = 210;
                double heightInMm = 297;

                // 转换为像素
                double widthInPixels = (widthInMm / 25.4) * dpi;
                double heightInPixels = (heightInMm / 25.4) * dpi;

                // 假设你的 Canvas 名字是 ScoreCanvas
                ScoreCanvas.Width = widthInPixels;
                ScoreCanvas.Height = heightInPixels;
            }
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                //将参数传给 ViewModel 进行初始化
                ViewModel.Initialize((JObject)e.Parameter);
            }
        }

    }
}
