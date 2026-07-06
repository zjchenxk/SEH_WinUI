using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Printing;
using Newtonsoft.Json.Linq;
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
    /// 编辑简谱页面
    /// </summary>
    public sealed partial class EditScorePage : Page
    {
        public EditScoreViewModel? ViewModel { get; }

        private readonly IMessenger? Messenger;

        //打印相关变量
        private PrintManager? PrintMan;
        private PrintDocument? PrintDoc;
        private IPrintDocumentSource? PrintDocSource;
        private UIElement? PrintContainer; //要打印的 UI 元素
        private IntPtr PrintHwnd;


        public EditScorePage()
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = App.Services?.GetRequiredService<EditScoreViewModel>();
            //获取 Messenger 实例
            Messenger = App.Services?.GetRequiredService<IMessenger>();

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            #region 动态测量字符实际宽度
            TextBlock charBlock = new()
            {
                Text = "8",
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 22
            };
            charBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (ViewModel != null)
            {
                ViewModel.CharWidth = charBlock.ActualWidth;
            }
            #endregion

            this.DataContext = ViewModel;

            this.Loaded += EditScorePage_Loaded;

            //获取简谱画布作为打印容器
            PrintContainer = ScoreCanvas;

            //关键：在 UI 线程提前创建 PrintDocument
            PrintDoc = new PrintDocument();
            PrintDocSource = PrintDoc.DocumentSource;
            PrintDoc.Paginate += Paginate;
            PrintDoc.GetPreviewPage += GetPreviewPage;
            PrintDoc.AddPages += AddPages;
        }

        private void EditScorePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //从 ViewModel 获取 DialogService 并设置 XamlRoot
            //注意：这里可以通过 App.Services 直接获取服务实例
            var dialogService = App.Services?.GetRequiredService<IDialogService>();
            if (dialogService != null && dialogService is SEH.Services.DialogService ds)
            {
                ds.XamlRoot = this.XamlRoot;
            }
        }

        protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                //将参数传给 ViewModel 进行初始化
                ViewModel?.Initialize((JObject)e.Parameter);
            }

            if (App.MainWindow == null) return;

            PrintHwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            Log.Information($"打印准备: HWND = {PrintHwnd}, 是否为零 = {PrintHwnd == IntPtr.Zero}");

            //直接使用强类型调用，不再需要反射
            try
            {
                PrintMan = PrintManagerInterop.GetForWindow(PrintHwnd);
                if (PrintMan != null)
                {
                    PrintMan.PrintTaskRequested += PrintTaskRequested;
                    Log.Information("PrintManager 事件注册成功");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "PrintManager 注册失败");
            }

            Messenger?.Register<PrintScoreMessage>(this, (r, m) => { _ = PrintAsync(); });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (PrintMan != null)
            {
                PrintMan.PrintTaskRequested -= PrintTaskRequested;
            }

            //使用 DI 获取的实例注销
            Messenger?.Unregister<PrintScoreMessage>(this);
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
            PrintDoc?.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            if (PrintContainer != null)
            {
                PrintDoc?.SetPreviewPage(e.PageNumber, PrintContainer);
            }
        }

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            if (PrintContainer != null)
            {
                PrintDoc?.AddPage(PrintContainer);
            }
            PrintDoc?.AddPagesComplete();
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

        private async void TextBlock_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //sender 是触发事件的 UI 控件 (即 TextBlock)
            if (sender is Microsoft.UI.Xaml.FrameworkElement element && element.DataContext is ScoreRenderTextElement textElement)
            {
                //调用 ViewModel 中的方法
                if (ViewModel != null)
                {
                    await ViewModel.OnNoteTappedAsync(textElement);
                }
            }
        }
    }
}
