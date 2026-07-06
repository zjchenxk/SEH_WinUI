using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Newtonsoft.Json.Linq;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System;
using System.Collections.ObjectModel;
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
        /// 每行小节数（默认为4）
        /// </summary>
        [Required(ErrorMessage = "每行小节数量不能为空！")]
        [ObservableProperty]
        private string _lineMeasureCount = "4";

        /// <summary>
        /// 每行小节数错误信息
        /// </summary>
        [ObservableProperty]
        private string? _lineMeasureCountError = "";

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
        /// 字符实际宽度（默认12）
        /// </summary>
        [ObservableProperty]
        private double _charWidth = 12;

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
            else if (e.PropertyName == nameof(LineMeasureCount))//当 LineMeasureCount 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(LineMeasureCount));
                LineMeasureCountError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
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
                    Composer = score.Composer ?? "";
                    Lyricist = score.Lyricist ?? "";
                    KeySignature = score.KeySignature;
                    MeasureBeatCount = score.MeasureBeatCount.ToString();
                    BeatDuration = score.BeatDuration.ToString();
                    Tempo = score.Tempo.ToString();
                    LineMeasureCount = score.LineMeasureCount.ToString();
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
                    _score.LineMeasureCount = 4;
                    _score.PaperSize = "A4";
                    _score.Direction = 1;
                    _score.LeftMargin = 20;
                    _score.TopMargin = 20;
                    _score.RightMargin = 20;
                    _score.BottomMargin = 20;

                    CategoryId = _score.CategoryId;
                    KeySignature = _score.KeySignature;
                    MeasureBeatCount = _score.MeasureBeatCount.ToString();
                    BeatDuration = _score.BeatDuration.ToString();
                    Tempo = _score.Tempo.ToString();
                    LineMeasureCount = _score.LineMeasureCount.ToString();
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

        partial void OnLineMeasureCountChanged(string value)
        {
            ValidateProperty(value, nameof(LineMeasureCount));

            if (!string.IsNullOrWhiteSpace(LineMeasureCountError))
            {
                return;
            }

            _score.LineMeasureCount = int.Parse(value);

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
        /// 新增一行命令
        /// </summary>
        [RelayCommand]
        private void NewLine()
        {
            _score.Lines ??= [];
            _line = new Line
            {
                Id = Guid.NewGuid().ToString(),
                ScoreId = _score.Id,
                Number = _score.Lines.Count + 1
            };
            _score.Lines.Add(_line);

            _measure = null;
            _note = null;
            _beam = null;

            //绘制简谱
            DrawScore();
        }

        /// <summary>
        /// 删除一行命令
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
            if (_line.Measures != null && _line.Measures.Count + 1 > _score.LineMeasureCount)
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
                //检查当前小节总拍数是否已满
                int currentMeasureBeats = 0;
                if (_measure.Notes != null)
                {
                    foreach (var note in _measure.Notes)
                    {
                        if (string.IsNullOrWhiteSpace(note.BeamId))
                        {
                            currentMeasureBeats++;
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
                    currentMeasureBeats++;
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
                    BeamId = ret.BeamId
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
            double canvasHeight = Height - _score.TopMargin - _score.BottomMargin;//设置画布高度
            double rowHeight = 120;//设置每行高度
            double measureLeftPadding = 10;//小节左边距
            double measureRightPadding = 10;//小节右边距

            double currentY = startY;

            #region 1.绘制元信息
            //绘制标题
            if (!string.IsNullOrEmpty(_score.Title))
            {
                double titleFontSize = 24;
                double titleWidth = _score.Title.Length * titleFontSize * 0.7; //粗略估算字宽

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = (canvasWidth - titleWidth) / 2, //居中
                    Y = currentY,
                    Text = _score.Title,
                    FontSize = titleFontSize,
                    IsBold = true
                });
                currentY += 50;
            }

            //绘制调号+拍号，作曲
            double metaY1 = currentY;
            double leftX1 = startX;
            double rightX1 = canvasWidth - startX;

            //左侧：调号
            if (!string.IsNullOrEmpty(_score.KeySignature))
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

            // 左侧：拍号（按分数上下显示）
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

            // 右侧：作曲
            if (!string.IsNullOrEmpty(_score.Composer))
            {
                string composer = $"作曲: {_score.Composer}";
                string lyricist = $"作词: {_score.Lyricist}";

                double fontSize = 18;
                double width = Math.Max(composer.Length, lyricist.Length) * fontSize * 0.7; //估算宽度

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = rightX1 - width,
                    Y = metaY1,
                    Text = composer,
                    FontSize = fontSize
                });
            }

            //绘制速度，作词
            double metaY2 = metaY1 + 35; //下移 35 像素作为第二行
            double leftX2 = startX;
            double rightX2 = canvasWidth - startX;

            //左侧：速度
            if (!string.IsNullOrEmpty(_score.Tempo.ToString()))
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
            if (!string.IsNullOrEmpty(_score.Lyricist))
            {
                string composer = $"作曲: {_score.Composer}";
                string lyricist = $"作词: {_score.Lyricist}";

                double fontSize = 18;
                double width = Math.Max(composer.Length, lyricist.Length) * fontSize * 0.7;

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = rightX2 - width,
                    Y = metaY2,
                    Text = lyricist,
                    FontSize = fontSize
                });
            }

            currentY = metaY2 + 40;

            #endregion

            #region 2.绘制简谱
            if (_score.Lines != null && _score.Lines.Count > 0)
            {
                int measureIndex = 1;//小节序号

                foreach (var line in _score.Lines)
                {
                    #region 1.计算当前行音符占位宽度
                    int currentLineBeats = 0;//当前行累计拍数
                    int currentLineNotes = 0;//当前行累计音符数
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
                                        currentLineBeats++;
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
                    if (_score.LineMeasureCount * _score.MeasureBeatCount > currentLineBeats)
                    {
                        currentLineNotes += (_score.LineMeasureCount * _score.MeasureBeatCount - currentLineBeats);
                    }
                    //计算当前行每个音符占位宽度
                    line.NoteWidth = (canvasWidth - _score.LineMeasureCount * (measureLeftPadding + measureRightPadding)) / currentLineNotes;
                    #endregion

                    #region 2.绘制行起点竖线
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

                    #region 3.绘制小节
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
                                double noteBaseXOffset = line.NoteWidth - CharWidth > 0 ? (line.NoteWidth - CharWidth) / 2 : 0;//音符在每行中的相对X位置
                                double noteBaseYOffset = 40;//音符在每行中的相对Y位置

                                int currentMeasureBeats = 0;

                                foreach (var note in measure.Notes)
                                {
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
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = note.Pitch,
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;
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
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = note.Pitch.Replace("-", ""),
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;

                                                //绘制低音音符下方的点，如果有减时线，就在减时线的下方绘制点
                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = currentX + noteBaseXOffset + 5,
                                                    Y = currentY + noteBaseYOffset + 20 + (note.Duration == 0.5 ? 5 : (note.Duration == 0.25 ? 10 : (note.Duration == 0.125 ? 15 : 0))),
                                                    Radius = 3
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
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = note.Pitch.Replace("+", ""),
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;

                                                //绘制高音音符上方的点
                                                RenderElements.Add(new ScoreRenderDotElement
                                                {
                                                    X = currentX + noteBaseXOffset + 5,
                                                    Y = currentY + noteBaseYOffset - 5,
                                                    Radius = 3
                                                });
                                            }
                                            break;
                                        #endregion

                                        #region 绘制休止符
                                        case "0":
                                            {
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = "0",
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;
                                            }
                                            break;
                                        #endregion

                                        #region 绘制噪音符
                                        case "X":
                                            {
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = "X",
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;
                                            }
                                            break;
                                        #endregion

                                        #region 绘制增时符
                                        case "-":
                                            {
                                                RenderElements.Add(new ScoreRenderLineElement
                                                {
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset + 18,
                                                    Width = 10,
                                                    Height = 1,
                                                    IsVertical = false,
                                                    Note = note
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = line.NoteWidth;
                                            }
                                            break;
                                        #endregion
                                    }
                                    currentX += line.NoteWidth;

                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        currentMeasureBeats++;
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
                            double beamBaseYOffset = 75;//音符组合线在每行中的相对Y位置
                            if (measure.Beams != null && measure.Beams.Count > 0)
                            {
                                foreach (var beam in measure.Beams)
                                {
                                    if (measure.Notes != null)
                                    {
                                        var notesInBeam = measure.Notes.Where(n => n.BeamId == beam.Id).OrderBy(n => n.Number);
                                        if (notesInBeam != null && notesInBeam.Count<Note>() > 0)
                                        {
                                            double beamX = notesInBeam.First<Note>().X ?? 0;
                                            double beamWidth = (notesInBeam.Last<Note>().X ?? 0) - (notesInBeam.First<Note>().X ?? 0) + CharWidth;

                                            if (beamX > 0 && beamWidth > 0)
                                            {
                                                //绘制组合横线
                                                //音符字符字体大小为22，像素高度为33，其中内边距上部为11，下部为5，实际字符高度为17
                                                RenderElements.Add(new ScoreRenderLineElement
                                                {
                                                    X = beamX,
                                                    Y = currentY + beamBaseYOffset,
                                                    Width = beamWidth,
                                                    Height = 1,
                                                    IsVertical = false
                                                });
                                            }
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

                    #region 4.自动换页
                    currentY += rowHeight;
                    if (currentY > canvasHeight)
                    {
                        if (_score.Direction == 1)//纵向
                        {
                            Height += 1123;
                        }
                        else//横向
                        {
                            Height += 794;
                        }
                        canvasHeight = Height - _score.TopMargin - _score.BottomMargin;//重置画布高度
                    }
                    #endregion
                }
            }
            #endregion
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
            _score.Composer = Composer.Trim();
            _score.Lyricist = Lyricist.Trim();
            _score.KeySignature = KeySignature.Trim();
            _score.MeasureBeatCount = int.Parse(MeasureBeatCount);
            _score.BeatDuration = int.Parse(BeatDuration);
            _score.Tempo = int.Parse(Tempo.Trim());
            _score.LineMeasureCount = int.Parse(LineMeasureCount.Trim());
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
