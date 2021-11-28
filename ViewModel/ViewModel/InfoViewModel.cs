using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TKHiLoader.Common;

namespace TKHiLoader.ViewModel
{
    public class InfoViewModel: ViewModelBase
    {
        private string _file;
        private ICommand _closeCommand;

        public string File
        {
            get
            {
                return _file;
            }
        }

        public string WindowTitle { get; set; }
        public string CloseButtonText { get; set; }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((o) => { return true; }, (o) => { WindowManager.CloseWindow(this); });
                }
                return _closeCommand;
            }
        }

        public InfoViewModel(string windowTitle, string file)
        {
            _file = file;
            WindowTitle = windowTitle;
            CloseButtonText = "Fechar";

        }
    }
}
