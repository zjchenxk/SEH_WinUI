using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.ViewModels;
using SEH.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEH.Services
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// 需要一个属性来接收当前的 XamlRoot (通常在页面加载时由 View 层传入)
        /// </summary>
        public XamlRoot? XamlRoot { get; set; }

        /// <summary>
        /// 显示音符编辑对话框
        /// </summary>
        /// <param name="beams"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public async Task<Note>? ShowEditNoteDialogAsync(List<Beam>? beams = null, Note? note = null)
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var viewModel = new EditNoteViewModel();

            //实现传参初始化
            viewModel.Initialize(beams, note);

            var dialog = new EditNoteDialog(viewModel)
            {
                XamlRoot = this.XamlRoot, //关键：设置 XamlRoot
                Title = note == null ? "新增音符" : "修改音符" //根据传参动态修改标题
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //这里简单处理，实际应提示错误并保持弹窗
                    return null;
                }
                return viewModel.GetNote();
            }

            return null;
        }

        /// <summary>
        /// 显示小节编辑对话框
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Measure>? ShowEditMeasureDialogAsync(Measure? measure = null)
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var viewModel = new EditMeasureViewModel();

            //实现传参初始化
            viewModel.Initialize(measure);

            var dialog = new EditMeasureDialog(viewModel)
            {
                XamlRoot = this.XamlRoot, //关键：设置 XamlRoot
                Title = measure == null ? "新增小节" : "修改小节" //根据传参动态修改标题
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //这里简单处理，实际应提示错误并保持弹窗
                    return null;
                }
                return viewModel.GetMeasure();
            }

            return null;
        }
    }
}
