using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEH.ViewModels
{
    public partial class EditBeamViewModel : ObservableValidator
    {
        /// <summary>
        /// 组合ID
        /// </summary>
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// 组合时值，如：0.5-八分音符，0.25-十六分音符，0.125-三十二分音符，默认0.5
        /// </summary>
        [Required(ErrorMessage = "组合时值不能为空！")]
        [ObservableProperty]
        private string _duration = "0.5";

        /// <summary>
        /// 组合时值错误信息
        /// </summary>
        [ObservableProperty]
        private string? _durationError = "";


        public EditBeamViewModel()
        {
            this.ErrorsChanged += EditBeamViewModel_ErrorsChanged;
        }

        private void EditBeamViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Duration))//当 Duration 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Duration));
                DurationError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        partial void OnDurationChanged(string value)
        {
            ValidateProperty(value, nameof(Duration));
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="beam"></param>
        public void Initialize(Beam? beam = null)
        {
            if (beam != null)
            {
                Id = beam.Id;
                Duration = beam.Duration.ToString();
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

        public Beam GetBeam()
        {
            return new Beam
            {
                Id = this.Id,
                Duration = float.Parse(this.Duration)
            };
        }
    }
}
