using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using SEH.Commons;
using SEH.Models;
using SEH.Services.Interfaces;
using SEH.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
        [ObservableProperty]
        private string _direction = "1";

        /// <summary>
        /// 页面左边距（单位：像素，默认为40）
        /// </summary>
        [ObservableProperty]
        private string _leftMargin = "40";

        /// <summary>
        /// 页面上边距（单位：像素，默认为40）
        /// </summary>
        [ObservableProperty]
        private string _topMargin = "40";

        /// <summary>
        /// 页面右边距（单位：像素，默认为40）
        /// </summary>
        [ObservableProperty]
        private string _rightMargin = "40";

        /// <summary>
        /// 页面下边距（单位：像素，默认为40）
        /// </summary>
        [ObservableProperty]
        private string _bottomMargin = "40";


        /// <summary>
        /// 简谱渲染元素集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ScoreRenderElement> _renderElements = [];

        /// <summary>
        /// 简谱对象，用于保存当前编辑的简谱数据
        /// </summary>
        private Score? _score = null;


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

            Direction = _score.Direction.ToString();
            LeftMargin = _score.LeftMargin.ToString();
            TopMargin = _score.TopMargin.ToString();
            RightMargin = _score.RightMargin.ToString();
            BottomMargin = _score.BottomMargin.ToString();
        }

        /// <summary>
        /// 绘制简谱
        /// </summary>
        public void DrawScore()
        {
            if (_score != null)
            {
                Height = _drawHelper.DrawScore(_score, Width, Height, RenderElements);
            }
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
        private void Edit()
        {

        }

        /// <summary>
        /// 打印简谱命令
        /// </summary>
        [RelayCommand]
        private void Print()
        {
            // 发送消息通知 View 触发打印
            //_messenger.Send(new PrintScoreMessage());
        }

        /// <summary>
        /// 播放简谱命令
        /// </summary>
        [RelayCommand]
        private void Play()
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
