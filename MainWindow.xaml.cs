using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace SEH
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        //public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            //this.ViewModel = new MainViewModel();

            InitializeComponent();

            //AppWindow.SetIcon("Assets/口琴.ico");
            //AppWindow.SetTaskbarIcon("Assets/口琴.ico");

            //AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;


            //隐藏系统默认标题栏
            ExtendsContentIntoTitleBar = true;

            //用自定义的标题栏替换系统默认标题栏
            SetTitleBar(AppTitleBar);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = true;
            presenter.IsMinimizable = true;
            presenter.IsMaximizable = true;
            AppWindow.SetPresenter(presenter);

            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            //IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            //AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            //appWindow.SetIcon("Assets/口琴.ico");
            //appWindow.SetTaskbarIcon("Assets/口琴.ico");

            OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
            presenter.Maximize();
        }

        private void AppTitleBar_BackRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args)
        {
            if (rootFrame.CanGoBack == true)
            {
                rootFrame.GoBack();
            }
        }
    }
}
