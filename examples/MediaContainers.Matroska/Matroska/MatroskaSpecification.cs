namespace MediaContainers.Matroska
{
   public static class MatroskaSpecification
   {
      public static readonly EBMLElementDefiniton Segment = new EBMLElementDefiniton(0x18538067, EBMLElementType.Master, @"\Segment", allowUnknownSize: true);
      public static readonly EBMLElementDefiniton SeekHead = new EBMLElementDefiniton(0x114D9B74, EBMLElementType.Master, @"\Segment\SeekHead");
      public static readonly EBMLElementDefiniton Seek = new EBMLElementDefiniton(0x4DBB, EBMLElementType.Master, @"\Segment\SeekHead\Seek");
      public static readonly EBMLElementDefiniton SeekID = new EBMLElementDefiniton(0x53AB, EBMLElementType.Binary, @"\Segment\SeekHead\Seek\SeekID");
      public static readonly EBMLElementDefiniton SeekPosition = new EBMLElementDefiniton(0x53AC, EBMLElementType.UnsignedInteger, @"\Segment\SeekHead\Seek\SeekPosition");
      public static readonly EBMLElementDefiniton Info = new EBMLElementDefiniton(0x1549A966, EBMLElementType.Master, @"\Segment\Info");
      public static readonly EBMLElementDefiniton SegmentUID = new EBMLElementDefiniton(0x73A4, EBMLElementType.Binary, @"\Segment\Info\SegmentUID");
      public static readonly EBMLElementDefiniton SegmentFilename = new EBMLElementDefiniton(0x7384, EBMLElementType.UTF8, @"\Segment\Info\SegmentFilename");
      public static readonly EBMLElementDefiniton PrevUID = new EBMLElementDefiniton(0x3CB923, EBMLElementType.Binary, @"\Segment\Info\PrevUID");
      public static readonly EBMLElementDefiniton PrevFilename = new EBMLElementDefiniton(0x3C83AB, EBMLElementType.UTF8, @"\Segment\Info\PrevFilename");
      public static readonly EBMLElementDefiniton NextUID = new EBMLElementDefiniton(0x3EB923, EBMLElementType.Binary, @"\Segment\Info\NextUID");
      public static readonly EBMLElementDefiniton NextFilename = new EBMLElementDefiniton(0x3E83BB, EBMLElementType.UTF8, @"\Segment\Info\NextFilename");
      public static readonly EBMLElementDefiniton SegmentFamily = new EBMLElementDefiniton(0x4444, EBMLElementType.Binary, @"\Segment\Info\SegmentFamily");
      public static readonly EBMLElementDefiniton ChapterTranslate = new EBMLElementDefiniton(0x6924, EBMLElementType.Master, @"\Segment\Info\ChapterTranslate");
      public static readonly EBMLElementDefiniton ChapterTranslateID = new EBMLElementDefiniton(0x69A5, EBMLElementType.Binary, @"\Segment\Info\ChapterTranslate\ChapterTranslateID");
      public static readonly EBMLElementDefiniton ChapterTranslateCodec = new EBMLElementDefiniton(0x69BF, EBMLElementType.UnsignedInteger, @"\Segment\Info\ChapterTranslate\ChapterTranslateCodec");
      public static readonly EBMLElementDefiniton ChapterTranslateEditionUID = new EBMLElementDefiniton(0x69FC, EBMLElementType.UnsignedInteger, @"\Segment\Info\ChapterTranslate\ChapterTranslateEditionUID");
      public static readonly EBMLElementDefiniton TimestampScale = new EBMLElementDefiniton(0x2AD7B1, EBMLElementType.UnsignedInteger, @"\Segment\Info\TimestampScale", defaultVal: "1000000");
      public static readonly EBMLElementDefiniton Duration = new EBMLElementDefiniton(0x4489, EBMLElementType.Float, @"\Segment\Info\Duration");
      public static readonly EBMLElementDefiniton DateUTC = new EBMLElementDefiniton(0x4461, EBMLElementType.Date, @"\Segment\Info\DateUTC");
      public static readonly EBMLElementDefiniton Title = new EBMLElementDefiniton(0x7BA9, EBMLElementType.UTF8, @"\Segment\Info\Title");
      public static readonly EBMLElementDefiniton MuxingApp = new EBMLElementDefiniton(0x4D80, EBMLElementType.UTF8, @"\Segment\Info\MuxingApp");
      public static readonly EBMLElementDefiniton WritingApp = new EBMLElementDefiniton(0x5741, EBMLElementType.UTF8, @"\Segment\Info\WritingApp");
      public static readonly EBMLElementDefiniton Cluster = new EBMLElementDefiniton(0x1F43B675, EBMLElementType.Master, @"\Segment\Cluster", allowUnknownSize: true);
      public static readonly EBMLElementDefiniton Timestamp = new EBMLElementDefiniton(0xE7, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\Timestamp");
      public static readonly EBMLElementDefiniton SilentTracks = new EBMLElementDefiniton(0x5854, EBMLElementType.Master, @"\Segment\Cluster\SilentTracks");
      public static readonly EBMLElementDefiniton SilentTrackNumber = new EBMLElementDefiniton(0x58D7, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\SilentTracks\SilentTrackNumber");
      public static readonly EBMLElementDefiniton Position = new EBMLElementDefiniton(0xA7, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\Position");
      public static readonly EBMLElementDefiniton PrevSize = new EBMLElementDefiniton(0xAB, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\PrevSize");
      public static readonly EBMLElementDefiniton SimpleBlock = new EBMLElementDefiniton(0xA3, EBMLElementType.Binary, @"\Segment\Cluster\SimpleBlock");
      public static readonly EBMLElementDefiniton BlockGroup = new EBMLElementDefiniton(0xA0, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup");
      public static readonly EBMLElementDefiniton Block = new EBMLElementDefiniton(0xA1, EBMLElementType.Binary, @"\Segment\Cluster\BlockGroup\Block");
      public static readonly EBMLElementDefiniton BlockVirtual = new EBMLElementDefiniton(0xA2, EBMLElementType.Binary, @"\Segment\Cluster\BlockGroup\BlockVirtual");
      public static readonly EBMLElementDefiniton BlockAdditions = new EBMLElementDefiniton(0x75A1, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup\BlockAdditions");
      public static readonly EBMLElementDefiniton BlockMore = new EBMLElementDefiniton(0xA6, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup\BlockAdditions\BlockMore");
      public static readonly EBMLElementDefiniton BlockAddID = new EBMLElementDefiniton(0xEE, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\BlockAdditions\BlockMore\BlockAddID", defaultVal: "1");
      public static readonly EBMLElementDefiniton BlockAdditional = new EBMLElementDefiniton(0xA5, EBMLElementType.Binary, @"\Segment\Cluster\BlockGroup\BlockAdditions\BlockMore\BlockAdditional");
      public static readonly EBMLElementDefiniton BlockDuration = new EBMLElementDefiniton(0x9B, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\BlockDuration");
      public static readonly EBMLElementDefiniton ReferencePriority = new EBMLElementDefiniton(0xFA, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\ReferencePriority", defaultVal: "0");
      public static readonly EBMLElementDefiniton ReferenceBlock = new EBMLElementDefiniton(0xFB, EBMLElementType.SignedInteger, @"\Segment\Cluster\BlockGroup\ReferenceBlock");
      public static readonly EBMLElementDefiniton ReferenceVirtual = new EBMLElementDefiniton(0xFD, EBMLElementType.SignedInteger, @"\Segment\Cluster\BlockGroup\ReferenceVirtual");
      public static readonly EBMLElementDefiniton CodecState = new EBMLElementDefiniton(0xA4, EBMLElementType.Binary, @"\Segment\Cluster\BlockGroup\CodecState");
      public static readonly EBMLElementDefiniton DiscardPadding = new EBMLElementDefiniton(0x75A2, EBMLElementType.SignedInteger, @"\Segment\Cluster\BlockGroup\DiscardPadding");
      public static readonly EBMLElementDefiniton Slices = new EBMLElementDefiniton(0x8E, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup\Slices");
      public static readonly EBMLElementDefiniton TimeSlice = new EBMLElementDefiniton(0xE8, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice");
      public static readonly EBMLElementDefiniton LaceNumber = new EBMLElementDefiniton(0xCC, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice\LaceNumber");
      public static readonly EBMLElementDefiniton FrameNumber = new EBMLElementDefiniton(0xCD, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice\FrameNumber", defaultVal: "0");
      public static readonly EBMLElementDefiniton BlockAdditionID = new EBMLElementDefiniton(0xCB, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice\BlockAdditionID", defaultVal: "0");
      public static readonly EBMLElementDefiniton Delay = new EBMLElementDefiniton(0xCE, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice\Delay", defaultVal: "0");
      public static readonly EBMLElementDefiniton SliceDuration = new EBMLElementDefiniton(0xCF, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\Slices\TimeSlice\SliceDuration", defaultVal: "0");
      public static readonly EBMLElementDefiniton ReferenceFrame = new EBMLElementDefiniton(0xC8, EBMLElementType.Master, @"\Segment\Cluster\BlockGroup\ReferenceFrame");
      public static readonly EBMLElementDefiniton ReferenceOffset = new EBMLElementDefiniton(0xC9, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\ReferenceFrame\ReferenceOffset");
      public static readonly EBMLElementDefiniton ReferenceTimestamp = new EBMLElementDefiniton(0xCA, EBMLElementType.UnsignedInteger, @"\Segment\Cluster\BlockGroup\ReferenceFrame\ReferenceTimestamp");
      public static readonly EBMLElementDefiniton EncryptedBlock = new EBMLElementDefiniton(0xAF, EBMLElementType.Binary, @"\Segment\Cluster\EncryptedBlock");
      public static readonly EBMLElementDefiniton Tracks = new EBMLElementDefiniton(0x1654AE6B, EBMLElementType.Master, @"\Segment\Tracks");
      public static readonly EBMLElementDefiniton TrackEntry = new EBMLElementDefiniton(0xAE, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry");
      public static readonly EBMLElementDefiniton TrackNumber = new EBMLElementDefiniton(0xD7, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackNumber");
      public static readonly EBMLElementDefiniton TrackUID = new EBMLElementDefiniton(0x73C5, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackUID");
      public static readonly EBMLElementDefiniton TrackType = new EBMLElementDefiniton(0x83, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackType");
      public static readonly EBMLElementDefiniton FlagEnabled = new EBMLElementDefiniton(0xB9, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagEnabled", defaultVal: "1");
      public static readonly EBMLElementDefiniton FlagDefault = new EBMLElementDefiniton(0x88, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagDefault", defaultVal: "1");
      public static readonly EBMLElementDefiniton FlagForced = new EBMLElementDefiniton(0x55AA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagForced", defaultVal: "0");
      public static readonly EBMLElementDefiniton FlagHearingImpaired = new EBMLElementDefiniton(0x55AB, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagHearingImpaired");
      public static readonly EBMLElementDefiniton FlagVisualImpaired = new EBMLElementDefiniton(0x55AC, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagVisualImpaired");
      public static readonly EBMLElementDefiniton FlagTextDescriptions = new EBMLElementDefiniton(0x55AD, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagTextDescriptions");
      public static readonly EBMLElementDefiniton FlagOriginal = new EBMLElementDefiniton(0x55AE, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagOriginal");
      public static readonly EBMLElementDefiniton FlagCommentary = new EBMLElementDefiniton(0x55AF, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagCommentary");
      public static readonly EBMLElementDefiniton FlagLacing = new EBMLElementDefiniton(0x9C, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\FlagLacing", defaultVal: "1");
      public static readonly EBMLElementDefiniton MinCache = new EBMLElementDefiniton(0x6DE7, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\MinCache", defaultVal: "0");
      public static readonly EBMLElementDefiniton MaxCache = new EBMLElementDefiniton(0x6DF8, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\MaxCache");
      public static readonly EBMLElementDefiniton DefaultDuration = new EBMLElementDefiniton(0x23E383, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\DefaultDuration");
      public static readonly EBMLElementDefiniton DefaultDecodedFieldDuration = new EBMLElementDefiniton(0x234E7A, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\DefaultDecodedFieldDuration");
      public static readonly EBMLElementDefiniton TrackTimestampScale = new EBMLElementDefiniton(0x23314F, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\TrackTimestampScale", defaultVal: "0x1p+0");
      public static readonly EBMLElementDefiniton TrackOffset = new EBMLElementDefiniton(0x537F, EBMLElementType.SignedInteger, @"\Segment\Tracks\TrackEntry\TrackOffset", defaultVal: "0");
      public static readonly EBMLElementDefiniton MaxBlockAdditionID = new EBMLElementDefiniton(0x55EE, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\MaxBlockAdditionID", defaultVal: "0");
      public static readonly EBMLElementDefiniton BlockAdditionMapping = new EBMLElementDefiniton(0x41E4, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\BlockAdditionMapping");
      public static readonly EBMLElementDefiniton BlockAddIDValue = new EBMLElementDefiniton(0x41F0, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\BlockAdditionMapping\BlockAddIDValue");
      public static readonly EBMLElementDefiniton BlockAddIDName = new EBMLElementDefiniton(0x41A4, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\BlockAdditionMapping\BlockAddIDName");
      public static readonly EBMLElementDefiniton BlockAddIDType = new EBMLElementDefiniton(0x41E7, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\BlockAdditionMapping\BlockAddIDType", defaultVal: "0");
      public static readonly EBMLElementDefiniton BlockAddIDExtraData = new EBMLElementDefiniton(0x41ED, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\BlockAdditionMapping\BlockAddIDExtraData");
      public static readonly EBMLElementDefiniton Name = new EBMLElementDefiniton(0x536E, EBMLElementType.UTF8, @"\Segment\Tracks\TrackEntry\Name");
      public static readonly EBMLElementDefiniton Language = new EBMLElementDefiniton(0x22B59C, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\Language", defaultVal: "eng");
      public static readonly EBMLElementDefiniton LanguageIETF = new EBMLElementDefiniton(0x22B59D, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\LanguageIETF");
      public static readonly EBMLElementDefiniton CodecID = new EBMLElementDefiniton(0x86, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\CodecID");
      public static readonly EBMLElementDefiniton CodecPrivate = new EBMLElementDefiniton(0x63A2, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\CodecPrivate");
      public static readonly EBMLElementDefiniton CodecName = new EBMLElementDefiniton(0x258688, EBMLElementType.UTF8, @"\Segment\Tracks\TrackEntry\CodecName");
      public static readonly EBMLElementDefiniton AttachmentLink = new EBMLElementDefiniton(0x7446, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\AttachmentLink");
      public static readonly EBMLElementDefiniton CodecSettings = new EBMLElementDefiniton(0x3A9697, EBMLElementType.UTF8, @"\Segment\Tracks\TrackEntry\CodecSettings");
      public static readonly EBMLElementDefiniton CodecInfoURL = new EBMLElementDefiniton(0x3B4040, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\CodecInfoURL");
      public static readonly EBMLElementDefiniton CodecDownloadURL = new EBMLElementDefiniton(0x26B240, EBMLElementType.String, @"\Segment\Tracks\TrackEntry\CodecDownloadURL");
      public static readonly EBMLElementDefiniton CodecDecodeAll = new EBMLElementDefiniton(0xAA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\CodecDecodeAll", defaultVal: "1");
      public static readonly EBMLElementDefiniton TrackOverlay = new EBMLElementDefiniton(0x6FAB, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackOverlay");
      public static readonly EBMLElementDefiniton CodecDelay = new EBMLElementDefiniton(0x56AA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\CodecDelay", defaultVal: "0");
      public static readonly EBMLElementDefiniton SeekPreRoll = new EBMLElementDefiniton(0x56BB, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\SeekPreRoll", defaultVal: "0");
      public static readonly EBMLElementDefiniton TrackTranslate = new EBMLElementDefiniton(0x6624, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\TrackTranslate");
      public static readonly EBMLElementDefiniton TrackTranslateTrackID = new EBMLElementDefiniton(0x66A5, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\TrackTranslate\TrackTranslateTrackID");
      public static readonly EBMLElementDefiniton TrackTranslateCodec = new EBMLElementDefiniton(0x66BF, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackTranslate\TrackTranslateCodec");
      public static readonly EBMLElementDefiniton TrackTranslateEditionUID = new EBMLElementDefiniton(0x66FC, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackTranslate\TrackTranslateEditionUID");
      public static readonly EBMLElementDefiniton Video = new EBMLElementDefiniton(0xE0, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\Video");
      public static readonly EBMLElementDefiniton FlagInterlaced = new EBMLElementDefiniton(0x9A, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\FlagInterlaced", defaultVal: "0");
      public static readonly EBMLElementDefiniton FieldOrder = new EBMLElementDefiniton(0x9D, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\FieldOrder", defaultVal: "2");
      public static readonly EBMLElementDefiniton StereoMode = new EBMLElementDefiniton(0x53B8, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\StereoMode", defaultVal: "0");
      public static readonly EBMLElementDefiniton AlphaMode = new EBMLElementDefiniton(0x53C0, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\AlphaMode", defaultVal: "0");
      public static readonly EBMLElementDefiniton OldStereoMode = new EBMLElementDefiniton(0x53B9, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\OldStereoMode");
      public static readonly EBMLElementDefiniton PixelWidth = new EBMLElementDefiniton(0xB0, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelWidth");
      public static readonly EBMLElementDefiniton PixelHeight = new EBMLElementDefiniton(0xBA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelHeight");
      public static readonly EBMLElementDefiniton PixelCropBottom = new EBMLElementDefiniton(0x54AA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelCropBottom", defaultVal: "0");
      public static readonly EBMLElementDefiniton PixelCropTop = new EBMLElementDefiniton(0x54BB, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelCropTop", defaultVal: "0");
      public static readonly EBMLElementDefiniton PixelCropLeft = new EBMLElementDefiniton(0x54CC, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelCropLeft", defaultVal: "0");
      public static readonly EBMLElementDefiniton PixelCropRight = new EBMLElementDefiniton(0x54DD, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\PixelCropRight", defaultVal: "0");
      public static readonly EBMLElementDefiniton DisplayWidth = new EBMLElementDefiniton(0x54B0, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\DisplayWidth");
      public static readonly EBMLElementDefiniton DisplayHeight = new EBMLElementDefiniton(0x54BA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\DisplayHeight");
      public static readonly EBMLElementDefiniton DisplayUnit = new EBMLElementDefiniton(0x54B2, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\DisplayUnit", defaultVal: "0");
      public static readonly EBMLElementDefiniton AspectRatioType = new EBMLElementDefiniton(0x54B3, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\AspectRatioType", defaultVal: "0");
      public static readonly EBMLElementDefiniton UncompressedFourCC = new EBMLElementDefiniton(0x2EB524, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\Video\UncompressedFourCC");
      public static readonly EBMLElementDefiniton GammaValue = new EBMLElementDefiniton(0x2FB523, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\GammaValue");
      public static readonly EBMLElementDefiniton FrameRate = new EBMLElementDefiniton(0x2383E3, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\FrameRate");
      public static readonly EBMLElementDefiniton Colour = new EBMLElementDefiniton(0x55B0, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\Video\Colour");
      public static readonly EBMLElementDefiniton MatrixCoefficients = new EBMLElementDefiniton(0x55B1, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\MatrixCoefficients", defaultVal: "2");
      public static readonly EBMLElementDefiniton BitsPerChannel = new EBMLElementDefiniton(0x55B2, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\BitsPerChannel", defaultVal: "0");
      public static readonly EBMLElementDefiniton ChromaSubsamplingHorz = new EBMLElementDefiniton(0x55B3, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\ChromaSubsamplingHorz");
      public static readonly EBMLElementDefiniton ChromaSubsamplingVert = new EBMLElementDefiniton(0x55B4, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\ChromaSubsamplingVert");
      public static readonly EBMLElementDefiniton CbSubsamplingHorz = new EBMLElementDefiniton(0x55B5, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\CbSubsamplingHorz");
      public static readonly EBMLElementDefiniton CbSubsamplingVert = new EBMLElementDefiniton(0x55B6, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\CbSubsamplingVert");
      public static readonly EBMLElementDefiniton ChromaSitingHorz = new EBMLElementDefiniton(0x55B7, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\ChromaSitingHorz", defaultVal: "0");
      public static readonly EBMLElementDefiniton ChromaSitingVert = new EBMLElementDefiniton(0x55B8, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\ChromaSitingVert", defaultVal: "0");
      public static readonly EBMLElementDefiniton Range = new EBMLElementDefiniton(0x55B9, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\Range", defaultVal: "0");
      public static readonly EBMLElementDefiniton TransferCharacteristics = new EBMLElementDefiniton(0x55BA, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\TransferCharacteristics", defaultVal: "2");
      public static readonly EBMLElementDefiniton Primaries = new EBMLElementDefiniton(0x55BB, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\Primaries", defaultVal: "2");
      public static readonly EBMLElementDefiniton MaxCLL = new EBMLElementDefiniton(0x55BC, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\MaxCLL");
      public static readonly EBMLElementDefiniton MaxFALL = new EBMLElementDefiniton(0x55BD, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Colour\MaxFALL");
      public static readonly EBMLElementDefiniton MasteringMetadata = new EBMLElementDefiniton(0x55D0, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata");
      public static readonly EBMLElementDefiniton PrimaryRChromaticityX = new EBMLElementDefiniton(0x55D1, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryRChromaticityX");
      public static readonly EBMLElementDefiniton PrimaryRChromaticityY = new EBMLElementDefiniton(0x55D2, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryRChromaticityY");
      public static readonly EBMLElementDefiniton PrimaryGChromaticityX = new EBMLElementDefiniton(0x55D3, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryGChromaticityX");
      public static readonly EBMLElementDefiniton PrimaryGChromaticityY = new EBMLElementDefiniton(0x55D4, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryGChromaticityY");
      public static readonly EBMLElementDefiniton PrimaryBChromaticityX = new EBMLElementDefiniton(0x55D5, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryBChromaticityX");
      public static readonly EBMLElementDefiniton PrimaryBChromaticityY = new EBMLElementDefiniton(0x55D6, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\PrimaryBChromaticityY");
      public static readonly EBMLElementDefiniton WhitePointChromaticityX = new EBMLElementDefiniton(0x55D7, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\WhitePointChromaticityX");
      public static readonly EBMLElementDefiniton WhitePointChromaticityY = new EBMLElementDefiniton(0x55D8, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\WhitePointChromaticityY");
      public static readonly EBMLElementDefiniton LuminanceMax = new EBMLElementDefiniton(0x55D9, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\LuminanceMax");
      public static readonly EBMLElementDefiniton LuminanceMin = new EBMLElementDefiniton(0x55DA, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Colour\MasteringMetadata\LuminanceMin");
      public static readonly EBMLElementDefiniton Projection = new EBMLElementDefiniton(0x7670, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\Video\Projection");
      public static readonly EBMLElementDefiniton ProjectionType = new EBMLElementDefiniton(0x7671, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Video\Projection\ProjectionType", defaultVal: "0");
      public static readonly EBMLElementDefiniton ProjectionPrivate = new EBMLElementDefiniton(0x7672, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\Video\Projection\ProjectionPrivate");
      public static readonly EBMLElementDefiniton ProjectionPoseYaw = new EBMLElementDefiniton(0x7673, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Projection\ProjectionPoseYaw", defaultVal: "0x0p+0");
      public static readonly EBMLElementDefiniton ProjectionPosePitch = new EBMLElementDefiniton(0x7674, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Projection\ProjectionPosePitch", defaultVal: "0x0p+0");
      public static readonly EBMLElementDefiniton ProjectionPoseRoll = new EBMLElementDefiniton(0x7675, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Video\Projection\ProjectionPoseRoll", defaultVal: "0x0p+0");
      public static readonly EBMLElementDefiniton Audio = new EBMLElementDefiniton(0xE1, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\Audio");
      public static readonly EBMLElementDefiniton SamplingFrequency = new EBMLElementDefiniton(0xB5, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Audio\SamplingFrequency", defaultVal: "0x1.f4p+12");
      public static readonly EBMLElementDefiniton OutputSamplingFrequency = new EBMLElementDefiniton(0x78B5, EBMLElementType.Float, @"\Segment\Tracks\TrackEntry\Audio\OutputSamplingFrequency");
      public static readonly EBMLElementDefiniton Channels = new EBMLElementDefiniton(0x9F, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Audio\Channels", defaultVal: "1");
      public static readonly EBMLElementDefiniton ChannelPositions = new EBMLElementDefiniton(0x7D7B, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\Audio\ChannelPositions");
      public static readonly EBMLElementDefiniton BitDepth = new EBMLElementDefiniton(0x6264, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\Audio\BitDepth");
      public static readonly EBMLElementDefiniton TrackOperation = new EBMLElementDefiniton(0xE2, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\TrackOperation");
      public static readonly EBMLElementDefiniton TrackCombinePlanes = new EBMLElementDefiniton(0xE3, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackCombinePlanes");
      public static readonly EBMLElementDefiniton TrackPlane = new EBMLElementDefiniton(0xE4, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackCombinePlanes\TrackPlane");
      public static readonly EBMLElementDefiniton TrackPlaneUID = new EBMLElementDefiniton(0xE5, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackCombinePlanes\TrackPlane\TrackPlaneUID");
      public static readonly EBMLElementDefiniton TrackPlaneType = new EBMLElementDefiniton(0xE6, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackCombinePlanes\TrackPlane\TrackPlaneType");
      public static readonly EBMLElementDefiniton TrackJoinBlocks = new EBMLElementDefiniton(0xE9, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackJoinBlocks");
      public static readonly EBMLElementDefiniton TrackJoinUID = new EBMLElementDefiniton(0xED, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrackOperation\TrackJoinBlocks\TrackJoinUID");
      public static readonly EBMLElementDefiniton TrickTrackUID = new EBMLElementDefiniton(0xC0, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrickTrackUID");
      public static readonly EBMLElementDefiniton TrickTrackSegmentUID = new EBMLElementDefiniton(0xC1, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\TrickTrackSegmentUID");
      public static readonly EBMLElementDefiniton TrickTrackFlag = new EBMLElementDefiniton(0xC6, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrickTrackFlag", defaultVal: "0");
      public static readonly EBMLElementDefiniton TrickMasterTrackUID = new EBMLElementDefiniton(0xC7, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\TrickMasterTrackUID");
      public static readonly EBMLElementDefiniton TrickMasterTrackSegmentUID = new EBMLElementDefiniton(0xC4, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\TrickMasterTrackSegmentUID");
      public static readonly EBMLElementDefiniton ContentEncodings = new EBMLElementDefiniton(0x6D80, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\ContentEncodings");
      public static readonly EBMLElementDefiniton ContentEncoding = new EBMLElementDefiniton(0x6240, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding");
      public static readonly EBMLElementDefiniton ContentEncodingOrder = new EBMLElementDefiniton(0x5031, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncodingOrder", defaultVal: "0");
      public static readonly EBMLElementDefiniton ContentEncodingScope = new EBMLElementDefiniton(0x5032, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncodingScope", defaultVal: "1");
      public static readonly EBMLElementDefiniton ContentEncodingType = new EBMLElementDefiniton(0x5033, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncodingType", defaultVal: "0");
      public static readonly EBMLElementDefiniton ContentCompression = new EBMLElementDefiniton(0x5034, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentCompression");
      public static readonly EBMLElementDefiniton ContentCompAlgo = new EBMLElementDefiniton(0x4254, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentCompression\ContentCompAlgo", defaultVal: "0");
      public static readonly EBMLElementDefiniton ContentCompSettings = new EBMLElementDefiniton(0x4255, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentCompression\ContentCompSettings");
      public static readonly EBMLElementDefiniton ContentEncryption = new EBMLElementDefiniton(0x5035, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption");
      public static readonly EBMLElementDefiniton ContentEncAlgo = new EBMLElementDefiniton(0x47E1, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentEncAlgo", defaultVal: "0");
      public static readonly EBMLElementDefiniton ContentEncKeyID = new EBMLElementDefiniton(0x47E2, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentEncKeyID");
      public static readonly EBMLElementDefiniton ContentEncAESSettings = new EBMLElementDefiniton(0x47E7, EBMLElementType.Master, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentEncAESSettings");
      public static readonly EBMLElementDefiniton AESSettingsCipherMode = new EBMLElementDefiniton(0x47E8, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentEncAESSettings\AESSettingsCipherMode");
      public static readonly EBMLElementDefiniton ContentSignature = new EBMLElementDefiniton(0x47E3, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentSignature");
      public static readonly EBMLElementDefiniton ContentSigKeyID = new EBMLElementDefiniton(0x47E4, EBMLElementType.Binary, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentSigKeyID");
      public static readonly EBMLElementDefiniton ContentSigAlgo = new EBMLElementDefiniton(0x47E5, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentSigAlgo", defaultVal: "0");
      public static readonly EBMLElementDefiniton ContentSigHashAlgo = new EBMLElementDefiniton(0x47E6, EBMLElementType.UnsignedInteger, @"\Segment\Tracks\TrackEntry\ContentEncodings\ContentEncoding\ContentEncryption\ContentSigHashAlgo", defaultVal: "0");
      public static readonly EBMLElementDefiniton Cues = new EBMLElementDefiniton(0x1C53BB6B, EBMLElementType.Master, @"\Segment\Cues");
      public static readonly EBMLElementDefiniton CuePoint = new EBMLElementDefiniton(0xBB, EBMLElementType.Master, @"\Segment\Cues\CuePoint");
      public static readonly EBMLElementDefiniton CueTime = new EBMLElementDefiniton(0xB3, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTime");
      public static readonly EBMLElementDefiniton CueTrackPositions = new EBMLElementDefiniton(0xB7, EBMLElementType.Master, @"\Segment\Cues\CuePoint\CueTrackPositions");
      public static readonly EBMLElementDefiniton CueTrack = new EBMLElementDefiniton(0xF7, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueTrack");
      public static readonly EBMLElementDefiniton CueClusterPosition = new EBMLElementDefiniton(0xF1, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueClusterPosition");
      public static readonly EBMLElementDefiniton CueRelativePosition = new EBMLElementDefiniton(0xF0, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueRelativePosition");
      public static readonly EBMLElementDefiniton CueDuration = new EBMLElementDefiniton(0xB2, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueDuration");
      public static readonly EBMLElementDefiniton CueBlockNumber = new EBMLElementDefiniton(0x5378, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueBlockNumber");
      public static readonly EBMLElementDefiniton CueCodecState = new EBMLElementDefiniton(0xEA, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueCodecState", defaultVal: "0");
      public static readonly EBMLElementDefiniton CueReference = new EBMLElementDefiniton(0xDB, EBMLElementType.Master, @"\Segment\Cues\CuePoint\CueTrackPositions\CueReference");
      public static readonly EBMLElementDefiniton CueRefTime = new EBMLElementDefiniton(0x96, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueReference\CueRefTime");
      public static readonly EBMLElementDefiniton CueRefCluster = new EBMLElementDefiniton(0x97, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueReference\CueRefCluster");
      public static readonly EBMLElementDefiniton CueRefNumber = new EBMLElementDefiniton(0x535F, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueReference\CueRefNumber", defaultVal: "1");
      public static readonly EBMLElementDefiniton CueRefCodecState = new EBMLElementDefiniton(0xEB, EBMLElementType.UnsignedInteger, @"\Segment\Cues\CuePoint\CueTrackPositions\CueReference\CueRefCodecState", defaultVal: "0");
      public static readonly EBMLElementDefiniton Attachments = new EBMLElementDefiniton(0x1941A469, EBMLElementType.Master, @"\Segment\Attachments");
      public static readonly EBMLElementDefiniton AttachedFile = new EBMLElementDefiniton(0x61A7, EBMLElementType.Master, @"\Segment\Attachments\AttachedFile");
      public static readonly EBMLElementDefiniton FileDescription = new EBMLElementDefiniton(0x467E, EBMLElementType.UTF8, @"\Segment\Attachments\AttachedFile\FileDescription");
      public static readonly EBMLElementDefiniton FileName = new EBMLElementDefiniton(0x466E, EBMLElementType.UTF8, @"\Segment\Attachments\AttachedFile\FileName");
      public static readonly EBMLElementDefiniton FileMimeType = new EBMLElementDefiniton(0x4660, EBMLElementType.String, @"\Segment\Attachments\AttachedFile\FileMimeType");
      public static readonly EBMLElementDefiniton FileData = new EBMLElementDefiniton(0x465C, EBMLElementType.Binary, @"\Segment\Attachments\AttachedFile\FileData");
      public static readonly EBMLElementDefiniton FileUID = new EBMLElementDefiniton(0x46AE, EBMLElementType.UnsignedInteger, @"\Segment\Attachments\AttachedFile\FileUID");
      public static readonly EBMLElementDefiniton FileReferral = new EBMLElementDefiniton(0x4675, EBMLElementType.Binary, @"\Segment\Attachments\AttachedFile\FileReferral");
      public static readonly EBMLElementDefiniton FileUsedStartTime = new EBMLElementDefiniton(0x4661, EBMLElementType.UnsignedInteger, @"\Segment\Attachments\AttachedFile\FileUsedStartTime");
      public static readonly EBMLElementDefiniton FileUsedEndTime = new EBMLElementDefiniton(0x4662, EBMLElementType.UnsignedInteger, @"\Segment\Attachments\AttachedFile\FileUsedEndTime");
      public static readonly EBMLElementDefiniton Chapters = new EBMLElementDefiniton(0x1043A770, EBMLElementType.Master, @"\Segment\Chapters");
      public static readonly EBMLElementDefiniton EditionEntry = new EBMLElementDefiniton(0x45B9, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry");
      public static readonly EBMLElementDefiniton EditionUID = new EBMLElementDefiniton(0x45BC, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\EditionUID");
      public static readonly EBMLElementDefiniton EditionFlagHidden = new EBMLElementDefiniton(0x45BD, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\EditionFlagHidden", defaultVal: "0");
      public static readonly EBMLElementDefiniton EditionFlagDefault = new EBMLElementDefiniton(0x45DB, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\EditionFlagDefault", defaultVal: "0");
      public static readonly EBMLElementDefiniton EditionFlagOrdered = new EBMLElementDefiniton(0x45DD, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\EditionFlagOrdered", defaultVal: "0");
      public static readonly EBMLElementDefiniton ChapterAtom = new EBMLElementDefiniton(0xB6, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry\+ChapterAtom");
      public static readonly EBMLElementDefiniton ChapterUID = new EBMLElementDefiniton(0x73C4, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterUID");
      public static readonly EBMLElementDefiniton ChapterStringUID = new EBMLElementDefiniton(0x5654, EBMLElementType.UTF8, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterStringUID");
      public static readonly EBMLElementDefiniton ChapterTimeStart = new EBMLElementDefiniton(0x91, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterTimeStart");
      public static readonly EBMLElementDefiniton ChapterTimeEnd = new EBMLElementDefiniton(0x92, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterTimeEnd");
      public static readonly EBMLElementDefiniton ChapterFlagHidden = new EBMLElementDefiniton(0x98, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterFlagHidden", defaultVal: "0");
      public static readonly EBMLElementDefiniton ChapterFlagEnabled = new EBMLElementDefiniton(0x4598, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterFlagEnabled", defaultVal: "1");
      public static readonly EBMLElementDefiniton ChapterSegmentUID = new EBMLElementDefiniton(0x6E67, EBMLElementType.Binary, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterSegmentUID");
      public static readonly EBMLElementDefiniton ChapterSegmentEditionUID = new EBMLElementDefiniton(0x6EBC, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterSegmentEditionUID");
      public static readonly EBMLElementDefiniton ChapterPhysicalEquiv = new EBMLElementDefiniton(0x63C3, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterPhysicalEquiv");
      public static readonly EBMLElementDefiniton ChapterTrack = new EBMLElementDefiniton(0x8F, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterTrack");
      public static readonly EBMLElementDefiniton ChapterTrackUID = new EBMLElementDefiniton(0x89, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterTrack\ChapterTrackUID");
      public static readonly EBMLElementDefiniton ChapterDisplay = new EBMLElementDefiniton(0x80, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterDisplay");
      public static readonly EBMLElementDefiniton ChapString = new EBMLElementDefiniton(0x85, EBMLElementType.UTF8, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterDisplay\ChapString");
      public static readonly EBMLElementDefiniton ChapLanguage = new EBMLElementDefiniton(0x437C, EBMLElementType.String, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterDisplay\ChapLanguage", defaultVal: "eng");
      public static readonly EBMLElementDefiniton ChapLanguageIETF = new EBMLElementDefiniton(0x437D, EBMLElementType.String, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterDisplay\ChapLanguageIETF");
      public static readonly EBMLElementDefiniton ChapCountry = new EBMLElementDefiniton(0x437E, EBMLElementType.String, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapterDisplay\ChapCountry");
      public static readonly EBMLElementDefiniton ChapProcess = new EBMLElementDefiniton(0x6944, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess");
      public static readonly EBMLElementDefiniton ChapProcessCodecID = new EBMLElementDefiniton(0x6955, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess\ChapProcessCodecID", defaultVal: "0");
      public static readonly EBMLElementDefiniton ChapProcessPrivate = new EBMLElementDefiniton(0x450D, EBMLElementType.Binary, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess\ChapProcessPrivate");
      public static readonly EBMLElementDefiniton ChapProcessCommand = new EBMLElementDefiniton(0x6911, EBMLElementType.Master, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess\ChapProcessCommand");
      public static readonly EBMLElementDefiniton ChapProcessTime = new EBMLElementDefiniton(0x6922, EBMLElementType.UnsignedInteger, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess\ChapProcessCommand\ChapProcessTime");
      public static readonly EBMLElementDefiniton ChapProcessData = new EBMLElementDefiniton(0x6933, EBMLElementType.Binary, @"\Segment\Chapters\EditionEntry\+ChapterAtom\ChapProcess\ChapProcessCommand\ChapProcessData");
      public static readonly EBMLElementDefiniton Tags = new EBMLElementDefiniton(0x1254C367, EBMLElementType.Master, @"\Segment\Tags");
      public static readonly EBMLElementDefiniton Tag = new EBMLElementDefiniton(0x7373, EBMLElementType.Master, @"\Segment\Tags\Tag");
      public static readonly EBMLElementDefiniton Targets = new EBMLElementDefiniton(0x63C0, EBMLElementType.Master, @"\Segment\Tags\Tag\Targets");
      public static readonly EBMLElementDefiniton TargetTypeValue = new EBMLElementDefiniton(0x68CA, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\Targets\TargetTypeValue", defaultVal: "50");
      public static readonly EBMLElementDefiniton TargetType = new EBMLElementDefiniton(0x63CA, EBMLElementType.String, @"\Segment\Tags\Tag\Targets\TargetType");
      public static readonly EBMLElementDefiniton TagTrackUID = new EBMLElementDefiniton(0x63C5, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\Targets\TagTrackUID", defaultVal: "0");
      public static readonly EBMLElementDefiniton TagEditionUID = new EBMLElementDefiniton(0x63C9, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\Targets\TagEditionUID", defaultVal: "0");
      public static readonly EBMLElementDefiniton TagChapterUID = new EBMLElementDefiniton(0x63C4, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\Targets\TagChapterUID", defaultVal: "0");
      public static readonly EBMLElementDefiniton TagAttachmentUID = new EBMLElementDefiniton(0x63C6, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\Targets\TagAttachmentUID", defaultVal: "0");
      public static readonly EBMLElementDefiniton SimpleTag = new EBMLElementDefiniton(0x67C8, EBMLElementType.Master, @"\Segment\Tags\Tag\+SimpleTag");
      public static readonly EBMLElementDefiniton TagName = new EBMLElementDefiniton(0x45A3, EBMLElementType.UTF8, @"\Segment\Tags\Tag\+SimpleTag\TagName");
      public static readonly EBMLElementDefiniton TagLanguage = new EBMLElementDefiniton(0x447A, EBMLElementType.String, @"\Segment\Tags\Tag\+SimpleTag\TagLanguage", defaultVal: "und");
      public static readonly EBMLElementDefiniton TagLanguageIETF = new EBMLElementDefiniton(0x447B, EBMLElementType.String, @"\Segment\Tags\Tag\+SimpleTag\TagLanguageIETF");
      public static readonly EBMLElementDefiniton TagDefault = new EBMLElementDefiniton(0x4484, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\+SimpleTag\TagDefault", defaultVal: "1");
      public static readonly EBMLElementDefiniton TagDefaultBogus = new EBMLElementDefiniton(0x44B4, EBMLElementType.UnsignedInteger, @"\Segment\Tags\Tag\+SimpleTag\TagDefaultBogus", defaultVal: "1");
      public static readonly EBMLElementDefiniton TagString = new EBMLElementDefiniton(0x4487, EBMLElementType.UTF8, @"\Segment\Tags\Tag\+SimpleTag\TagString");
      public static readonly EBMLElementDefiniton TagBinary = new EBMLElementDefiniton(0x4485, EBMLElementType.Binary, @"\Segment\Tags\Tag\+SimpleTag\TagBinary");

      private static readonly EBMLElementDefiniton[] elements = new EBMLElementDefiniton[]
      {
         Segment, SeekHead, Seek, SeekID, SeekPosition, Info, SegmentUID, SegmentFilename, PrevUID, PrevFilename,
         NextUID, NextFilename, SegmentFamily, ChapterTranslate, ChapterTranslateID, ChapterTranslateCodec, ChapterTranslateEditionUID, TimestampScale,
         Duration, DateUTC, Title, MuxingApp, WritingApp, Cluster, Timestamp, SilentTracks, SilentTrackNumber, Position,
         PrevSize, SimpleBlock, BlockGroup, Block, BlockVirtual, BlockAdditions, BlockMore, BlockAddID, BlockAdditional, BlockDuration,
         ReferencePriority, ReferenceBlock, ReferenceVirtual, CodecState, DiscardPadding, Slices, TimeSlice, LaceNumber, FrameNumber, BlockAdditionID,
         Delay, SliceDuration, ReferenceFrame, ReferenceOffset, ReferenceTimestamp, EncryptedBlock, Tracks, TrackEntry, TrackNumber, TrackUID,
         TrackType, FlagEnabled, FlagDefault, FlagForced, FlagHearingImpaired, FlagVisualImpaired, FlagTextDescriptions, FlagOriginal, FlagCommentary, FlagLacing,
         MinCache, MaxCache, DefaultDuration, DefaultDecodedFieldDuration, TrackTimestampScale, TrackOffset, MaxBlockAdditionID, BlockAdditionMapping, BlockAddIDValue, BlockAddIDName,
         BlockAddIDType, BlockAddIDExtraData, Name, Language, LanguageIETF, CodecID, CodecPrivate, CodecName, AttachmentLink, CodecSettings,
         CodecInfoURL, CodecDownloadURL, CodecDecodeAll, TrackOverlay, CodecDelay, SeekPreRoll, TrackTranslate, TrackTranslateTrackID, TrackTranslateCodec, TrackTranslateEditionUID,
         Video, FlagInterlaced, FieldOrder, StereoMode, AlphaMode, OldStereoMode, PixelWidth, PixelHeight, PixelCropBottom, PixelCropTop,
         PixelCropLeft, PixelCropRight, DisplayWidth, DisplayHeight, DisplayUnit, AspectRatioType, UncompressedFourCC, GammaValue, FrameRate, Colour,
         MatrixCoefficients, BitsPerChannel, ChromaSubsamplingHorz, ChromaSubsamplingVert, CbSubsamplingHorz, CbSubsamplingVert, ChromaSitingHorz, ChromaSitingVert, Range, TransferCharacteristics,
         Primaries, MaxCLL, MaxFALL, MasteringMetadata, PrimaryRChromaticityX, PrimaryRChromaticityY, PrimaryGChromaticityX, PrimaryGChromaticityY, PrimaryBChromaticityX, PrimaryBChromaticityY,
         WhitePointChromaticityX, WhitePointChromaticityY, LuminanceMax, LuminanceMin, Projection, ProjectionType, ProjectionPrivate, ProjectionPoseYaw, ProjectionPosePitch, ProjectionPoseRoll,
         Audio, SamplingFrequency, OutputSamplingFrequency, Channels, ChannelPositions, BitDepth, TrackOperation, TrackCombinePlanes, TrackPlane, TrackPlaneUID,
         TrackPlaneType, TrackJoinBlocks, TrackJoinUID, TrickTrackUID, TrickTrackSegmentUID, TrickTrackFlag, TrickMasterTrackUID, TrickMasterTrackSegmentUID, ContentEncodings, ContentEncoding,
         ContentEncodingOrder, ContentEncodingScope, ContentEncodingType, ContentCompression, ContentCompAlgo, ContentCompSettings, ContentEncryption, ContentEncAlgo, ContentEncKeyID, ContentEncAESSettings,
         AESSettingsCipherMode, ContentSignature, ContentSigKeyID, ContentSigAlgo, ContentSigHashAlgo, Cues, CuePoint, CueTime, CueTrackPositions, CueTrack,
         CueClusterPosition, CueRelativePosition, CueDuration, CueBlockNumber, CueCodecState, CueReference, CueRefTime, CueRefCluster, CueRefNumber, CueRefCodecState,
         Attachments, AttachedFile, FileDescription, FileName, FileMimeType, FileData, FileUID, FileReferral, FileUsedStartTime, FileUsedEndTime,
         Chapters, EditionEntry, EditionUID, EditionFlagHidden, EditionFlagDefault, EditionFlagOrdered, ChapterAtom, ChapterUID, ChapterStringUID, ChapterTimeStart,
         ChapterTimeEnd, ChapterFlagHidden, ChapterFlagEnabled, ChapterSegmentUID, ChapterSegmentEditionUID, ChapterPhysicalEquiv, ChapterTrack, ChapterTrackUID, ChapterDisplay, ChapString,
         ChapLanguage, ChapLanguageIETF, ChapCountry, ChapProcess, ChapProcessCodecID, ChapProcessPrivate, ChapProcessCommand, ChapProcessTime, ChapProcessData, Tags,
         Tag, Targets, TargetTypeValue, TargetType, TagTrackUID, TagEditionUID, TagChapterUID, TagAttachmentUID, SimpleTag, TagName,
         TagLanguage, TagLanguageIETF, TagDefault, TagDefaultBogus, TagString, TagBinary,
      };

      public static void AddElements(EBMLReader reader)
      {
         for (int i = 0; i < elements.Length; i++)
         {
            reader.AddElementDefinition(elements[i]);
         }
      }

      private static bool registered;

      public static void RegisterFormat()
      {
         if (registered) { return; }
         registered = true;
         EBMLDocTypeLookup.AddEBMLDocType((header, reader) =>
         {
            if (header.DocType == "matroska" || header.DocType == "webm")
            {
               AddElements(reader);
               return true;
            }
            return false;
         });
      }
   }
}
