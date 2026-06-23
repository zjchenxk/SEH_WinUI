using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SEH.Services;
using SEH.ViewModels;

namespace SEH
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }
        private readonly INavigationService NavigationService;

        public MainWindow(MainViewModel viewModel, INavigationService navigationService)
        {
            //将依赖注入的 ViewModel 注入到 MainWindow 中
            ViewModel = viewModel;
            ViewModel.LoadScoreItems();

            //初始化组件（XAML UI 元素）
            InitializeComponent();

            //将依赖注入的 NavigationService 注入到 MainWindow 中
            //一定要放在 InitializeComponent() 之后，否则导航服务可能无法正确初始化
            NavigationService = navigationService;
            NavigationService.Initialize(rootFrame);

            //隐藏系统默认标题栏
            ExtendsContentIntoTitleBar = true;

            //用自定义的标题栏替换系统默认标题栏
            SetTitleBar(AppTitleBar);

            //实现窗口最大化、最小化、关闭按钮的功能
            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = true;
            presenter.IsMinimizable = true;
            presenter.IsMaximizable = true;
            AppWindow.SetPresenter(presenter);

            //注册窗口激活事件，当窗口被激活时自动最大化
            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
            presenter.Maximize();
        }

    }
}
