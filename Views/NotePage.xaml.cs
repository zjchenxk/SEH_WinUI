using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SEH.Models;

namespace SEH.Views
{
    public sealed partial class NotePage : Page
    {
        private File? model;

        public NotePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is File file)
            {
                model = file;
            }
            else
            {
                model = new File();
            }
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

            if (Frame.CanGoBack == true)
            {
                Frame.GoBack();
            }
        }
    }
}
