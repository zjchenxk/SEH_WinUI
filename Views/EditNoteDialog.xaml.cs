using Microsoft.UI.Xaml.Controls;
using SEH.ViewModels;

namespace SEH.Views
{
    /// <summary>
    /// 音符录入对话框
    /// </summary>
    public sealed partial class EditNoteDialog : ContentDialog
    {
        public EditNoteViewModel ViewModel { get; }


        public EditNoteDialog(EditNoteViewModel viewModel)
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = viewModel;

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.DataContext = ViewModel;
        }

    }
}
