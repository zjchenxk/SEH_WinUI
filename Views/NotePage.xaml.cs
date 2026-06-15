using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace SEH.Views
{
    public sealed partial class NotePage : Page
    {
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private StorageFile? noteFile = null;
        private string fileName = "note.txt";

        public NotePage()
        {
            InitializeComponent();

            Loaded += NotePage_Loaded;
        }

        private async void NotePage_Loaded(object sender, RoutedEventArgs e)
        {
            noteFile = (StorageFile)await storageFolder.TryGetItemAsync(fileName);
            if (noteFile is not null)
            {
                NoteEditor.Text = await FileIO.ReadTextAsync(noteFile);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (noteFile is null)
            {
                noteFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(noteFile, NoteEditor.Text);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (noteFile is not null)
            {
                await noteFile.DeleteAsync();
                noteFile = null;
                NoteEditor.Text = string.Empty;
            }
        }
    }
}
