using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace SEH
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ExplorerItem> DataSource { get; set; }


        public MainWindow()
        {
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

            DataSource = GetData();
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
            presenter.Maximize();
        }

        private ObservableCollection<ExplorerItem> GetData()
        {
            return new ObservableCollection<ExplorerItem>
            {
                new ExplorerItem
                {
                    Name = "Documents",
                    Type = ExplorerItem.ExplorerItemType.Folder,
                    Children =
                    {
                        new ExplorerItem
                        {
                            Name = "ProjectProposal",
                            Type = ExplorerItem.ExplorerItemType.File,
                        },
                        new ExplorerItem
                        {
                            Name = "BudgetReport",
                            Type = ExplorerItem.ExplorerItemType.File,
                        },
                    },
                },
                new ExplorerItem
                {
                    Name = "Projects",
                    Type = ExplorerItem.ExplorerItemType.Folder,
                    Children =
                    {
                        new ExplorerItem
                        {
                            Name = "Project Plan",
                            Type = ExplorerItem.ExplorerItemType.File,
                        },
                    },
                },
            };
        }

    }


    public class ExplorerItem
    {
        public enum ExplorerItemType
        {
            Folder,
            File,
        }

        public string Name { get; set; } = string.Empty;
        public ExplorerItemType Type { get; set; }
        public ObservableCollection<ExplorerItem> Children { get; set; } = new ObservableCollection<ExplorerItem>();
    }

    class ExplorerItemTemplateSelector : DataTemplateSelector
    {
        // Template to use for folder items in the TreeView.
        public DataTemplate? FolderTemplate { get; set; }

        // Template to use for file items in the TreeView.
        public DataTemplate? FileTemplate { get; set; }

        // Determines which template to use for each item in the TreeView based on its type.
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            // Cast the item to the ExplorerItem type.
            var explorerItem = (ExplorerItem)item;

            // Return the appropriate template: FolderTemplate for folders, FileTemplate for files.
            return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder
                ? FolderTemplate
                : FileTemplate;
        }
    }


}
