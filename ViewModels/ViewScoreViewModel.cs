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
        /// 简谱对象，用于保存当前编辑的简谱数据
        /// </summary>
        private Score? _score = null;

        //播放控制标志
        private bool _isPlaying = false;


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
        /// </summary>
        [RelayCommand]
        private async Task Play()
        {
            if (_score == null)
            {
                await _messageService.ShowErrorAsync("简谱数据未加载，无法编辑！");
                return;
            }

            //防止重复播放
            if (_isPlaying)
            {
                return;
            }

            try
            {
                _isPlaying = true;

                //1.计算基础时间单位(毫秒)
                //例如：Tempo = 120，意味着四分音符时长 = 60000 / 120 = 500ms
                double beatDurationMs = 60000.0 / _score.Tempo;

                //2.遍历所有行、小节、音符
                if (_score.Lines != null && _score.Lines.Count > 0)
                {
                    foreach (var line in _score.Lines)
                    {
                        if (!_isPlaying) break; //支持停止

                        if (line.Measures != null && line.Measures.Count > 0)
                        {
                            foreach (var measure in line.Measures)
                            {
                                if (!_isPlaying) break; //支持停止

                                if (measure.Notes != null && measure.Notes.Count > 0)
                                {
                                    foreach (var note in measure.Notes)
                                    {
                                        if (!_isPlaying) break; //支持停止

                                        //3.视觉高亮
                                        //找到对应的 RenderElement 并设置高亮颜色 (如红色)
                                        //UpdateNoteHighlight(note.Id, true); 

                                        //4.音频播放
                                        //await _audioService.PlayNoteAsync(note.Pitch, CalculateDuration(note));

                                        //5.计算等待时间
                                        double noteTime = beatDurationMs * note.Duration;
                                        if (note.Dots > 0) noteTime *= 1.5; // 附点处理简化版

                                        await Task.Delay((int)noteTime);

                                        //6.取消高亮
                                        //UpdateNoteHighlight(note.Id, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _isPlaying = false;
            }
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
