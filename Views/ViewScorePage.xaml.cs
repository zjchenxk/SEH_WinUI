using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Printing;
using SEH.Commons;
using SEH.Services.Interfaces;
using SEH.ViewModels;
using Serilog;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;

namespace SEH.Views
{
    /// <summary>
    /// 查看简谱页面
    /// </summary>
    public sealed partial class ViewScorePage : Page
    {
        public ViewScoreViewModel? ViewModel { get; }
        private readonly IMessenger? Messenger;

        //打印相关变量
        private PrintManager? PrintMan;
        private PrintDocument? PrintDoc;
        private IPrintDocumentSource? PrintDocSource;
        private UIElement? PrintContainer; //要打印的 UI 元素
        private IntPtr PrintHwnd;
        private PrintPageDescription PrintPageDesc;


        public ViewScorePage()
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = App.Services?.GetRequiredService<ViewScoreViewModel>();
            //获取 Messenger 实例
            Messenger = App.Services?.GetRequiredService<IMessenger>();

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            this.DataContext = ViewModel;

            this.Loaded += ViewScorePage_Loaded;

            //获取简谱画布作为打印容器
            PrintContainer = ScoreCanvas;

            //关键：在 UI 线程提前创建 PrintDocument
            PrintDoc = new PrintDocument();
            PrintDocSource = PrintDoc.DocumentSource;
            PrintDoc.Paginate += Paginate;
            PrintDoc.GetPreviewPage += GetPreviewPage;
            PrintDoc.AddPages += AddPages;
        }

        private void ViewScorePage_Loaded(object sender, RoutedEventArgs e)
        {
            //从 ViewModel 获取 DialogService 并设置 XamlRoot
            //注意：这里可以通过 App.Services 直接获取服务实例
            var dialogService = App.Services?.GetRequiredService<IDialogService>();
            if (dialogService != null && dialogService is SEH.Services.DialogService ds)
            {
                ds.XamlRoot = this.XamlRoot;
            }
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //1.初始化业务逻辑
            if (e.Parameter != null)
            {
                //将参数传给 ViewModel 进行初始化
                ViewModel?.Initialize((string)e.Parameter);
            }

            //2.初始化打印环境
            if (App.MainWindow == null) return;

            PrintHwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

            //直接使用强类型调用，不再需要反射
            try
            {
                PrintMan = PrintManagerInterop.GetForWindow(PrintHwnd);
                if (PrintMan != null)
                {
                    PrintMan.PrintTaskRequested += PrintTaskRequested;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "PrintManager 注册失败");
            }

            //3.注册消息
            Messenger?.Register<PrintScoreMessage>(this, (r, m) => { _ = PrintAsync(); });

            //4.监听 ViewModel 属性变化（用于驱动蒙板）
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (PrintMan != null)
            {
                PrintMan.PrintTaskRequested -= PrintTaskRequested;
            }

            //使用 DI 获取的实例注销
            Messenger?.Unregister<PrintScoreMessage>(this);

            //取消监听
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        private async void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            //直接使用提前创建好的 PrintDocSource
            PrintTask printTask = args.Request.CreatePrintTask("简谱打印导出", sourceRequested =>
            {
                sourceRequested.SetSource(PrintDocSource);
            });

            deferral.Complete();
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            if (ViewModel == null) return;

            //获取打印设置中的第一页页面描述，并缓存起来
            PrintPageDesc = e.PrintTaskOptions.GetPageDescription(0);
            double printableHeight = PrintPageDesc.ImageableRect.Height;

            //直接使用 ViewModel 中的简谱总高度（去除界面边距和边框的影响）
            double contentHeight = ViewModel.Height;

            //计算需要打印的总页数
            int pageCount = (int)(contentHeight / printableHeight);

            //加入 1 像素容差防止精度误差导致多出空白页
            //只有当内容高度大于一页，且超出部分大于 1 像素时，才增加一页
            if (contentHeight > printableHeight && (contentHeight % printableHeight) > 1.0)
            {
                pageCount++;
            }
            if (pageCount < 1) pageCount = 1;

            PrintDoc?.SetPreviewPageCount(pageCount, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            if (PrintContainer != null)
            {
                var pageElement = CreatePageForPrinting(e.PageNumber, PrintPageDesc);
                PrintDoc?.SetPreviewPage(e.PageNumber, pageElement);
            }
        }

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            if (ViewModel == null) return;

            double printableHeight = PrintPageDesc.ImageableRect.Height;
            double contentHeight = ViewModel.Height;

            //同样使用带容差的计算方式
            int pageCount = (int)(contentHeight / printableHeight);
            if (contentHeight > printableHeight && (contentHeight % printableHeight) > 1.0)
            {
                pageCount++;
            }
            if (pageCount < 1) pageCount = 1;

            for (int i = 1; i <= pageCount; i++)
            {
                var pageElement = CreatePageForPrinting(i, PrintPageDesc);
                PrintDoc?.AddPage(pageElement);
            }
            PrintDoc?.AddPagesComplete();
        }

        /// <summary>
        /// 根据页码动态创建带有偏移和裁剪的单页 UI 元素
        /// </summary>
        private UIElement CreatePageForPrinting(int pageNumber, PrintPageDescription pageDesc)
        {
            if (ViewModel == null) return new Canvas();

            double printableHeight = pageDesc.ImageableRect.Height;
            double printableWidth = pageDesc.ImageableRect.Width;

            //计算当前页需要偏移的高度
            double offsetY = (pageNumber - 1) * printableHeight;

            //创建一个 Canvas 作为当前页的外壳，限定其尺寸为单张打印纸大小
            Canvas printPage = new Canvas
            {
                Width = pageDesc.PageSize.Width,
                Height = pageDesc.PageSize.Height
            };

            //使用 XamlReader 动态创建带有指定宽高 Grid 的 ItemsControl
            string itemsControlXaml = $@"
                <ItemsControl xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <ItemsControl.ItemsPanel>
                        <ItemsPanelTemplate>
                            <Grid Width=""{ViewModel.Width}"" Height=""{ViewModel.Height}""/>
                        </ItemsPanelTemplate>
                    </ItemsControl.ItemsPanel>
                </ItemsControl>";

            var printItemsControl = (ItemsControl)Microsoft.UI.Xaml.Markup.XamlReader.Load(itemsControlXaml);

            //绑定同一份数据源
            printItemsControl.ItemsSource = ViewModel.RenderElements;

            //复用页面中的模板选择器
            printItemsControl.ItemTemplateSelector = (ScoreRenderTemplateSelector)this.Resources["ScoreTemplateSelector"];

            //将动态生成的控件放入裁剪容器中
            printPage.Children.Add(printItemsControl);

            //设置偏移，让目标页的内容显示在可视区域内，并加上打印机要求的安全边距
            printItemsControl.RenderTransform = new TranslateTransform
            {
                X = pageDesc.ImageableRect.X,
                Y = pageDesc.ImageableRect.Y - offsetY
            };

            //使用矩形裁剪，防止其他页的内容溢出到当前页
            printPage.Clip = new RectangleGeometry
            {
                Rect = new Rect(pageDesc.ImageableRect.X, pageDesc.ImageableRect.Y, printableWidth, printableHeight)
            };

            return printPage;
        }

        private async Task PrintAsync()
        {
            try
            {
                if (PrintMan == null)
                {
                    Log.Error("打印执行: PrintMan 为 null，未注册打印管理器");
                    return;
                }

                if (PrintHwnd == IntPtr.Zero)
                {
                    Log.Error("打印执行: HWND 为零");
                    return;
                }

                if (!PrintManager.IsSupported())
                {
                    Log.Warning("打印执行: 系统不支持打印");
                    return;
                }

                Log.Information($"打印执行: 即将调用 ShowPrintUIForWindowAsync，HWND = {PrintHwnd}");

                //直接使用强类型调用，返回的就是标准的 IAsyncAction
                await PrintManagerInterop.ShowPrintUIForWindowAsync(PrintHwnd);

                Log.Information("打印执行: 打印 UI 调用成功");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "打印执行: 发生异常");
            }
        }

        /// <summary>
        /// 蒙板移动逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //监听 CurrentPlayingNote 的变化
            if (e.PropertyName == nameof(ViewModel.CurrentPlayingNote))
            {
                UpdateHighlightMask();
            }
        }

        /// <summary>
        /// 根据当前播放的音符，更新蒙板的位置和尺寸
        /// </summary>
        private void UpdateHighlightMask()
        {
            //确保在 UI 网格加载完成后操作
            if (ViewModel == null || HighlightMask == null) return;

            var note = ViewModel.CurrentPlayingNote;

            //1.如果没有播放音符，隐藏蒙板
            if (note == null || note.X == null || note.Y == null)
            {
                HighlightMask.Opacity = 0;
                return;
            }

            //2.设置蒙板尺寸
            //DrawHelper 计算的尺寸可能比较紧凑，我们可以稍微加一点 padding 视觉效果更好
            double width = note.Width ?? 20;
            double height = note.Height ?? 20;

            HighlightMask.Width = width;
            HighlightMask.Height = height;

            //3.设置蒙板位置
            //因为蒙板在 ScoreCanvas 内部的 Grid 中，坐标系是一致的 (0,0) 对齐
            //直接使用 Margin.Left 和 Margin.Top 进行绝对定位
            HighlightMask.Margin = new Thickness(note.X.Value, note.Y.Value, 0, 0);

            //4.显示蒙板
            HighlightMask.Opacity = 0.6;

            //强制刷新视觉层，防止快速播放时卡顿
            HighlightMask.InvalidateArrange();
        }

        private async void ScoreRegion_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //获取触发点击事件的原始源（通常是一个 UI 元素）
            if (e.OriginalSource is FrameworkElement fe)
            {
                //检查它的 DataContext 是哪种渲染元素
                if (fe.DataContext is ScoreRenderTextElement textElement)
                {
                    //如果是文本元素，拿到绑定的 Note 对象，调用 ViewModel 的方法
                    if (ViewModel != null && textElement != null && textElement.Note != null)
                    {
                        //调用 ViewModel 中的点击事件程序
                        await ViewModel.OnNoteTappedAsync(textElement.Note);
                    }
                }
                else if (fe.DataContext is ScoreRenderLineElement lineElement)
                {
                    //如果是线条元素，拿到绑定的 Note 对象，调用 ViewModel 的方法
                    if (ViewModel != null && lineElement != null && lineElement.Note != null)
                    {
                        //调用 ViewModel 中的点击事件程序
                        await ViewModel.OnNoteTappedAsync(lineElement.Note);
                    }
                }
            }
        }

    }
}
