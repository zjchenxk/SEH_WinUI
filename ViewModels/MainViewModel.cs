using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SEH.Commons;
using SEH.Services.Interfaces;
using SEH.Views;
using System.Collections.Generic;

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
        /// 新增类别命令
        /// [RelayCommand] 特性会自动生成一个名为 NewCategoryCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private void NewCategory()
        {
            _navigationService.NavigateTo(typeof(EditCategoryPage));
        }

        public MainViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;

            _scoreItems = [];

            _messenger.Register<MainViewModel, RefreshScoreListMessage>(this, (r, m) =>
            {
                LoadScoreItems();
            });
        }

        /// <summary>
        /// 读取简谱列表
        /// </summary>
        public void LoadScoreItems()
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

    }
}
