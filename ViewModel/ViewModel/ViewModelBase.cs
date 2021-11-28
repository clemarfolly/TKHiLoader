using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TKHiLoader.ViewModel
{
    public class ViewModelBase :  IDisposable, INotifyPropertyChanged
    {
        private static IWindowManager _windowManager;

        public bool Disposed { get; private set; }

        public IWindowManager WindowManager
        {
            get
            {
                return _windowManager;
            }
            set
            {
                _windowManager = value;
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
            Disposed = true;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
