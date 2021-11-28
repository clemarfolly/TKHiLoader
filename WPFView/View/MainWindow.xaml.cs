using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TKHiLoader.ViewModel;

namespace TKHiLoader.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var vm = new MainWindowViewModel();
            vm.WindowManager = new WindowManager();
            this.DataContext = vm;
            InitializeComponent();

            //while (sw.ElapsedMilliseconds < 2000)
            //    System.Threading.Thread.Sleep(100);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var vm = this.DataContext as ViewModelBase;
            vm?.Dispose();
        }
    }
}
