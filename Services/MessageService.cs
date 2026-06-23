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
        /// 显示错误信息
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
    }
}
