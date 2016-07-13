﻿using Infralution.Localization.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TAS.Common
{
    public enum TServerType { Caspar = 0 }

    public enum TDirectoryAccessType { Direct, FTP };

    public enum TFileOperationKind { None, Copy, Move, Convert, Export, Delete, Loudness };

    [TypeConverter(typeof(FileOperationStatusEnumConverter))]
    public enum FileOperationStatus
    {
        Unknown,
        Waiting,
        InProgress,
        Finished,
        Failed,
        Aborted,
    };

    class FileOperationStatusEnumConverter : ResourceEnumConverter
    {
        public FileOperationStatusEnumConverter()
            : base(typeof(FileOperationStatus), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [Flags]
    public enum AutoStartFlags: byte
    {
        None,
        Force,
        Daily
    }

    [TypeConverter(typeof(IngestStatusEnumConverter))]
    public enum TIngestStatus
    {
        Unknown,
        NotReady,
        InProgress,
        Ready
    }
    class IngestStatusEnumConverter : ResourceEnumConverter
    {
        public IngestStatusEnumConverter()
            : base(typeof(TIngestStatus), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [Flags]
    public enum VideoLayer : sbyte
    {
        None = -1,
        Program = 0x10,
        CG1 = Program | 1,
        CG2 = Program | 2,
        CG3 = Program | 3,
        CG4 = Program | 4,
        CG5 = Program | 5,
        Animation = Program | 0xA,
        Preset = 0x2F,
        Preview = 0x30,
        PreviewCG1 = Preview | CG1,
        PreviewCG2 = Preview | CG2,
        PreviewCG3 = Preview | CG3,
        PreviewCG4 = Preview | CG4,
        PreviewCG5 = Preview | CG5,
        PreviewAnimation = Preview | Animation,
    }
    public enum TEngineOperation { Play, Pause, Stop, Clear, Load, Schedule }
    public enum TEngineState { NotInitialized, Idle, Running, Hold }
    public enum TemplateMethod: byte { Add, Play, Stop, Next, Remove, Clear, Update, Invoke }

    [Flags]
    [TypeConverter(typeof(TAspectRatioControlEnumConverter))]
    public enum TAspectRatioControl
    {
        None,
        GPI,
        ImageResize,
        GPIandImageResize = GPI + ImageResize,
    }
    class TAspectRatioControlEnumConverter : ResourceEnumConverter
    {
        public TAspectRatioControlEnumConverter()
            : base(typeof(TAspectRatioControl), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TAspectConversionEnumConverter))]
    public enum TAspectConversion : byte
    {
        NoConversion,
        Force4_3,
        Force16_9,
        PillarBox,
        TiltScan,
        Letterbox,
        PanScan
    }
    class TAspectConversionEnumConverter : ResourceEnumConverter
    {
        public TAspectConversionEnumConverter()
            : base(typeof(TAspectConversion), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TmXFAudioExportFormatEnumConverter))]
    public enum TmXFAudioExportFormat : byte
    {
        Channels4Bits16,
        Channels8Bits16,
        Channels4Bits24,
    }

    class TmXFAudioExportFormatEnumConverter : ResourceEnumConverter
    {
        public TmXFAudioExportFormatEnumConverter()
            : base(typeof(TmXFAudioExportFormat), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    public enum TmXFVideoExportFormat : byte
    {
        IMX50,
        IMX40,
        IMX30
    }

    public enum TMediaExportContainerFormat
    {
        mov,
        mp4,
        mxf,
    }

    public enum TArchivePolicyType { NoArchive, ArchivePlayedAndNotUsedWhenDeleteEvent };
    class TArchivePolicyTypeConversionEnumConverter : ResourceEnumConverter
    {
        public TArchivePolicyTypeConversionEnumConverter()
            : base(typeof(TArchivePolicyType), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }


    [TypeConverter(typeof(TAudioChannelMappingConversionEnumConverter))]
    public enum TAudioChannelMappingConversion : byte
    {
        Default,
        FirstTwoChannels,
        SecondTwoChannels,
        FirstChannelOnly, 
        SecondChannelOnly,
        Combine1plus2,
        Combine3plus4
    }
    class TAudioChannelMappingConversionEnumConverter : ResourceEnumConverter
    {
        public TAudioChannelMappingConversionEnumConverter()
            : base(typeof(TAudioChannelMappingConversion), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TVideoFormatEnumConverter))]
    public enum TVideoFormat : byte
    {
        PAL_FHA    = 0x0,
        PAL        = 0x1,
        NTSC       = 0x2,
        PAL_FHA_P  = 0x3,
        PAL_P      = 0x4,
        NTSC_FHA   = 0x5,
        HD720p2500 = 0x8,
        HD720p5000 = 0x9,
        HD720p5994 = 0xA,
        HD720p6000 = 0xB,
        HD1080p2398 = 0x10,
        HD1080p2400 = 0x11,
        HD1080p2500 = 0x12,
        HD1080p2997 = 0x13,
        HD1080p3000 = 0x14,
        HD1080p5000	= 0x15,
        HD1080i5000	= 0x16,
        HD1080p5994	= 0x17,
        HD1080i5994	= 0x18,
        HD1080p6000	= 0x19,
        HD1080i6000	= 0x1A,
        HD2160p2398	= 0x20,
        HD2160p2400	= 0x21,
        HD2160p2500	= 0x22,
        HD2160p2997	= 0x23,
        HD2160p3000 = 0x24,
        Other = 0xFF
    }
    class TVideoFormatEnumConverter : ResourceEnumConverter
    {
        public TVideoFormatEnumConverter()
            : base(typeof(TVideoFormat), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
        protected override string GetValueText(System.Globalization.CultureInfo culture, object value)
        {
            string resourceName = GetResourceName(value);
            string result = _resourceManager.GetString(resourceName, culture);
            if (result == null)
                result = value.ToString();
            return result;
        }
    }

    [TypeConverter(typeof(TMediaCategoryEnumConverter))]
    public enum TMediaCategory
    {
        Uncategorized,
        Show,
        Commercial,
        Promo,
        Sponsored,
        Fill,
        Insert,
        Jingle
    };
    class TMediaCategoryEnumConverter : ResourceEnumConverter
    {
        public TMediaCategoryEnumConverter()
            : base(typeof(TMediaCategory), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TAudioChannelMappingEnumConverter))]
    public enum TAudioChannelMapping : byte
    {
        Unknown = 0,          // will pass everything as is
        Mono = 1,         // 1.0          L
        Stereo = 2,           // 2.0           L R
        Dts = 3,              // 5.1           C L R Ls Rs LFE
        DolbyE = 4,           // 5.1+stereomix L R C LFE Ls Rs Lmix Rmix
        DolbyDigital = 5,     // 5.1           L C R Ls Rs LFE
        Smpte = 6,            // 5.1           L R C LFE Ls Rs
    }
    class TAudioChannelMappingEnumConverter : ResourceEnumConverter
    {
        public TAudioChannelMappingEnumConverter()
            : base(typeof(TAudioChannelMapping), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }
    [TypeConverter(typeof(TMediaTypeEnumConverter))]
    public enum TMediaType
    {
        Unknown,
        Movie,
        Still,
        Audio,
        Animation,
    };
    class TMediaTypeEnumConverter : ResourceEnumConverter
    {
        public TMediaTypeEnumConverter()
            : base(typeof(TMediaType), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }
    [TypeConverter(typeof(TMediaStatusEnumConverter))]
    public enum TMediaStatus: byte
    {
        Unknown,
        Available,
        CopyPending,
        Copying,
        Copied,
        Deleted,
        CopyError,
        Required,
        ValidationError,
    };
    class TMediaStatusEnumConverter : ResourceEnumConverter
    {
        public TMediaStatusEnumConverter()
            : base(typeof(TMediaStatus), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }
    

    [TypeConverter(typeof(TMediaErrorInfoEnumConverter))]
    public enum TMediaErrorInfo
    {
        NoError,
        Missing,
        TooShort,
    }
    class TMediaErrorInfoEnumConverter : ResourceEnumConverter
    {
        public TMediaErrorInfoEnumConverter()
            : base(typeof(TMediaErrorInfo), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }


    [TypeConverter(typeof(TFieldOrderEnumConverter))]
    public enum TFieldOrder
    {
        Unknown,
        TFF,
        BFF,
        Progressive
    }
    class TFieldOrderEnumConverter : ResourceEnumConverter
    {
        public TFieldOrderEnumConverter()
            : base(typeof(TFieldOrder), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TEventTypeEnumConverter))]
    public enum TEventType
    {
        Rundown = 0,
        Movie = 1,
        StillImage = 2,
        Live = 4,
        Container = 5,
        Animation = 6
    };
    class TEventTypeEnumConverter : ResourceEnumConverter
    {
        public TEventTypeEnumConverter()
            : base(typeof(TEventType), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TStartTypeEnumConverter))]
    public enum TStartType
    {
        After,
        With,
        Manual,
        OnFixedTime,
        None,
    };
    class TStartTypeEnumConverter : ResourceEnumConverter
    {
        public TStartTypeEnumConverter()
            : base(typeof(TStartType), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TPlayStateEnumConverter))]
    public enum TPlayState
    {
        Scheduled,
        Paused,
        Playing,
        Fading,
        Played,
        Aborted
    };
    class TPlayStateEnumConverter : ResourceEnumConverter
    {
        public TPlayStateEnumConverter()
            : base(typeof(TPlayState), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TLogoEnumConverter))]
    public enum TLogo
    {
        NoLogo,
        Normal,
        Live,
        Premiere,
        Replay,
    }
    class TLogoEnumConverter : ResourceEnumConverter
    {
        public TLogoEnumConverter()
            : base(typeof(TLogo), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TCrawlEnumConverter))]
    public enum TCrawl
    {
        NoCrawl,
        Normal,
        Urgent,
        Sport,
    }
    class TCrawlEnumConverter : ResourceEnumConverter
    {
        public TCrawlEnumConverter()
            : base(typeof(TCrawl), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }


    public enum TTransitionType {
        Cut = 0,
        Mix = 1,
        Push = 2,
        Slide = 3,
        Wipe = 4,
        Squeeze = 5,
    };
    
    [TypeConverter(typeof(TParentalEnumConverter))]
    public enum TParental
    {
        None,
        NoLimit,
        Limit07,
        Limit12,
        Limit16,
        Limit18,
    }
    class TParentalEnumConverter:ResourceEnumConverter
    {
        public TParentalEnumConverter()
            : base(typeof(TParental), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    [TypeConverter(typeof(TMediaEmphasisEnumConverter))]
    public enum TMediaEmphasis : byte
    {
        [Color(0x0)]
        None = 0,
        [Color(0xFFC0C000)]
        Olive = 1,
        [Color(0xFFFFB6C1)]
        Pink = 2,
        [Color(0xFFFFE4C4)]
        Beige = 3,
        [Color(0xFF87CEFA)]
        SkyBlue = 4,
        [Color(0xFFFFFFC0)]
        Yellow = 5,
        [Color(0xFFEE82EE)]
        Violet = 6,
        [Color(0xFFFFA500)]
        Orange = 7,
    }
    class TMediaEmphasisEnumConverter : ResourceEnumConverter
    {
        public TMediaEmphasisEnumConverter()
            : base(typeof(TMediaEmphasis), TAS.Server.Common.Properties.Resources.ResourceManager)
        { }
    }

    public enum TEasing
    {
        Linear = 1,
        None,
        InQuad,
        OutQuad,
        InOutQuad,
        OutInQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        OutInCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        OutInQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        OutInQuint,
        InSine,
        OutSine,
        InOutSine,
        OutInSine,
        InExpo,
        OutExpo,
        InOutExpo,
        OutInExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        OutInCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        OutInElastic,
        InBack,
        OutBack,
        InOutBack,
        OutInBack,
        OutBounce,
        InBounce,
        InOutBounce,
        OutInBounce
    }
}
