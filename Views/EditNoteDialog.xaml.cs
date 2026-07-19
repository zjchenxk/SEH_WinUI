using Microsoft.UI.Dispatching;
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

            //确保在 ViewModel 数据加载完成后初始化选中项
            this.Loaded += (s, e) =>
            {
                if (ViewModel.StartSlurs != null && ViewModel.EndSlurs != null)
                {
                    InitializeSelections();
                }
            };
        }

        private void InitializeSelections()
        {
            //获取当前线程的 DispatcherQueue
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            //如果不在 UI 线程，可能为 null，需处理
            if (dispatcherQueue == null) return;

            dispatcherQueue.TryEnqueue(() =>
            {
                // 检查控件是否为 null
                if (StartSlursListView == null || EndSlursListView == null)
                {
                    return;
                }

                //初始化开始连音线的选中项
                if (ViewModel.SelectedStartSlurs != null && ViewModel.SelectedStartSlurs.Count > 0)
                {
                    //暂时取消绑定事件，防止 Add 操作触发 SelectionChanged 导致集合修改异常
                    StartSlursListView.SelectionChanged -= StartSlursListView_SelectionChanged;

                    StartSlursListView.SelectedItems.Clear();

                    foreach (var slur in ViewModel.SelectedStartSlurs)
                    {
                        //确保 ViewModel 中的项存在于 ListView 的 ItemsSource 中
                        if (ViewModel.StartSlurs.Contains(slur))
                        {
                            StartSlursListView.SelectedItems.Add(slur);
                        }
                    }

                    // 恢复绑定事件
                    StartSlursListView.SelectionChanged += StartSlursListView_SelectionChanged;
                }

                //初始化结束连音线的选中项
                if (ViewModel.SelectedEndSlurs != null && ViewModel.SelectedEndSlurs.Count > 0)
                {
                    //暂时取消绑定事件
                    EndSlursListView.SelectionChanged -= EndSlursListView_SelectionChanged;

                    EndSlursListView.SelectedItems.Clear();

                    foreach (var slur in ViewModel.SelectedEndSlurs)
                    {
                        if (ViewModel.EndSlurs.Contains(slur))
                        {
                            EndSlursListView.SelectedItems.Add(slur);
                        }
                    }

                    //恢复绑定事件
                    EndSlursListView.SelectionChanged += EndSlursListView_SelectionChanged;
                }
            });
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
                    ViewModel.SelectedStartSlurs.Clear();

                    foreach (var item in selectedItems)
                    {
                        ViewModel.SelectedStartSlurs.Add((Slur)item);
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
                    ViewModel.SelectedEndSlurs.Clear();

                    foreach (var item in selectedItems)
                    {
                        ViewModel.SelectedEndSlurs.Add((Slur)item);
                    }
                }
            }
        }
    }
}
