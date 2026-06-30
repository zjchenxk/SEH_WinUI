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
        /// 简谱渲染元素集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ScoreRenderElement> _renderElements = [];

        /// <summary>
        /// 画布宽度（默认A4纸宽度）
        /// </summary>
        [ObservableProperty]
        private double _canvasWidth = 794;

        /// <summary>
        /// 画布高度（默认两张A4纸高度）
        /// </summary>
        [ObservableProperty]
        private double _canvasHeight = 2246;


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
            else if (e.PropertyName == nameof(Direction))//当 Direction 属性的验证状态发生变化时，更新错误提示文本
            {
                //GetErrors 返回的是 ValidationResult 集合
                var errors = GetErrors(nameof(Direction));
                DirectionError = errors.Cast<ValidationResult>().FirstOrDefault()?.ErrorMessage;
            }
        }

        public void Initialize(JObject param)
        {
            if (param != null)
            {
                if (param["Id"] != null)//修改简谱
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
                    MeasureBeatCount = score.MeasureBeatCount.ToString();
                    BeatDuration = score.BeatDuration.ToString();
                    Tempo = score.Tempo.ToString();
                    LineMeasureCount = score.LineMeasureCount.ToString();
                    Direction = score.Direction.ToString();

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
                }
                else//新增简谱
                {
                    if (param["CategoryId"] != null)
                    {
                        _score.CategoryId = param["CategoryId"].ToString();
                    }
                    _score.KeySignature = "C";
                    _score.MeasureBeatCount = 4;
                    _score.BeatDuration = 4;
                    _score.Tempo = 90;
                    _score.LineMeasureCount = 4;
                    _score.Direction = 1;

                    CategoryId = _score.CategoryId;
                    KeySignature = _score.KeySignature;
                    MeasureBeatCount = _score.MeasureBeatCount.ToString();
                    BeatDuration = _score.BeatDuration.ToString();
                    Tempo = _score.Tempo.ToString();
                    LineMeasureCount = _score.LineMeasureCount.ToString();
                    Direction = _score.Direction.ToString();
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

        partial void OnDirectionChanged(string value)
        {
            ValidateProperty(value, nameof(Direction));

            if (!string.IsNullOrWhiteSpace(DirectionError))
            {
                return;
            }

            _score.Direction = int.Parse(value);
            if (_score.Direction == 1)//横向
            {
                CanvasWidth = 794;
            }
            else//纵向
            {
                CanvasWidth = 1123;
            }

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

            //判断行宽度是否超出画布宽度？
            if (_line.Measures != null && _line.Measures.Count > 0)
            {
                //获取当前所有小节总宽度
                double totalMeasureWidth = 0;
                foreach (var measure in _line.Measures)
                {
                    totalMeasureWidth += measure.Width;
                }
                if (totalMeasureWidth + 10/*左边距*/ + 40 * _score.MeasureBeatCount/*音符总宽度*/ + 10/*右边距*/ + 2/*小节终点竖线*/ >= CanvasWidth)
                {
                    await _messageService.ShowErrorAsync("页面宽度不足，请新增一行！");
                    return;
                }
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

            #region 检查当前小节内的音符总拍数是否已满
            int totalBeats = 0;
            if (_measure.Notes != null)
            {
                foreach (var note in _measure.Notes)
                {
                    if (string.IsNullOrWhiteSpace(note.BeamId))
                    {
                        totalBeats++;
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
                            totalBeats++;
                        }
                    }
                }
            }
            if (totalBeats >= _score.MeasureBeatCount)
            {
                await _messageService.ShowErrorAsync("当前小节内的音符总拍数已满，请新增小节！");
                return;
            }
            #endregion

            //调用服务显示弹窗并获取结果
            var ret = await _dialogService.ShowEditNoteDialogAsync(_measure.Beams, null);
            if (ret != null)
            {
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
        private void EditNote()
        {
            if (_note == null)
            {
                return;
            }

        }

        /// <summary>
        /// 删除音符命令
        /// </summary>
        [RelayCommand]
        private void DeleteNote()
        {

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

            double startX = 20;
            double startY = 20;
            double rowHeight = 120;
            double noteWidth = 40;
            double noteBaseXOffset = 10;//音符在每行中的相对X位置
            double noteBaseYOffset = 40;//音符在每行中的相对Y位置
            double beamBaseYOffset = 75;//音符组合线在每行中的相对Y位置
            //int beats = 4;//每小节拍数，默认4拍

            //设置工作区宽度，页边距20
            double workspaceWidth = CanvasWidth - 40;

            double currentY = startY;

            #region 绘制元信息
            //绘制标题
            if (!string.IsNullOrEmpty(_score.Title))
            {
                double titleFontSize = 24;
                double titleWidth = _score.Title.Length * titleFontSize * 0.7; //粗略估算字宽

                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = (workspaceWidth - titleWidth) / 2, //居中
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
            double rightX1 = workspaceWidth - startX;

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
                    Height = 2,
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
            double rightX2 = workspaceWidth - startX;

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

            #region 绘制行
            if (_score.Lines != null)
            {
                int measureIndex = 1;//小节序号

                foreach (var line in _score.Lines)
                {
                    double currentX = startX;

                    //绘制行起点竖线
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
                        X = currentX,
                        Y = currentY + 40,
                        Width = 2,
                        Height = 40,
                        IsVertical = true
                    });

                    currentX += 2;

                    #region 绘制小节
                    if (line.Measures != null)
                    {
                        foreach (var measure in line.Measures)
                        {
                            //绘制小节序号
                            RenderElements.Add(new ScoreRenderTextElement
                            {
                                FontSize = 8,
                                X = currentX - 4,
                                Y = currentY + 20,
                                Text = measureIndex.ToString(),
                            });

                            currentX += 10;//行起点竖线或小节竖线与音符的间距

                            #region 绘制音符
                            //|    40    |
                            //|  |    |  |
                            //|10| 20 |10|
                            //|  |    |  |
                            double totalNoteWidth = 0;
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                int totalBeats = 0;

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
                                                    Text = note.Pitch
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;
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
                                                    Text = note.Pitch.Replace("-", "")
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;

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
                                                    Text = note.Pitch.Replace("+", "")
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;

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
                                                    Text = "0"
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;
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
                                                    Text = "X"
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;
                                            }
                                            break;
                                        #endregion

                                        #region 绘制增时符
                                        case "-":
                                            {
                                                RenderElements.Add(new ScoreRenderTextElement
                                                {
                                                    FontSize = 22,
                                                    X = currentX + noteBaseXOffset,
                                                    Y = currentY + noteBaseYOffset,
                                                    Text = "-"
                                                });

                                                note.X = currentX + noteBaseXOffset;
                                                note.Y = currentY + noteBaseYOffset;
                                                note.Width = noteWidth;
                                            }
                                            break;
                                        #endregion
                                    }

                                    if (string.IsNullOrWhiteSpace(note.BeamId))
                                    {
                                        totalBeats++;
                                    }

                                    totalNoteWidth += noteWidth;
                                    currentX += noteWidth;
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
                                                totalBeats++;
                                            }
                                        }
                                    }
                                }

                                //填充空白音符宽度，如果小节内的音符总拍数小于拍号中的分子，则需要绘制空白音符占位
                                if (totalBeats < _score.MeasureBeatCount)
                                {
                                    double emptyNoteWidth = noteWidth * (_score.MeasureBeatCount - totalBeats);
                                    totalNoteWidth += emptyNoteWidth;
                                    currentX += emptyNoteWidth;
                                }
                            }
                            else
                            {
                                //绘制空白小节
                                totalNoteWidth = noteWidth * _score.MeasureBeatCount/*每小节拍数*/;
                                currentX += totalNoteWidth;
                            }
                            #endregion

                            #region 绘制音符组合线
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
                                            double beamWidth = (notesInBeam.Last<Note>().X ?? 0) - (notesInBeam.First<Note>().X ?? 0) + 10;

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

                            currentX += 10;

                            //绘制小节终点竖线
                            RenderElements.Add(new ScoreRenderLineElement
                            {
                                X = currentX,
                                Y = currentY + 40,
                                Width = 2,
                                Height = 40,
                                IsVertical = true
                            });

                            measure.Width = 10/*左边距*/ + totalNoteWidth/*所有音符宽度*/ + 10/*右边距*/ + 2/*小节终点竖线宽度*/;

                            currentX += 2;

                            measureIndex++;
                        }
                    }
                    #endregion

                    currentY += rowHeight;
                }
            }
            #endregion
        }

        /// <summary>
        /// 音符点击事件
        /// </summary>
        /// <param name="textElement"></param>
        public void OnNoteTappedAsync(ScoreRenderTextElement textElement)
        {
            if (textElement != null)
            {
                //获取关联的原始音符数据
                var clickedNote = textElement.NoteSource;

                if (clickedNote != null)
                {
                    //这里可以执行点击后的逻辑，例如播放音符、弹窗提示等
                    string msg = $"你点击了音符: {clickedNote.Pitch} (时值:{clickedNote.Duration}, 附点数:{clickedNote.Dots}, 连音线标志:{clickedNote.Slur}，演奏方法:{clickedNote.Articulation}，延长号标志:{clickedNote.Fermata}，歌词:{clickedNote.Lyrics})";

                    //实际开发中可以调用依赖注入的播放服务或弹出通知
                    //这里为了演示，修改该音符的颜色（需要给 TextElement 加颜色属性）
                    _messageService.ShowInfoAsync(msg).Wait();
                }
            }
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
