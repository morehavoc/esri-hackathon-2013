using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ESRI.ArcGIS.OperationsDashboard;
using client = ESRI.ArcGIS.Client;

namespace OperationsDashboardAddIn2
{
    /// <summary>
    /// A Widget is a dockable add-in class for Operations Dashboard for ArcGIS that implements IWidget. By returning true from CanConfigure, 
    /// this widget provides the ability for the user to configure the widget properties showing a settings Window in the Configure method.
    /// By implementing IDataSourceConsumer, this Widget indicates it requires a DataSource to function and will be notified when the 
    /// data source is updated or removed.
    /// </summary>
    [Export("ESRI.ArcGIS.OperationsDashboard.Widget")]
    [ExportMetadata("DisplayName", "OperationsDashboardAddIn2 SightingsWidget")]
    [ExportMetadata("Description", "OperationsDashboardAddIn2 SightingsWidget description")]
    [ExportMetadata("ImagePath", "/OperationsDashboardAddIn2;component/Images/Widget32.png")]
    [ExportMetadata("DataSourceRequired", true)]
    [DataContract]
    public partial class SightingsWidget : UserControl, IWidget, IDataSourceConsumer
    {
        /// <summary>
        /// A unique identifier of a data source in the configuration. This property is set during widget configuration.
        /// </summary>
        [DataMember(Name = "dataSourceId")]
        public string DataSourceId { get; set; }

        /// <summary>
        /// The name of a field within the selected data source. This property is set during widget configuration.
        /// </summary>
        [DataMember(Name = "field")]
        public string Field { get; set; }

        public int eventMouse = 0;
        ESRI.ArcGIS.Client.FeatureLayer globalFeatureLayer = new client.FeatureLayer();
        ESRI.ArcGIS.OperationsDashboard.MapWidget globalMapWidget;// = new ESRI.ArcGIS.OperationsDashboard.MapWidget();
                        
        public SightingsWidget()
        {
            InitializeComponent();
        }

        private void UpdateControls()
        {
            DataSourceBox.Text = DataSourceId;
            FieldBox.Text = Field;
        }

        #region IWidget Members

        private string _caption = "Default Caption";
        /// <summary>
        /// The text that is displayed in the widget's containing window title bar. This property is set during widget configuration.
        /// </summary>
        [DataMember(Name = "caption")]
        public string Caption
        {
            get
            {
                return _caption;
            }

            set
            {
                if (value != _caption)
                {
                    _caption = value;
                }
            }
        }

        /// <summary>
        /// The unique identifier of the widget, set by the application when the widget is added to the configuration.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// OnActivated is called when the widget is first added to the configuration, or when loading from a saved configuration, after all 
        /// widgets have been restored. Saved properties can be retrieved, including properties from other widgets.
        /// Note that some widgets may have properties which are set asynchronously and are not yet available.
        /// </summary>
        public void OnActivated()
        {
            UpdateControls();
        }

        /// <summary>
        ///  OnDeactivated is called before the widget is removed from the configuration.
        /// </summary>
        public void OnDeactivated()
        {
        }

        /// <summary>
        ///  Determines if the Configure method is called after the widget is created, before it is added to the configuration. Provides an opportunity to gather user-defined settings.
        /// </summary>
        /// <value>Return true if the Configure method should be called, otherwise return false.</value>
        public bool CanConfigure
        {
            get { return true; }
        }

        /// <summary>
        ///  Provides functionality for the widget to be configured by the end user through a dialog.
        /// </summary>
        /// <param name="owner">The application window which should be the owner of the dialog.</param>
        /// <param name="dataSources">The complete list of DataSources in the configuration.</param>
        /// <returns>True if the user clicks ok, otherwise false.</returns>
        public bool Configure(Window owner, IList<DataSource> dataSources)
        {
            // Show the configuration dialog.
            Config.SightingsWidgetDialog dialog = new Config.SightingsWidgetDialog(dataSources, Caption, DataSourceId, Field) { Owner = owner };
            if (dialog.ShowDialog() != true)
                return false;

            // Retrieve the selected values for the properties from the configuration dialog.
            Caption = dialog.Caption;
            DataSourceId = dialog.DataSource.Id;
            Field = dialog.Field.Name;
            //sizeComboBox.IsEnabled = false;
            btnSaveSighting.IsEnabled = false;

            // The default UI simply shows the values of the configured properties.
            UpdateControls();

            return true;

        }


        #endregion

        #region IDataSourceConsumer Members

        /// <summary>
        /// Returns the ID(s) of the data source(s) consumed by the widget.
        /// </summary>
        public string[] DataSourceIds
        {
            get { return new string[] { DataSourceId }; }
        }

        /// <summary>
        /// Called when a DataSource is removed from the configuration. 
        /// </summary>
        /// <param name="dataSource">The DataSource being removed.</param>
        public void OnRemove(DataSource dataSource)
        {
            // Respond to data source being removed.
            DataSourceId = null;
        }

        /// <summary>
        /// Called when a DataSource found in the DataSourceIds property is updated.
        /// </summary>
        /// <param name="dataSource">The DataSource being updated.</param>
        public void OnRefresh(DataSource dataSource)
        {
            // If required, respond to the update from the selected data source.
            // Consider using an async method.
        }

        #endregion

        private void btnSaveSighting_Click(object sender, RoutedEventArgs e)
        {
            btnAddSighting.IsEnabled = true;
            btnSaveSighting.IsEnabled = false;

        }
        private void btnAddSighting_Click(object sender, RoutedEventArgs e)
        {
            btnAddSighting.IsEnabled = false;
            btnSaveSighting.IsEnabled = true;
            ESRI.ArcGIS.OperationsDashboard.MapWidget mapWidget;
            
            DataSource answer=null;
            
            foreach (DataSource dS in OperationsDashboard.Instance.DataSources)
            {
                if (dS.Name.ToString() == "animal_sightings - animal_sightings")
                {
                    answer = dS;
                }
            }
            if (answer != null)
            {
                mapWidget = (ESRI.ArcGIS.OperationsDashboard.MapWidget)OperationsDashboard.Instance.FindWidget(answer);
                globalMapWidget = mapWidget;
                globalFeatureLayer = mapWidget.FindFeatureLayer(answer);
               
                if (this.eventMouse == 0)
                {
                    mapWidget.Map.MouseClick += Map_MouseClick;
                    this.eventMouse = 1;
                }
            }
        }

        void Map_MouseClick(object sender, client.Map.MouseEventArgs e)
        {
            if (sizeComboBox.Text=="Select Size")
                MessageBox.Show("Please select a size of animal before adding to the map.");
            if (btnAddSighting.IsEnabled == false && sizeComboBox.Text!="Select Size")
            {
                ESRI.ArcGIS.Client.Graphic graphicFeature = new client.Graphic();
                ESRI.ArcGIS.Client.FeatureLayer featureLayer = new client.FeatureLayer();
                featureLayer = this.globalFeatureLayer;
                ESRI.ArcGIS.Client.Geometry.MapPoint point = new client.Geometry.MapPoint();
                point = e.MapPoint;
                graphicFeature.Geometry=point;
                graphicFeature.Attributes.Add("size", sizeComboBox.Text.ToString());
                featureLayer.Graphics.Add(graphicFeature);
                featureLayer.SaveEdits();
                MessageBox.Show("Successfully added sighting!");
                ///Thread.Sleep(10000);
                //MessageBox.Show("finished");
                //featureLayer.Update();
                //featureLayer.Refresh();
                //globalMapWidget.Map.UpdateLayout();
                
            }
        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSaveSighting.IsEnabled = true;
        }
    }
}
