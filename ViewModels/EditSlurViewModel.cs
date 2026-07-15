using CommunityToolkit.Mvvm.ComponentModel;
using SEH.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEH.ViewModels
{
    public partial class EditSlurViewModel : ObservableValidator
    {
        /// <summary>
        /// 连音线ID
        /// </summary>
        [ObservableProperty]
        private string _id = Guid.NewGuid().ToString();

        /// <summary>
        /// 连音线名称
        /// </summary>
        [Required(ErrorMessage = "连音线名称不能为空！")]
        [MaxLength(30, ErrorMessage = "连音线名称长度不能超过30个字！")]
        [ObservableProperty]
        private string _name = "4";

        /// <summary>
        /// 连音线名称错误信息
        /// </summary>
        [ObservableProperty]
        private string? _nameError = "";


        public EditSlurViewModel()
        {
            this.ErrorsChanged += EditSlurViewModel_ErrorsChanged;
        }

        private void EditSlurViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name))//当 Name 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Name));
                NameError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        partial void OnNameChanged(string value)
        {
            ValidateProperty(value, nameof(Name));
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="slur"></param>
        public void Initialize(Slur? slur = null)
        {
            if (slur != null)
            {
                Id = slur.Id;
                Name = slur.Name;
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

        public Slur GetSlur()
        {
            return new Slur
            {
                Id = this.Id,
                Name = this.Name,
            };
        }

    }
}
