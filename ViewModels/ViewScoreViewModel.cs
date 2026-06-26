using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SEH.Services.Interfaces;

namespace SEH.ViewModels
{
    public partial class ViewScoreViewModel : ObservableValidator
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


        public ViewScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        public void Initialize(string id)
        {
            Id = id;
        }

    }
}
