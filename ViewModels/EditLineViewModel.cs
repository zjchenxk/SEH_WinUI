using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEH.ViewModels
{
    public partial class EditLineViewModel : ObservableValidator
    {
        /// <summary>
        /// 行ID
        /// </summary>
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// 小节数（默认为4）
        /// </summary>
        [Required(ErrorMessage = "每行小节数量不能为空！")]
        [ObservableProperty]
        private string _measureCount = "4";

        /// <summary>
        /// 小节数错误信息
        /// </summary>
        [ObservableProperty]
        private string? _measureCountError = "";


        public EditLineViewModel()
        {
            this.ErrorsChanged += EditLineViewModel_ErrorsChanged;
        }

        private void EditLineViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MeasureCount))//当 MeasureCount 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(MeasureCount));
                MeasureCountError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        partial void OnMeasureCountChanged(string value)
        {
            ValidateProperty(value, nameof(MeasureCount));
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="line"></param>
        public void Initialize(Line? line = null)
        {
            if (line != null)
            {
                Id = line.Id;
                MeasureCount = line.MeasureCount.ToString();
            }
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        public bool ValidateProperties()
        {
            ValidateAllProperties();

            return !HasErrors;
        }

        public Line GetLine()
        {
            return new Line
            {
                Id = this.Id,
                MeasureCount = int.Parse(this.MeasureCount),
            };
        }

    }
}
