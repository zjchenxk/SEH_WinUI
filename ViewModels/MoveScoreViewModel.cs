using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEH.ViewModels
{
    public partial class MoveScoreViewModel : ObservableValidator
    {
        /// <summary>
        /// 消息服务，用于在不同的ViewModel之间传递消息
        /// </summary>
        private readonly IMessenger _messenger;

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
        /// 简谱ID
        /// </summary>
        [ObservableProperty]
        private string _id = "";

        /// <summary>
        /// 类别集合
        /// </summary>
        [ObservableProperty]
        private List<Category>? _categories = null;

        /// <summary>
        /// 当前选中类别
        /// </summary>
        [ObservableProperty]
        private Category? _selectedCategory = null;


        public MoveScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;

            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories = _dataService.GetCategories();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        public void Initialize(string id)
        {
            Id = id;
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        [RelayCommand]
        private async Task Ok()
        {
            //触发属性验证
            ValidateAllProperties();

            //检查是否有错误
            if (HasErrors)
            {
                //错误会自动通知 UI，不需要弹窗
                return;
            }

            if (SelectedCategory == null)
            {
                await _messageService.ShowErrorAsync("未选择类别！");
                return;
            }

            if (string.IsNullOrWhiteSpace(Id))
            {
                await _messageService.ShowErrorAsync("简谱ID不能为空！");
                return;
            }

            if (!_dataService.UpdateScoreCategory(Id, SelectedCategory.Id))
            {
                await _messageService.ShowErrorAsync("移动简谱失败！");
                return;
            }

            //保存成功后，导航回主页
            _navigationService.NavigateTo(typeof(HomePage));

            //保存成功后，通知主窗体更新简谱树形列表
            _messenger.Send(new RefreshScoreListMessage());
        }

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            _navigationService.NavigateTo(typeof(HomePage));
        }

    }
}
