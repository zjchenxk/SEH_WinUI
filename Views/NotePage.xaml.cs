using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SEH.Models;
using System;
using Windows.Storage;

namespace SEH.Views
{
    public sealed partial class NotePage : Page
    {
        private File model = new File();

        public NotePage()
        {
            InitializeComponent();
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (model is not null)
            {
                await model.SaveAsync();
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (model is not null)
            {
                await model.DeleteAsync();
            }
        }
    }
}
