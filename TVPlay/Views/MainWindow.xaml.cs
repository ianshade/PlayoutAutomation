﻿using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Diagnostics;
using TAS.Server;
using TAS.Client.Common;
using TAS.Client.ViewModels;
using System.Threading;
using resources = TAS.Client.Common.Properties.Resources;


namespace TAS.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Mutex mutex = new Mutex(false, "TASClientApplication");
        bool _systemShutdown;
        public MainWindow()
        {
            try
            {
                if (!mutex.WaitOne(5000)
                    && (MessageBox.Show(resources._query_StartAnotherInstance,
                                    Common.Properties.Resources._caption_Confirmation, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel))
                {
                    _systemShutdown = true;
                    Application.Current.Shutdown(0);
                }
                else
                {
                    InitializeComponent();
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
            }
            catch (AbandonedMutexException)
            {
                mutex.ReleaseMutex();
                mutex.WaitOne();
            }
        }

        
        private void AppMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Engine engine in App.EngineController.Engines)
            {
                TabItem newtab = new TabItem();
                newtab.Header = engine.EngineName;
                PreviewViewmodel previewViewmodel = new PreviewViewmodel(engine);
                Debug.WriteLine(engine, "Creating viewmodel for");
                var engineViewModel = new EngineViewmodel(engine, previewViewmodel);
                Debug.WriteLine(engine, "Creating commands for");
                newtab.Content = engineViewModel.View;
                tcChannels.Items.Add(newtab);

                Debug.WriteLine(engine.MediaManager, "Creating tab for");
                TabItem tabIngest = new TabItem();
                tabIngest.Header = engine.EngineName + " - Media";
                MediaManagerViewmodel newMediaManagerViewmodel = new MediaManagerViewmodel(engine.MediaManager, previewViewmodel);
                tabIngest.Content = newMediaManagerViewmodel.View;
                tcChannels.Items.Add(tabIngest);

                //Debug.WriteLine(engine.Templates, "Creating tab for");
                //TabItem tabTemplates = new TabItem();
                //tabTemplates.Header = engine.EngineName + " - Animacje";
                //TemplatesView newTemplatesView = new TemplatesView();
                //TemplatesViewmodel newTemplatesViewmodel = new TemplatesViewmodel(engine);
                //newTemplatesView.DataContext = newTemplatesViewmodel;
                //tabTemplates.Content = newTemplatesView;
                //tcChannels.Items.Add(tabTemplates);
            }
            tcChannels.SelectedIndex = 0;
        }

        private void AppMainWindow_Closed(object sender, EventArgs e)
        {
        }

        private void AppMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
#if DEBUG == false
            e.Cancel = !_systemShutdown && MessageBox.Show(Properties.Resources._query_ExitApplication, TAS.Client.Common.Properties.Resources._caption_Confirmation, MessageBoxButton.YesNo) == MessageBoxResult.No;
#endif // DEBUG
        }

        private void AppMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (e.Key == Key.G && e.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Control))
            {
                GC.Collect(GC.MaxGeneration);
                Debug.WriteLine("CG enforced");
                e.Handled = true;
            }
#endif // DEBUG
        }
    }
}
