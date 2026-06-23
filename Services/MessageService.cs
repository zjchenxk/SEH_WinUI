using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        public async Task ShowErrorAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
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
