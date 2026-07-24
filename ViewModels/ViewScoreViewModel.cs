using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace SEH.ViewModels
{
    public partial class ViewScoreViewModel : ObservableValidator
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
        /// 播放服务，用于简谱播放
        /// </summary>
        private readonly IAudioService _audioService;

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
        /// 简谱渲染元素集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ScoreRenderElement> _renderElements = [];

        /// <summary>
        /// 当前正在播放的音符（用于驱动 UI 高亮蒙板移动）
        /// </summary>
        [ObservableProperty]
        private Note? _currentPlayingNote;

        //播放控制标志
        //使用 [ObservableProperty] 生成 IsPlaying 属性
        //使用 [NotifyCanExecuteChangedFor] 在状态改变时自动通知命令刷新按钮状态
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PlayCommand))]
        [NotifyCanExecuteChangedFor(nameof(StopCommand))]
        private bool _isPlaying = false;

        private CancellationTokenSource? _cts;

        /// <summary>
        /// 简谱对象，用于保存当前编辑的简谱数据
        /// </summary>
        private Score? _score = null;


        public ViewScoreViewModel(IMessenger messenger, INavigationService navigationService, IDataService dataService, IMessageService messageService, IAudioService audioService)
        {
            _messenger = messenger;
            _navigationService = navigationService;
            _dataService = dataService;
            _messageService = messageService;
            _audioService = audioService;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        public void Initialize(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _messageService.ShowErrorAsync("简谱ID不能为空！").Wait();
                _navigationService.NavigateTo(typeof(HomePage));
                return;
            }

            _score = _dataService.GetScore(id);
            if (_score == null)
            {
                _messageService.ShowErrorAsync("未找到指定的简谱数据！").Wait();
                _navigationService.NavigateTo(typeof(HomePage));
                return;
            }

            //初始化页面宽高，根据简谱的方向设置
            if (_score.Direction == 1)//纵向
            {
                Width = 794;
            }
            else//横向
            {
                Width = 1123;
            }

            //绘制简谱
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
        /// 编辑简谱命令
        /// </summary>
        [RelayCommand]
        private async Task Edit()
        {
            if (_score == null)
            {
                await _messageService.ShowErrorAsync("简谱数据未加载，无法编辑！");
                return;
            }

            string id = _score.Id;

            JObject param = new(new JProperty("Id", id));

            _navigationService.NavigateTo(typeof(EditScorePage), param);

            //通过 MainViewModel 设置面包屑导航
            var mainViewModel = App.Services?.GetRequiredService<MainViewModel>();
            if (mainViewModel != null)
            {
                mainViewModel.BreadcrumbItems = ["首页", "修改简谱"];
            }
        }

        /// <summary>
        /// 打印简谱命令
        /// </summary>
        [RelayCommand]
        private async Task Print()
        {
            if (_score == null)
            {
                await _messageService.ShowErrorAsync("简谱数据未加载，无法编辑！");
                return;
            }

            // 发送消息通知 View 触发打印
            _messenger.Send(new PrintScoreMessage());
        }

        /// <summary>
        /// 播放简谱命令
        /// 添加 CanExecute：只有不在播放时才能点击
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPlay))]
        private async Task Play()
        {
            if (_score == null)
            {
                await _messageService.ShowErrorAsync("简谱数据未加载，无法编辑！");
                return;
            }

            try
            {
                //1.设置状态为播放中 (触发 UI：播放按钮灰化，停止按钮激活)
                IsPlaying = true;

                //2.初始化播放环境
                _cts = new CancellationTokenSource();

                //3.初始化 MIDI 合成器（如果尚未初始化）
                await _audioService.InitializeAsync();

                //4.计算基础时间单位 (BPM转毫秒)
                //例如：Tempo = 120，意味着四分音符时长 = 60000 / 120 = 500ms
                double baseBeatMs = 60000.0 / _score.Tempo;

                //5.遍历所有行、小节、音符
                if (_score.Lines != null && _score.Lines.Count > 0)
                {
                    foreach (var line in _score.Lines)
                    {
                        if (line.Measures != null && line.Measures.Count > 0)
                        {
                            foreach (var measure in line.Measures)
                            {
                                if (measure.Notes != null && measure.Notes.Count > 0)
                                {
                                    foreach (var note in measure.Notes)
                                    {
                                        //检查是否请求了停止
                                        if (_cts.IsCancellationRequested)
                                        {
                                            break;
                                        }

                                        //6.计算当前音符的实际时长
                                        double durationMs = CalculateNoteDuration(note, baseBeatMs);

                                        //7.UI 交互：设置当前播放音符（触发 View 移动蒙板）
                                        CurrentPlayingNote = note;

                                        //7.音频播放
                                        //过滤掉休止符和空白符，只播放实际音符
                                        bool isSilent = note.Pitch == "0" || note.Pitch == "_" || note.Pitch == "X";
                                        if (!isSilent)
                                        {
                                            await _audioService.PlayNoteAsync(note.Pitch, durationMs);
                                        }
                                        else
                                        {
                                            //如果是休止符，只等待时间，不发声
                                            await Task.Delay((int)durationMs, _cts.Token);
                                        }

                                        //8.播放完毕，清除高亮
                                        CurrentPlayingNote = null;
                                    }
                                }

                                //检查是否请求了停止
                                if (_cts.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                        }

                        //检查是否请求了停止
                        if (_cts.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                //正常停止，无需处理
            }
            finally
            {
                //9.恢复状态 (触发 UI：播放按钮激活，停止按钮灰化)
                IsPlaying = false;

                CurrentPlayingNote = null;
            }
        }

        /// <summary>
        /// 判断是否可以播放
        /// </summary>
        /// <returns></returns>
        private bool CanPlay() => !IsPlaying;

        /// <summary>
        /// 根据音符属性计算实际播放时长（含附点）
        /// </summary>
        private double CalculateNoteDuration(Note note, double baseBeatMs)
        {
            //基础时长 = 基础拍长 * 音符时值 (如四分音符=1, 八分音符=0.5)
            double duration = baseBeatMs * note.Duration;

            //附点处理逻辑 (参考 DrawScoreHelper 的计算方式)
            //每一个附点增加前一时值的一半长度
            //1个附点: duration += duration * 0.5
            //2个附点: duration += duration * (0.5 + 0.25) ...
            double dotMultiplier = 0;
            double fraction = 0.5; //第一个附点加 50%

            for (int i = 0; i < note.Dots; i++)
            {
                dotMultiplier += fraction;
                fraction *= 0.5; // 后续附点比例减半
            }

            duration += (baseBeatMs * note.Duration) * dotMultiplier;

            return duration;
        }

        /// <summary>
        /// 内部停止逻辑
        /// </summary>
        private void StopInternal()
        {
            _cts?.Cancel();
            _isPlaying = false;
            CurrentPlayingNote = null;
        }

        /// <summary>
        /// 停止播放命令
        /// 添加 CanExecute：只有正在播放时才能点击
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanStop))]
        private void Stop()
        {
            //调用取消源
            _cts?.Cancel();
        }

        /// <summary>
        /// 判断是否可以停止
        /// </summary>
        /// <returns></returns>
        private bool CanStop() => IsPlaying;

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
