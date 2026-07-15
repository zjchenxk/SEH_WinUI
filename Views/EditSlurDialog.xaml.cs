using Microsoft.UI.Xaml.Controls;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 连音线编辑对话框
    /// </summary>
    public sealed partial class EditSlurDialog : ContentDialog
    {
        public EditSlurViewModel ViewModel { get; }


        public EditSlurDialog(EditSlurViewModel viewModel)
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = viewModel;

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.DataContext = ViewModel;
        }
    }
}
