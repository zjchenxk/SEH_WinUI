using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace SEH.Models
{
    public class AllFile
    {
        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();

        public AllFile()
        {
            LoadFiles();
        }

        public async void LoadFiles()
        {
            Files.Clear();

            // Get the folder where the notes are stored.
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            await GetFilesInFolderAsync(storageFolder);
        }

        private async Task GetFilesInFolderAsync(StorageFolder folder)
        {
            // Each StorageItem can be either a folder or a file.
            IReadOnlyList<IStorageItem> storageItems = await folder.GetItemsAsync();
            foreach (IStorageItem storageItem in storageItems)
            {
                if (storageItem.IsOfType(StorageItemTypes.Folder))
                {
                    // Recursively get items from subfolders.
                    await GetFilesInFolderAsync((StorageFolder)storageItem);
                }
                else if (storageItem.IsOfType(StorageItemTypes.File))
                {
                    StorageFile storageFile = (StorageFile)storageItem;
                    File file = new File()
                    {
                        FileName = storageFile.Name,
                        Text = await FileIO.ReadTextAsync(storageFile),
                        Date = storageFile.DateCreated.DateTime
                    };
                    Files.Add(file);
                }
            }
        }

    }
}
