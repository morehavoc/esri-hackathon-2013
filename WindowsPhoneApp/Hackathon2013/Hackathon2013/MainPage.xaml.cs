using System.Windows;
using Microsoft.Phone.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Device.Location;
using ESRI.ArcGIS.Client.Geometry;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.Symbols;
using System;
using System.Linq;
using ESRI.ArcGIS.Client.FeatureService;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;



namespace Hackathon2013
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        FeatureLayer featureLayer;
        Editor editor;
        public MainPage()
        {
            InitializeComponent();
            this.MainViewModel = LayoutRoot.Resources["MainViewModel"] as MainViewModel;
            editor = LayoutRoot.Resources["MyEditor"] as Editor;
            featureLayer = Map.Layers["AnimalSighting"] as FeatureLayer;
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

        private void RouteButton_Click(object sender, System.EventArgs e)
        {
            Popup popup = new Popup();
            popup.Height = 300;
            popup.Width = 400;
            popup.VerticalOffset = 100;
            AddressInputControl control = new AddressInputControl();
            control.ThrowEvent += new AddressInputControl.EventHandler(control_ThrowEvent);
            popup.Child = control;
            popup.IsOpen = true;
        }

        void control_ThrowEvent(object sender, System.EventArgs args)
        {
            GetCurrentLocation();
            GetEndLocation();
        }

        public void GetCurrentLocation()
        {
            //GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            //watcher.Start();
            //MapPoint currentLocation = new MapPoint(watcher.Position.Location.Longitude, watcher.Position.Location.Latitude);
            //watcher.Stop();
            MapPoint currentLocation = new MapPoint(-12973020.95, 4005240.84, new SpatialReference(3857));
            GraphicsLayer stopsGraphicsLayer = Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            if (stopsGraphicsLayer.Graphics != null)
                stopsGraphicsLayer.Graphics.Clear();
            Graphic stop = new Graphic() { Geometry = currentLocation, Symbol = LayoutRoot.Resources["StopSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol };
            stopsGraphicsLayer.Graphics.Add(stop);

            Map.ZoomTo(new Envelope(currentLocation.X - 1000, currentLocation.Y - 1000, currentLocation.X + 1000, currentLocation.Y + 1000));

        }

        public void GetEndLocation()
        {
            Locator locatorTask = new Locator("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Locators/ESRI_Geocode_USA/GeocodeServer");
            locatorTask.AddressToLocationsCompleted += LocatorTask_AddressToLocationsCompleted;

            // Initialize the address.
            AddressToLocationsParameters addressParameters = new AddressToLocationsParameters();
            Dictionary<string, string> address = addressParameters.Address;
            addressParameters.OutSpatialReference = new SpatialReference(3857);
            string[] addressArray = _mainViewModel.EndLocation.Split(',');
            address.Add("Address", addressArray[0]);
            address.Add("City", addressArray[1]);
            address.Add("State", addressArray[2]);
            address.Add("Zip", addressArray[3]);

            // Call method to locate the address.
            locatorTask.AddressToLocationsAsync(addressParameters);
        }

        private void LocatorTask_AddressToLocationsCompleted(object sender, ESRI.ArcGIS.Client.Tasks.AddressToLocationsEventArgs args)
        {
            if (args.Results.Count > 0)
            {
                AddressCandidate bestCandidate = args.Results[0];

                MapPoint endLocation = new MapPoint(bestCandidate.Location.X, bestCandidate.Location.Y, new SpatialReference(3857));
                GraphicsLayer stopsGraphicsLayer = Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
                Graphic stop = new Graphic() { Geometry = endLocation, Symbol = LayoutRoot.Resources["StopSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol };
                stopsGraphicsLayer.Graphics.Add(stop);
                Route();
                NavigateSampleRoute();
            }
        }
        public void Route()
        {
            GraphicsLayer stopsGraphicsLayer = Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            RouteTask routeTask = LayoutRoot.Resources["MyRouteTask"] as RouteTask;
            routeTask.SolveAsync(new RouteParameters() { Stops = stopsGraphicsLayer, UseTimeWindows = false, ReturnDirections = true });
        }

        private void MyRouteTask_Failed(object sender, TaskFailedEventArgs e)
        {
            string errorMessage = "Routing error: ";
            errorMessage += e.Error.Message;
            foreach (string detail in (e.Error as ServiceException).Details)
                errorMessage += "," + detail;

            MessageBox.Show(errorMessage);
            GraphicsLayer stopsGraphicsLayer = Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            stopsGraphicsLayer.Graphics.RemoveAt(stopsGraphicsLayer.Graphics.Count - 1);
        }

        FeatureSet directionsFeatureSet = new FeatureSet();
        List<Graphic> directionsList = new List<Graphic>();
        private void MyRouteTask_SolveCompleted(object sender, RouteEventArgs e)
        {
            GraphicsLayer routeGraphicsLayer = Map.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
            routeGraphicsLayer.Graphics.Clear();

            RouteResult routeResult = e.RouteResults[0];
            routeResult.Route.Symbol = LayoutRoot.Resources["RouteSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            directionsFeatureSet = routeResult.Directions;
            foreach (Graphic direction in routeResult.Directions.Features)
            {
                directionsList.Add(direction);
            }

            Graphic lastRoute = routeResult.Route;

            decimal totalTime = (decimal)lastRoute.Attributes["Total_Time"];
            string tip = string.Format("Total Time: {0} minutes", totalTime.ToString("#0.00"));
            TimeText.Text = tip;

            routeGraphicsLayer.Graphics.Add(lastRoute);
        }

        public void NavigateSampleRoute()
        {
            QueryTask queryTask = new QueryTask("http://services.arcgis.com/JbibFpsuaEQr3FFG/arcgis/rest/services/drive_path_points5/FeatureServer/0");
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);

            Query query = new Query();
            query.ReturnGeometry = true;
            query.OutFields.Add("Directions");
            query.Where = "1=1";

            queryTask.ExecuteAsync(query);
        }

        void queryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Query failed: " + e.Error);
        }

        List<Graphic> routePoints = new List<Graphic>();
        DispatcherTimer newTimer;
        void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            FeatureSet featureSet = e.FeatureSet;
            if (featureSet.Features.Count > 0)
            {
                foreach (Graphic resultFeature in featureSet.Features)
                {
                    routePoints.Add(resultFeature);
                }
            }
            newTimer = new DispatcherTimer();
            newTimer.Interval = TimeSpan.FromSeconds(1);
            newTimer.Tick += OnTimerTick;
            newTimer.Start();
        }

        int i = 0;
        void OnTimerTick(Object sender, EventArgs args)
        {
            SetCurrectLocation(routePoints[i]);

            if (routePoints[i].Attributes["Directions"] != null && routePoints[i].Attributes["Directions"] != TimeText.Text)
                TimeText.Text = routePoints[i].Attributes["Directions"].ToString();

            QueryTask intersectQueryTask = new QueryTask("http://services.arcgis.com/JbibFpsuaEQr3FFG/arcgis/rest/services/animal_polygons3/FeatureServer/0");
            intersectQueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(intersectQueryTask_ExecuteCompleted);
            intersectQueryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);
            Query query = new Query();
            query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            query.Geometry = currentLocation.Geometry;
            intersectQueryTask.ExecuteAsync(query);


        }

        void directionsQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        Graphic currentLocation = null;
        public void SetCurrectLocation(Graphic g)
        {
            currentLocation = g;
            g.Symbol = LayoutRoot.Resources["StrobeMarkerSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            GraphicsLayer navGraphicsLayer = Map.Layers["MyNavigationGraphicsLayer"] as GraphicsLayer;
            navGraphicsLayer.Graphics.Clear();
            navGraphicsLayer.Graphics.Add(g);
            Map.PanTo(g.Geometry);
        }

        public bool inZone = false;
        void intersectQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            FeatureSet fs = e.FeatureSet;
            if (fs.Count() != 0)
            {
                if (inZone == false)
                {
                    inZone = true;
                    var stream = TitleContainer.OpenStream("Sounds/x1.wav");
                    var effect = SoundEffect.FromStream(stream);
                    FrameworkDispatcher.Update();
                    effect.Play();
                }

            }
            else
            {
                if (inZone == true)
                    inZone = false;
            }
            i++;
        } 

        public void FeatureLayer_Initialized(object sender, EventArgs e)
        {
            //hard coded values because feature class did not return them in the correct order
            List<string> animalSizes = new List<string>();
            animalSizes.Add("Extra Large");
            animalSizes.Add("Large");
            animalSizes.Add("Medium");
            animalSizes.Add("Small");
            animalSizes.Add("Extra Small"); 
            FeatureLayer fl = sender as FeatureLayer;

            buttonsStackPanel.Children.Clear();
            foreach (string name in animalSizes)
            {
                Button button = new Button();
                button.Content = GetIconImage(name);
                button.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                button.Opacity = .6;
                button.Width = 110;
                button.Height = 110;
                button.Name = name;
                button.Click += new RoutedEventHandler(button_Click);
                buttonsStackPanel.Children.Add(button);
            }

            IDictionary<object, FeatureType> featureTypes = fl.LayerInfo.FeatureTypes;
            if (fl.Renderer != null)
            {
                Symbol defaultSymbol = fl.Renderer.GetSymbol(null);
                if (featureTypes != null && featureTypes.Count > 0)
                {
                    foreach (KeyValuePair<object, FeatureType> featureTypePairs in featureTypes)
                    {
                        if (featureTypePairs.Value != null && featureTypePairs.Value.Templates != null && featureTypePairs.Value.Templates.Count > 0)
                        {

                            foreach (KeyValuePair<string, FeatureTemplate> featureTemplate in featureTypePairs.Value.Templates)
                            {
                                string name = featureTypePairs.Value.Name;
                                if (featureTypePairs.Value.Templates.Count > 1)
                                    name = string.Format("{0}-{1}", featureTypePairs.Value.Name, featureTemplate.Value.Name);
                            }
                        }
                    }
                }
            }
        }

        public Image GetIconImage(string name)
        {
            Image image = new Image();
            
            switch (name)
            {
                case "Extra Large":
                    image.Source= new BitmapImage(new Uri("Images/bear.png", UriKind.Relative));
                    break;
                case "Large":
                    image.Source= new BitmapImage(new Uri("Images/deer.png", UriKind.Relative));
                    break;
                case "Medium":
                    image.Source= new BitmapImage(new Uri("Images/dog.png", UriKind.Relative));
                    break;
                case "Small":
                    image.Source = new BitmapImage(new Uri("Images/cat.png", UriKind.Relative));
                    break;
                case "Extra Small":
                    image.Source = new BitmapImage(new Uri("Images/squirrel.png", UriKind.Relative));
                    break;
            }
            return image;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Graphic g = new Graphic();
            g.Geometry = currentLocation.Geometry;
            g.Attributes.Add("size", button.Name);
            featureLayer.Graphics.Add(g);
            featureLayer.SaveEdits();           
            FeatureTypeChoicesPage.IsOpen = false;
        }

        private void Menu_List_Clear(object sender, System.EventArgs e)
        {
            GraphicsLayer stopsGraphicsLayer = Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            stopsGraphicsLayer.Graphics.Clear();

            GraphicsLayer routeGraphicsLayer = Map.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
            routeGraphicsLayer.Graphics.Clear();

            newTimer.Stop();
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

        private void AddItemButton_Click(object sender, System.EventArgs e)
        {
            FeatureTypeChoicesPage.IsOpen = true;
        }
         
        private void CloseEditor_Click(object sender, RoutedEventArgs e)
        {
            FeatureTypeChoicesPage.IsOpen = false;
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            GetCurrentLocation();
        }
    }
  
}
