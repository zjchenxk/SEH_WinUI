using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 新增类别页面
    /// </summary>
    public sealed partial class EditCategoryPage : Page
    {
        public EditCategoryViewModel ViewModel { get; }


        public EditCategoryPage()
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = App.Services.GetRequiredService<EditCategoryViewModel>();

            //初始化组件（XAML UI 元素）
            InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                // 将参数传给 ViewModel 进行初始化
                //ViewModel.Initialize(e.Parameter);
            }
        }

    }
}
