using Microsoft.UI.Xaml;

namespace SEH.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainWindow : Window
    {
        //public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            //this.ViewModel = new MainViewModel();

            InitializeComponent();

            //隐藏系统默认标题栏
            ExtendsContentIntoTitleBar = true;

            //用自定义的标题栏替换系统默认标题栏
            SetTitleBar(AppTitleBar);

        }
    }
}
