using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SEH.Models
{
    public class File
    {
        private StorageFolder StorageFolder = ApplicationData.Current.LocalFolder;
        public string FileName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;


        public File()
        {
            FileName = "notes" + DateTime.Now.ToBinary().ToString() + ".txt";
        }

        public async Task SaveAsync()
        {
            StorageFile storageFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
            if (storageFile is null)
            {
                storageFile = await StorageFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(storageFile, Text);
        }

        public async Task DeleteAsync()
        {
            StorageFile storageFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
            if (storageFile is not null)
            {
                await storageFile.DeleteAsync();
            }
        }
    }
}
