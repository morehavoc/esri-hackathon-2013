﻿#pragma checksum "C:\Users\codergrl\Documents\Visual Studio 2010\Projects\Hackathon2013\Hackathon2013\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "56EE211BF8C92DF3920D2BA9544EBB85"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Hackathon2013 {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton RouteButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AddItem;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal ESRI.ArcGIS.Client.Tasks.RouteTask MyRouteTask;
        
        internal System.Windows.Controls.Grid ContentGrid;
        
        internal ESRI.ArcGIS.Client.Map Map;
        
        internal ESRI.ArcGIS.Client.Toolkit.Primitives.ChildPage FeatureTypeChoicesPage;
        
        internal System.Windows.Controls.StackPanel buttonsStackPanel;
        
        internal System.Windows.Controls.Button CloseEditor;
        
        internal System.Windows.Controls.Grid InfoGrid;
        
        internal System.Windows.Controls.TextBlock TimeText;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Hackathon2013;component/MainPage.xaml", System.UriKind.Relative));
            this.RouteButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("RouteButton")));
            this.AddItem = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AddItem")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MyRouteTask = ((ESRI.ArcGIS.Client.Tasks.RouteTask)(this.FindName("MyRouteTask")));
            this.ContentGrid = ((System.Windows.Controls.Grid)(this.FindName("ContentGrid")));
            this.Map = ((ESRI.ArcGIS.Client.Map)(this.FindName("Map")));
            this.FeatureTypeChoicesPage = ((ESRI.ArcGIS.Client.Toolkit.Primitives.ChildPage)(this.FindName("FeatureTypeChoicesPage")));
            this.buttonsStackPanel = ((System.Windows.Controls.StackPanel)(this.FindName("buttonsStackPanel")));
            this.CloseEditor = ((System.Windows.Controls.Button)(this.FindName("CloseEditor")));
            this.InfoGrid = ((System.Windows.Controls.Grid)(this.FindName("InfoGrid")));
            this.TimeText = ((System.Windows.Controls.TextBlock)(this.FindName("TimeText")));
        }
    }
}

