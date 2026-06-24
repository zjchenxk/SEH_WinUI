using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SEH.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SEH.Services
{
    public class MessageService : IMessageService
    {
        private readonly Window _mainWindow;

        public MessageService(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 显示错误信息对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ShowErrorAsync(string message)
        {
            var titlePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8 // 图标和文字之间的间距
            };

            titlePanel.Children.Add(new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/错误.png")),
                MaxHeight = 32, // 限制图片高度，防止图片过大撑爆弹窗
                MaxWidth = 32,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            titlePanel.Children.Add(new TextBlock
            {
                Text = "错误",
                VerticalAlignment = VerticalAlignment.Center
            });

            var dialog = new ContentDialog
            {
                Title = titlePanel,
                Content = message,
                CloseButtonText = "确定"
            };

            // 关键：从主窗口的内容获取 XamlRoot
            if (_mainWindow.Content is FrameworkElement rootElement)
            {
                dialog.XamlRoot = rootElement.XamlRoot;
            }

            await dialog.ShowAsync();
        }

        /// <summary>
        /// 显示提示信息对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ShowInfoAsync(string message)
        {
            var titlePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8 // 图标和文字之间的间距
            };

            titlePanel.Children.Add(new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/提示.png")),
                MaxHeight = 32, // 限制图片高度，防止图片过大撑爆弹窗
                MaxWidth = 32,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            titlePanel.Children.Add(new TextBlock
            {
                Text = "提示",
                VerticalAlignment = VerticalAlignment.Center
            });

            var dialog = new ContentDialog
            {
                Title = titlePanel,
                Content = message,
                CloseButtonText = "确定"
            };

            // 关键：从主窗口的内容获取 XamlRoot
            if (_mainWindow.Content is FrameworkElement rootElement)
            {
                dialog.XamlRoot = rootElement.XamlRoot;
            }

            await dialog.ShowAsync();
        }

        /// <summary>
        /// 显示警告信息对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ShowWarningAsync(string message)
        {
            var titlePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8 // 图标和文字之间的间距
            };

            titlePanel.Children.Add(new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/警告.png")),
                MaxHeight = 32, // 限制图片高度，防止图片过大撑爆弹窗
                MaxWidth = 32,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            titlePanel.Children.Add(new TextBlock
            {
                Text = "警告",
                VerticalAlignment = VerticalAlignment.Center
            });

            var dialog = new ContentDialog
            {
                Title = titlePanel,
                Content = message,
                CloseButtonText = "确定"
            };

            // 关键：从主窗口的内容获取 XamlRoot
            if (_mainWindow.Content is FrameworkElement rootElement)
            {
                dialog.XamlRoot = rootElement.XamlRoot;
            }

            await dialog.ShowAsync();
        }

        /// <summary>
        /// 显示确认信息对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> ShowConfirmAsync(string message)
        {
            var titlePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8 // 图标和文字之间的间距
            };

            titlePanel.Children.Add(new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/问题.png")),
                MaxHeight = 32, // 限制图片高度，防止图片过大撑爆弹窗
                MaxWidth = 32,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            titlePanel.Children.Add(new TextBlock
            {
                Text = "确认",
                VerticalAlignment = VerticalAlignment.Center
            });

            var dialog = new ContentDialog
            {
                Title = titlePanel,
                Content = message,
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };

            // 关键：从主窗口的内容获取 XamlRoot
            if (_mainWindow.Content is FrameworkElement rootElement)
            {
                dialog.XamlRoot = rootElement.XamlRoot;
            }

            //显示弹窗并等待用户点击
            ContentDialogResult result = await dialog.ShowAsync();

            //如果用户点击了“确定”，返回 true，否则返回 false
            return result == ContentDialogResult.Primary;
        }
    }
}
