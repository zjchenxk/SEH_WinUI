using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.ViewModels;
using SEH.Views;
using System;
using System.Threading.Tasks;

namespace SEH.Services
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// 需要一个属性来接收当前的 XamlRoot (通常在页面加载时由 View 层传入)
        /// </summary>
        public XamlRoot? XamlRoot { get; set; }

        public async Task<Note>? ShowEditNoteDialogAsync()
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var viewModel = new EditNoteViewModel();
            var dialog = new EditNoteDialog(viewModel)
            {
                XamlRoot = this.XamlRoot // 关键：设置 XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //验证数据
                viewModel.ValidateAllProperties();
                if (viewModel.HasErrors)
                {
                    //这里简单处理，实际应提示错误并保持弹窗
                    return null;
                }
                return viewModel.GetNote();
            }

            return null;
        }
    }
}
