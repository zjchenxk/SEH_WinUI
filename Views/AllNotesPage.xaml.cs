using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SEH.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEH.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllNotesPage : Page
    {
        private AllFile model = new AllFile();

        public AllNotesPage()
        {
            InitializeComponent();

            // 设置 CommandBar 中 AppBarButton 的字体大小
            SetAppBarButtonLabelFontSize();
        }

        private void SetAppBarButtonLabelFontSize()
        {
            // 通过加载事件来访问内部的 TextBlock
            MainCommandBar.Loaded += (s, e) =>
            {
                foreach (var item in MainCommandBar.PrimaryCommands.OfType<ICommandBarElement>())
                {
                    if (item is AppBarButton button)
                    {
                        button.Loaded += (btn, ev) =>
                        {
                            // 查找所有 TextBlock 后代
                            var textBlocks = FindChildren<TextBlock>(button);
                            foreach (var tb in textBlocks)
                            {
                                // 检查是否是标签（通常包含 Label 文本）
                                if (tb.Text == button.Label)
                                {
                                    tb.FontSize = 22;
                                }
                            }
                        };
                    }
                }
            };
        }

        // 递归查找所有子元素
        private static IEnumerable<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                {
                    yield return result;
                }

                foreach (var descendant in FindChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotePage));
        }

        private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
        {
            Frame.Navigate(typeof(NotePage), args.InvokedItem);
        }
    }
}
