using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json.Linq;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Text;

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
        /// 对话框服务，用于弹出对话框
        /// </summary>
        private readonly IDialogService _dialogService;


        /// <summary>
        /// 类别Id
        /// </summary>
        [ObservableProperty]
        private string _categoryId = "";

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空！")]
        [MaxLength(30, ErrorMessage = "标题长度不能超过30个字！")]
        [ObservableProperty]
        private string _title = "";

        /// <summary>
        /// 标题错误信息
        /// </summary>
        [ObservableProperty]
        private string? _titleError = "";

        /// <summary>
        /// 副标题
        /// </summary>
        [MaxLength(30, ErrorMessage = "副标题长度不能超过30个字！")]
        [ObservableProperty]
        private string _subTitle = "";

        /// <summary>
        /// 副标题错误信息
        /// </summary>
        [ObservableProperty]
        private string? _subTitleError = "";

        /// <summary>
        /// 作曲人
        /// </summary>
        [MaxLength(20, ErrorMessage = "作曲人长度不能超过20个字！")]
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
        [MaxLength(20, ErrorMessage = "作词人长度不能超过20个字！")]
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
        private string _keySignature = "C";

        /// <summary>
        /// 调号错误信息
        /// </summary>
        [ObservableProperty]
        private string? _keySignatureError = "";

        /// <summary>
        /// 每小节拍数（拍号分子，如：2、3、4、5、6、7、9、12）
        /// </summary>
        [Required(ErrorMessage = "每小节拍数不能为空！")]
        [ObservableProperty]
        private string _measureBeatCount = "4";

        /// <summary>
        /// 每小节拍数错误信息
        /// </summary>
        [ObservableProperty]
        private string? _measureBeatCountError = "";

        /// <summary>
        /// 每拍时值（拍号分母，如：2、4、8）
        /// </summary>
        [Required(ErrorMessage = "每拍时值不能为空！")]
        [ObservableProperty]
        private string _beatDuration = "4";

        /// <summary>
        /// 每小节拍数错误信息
        /// </summary>
        [ObservableProperty]
        private string? _beatDurationError = "";

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
        /// 纸张尺寸
        /// </summary>
        [Required(ErrorMessage = "纸张尺寸不能为空！")]
        [ObservableProperty]
        private string _paperSize = "A4";

        /// <summary>
        /// 纸张尺寸错误信息
        /// </summary>
        [ObservableProperty]
        private string? _paperSizeError = "";

        /// <summary>
        /// 页面宽度（默认A4纸宽度）
        /// </summary>
        [ObservableProperty]
        private double _width = 794;

        /// <summary>
        /// 页面高度（默认A4纸高度）
        /// </summary>
        [ObservableProperty]
        private double _height = 1123;

        /// <summary>
        /// 页面方向（1-纵向，2-横向，默认为纵向）
        /// </summary>
        [Required(ErrorMessage = "页面方向不能为空！")]
        [ObservableProperty]
        private string _direction = "1";

        /// <summary>
        /// 页面方向错误信息
        /// </summary>
        [ObservableProperty]
        private string? _directionError = "";

        /// <summary>
        /// 页面左边距（单位：像素，默认为40）
        /// </summary>
        [Required(ErrorMessage = "页面左边距不能为空！")]
        [ObservableProperty]
        private string _leftMargin = "40";

        /// <summary>
        /// 页面左边距错误信息
        /// </summary>
        [ObservableProperty]
        private string? _leftMarginError = "";

        /// <summary>
        /// 页面上边距（单位：像素，默认为40）
        /// </summary>
        [Required(ErrorMessage = "页面上边距不能为空！")]
        [ObservableProperty]
        private string _topMargin = "40";

        /// <summary>
        /// 页面上边距错误信息
        /// </summary>
        [ObservableProperty]
        private string? _topMarginError = "";

        /// <summary>
        /// 页面右边距（单位：像素，默认为40）
        /// </summary>
        [Required(ErrorMessage = "页面右边距不能为空！")]
        [ObservableProperty]
        private string _rightMargin = "40";

        /// <summary>
        /// 页面右边距错误信息
        /// </summary>
        [ObservableProperty]
        private string? _rightMarginError = "";

        /// <summary>
        /// 页面下边距（单位：像素，默认为40）
        /// </summary>
        [Required(ErrorMessage = "页面下边距不能为空！")]
        [ObservableProperty]
        private string _bottomMargin = "40";

        /// <summary>
        /// 页面下边距错误信息
        /// </summary>
        [ObservableProperty]
        private string? _bottomMarginError = "";


        /// <summary>
        /// 简谱渲染元素集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ScoreRenderElement> _renderElements = [];

        /// <summary>
        /// 简谱对象，用于保存当前编辑的简谱数据
        /// </summary>
        private Score _score = new() { Id = Guid.NewGuid().ToString() };
        /// <summary>
        /// 当前行对象
        /// </summary>
        private Line? _line = null;
        /// <summary>
        /// 当前小节对象
        /// </summary>
        private Measure? _measure = null;
        /// <summary>
        /// 当前音符对象
        /// </summary>
        private Note? _note = null;
        /// <summary>
        /// 当前组合对象
        /// </summary>
        private Beam? _beam = null;


        /// <summary>
        /// 定时器
        /// </summary>
        private DispatcherTimer? _debounceTimer;


        public EditScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService, IDialogService dialogService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;
            _dialogService = dialogService;

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
            else if (e.PropertyName == nameof(SubTitle))//当 SubTitle 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(SubTitle));
                SubTitleError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
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
            else if (e.PropertyName == nameof(MeasureBeatCount))//当 MeasureBeatCount 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(MeasureBeatCount));
                MeasureBeatCountError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(BeatDuration))//当 BeatDuration 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(BeatDuration));
                BeatDurationError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Tempo))//当 Tempo 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Tempo));
                TempoError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(PaperSize))//当 PaperSize 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(PaperSize));
                PaperSizeError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(Direction))//当 Direction 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Direction));
                DirectionError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(LeftMargin))//当 LeftMargin 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(LeftMargin));
                LeftMarginError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(TopMargin))//当 TopMargin 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(TopMargin));
                TopMarginError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(RightMargin))//当 RightMargin 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(RightMargin));
                RightMarginError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
            else if (e.PropertyName == nameof(BottomMargin))//当 BottomMargin 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(BottomMargin));
                BottomMarginError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        public void Initialize(JObject param)
        {
            if (param != null)
            {
                if (param["Id"] != null)
                {
                    #region 修改简谱
                    var score = _dataService.GetScore((param["Id"]?.ToString()) ?? "");
                    if (score == null)
                    {
                        _messageService.ShowErrorAsync("未找到指定的简谱数据！").Wait();
                        _navigationService.NavigateTo(typeof(HomePage));
                        return;
                    }

                    CategoryId = score.CategoryId;
                    Title = score.Title;
                    SubTitle = score.Subtitle;
                    Composer = score.Composer ?? "";
                    Lyricist = score.Lyricist ?? "";
                    KeySignature = score.KeySignature;
                    MeasureBeatCount = score.MeasureBeatCount.ToString();
                    BeatDuration = score.BeatDuration.ToString();
                    Tempo = score.Tempo.ToString();
                    PaperSize = score.PaperSize;
                    Direction = score.Direction.ToString();
                    LeftMargin = score.LeftMargin.ToString();
                    TopMargin = score.TopMargin.ToString();
                    RightMargin = score.RightMargin.ToString();
                    BottomMargin = score.BottomMargin.ToString();

                    _score = score;
                    _line = null;
                    if (_score.Lines != null && _score.Lines.Count > 0)
                    {
                        _line = _score.Lines[^1];
                    }
                    _measure = null;
                    if (_line != null && _line.Measures != null && _line.Measures.Count > 0)
                    {
                        _measure = _line.Measures[^1];
                    }
                    _note = null;
                    if (_measure != null && _measure.Notes != null && _measure.Notes.Count > 0)
                    {
                        _note = _measure.Notes[^1];
                    }
                    _beam = null;
                    if (_measure != null && _measure.Beams != null && _measure.Beams.Count > 0)
                    {
                        _beam = _measure.Beams[^1];
                    }
                    #endregion
                }
                else
                {
                    #region 新增简谱
                    if (param["CategoryId"] != null)
                    {
                        _score.CategoryId = (param["CategoryId"]?.ToString()) ?? "";
                    }
                    _score.KeySignature = "C";
                    _score.MeasureBeatCount = 4;
                    _score.BeatDuration = 4;
                    _score.Tempo = 90;
                    _score.PaperSize = "A4";
                    _score.Direction = 1;
                    _score.LeftMargin = 40;
                    _score.TopMargin = 40;
                    _score.RightMargin = 40;
                    _score.BottomMargin = 40;

                    CategoryId = _score.CategoryId;
                    KeySignature = _score.KeySignature;
                    MeasureBeatCount = _score.MeasureBeatCount.ToString();
                    BeatDuration = _score.BeatDuration.ToString();
                    Tempo = _score.Tempo.ToString();
                    PaperSize = _score.PaperSize;
                    Direction = _score.Direction.ToString();
                    LeftMargin = _score.LeftMargin.ToString();
                    TopMargin = _score.TopMargin.ToString();
                    RightMargin = _score.RightMargin.ToString();
                    BottomMargin = _score.BottomMargin.ToString();
                    #endregion
                }

                ScheduleRedraw();
            }
        }

        partial void OnTitleChanged(string value)
        {
            ValidateProperty(value, nameof(Title));

            if (!string.IsNullOrWhiteSpace(TitleError))
            {
                return;
            }

            _score.Title = value;

            ScheduleRedraw();
        }

        partial void OnSubTitleChanged(string value)
        {
            ValidateProperty(value, nameof(SubTitle));

            if (!string.IsNullOrWhiteSpace(SubTitleError))
            {
                return;
            }

            _score.Subtitle = value;

            ScheduleRedraw();
        }

        partial void OnComposerChanged(string value)
        {
            ValidateProperty(value, nameof(Composer));

            if (!string.IsNullOrWhiteSpace(ComposerError))
            {
                return;
            }

            _score.Composer = value;

            ScheduleRedraw();
        }

        partial void OnLyricistChanged(string value)
        {
            ValidateProperty(value, nameof(Lyricist));

            if (!string.IsNullOrWhiteSpace(LyricistError))
            {
                return;
            }

            _score.Lyricist = value;

            ScheduleRedraw();
        }

        partial void OnKeySignatureChanged(string value)
        {
            ValidateProperty(value, nameof(KeySignature));

            if (!string.IsNullOrWhiteSpace(KeySignatureError))
            {
                return;
            }

            _score.KeySignature = value;

            ScheduleRedraw();
        }

        partial void OnMeasureBeatCountChanged(string value)
        {
            ValidateProperty(value, nameof(MeasureBeatCount));

            if (!string.IsNullOrWhiteSpace(MeasureBeatCountError))
            {
                return;
            }

            _score.MeasureBeatCount = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnBeatDurationChanged(string value)
        {
            ValidateProperty(value, nameof(BeatDuration));

            if (!string.IsNullOrWhiteSpace(BeatDurationError))
            {
                return;
            }

            _score.BeatDuration = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnTempoChanged(string value)
        {
            ValidateProperty(value, nameof(Tempo));

            if (!string.IsNullOrWhiteSpace(TempoError))
            {
                return;
            }

            _score.Tempo = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnPaperSizeChanged(string value)
        {
            ValidateProperty(value, nameof(PaperSize));

            if (!string.IsNullOrWhiteSpace(PaperSizeError))
            {
                return;
            }

            _score.PaperSize = value;

            ScheduleRedraw();
        }

        partial void OnDirectionChanged(string value)
        {
            ValidateProperty(value, nameof(Direction));

            if (!string.IsNullOrWhiteSpace(DirectionError))
            {
                return;
            }

            _score.Direction = int.Parse(value);
            if (_score.PaperSize == "A4")
            {
                if (_score.Direction == 1)//纵向
                {
                    Width = 794;
                }
                else//横向
                {
                    Width = 1123;
                }
            }

            ScheduleRedraw();
        }

        partial void OnLeftMarginChanged(string value)
        {
            ValidateProperty(value, nameof(LeftMargin));

            if (!string.IsNullOrWhiteSpace(LeftMarginError))
            {
                return;
            }

            _score.LeftMargin = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnTopMarginChanged(string value)
        {
            ValidateProperty(value, nameof(TopMargin));

            if (!string.IsNullOrWhiteSpace(TopMarginError))
            {
                return;
            }

            _score.TopMargin = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnRightMarginChanged(string value)
        {
            ValidateProperty(value, nameof(RightMargin));

            if (!string.IsNullOrWhiteSpace(RightMarginError))
            {
                return;
            }

            _score.RightMargin = int.Parse(value);

            ScheduleRedraw();
        }

        partial void OnBottomMarginChanged(string value)
        {
            ValidateProperty(value, nameof(BottomMargin));

            if (!string.IsNullOrWhiteSpace(BottomMarginError))
            {
                return;
            }

            _score.BottomMargin = int.Parse(value);

            ScheduleRedraw();
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
        /// 新增行命令
        /// </summary>
        [RelayCommand]
        private async Task NewLine()
        {
            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditLineDialogAsync();
            if (ret != null)
            {
                _score.Lines ??= [];
                _line = new Line
                {
                    Id = ret.Id,
                    ScoreId = _score.Id,
                    Number = _score.Lines.Count + 1,
                    MeasureCount = ret.MeasureCount
                };
                _score.Lines.Add(_line);

                _measure = null;
                _note = null;
                _beam = null;

                //绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 修改行命令
        /// </summary>
        [RelayCommand]
        private async Task EditLine()
        {
            if (_line == null)
            {
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditLineDialogAsync(_line);
            if (ret != null)
            {
                //修改行
                _line.MeasureCount = ret.MeasureCount;

                //绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 删除行命令
        /// </summary>
        [RelayCommand]
        private void DeleteLine()
        {
            if (_score.Lines == null || _score.Lines.Count == 0)
            {
                return;
            }
            if (_line == null)
            {
                return;
            }

            _score.Lines.Remove(_line);

            if (_score.Lines.Count == 0)
            {
                _score.Lines = null;
                _line = null;
            }
            else
            {
                _line = _score.Lines[^1];//[^1]代表最后一个元素
            }

            //绘制简谱
            DrawScore();
        }

        /// <summary>
        /// 新增小节命令
        /// </summary>
        [RelayCommand]
        private async Task NewMeasure()
        {
            if (_line == null)
            {
                await _messageService.ShowErrorAsync("请新增一行！");
                return;
            }

            //判断当前行小节数是否超出排版设置？
            if (_line.Measures != null && _line.Measures.Count + 1 > _line.MeasureCount)
            {
                await _messageService.ShowErrorAsync("小节数超出每行最大小节数，请新增一行！");
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditMeasureDialogAsync();
            if (ret != null)
            {
                _line.Measures ??= [];

                //新增小节
                _measure = new Measure()
                {
                    Id = ret.Id,
                    LineId = _line.Id,
                    ScoreId = _score.Id,
                    Number = _line.Measures.Count + 1,
                    LeftLine = ret.LeftLine,
                    RightLine = ret.RightLine,
                };
                _line.Measures.Add(_measure);

                _note = null;
                _beam = null;

                //绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 修改小节命令
        /// </summary>
        [RelayCommand]
        private async Task EditMeasure()
        {
            if (_measure == null)
            {
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditMeasureDialogAsync(_measure);
            if (ret != null)
            {
                //修改小节
                _measure.LeftLine = ret.LeftLine;
                _measure.RightLine = ret.RightLine;

                //绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 删除小节命令
        /// </summary>
        [RelayCommand]
        private void DeleteMeasure()
        {
            if (_line == null)
            {
                return;
            }
            if (_line.Measures == null || _line.Measures.Count == 0)
            {
                return;
            }
            if (_measure == null)
            {
                return;
            }

            _line.Measures.Remove(_measure);

            if (_line.Measures.Count == 0)
            {
                _line.Measures = null;
                _measure = null;
            }
            else
            {
                _measure = _line.Measures[^1];//[^1]代表最后一个元素
            }

            //绘制简谱
            DrawScore();
        }

        /// <summary>
        /// 新增音符命令
        /// </summary>
        [RelayCommand]
        private async Task NewNote()
        {
            if (_line == null)
            {
                await _messageService.ShowErrorAsync("请新增一行！");
                return;
            }
            if (_measure == null)
            {
                await _messageService.ShowErrorAsync("请新增小节！");
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditNoteDialogAsync(_measure.Beams, null);
            if (ret != null)
            {
                #region 检查当前小节总拍数是否已满
                double currentMeasureBeats = 0;
                if (_measure.Notes != null)
                {
                    foreach (var note in _measure.Notes)
                    {
                        if (string.IsNullOrWhiteSpace(note.BeamId))
                        {
                            if (note.Duration == 1)//四分音符
                            {
                                currentMeasureBeats += 1;

                                if (note.Dots == 1)//单附点延长音符时值的二分之一
                                {
                                    currentMeasureBeats += (1 * 0.5);
                                }
                                else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                {
                                    currentMeasureBeats += (1 * 3.0 / 4.0);
                                }
                                else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                {
                                    currentMeasureBeats += (1 * 7.0 / 8.0);
                                }
                            }
                            else if (note.Duration == 0.5)//八分音符
                            {
                                currentMeasureBeats += 0.5;

                                if (note.Dots == 1)//单附点延长音符时值的二分之一
                                {
                                    currentMeasureBeats += (0.5 * 0.5);
                                }
                                else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                {
                                    currentMeasureBeats += (0.5 * 3.0 / 4.0);
                                }
                                else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                {
                                    currentMeasureBeats += (0.5 * 7.0 / 8.0);
                                }
                            }
                            else if (note.Duration == 0.25)//十六分音符
                            {
                                currentMeasureBeats += 0.25;

                                if (note.Dots == 1)//单附点延长音符时值的二分之一
                                {
                                    currentMeasureBeats += (0.25 * 0.5);
                                }
                                else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                {
                                    currentMeasureBeats += (0.25 * 3.0 / 4.0);
                                }
                                else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                {
                                    currentMeasureBeats += (0.25 * 7.0 / 8.0);
                                }
                            }
                            else if (note.Duration == 0.125)//三十二分音符
                            {
                                currentMeasureBeats += 0.125;

                                if (note.Dots == 1)//单附点延长音符时值的二分之一
                                {
                                    currentMeasureBeats += (0.125 * 0.5);
                                }
                                else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                {
                                    currentMeasureBeats += (0.125 * 3.0 / 4.0);
                                }
                                else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                {
                                    currentMeasureBeats += (0.125 * 7.0 / 8.0);
                                }
                            }
                        }
                    }
                }
                if (_measure.Beams != null)
                {
                    foreach (var beam in _measure.Beams)
                    {
                        if (_measure.Notes != null)
                        {
                            var notesInBeam = _measure.Notes.Where(n => n.BeamId == beam.Id);
                            if (notesInBeam.Any())
                            {
                                currentMeasureBeats++;
                            }
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(ret.BeamId))
                {
                    if (ret.Duration == 1)//四分音符
                    {
                        currentMeasureBeats += 1;

                        if (ret.Dots == 1)//单附点延长音符时值的二分之一
                        {
                            currentMeasureBeats += (1 * 0.5);
                        }
                        else if (ret.Dots == 2)//复附点延长音符时值的四分之三
                        {
                            currentMeasureBeats += (1 * 3.0 / 4.0);
                        }
                        else if (ret.Dots == 3)//三附点延长音符时值的八分之七
                        {
                            currentMeasureBeats += (1 * 7.0 / 8.0);
                        }
                    }
                    else if (ret.Duration == 0.5)//八分音符
                    {
                        currentMeasureBeats += 0.5;

                        if (ret.Dots == 1)//单附点延长音符时值的二分之一
                        {
                            currentMeasureBeats += (0.5 * 0.5);
                        }
                        else if (ret.Dots == 2)//复附点延长音符时值的四分之三
                        {
                            currentMeasureBeats += (0.5 * 3.0 / 4.0);
                        }
                        else if (ret.Dots == 3)//三附点延长音符时值的八分之七
                        {
                            currentMeasureBeats += (0.5 * 7.0 / 8.0);
                        }
                    }
                    else if (ret.Duration == 0.25)//十六分音符
                    {
                        currentMeasureBeats += 0.25;

                        if (ret.Dots == 1)//单附点延长音符时值的二分之一
                        {
                            currentMeasureBeats += (0.25 * 0.5);
                        }
                        else if (ret.Dots == 2)//复附点延长音符时值的四分之三
                        {
                            currentMeasureBeats += (0.25 * 3.0 / 4.0);
                        }
                        else if (ret.Dots == 3)//三附点延长音符时值的八分之七
                        {
                            currentMeasureBeats += (0.25 * 7.0 / 8.0);
                        }
                    }
                    else if (ret.Duration == 0.125)//三十二分音符
                    {
                        currentMeasureBeats += 0.125;

                        if (ret.Dots == 1)//单附点延长音符时值的二分之一
                        {
                            currentMeasureBeats += (0.125 * 0.5);
                        }
                        else if (ret.Dots == 2)//复附点延长音符时值的四分之三
                        {
                            currentMeasureBeats += (0.125 * 3.0 / 4.0);
                        }
                        else if (ret.Dots == 3)//三附点延长音符时值的八分之七
                        {
                            currentMeasureBeats += (0.125 * 7.0 / 8.0);
                        }
                    }
                }
                else
                {
                    if (_measure.Notes != null)
                    {
                        var notesInBeam = _measure.Notes.Where(n => n.BeamId == ret.BeamId);
                        if (!notesInBeam.Any())
                        {
                            currentMeasureBeats++;
                        }
                    }
                }
                if (currentMeasureBeats > _score.MeasureBeatCount)
                {
                    await _messageService.ShowErrorAsync("当前小节拍数已满，请新增小节！");
                    return;
                }
                #endregion

                //初始化 Note 集合
                _measure.Notes ??= [];

                //新增音符
                _note = new Note
                {
                    Id = ret.Id,
                    MeasureId = _measure.Id,
                    LineId = _line.Id,
                    ScoreId = _score.Id,
                    Number = _measure.Notes.Count + 1,
                    Pitch = ret.Pitch,
                    Duration = ret.Duration,
                    Dots = ret.Dots,
                    Slur = ret.Slur,
                    Articulation = ret.Articulation,
                    Fermata = ret.Fermata,
                    Lyrics = ret.Lyrics,
                    BeamId = ret.BeamId,
                    Beam = ret.Beam
                };

                //添加到集合
                _measure.Notes.Add(_note);

                //重新绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 修改音符命令
        /// </summary>
        [RelayCommand]
        private async Task EditNote()
        {
            if (_line == null)
            {
                return;
            }
            if (_measure == null)
            {
                return;
            }
            if (_note == null)
            {
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditNoteDialogAsync(_measure.Beams, _note);
            if (ret != null)
            {
                //修改音符
                _note.Pitch = ret.Pitch;
                _note.Duration = ret.Duration;
                _note.Dots = ret.Dots;
                _note.Slur = ret.Slur;
                _note.Articulation = ret.Articulation;
                _note.Fermata = ret.Fermata;
                _note.Lyrics = ret.Lyrics;
                _note.BeamId = ret.BeamId;
                _note.Beam = ret.Beam;

                //重新绘制简谱
                DrawScore();
            }
        }

        /// <summary>
        /// 删除音符命令
        /// </summary>
        [RelayCommand]
        private void DeleteNote()
        {
            if (_line == null)
            {
                return;
            }
            if (_measure == null)
            {
                return;
            }
            if (_measure.Notes == null || _measure.Notes.Count == 0)
            {
                return;
            }
            if (_note == null)
            {
                return;
            }

            _measure.Notes.Remove(_note);

            if (_measure.Notes.Count == 0)
            {
                _measure.Notes = null;
                _note = null;
            }
            else
            {
                _note = _measure.Notes[^1];//[^1]代表最后一个元素
            }

            //绘制简谱
            DrawScore();
        }

        /// <summary>
        /// 新增组合命令
        /// </summary>
        [RelayCommand]
        private async Task NewBeam()
        {
            if (_line == null)
            {
                await _messageService.ShowErrorAsync("请新增一行！");
                return;
            }
            if (_measure == null)
            {
                await _messageService.ShowErrorAsync("请新增小节！");
                return;
            }

            //新增组合
            _measure.Beams ??= [];
            _beam = new Beam()
            {
                Id = Guid.NewGuid().ToString(),
                MeasureId = _measure.Id,
                LineId = _line.Id,
                ScoreId = _score.Id,
                Number = _measure.Beams.Count + 1,
                Name = $"组合{_measure.Beams.Count + 1}"
            };
            _measure.Beams.Add(_beam);

            await _messageService.ShowInfoAsync("新增组合成功！");
        }

        /// <summary>
        /// 删除组合命令
        /// </summary>
        [RelayCommand]
        private void DeleteBeam()
        {
            if (_line == null)
            {
                return;
            }
            if (_measure == null)
            {
                return;
            }
            if (_measure.Beams == null || _measure.Beams.Count == 0)
            {
                return;
            }
            if (_beam == null)
            {
                return;
            }

            //清除组合中的音符
            if (_measure.Notes != null)
            {
                foreach (var note in _measure.Notes)
                {
                    if (note.BeamId == _beam.Id)
                    {
                        note.BeamId = null;
                    }
                }
            }

            _measure.Beams.Remove(_beam);

            if (_measure.Beams.Count == 0)
            {
                _measure.Beams = null;
                _beam = null;
            }
            else
            {
                _beam = _measure.Beams[^1];//[^1]代表最后一个元素
            }

            //绘制简谱
            DrawScore();
        }

        /// <summary>
        /// 延迟1秒后绘制简谱
        /// </summary>
        private void ScheduleRedraw()
        {
            _debounceTimer?.Stop();
            _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _debounceTimer.Tick += (s, e) =>
            {
                _debounceTimer.Stop();

                DrawScore();
            };
            _debounceTimer.Start();
        }

        /// <summary>
        /// 绘制简谱
        /// </summary>
        private void DrawScore()
        {
            RenderElements.Clear();

            double startX = _score.LeftMargin;
            double startY = _score.TopMargin;
            double canvasWidth = Width - _score.LeftMargin - _score.RightMargin;//设置画布宽度
            double lineHeight = 120;//设置每行高度（默认为120）
            double measureLeftPadding = 10;//小节左边距
            double measureRightPadding = 10;//小节右边距
            double noteBaseYOffset = 40;//音符在每行中的相对Y位置

            double currentY = startY;

            #region 1.绘制元信息

            #region 1.绘制标题
            if (!string.IsNullOrWhiteSpace(_score.Title))
            {
                double fontSize = 24;
                double titleWidth = CalcTextWidth(_score.Title, fontSize, FontWeights.Bold).Width; //动态计算标题宽度

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = _score.LeftMargin + (canvasWidth - titleWidth) / 2, //居中
                    Y = currentY,
                    Text = _score.Title,
                    FontSize = fontSize,
                    IsBold = true
                });
                currentY += 50;
            }
            #endregion

            #region 2.绘制调号、拍号和作曲
            {
                double metaY1 = currentY;
                double leftX1 = startX;

                //左侧：调号
                if (!string.IsNullOrWhiteSpace(_score.KeySignature))
                {
                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1,
                        Text = $"1={_score.KeySignature}",
                        FontSize = 18
                    });
                    leftX1 += 40; //调号后留出间距
                }

                //左侧：拍号（按分数上下显示）
                {
                    //绘制分子（往上偏移）
                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1 - 10,
                        Text = _score.MeasureBeatCount.ToString(),
                        FontSize = 18
                    });
                    //绘制中间的横线
                    RenderElements.Add(new ScoreRenderLineElement
                    {
                        X = leftX1,
                        Y = metaY1 + 14,
                        Width = 14,
                        Height = 1,
                        IsVertical = false
                    });
                    //绘制分母 （往下偏移）
                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX1,
                        Y = metaY1 + 8,
                        Text = _score.BeatDuration.ToString(),
                        FontSize = 18
                    });
                    leftX1 += 30; // 拍号占位
                }

                //右侧：作曲
                if (!string.IsNullOrWhiteSpace(_score.Composer))
                {
                    string composer = $"作曲: {_score.Composer}";
                    string lyricist = $"作词: {_score.Lyricist}";

                    double fontSize = 18;
                    double composerWidth = CalcTextWidth(composer, fontSize, FontWeights.Normal).Width;//动态计算作曲宽度
                    double lyricistWidth = CalcTextWidth(lyricist, fontSize, FontWeights.Normal).Width;//动态计算作词宽度
                    double maxWidth = Math.Max(composerWidth, lyricistWidth); //计算最大宽度

                    double rightX1 = Width - _score.RightMargin - maxWidth;

                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = rightX1,
                        Y = metaY1,
                        Text = composer,
                        FontSize = fontSize
                    });
                }
            }
            #endregion

            #region 3.绘制速度和作词
            {
                double metaY2 = currentY + 35; //下移 35 像素作为第二行
                double leftX2 = startX;

                //左侧：速度
                if (!string.IsNullOrWhiteSpace(_score.Tempo.ToString()))
                {
                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = leftX2,
                        Y = metaY2,
                        Text = $"♩={_score.Tempo}",
                        FontSize = 18
                    });
                }

                //右侧：作词
                if (!string.IsNullOrWhiteSpace(_score.Lyricist))
                {
                    string composer = $"作曲: {_score.Composer}";
                    string lyricist = $"作词: {_score.Lyricist}";

                    double fontSize = 18;
                    double composerWidth = CalcTextWidth(composer, fontSize, FontWeights.Normal).Width;//动态计算作曲宽度
                    double lyricistWidth = CalcTextWidth(lyricist, fontSize, FontWeights.Normal).Width;//动态计算作词宽度
                    double maxWidth = Math.Max(composerWidth, lyricistWidth);//计算最大宽度

                    double rightX2 = Width - _score.RightMargin - maxWidth;

                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        X = rightX2,
                        Y = metaY2,
                        Text = lyricist,
                        FontSize = fontSize
                    });
                }
            }
            #endregion

            #region 4.绘制副标题
            if (!string.IsNullOrWhiteSpace(_score.Subtitle))
            {
                double fontSize = 18;
                Size subTitleSize = CalcTextWidth(_score.Subtitle, fontSize, FontWeights.Normal);//动态计算标题宽度

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = _score.LeftMargin + (canvasWidth - subTitleSize.Width) / 2, //水平居中
                    Y = currentY + (75 - subTitleSize.Height) / 2,//垂直居中
                    Text = _score.Subtitle,
                    FontSize = fontSize
                });
            }
            #endregion

            currentY += 75;

            #endregion

            #region 2.绘制简谱
            if (_score.Lines != null && _score.Lines.Count > 0)
            {
                int measureIndex = 1;//小节序号

                foreach (var line in _score.Lines)
                {
                    #region 1.计算行高
                    {
                        lineHeight = 120;//重置行高

                        if (line.Measures != null && line.Measures.Count > 0)
                        {
                            foreach (var measure in line.Measures)
                            {
                                if (measure.Notes != null && measure.Notes.Count > 0)
                                {
                                    foreach (var note in measure.Notes)
                                    {
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics2))
                                        {
                                            if (lineHeight < 140)
                                            {
                                                lineHeight = 140;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics3))
                                        {
                                            if (lineHeight < 160)
                                            {
                                                lineHeight = 160;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        line.Height = lineHeight;

                        if (currentY + line.Height + _score.BottomMargin > Height)
                        {
                            currentY = (Height + startY);

                            if (_score.Direction == 1)//纵向
                            {
                                Height += 1123;
                            }
                            else//横向
                            {
                                Height += 794;
                            }
                        }
                    }
                    #endregion

                    #region 2.计算当前行音符占位宽度
                    double currentLineBeats = 0;//当前行累计拍数
                    double currentLineNotes = 0;//当前行累计音符数
                    if (line.Measures != null && line.Measures.Count > 0)
                    {
                        foreach (var measure in line.Measures)
                        {
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                //一个占位音符为一拍
                                foreach (var note in measure.Notes)
                                {
                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        if (note.Duration == 1)//四分音符
                                        {
                                            currentLineBeats += 1;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (1 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (1 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (1 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.5)//八分音符
                                        {
                                            currentLineBeats += 0.5;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.5 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.5 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.5 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.25)//十六分音符
                                        {
                                            currentLineBeats += 0.25;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.25 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.25 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.25 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.125)//三十二分音符
                                        {
                                            currentLineBeats += 0.125;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentLineBeats += (0.125 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentLineBeats += (0.125 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentLineBeats += (0.125 * 7.0 / 8.0);
                                            }
                                        }
                                    }
                                }
                                //计算组合拍数，一个组合为一拍
                                if (measure.Beams != null && measure.Beams.Count > 0)
                                {
                                    foreach (var beam in measure.Beams)
                                    {
                                        if (measure.Notes != null)
                                        {
                                            var notesInBeam = measure.Notes.Where(n => n.BeamId == beam.Id);
                                            if (notesInBeam.Any())
                                            {
                                                currentLineBeats++;
                                            }
                                        }
                                    }
                                }
                                currentLineNotes += (measure.Notes?.Count) ?? 0;
                            }
                        }
                    }
                    //如果当前累计拍数小于规定拍数，则添加音符占位
                    if (line.MeasureCount * _score.MeasureBeatCount > currentLineBeats)
                    {
                        currentLineNotes += (line.MeasureCount * _score.MeasureBeatCount - currentLineBeats);
                    }
                    //计算当前行每个音符占位宽度
                    line.NoteWidth = (canvasWidth - line.MeasureCount * (measureLeftPadding + measureRightPadding)) / currentLineNotes;
                    #endregion

                    #region 3.绘制行起点竖线
                    //每行的高度为：rowHeight，上边距为：20，下边距为：20，中间绘制区高度为：80
                    //-------------------------
                    //   20
                    //-------------------------
                    //           20
                    //        -----------------
                    //
                    //   80      40 音符绘制区
                    //
                    //        -----------------
                    //           20
                    //-------------------------
                    //   20
                    //-------------------------
                    RenderElements.Add(new ScoreRenderLineElement
                    {
                        X = startX,
                        Y = currentY + 40,
                        Width = 1,
                        Height = 40,
                        IsVertical = true
                    });
                    #endregion

                    #region 4.绘制小节
                    if (line.Measures != null && line.Measures.Count > 0)
                    {
                        double currentX = startX;

                        foreach (var measure in line.Measures)
                        {
                            #region 1.绘制小节序号
                            RenderElements.Add(new ScoreRenderTextElement
                            {
                                FontSize = 8,
                                X = currentX,
                                Y = currentY + 20,
                                Text = measureIndex.ToString(),
                            });
                            #endregion

                            #region 2.绘制小节左边线（0-无，1-小节线，2-反复起始线）
                            if (measure.LeftLine == 0)//无
                            {
                                //默认不绘制
                            }
                            else if (measure.LeftLine == 1)//小节线
                            {
                                //默认不绘制左小节线
                            }
                            else if (measure.LeftLine == 2)//反复起始线
                            {
                                #region 绘制反复起始线
                                //查找上一个小节是否为反复终止线
                                int i = 0;
                                while (i < line.Measures.Count)
                                {
                                    if (line.Measures[i].Number == measure.Number - 1 && line.Measures[i].RightLine == 4)
                                    {
                                        break;
                                    }
                                    i++;
                                }
                                if (i < line.Measures.Count)
                                {
                                    #region 合并反复起始线和反复终止线
                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX,
                                        Y = currentY + 40,
                                        Width = 2,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + 3,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 5,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 5,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });
                                    #endregion
                                }
                                else
                                {
                                    #region 绘制独立反复起始线
                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX,
                                        Y = currentY + 40,
                                        Width = 3,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + 4,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 6,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + 6,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });
                                    #endregion
                                }
                                #endregion
                            }
                            currentX += measureLeftPadding;
                            #endregion

                            #region 3.绘制音符
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                double currentMeasureBeats = 0;

                                foreach (var note in measure.Notes)
                                {
                                    #region 1.绘制音高
                                    switch (note.Pitch)
                                    {
                                        #region 绘制中音
                                        case "1":
                                        case "2":
                                        case "3":
                                        case "4":
                                        case "5":
                                        case "6":
                                        case "7":
                                            {
                                                var size = CalcTextWidth(note.Pitch, 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch,
                                                    Note = note
                                                });

                                            }
                                            break;
                                        #endregion

                                        #region 绘制低音
                                        case "-1":
                                        case "-2":
                                        case "-3":
                                        case "-4":
                                        case "-5":
                                        case "-6":
                                        case "-7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("-", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("-", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制倍低
                                        case "--1":
                                        case "--2":
                                        case "--3":
                                        case "--4":
                                        case "--5":
                                        case "--6":
                                        case "--7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("--", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("--", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制超低
                                        case "---1":
                                        case "---2":
                                        case "---3":
                                        case "---4":
                                        case "---5":
                                        case "---6":
                                        case "---7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("---", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("---", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制高音
                                        case "+1":
                                        case "+2":
                                        case "+3":
                                        case "+4":
                                        case "+5":
                                        case "+6":
                                        case "+7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("+", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("+", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制倍高
                                        case "++1":
                                        case "++2":
                                        case "++3":
                                        case "++4":
                                        case "++5":
                                        case "++6":
                                        case "++7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("++", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("++", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制极高
                                        case "+++1":
                                        case "+++2":
                                        case "+++3":
                                        case "+++4":
                                        case "+++5":
                                        case "+++6":
                                        case "+++7":
                                            {
                                                var size = CalcTextWidth(note.Pitch.Replace("+++", ""), 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = note.Pitch.Replace("+++", ""),
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制休止符
                                        case "0":
                                            {
                                                var size = CalcTextWidth("0", 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = "0",
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制噪音符
                                        case "X":
                                            {
                                                var size = CalcTextWidth("X", 22, FontWeights.Normal);//动态计算字符大小
                                                var noteBaseXOffset = (double)(line.NoteWidth - size.Width > 0 ? (line.NoteWidth - size.Width) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = size.Width;
                                                note.Height = size.Height;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = (double)note.X,
                                                    Y = (double)note.Y,
                                                    Text = "X",
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制增时符
                                        case "-":
                                            {
                                                //增时符宽度固定为10
                                                var noteBaseXOffset = (double)(line.NoteWidth - 10 > 0 ? (line.NoteWidth - 10) / 2 : 0);//音符在每个占位宽度中水平居中偏移量

                                                note.Width = 10;
                                                note.Height = 30;
                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;

                                                RenderElements.Add(new ScoreRenderLineElement
                                                {
                                                    X = (double)note.X,
                                                    Y = (double)note.Y + 18,
                                                    Width = 10,
                                                    Height = 1,
                                                    IsVertical = false,
                                                    Note = note
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制空白符
                                        case "_":
                                            break;
                                        #endregion
                                    }
                                    #endregion

                                    #region 2.绘制上方符号
                                    {
                                        double topYOffset = currentY + noteBaseYOffset;

                                        #region 1.绘制高音点
                                        if (note.Pitch.StartsWith("+++") || note.Pitch.StartsWith("++") || note.Pitch.StartsWith("+"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 4;
                                            }
                                        }
                                        if (note.Pitch.StartsWith("+++") || note.Pitch.StartsWith("++"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 4;
                                            }
                                        }
                                        if (note.Pitch.StartsWith("+++"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)topYOffset,
                                                    Radius = 4
                                                });

                                                topYOffset -= 4;
                                            }
                                        }
                                        #endregion

                                        #region 2.绘制延长号
                                        if (note.Fermata == 1)
                                        {

                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 3.绘制下方符号和歌词
                                    {
                                        double bottomYOffset = 0;

                                        #region 1.绘制减时线
                                        if (note.Duration == 0.5 || note.Duration == 0.25 || note.Duration == 0.125)
                                        {
                                            //如果是八分音符、十六分音符和三十二分音符
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    RenderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height);
                                            }
                                        }
                                        if (note.Duration == 0.25 || note.Duration == 0.125)
                                        {
                                            //如果是十六分音符和三十二分音符，则绘制第二条减时线
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    RenderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height + 3),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height + 3);
                                            }
                                        }
                                        if (note.Duration == 0.125)
                                        {
                                            //如果是三十二分音符，则绘制第三条减时线
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                //如果没有加入减时组合，则绘制减时线
                                                if (string.IsNullOrWhiteSpace(note.BeamId))
                                                {
                                                    RenderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)note.X,
                                                        Y = (double)(note.Y + note.Height + 6),
                                                        Width = (double)note.Width,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                                bottomYOffset = (double)(note.Y + note.Height + 6);
                                            }
                                        }
                                        #endregion

                                        #region 2.绘制低音点
                                        if (note.Pitch.StartsWith("---") || note.Pitch.StartsWith("--") || (note.Pitch.StartsWith('-') && note.Pitch != "-"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                            {
                                                if (bottomYOffset == 0)
                                                {
                                                    bottomYOffset = (double)(note.Y + note.Height);
                                                }
                                                else
                                                {
                                                    bottomYOffset += 4;
                                                }

                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        if (note.Pitch.StartsWith("---") || note.Pitch.StartsWith("--"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                bottomYOffset += 4;

                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        if (note.Pitch.StartsWith("---"))
                                        {
                                            if (note.X != null && note.Y != null && note.Width != null)
                                            {
                                                bottomYOffset += 4;

                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = (double)(note.X + note.Width / 2 - 2),
                                                    Y = (double)bottomYOffset,
                                                    Radius = 4
                                                });
                                            }
                                        }
                                        #endregion

                                        #region 3.绘制歌词
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics) && note.X != null)
                                        {
                                            RenderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 100,
                                                Text = note.Lyrics
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics2) && note.X != null)
                                        {
                                            RenderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 120,
                                                Text = note.Lyrics2
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(note.Lyrics3) && note.X != null)
                                        {
                                            RenderElements.Add(new ScoreRenderTextElement
                                            {
                                                FontSize = 14,
                                                X = (double)note.X,
                                                Y = currentY + 140,
                                                Text = note.Lyrics3
                                            });
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 4.绘制附点
                                    if (note.Dots > 0 && note.X != null && note.Y != null && note.Width != null && note.Height != null)
                                    {
                                        for (int i = 0; i < note.Dots; i++)
                                        {
                                            RenderElements.Add(new ScoreRenderDotElement
                                            {
                                                X = (double)(note.X + note.Width + 2 + i * 5),
                                                Y = (double)(note.Y + note.Height / 2),
                                                Radius = 4
                                            });
                                        }
                                    }
                                    #endregion

                                    currentX += line.NoteWidth;

                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        if (note.Duration == 1)//四分音符
                                        {
                                            currentMeasureBeats += 1;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (1 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (1 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (1 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.5)//八分音符
                                        {
                                            currentMeasureBeats += 0.5;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.5 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.5 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.5 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.25)//十六分音符
                                        {
                                            currentMeasureBeats += 0.25;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.25 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.25 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.25 * 7.0 / 8.0);
                                            }
                                        }
                                        else if (note.Duration == 0.125)//三十二分音符
                                        {
                                            currentMeasureBeats += 0.125;

                                            if (note.Dots == 1)//单附点延长音符时值的二分之一
                                            {
                                                currentMeasureBeats += (0.125 * 0.5);
                                            }
                                            else if (note.Dots == 2)//复附点延长音符时值的四分之三
                                            {
                                                currentMeasureBeats += (0.125 * 3.0 / 4.0);
                                            }
                                            else if (note.Dots == 3)//三附点延长音符时值的八分之七
                                            {
                                                currentMeasureBeats += (0.125 * 7.0 / 8.0);
                                            }
                                        }
                                    }
                                }

                                //计算组合拍数，一个组合为一拍
                                if (measure.Beams != null && measure.Beams.Count > 0)
                                {
                                    foreach (var beam in measure.Beams)
                                    {
                                        if (measure.Notes != null)
                                        {
                                            var notesInBeam = measure.Notes.Where(n => n.BeamId == beam.Id);
                                            if (notesInBeam.Any())
                                            {
                                                currentMeasureBeats++;
                                            }
                                        }
                                    }
                                }

                                //填充占位音符宽度，如果当前小节拍数小于拍号中的小节拍数，则需要绘制空白音符占位
                                if (currentMeasureBeats < _score.MeasureBeatCount)
                                {
                                    currentX += line.NoteWidth * (_score.MeasureBeatCount - currentMeasureBeats);
                                }
                            }
                            else
                            {
                                currentX += line.NoteWidth * _score.MeasureBeatCount;
                            }
                            #endregion

                            #region 4.绘制减时组合线
                            if (measure.Beams != null && measure.Beams.Count > 0)
                            {
                                foreach (var beam in measure.Beams)
                                {
                                    if (measure.Notes != null)
                                    {
                                        var beamNotes = measure.Notes.Where(n => n.BeamId == beam.Id).OrderBy(n => n.Number);
                                        if (beamNotes != null && beamNotes.Any())
                                        {
                                            #region 1.默认绘制八分音符组合线
                                            {
                                                double? beamX = beamNotes.First<Note>().X;
                                                double? beamY = beamNotes.First<Note>().Y + beamNotes.First<Note>().Height;
                                                double? beamWidth = beamNotes.Last<Note>().X - beamNotes.First<Note>().X + beamNotes.Last<Note>().Width;

                                                if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                {
                                                    RenderElements.Add(new ScoreRenderLineElement
                                                    {
                                                        X = (double)beamX,
                                                        Y = (double)beamY,
                                                        Width = (double)beamWidth,
                                                        Height = 1,
                                                        IsVertical = false
                                                    });
                                                }
                                            }
                                            #endregion

                                            #region 2.绘制十六分音符组合线
                                            {
                                                var noteSequences = FindConsecutiveNoteSequences(beamNotes.ToList(), 0.25F, 1);
                                                if (noteSequences != null && noteSequences.Count > 0)
                                                {
                                                    foreach (var noteSequence in noteSequences)
                                                    {
                                                        double? beamX = noteSequence.First<Note>().X;
                                                        double? beamY = noteSequence.First<Note>().Y + noteSequence.First<Note>().Height;
                                                        double? beamWidth = noteSequence.Last<Note>().X - noteSequence.First<Note>().X + noteSequence.Last<Note>().Width;

                                                        if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                        {
                                                            RenderElements.Add(new ScoreRenderLineElement
                                                            {
                                                                X = (double)beamX,
                                                                Y = (double)beamY + 3,
                                                                Width = (double)beamWidth,
                                                                Height = 1,
                                                                IsVertical = false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region 3.绘制三十二分音符组合线
                                            {
                                                var noteSequences = FindConsecutiveNoteSequences(beamNotes.ToList(), 0.125F, 1);
                                                if (noteSequences != null && noteSequences.Count > 0)
                                                {
                                                    foreach (var noteSequence in noteSequences)
                                                    {
                                                        double? beamX = noteSequence.First<Note>().X;
                                                        double? beamY = noteSequence.First<Note>().Y + noteSequence.First<Note>().Height;
                                                        double? beamWidth = noteSequence.Last<Note>().X - noteSequence.First<Note>().X + noteSequence.Last<Note>().Width;

                                                        if (beamX > 0 && beamY > 0 && beamWidth > 0)
                                                        {
                                                            RenderElements.Add(new ScoreRenderLineElement
                                                            {
                                                                X = (double)beamX,
                                                                Y = (double)beamY + 6,
                                                                Width = (double)beamWidth,
                                                                Height = 1,
                                                                IsVertical = false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 5.绘制小节右边线（0-无，1-小节线，2-虚小节线，3-段落线，4-反复终止线，5-终止线）
                            if (measure.RightLine == 0)//无
                            {
                                //不绘制边线
                            }
                            else if (measure.RightLine == 1)//小节线
                            {
                                #region 绘制小节线
                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 2)//虚小节线
                            {
                                #region 绘制虚小节线
                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true,
                                    IsDashed = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 3)//段落线
                            {
                                #region 绘制段落线
                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 4,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });

                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 1,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            else if (measure.RightLine == 4)//反复终止线
                            {
                                #region 绘制反复终止线
                                //查找下一个小节是否为反复起始线
                                int i = 0;
                                while (i < line.Measures.Count)
                                {
                                    if (line.Measures[i].Number == measure.Number + 1 && line.Measures[i].LeftLine == 2)
                                    {
                                        break;
                                    }
                                    i++;
                                }
                                if (i < line.Measures.Count)
                                {
                                    #region 合并反复终止线和反复起始线
                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 10,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 10,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 6,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 2,
                                        Y = currentY + 40,
                                        Width = 2,
                                        Height = 40,
                                        IsVertical = true
                                    });
                                    #endregion
                                }
                                else
                                {
                                    #region 绘制独立反复终止线
                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 11,
                                        Y = currentY + 40 + 10,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderDotElement
                                    {
                                        X = currentX + measureRightPadding - 11,
                                        Y = currentY + 40 + 30,
                                        Radius = 2
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 7,
                                        Y = currentY + 40,
                                        Width = 1,
                                        Height = 40,
                                        IsVertical = true
                                    });

                                    RenderElements.Add(new ScoreRenderLineElement
                                    {
                                        X = currentX + measureRightPadding - 3,
                                        Y = currentY + 40,
                                        Width = 3,
                                        Height = 40,
                                        IsVertical = true
                                    });
                                    #endregion
                                }
                                #endregion
                            }
                            else if (measure.RightLine == 5)//终止线
                            {
                                #region 绘制乐谱终止线
                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 7,
                                    Y = currentY + 40,
                                    Width = 1,
                                    Height = 40,
                                    IsVertical = true
                                });

                                RenderElements.Add(new ScoreRenderLineElement
                                {
                                    X = currentX + measureRightPadding - 3,
                                    Y = currentY + 40,
                                    Width = 3,
                                    Height = 40,
                                    IsVertical = true
                                });
                                #endregion
                            }
                            currentX += measureRightPadding;
                            #endregion

                            measureIndex++;
                        }
                    }
                    #endregion

                    #region 5.绘制连音线
                    {
                        List<(Note? from, Note? to)> slurNotes = [];//连音音符集合

                        if (line.Measures != null && line.Measures.Count > 0)
                        {
                            foreach (var measure in line.Measures)
                            {
                                if (measure.Notes != null && measure.Notes.Count > 0)
                                {
                                    foreach (var note in measure.Notes)
                                    {
                                        if (note.Slur == 1)//连音线开始
                                        {
                                            slurNotes.Add((note, null));
                                        }
                                        else if (note.Slur == 0)//连音线结束
                                        {
                                            int i = slurNotes.Count - 1;
                                            while (i >= 0)
                                            {
                                                if (slurNotes[i].from != null && slurNotes[i].to == null)
                                                {
                                                    slurNotes[i] = (slurNotes[i].from, note);
                                                    break;
                                                }
                                                i--;
                                            }
                                            if (i < 0)
                                            {
                                                slurNotes.Add((null, note));
                                            }
                                        }
                                        else if (note.Slur == 2)//连音线结束后开始新连音线
                                        {
                                            int i = slurNotes.Count - 1;
                                            while (i >= 0)
                                            {
                                                if (slurNotes[i].from != null && slurNotes[i].to == null)
                                                {
                                                    slurNotes[i] = (slurNotes[i].from, note);
                                                    break;
                                                }
                                                i--;
                                            }
                                            if (i < 0)
                                            {
                                                slurNotes.Add((null, note));
                                            }

                                            slurNotes.Add((note, null));
                                        }
                                    }
                                }
                            }
                        }
                        if (slurNotes.Count > 0)
                        {
                            for (int i = 0; i < slurNotes.Count; i++)
                            {
                                Note? fromNote = slurNotes[i].from;
                                Note? toNote = slurNotes[i].to;

                                if (fromNote != null && toNote != null)
                                {
                                    if (fromNote.X != null && fromNote.Width != null && toNote.X != null && toNote.Width != null)
                                    {
                                        double fromX = (double)(fromNote.X + fromNote.Width / 2);
                                        double fromY = currentY + noteBaseYOffset;
                                        if (fromNote.Pitch.StartsWith("+++") || fromNote.Pitch.StartsWith("++") || fromNote.Pitch.StartsWith("+"))
                                        {
                                            fromY -= 4;
                                        }
                                        if (fromNote.Pitch.StartsWith("+++") || fromNote.Pitch.StartsWith("++"))
                                        {
                                            fromY -= 4;
                                        }
                                        if (fromNote.Pitch.StartsWith("+++"))
                                        {
                                            fromY -= 4;
                                        }
                                        if (fromNote.Fermata == 1)
                                        {
                                            fromY -= 20;
                                        }

                                        double toX = (double)(toNote.X + toNote.Width / 2);
                                        double toY = currentY + noteBaseYOffset;
                                        if (toNote.Pitch.StartsWith("+++") || toNote.Pitch.StartsWith("++") || toNote.Pitch.StartsWith("+"))
                                        {
                                            toY -= 4;
                                        }
                                        if (toNote.Pitch.StartsWith("+++") || toNote.Pitch.StartsWith("++"))
                                        {
                                            toY -= 4;
                                        }
                                        if (toNote.Pitch.StartsWith("+++"))
                                        {
                                            toY -= 4;
                                        }
                                        if (toNote.Fermata == 1)
                                        {
                                            toY -= 20;
                                        }

                                        double Y = Math.Min(fromY, toY);

                                        RenderElements.Add(new ScoreRenderArcElement
                                        {
                                            StartX = fromX,
                                            StartY = Y,
                                            EndX = toX,
                                            EndY = Y,
                                        });
                                    }
                                }
                                else
                                {
                                    if (fromNote != null)
                                    {

                                    }
                                    else if (toNote != null)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    currentY += line.Height;
                }
            }
            #endregion

            #region 3.绘制分页符和页码
            {
                int pageHeight;
                if (_score.Direction == 1)//纵向
                {
                    pageHeight = 1123;
                }
                else//横向
                {
                    pageHeight = 794;
                }

                int pageCount = (int)(Height / pageHeight);

                //绘制页码
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    var pageNo = $"第{pageIndex}页 共{pageCount}页";
                    var pageNoSize = CalcTextWidth(pageNo, 14, FontWeights.Normal);

                    RenderElements.Add(new ScoreRenderTextElement
                    {
                        FontSize = 14,
                        X = startX + (canvasWidth - pageNoSize.Width) / 2,
                        Y = pageIndex * pageHeight - _score.BottomMargin + (_score.BottomMargin - pageNoSize.Height) / 2,
                        Text = pageNo
                    });
                }

                //绘制分页符
                for (int pageIndex = 1; pageIndex < pageCount; pageIndex++)
                {
                    RenderElements.Add(new ScoreRenderLineElement
                    {
                        X = 0,
                        Y = pageIndex * pageHeight,
                        Width = Width,
                        Height = 1,
                        IsVertical = false,
                        IsDashed = true,
                        LineBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 118, 118, 118))
                    });
                }
            }
            #endregion
        }

        /// <summary>
        /// 动态测量文字实际宽度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        private Size CalcTextWidth(string text, double fontSize, FontWeight fontWeight)
        {
            //创建一个临时的 TextBlock 来测量文本宽度
            TextBlock textBlock = new()
            {
                Text = text,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = fontSize,
                FontWeight = fontWeight
            };

            //默认是 Tabular (等宽)，改为 Proportional (比例宽度)
            //设置为比例数字，使每个数字宽度自适应内容
            Microsoft.UI.Xaml.Documents.Typography.SetNumeralAlignment(textBlock, Microsoft.UI.Xaml.FontNumeralAlignment.Proportional);

            //测量文本的实际宽度
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return textBlock.DesiredSize;
        }

        /// <summary>
        /// 找出集合中所有连续的音符序列
        /// </summary>
        /// <param name="notes">音符集合</param>
        /// <param name="duration">时值，默认为八分音符0.5</param>
        /// <param name="minCount">最小连续数量，默认为2（单个音符不构成序列）</param>
        /// <returns></returns>
        private List<List<Note>> FindConsecutiveNoteSequences(List<Note> notes, float duration = 0.5F, int minCount = 2)
        {
            var result = new List<List<Note>>();
            var currentSequence = new List<Note>();

            foreach (var note in notes)
            {
                //判断是否为指定时值音符
                if (note.Duration == duration)
                {
                    currentSequence.Add(note);
                }
                else
                {
                    //遇到了非十六分音符，说明连续序列断裂
                    //如果当前序列满足最小长度要求，则加入结果集
                    if (currentSequence.Count >= minCount)
                    {
                        result.Add(currentSequence);
                    }

                    //清空当前序列，准备寻找下一个连续序列
                    currentSequence = [];
                }
            }

            //遍历结束后，检查最后一段序列（因为可能集合的最后一个元素也是指定时值音符）
            if (currentSequence.Count >= minCount)
            {
                result.Add(currentSequence);
            }

            return result;
        }

        /// <summary>
        /// 音符点击事件
        /// </summary>
        /// <param name="textElement"></param>
        public async Task OnNoteTappedAsync(Note? clickedNote)
        {
            if (clickedNote != null)
            {
                //这里可以执行点击后的逻辑，例如播放音符、弹窗提示等
                string msg = $"你点击了音符: {clickedNote.Pitch} (时值:{clickedNote.Duration}, 附点数:{clickedNote.Dots}, 连音线标志:{clickedNote.Slur}，演奏方法:{clickedNote.Articulation}，延长号标志:{clickedNote.Fermata}，歌词:{clickedNote.Lyrics})";

                //实际开发中可以调用依赖注入的播放服务或弹出通知
                //这里为了演示，修改该音符的颜色（需要给 TextElement 加颜色属性）
                await _messageService.ShowInfoAsync(msg);
            }
        }

        /// <summary>
        /// 打印简谱命令
        /// </summary>
        [RelayCommand]
        private void Print()
        {
            // 发送消息通知 View 触发打印
            _messenger.Send(new PrintScoreMessage());
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
            _score.Subtitle = SubTitle.Trim();
            _score.Composer = Composer.Trim();
            _score.Lyricist = Lyricist.Trim();
            _score.KeySignature = KeySignature.Trim();
            _score.MeasureBeatCount = int.Parse(MeasureBeatCount);
            _score.BeatDuration = int.Parse(BeatDuration);
            _score.Tempo = int.Parse(Tempo.Trim());
            _score.PaperSize = PaperSize.Trim();
            _score.Direction = int.Parse(Direction.Trim());
            _score.LeftMargin = int.Parse(LeftMargin.Trim());
            _score.TopMargin = int.Parse(TopMargin.Trim());
            _score.RightMargin = int.Parse(RightMargin.Trim());
            _score.BottomMargin = int.Parse(BottomMargin.Trim());

            if (_dataService.IsScoreIdExists(_score.Id))
            {
                if (!_dataService.UpdateScore(_score))
                {
                    await _messageService.ShowErrorAsync("保存简谱失败！");
                    return;
                }
            }
            else
            {
                if (!_dataService.AddScore(_score))
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
