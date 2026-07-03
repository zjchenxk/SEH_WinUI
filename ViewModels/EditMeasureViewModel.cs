using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEH.ViewModels
{
    public partial class EditMeasureViewModel : ObservableValidator
    {
        /// <summary>
        /// 小节ID
        /// </summary>
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// 左小节线类型
        /// </summary>
        [Required(ErrorMessage = "左小节线类型不能为空！")]
        [ObservableProperty]
        private string _leftLine = "0"; // 默认无

        /// <summary>
        /// 左小节线类型错误信息
        /// </summary>
        [ObservableProperty]
        private string? _leftLineError = "";

        /// <summary>
        /// 右小节线类型
        /// </summary>
        [Required(ErrorMessage = "右小节线类型不能为空！")]
        [ObservableProperty]
        private string _rightLine = "1"; // 默认小节线

        /// <summary>
        /// 右小节线类型错误信息
        /// </summary>
        [ObservableProperty]
        private string? _rightLineError = "";


        public EditMeasureViewModel()
        {
            this.ErrorsChanged += EditMeasureViewModel_ErrorsChanged;
        }

        private void EditMeasureViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LeftLine))//当 LeftLine 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(LeftLine));
                LeftLineError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(RightLine))//当 RightLine 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(RightLine));
                RightLineError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        partial void OnLeftLineChanged(string value)
        {
            ValidateProperty(value, nameof(LeftLine));
        }

        partial void OnRightLineChanged(string value)
        {
            ValidateProperty(value, nameof(RightLine));
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="measure"></param>
        public void Initialize(Measure? measure = null)
        {
            if (measure != null)
            {
                Id = measure.Id;
                LeftLine = measure.LeftLine.ToString();
                RightLine = measure.RightLine.ToString();
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

        public Measure GetMeasure()
        {
            return new Measure
            {
                Id = this.Id,
                LeftLine = int.Parse(this.LeftLine),
                RightLine = int.Parse(this.RightLine)
            };
        }

    }
}
