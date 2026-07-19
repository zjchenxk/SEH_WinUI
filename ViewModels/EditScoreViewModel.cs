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
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI.Text;

namespace SEH.ViewModels
{
    public partial class EditScoreViewModel : ObservableValidator
    {
        /// <summary>
        /// 注入或实例化 DrawScoreHelper
        /// </summary>
        private readonly DrawScoreHelper _drawHelper = new();

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
        /// 当前连音线对象
        /// </summary>
        private Slur? _slur = null;

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
                    _slur = null;
                    if (_score.Slurs != null && _score.Slurs.Count > 0)
                    {
                        _slur = _score.Slurs[^1];
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

            //获取当前小节开始的连音线
            var startSlurs = _score.Slurs?.Where(s => s.StartLineId == _line.Id && s.StartMeasureId == _measure.Id && string.IsNullOrWhiteSpace(s.StartNoteId)).ToList();

            //获取当前已打开的连音线
            var endSlurs = _score.Slurs?.Where(s => string.IsNullOrEmpty(s.EndNoteId)).ToList();

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditNoteDialogAsync(_measure.Beams, startSlurs, endSlurs, null);
            if (ret != null)
            {
                #region 1.检查当前小节总拍数是否已满
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

                #region 2.添加音符到小节

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
                    Articulation = ret.Articulation,
                    Fermata = ret.Fermata,
                    Paren = ret.Paren,
                    Lyrics = ret.Lyrics,
                    Lyrics2 = ret.Lyrics2,
                    Lyrics3 = ret.Lyrics3,
                    Lyrics4 = ret.Lyrics4,
                    Lyrics5 = ret.Lyrics5,
                    Lyrics6 = ret.Lyrics6,
                    BeamId = ret.BeamId,
                    Beam = ret.Beam,
                    StartSlurs = ret.StartSlurs,
                    EndSlurs = ret.EndSlurs
                };

                //添加到集合
                _measure.Notes.Add(_note);

                #endregion

                #region 3.绑定连音线

                //绑定开始连音线
                if (ret.StartSlurs != null && ret.StartSlurs.Count > 0)
                {
                    foreach (var startSlur in ret.StartSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == startSlur.Id);
                        if (slur != null)
                        {
                            slur.StartNoteId = _note.Id;
                        }
                    }
                }

                //绑定结束连音线
                if (ret.EndSlurs != null && ret.EndSlurs.Count > 0)
                {
                    foreach (var endSlur in ret.EndSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == endSlur.Id);
                        if (slur != null)
                        {
                            slur.EndLineId = _line.Id;
                            slur.EndMeasureId = _measure.Id;
                            slur.EndNoteId = _note.Id;
                        }
                    }
                }

                #endregion

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

            //获取当前小节开始的连音线
            var startSlurs = _score.Slurs?.Where(s => (s.StartLineId == _line.Id && s.StartMeasureId == _measure.Id && string.IsNullOrWhiteSpace(s.StartNoteId)) || s.StartNoteId == _note.Id).ToList();

            //获取当前已打开的连音线
            var endSlurs = _score.Slurs?.Where(s => string.IsNullOrEmpty(s.EndNoteId) || s.EndNoteId == _note.Id).ToList();

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditNoteDialogAsync(_measure.Beams, startSlurs, endSlurs, _note);
            if (ret != null)
            {
                #region 1.解绑连音线

                //解绑开始连音线
                if (_note.StartSlurs != null && _note.StartSlurs.Count > 0)
                {
                    foreach (var startSlur in _note.StartSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == startSlur.Id);
                        if (slur != null)
                        {
                            slur.StartNoteId = "";
                        }
                    }
                }

                //解绑结束连音线
                if (_note.EndSlurs != null && _note.EndSlurs.Count > 0)
                {
                    foreach (var endSlur in _note.EndSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == endSlur.Id);
                        if (slur != null)
                        {
                            slur.EndLineId = "";
                            slur.EndMeasureId = "";
                            slur.EndNoteId = "";
                        }
                    }
                }

                #endregion

                #region 2.修改音符

                _note.Pitch = ret.Pitch;
                _note.Duration = ret.Duration;
                _note.Dots = ret.Dots;
                _note.Articulation = ret.Articulation;
                _note.Fermata = ret.Fermata;
                _note.Paren = ret.Paren;
                _note.Lyrics = ret.Lyrics;
                _note.Lyrics2 = ret.Lyrics2;
                _note.Lyrics3 = ret.Lyrics3;
                _note.Lyrics4 = ret.Lyrics4;
                _note.Lyrics5 = ret.Lyrics5;
                _note.Lyrics6 = ret.Lyrics6;
                _note.BeamId = ret.BeamId;
                _note.Beam = ret.Beam;
                _note.StartSlurs = ret.StartSlurs;
                _note.EndSlurs = ret.EndSlurs;

                #endregion

                #region 3.绑定连音线

                //绑定开始连音线
                if (ret.StartSlurs != null && ret.StartSlurs.Count > 0)
                {
                    foreach (var startSlur in ret.StartSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == startSlur.Id);
                        if (slur != null)
                        {
                            slur.StartNoteId = _note.Id;
                        }
                    }
                }

                //绑定结束连音线
                if (ret.EndSlurs != null && ret.EndSlurs.Count > 0)
                {
                    foreach (var endSlur in ret.EndSlurs)
                    {
                        var slur = _score.Slurs?.FirstOrDefault(s => s.Id == endSlur.Id);
                        if (slur != null)
                        {
                            slur.EndLineId = _line.Id;
                            slur.EndMeasureId = _measure.Id;
                            slur.EndNoteId = _note.Id;
                        }
                    }
                }

                #endregion

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

            #region 解绑连音线

            //解绑开始连音线
            if (_note.StartSlurs != null && _note.StartSlurs.Count > 0)
            {
                foreach (var startSlur in _note.StartSlurs)
                {
                    var slur = _score.Slurs?.FirstOrDefault(s => s.Id == startSlur.Id);
                    if (slur != null)
                    {
                        slur.StartNoteId = "";
                    }
                }
            }

            //解绑结束连音线
            if (_note.EndSlurs != null && _note.EndSlurs.Count > 0)
            {
                foreach (var endSlur in _note.EndSlurs)
                {
                    var slur = _score.Slurs?.FirstOrDefault(s => s.Id == endSlur.Id);
                    if (slur != null)
                    {
                        slur.EndLineId = "";
                        slur.EndMeasureId = "";
                        slur.EndNoteId = "";
                    }
                }
            }

            #endregion

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
        /// 新增连音线命令
        /// </summary>
        [RelayCommand]
        private async Task NewSlur()
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

            //小节总序号
            int measureIndex = 0;
            if (_score.Lines != null && _score.Lines.Count > 0)
            {
                foreach (var line in _score.Lines)
                {
                    if (line.Measures != null)
                    {
                        measureIndex += line.Measures.Count;
                    }
                }
            }

            var slurs = _score.Slurs?.Where(s => s.ScoreId == _score.Id && s.StartLineId == _line.Id && s.StartMeasureId == _measure.Id).ToList();
            var name = $"第{measureIndex}节第{(slurs?.Count + 1 ?? 1)}条连音线";

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditSlurDialogAsync(name);
            if (ret != null)
            {
                _score.Slurs ??= [];

                //新增连音线
                _slur = new Slur()
                {
                    Id = ret.Id,
                    ScoreId = _score.Id,
                    StartLineId = _line.Id,
                    StartMeasureId = _measure.Id,
                    Number = _score.Slurs.Count + 1,
                    Name = ret.Name,
                };
                _score.Slurs.Add(_slur);
            }
        }

        /// <summary>
        /// 修改连音线命令
        /// </summary>
        [RelayCommand]
        private async Task EditSlur()
        {
            if (_slur == null)
            {
                return;
            }

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditSlurDialogAsync("", _slur);
            if (ret != null)
            {
                //修改连音线
                _slur.Name = ret.Name;
            }
        }

        /// <summary>
        /// 删除连音线命令
        /// </summary>
        [RelayCommand]
        private void DeleteSlur()
        {
            if (_score.Slurs == null || _score.Slurs.Count == 0)
            {
                return;
            }
            if (_slur == null)
            {
                return;
            }

            _score.Slurs.Remove(_slur);

            if (_score.Slurs.Count == 0)
            {
                _score.Slurs = null;
                _slur = null;
            }
            else
            {
                _slur = _score.Slurs[^1];//[^1]代表最后一个元素
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
            //将当前的上下文传递给 Helper 类
            Height = _drawHelper.DrawScore(_score, Width, Height, RenderElements);
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
                string msg = $"你点击了音符: {clickedNote.Pitch} (时值:{clickedNote.Duration}, 附点数:{clickedNote.Dots}, 演奏方法:{clickedNote.Articulation}，延长号标志:{clickedNote.Fermata}，歌词:{clickedNote.Lyrics})";

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
