using Microsoft.UI.Xaml.Controls;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 编辑小节对话框
    /// </summary>
    public sealed partial class EditMeasureDialog : ContentDialog
    {
        public EditMeasureViewModel ViewModel { get; }


        public EditMeasureDialog(EditMeasureViewModel viewModel)
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = viewModel;

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.DataContext = ViewModel;
        }
    }
}
