using System;
using System.IO;
using System.Web;
using System.Windows;
using TKHiLoader.ViewModel;

namespace TKHiLoader.View
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private string _tempFile;

        public InfoWindow()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as InfoViewModel;

            if (vm != null)
            {
                _tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(vm.File));

                File.Copy(vm.File, _tempFile);

                wbView.Navigate($"file:///{HttpUtility.HtmlEncode(_tempFile)}");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_tempFile) && File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }
    }
}
