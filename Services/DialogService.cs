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
        /// 显示行编辑对话框
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public async Task<Line?> ShowEditLineDialogAsync(Line? line = null)
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var viewModel = new EditLineViewModel();

            //实现传参初始化
            viewModel.Initialize(line);

            var dialog = new EditLineDialog(viewModel)
            {
                XamlRoot = this.XamlRoot, //关键：设置 XamlRoot
                Title = line == null ? "新增行" : "修改行" //根据传参动态修改标题
            };

            dialog.PrimaryButtonClick += (sender, e) =>
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //阻止对话框关闭
                    e.Cancel = true;
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return viewModel.GetLine();
            }

            return null;
        }

        /// <summary>
        /// 显示小节编辑对话框
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        public async Task<Measure?> ShowEditMeasureDialogAsync(Measure? measure = null)
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

            dialog.PrimaryButtonClick += (sender, e) =>
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //阻止对话框关闭
                    e.Cancel = true;
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return viewModel.GetMeasure();
            }

            return null;
        }

        /// <summary>
        /// 显示音符编辑对话框
        /// </summary>
        /// <param name="beams"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public async Task<Note?> ShowEditNoteDialogAsync(List<Beam>? beams = null, Note? note = null)
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

            dialog.PrimaryButtonClick += (sender, e) =>
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //阻止对话框关闭
                    e.Cancel = true;
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return viewModel.GetNote();
            }

            return null;
        }

        /// <summary>
        /// 显示减时组合编辑对话框
        /// </summary>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Beam?> ShowEditBeamDialogAsync(Beam? beam = null)
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var viewModel = new EditBeamViewModel();

            //实现传参初始化
            viewModel.Initialize(beam);

            var dialog = new EditBeamDialog(viewModel)
            {
                XamlRoot = this.XamlRoot, //关键：设置 XamlRoot
                Title = beam == null ? "新增组合" : "修改组合" //根据传参动态修改标题
            };

            dialog.PrimaryButtonClick += (sender, e) =>
            {
                //验证数据
                bool isValid = viewModel.ValidateProperties();
                if (!isValid)
                {
                    //阻止对话框关闭
                    e.Cancel = true;
                }
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return viewModel.GetBeam();
            }

            return null;
        }

    }
}
