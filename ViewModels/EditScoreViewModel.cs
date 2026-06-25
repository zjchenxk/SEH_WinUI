using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using SEH.Services.Interfaces;
using SEH.Views;
using System.ComponentModel.DataAnnotations;

namespace SEH.ViewModels
{
    public partial class EditScoreViewModel : ObservableValidator
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
        /// 类别Id
        /// </summary>
        [ObservableProperty]
        private string _categoryId = "";

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空！")]
        [MaxLength(100, ErrorMessage = "标题长度不能超过100个字！")]
        [ObservableProperty]
        private string _title = "";

        /// <summary>
        /// 标题错误信息
        /// </summary>
        [ObservableProperty]
        private string? _titleError = "";

        /// <summary>
        /// 作曲人
        /// </summary>
        [MaxLength(50, ErrorMessage = "作曲人长度不能超过50个字！")]
        [ObservableProperty]
        private string _composer = "";

        /// <summary>
        /// 作曲人错误信息
        /// </summary>
        [ObservableProperty]
        private string? _composerError = "";

        /// <summary>
        /// 作词人
        /// </summary>
        [MaxLength(50, ErrorMessage = "作词人长度不能超过50个字！")]
        [ObservableProperty]
        private string _lyricist = "";

        /// <summary>
        /// 作词人错误信息
        /// </summary>
        [ObservableProperty]
        private string? _lyricistError = "";

        /// <summary>
        /// 调号（如：C、D等）
        /// </summary>
        [Required(ErrorMessage = "调号不能为空！")]
        [MaxLength(1, ErrorMessage = "调号长度不能超过1个字！")]
        [ObservableProperty]
        private string _keySignature = "";

        /// <summary>
        /// 调号错误信息
        /// </summary>
        [ObservableProperty]
        private string? _keySignatureError = "";

        /// <summary>
        /// 拍号，如：3/4、4/4等。下面的数字表示以何种时值的音符为一拍，上面的数字表示每小节有几拍。
        /// </summary>
        [Required(ErrorMessage = "拍号不能为空！")]
        [MaxLength(5, ErrorMessage = "拍号长度不能超过5个字！")]
        [ObservableProperty]
        private string _timeSignature = "";

        /// <summary>
        /// 拍号错误信息
        /// </summary>
        [ObservableProperty]
        private string? _timeSignatureError = "";

        /// <summary>
        /// 速度（如：80、120）
        /// </summary>
        [Required(ErrorMessage = "速度不能为空！")]
        [ObservableProperty]
        private string _tempo = "";

        /// <summary>
        /// 拍号错误信息
        /// </summary>
        [ObservableProperty]
        private string? _tempoError = "";


        public EditScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;
        }

        public void Initialize(JObject param)
        {
            if (param != null)
            {
                if (param["CategoryId"] != null)
                {
                    CategoryId = param["CategoryId"].ToString();
                }
            }
        }

        /// <summary>
        /// 保存简谱命令
        /// </summary>
        [RelayCommand]
        private void Save()
        {
        }

        /// <summary>
        /// 关闭退出命令
        /// </summary>
        [RelayCommand]
        private void Close()
        {
            _navigationService.NavigateTo(typeof(HomePage));
        }

    }
}
