using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TKHiLoader.Common;

namespace TKHiLoader.Common
{
    public class MenuItem
    {
        private ICommand _command;

        public MenuItem()
        {
        }

        public string Header { get; set; }

        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public ICommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
            }
        }
    }
}
