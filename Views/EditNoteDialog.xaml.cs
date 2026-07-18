using Microsoft.UI.Xaml.Controls;
using SEH.Models;
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

        private void StartSlursListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                //获取当前选中的项
                var selectedItems = listView.SelectedItems;
                if (selectedItems != null && selectedItems.Count > 0)
                {
                    //清空并更新 ViewModel 中的选中集合
                    ViewModel.SelectedStartSlurs ??= [];
                    ViewModel.SelectedStartSlurs?.Clear();

                    foreach (var item in selectedItems)
                    {
                        ViewModel.SelectedStartSlurs?.Add((Slur)item);
                    }
                }
            }
        }

        private void EndSlursListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                //获取当前选中的项
                var selectedItems = listView.SelectedItems;
                if (selectedItems != null && selectedItems.Count > 0)
                {
                    //清空并更新 ViewModel 中的选中集合
                    ViewModel.SelectedEndSlurs ??= [];
                    ViewModel.SelectedEndSlurs?.Clear();

                    foreach (var item in selectedItems)
                    {
                        ViewModel.SelectedEndSlurs?.Add((Slur)item);
                    }
                }
            }
        }
    }
}
