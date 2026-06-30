using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using SEH.Commons;
using SEH.Services.Interfaces;
using SEH.ViewModels;

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

            this.DataContext = ViewModel;

            this.Loaded += EditScorePage_Loaded;
        }

        private void EditScorePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //从 ViewModel 获取 DialogService 并设置 XamlRoot
            //注意：这里可以通过 App.Services 直接获取服务实例
            var dialogService = App.Services.GetRequiredService<IDialogService>();
            if (dialogService is SEH.Services.DialogService ds)
            {
                ds.XamlRoot = this.XamlRoot;
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

        private void TextBlock_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //sender 是触发事件的 UI 控件 (即 TextBlock)
            if (sender is Microsoft.UI.Xaml.FrameworkElement element && element.DataContext is ScoreRenderTextElement textElement)
            {
                //调用 ViewModel 中的方法
                ViewModel.OnNoteTappedAsync(textElement);
            }
        }
    }
}
