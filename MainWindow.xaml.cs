using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SEH.ViewModels;

namespace SEH
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            ViewModel.LoadScoreItems();

            InitializeComponent();

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
            OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
            presenter.Maximize();
        }

    }
}
