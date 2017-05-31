﻿using TAS.Server.Common;
using TAS.Server.Common.Interfaces;

namespace TAS.Client.ViewModels
{
    public class EventPanelAutoStartEventViewmodel:EventPanelRundownElementViewmodelBase
    {
        public enum TAutoStartPlayState
        {
            ScheduledFuture,
            ScheduledPast,
            Playing,
            Played,
            Disabled
        }


        public EventPanelAutoStartEventViewmodel(IEvent ev):base(ev, null) { }

        public string ScheduledDate => _event.ScheduledTime.ToLocalTime().ToString("d");

        public TAutoStartPlayState AutoStartPlayState
        {
            get
            {
                if (!_event.IsEnabled)
                    return TAutoStartPlayState.Disabled;
                if (_event.PlayState == TPlayState.Playing)
                    return TAutoStartPlayState.Playing;
                if (_event.PlayState == TPlayState.Played)
                    return TAutoStartPlayState.Played;
                if (_event.PlayState == TPlayState.Scheduled)
                    if (_engine.CurrentTime < _event.ScheduledTime || (_event.AutoStartFlags & AutoStartFlags.Daily )!= AutoStartFlags.None)
                        return TAutoStartPlayState.ScheduledFuture;
                else
                return TAutoStartPlayState.ScheduledPast;
                return TAutoStartPlayState.Disabled;
            }
        }
        
        protected override void NotifyPropertyChanged(string propertyName)
        {
            base.NotifyPropertyChanged(propertyName);
            if (propertyName == nameof(ScheduledTime))
               NotifyPropertyChanged(nameof(ScheduledDate));
            if (propertyName == nameof(IsEnabled) || propertyName == nameof(PlayState) || propertyName == nameof(ScheduledTime))
                NotifyPropertyChanged(nameof(AutoStartPlayState));
        }

    }
}
