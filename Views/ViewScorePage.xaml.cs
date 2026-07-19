using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SEH.Commons;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 查看简谱页面
    /// </summary>
    public sealed partial class ViewScorePage : Page
    {
        public ViewScoreViewModel? ViewModel { get; }

        public ViewScorePage()
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = App.Services?.GetRequiredService<ViewScoreViewModel>();

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                //将参数传给 ViewModel 进行初始化
                ViewModel?.Initialize((string)e.Parameter);
            }
        }

        private async void ScoreRegion_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //获取触发点击事件的原始源（通常是一个 UI 元素）
            if (e.OriginalSource is FrameworkElement fe)
            {
                //检查它的 DataContext 是哪种渲染元素
                if (fe.DataContext is ScoreRenderTextElement textElement)
                {
                    //如果是文本元素，拿到绑定的 Note 对象，调用 ViewModel 的方法
                    if (ViewModel != null && textElement != null && textElement.Note != null)
                    {
                        //调用 ViewModel 中的点击事件程序
                        await ViewModel.OnNoteTappedAsync(textElement.Note);
                    }
                }
                else if (fe.DataContext is ScoreRenderLineElement lineElement)
                {
                    //如果是线条元素，拿到绑定的 Note 对象，调用 ViewModel 的方法
                    if (ViewModel != null && lineElement != null && lineElement.Note != null)
                    {
                        //调用 ViewModel 中的点击事件程序
                        await ViewModel.OnNoteTappedAsync(lineElement.Note);
                    }
                }
            }
        }

    }
}
