using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NEbml.Core;

namespace Matroska
{
    /// <summary>
    /// Contains the EBML elements specified by the Matroska project.
    /// </summary>
    /// <remarks>
    /// See https://matroska.org/technical/specs/index.html for more info.
    /// </remarks>
    public static class MatroskaSpecification
    {
        #region Helper
        private static readonly Dictionary<VInt, ElementDescriptor> _elementDescriptorsByVInt = new Dictionary<VInt, ElementDescriptor>();
        private static readonly Dictionary<ulong, ElementDescriptor> _elementDescriptorsByIdentifier = new Dictionary<ulong, ElementDescriptor>();

        /// <summary>
        /// Gets a dictionary of all Matroska elements.
        /// </summary>
        public static IReadOnlyDictionary<VInt, ElementDescriptor> ElementDescriptors => _elementDescriptorsByVInt;

        public static IReadOnlyDictionary<ulong, ElementDescriptor> ElementDescriptorsByIdentifier => _elementDescriptorsByIdentifier;

        static MatroskaSpecification()
        {
            var fields = typeof(MatroskaSpecification).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields.Where(ft => ft.FieldType == typeof(ElementDescriptor)))
            {
                var value = (ElementDescriptor)field.GetValue(null);
                _elementDescriptorsByVInt.Add(value.Identifier, value);
            }

            foreach (FieldInfo field in fields.Where(ft => ft.FieldType == typeof(ulong)))
            {
                var identifier = (ulong)field.GetValue(null);
                _elementDescriptorsByIdentifier.Add(identifier, _elementDescriptorsByVInt[NEbml.Core.VInt.FromEncoded(identifier)]);
            }
        }
        #endregion

        #region Definitions
        /// <summary>Set the EBML characteristics of the data to follow. Each EBML document has to start with this.</summary>
        public const ulong EBML = 0x1A45DFA3;
        public static readonly ElementDescriptor EBMLDescriptor = new ElementDescriptor((long)EBML, nameof(EBML), ElementType.MasterElement);

        /// <summary>The version of EBML parser used to create the file.</summary>
        public const ulong EBMLVersion = 0x00004286;
        public static readonly ElementDescriptor EBMLVersionDescriptor = new ElementDescriptor((long)EBMLVersion, nameof(EBMLVersion), ElementType.UnsignedInteger);

        /// <summary>The minimum EBML version a parser has to support to read this file.</summary>
        public const ulong EBMLReadVersion = 0x000042F7;
        public static readonly ElementDescriptor EBMLReadVersionDescriptor = new ElementDescriptor((long)EBMLReadVersion, nameof(EBMLReadVersion), ElementType.UnsignedInteger);

        /// <summary>The maximum length of the IDs you'll find in this file (4 or less in Matroska).</summary>
        public const ulong EBMLMaxIDLength = 0x000042F2;
        public static readonly ElementDescriptor EBMLMaxIDLengthDescriptor = new ElementDescriptor((long)EBMLMaxIDLength, nameof(EBMLMaxIDLength), ElementType.UnsignedInteger);

        /// <summary>The maximum length of the sizes you'll find in this file (8 or less in Matroska). This does not override the Element size indicated at the beginning of an Element. Elements that have an indicated size which is larger than what is allowed by EBMLMaxSizeLength shall be considered invalid.</summary>
        public const ulong EBMLMaxSizeLength = 0x000042F3;
        public static readonly ElementDescriptor EBMLMaxSizeLengthDescriptor = new ElementDescriptor((long)EBMLMaxSizeLength, nameof(EBMLMaxSizeLength), ElementType.UnsignedInteger);

        /// <summary>A string that describes the type of document that follows this EBML header. 'matroska' in our case or 'webm' for webm files.</summary>
        public const ulong DocType = 0x00004282;
        public static readonly ElementDescriptor DocTypeDescriptor = new ElementDescriptor((long)DocType, nameof(DocType), ElementType.AsciiString);

        /// <summary>The version of DocType interpreter used to create the file.</summary>
        public const ulong DocTypeVersion = 0x00004287;
        public static readonly ElementDescriptor DocTypeVersionDescriptor = new ElementDescriptor((long)DocTypeVersion, nameof(DocTypeVersion), ElementType.UnsignedInteger);

        /// <summary>The minimum DocType version an interpreter has to support to read this file.</summary>
        public const ulong DocTypeReadVersion = 0x00004285;
        public static readonly ElementDescriptor DocTypeReadVersionDescriptor = new ElementDescriptor((long)DocTypeReadVersion, nameof(DocTypeReadVersion), ElementType.UnsignedInteger);

        /// <summary>Used to void damaged data, to avoid unexpected behaviors when using damaged data. The content is discarded. Also used to reserve space in a sub-element for later use.</summary>
        public const ulong Void = 0x000000EC;
        public static readonly ElementDescriptor VoidDescriptor = new ElementDescriptor((long)Void, nameof(Void), ElementType.Binary);

        /// <summary>The CRC is computed on all the data of the Master-element it's in. The CRC Element should be the first in it's parent master for easier reading. All level 1 Elements should include a CRC-32. The CRC in use is the IEEE CRC32 Little Endian</summary>
        public const ulong CRC32 = 0x000000BF;
        public static readonly ElementDescriptor CRC32Descriptor = new ElementDescriptor((long)CRC32, "CRC-32", ElementType.Binary);

        /// <summary>Contain signature of some (coming) Elements in the stream.</summary>
        public const ulong SignatureSlot = 0x1B538667;
        public static readonly ElementDescriptor SignatureSlotDescriptor = new ElementDescriptor((long)SignatureSlot, nameof(SignatureSlot), ElementType.MasterElement);

        /// <summary>Signature algorithm used (1=RSA, 2=elliptic).</summary>
        public const ulong SignatureAlgo = 0x00007E8A;
        public static readonly ElementDescriptor SignatureAlgoDescriptor = new ElementDescriptor((long)SignatureAlgo, nameof(SignatureAlgo), ElementType.UnsignedInteger);

        /// <summary>Hash algorithm used (1=SHA1-160, 2=MD5).</summary>
        public const ulong SignatureHash = 0x00007E9A;
        public static readonly ElementDescriptor SignatureHashDescriptor = new ElementDescriptor((long)SignatureHash, nameof(SignatureHash), ElementType.UnsignedInteger);

        /// <summary>The public key to use with the algorithm (in the case of a PKI-based signature).</summary>
        public const ulong SignaturePublicKey = 0x00007EA5;
        public static readonly ElementDescriptor SignaturePublicKeyDescriptor = new ElementDescriptor((long)SignaturePublicKey, nameof(SignaturePublicKey), ElementType.Binary);

        /// <summary>The signature of the data (until a new.</summary>
        public const ulong Signature = 0x00007EB5;
        public static readonly ElementDescriptor SignatureDescriptor = new ElementDescriptor((long)Signature, nameof(Signature), ElementType.Binary);

        /// <summary>Contains Elements that will be used to compute the signature.</summary>
        public const ulong SignatureElements = 0x00007E5B;
        public static readonly ElementDescriptor SignatureElementsDescriptor = new ElementDescriptor((long)SignatureElements, nameof(SignatureElements), ElementType.MasterElement);

        /// <summary>A list consists of a number of consecutive Elements that represent one case where data is used in signature. Ex: Cluster|Block|BlockAdditional means that the BlockAdditional of all Blocks in all Clusters is used for encryption.</summary>
        public const ulong SignatureElementList = 0x00007E7B;
        public static readonly ElementDescriptor SignatureElementListDescriptor = new ElementDescriptor((long)SignatureElementList, nameof(SignatureElementList), ElementType.MasterElement);

        /// <summary>An Element ID whose data will be used to compute the signature.</summary>
        public const ulong SignedElement = 0x00006532;
        public static readonly ElementDescriptor SignedElementDescriptor = new ElementDescriptor((long)SignedElement, nameof(SignedElement), ElementType.Binary);

        /// <summary>The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.</summary>
        public const ulong Segment = 0x18538067;
        public static readonly ElementDescriptor SegmentDescriptor = new ElementDescriptor((long)Segment, nameof(Segment), ElementType.MasterElement);

        /// <summary>Contains the position of other Top-Level Elements.</summary>
        public const ulong SeekHead = 0x114D9B74;
        public static readonly ElementDescriptor SeekHeadDescriptor = new ElementDescriptor((long)SeekHead, nameof(SeekHead), ElementType.MasterElement);

        /// <summary>Contains a single seek entry to an EBML Element.</summary>
        public const ulong Seek = 0x00004DBB;
        public static readonly ElementDescriptor SeekDescriptor = new ElementDescriptor((long)Seek, nameof(Seek), ElementType.MasterElement);

        /// <summary>The binary ID corresponding to the Element name.</summary>
        public const ulong SeekID = 0x000053AB;
        public static readonly ElementDescriptor SeekIDDescriptor = new ElementDescriptor((long)SeekID, nameof(SeekID), ElementType.Binary);

        /// <summary>The position of the Element in the Segment in octets (0 = first level 1 Element).</summary>
        public const ulong SeekPosition = 0x000053AC;
        public static readonly ElementDescriptor SeekPositionDescriptor = new ElementDescriptor((long)SeekPosition, nameof(SeekPosition), ElementType.UnsignedInteger);

        /// <summary>Contains miscellaneous general information and statistics on the file.</summary>
        public const ulong Info = 0x1549A966;
        public static readonly ElementDescriptor InfoDescriptor = new ElementDescriptor((long)Info, nameof(Info), ElementType.MasterElement);

        /// <summary>A randomly generated unique ID to identify the current Segment between many others (128 bits).</summary>
        public const ulong SegmentUID = 0x000073A4;
        public static readonly ElementDescriptor SegmentUIDDescriptor = new ElementDescriptor((long)SegmentUID, nameof(SegmentUID), ElementType.Binary);

        /// <summary>A filename corresponding to this Segment.</summary>
        public const ulong SegmentFilename = 0x00007384;
        public static readonly ElementDescriptor SegmentFilenameDescriptor = new ElementDescriptor((long)SegmentFilename, nameof(SegmentFilename), ElementType.Utf8String);

        /// <summary>A unique ID to identify the previous chained Segment (128 bits).</summary>
        public const ulong PrevUID = 0x003CB923;
        public static readonly ElementDescriptor PrevUIDDescriptor = new ElementDescriptor((long)PrevUID, nameof(PrevUID), ElementType.Binary);

        /// <summary>An escaped filename corresponding to the previous Segment.</summary>
        public const ulong PrevFilename = 0x003C83AB;
        public static readonly ElementDescriptor PrevFilenameDescriptor = new ElementDescriptor((long)PrevFilename, nameof(PrevFilename), ElementType.Utf8String);

        /// <summary>A unique ID to identify the next chained Segment (128 bits).</summary>
        public const ulong NextUID = 0x003EB923;
        public static readonly ElementDescriptor NextUIDDescriptor = new ElementDescriptor((long)NextUID, nameof(NextUID), ElementType.Binary);

        /// <summary>An escaped filename corresponding to the next Segment.</summary>
        public const ulong NextFilename = 0x003E83BB;
        public static readonly ElementDescriptor NextFilenameDescriptor = new ElementDescriptor((long)NextFilename, nameof(NextFilename), ElementType.Utf8String);

        /// <summary>A randomly generated unique ID that all Segments related to each other must use (128 bits).</summary>
        public const ulong SegmentFamily = 0x00004444;
        public static readonly ElementDescriptor SegmentFamilyDescriptor = new ElementDescriptor((long)SegmentFamily, nameof(SegmentFamily), ElementType.Binary);

        /// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
        public const ulong ChapterTranslate = 0x00006924;
        public static readonly ElementDescriptor ChapterTranslateDescriptor = new ElementDescriptor((long)ChapterTranslate, nameof(ChapterTranslate), ElementType.MasterElement);

        /// <summary>Specify an edition UID on which this correspondance applies. When not specified, it means for all editions found in the Segment.</summary>
        public const ulong ChapterTranslateEditionUID = 0x000069FC;
        public static readonly ElementDescriptor ChapterTranslateEditionUIDDescriptor = new ElementDescriptor((long)ChapterTranslateEditionUID, nameof(ChapterTranslateEditionUID), ElementType.UnsignedInteger);

        /// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
        public const ulong ChapterTranslateCodec = 0x000069BF;
        public static readonly ElementDescriptor ChapterTranslateCodecDescriptor = new ElementDescriptor((long)ChapterTranslateCodec, nameof(ChapterTranslateCodec), ElementType.UnsignedInteger);

        /// <summary>The binary value used to represent this Segment in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
        public const ulong ChapterTranslateID = 0x000069A5;
        public static readonly ElementDescriptor ChapterTranslateIDDescriptor = new ElementDescriptor((long)ChapterTranslateID, nameof(ChapterTranslateID), ElementType.Binary);

        /// <summary>Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).</summary>
        public const ulong TimestampScale = 0x002AD7B1;
        public static readonly ElementDescriptor TimestampScaleDescriptor = new ElementDescriptor((long)TimestampScale, nameof(TimestampScale), ElementType.UnsignedInteger);

        /// <summary>Duration of the Segment (based on TimecodeScale).</summary>
        public const ulong Duration = 0x00004489;
        public static readonly ElementDescriptor DurationDescriptor = new ElementDescriptor((long)Duration, nameof(Duration), ElementType.Float);

        /// <summary>Date of the origin of timestamp (value 0), i.e. production date.</summary>
        public const ulong DateUTC = 0x00004461;
        public static readonly ElementDescriptor DateUTCDescriptor = new ElementDescriptor((long)DateUTC, nameof(DateUTC), ElementType.Date);

        /// <summary>General name of the Segment.</summary>
        public const ulong Title = 0x00007BA9;
        public static readonly ElementDescriptor TitleDescriptor = new ElementDescriptor((long)Title, nameof(Title), ElementType.Utf8String);

        /// <summary>Muxing application or library ("libmatroska-0.4.3").</summary>
        public const ulong MuxingApp = 0x00004D80;
        public static readonly ElementDescriptor MuxingAppDescriptor = new ElementDescriptor((long)MuxingApp, nameof(MuxingApp), ElementType.Utf8String);

        /// <summary>Writing application ("mkvmerge-0.3.3").</summary>
        public const ulong WritingApp = 0x00005741;
        public static readonly ElementDescriptor WritingAppDescriptor = new ElementDescriptor((long)WritingApp, nameof(WritingApp), ElementType.Utf8String);

        /// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
        public const ulong Cluster = 0x1F43B675;
        public static readonly ElementDescriptor ClusterDescriptor = new ElementDescriptor((long)Cluster, nameof(Cluster), ElementType.MasterElement);

        /// <summary>Absolute timestamp of the cluster (based on TimecodeScale).</summary>
        public const ulong Timecode = 0x000000E7;
        public static readonly ElementDescriptor TimecodeDescriptor = new ElementDescriptor((long)Timecode, nameof(Timecode), ElementType.UnsignedInteger);

        /// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking. Then you should decide what track to use.</summary>
        public const ulong SilentTracks = 0x00005854;
        public static readonly ElementDescriptor SilentTracksDescriptor = new ElementDescriptor((long)SilentTracks, nameof(SilentTracks), ElementType.MasterElement);

        /// <summary>One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.</summary>
        public const ulong SilentTrackNumber = 0x000058D7;
        public static readonly ElementDescriptor SilentTrackNumberDescriptor = new ElementDescriptor((long)SilentTrackNumber, nameof(SilentTrackNumber), ElementType.UnsignedInteger);

        /// <summary>The Position of the Cluster in the Segment (0 in live broadcast streams). It might help to resynchronise offset on damaged streams.</summary>
        public const ulong Position = 0x000000A7;
        public static readonly ElementDescriptor PositionDescriptor = new ElementDescriptor((long)Position, nameof(Position), ElementType.UnsignedInteger);

        /// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
        public const ulong PrevSize = 0x000000AB;
        public static readonly ElementDescriptor PrevSizeDescriptor = new ElementDescriptor((long)PrevSize, nameof(PrevSize), ElementType.UnsignedInteger);

        /// <summary>Similar to Block but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see SimpleBlock Structure)</summary>
        public const ulong SimpleBlock = 0x000000A3;
        public static readonly ElementDescriptor SimpleBlockDescriptor = new ElementDescriptor((long)SimpleBlock, nameof(SimpleBlock), ElementType.Binary);

        /// <summary>Basic container of information containing a single Block or BlockVirtual, and information specific to that Block/VirtualBlock.</summary>
        public const ulong BlockGroup = 0x000000A0;
        public static readonly ElementDescriptor BlockGroupDescriptor = new ElementDescriptor((long)BlockGroup, nameof(BlockGroup), ElementType.MasterElement);

        /// <summary>Block containing the actual data to be rendered and a timestamp relative to the Cluster Timecode. (see Block Structure)</summary>
        public const ulong Block = 0x000000A1;
        public static readonly ElementDescriptor BlockDescriptor = new ElementDescriptor((long)Block, nameof(Block), ElementType.Binary);

        /// <summary>A Block with no data. It must be stored in the stream at the place the real Block should be in display order. (see Block Virtual)</summary>
        public const ulong BlockVirtual = 0x000000A2;
        public static readonly ElementDescriptor BlockVirtualDescriptor = new ElementDescriptor((long)BlockVirtual, nameof(BlockVirtual), ElementType.Binary);

        /// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
        public const ulong BlockAdditions = 0x000075A1;
        public static readonly ElementDescriptor BlockAdditionsDescriptor = new ElementDescriptor((long)BlockAdditions, nameof(BlockAdditions), ElementType.MasterElement);

        /// <summary>Contain the BlockAdditional and some parameters.</summary>
        public const ulong BlockMore = 0x000000A6;
        public static readonly ElementDescriptor BlockMoreDescriptor = new ElementDescriptor((long)BlockMore, nameof(BlockMore), ElementType.MasterElement);

        /// <summary>An ID to identify the BlockAdditional level.</summary>
        public const ulong BlockAddID = 0x000000EE;
        public static readonly ElementDescriptor BlockAddIDDescriptor = new ElementDescriptor((long)BlockAddID, nameof(BlockAddID), ElementType.UnsignedInteger);

        /// <summary>Interpreted by the codec as it wishes (using the BlockAddID).</summary>
        public const ulong BlockAdditional = 0x000000A5;
        public static readonly ElementDescriptor BlockAdditionalDescriptor = new ElementDescriptor((long)BlockAdditional, nameof(BlockAdditional), ElementType.Binary);

        /// <summary>The duration of the Block (based on TimecodeScale). This Element is mandatory when DefaultDuration is set for the track (but can be omitted as other default values). When not written and with no DefaultDuration, the value is assumed to be the difference between the timestamp of this Block and the timestamp of the next Block in "display" order (not coding order). This Element can be useful at the end of a Track (as there is not other Block available), or when there is a break in a track like for subtitle tracks. When set to 0 that means the frame is not a keyframe.</summary>
        public const ulong BlockDuration = 0x0000009B;
        public static readonly ElementDescriptor BlockDurationDescriptor = new ElementDescriptor((long)BlockDuration, nameof(BlockDuration), ElementType.UnsignedInteger);

        /// <summary>This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.</summary>
        public const ulong ReferencePriority = 0x000000FA;
        public static readonly ElementDescriptor ReferencePriorityDescriptor = new ElementDescriptor((long)ReferencePriority, nameof(ReferencePriority), ElementType.UnsignedInteger);

        /// <summary>Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.</summary>
        public const ulong ReferenceBlock = 0x000000FB;
        public static readonly ElementDescriptor ReferenceBlockDescriptor = new ElementDescriptor((long)ReferenceBlock, nameof(ReferenceBlock), ElementType.SignedInteger);

        /// <summary>Relative position of the data that should be in position of the virtual block.</summary>
        public const ulong ReferenceVirtual = 0x000000FD;
        public static readonly ElementDescriptor ReferenceVirtualDescriptor = new ElementDescriptor((long)ReferenceVirtual, nameof(ReferenceVirtual), ElementType.SignedInteger);

        /// <summary>The new codec state to use. Data interpretation is private to the codec. This information should always be referenced by a seek entry.</summary>
        public const ulong CodecState = 0x000000A4;
        public static readonly ElementDescriptor CodecStateDescriptor = new ElementDescriptor((long)CodecState, nameof(CodecState), ElementType.Binary);

        /// <summary>Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in the duration of the TrackEntry and should be discarded during playback.</summary>
        public const ulong DiscardPadding = 0x000075A2;
        public static readonly ElementDescriptor DiscardPaddingDescriptor = new ElementDescriptor((long)DiscardPadding, nameof(DiscardPadding), ElementType.SignedInteger);

        /// <summary>Contains slices description.</summary>
        public const ulong Slices = 0x0000008E;
        public static readonly ElementDescriptor SlicesDescriptor = new ElementDescriptor((long)Slices, nameof(Slices), ElementType.MasterElement);

        /// <summary>Contains extra time information about the data contained in the Block. While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
        public const ulong TimeSlice = 0x000000E8;
        public static readonly ElementDescriptor TimeSliceDescriptor = new ElementDescriptor((long)TimeSlice, nameof(TimeSlice), ElementType.MasterElement);

        /// <summary>The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). While there are a few files in the wild with this Element, it is no longer in use and has been deprecated. Being able to interpret this Element is not required for playback.</summary>
        public const ulong LaceNumber = 0x000000CC;
        public static readonly ElementDescriptor LaceNumberDescriptor = new ElementDescriptor((long)LaceNumber, nameof(LaceNumber), ElementType.UnsignedInteger);

        /// <summary>The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).</summary>
        public const ulong FrameNumber = 0x000000CD;
        public static readonly ElementDescriptor FrameNumberDescriptor = new ElementDescriptor((long)FrameNumber, nameof(FrameNumber), ElementType.UnsignedInteger);

        /// <summary>The ID of the BlockAdditional Element (0 is the main Block).</summary>
        public const ulong BlockAdditionID = 0x000000CB;
        public static readonly ElementDescriptor BlockAdditionIDDescriptor = new ElementDescriptor((long)BlockAdditionID, nameof(BlockAdditionID), ElementType.UnsignedInteger);

        /// <summary>The (scaled) delay to apply to the Element.</summary>
        public const ulong Delay = 0x000000CE;
        public static readonly ElementDescriptor DelayDescriptor = new ElementDescriptor((long)Delay, nameof(Delay), ElementType.UnsignedInteger);

        /// <summary>The (scaled) duration to apply to the Element.</summary>
        public const ulong SliceDuration = 0x000000CF;
        public static readonly ElementDescriptor SliceDurationDescriptor = new ElementDescriptor((long)SliceDuration, nameof(SliceDuration), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong ReferenceFrame = 0x000000C8;
        public static readonly ElementDescriptor ReferenceFrameDescriptor = new ElementDescriptor((long)ReferenceFrame, nameof(ReferenceFrame), ElementType.MasterElement);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong ReferenceOffset = 0x000000C9;
        public static readonly ElementDescriptor ReferenceOffsetDescriptor = new ElementDescriptor((long)ReferenceOffset, nameof(ReferenceOffset), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong ReferenceTimeCode = 0x000000CA;
        public static readonly ElementDescriptor ReferenceTimeCodeDescriptor = new ElementDescriptor((long)ReferenceTimeCode, nameof(ReferenceTimeCode), ElementType.UnsignedInteger);

        /// <summary>Similar to SimpleBlock but the data inside the Block are Transformed (encrypt and/or signed). (see EncryptedBlock Structure)</summary>
        public const ulong EncryptedBlock = 0x000000AF;
        public static readonly ElementDescriptor EncryptedBlockDescriptor = new ElementDescriptor((long)EncryptedBlock, nameof(EncryptedBlock), ElementType.Binary);

        /// <summary>A Top-Level Element of information with many tracks described.</summary>
        public const ulong Tracks = 0x1654AE6B;
        public static readonly ElementDescriptor TracksDescriptor = new ElementDescriptor((long)Tracks, nameof(Tracks), ElementType.MasterElement);

        /// <summary>Describes a track with all Elements.</summary>
        public const ulong TrackEntry = 0x000000AE;
        public static readonly ElementDescriptor TrackEntryDescriptor = new ElementDescriptor((long)TrackEntry, nameof(TrackEntry), ElementType.MasterElement);

        /// <summary>The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).</summary>
        public const ulong TrackNumber = 0x000000D7;
        public static readonly ElementDescriptor TrackNumberDescriptor = new ElementDescriptor((long)TrackNumber, nameof(TrackNumber), ElementType.UnsignedInteger);

        /// <summary>A unique ID to identify the Track. This should be kept the same when making a direct stream copy of the Track to another file.</summary>
        public const ulong TrackUID = 0x000073C5;
        public static readonly ElementDescriptor TrackUIDDescriptor = new ElementDescriptor((long)TrackUID, nameof(TrackUID), ElementType.UnsignedInteger);

        /// <summary>A set of track types coded on 8 bits (1: video, 2: audio, 3: complex, 0x10: logo, 0x11: subtitle, 0x12: buttons, 0x20: control).</summary>
        public const ulong TrackType = 0x00000083;
        public static readonly ElementDescriptor TrackTypeDescriptor = new ElementDescriptor((long)TrackType, nameof(TrackType), ElementType.UnsignedInteger);

        /// <summary>Set if the track is usable. (1 bit)</summary>
        public const ulong FlagEnabled = 0x000000B9;
        public static readonly ElementDescriptor FlagEnabledDescriptor = new ElementDescriptor((long)FlagEnabled, nameof(FlagEnabled), ElementType.UnsignedInteger);

        /// <summary>Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)</summary>
        public const ulong FlagDefault = 0x00000088;
        public static readonly ElementDescriptor FlagDefaultDescriptor = new ElementDescriptor((long)FlagDefault, nameof(FlagDefault), ElementType.UnsignedInteger);

        /// <summary>Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player should select the one which language matches the user preference or the default + forced track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)</summary>
        public const ulong FlagForced = 0x000055AA;
        public static readonly ElementDescriptor FlagForcedDescriptor = new ElementDescriptor((long)FlagForced, nameof(FlagForced), ElementType.UnsignedInteger);

        /// <summary>Set if the track may contain blocks using lacing. (1 bit)</summary>
        public const ulong FlagLacing = 0x0000009C;
        public static readonly ElementDescriptor FlagLacingDescriptor = new ElementDescriptor((long)FlagLacing, nameof(FlagLacing), ElementType.UnsignedInteger);

        /// <summary>The minimum number of frames a player should be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.</summary>
        public const ulong MinCache = 0x00006DE7;
        public static readonly ElementDescriptor MinCacheDescriptor = new ElementDescriptor((long)MinCache, nameof(MinCache), ElementType.UnsignedInteger);

        /// <summary>The maximum cache size required to store referenced frames in and the current frame. 0 means no cache is needed.</summary>
        public const ulong MaxCache = 0x00006DF8;
        public static readonly ElementDescriptor MaxCacheDescriptor = new ElementDescriptor((long)MaxCache, nameof(MaxCache), ElementType.UnsignedInteger);

        /// <summary>Number of nanoseconds (not scaled via TimecodeScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).</summary>
        public const ulong DefaultDuration = 0x0023E383;
        public static readonly ElementDescriptor DefaultDurationDescriptor = new ElementDescriptor((long)DefaultDuration, nameof(DefaultDuration), ElementType.UnsignedInteger);

        /// <summary>The period in nanoseconds (not scaled by TimcodeScale) between two successive fields at the output of the decoding process (see the notes)</summary>
        public const ulong DefaultDecodedFieldDuration = 0x00234E7A;
        public static readonly ElementDescriptor DefaultDecodedFieldDurationDescriptor = new ElementDescriptor((long)DefaultDecodedFieldDuration, nameof(DefaultDecodedFieldDuration), ElementType.UnsignedInteger);

        /// <summary>DEPRECATED, DO NOT USE. The scale to apply on this track to work at normal speed in relation with other tracks (mostly used to adjust video speed when the audio length differs).</summary>
        public const ulong TrackTimecodeScale = 0x0023314F;
        public static readonly ElementDescriptor TrackTimecodeScaleDescriptor = new ElementDescriptor((long)TrackTimecodeScale, nameof(TrackTimecodeScale), ElementType.Float);

        /// <summary>A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.</summary>
        public const ulong TrackOffset = 0x0000537F;
        public static readonly ElementDescriptor TrackOffsetDescriptor = new ElementDescriptor((long)TrackOffset, nameof(TrackOffset), ElementType.SignedInteger);

        /// <summary>The maximum value of BlockAddID. A value 0 means there is no BlockAdditions for this track.</summary>
        public const ulong MaxBlockAdditionID = 0x000055EE;
        public static readonly ElementDescriptor MaxBlockAdditionIDDescriptor = new ElementDescriptor((long)MaxBlockAdditionID, nameof(MaxBlockAdditionID), ElementType.UnsignedInteger);

        /// <summary>A human-readable track name.</summary>
        public const ulong Name = 0x0000536E;
        public static readonly ElementDescriptor NameDescriptor = new ElementDescriptor((long)Name, nameof(Name), ElementType.Utf8String);

        /// <summary>Specifies the language of the track in the Matroska languages form.</summary>
        public const ulong Language = 0x0022B59C;
        public static readonly ElementDescriptor LanguageDescriptor = new ElementDescriptor((long)Language, nameof(Language), ElementType.AsciiString);

        /// <summary>An ID corresponding to the codec, see the codec page for more info.</summary>
        public const ulong CodecID = 0x00000086;
        public static readonly ElementDescriptor CodecIDDescriptor = new ElementDescriptor((long)CodecID, nameof(CodecID), ElementType.AsciiString);

        /// <summary>Private data only known to the codec.</summary>
        public const ulong CodecPrivate = 0x000063A2;
        public static readonly ElementDescriptor CodecPrivateDescriptor = new ElementDescriptor((long)CodecPrivate, nameof(CodecPrivate), ElementType.Binary);

        /// <summary>A human-readable string specifying the codec.</summary>
        public const ulong CodecName = 0x00258688;
        public static readonly ElementDescriptor CodecNameDescriptor = new ElementDescriptor((long)CodecName, nameof(CodecName), ElementType.Utf8String);

        /// <summary>The UID of an attachment that is used by this codec.</summary>
        public const ulong AttachmentLink = 0x00007446;
        public static readonly ElementDescriptor AttachmentLinkDescriptor = new ElementDescriptor((long)AttachmentLink, nameof(AttachmentLink), ElementType.UnsignedInteger);

        /// <summary>A string describing the encoding setting used.</summary>
        public const ulong CodecSettings = 0x003A9697;
        public static readonly ElementDescriptor CodecSettingsDescriptor = new ElementDescriptor((long)CodecSettings, nameof(CodecSettings), ElementType.Utf8String);

        /// <summary>A URL to find information about the codec used.</summary>
        public const ulong CodecInfoURL = 0x003B4040;
        public static readonly ElementDescriptor CodecInfoURLDescriptor = new ElementDescriptor((long)CodecInfoURL, nameof(CodecInfoURL), ElementType.AsciiString);

        /// <summary>A URL to download about the codec used.</summary>
        public const ulong CodecDownloadURL = 0x0026B240;
        public static readonly ElementDescriptor CodecDownloadURLDescriptor = new ElementDescriptor((long)CodecDownloadURL, nameof(CodecDownloadURL), ElementType.AsciiString);

        /// <summary>The codec can decode potentially damaged data (1 bit).</summary>
        public const ulong CodecDecodeAll = 0x000000AA;
        public static readonly ElementDescriptor CodecDecodeAllDescriptor = new ElementDescriptor((long)CodecDecodeAll, nameof(CodecDecodeAll), ElementType.UnsignedInteger);

        /// <summary>Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see SilentTracks) the overlay track should be used instead. The order of multiple TrackOverlay matters, the first one is the one that should be used. If not found it should be the second, etc.</summary>
        public const ulong TrackOverlay = 0x00006FAB;
        public static readonly ElementDescriptor TrackOverlayDescriptor = new ElementDescriptor((long)TrackOverlay, nameof(TrackOverlay), ElementType.UnsignedInteger);

        /// <summary>CodecDelay is The codec-built-in delay in nanoseconds. This value must be subtracted from each block timestamp in order to get the actual timestamp. The value should be small so the muxing of tracks with the same actual timestamp are in the same Cluster.</summary>
        public const ulong CodecDelay = 0x000056AA;
        public static readonly ElementDescriptor CodecDelayDescriptor = new ElementDescriptor((long)CodecDelay, nameof(CodecDelay), ElementType.UnsignedInteger);

        /// <summary>After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder must decode before the decoded data is valid.</summary>
        public const ulong SeekPreRoll = 0x000056BB;
        public static readonly ElementDescriptor SeekPreRollDescriptor = new ElementDescriptor((long)SeekPreRoll, nameof(SeekPreRoll), ElementType.UnsignedInteger);

        /// <summary>The track identification for the given Chapter Codec.</summary>
        public const ulong TrackTranslate = 0x00006624;
        public static readonly ElementDescriptor TrackTranslateDescriptor = new ElementDescriptor((long)TrackTranslate, nameof(TrackTranslate), ElementType.MasterElement);

        /// <summary>Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.</summary>
        public const ulong TrackTranslateEditionUID = 0x000066FC;
        public static readonly ElementDescriptor TrackTranslateEditionUIDDescriptor = new ElementDescriptor((long)TrackTranslateEditionUID, nameof(TrackTranslateEditionUID), ElementType.UnsignedInteger);

        /// <summary>The chapter codec using this ID (0: Matroska Script, 1: DVD-menu).</summary>
        public const ulong TrackTranslateCodec = 0x000066BF;
        public static readonly ElementDescriptor TrackTranslateCodecDescriptor = new ElementDescriptor((long)TrackTranslateCodec, nameof(TrackTranslateCodec), ElementType.UnsignedInteger);

        /// <summary>The binary value used to represent this track in the chapter codec data. The format depends on the ChapProcessCodecID used.</summary>
        public const ulong TrackTranslateTrackID = 0x000066A5;
        public static readonly ElementDescriptor TrackTranslateTrackIDDescriptor = new ElementDescriptor((long)TrackTranslateTrackID, nameof(TrackTranslateTrackID), ElementType.Binary);

        /// <summary>Video settings.</summary>
        public const ulong Video = 0x000000E0;
        public static readonly ElementDescriptor VideoDescriptor = new ElementDescriptor((long)Video, nameof(Video), ElementType.MasterElement);

        /// <summary>A flag to declare is the video is known to be progressive or interlaced and if applicable to declare details about the interlacement. (0: undetermined, 1: interlaced, 2: progressive)</summary>
        public const ulong FlagInterlaced = 0x0000009A;
        public static readonly ElementDescriptor FlagInterlacedDescriptor = new ElementDescriptor((long)FlagInterlaced, nameof(FlagInterlaced), ElementType.UnsignedInteger);

        /// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored. (0: Progressive, 1: Interlaced with top field display first and top field stored first, 2: Undetermined field order, 6: Interlaced with bottom field displayed first and bottom field stored first, 9: Interlaced with bottom field displayed first and top field stored first, 14: Interlaced with top field displayed first and bottom field stored first)</summary>
        public const ulong FieldOrder = 0x0000009D;
        public static readonly ElementDescriptor FieldOrderDescriptor = new ElementDescriptor((long)FieldOrder, nameof(FieldOrder), ElementType.UnsignedInteger);

        /// <summary>Stereo-3D video mode (0: mono, 1: side by side (left eye is first), 2: top-bottom (right eye is first), 3: top-bottom (left eye is first), 4: checkboard (right is first), 5: checkboard (left is first), 6: row interleaved (right is first), 7: row interleaved (left is first), 8: column interleaved (right is first), 9: column interleaved (left is first), 10: anaglyph (cyan/red), 11: side by side (right eye is first), 12: anaglyph (green/magenta), 13 both eyes laced in one Block (left eye is first), 14 both eyes laced in one Block (right eye is first)) . There are some more details on 3D support in the Specification Notes.</summary>
        public const ulong StereoMode = 0x000053B8;
        public static readonly ElementDescriptor StereoModeDescriptor = new ElementDescriptor((long)StereoMode, nameof(StereoMode), ElementType.UnsignedInteger);

        /// <summary>Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.</summary>
        public const ulong AlphaMode = 0x000053C0;
        public static readonly ElementDescriptor AlphaModeDescriptor = new ElementDescriptor((long)AlphaMode, nameof(AlphaMode), ElementType.UnsignedInteger);

        /// <summary>DEPRECATED, DO NOT USE. Bogus StereoMode value used in old versions of libmatroska. (0: mono, 1: right eye, 2: left eye, 3: both eyes).</summary>
        public const ulong OldStereoMode = 0x000053B9;
        public static readonly ElementDescriptor OldStereoModeDescriptor = new ElementDescriptor((long)OldStereoMode, nameof(OldStereoMode), ElementType.UnsignedInteger);

        /// <summary>Width of the encoded video frames in pixels.</summary>
        public const ulong PixelWidth = 0x000000B0;
        public static readonly ElementDescriptor PixelWidthDescriptor = new ElementDescriptor((long)PixelWidth, nameof(PixelWidth), ElementType.UnsignedInteger);

        /// <summary>Height of the encoded video frames in pixels.</summary>
        public const ulong PixelHeight = 0x000000BA;
        public static readonly ElementDescriptor PixelHeightDescriptor = new ElementDescriptor((long)PixelHeight, nameof(PixelHeight), ElementType.UnsignedInteger);

        /// <summary>The number of video pixels to remove at the bottom of the image (for HDTV content).</summary>
        public const ulong PixelCropBottom = 0x000054AA;
        public static readonly ElementDescriptor PixelCropBottomDescriptor = new ElementDescriptor((long)PixelCropBottom, nameof(PixelCropBottom), ElementType.UnsignedInteger);

        /// <summary>The number of video pixels to remove at the top of the image.</summary>
        public const ulong PixelCropTop = 0x000054BB;
        public static readonly ElementDescriptor PixelCropTopDescriptor = new ElementDescriptor((long)PixelCropTop, nameof(PixelCropTop), ElementType.UnsignedInteger);

        /// <summary>The number of video pixels to remove on the left of the image.</summary>
        public const ulong PixelCropLeft = 0x000054CC;
        public static readonly ElementDescriptor PixelCropLeftDescriptor = new ElementDescriptor((long)PixelCropLeft, nameof(PixelCropLeft), ElementType.UnsignedInteger);

        /// <summary>The number of video pixels to remove on the right of the image.</summary>
        public const ulong PixelCropRight = 0x000054DD;
        public static readonly ElementDescriptor PixelCropRightDescriptor = new ElementDescriptor((long)PixelCropRight, nameof(PixelCropRight), ElementType.UnsignedInteger);

        /// <summary>Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
        public const ulong DisplayWidth = 0x000054B0;
        public static readonly ElementDescriptor DisplayWidthDescriptor = new ElementDescriptor((long)DisplayWidth, nameof(DisplayWidth), ElementType.UnsignedInteger);

        /// <summary>Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements). The default value is only valid when DisplayUnit is 0.</summary>
        public const ulong DisplayHeight = 0x000054BA;
        public static readonly ElementDescriptor DisplayHeightDescriptor = new ElementDescriptor((long)DisplayHeight, nameof(DisplayHeight), ElementType.UnsignedInteger);

        /// <summary>How DisplayWidth & DisplayHeight should be interpreted (0: pixels, 1: centimeters, 2: inches, 3: Display Aspect Ratio, 4: Unknown).</summary>
        public const ulong DisplayUnit = 0x000054B2;
        public static readonly ElementDescriptor DisplayUnitDescriptor = new ElementDescriptor((long)DisplayUnit, nameof(DisplayUnit), ElementType.UnsignedInteger);

        /// <summary>Specify the possible modifications to the aspect ratio (0: free resizing, 1: keep aspect ratio, 2: fixed).</summary>
        public const ulong AspectRatioType = 0x000054B3;
        public static readonly ElementDescriptor AspectRatioTypeDescriptor = new ElementDescriptor((long)AspectRatioType, nameof(AspectRatioType), ElementType.UnsignedInteger);

        /// <summary>Same value as in AVI (32 bits).</summary>
        public const ulong ColourSpace = 0x002EB524;
        public static readonly ElementDescriptor ColourSpaceDescriptor = new ElementDescriptor((long)ColourSpace, nameof(ColourSpace), ElementType.Binary);

        /// <summary>Gamma Value.</summary>
        public const ulong GammaValue = 0x002FB523;
        public static readonly ElementDescriptor GammaValueDescriptor = new ElementDescriptor((long)GammaValue, nameof(GammaValue), ElementType.Float);

        /// <summary>Number of frames per second. Informational only.</summary>
        public const ulong FrameRate = 0x002383E3;
        public static readonly ElementDescriptor FrameRateDescriptor = new ElementDescriptor((long)FrameRate, nameof(FrameRate), ElementType.Float);

        /// <summary> Settings describing the colour format.</summary>
        public const ulong Colour = 0x000055B0;
        public static readonly ElementDescriptor ColourDescriptor = new ElementDescriptor((long)Colour, nameof(Colour), ElementType.MasterElement);

        /// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from reg, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC 23001-8:2013/DCOR1. (0:GBR, 1: BT709, 2: Unspecified, 3: Reserved, 4: FCC, 5: BT470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: YCOCG, 9: BT2020 Non-constant Luminance, 10: BT2020 Constant Luminance)</summary>
        public const ulong MatrixCoefficients = 0x000055B1;
        public static readonly ElementDescriptor MatrixCoefficientsDescriptor = new ElementDescriptor((long)MatrixCoefficients, nameof(MatrixCoefficients), ElementType.UnsignedInteger);

        /// <summary>Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.</summary>
        public const ulong BitsPerChannel = 0x000055B2;
        public static readonly ElementDescriptor BitsPerChannelDescriptor = new ElementDescriptor((long)BitsPerChannel, nameof(BitsPerChannel), ElementType.UnsignedInteger);

        /// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz should be set to 1.</summary>
        public const ulong ChromaSubsamplingHorz = 0x000055B3;
        public static readonly ElementDescriptor ChromaSubsamplingHorzDescriptor = new ElementDescriptor((long)ChromaSubsamplingHorz, nameof(ChromaSubsamplingHorz), ElementType.UnsignedInteger);

        /// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert should be set to 1.</summary>
        public const ulong ChromaSubsamplingVert = 0x000055B4;
        public static readonly ElementDescriptor ChromaSubsamplingVertDescriptor = new ElementDescriptor((long)ChromaSubsamplingVert, nameof(ChromaSubsamplingVert), ElementType.UnsignedInteger);

        /// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz should be set to 1 and CbSubsamplingHorz should be set to 1.</summary>
        public const ulong CbSubsamplingHorz = 0x000055B5;
        public static readonly ElementDescriptor CbSubsamplingHorzDescriptor = new ElementDescriptor((long)CbSubsamplingHorz, nameof(CbSubsamplingHorz), ElementType.UnsignedInteger);

        /// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.</summary>
        public const ulong CbSubsamplingVert = 0x000055B6;
        public static readonly ElementDescriptor CbSubsamplingVertDescriptor = new ElementDescriptor((long)CbSubsamplingVert, nameof(CbSubsamplingVert), ElementType.UnsignedInteger);

        /// <summary>How chroma is subsampled horizontally. (0: Unspecified, 1: Left Collocated, 2: Half)</summary>
        public const ulong ChromaSitingHorz = 0x000055B7;
        public static readonly ElementDescriptor ChromaSitingHorzDescriptor = new ElementDescriptor((long)ChromaSitingHorz, nameof(ChromaSitingHorz), ElementType.UnsignedInteger);

        /// <summary>How chroma is subsampled vertically. (0: Unspecified, 1: Top Collocated, 2: Half)</summary>
        public const ulong ChromaSitingVert = 0x000055B8;
        public static readonly ElementDescriptor ChromaSitingVertDescriptor = new ElementDescriptor((long)ChromaSitingVert, nameof(ChromaSitingVert), ElementType.UnsignedInteger);

        /// <summary>Clipping of the color ranges. (0: Unspecified, 1: Broadcast Range, 2: Full range (no clipping), 3: Defined by MatrixCoefficients/TransferCharacteristics)</summary>
        public const ulong Range = 0x000055B9;
        public static readonly ElementDescriptor RangeDescriptor = new ElementDescriptor((long)Range, nameof(Range), ElementType.UnsignedInteger);

        /// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics 1-15 are adopted from Table 3 of ISO/IEC 23001-8:2013/DCOR1. TransferCharacteristics 16-18 are proposed values. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: Gamma 2.2 curve, 5: Gamma 2.8 curve, 6: SMPTE 170M, 7: SMPTE 240M, 8: Linear, 9: Log, 10: Log Sqrt, 11: IEC 61966-2-4, 12: ITU-R BT.1361 Extended Colour Gamut, 13: IEC 61966-2-1, 14: ITU-R BT.2020 10 bit, 15: ITU-R BT.2020 12 bit, 16: SMPTE ST 2084, 17: SMPTE ST 428-1 18: ARIB STD-B67 (HLG))</summary>
        public const ulong TransferCharacteristics = 0x000055BA;
        public static readonly ElementDescriptor TransferCharacteristicsDescriptor = new ElementDescriptor((long)TransferCharacteristics, nameof(TransferCharacteristics), ElementType.UnsignedInteger);

        /// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23001-8:2013/DCOR1. (0: Reserved, 1: ITU-R BT.709, 2: Unspecified, 3: Reserved, 4: ITU-R BT.470M, 5: ITU-R BT.470BG, 6: SMPTE 170M, 7: SMPTE 240M, 8: FILM, 9: ITU-R BT.2020, 10: SMPTE ST 428-1, 22: JEDEC P22 phosphors)</summary>
        public const ulong Primaries = 0x000055BB;
        public static readonly ElementDescriptor PrimariesDescriptor = new ElementDescriptor((long)Primaries, nameof(Primaries), ElementType.UnsignedInteger);

        /// <summary>Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/mý).</summary>
        public const ulong MaxCLL = 0x000055BC;
        public static readonly ElementDescriptor MaxCLLDescriptor = new ElementDescriptor((long)MaxCLL, nameof(MaxCLL), ElementType.UnsignedInteger);

        /// <summary>Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/mý).</summary>
        public const ulong MaxFALL = 0x000055BD;
        public static readonly ElementDescriptor MaxFALLDescriptor = new ElementDescriptor((long)MaxFALL, nameof(MaxFALL), ElementType.UnsignedInteger);

        /// <summary>SMPTE 2086 mastering data.</summary>
        public const ulong MasteringMetadata = 0x000055D0;
        public static readonly ElementDescriptor MasteringMetadataDescriptor = new ElementDescriptor((long)MasteringMetadata, nameof(MasteringMetadata), ElementType.MasterElement);

        /// <summary>Red X chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryRChromaticityX = 0x000055D1;
        public static readonly ElementDescriptor PrimaryRChromaticityXDescriptor = new ElementDescriptor((long)PrimaryRChromaticityX, nameof(PrimaryRChromaticityX), ElementType.Float);

        /// <summary>Red Y chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryRChromaticityY = 0x000055D2;
        public static readonly ElementDescriptor PrimaryRChromaticityYDescriptor = new ElementDescriptor((long)PrimaryRChromaticityY, nameof(PrimaryRChromaticityY), ElementType.Float);

        /// <summary>Green X chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryGChromaticityX = 0x000055D3;
        public static readonly ElementDescriptor PrimaryGChromaticityXDescriptor = new ElementDescriptor((long)PrimaryGChromaticityX, nameof(PrimaryGChromaticityX), ElementType.Float);

        /// <summary>Green Y chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryGChromaticityY = 0x000055D4;
        public static readonly ElementDescriptor PrimaryGChromaticityYDescriptor = new ElementDescriptor((long)PrimaryGChromaticityY, nameof(PrimaryGChromaticityY), ElementType.Float);

        /// <summary>Blue X chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryBChromaticityX = 0x000055D5;
        public static readonly ElementDescriptor PrimaryBChromaticityXDescriptor = new ElementDescriptor((long)PrimaryBChromaticityX, nameof(PrimaryBChromaticityX), ElementType.Float);

        /// <summary>Blue Y chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong PrimaryBChromaticityY = 0x000055D6;
        public static readonly ElementDescriptor PrimaryBChromaticityYDescriptor = new ElementDescriptor((long)PrimaryBChromaticityY, nameof(PrimaryBChromaticityY), ElementType.Float);

        /// <summary>White X chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong WhitePointChromaticityX = 0x000055D7;
        public static readonly ElementDescriptor WhitePointChromaticityXDescriptor = new ElementDescriptor((long)WhitePointChromaticityX, nameof(WhitePointChromaticityX), ElementType.Float);

        /// <summary>White Y chromaticity coordinate as defined by CIE 1931.</summary>
        public const ulong WhitePointChromaticityY = 0x000055D8;
        public static readonly ElementDescriptor WhitePointChromaticityYDescriptor = new ElementDescriptor((long)WhitePointChromaticityY, nameof(WhitePointChromaticityY), ElementType.Float);

        /// <summary>Maximum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
        public const ulong LuminanceMax = 0x000055D9;
        public static readonly ElementDescriptor LuminanceMaxDescriptor = new ElementDescriptor((long)LuminanceMax, nameof(LuminanceMax), ElementType.Float);

        /// <summary>Mininum luminance. Shall be represented in candelas per square meter (cd/mý).</summary>
        public const ulong LuminanceMin = 0x000055DA;
        public static readonly ElementDescriptor LuminanceMinDescriptor = new ElementDescriptor((long)LuminanceMin, nameof(LuminanceMin), ElementType.Float);

        /// <summary>Audio settings.</summary>
        public const ulong Audio = 0x000000E1;
        public static readonly ElementDescriptor AudioDescriptor = new ElementDescriptor((long)Audio, nameof(Audio), ElementType.MasterElement);

        /// <summary>Sampling frequency in Hz.</summary>
        public const ulong SamplingFrequency = 0x000000B5;
        public static readonly ElementDescriptor SamplingFrequencyDescriptor = new ElementDescriptor((long)SamplingFrequency, nameof(SamplingFrequency), ElementType.Float);

        /// <summary>Real output sampling frequency in Hz (used for SBR techniques).</summary>
        public const ulong OutputSamplingFrequency = 0x000078B5;
        public static readonly ElementDescriptor OutputSamplingFrequencyDescriptor = new ElementDescriptor((long)OutputSamplingFrequency, nameof(OutputSamplingFrequency), ElementType.Float);

        /// <summary>Numbers of channels in the track.</summary>
        public const ulong Channels = 0x0000009F;
        public static readonly ElementDescriptor ChannelsDescriptor = new ElementDescriptor((long)Channels, nameof(Channels), ElementType.UnsignedInteger);

        /// <summary>Table of horizontal angles for each successive channel, see appendix.</summary>
        public const ulong ChannelPositions = 0x00007D7B;
        public static readonly ElementDescriptor ChannelPositionsDescriptor = new ElementDescriptor((long)ChannelPositions, nameof(ChannelPositions), ElementType.Binary);

        /// <summary>Bits per sample, mostly used for PCM.</summary>
        public const ulong BitDepth = 0x00006264;
        public static readonly ElementDescriptor BitDepthDescriptor = new ElementDescriptor((long)BitDepth, nameof(BitDepth), ElementType.UnsignedInteger);

        /// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details look at the Specification Notes on the subject.</summary>
        public const ulong TrackOperation = 0x000000E2;
        public static readonly ElementDescriptor TrackOperationDescriptor = new ElementDescriptor((long)TrackOperation, nameof(TrackOperation), ElementType.MasterElement);

        /// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
        public const ulong TrackCombinePlanes = 0x000000E3;
        public static readonly ElementDescriptor TrackCombinePlanesDescriptor = new ElementDescriptor((long)TrackCombinePlanes, nameof(TrackCombinePlanes), ElementType.MasterElement);

        /// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
        public const ulong TrackPlane = 0x000000E4;
        public static readonly ElementDescriptor TrackPlaneDescriptor = new ElementDescriptor((long)TrackPlane, nameof(TrackPlane), ElementType.MasterElement);

        /// <summary>The trackUID number of the track representing the plane.</summary>
        public const ulong TrackPlaneUID = 0x000000E5;
        public static readonly ElementDescriptor TrackPlaneUIDDescriptor = new ElementDescriptor((long)TrackPlaneUID, nameof(TrackPlaneUID), ElementType.UnsignedInteger);

        /// <summary>The kind of plane this track corresponds to (0: left eye, 1: right eye, 2: background).</summary>
        public const ulong TrackPlaneType = 0x000000E6;
        public static readonly ElementDescriptor TrackPlaneTypeDescriptor = new ElementDescriptor((long)TrackPlaneType, nameof(TrackPlaneType), ElementType.UnsignedInteger);

        /// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
        public const ulong TrackJoinBlocks = 0x000000E9;
        public static readonly ElementDescriptor TrackJoinBlocksDescriptor = new ElementDescriptor((long)TrackJoinBlocks, nameof(TrackJoinBlocks), ElementType.MasterElement);

        /// <summary>The trackUID number of a track whose blocks are used to create this virtual track.</summary>
        public const ulong TrackJoinUID = 0x000000ED;
        public static readonly ElementDescriptor TrackJoinUIDDescriptor = new ElementDescriptor((long)TrackJoinUID, nameof(TrackJoinUID), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong TrickTrackUID = 0x000000C0;
        public static readonly ElementDescriptor TrickTrackUIDDescriptor = new ElementDescriptor((long)TrickTrackUID, nameof(TrickTrackUID), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong TrickTrackSegmentUID = 0x000000C1;
        public static readonly ElementDescriptor TrickTrackSegmentUIDDescriptor = new ElementDescriptor((long)TrickTrackSegmentUID, nameof(TrickTrackSegmentUID), ElementType.Binary);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong TrickTrackFlag = 0x000000C6;
        public static readonly ElementDescriptor TrickTrackFlagDescriptor = new ElementDescriptor((long)TrickTrackFlag, nameof(TrickTrackFlag), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong TrickMasterTrackUID = 0x000000C7;
        public static readonly ElementDescriptor TrickMasterTrackUIDDescriptor = new ElementDescriptor((long)TrickMasterTrackUID, nameof(TrickMasterTrackUID), ElementType.UnsignedInteger);

        /// <summary>DivX trick track extenstions</summary>
        public const ulong TrickMasterTrackSegmentUID = 0x000000C4;
        public static readonly ElementDescriptor TrickMasterTrackSegmentUIDDescriptor = new ElementDescriptor((long)TrickMasterTrackSegmentUID, nameof(TrickMasterTrackSegmentUID), ElementType.Binary);

        /// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
        public const ulong ContentEncodings = 0x00006D80;
        public static readonly ElementDescriptor ContentEncodingsDescriptor = new ElementDescriptor((long)ContentEncodings, nameof(ContentEncodings), ElementType.MasterElement);

        /// <summary>Settings for one content encoding like compression or encryption.</summary>
        public const ulong ContentEncoding = 0x00006240;
        public static readonly ElementDescriptor ContentEncodingDescriptor = new ElementDescriptor((long)ContentEncoding, nameof(ContentEncoding), ElementType.MasterElement);

        /// <summary>Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be unique over all ContentEncodingOrder Elements in the Segment.</summary>
        public const ulong ContentEncodingOrder = 0x00005031;
        public static readonly ElementDescriptor ContentEncodingOrderDescriptor = new ElementDescriptor((long)ContentEncodingOrder, nameof(ContentEncodingOrder), ElementType.UnsignedInteger);

        /// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed. Possible values: 1 - all frame contents, 2 - the track's private data, 4 - the next ContentEncoding (next ContentEncodingOrder. Either the data inside ContentCompression and/or ContentEncryption)</summary>
        public const ulong ContentEncodingScope = 0x00005032;
        public static readonly ElementDescriptor ContentEncodingScopeDescriptor = new ElementDescriptor((long)ContentEncodingScope, nameof(ContentEncodingScope), ElementType.UnsignedInteger);

        /// <summary>A value describing what kind of transformation has been done. Possible values: 0 - compression, 1 - encryption</summary>
        public const ulong ContentEncodingType = 0x00005033;
        public static readonly ElementDescriptor ContentEncodingTypeDescriptor = new ElementDescriptor((long)ContentEncodingType, nameof(ContentEncodingType), ElementType.UnsignedInteger);

        /// <summary>Settings describing the compression used. Must be present if the value of ContentEncodingType is 0 and absent otherwise. Each block must be decompressable even if no previous block is available in order not to prevent seeking.</summary>
        public const ulong ContentCompression = 0x00005034;
        public static readonly ElementDescriptor ContentCompressionDescriptor = new ElementDescriptor((long)ContentCompression, nameof(ContentCompression), ElementType.MasterElement);

        /// <summary>The compression algorithm used. Algorithms that have been specified so far are: 0 - zlib,1 - bzlib,2 - lzo1x 3 - Header Stripping</summary>
        public const ulong ContentCompAlgo = 0x00004254;
        public static readonly ElementDescriptor ContentCompAlgoDescriptor = new ElementDescriptor((long)ContentCompAlgo, nameof(ContentCompAlgo), ElementType.UnsignedInteger);

        /// <summary>Settings that might be needed by the decompressor. For Header Stripping (ContentCompAlgo=3), the bytes that were removed from the beggining of each frames of the track.</summary>
        public const ulong ContentCompSettings = 0x00004255;
        public static readonly ElementDescriptor ContentCompSettingsDescriptor = new ElementDescriptor((long)ContentCompSettings, nameof(ContentCompSettings), ElementType.Binary);

        /// <summary>Settings describing the encryption used. Must be present if the value of ContentEncodingType is 1 and absent otherwise.</summary>
        public const ulong ContentEncryption = 0x00005035;
        public static readonly ElementDescriptor ContentEncryptionDescriptor = new ElementDescriptor((long)ContentEncryption, nameof(ContentEncryption), ElementType.MasterElement);

        /// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed. Predefined values: 1 - DES, 2 - 3DES, 3 - Twofish, 4 - Blowfish, 5 - AES</summary>
        public const ulong ContentEncAlgo = 0x000047E1;
        public static readonly ElementDescriptor ContentEncAlgoDescriptor = new ElementDescriptor((long)ContentEncAlgo, nameof(ContentEncAlgo), ElementType.UnsignedInteger);

        /// <summary>For public key algorithms this is the ID of the public key the the data was encrypted with.</summary>
        public const ulong ContentEncKeyID = 0x000047E2;
        public static readonly ElementDescriptor ContentEncKeyIDDescriptor = new ElementDescriptor((long)ContentEncKeyID, nameof(ContentEncKeyID), ElementType.Binary);

        /// <summary>A cryptographic signature of the contents.</summary>
        public const ulong ContentSignature = 0x000047E3;
        public static readonly ElementDescriptor ContentSignatureDescriptor = new ElementDescriptor((long)ContentSignature, nameof(ContentSignature), ElementType.Binary);

        /// <summary>This is the ID of the private key the data was signed with.</summary>
        public const ulong ContentSigKeyID = 0x000047E4;
        public static readonly ElementDescriptor ContentSigKeyIDDescriptor = new ElementDescriptor((long)ContentSigKeyID, nameof(ContentSigKeyID), ElementType.Binary);

        /// <summary>The algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - RSA</summary>
        public const ulong ContentSigAlgo = 0x000047E5;
        public static readonly ElementDescriptor ContentSigAlgoDescriptor = new ElementDescriptor((long)ContentSigAlgo, nameof(ContentSigAlgo), ElementType.UnsignedInteger);

        /// <summary>The hash algorithm used for the signature. A value of '0' means that the contents have not been signed but only encrypted. Predefined values: 1 - SHA1-160 2 - MD5</summary>
        public const ulong ContentSigHashAlgo = 0x000047E6;
        public static readonly ElementDescriptor ContentSigHashAlgoDescriptor = new ElementDescriptor((long)ContentSigHashAlgo, nameof(ContentSigHashAlgo), ElementType.UnsignedInteger);

        /// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment. Should be mandatory for non "live" streams.</summary>
        public const ulong Cues = 0x1C53BB6B;
        public static readonly ElementDescriptor CuesDescriptor = new ElementDescriptor((long)Cues, nameof(Cues), ElementType.MasterElement);

        /// <summary>Contains all information relative to a seek point in the Segment.</summary>
        public const ulong CuePoint = 0x000000BB;
        public static readonly ElementDescriptor CuePointDescriptor = new ElementDescriptor((long)CuePoint, nameof(CuePoint), ElementType.MasterElement);

        /// <summary>Absolute timestamp according to the Segment time base.</summary>
        public const ulong CueTime = 0x000000B3;
        public static readonly ElementDescriptor CueTimeDescriptor = new ElementDescriptor((long)CueTime, nameof(CueTime), ElementType.UnsignedInteger);

        /// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
        public const ulong CueTrackPositions = 0x000000B7;
        public static readonly ElementDescriptor CueTrackPositionsDescriptor = new ElementDescriptor((long)CueTrackPositions, nameof(CueTrackPositions), ElementType.MasterElement);

        /// <summary>The track for which a position is given.</summary>
        public const ulong CueTrack = 0x000000F7;
        public static readonly ElementDescriptor CueTrackDescriptor = new ElementDescriptor((long)CueTrack, nameof(CueTrack), ElementType.UnsignedInteger);

        /// <summary>The position of the Cluster containing the required Block.</summary>
        public const ulong CueClusterPosition = 0x000000F1;
        public static readonly ElementDescriptor CueClusterPositionDescriptor = new ElementDescriptor((long)CueClusterPosition, nameof(CueClusterPosition), ElementType.UnsignedInteger);

        /// <summary>The relative position of the referenced block inside the cluster with 0 being the first possible position for an Element inside that cluster.</summary>
        public const ulong CueRelativePosition = 0x000000F0;
        public static readonly ElementDescriptor CueRelativePositionDescriptor = new ElementDescriptor((long)CueRelativePosition, nameof(CueRelativePosition), ElementType.UnsignedInteger);

        /// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
        public const ulong CueDuration = 0x000000B2;
        public static readonly ElementDescriptor CueDurationDescriptor = new ElementDescriptor((long)CueDuration, nameof(CueDuration), ElementType.UnsignedInteger);

        /// <summary>Number of the Block in the specified Cluster.</summary>
        public const ulong CueBlockNumber = 0x00005378;
        public static readonly ElementDescriptor CueBlockNumberDescriptor = new ElementDescriptor((long)CueBlockNumber, nameof(CueBlockNumber), ElementType.UnsignedInteger);

        /// <summary>The position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
        public const ulong CueCodecState = 0x000000EA;
        public static readonly ElementDescriptor CueCodecStateDescriptor = new ElementDescriptor((long)CueCodecState, nameof(CueCodecState), ElementType.UnsignedInteger);

        /// <summary>The Clusters containing the required referenced Blocks.</summary>
        public const ulong CueReference = 0x000000DB;
        public static readonly ElementDescriptor CueReferenceDescriptor = new ElementDescriptor((long)CueReference, nameof(CueReference), ElementType.MasterElement);

        /// <summary>Timestamp of the referenced Block.</summary>
        public const ulong CueRefTime = 0x00000096;
        public static readonly ElementDescriptor CueRefTimeDescriptor = new ElementDescriptor((long)CueRefTime, nameof(CueRefTime), ElementType.UnsignedInteger);

        /// <summary>The Position of the Cluster containing the referenced Block.</summary>
        public const ulong CueRefCluster = 0x00000097;
        public static readonly ElementDescriptor CueRefClusterDescriptor = new ElementDescriptor((long)CueRefCluster, nameof(CueRefCluster), ElementType.UnsignedInteger);

        /// <summary>Number of the referenced Block of Track X in the specified Cluster.</summary>
        public const ulong CueRefNumber = 0x0000535F;
        public static readonly ElementDescriptor CueRefNumberDescriptor = new ElementDescriptor((long)CueRefNumber, nameof(CueRefNumber), ElementType.UnsignedInteger);

        /// <summary>The position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.</summary>
        public const ulong CueRefCodecState = 0x000000EB;
        public static readonly ElementDescriptor CueRefCodecStateDescriptor = new ElementDescriptor((long)CueRefCodecState, nameof(CueRefCodecState), ElementType.UnsignedInteger);

        /// <summary>Contain attached files.</summary>
        public const ulong Attachments = 0x1941A469;
        public static readonly ElementDescriptor AttachmentsDescriptor = new ElementDescriptor((long)Attachments, nameof(Attachments), ElementType.MasterElement);

        /// <summary>An attached file.</summary>
        public const ulong AttachedFile = 0x000061A7;
        public static readonly ElementDescriptor AttachedFileDescriptor = new ElementDescriptor((long)AttachedFile, nameof(AttachedFile), ElementType.MasterElement);

        /// <summary>A human-friendly name for the attached file.</summary>
        public const ulong FileDescription = 0x0000467E;
        public static readonly ElementDescriptor FileDescriptionDescriptor = new ElementDescriptor((long)FileDescription, nameof(FileDescription), ElementType.Utf8String);

        /// <summary>Filename of the attached file.</summary>
        public const ulong FileName = 0x0000466E;
        public static readonly ElementDescriptor FileNameDescriptor = new ElementDescriptor((long)FileName, nameof(FileName), ElementType.Utf8String);

        /// <summary>MIME type of the file.</summary>
        public const ulong FileMimeType = 0x00004660;
        public static readonly ElementDescriptor FileMimeTypeDescriptor = new ElementDescriptor((long)FileMimeType, nameof(FileMimeType), ElementType.AsciiString);

        /// <summary>The data of the file.</summary>
        public const ulong FileData = 0x0000465C;
        public static readonly ElementDescriptor FileDataDescriptor = new ElementDescriptor((long)FileData, nameof(FileData), ElementType.Binary);

        /// <summary>Unique ID representing the file, as random as possible.</summary>
        public const ulong FileUID = 0x000046AE;
        public static readonly ElementDescriptor FileUIDDescriptor = new ElementDescriptor((long)FileUID, nameof(FileUID), ElementType.UnsignedInteger);

        /// <summary>A binary value that a track/codec can refer to when the attachment is needed.</summary>
        public const ulong FileReferral = 0x00004675;
        public static readonly ElementDescriptor FileReferralDescriptor = new ElementDescriptor((long)FileReferral, nameof(FileReferral), ElementType.Binary);

        /// <summary>DivX font extension</summary>
        public const ulong FileUsedStartTime = 0x00004661;
        public static readonly ElementDescriptor FileUsedStartTimeDescriptor = new ElementDescriptor((long)FileUsedStartTime, nameof(FileUsedStartTime), ElementType.UnsignedInteger);

        /// <summary>DivX font extension</summary>
        public const ulong FileUsedEndTime = 0x00004662;
        public static readonly ElementDescriptor FileUsedEndTimeDescriptor = new ElementDescriptor((long)FileUsedEndTime, nameof(FileUsedEndTime), ElementType.UnsignedInteger);

        /// <summary>A system to define basic menus and partition data. For more detailed information, look at the Chapters Explanation.</summary>
        public const ulong Chapters = 0x1043A770;
        public static readonly ElementDescriptor ChaptersDescriptor = new ElementDescriptor((long)Chapters, nameof(Chapters), ElementType.MasterElement);

        /// <summary>Contains all information about a Segment edition.</summary>
        public const ulong EditionEntry = 0x000045B9;
        public static readonly ElementDescriptor EditionEntryDescriptor = new ElementDescriptor((long)EditionEntry, nameof(EditionEntry), ElementType.MasterElement);

        /// <summary>A unique ID to identify the edition. It's useful for tagging an edition.</summary>
        public const ulong EditionUID = 0x000045BC;
        public static readonly ElementDescriptor EditionUIDDescriptor = new ElementDescriptor((long)EditionUID, nameof(EditionUID), ElementType.UnsignedInteger);

        /// <summary>If an edition is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
        public const ulong EditionFlagHidden = 0x000045BD;
        public static readonly ElementDescriptor EditionFlagHiddenDescriptor = new ElementDescriptor((long)EditionFlagHidden, nameof(EditionFlagHidden), ElementType.UnsignedInteger);

        /// <summary>If a flag is set (1) the edition should be used as the default one. (1 bit)</summary>
        public const ulong EditionFlagDefault = 0x000045DB;
        public static readonly ElementDescriptor EditionFlagDefaultDescriptor = new ElementDescriptor((long)EditionFlagDefault, nameof(EditionFlagDefault), ElementType.UnsignedInteger);

        /// <summary>Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)</summary>
        public const ulong EditionFlagOrdered = 0x000045DD;
        public static readonly ElementDescriptor EditionFlagOrderedDescriptor = new ElementDescriptor((long)EditionFlagOrdered, nameof(EditionFlagOrdered), ElementType.UnsignedInteger);

        /// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
        public const ulong ChapterAtom = 0x000000B6;
        public static readonly ElementDescriptor ChapterAtomDescriptor = new ElementDescriptor((long)ChapterAtom, nameof(ChapterAtom), ElementType.MasterElement);

        /// <summary>A unique ID to identify the Chapter.</summary>
        public const ulong ChapterUID = 0x000073C4;
        public static readonly ElementDescriptor ChapterUIDDescriptor = new ElementDescriptor((long)ChapterUID, nameof(ChapterUID), ElementType.UnsignedInteger);

        /// <summary>A unique string ID to identify the Chapter. Use for WebVTT cue identifier storage.</summary>
        public const ulong ChapterStringUID = 0x00005654;
        public static readonly ElementDescriptor ChapterStringUIDDescriptor = new ElementDescriptor((long)ChapterStringUID, nameof(ChapterStringUID), ElementType.Utf8String);

        /// <summary>Timestamp of the start of Chapter (not scaled).</summary>
        public const ulong ChapterTimeStart = 0x00000091;
        public static readonly ElementDescriptor ChapterTimeStartDescriptor = new ElementDescriptor((long)ChapterTimeStart, nameof(ChapterTimeStart), ElementType.UnsignedInteger);

        /// <summary>Timestamp of the end of Chapter (timestamp excluded, not scaled).</summary>
        public const ulong ChapterTimeEnd = 0x00000092;
        public static readonly ElementDescriptor ChapterTimeEndDescriptor = new ElementDescriptor((long)ChapterTimeEnd, nameof(ChapterTimeEnd), ElementType.UnsignedInteger);

        /// <summary>If a chapter is hidden (1), it should not be available to the user interface (but still to Control Tracks; see flag notes). (1 bit)</summary>
        public const ulong ChapterFlagHidden = 0x00000098;
        public static readonly ElementDescriptor ChapterFlagHiddenDescriptor = new ElementDescriptor((long)ChapterFlagHidden, nameof(ChapterFlagHidden), ElementType.UnsignedInteger);

        /// <summary>Specify wether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie should skip all the content between the TimeStart and TimeEnd of this chapter (see flag notes). (1 bit)</summary>
        public const ulong ChapterFlagEnabled = 0x00004598;
        public static readonly ElementDescriptor ChapterFlagEnabledDescriptor = new ElementDescriptor((long)ChapterFlagEnabled, nameof(ChapterFlagEnabled), ElementType.UnsignedInteger);

        /// <summary>A Segment to play in place of this chapter. Edition ChapterSegmentEditionUID should be used for this Segment, otherwise no edition is used.</summary>
        public const ulong ChapterSegmentUID = 0x00006E67;
        public static readonly ElementDescriptor ChapterSegmentUIDDescriptor = new ElementDescriptor((long)ChapterSegmentUID, nameof(ChapterSegmentUID), ElementType.Binary);

        /// <summary>The EditionUID to play from the Segment linked in ChapterSegmentUID.</summary>
        public const ulong ChapterSegmentEditionUID = 0x00006EBC;
        public static readonly ElementDescriptor ChapterSegmentEditionUIDDescriptor = new ElementDescriptor((long)ChapterSegmentEditionUID, nameof(ChapterSegmentEditionUID), ElementType.UnsignedInteger);

        /// <summary>Specify the physical equivalent of this ChapterAtom like "DVD" (60) or "SIDE" (50), see complete list of values.</summary>
        public const ulong ChapterPhysicalEquiv = 0x000063C3;
        public static readonly ElementDescriptor ChapterPhysicalEquivDescriptor = new ElementDescriptor((long)ChapterPhysicalEquiv, nameof(ChapterPhysicalEquiv), ElementType.UnsignedInteger);

        /// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
        public const ulong ChapterTrack = 0x0000008F;
        public static readonly ElementDescriptor ChapterTrackDescriptor = new ElementDescriptor((long)ChapterTrack, nameof(ChapterTrack), ElementType.MasterElement);

        /// <summary>UID of the Track to apply this chapter too. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter should be applied to any currently used Tracks.</summary>
        public const ulong ChapterTrackNumber = 0x00000089;
        public static readonly ElementDescriptor ChapterTrackNumberDescriptor = new ElementDescriptor((long)ChapterTrackNumber, nameof(ChapterTrackNumber), ElementType.UnsignedInteger);

        /// <summary>Contains all possible strings to use for the chapter display.</summary>
        public const ulong ChapterDisplay = 0x00000080;
        public static readonly ElementDescriptor ChapterDisplayDescriptor = new ElementDescriptor((long)ChapterDisplay, nameof(ChapterDisplay), ElementType.MasterElement);

        /// <summary>Contains the string to use as the chapter atom.</summary>
        public const ulong ChapString = 0x00000085;
        public static readonly ElementDescriptor ChapStringDescriptor = new ElementDescriptor((long)ChapString, nameof(ChapString), ElementType.Utf8String);

        /// <summary>The languages corresponding to the string, in the bibliographic ISO-639-2 form.</summary>
        public const ulong ChapLanguage = 0x0000437C;
        public static readonly ElementDescriptor ChapLanguageDescriptor = new ElementDescriptor((long)ChapLanguage, nameof(ChapLanguage), ElementType.AsciiString);

        /// <summary>The countries corresponding to the string, same 2 octets as in Internet domains.</summary>
        public const ulong ChapCountry = 0x0000437E;
        public static readonly ElementDescriptor ChapCountryDescriptor = new ElementDescriptor((long)ChapCountry, nameof(ChapCountry), ElementType.AsciiString);

        /// <summary>Contains all the commands associated to the Atom.</summary>
        public const ulong ChapProcess = 0x00006944;
        public static readonly ElementDescriptor ChapProcessDescriptor = new ElementDescriptor((long)ChapProcess, nameof(ChapProcess), ElementType.MasterElement);

        /// <summary>Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the DVD command set is used. More codec IDs can be added later.</summary>
        public const ulong ChapProcessCodecID = 0x00006955;
        public static readonly ElementDescriptor ChapProcessCodecIDDescriptor = new ElementDescriptor((long)ChapProcessCodecID, nameof(ChapProcessCodecID), ElementType.UnsignedInteger);

        /// <summary>Some optional data attached to the ChapProcessCodecID information. For ChapProcessCodecID = 1, it is the "DVD level" equivalent.</summary>
        public const ulong ChapProcessPrivate = 0x0000450D;
        public static readonly ElementDescriptor ChapProcessPrivateDescriptor = new ElementDescriptor((long)ChapProcessPrivate, nameof(ChapProcessPrivate), ElementType.Binary);

        /// <summary>Contains all the commands associated to the Atom.</summary>
        public const ulong ChapProcessCommand = 0x00006911;
        public static readonly ElementDescriptor ChapProcessCommandDescriptor = new ElementDescriptor((long)ChapProcessCommand, nameof(ChapProcessCommand), ElementType.MasterElement);

        /// <summary>Defines when the process command should be handled (0: during the whole chapter, 1: before starting playback, 2: after playback of the chapter).</summary>
        public const ulong ChapProcessTime = 0x00006922;
        public static readonly ElementDescriptor ChapProcessTimeDescriptor = new ElementDescriptor((long)ChapProcessTime, nameof(ChapProcessTime), ElementType.UnsignedInteger);

        /// <summary>Contains the command information. The data should be interpreted depending on the ChapProcessCodecID value. For ChapProcessCodecID = 1, the data correspond to the binary DVD cell pre/post commands.</summary>
        public const ulong ChapProcessData = 0x00006933;
        public static readonly ElementDescriptor ChapProcessDataDescriptor = new ElementDescriptor((long)ChapProcessData, nameof(ChapProcessData), ElementType.Binary);

        /// <summary>Element containing Elements specific to Tracks/Chapters. A list of valid tags can be found here.</summary>
        public const ulong Tags = 0x1254C367;
        public static readonly ElementDescriptor TagsDescriptor = new ElementDescriptor((long)Tags, nameof(Tags), ElementType.MasterElement);

        /// <summary>Element containing Elements specific to Tracks/Chapters.</summary>
        public const ulong Tag = 0x00007373;
        public static readonly ElementDescriptor TagDescriptor = new ElementDescriptor((long)Tag, nameof(Tag), ElementType.MasterElement);

        /// <summary>Contain all UIDs where the specified meta data apply. It is empty to describe everything in the Segment.</summary>
        public const ulong Targets = 0x000063C0;
        public static readonly ElementDescriptor TargetsDescriptor = new ElementDescriptor((long)Targets, nameof(Targets), ElementType.MasterElement);

        /// <summary>A number to indicate the logical level of the target (see TargetType).</summary>
        public const ulong TargetTypeValue = 0x000068CA;
        public static readonly ElementDescriptor TargetTypeValueDescriptor = new ElementDescriptor((long)TargetTypeValue, nameof(TargetTypeValue), ElementType.UnsignedInteger);

        /// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see TargetType).</summary>
        public const ulong TargetType = 0x000063CA;
        public static readonly ElementDescriptor TargetTypeDescriptor = new ElementDescriptor((long)TargetType, nameof(TargetType), ElementType.AsciiString);

        /// <summary>A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.</summary>
        public const ulong TagTrackUID = 0x000063C5;
        public static readonly ElementDescriptor TagTrackUIDDescriptor = new ElementDescriptor((long)TagTrackUID, nameof(TagTrackUID), ElementType.UnsignedInteger);

        /// <summary>A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.</summary>
        public const ulong TagEditionUID = 0x000063C9;
        public static readonly ElementDescriptor TagEditionUIDDescriptor = new ElementDescriptor((long)TagEditionUID, nameof(TagEditionUID), ElementType.UnsignedInteger);

        /// <summary>A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.</summary>
        public const ulong TagChapterUID = 0x000063C4;
        public static readonly ElementDescriptor TagChapterUIDDescriptor = new ElementDescriptor((long)TagChapterUID, nameof(TagChapterUID), ElementType.UnsignedInteger);

        /// <summary>A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.</summary>
        public const ulong TagAttachmentUID = 0x000063C6;
        public static readonly ElementDescriptor TagAttachmentUIDDescriptor = new ElementDescriptor((long)TagAttachmentUID, nameof(TagAttachmentUID), ElementType.UnsignedInteger);

        /// <summary>Contains general information about the target.</summary>
        public const ulong SimpleTag = 0x000067C8;
        public static readonly ElementDescriptor SimpleTagDescriptor = new ElementDescriptor((long)SimpleTag, nameof(SimpleTag), ElementType.MasterElement);

        /// <summary>The name of the Tag that is going to be stored.</summary>
        public const ulong TagName = 0x000045A3;
        public static readonly ElementDescriptor TagNameDescriptor = new ElementDescriptor((long)TagName, nameof(TagName), ElementType.Utf8String);

        /// <summary>Specifies the language of the tag specified, in the Matroska languages form.</summary>
        public const ulong TagLanguage = 0x0000447A;
        public static readonly ElementDescriptor TagLanguageDescriptor = new ElementDescriptor((long)TagLanguage, nameof(TagLanguage), ElementType.AsciiString);

        /// <summary>Indication to know if this is the default/original language to use for the given tag. (1 bit)</summary>
        public const ulong TagDefault = 0x00004484;
        public static readonly ElementDescriptor TagDefaultDescriptor = new ElementDescriptor((long)TagDefault, nameof(TagDefault), ElementType.UnsignedInteger);

        /// <summary>The value of the Tag.</summary>
        public const ulong TagString = 0x00004487;
        public static readonly ElementDescriptor TagStringDescriptor = new ElementDescriptor((long)TagString, nameof(TagString), ElementType.Utf8String);

        /// <summary>The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.</summary>
        public const ulong TagBinary = 0x00004485;
        public static readonly ElementDescriptor TagBinaryDescriptor = new ElementDescriptor((long)TagBinary, nameof(TagBinary), ElementType.Binary);
        #endregion
    }
}
