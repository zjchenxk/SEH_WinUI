using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SEH.Database;
using System.Collections.Generic;

namespace SEH.ViewModels
{
    /// <summary>
    /// 主窗体的视图模型，负责管理界面数据和逻辑
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 简谱列表
        /// [ObservableProperty] 特性会自动生成一个名为 ScoreItems 的公共属性和对应的属性变更通知代码，只需要维护私有字段就够了
        /// </summary>
        [ObservableProperty]
        private List<ScoreItem>? _scoreItems = null;

        /// <summary>
        /// 新增类别命令
        /// [RelayCommand] 特性会自动生成一个名为 NewCategoryCommand 的公共命令属性。这个方法会在按钮被点击时执行
        /// </summary>
        [RelayCommand]
        private void NewCategory()
        {
            //rootFrame.Navigate(typeof(MainViewModel));
        }

        public MainViewModel()
        {
            _scoreItems = [];
        }

        /// <summary>
        /// 读取简谱列表
        /// </summary>
        public void LoadScoreItems()
        {
            var repository = new Repository();
            var categories = repository.GetCategories();
            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    var scoreItem = new ScoreItem
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Type = ScoreItem.ScoreItemType.Folder,
                    };

                    var scores = repository.GetScoresByCategoryId(category.Id);
                    if (scores != null && scores.Count > 0)
                    {
                        foreach (var score in scores)
                        {
                            scoreItem.Children.Add(new ScoreItem
                            {
                                Id = score.Id,
                                Name = score.Title,
                                Type = ScoreItem.ScoreItemType.File,
                            });
                        }
                    }

                    ScoreItems.Add(scoreItem);
                }
            }
        }

    }

    /// <summary>
    /// 简谱树形列表节点类
    /// </summary>
    public class ScoreItem
    {
        public enum ScoreItemType
        {
            Folder,
            File,
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ScoreItemType Type { get; set; }
        public List<ScoreItem> Children { get; set; } = new List<ScoreItem>();
    }

    /// <summary>
    /// 简谱树形列表节点选择器类
    /// </summary>
    class ScoreItemTemplateSelector : DataTemplateSelector
    {
        // Template to use for folder items in the TreeView.
        public DataTemplate? FolderTemplate { get; set; }

        // Template to use for file items in the TreeView.
        public DataTemplate? FileTemplate { get; set; }

        // Determines which template to use for each item in the TreeView based on its type.
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            // Cast the item to the ExplorerItem type.
            var explorerItem = (ScoreItem)item;

            // Return the appropriate template: FolderTemplate for folders, FileTemplate for files.
            return explorerItem.Type == ScoreItem.ScoreItemType.Folder
                ? FolderTemplate
                : FileTemplate;
        }
    }
}
