using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
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
    }
}
