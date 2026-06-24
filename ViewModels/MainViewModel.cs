using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SEH.Commons;
using SEH.Services.Interfaces;
using SEH.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SEH.Commons.ScoreItem;

namespace SEH.ViewModels
{
    /// <summary>
    /// 主窗体的视图模型，负责管理界面数据和逻辑
    /// </summary>
    public partial class MainViewModel : ObservableObject
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
        /// 简谱列表
        /// [ObservableProperty] 特性会自动生成一个名为 ScoreItems 的公共属性和对应的属性变更通知代码，只需要维护私有字段就够了
        /// </summary>
        [ObservableProperty]
        private List<ScoreItem>? _scoreItems = null;

        /// <summary>
        /// 选中简谱列表项
        /// </summary>
        [ObservableProperty]
        private ScoreItem? _selectedScoreItem = null;

        /// <summary>
        /// 构造函数，初始化 MainViewModel 实例
        /// </summary>
        /// <param name="messenger"></param>
        /// <param name="navigationService"></param>
        /// <param name="dataService"></param>
        public MainViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;

            //注册消息接收器，当收到 RefreshScoreListMessage 消息时，调用 LoadScoreItems 方法刷新简谱列表
            _messenger.Register<MainViewModel, RefreshScoreListMessage>(this, (r, m) =>
            {
                LoadScoreItems();
            });

            //初始化时加载简谱列表
            LoadScoreItems();
        }

        /// <summary>
        /// 读取简谱列表
        /// </summary>
        private void LoadScoreItems()
        {
            List<ScoreItem> scoreItems = [];

            var categories = _dataService.GetCategories();
            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    var scoreItem = new ScoreItem
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Type = ScoreItem.ScoreItemType.Folder,
                    };

                    var scores = _dataService.GetScoresByCategoryId(category.Id);
                    if (scores != null && scores.Count > 0)
                    {
                        foreach (var score in scores)
                        {
                            scoreItem.Children.Add(new ScoreItem
                            {
                                Id = score.Id,
                                Name = score.Title,
                                Type = ScoreItem.ScoreItemType.File,
                            });
                        }
                    }

                    scoreItems.Add(scoreItem);
                }
            }

            ScoreItems = scoreItems;
        }

        /// <summary>
        /// 新增类别命令
        /// [RelayCommand] 特性会自动生成一个名为 NewCategoryCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private void NewCategory()
        {
            _navigationService.NavigateTo(typeof(EditCategoryPage));
        }

        /// <summary>
        /// 修改类别命令
        /// [RelayCommand] 特性会自动生成一个名为 EditCategoryCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private async Task EditCategory()
        {
            var _messageService = App.Services.GetRequiredService<IMessageService>();

            if (SelectedScoreItem == null)
            {
                await _messageService.ShowInfoAsync("未选择类别！");
                return;
            }
            if (SelectedScoreItem.Type == ScoreItemType.File)
            {
                await _messageService.ShowInfoAsync("未选择类别！");
                return;
            }

            string categoryId = SelectedScoreItem.Id;
            string categoryName = SelectedScoreItem.Name;

            _navigationService.NavigateTo(typeof(EditCategoryPage), categoryId);
        }

        /// <summary>
        /// 删除类别命令
        /// [RelayCommand] 特性会自动生成一个名为 DeleteCategoryCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private async Task DeleteCategory()
        {
            var _messageService = App.Services.GetRequiredService<IMessageService>();

            if (SelectedScoreItem == null)
            {
                await _messageService.ShowInfoAsync("未选择类别！");
                return;
            }
            if (SelectedScoreItem.Type == ScoreItemType.File)
            {
                await _messageService.ShowInfoAsync("未选择类别！");
                return;
            }

            string categoryId = SelectedScoreItem.Id;
            string categoryName = SelectedScoreItem.Name;

            //检查类别下是否有简谱
            var scores = _dataService.GetScoresByCategoryId(categoryId);
            if (scores != null && scores.Count > 0)
            {
                await _messageService.ShowErrorAsync("该类别下面存在简谱，不能删除！");
                return;
            }

            if (await _messageService.ShowConfirmAsync($"确定要删除类别【{categoryName}】吗？") != true)
            {
                return;
            }

            if (!_dataService.DeleteCategory(categoryId))
            {
                await _messageService.ShowErrorAsync("删除类别失败！");
                return;
            }

            LoadScoreItems();
        }

        /// <summary>
        /// 新增简谱命令
        /// [RelayCommand] 特性会自动生成一个名为 NewScoreCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private void NewScore()
        {
            _navigationService.NavigateTo(typeof(EditScorePage));
        }
    }
}
