using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SEH.Services.Interfaces;
using SEH.Views;
using System.Threading.Tasks;

namespace SEH.ViewModels
{
    public partial class EditCategoryViewModel : ObservableObject
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
        [ObservableProperty]
        private string _categoryName = "";


        public EditCategoryViewModel(INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;
        }

        /// <summary>
        /// 保存命令
        /// [RelayCommand] 特性会自动生成一个名为 SaveCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private async Task Save()
        {
            //1.验证逻辑：检查是否为空
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                //2.调用服务弹出提示框
                await _messageService.ShowErrorAsync("错误", "类别名称不能为空，请重新输入！");
                return;
            }

            // 3. 验证通过，执行后续业务逻辑...
            await _messageService.ShowErrorAsync("消息", "验证通过！");
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
