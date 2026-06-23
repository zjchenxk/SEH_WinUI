using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SEH.Commons
{
    public class ScoreItemTemplateSelector : DataTemplateSelector
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
