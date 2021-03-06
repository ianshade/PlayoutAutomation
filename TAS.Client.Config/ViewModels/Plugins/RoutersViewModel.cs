﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows.Data;
using TAS.Client.Common;
using TAS.Client.Config.Model;
using TAS.Common.Interfaces;
using TAS.Common.Interfaces.Configurator;

namespace TAS.Client.Config.ViewModels.Plugins
{
    public class RoutersViewModel : ViewModelBase, IPluginManager
    {
        private IConfigEngine _engine;
        public event EventHandler PluginChanged;

        private List<IPluginConfigurator> _configurators = new List<IPluginConfigurator>();
        private IPluginConfigurator _selectedConfigurator;

        private bool? _isEnabled;

        public RoutersViewModel(IConfigEngine engine)
        {
            _engine = engine;
            Configurators = CollectionViewSource.GetDefaultView(_configurators);

            using (var catalog = new DirectoryCatalog(Path.Combine(Directory.GetCurrentDirectory(), "Plugins"), PluginsViewModel.FileNameSearchPattern))
            {
                using (var container = new CompositionContainer(catalog))
                {
                    container.ComposeExportedValue("Engine", _engine);
                    var pluginConfigurators = container.GetExportedValues<IPluginConfigurator>().Where(configurator => configurator.GetModel() is IVideoSwitch);

                    foreach (var pluginConfigurator in pluginConfigurators)
                    {
                        pluginConfigurator.PluginChanged += PluginConfigurator_PluginChanged;
                        pluginConfigurator.Initialize(_engine.Router);
                        _configurators.Add(pluginConfigurator);
                    }
                    
                    SelectedConfigurator = _configurators.FirstOrDefault(p => p.GetModel()?.GetType() == _engine.Router?.GetType());
                }
            }
        }

        private void PluginConfigurator_PluginChanged(object sender, EventArgs e)
        {
            PluginChanged?.Invoke(this, EventArgs.Empty);
        }

        public ICollectionView Configurators { get; }

        public IPluginConfigurator SelectedConfigurator
        {
            get => _selectedConfigurator;
            set
            {
                if (!SetField(ref _selectedConfigurator, value))
                    return;

                _isEnabled = _selectedConfigurator?.IsEnabled ?? false;
                NotifyPropertyChanged(nameof(IsEnabled));
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!SetField(ref _isEnabled, value))
                    return;

                if (value == null)
                    return;

                if (_selectedConfigurator != null)
                    _selectedConfigurator.IsEnabled = (bool)value;
            }
        }

        public string Name => _selectedConfigurator.PluginName;

        public IVideoSwitch Router => (IVideoSwitch)_selectedConfigurator.GetModel();

        protected override void OnDispose()
        {
            foreach (var routerConfigurator in _configurators)
            {
                routerConfigurator.PluginChanged -= PluginConfigurator_PluginChanged;                
            }
        }

        public void Save()
        {
            _selectedConfigurator.Save();
        }
    }
}
