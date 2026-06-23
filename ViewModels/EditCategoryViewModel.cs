using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SEH.Services.Interfaces;
using SEH.Views;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SEH.ViewModels
{
    public partial class EditCategoryViewModel : ObservableValidator
    {
        /// <summary>
        /// 导航服务，用于在不同页面之间进行导航
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// 数据服务，用于访问和操作简谱数据
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// 消息服务，用于显示消息提示
        /// </summary>
        private readonly IMessageService _messageService;

        /// <summary>
        /// 类别Id
        /// [ObservableProperty] 特性会自动生成一个名为 CategoryId 的公共属性和对应的属性变更通知代码，只需要维护私有字段就够了
        /// </summary>
        [ObservableProperty]
        private string _categoryId = "";

        /// <summary>
        /// 类别名称
        /// [ObservableProperty] 特性会自动生成一个名为 CategoryName 的公共属性和对应的属性变更通知代码，只需要维护私有字段就够了
        /// </summary>
        [Required(ErrorMessage = "类别名称不能为空！")]
        [MaxLength(50, ErrorMessage = "类别名称长度不能超过50个字！")]
        [ObservableProperty]
        private string _categoryName = "";

        /// <summary>
        /// 类别名称改变事件
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        partial void OnCategoryNameChanged(string value)
        {
            //对当前修改的属性单独进行验证
            ValidateProperty(value, nameof(CategoryName));
        }

        /// <summary>
        /// 类别名称错误信息，用于在界面上显示
        /// </summary>
        [ObservableProperty]
        private string? _categoryNameError = "";


        public EditCategoryViewModel(INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;

            this.ErrorsChanged += EditCategoryViewModel_ErrorsChanged;
        }

        private void EditCategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            //当 CategoryName 属性的验证状态发生变化时，更新错误提示文本
            if (e.PropertyName == nameof(CategoryName))
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(CategoryName));
                CategoryNameError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        /// <summary>
        /// 保存命令
        /// [RelayCommand] 特性会自动生成一个名为 SaveCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private async Task Save()
        {
            //触发属性验证
            ValidateAllProperties();

            //检查是否有错误
            if (HasErrors)
            {
                //错误会自动通知 UI，不需要弹窗
                return;
            }

            await _messageService.ShowErrorAsync(CategoryName);
        }

        /// <summary>
        /// 取消命令
        /// [RelayCommand] 特性会自动生成一个名为 CancelCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            _navigationService.NavigateTo(typeof(HomePage));
        }

    }
}
