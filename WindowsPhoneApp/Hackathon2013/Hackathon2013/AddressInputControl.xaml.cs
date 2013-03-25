using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace Hackathon2013
{
    public partial class AddressInputControl : UserControl, INotifyPropertyChanged
    {
        public AddressInputControl()
        {
            InitializeComponent();
            this.MainViewModel = LayoutRoot.Resources["MainViewModel"] as MainViewModel;
        }

        private MainViewModel _mainViewModel = null;
        public MainViewModel MainViewModel
        {
            get
            {
                return _mainViewModel;
            }
            private set
            {
                _mainViewModel = value;
                OnPropertyChanged("MainViewModel");
            }
        }

        public delegate void EventHandler(object sender, EventArgs args);
        public event EventHandler ThrowEvent = delegate { };

        private void GetDirectionsButton_Click(object sender, RoutedEventArgs e)
        {
            Popup popup = this.Parent as Popup;
            popup.IsOpen = false;
            ThrowEvent(this, new EventArgs());
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    
    }
}
