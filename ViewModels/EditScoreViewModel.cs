using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [CustomValidation(typeof(EditScoreViewModel), nameof(ValidateTempo))]
        [ObservableProperty]
        private string _tempo = "";

        /// <summary>
        /// 拍号错误信息
        /// </summary>
        [ObservableProperty]
        private string? _tempoError = "";

        /// <summary>
        /// 简谱对象，用于保存当前编辑的简谱数据
        /// </summary>
        private Score? _score = new();


        public EditScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;

            this.ErrorsChanged += EditScoreViewModel_ErrorsChanged;
        }

        private void EditScoreViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Title))//当 Title 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Title));
                TitleError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Composer))//当 Composer 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Composer));
                ComposerError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Lyricist))//当 Lyricist 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Lyricist));
                LyricistError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(KeySignature))//当 KeySignature 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(KeySignature));
                KeySignatureError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(TimeSignature))//当 TimeSignature 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(TimeSignature));
                TimeSignatureError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Tempo))//当 Tempo 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Tempo));
                TempoError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        public void Initialize(JObject param)
        {
            if (param != null)
            {
                if (param["CategoryId"] != null)
                {
                    CategoryId = param["CategoryId"].ToString();
                }
                else if (param["Id"] != null)
                {
                    var score = _dataService.GetScore(param["Id"].ToString());
                    if (score == null)
                    {
                        _messageService.ShowErrorAsync("未找到指定的简谱数据！").Wait();
                        _navigationService.NavigateTo(typeof(HomePage));
                        return;
                    }

                    CategoryId = score.CategoryId;
                    Title = score.Title;
                    Composer = score.Composer ?? "";
                    Lyricist = score.Lyricist ?? "";
                    KeySignature = score.KeySignature;
                    TimeSignature = score.TimeSignature;
                    Tempo = score.Tempo.ToString();

                    _score = score;
                }
            }
        }

        partial void OnTitleChanged(string value)
        {
            ValidateProperty(value, nameof(Title));
        }

        partial void OnComposerChanged(string value)
        {
            ValidateProperty(value, nameof(Composer));
        }

        partial void OnLyricistChanged(string value)
        {
            ValidateProperty(value, nameof(Lyricist));
        }

        partial void OnKeySignatureChanged(string value)
        {
            ValidateProperty(value, nameof(KeySignature));
        }

        partial void OnTimeSignatureChanged(string value)
        {
            ValidateProperty(value, nameof(TimeSignature));
        }

        partial void OnTempoChanged(string value)
        {
            ValidateProperty(value, nameof(Tempo));
        }

        /// <summary>
        /// 验证速度是否为合法数字
        /// </summary>
        /// <param name="tempo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult? ValidateTempo(string tempo, ValidationContext context)
        {
            //1.检查是否为空
            if (string.IsNullOrWhiteSpace(tempo))
            {
                return new("速度不能为空！");
            }

            //2.检查是否为数字
            if (!int.TryParse(tempo, out int result))
            {
                return new("请输入有效的数字！");
            }

            //3.检查数值范围 (例如：速度必须在 20-300 之间)
            if (result < 30 || result > 300)
            {
                return new("速度必须在 30 到 300 之间！");
            }

            //4.验证通过
            return ValidationResult.Success;
        }

        /// <summary>
        /// 保存简谱命令
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

            if (string.IsNullOrWhiteSpace(CategoryId))
            {
                await _messageService.ShowErrorAsync("类别Id不能为空！");
                return;
            }

            //保存简谱数据
            _score.CategoryId = CategoryId;
            _score.Title = Title.Trim();
            _score.Composer = Composer.Trim();
            _score.Lyricist = Lyricist.Trim();
            _score.KeySignature = KeySignature.Trim();
            _score.TimeSignature = TimeSignature.Trim();
            _score.Tempo = int.Parse(Tempo.Trim());

            if (string.IsNullOrWhiteSpace(_score.Id))
            {
                _score.Id = Guid.NewGuid().ToString();

                if (!_dataService.AddScore(_score))
                {
                    _score.Id = "";
                    await _messageService.ShowErrorAsync("保存简谱失败！");
                    return;
                }
            }
            else
            {
                if (!_dataService.UpdateScore(_score))
                {
                    await _messageService.ShowErrorAsync("保存简谱失败！");
                    return;
                }
            }

            await _messageService.ShowInfoAsync("保存简谱成功！");

            //保存成功后，通知主窗体更新简谱树形列表
            _messenger.Send(new RefreshScoreListMessage());
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
