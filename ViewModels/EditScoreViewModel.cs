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
using System.Collections.Generic;
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
        private Score _score = new();
        /// <summary>
        /// 当前行对象
        /// </summary>
        private Line? _line = null;
        /// <summary>
        /// 当前小节对象
        /// </summary>
        private Measure? _measure = null;

        /// <summary>
        /// 定时器
        /// </summary>
        private DispatcherTimer? _debounceTimer;


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
                    TimeSignature = score.TimeSignature;
                    Tempo = score.Tempo.ToString();

                    _score = score;
                }
                else//新增简谱
                {
                    if (param["CategoryId"] != null)
                    {
                        CategoryId = param["CategoryId"].ToString();
                    }
                    KeySignature = "C";
                    TimeSignature = "4/4";
                    Tempo = "90";
                }
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

        partial void OnTimeSignatureChanged(string value)
        {
            ValidateProperty(value, nameof(TimeSignature));

            if (!string.IsNullOrWhiteSpace(TimeSignatureError))
            {
                return;
            }

            _score.TimeSignature = value;

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
            _line = new Line();
            _score.Lines ??= [];
            _score.Lines.Add(_line);

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
            _score.Lines.RemoveAt(_score.Lines.Count - 1);
            if (_score.Lines.Count == 0)
            {
                _score.Lines = null;
                _line = null;
            }
            else
            {
                _line = _score.Lines[^1];//[^1]代表最后一个元素
            }

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
                await _messageService.ShowErrorAsync("请先新增一行！");
                return;
            }


        }

        /// <summary>
        /// 删除小节命令
        /// </summary>
        [RelayCommand]
        private void DeleteMeasure()
        {

        }

        /// <summary>
        /// 新增音符命令
        /// </summary>
        [RelayCommand]
        private void NewNote()
        {

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
        private void NewBeam()
        {

        }

        /// <summary>
        /// 删除组合命令
        /// </summary>
        [RelayCommand]
        private void DeleteBeam()
        {

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
            double noteBaseYOffset = 40;//音符在每行中的相对Y位置

            //设置工作区宽度，页边距20
            double workspaceWidth = CanvasWidth - 40;

            double currentY = startY;

            #region 绘制基本信息
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

            //绘制元信息行（第一行和第二行）
            //第一行：左侧调号+拍号，右侧作曲
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
            if (!string.IsNullOrEmpty(_score.TimeSignature) && _score.TimeSignature.Contains('/'))
            {
                var parts = _score.TimeSignature.Split('/');
                string numerator = parts[0];     //分子（如：4）
                string denominator = parts[1];   //分母（如：4）

                //绘制分子（往上偏移）
                RenderElements.Add(new ScoreRenderTextElement
                {
                    X = leftX1,
                    Y = metaY1 - 10,
                    Text = numerator,
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
                    Text = denominator,
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

            //第二行：左侧速度，右侧作词
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

            //元信息结束，为乐谱音符区腾出空间
            currentY = metaY2 + 40;

            #endregion

            #region 绘制行
            if (_score.Lines != null)
            {
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
                            currentX += 10;//行起点竖线或小节竖线与音符的间距

                            #region 绘制音符
                            if (measure.Notes != null && measure.Notes.Count > 0)
                            {
                                foreach (var note in measure.Notes)
                                {

                                }
                            }
                            else
                            {
                                currentX += 160;//绘制空白小节
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
                            currentX += 2;
                        }
                    }
                    #endregion

                    currentY += rowHeight;
                }
            }
            #endregion
        }

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
