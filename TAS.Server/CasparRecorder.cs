﻿using Svt.Caspar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using jNet.RPC.Server;
using TAS.Common;
using TAS.Common.Interfaces;
using TAS.Common.Interfaces.Media;
using TAS.Common.Interfaces.MediaDirectory;
using TAS.Server.Media;
using TAS.Database.Common;
using jNet.RPC;

namespace TAS.Server
{
    [DtoType(typeof(IRecorder))]
    public class CasparRecorder: ServerObjectBase, IRecorder
    {
        private TVideoFormat _tcFormat = TVideoFormat.PAL;
        private Recorder _recorder;
        private IMedia _recordingMedia;
        internal IArchiveDirectory ArchiveDirectory;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private CasparServer _ownerServer;

        private TimeSpan _currentTc;
        private TimeSpan _timeLimit;
        private TDeckState _deckState;
        private TDeckControl _deckControl;
        private bool _isDeckConnected;
        private bool _isServerConnected;
        private string _recorderName;        

        #region Deserialized properties
        public int RecorderNumber { get; set; }

        [DtoMember, Hibernate]
        public int Id { get; set; }

        [DtoMember, Hibernate]
        public string RecorderName
        {
            get => _recorderName;
            set => SetField(ref _recorderName, value);
        }

        [DtoMember, Hibernate]
        public int DefaultChannel { get; set; }

        #endregion Deserialized properties

        #region IRecorder
        [DtoMember]
        public int ServerId { get => (int)_ownerServer.Id; }

        [DtoMember]
        public TimeSpan CurrentTc { get => _currentTc; private set => SetField(ref _currentTc, value); }

        [DtoMember]
        public TimeSpan TimeLimit { get => _timeLimit; private set => SetField(ref _timeLimit, value); }

        [DtoMember]
        public TDeckState DeckState { get => _deckState; private set => SetField(ref _deckState, value); }

        [DtoMember]
        public TDeckControl DeckControl { get => _deckControl; private set => SetField(ref _deckControl, value); }

        [DtoMember]
        public bool IsDeckConnected { get => _isDeckConnected; private set => SetField(ref _isDeckConnected, value); }

        [DtoMember]
        public bool IsServerConnected { get => _isServerConnected; internal set => SetField(ref _isServerConnected, value); }

        [DtoMember]
        public IEnumerable<IPlayoutServerChannel> Channels => _ownerServer.Channels.ToList();

        [DtoMember]
        public IMedia RecordingMedia { get => _recordingMedia; private set => SetField(ref _recordingMedia, value); }

        [DtoMember]
        public IWatcherDirectory RecordingDirectory => _ownerServer.MediaDirectory;
        
        public IMedia Capture(IPlayoutServerChannel channel, TimeSpan tcIn, TimeSpan tcOut, bool narrowMode, string mediaName, string fileName, int[] channelMap)
        {
            _tcFormat = channel.VideoFormat;
            var directory = (ServerDirectory)_ownerServer.MediaDirectory;
            var newMedia = new ServerMedia
            {
                MediaName = mediaName,
                LastUpdated = DateTime.UtcNow,
                MediaGuid = Guid.NewGuid(),
                MediaType = TMediaType.Movie,
                FileName = fileName,
                TcStart = tcIn,
                TcPlay = tcIn,
                Duration = tcOut - tcIn,
                MediaStatus = TMediaStatus.Copying,
            };
            directory.AddMedia(newMedia);
            if (_recorder?.Capture(channel.Id, tcIn.ToSmpteTimecodeString(channel.VideoFormat), tcOut.ToSmpteTimecodeString(channel.VideoFormat), narrowMode, fileName, channelMap) == true)
            {
                RecordingMedia = newMedia;
                Logger.Debug("Started recording from {0} file {1} TcIn {2} TcOut {3}", channel.ChannelName, fileName, tcIn, tcOut);
                return newMedia;
            }
            Logger.Error("Unsuccessfull recording from {0} file {1} TcIn {2} TcOut {3}", channel.ChannelName, fileName, tcIn, tcOut);
            return null;
        }

        public IMedia Capture(IPlayoutServerChannel channel, TimeSpan timeLimit, bool narrowMode, string mediaName, string fileName, int[] channelMap)
        {
            _tcFormat = channel.VideoFormat;
            var directory = (ServerDirectory)_ownerServer.MediaDirectory;
            var newMedia = new ServerMedia
            {
                MediaName = mediaName,
                LastUpdated = DateTime.UtcNow,
                MediaType = TMediaType.Movie,
                MediaGuid = Guid.NewGuid(),
                FileName = fileName,
                TcStart = TimeSpan.Zero,
                TcPlay =TimeSpan.Zero,
                Duration = timeLimit,
                MediaStatus = TMediaStatus.Copying,
            };
            directory.AddMedia(newMedia);
            if (_recorder?.Capture(channel.Id,  timeLimit.ToSmpteFrames(channel.VideoFormat), narrowMode, fileName, channelMap) == true)
            {
                RecordingMedia = newMedia;
                Logger.Debug("Started recording from {0} file {1} with time limit {2} ", channel.ChannelName, fileName, timeLimit);
                return newMedia;
            }
            Logger.Error("Unsuccessfull recording from {0} file {1} with time limit {2}", channel.ChannelName, fileName, timeLimit);
            return null;
        }

        public void SetTimeLimit(TimeSpan value)
        {
            _recorder?.SetTimeLimit(value.ToSmpteFrames(_tcFormat));
        }

        public void Finish()
        {
            _recorder?.Finish();
        }
        
        public void Abort()
        {
            _recorder?.Abort();
        }

        public void DeckPlay()
        {
            _recorder?.Play();
        }
        public void DeckStop()
        {
            _recorder?.Stop();
        }

        public void DeckFastForward()
        {
            _recorder.FastForward();
        }

        public void DeckRewind()
        {
            _recorder.Rewind();
        }

        public void GoToTimecode(TimeSpan tc, TVideoFormat format)
        {
            _recorder?.GotoTimecode(tc.ToSmpteTimecodeString(format));
        }

        #endregion IRecorder

        internal void SetRecorder(Recorder value)
        {
            var oldRecorder = _recorder;
            if (_recorder != value)
            {
                if (oldRecorder != null)
                {
                    oldRecorder.Tc -= _recorder_Tc;
                    oldRecorder.FramesLeft -= _recorder_FramesLeft;
                    oldRecorder.DeckConnected -= _recorder_DeckConnected;
                    oldRecorder.DeckControl -= _recorder_DeckControl;
                    oldRecorder.DeckState -= _recorder_DeckState;
                }
                _recorder = value;
                if (value != null)
                {
                    value.Tc += _recorder_Tc;
                    value.DeckConnected += _recorder_DeckConnected;
                    value.DeckControl += _recorder_DeckControl;
                    value.DeckState += _recorder_DeckState;
                    value.FramesLeft += _recorder_FramesLeft;
                    IsDeckConnected = value.IsConnected;
                    DeckState = TDeckState.Unknown;
                    DeckControl = TDeckControl.None;
                }
            }
        }

        internal void SetOwner(CasparServer owner)
        {
            _ownerServer = owner;            
        }

        internal event EventHandler<MediaEventArgs> CaptureSuccess;

        private void _recorder_FramesLeft(object sender, FramesLeftEventArgs e)
        {
            var media = _recordingMedia;
            if (media != null)
                TimeLimit = e.FramesLeft.SmpteFramesToTimeSpan(media.VideoFormat);
        }

        private void _recorder_DeckState(object sender, DeckStateEventArgs e)
        {
            DeckState = (TDeckState)e.State;
        }

        private void _recorder_DeckControl(object sender, DeckControlEventArgs e)
        {
            if (e.ControlEvent == Svt.Caspar.DeckControl.capture_complete)
                _captureCompleted();
            if (e.ControlEvent == Svt.Caspar.DeckControl.aborted)
                _captureAborted();
            DeckControl = (TDeckControl)e.ControlEvent;
        }

        private void _captureAborted()
        {
            _recordingMedia?.Delete();
            RecordingMedia = null;
            Logger.Trace("Capture aborted notified");
        }

        private void _captureCompleted()
        {
            var media = _recordingMedia;
            if (media?.MediaStatus == TMediaStatus.Copying)
            {
                media.MediaStatus = TMediaStatus.Copied;
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    media.Verify(true);
                    if (media.MediaStatus == TMediaStatus.Available)
                        CaptureSuccess?.Invoke(this, new MediaEventArgs(media));
                });
            }
            RecordingMedia = null;
            Logger.Trace("Capture completed notified");
        }

        private void _recorder_DeckConnected(object sender, DeckConnectedEventArgs e)
        {
            IsDeckConnected = e.IsConnected;
            Logger.Trace("Deck {0}", e.IsConnected ? "connected" : "disconnected");
        }

        private void _recorder_Tc(object sender, TcEventArgs e)
        {
            if (e.Tc.IsValidSmpteTimecode(_tcFormat))
                CurrentTc = e.Tc.SmpteTimecodeToTimeSpan(_tcFormat);
        }

    }
}
