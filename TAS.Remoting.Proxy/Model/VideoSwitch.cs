﻿using System;
using System.Collections.Generic;
using jNet.RPC;
using jNet.RPC.Client;
using TAS.Common;
using TAS.Common.Interfaces;

namespace TAS.Remoting.Model
{
    public class VideoSwitch : ProxyObjectBase, IVideoSwitch
    {
#pragma warning disable CS0649 

        [DtoMember(nameof(IVideoSwitch.InputPorts))]
        private IList<IVideoSwitchPort> _inputPorts;

        [DtoMember(nameof(IVideoSwitch.SelectedInputPort))]
        private IVideoSwitchPort _selectedInputPort;

        [DtoMember(nameof(IVideoSwitch.IsConnected))]
        private bool _isConnected;       

        [DtoMember(nameof(IVideoSwitch.IsEnabled))]
        private bool _isEnabled;

        [DtoMember(nameof(IVideoSwitch.DefaultEffect))]
        private VideoSwitchEffect _defaultEffect;

        [DtoMember(nameof(IVideoSwitch.Preload))]
        private bool _preload;

#pragma warning restore

        public IList<IVideoSwitchPort> InputPorts => _inputPorts;

        public IVideoSwitchPort SelectedInputPort => _selectedInputPort;

        public bool IsConnected => _isConnected;       

        public bool IsEnabled { get => _isEnabled; set => Set(value); }

        public bool Preload => _preload;

        public VideoSwitchEffect DefaultEffect => _defaultEffect;

        public event EventHandler Started;

        public void Connect()
        {
            Invoke();
        }

        public void SelectInput(int inputId)
        {
            Invoke(parameters: new object[] { inputId });
        }

        public void SetTransitionEffect(VideoSwitchEffect videoSwitchEffect)
        {
            Invoke(parameters: new object[] { videoSwitchEffect });
        }

        protected override void OnEventNotification(SocketMessage message) { }
    }
}
