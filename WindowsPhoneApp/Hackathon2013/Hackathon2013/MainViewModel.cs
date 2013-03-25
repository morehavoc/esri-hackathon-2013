using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.ComponentModel;
using System.Device;
using System.Device.Location;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
namespace Hackathon2013
{
    public class MainViewModel :DependencyObject, INotifyPropertyChanged
    {

        public static readonly DependencyProperty StartLocationProperty = DependencyProperty.Register("StartLocation", typeof(string), typeof(MainViewModel), new PropertyMetadata("Current Location", null ));
        public string StartLocation
        {
            get
            {
                return (string)GetValue(StartLocationProperty);
            }
            set
            {
                SetValue(StartLocationProperty, value);
            }
        }

        
        public static readonly DependencyProperty EndLocationProperty = DependencyProperty.Register("EndLocation", typeof(string), typeof(MainViewModel), new PropertyMetadata("3900 Main St, Riverside, CA, 92501", null ));
        public string EndLocation
        {
            get
            {
                return (string)GetValue(EndLocationProperty);
            }
            set
            {
                SetValue(EndLocationProperty, value);
            }
        }

        bool routeMap = false;

        public bool RouteMap
        {
            get { return routeMap; }
            set { routeMap = value;
            OnPropertyChanged("RouteMap");
            }
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
    

        //public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(ESRI.ArcGIS.Client.Map), typeof(MainViewModel), null);
        //public ESRI.ArcGIS.Client.Map Map
        //{
        //    get
        //    {
        //        return (ESRI.ArcGIS.Client.Map)GetValue(MapProperty);
        //    }
        //    set
        //    {
        //        SetValue(MapProperty, value);
        //    }
        //}

        

    }
}
