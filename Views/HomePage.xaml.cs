using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 首页
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //在页面导航到时，调用 MainViewModel 的 ResetBreadcrumbItems 方法重置面包屑导航项为首页
            var viewModel = App.Services?.GetRequiredService<MainViewModel>();
            viewModel?.ResetBreadcrumbItems();
        }
    }
}
