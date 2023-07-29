using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaTrackEntry
   {
      public int TrackNumber { get; set; }
      public ulong TrackUID { get; set; }
      public MatroskaTrackType TrackType { get; set; }
      public bool FlagEnabled { get; set; } = true;
      public bool FlagDefault { get; set; } = true;
      public bool FlagForced { get; set; }
      public bool FlagHearingImpaired { get; set; }
      public bool FlagVisualImpaired { get; set; }
      public bool FlagTextDescriptions { get; set; }
      public bool FlagOriginal { get; set; }
      public bool FlagCommentary { get; set; }
      public bool FlagLacing { get; set; } = true;
      public int MinCache { get; set; }
      public int? MaxCache { get; set; }
      public ulong DefaultDuration { get; set; }
      public ulong DefaultDecodedFieldDuration { get; set; }
      public ulong MaxBlockAdditionID { get; set; }
      public List<BlockAdditionMapping> BlockAdditionMappingEntries { get; set; }
      public string Name { get; set; }
      public string Language { get; set; }
      public string LanguageIETF { get; set; }
      public string CodecID { get; set; }
      public byte[] CodecPrivate { get; set; }
      public string CodecName { get; set; }
      public bool CodecDecodeAll { get; set; } = true;
      public List<int> TrackOverlay { get; set; }
      public int CodecDelay { get; set; }
      public int SeekPreRoll { get; set; }
      public List<TrackTranslate> TrackTranslateEntries { get; set; }

      public bool HasVideo { get; set; }
      public VideoInterlacing FlagInterlaced { get; set; } = VideoInterlacing.Undetermined;
      public VideoInterlacingFieldOrder FieldOrder { get; set; } = VideoInterlacingFieldOrder.Undetermined;
      public int StereoMode { get; set; }
      public int AlphaMode { get; set; }
      public int PixelWidth { get; set; }
      public int PixelHeight { get; set; }
      public int PixelCropBottom { get; set; }
      public int PixelCropTop { get; set; }
      public int PixelCropLeft { get; set; }
      public int PixelCropRight { get; set; }
      public int DisplayWidth { get; set; }
      public int DisplayHeight { get; set; }
      public VideoDisplayUnit DisplayUnit { get; set; } = VideoDisplayUnit.Pixels;
      public VideoAspectRatioType AspectRatioType { get; set; } = VideoAspectRatioType.FreeResizing;
      public byte[] UncompressedFourCC { get; set; }
      public double GammaValue { get; set; }
      public double FrameRate { get; set; }

      public bool HasAudio { get; set; }
      public double SamplingFrequency { get; set; }
      public double OutputSamplingFrequency { get; set; }
      public int Channels { get; set; }
      public byte[] ChannelPositions { get; set; }
      public int BitDepth { get; set; }

      public class BlockAdditionMapping
      {
         public ulong BlockAddIDValue { get; set; }
         public string BlockAddIDName { get; set; }
         public ulong BlockAddIDType { get; set; }
         public byte[] BlockAddIDExtraData { get; set; }
      }

      public class TrackTranslate
      {
         public byte[] TrackTranslateTrackID { get; set; }
         public int TrackTranslateCodec { get; set; }
         public byte[] TrackTranslateEditionUID { get; set; }
      }

      public enum VideoInterlacing
      {
         Undetermined = 0,
         Interlaced = 1,
         Progressive = 2,
      }

      public enum VideoInterlacingFieldOrder
      { 
         Progressive = 0,
         Tff = 1,
         Undetermined = 2,
         Bff = 6, 
         BffSwapped = 9,
         TffSwapped = 14,
      }

      public enum VideoDisplayUnit
      {
         Pixels = 0,
         Centimeters = 1,
         Inches = 2,
         DisplayAspectRatio = 3,
         Unknown = 4
      }

      public enum VideoAspectRatioType
      {
         FreeResizing = 0,
         KeepAspectRatio = 1,
         Fixed = 2
      }

      public void CopyTo(MatroskaTrackEntry track, bool shallow = false)
      {
         track.TrackNumber = TrackNumber;
         track.TrackUID = TrackUID;
         track.TrackType = TrackType;
         track.FlagEnabled = FlagEnabled;
         track.FlagDefault = FlagDefault;
         track.FlagForced = FlagForced;
         track.FlagHearingImpaired = FlagHearingImpaired;
         track.FlagVisualImpaired = FlagVisualImpaired;
         track.FlagTextDescriptions = FlagTextDescriptions;
         track.FlagOriginal = FlagOriginal;
         track.FlagCommentary = FlagCommentary;
         track.FlagLacing = FlagLacing;
         track.MinCache = MinCache;
         track.MaxCache = MaxCache;
         track.DefaultDuration = DefaultDuration;
         track.DefaultDecodedFieldDuration = DefaultDecodedFieldDuration;
         track.MaxBlockAdditionID = MaxBlockAdditionID;
         if (shallow) { track.BlockAdditionMappingEntries = BlockAdditionMappingEntries; }
         else
         {
            track.BlockAdditionMappingEntries = null;
            if (BlockAdditionMappingEntries != null)
            {
               track.BlockAdditionMappingEntries = BlockAdditionMappingEntries.Select(x => new BlockAdditionMapping()
               {
                  BlockAddIDExtraData = x.BlockAddIDExtraData,
                  BlockAddIDName = x.BlockAddIDName,
                  BlockAddIDType = x.BlockAddIDType,
                  BlockAddIDValue = x.BlockAddIDValue
               })
               .ToList();
            }
         }
         track.Name = Name;
         track.Language = Language;
         track.LanguageIETF = LanguageIETF;
         track.CodecID = CodecID;
         track.CodecPrivate = CodecPrivate;
         track.CodecName = CodecName;
         track.CodecDecodeAll = CodecDecodeAll;
         if (shallow) { track.TrackOverlay = TrackOverlay; }
         else { track.TrackOverlay = TrackOverlay?.ToList(); }
         track.CodecDelay = CodecDelay;
         track.SeekPreRoll = SeekPreRoll;
         if (shallow) { track.TrackTranslateEntries = TrackTranslateEntries; }
         else
         {
            track.TrackTranslateEntries = null;
            if (TrackTranslateEntries != null)
            {
               track.TrackTranslateEntries = TrackTranslateEntries.Select(x => new TrackTranslate()
               {
                  TrackTranslateCodec = x.TrackTranslateCodec,
                  TrackTranslateEditionUID = x.TrackTranslateEditionUID,
                  TrackTranslateTrackID = x.TrackTranslateTrackID
               })
               .ToList();
            }
         }
         track.HasVideo = HasVideo;
         track.FlagInterlaced = FlagInterlaced;
         track.FieldOrder = FieldOrder;
         track.StereoMode = StereoMode;
         track.AlphaMode = AlphaMode;
         track.PixelWidth = PixelWidth;
         track.PixelHeight = PixelHeight;
         track.PixelCropBottom = PixelCropBottom;
         track.PixelCropTop = PixelCropTop;
         track.PixelCropLeft = PixelCropLeft;
         track.PixelCropRight = PixelCropRight;
         track.DisplayWidth = DisplayWidth;
         track.DisplayHeight = DisplayHeight;
         track.DisplayUnit = DisplayUnit;
         track.AspectRatioType = AspectRatioType;
         track.UncompressedFourCC = UncompressedFourCC;
         track.GammaValue = GammaValue;
         track.FrameRate = FrameRate;
         track.HasAudio = HasAudio;
         track.SamplingFrequency = SamplingFrequency;
         track.Channels = Channels;
         track.ChannelPositions = ChannelPositions;
         track.BitDepth = BitDepth;
      }

      public void ReadFrom(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition != MatroskaSpecification.TrackEntry) { return; }
         foreach (var child in element.Children)
         {
            if (child.Definition == MatroskaSpecification.TrackNumber) { TrackNumber = (int)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.TrackUID) { TrackUID = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.TrackType) { TrackType = (MatroskaTrackType)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.FlagEnabled) { FlagEnabled = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagDefault) { FlagDefault = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagForced) { FlagForced = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagHearingImpaired) { FlagHearingImpaired = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagVisualImpaired) { FlagVisualImpaired = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagTextDescriptions) { FlagTextDescriptions = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagOriginal) { FlagOriginal = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagCommentary) { FlagCommentary = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.FlagLacing) { FlagLacing = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.MinCache) { MinCache = (int)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.MaxCache) { MaxCache = (int)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.DefaultDuration) { DefaultDuration = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.DefaultDecodedFieldDuration) { DefaultDecodedFieldDuration = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.MaxBlockAdditionID) { MaxBlockAdditionID = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.BlockAdditionMapping)
            {
               var entry = new BlockAdditionMapping();
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.BlockAddIDValue) { entry.BlockAddIDValue = child.UIntValue; }
                  else if (sub.Definition == MatroskaSpecification.BlockAddIDName) { entry.BlockAddIDName = child.StringValue; }
                  else if (sub.Definition == MatroskaSpecification.BlockAddIDType) { entry.BlockAddIDType = child.UIntValue; }
                  else if (sub.Definition == MatroskaSpecification.BlockAddIDExtraData) { entry.BlockAddIDExtraData = (child as EBMLBinaryElement).Value.ToArray(); }
               }
               if (BlockAdditionMappingEntries == null) { BlockAdditionMappingEntries = new List<BlockAdditionMapping>(); }
               BlockAdditionMappingEntries.Add(entry);
            }
            else if (child.Definition == MatroskaSpecification.Name) { Name = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.Language) { Language = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.LanguageIETF) { LanguageIETF = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.CodecID) { CodecID = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.CodecPrivate) { CodecPrivate = (child as EBMLBinaryElement).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.CodecName) { CodecName = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.CodecDecodeAll) { CodecDecodeAll = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.TrackOverlay)
            {
               if (TrackOverlay == null) { TrackOverlay = new List<int>(); }
               TrackOverlay.Add((int)child.IntValue);
            }
            else if (child.Definition == MatroskaSpecification.CodecDelay) { CodecDelay = (int)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.SeekPreRoll) { SeekPreRoll = (int)child.IntValue; }
            else if (child.Definition == MatroskaSpecification.TrackTranslate)
            {
               var entry = new TrackTranslate();
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.TrackTranslateTrackID) { entry.TrackTranslateTrackID = (sub as EBMLBinaryElement).Value.ToArray(); }
                  else if (sub.Definition == MatroskaSpecification.TrackTranslateCodec) { entry.TrackTranslateCodec = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.TrackTranslateEditionUID) { entry.TrackTranslateEditionUID = (sub as EBMLBinaryElement).Value.ToArray(); }
               }
               if (TrackTranslateEntries == null) { TrackTranslateEntries = new List<TrackTranslate>(); }
               TrackTranslateEntries.Add(entry);
            }
            else if (child.Definition == MatroskaSpecification.Video)
            {
               HasVideo = true;
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.FlagInterlaced) { FlagInterlaced = (VideoInterlacing)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.FieldOrder) { FieldOrder = (VideoInterlacingFieldOrder)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.StereoMode) { StereoMode = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.AlphaMode) { AlphaMode = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelWidth) { PixelWidth = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelHeight) { PixelHeight = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelCropBottom) { PixelCropBottom = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelCropTop) { PixelCropTop = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelCropLeft) { PixelCropLeft = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.PixelCropRight) { PixelCropRight = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.DisplayWidth) { DisplayWidth = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.DisplayHeight) { DisplayHeight = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.DisplayUnit) { DisplayUnit = (VideoDisplayUnit)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.AspectRatioType) { AspectRatioType = (VideoAspectRatioType)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.UncompressedFourCC) { UncompressedFourCC = (sub as EBMLBinaryElement).Value.ToArray(); }
                  else if (sub.Definition == MatroskaSpecification.GammaValue) { GammaValue = sub.DoubleValue; }
                  else if (sub.Definition == MatroskaSpecification.FrameRate) { FrameRate = sub.DoubleValue; }
                  else if (sub.Definition == MatroskaSpecification.Colour)
                  {
                     /* ... */
                  }
                  else if (sub.Definition == MatroskaSpecification.Projection)
                  {
                     /* ... */
                  }
               }
            }
            else if (child.Definition == MatroskaSpecification.Audio)
            {
               HasAudio = true;
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.SamplingFrequency) { SamplingFrequency = sub.DoubleValue; }
                  else if (sub.Definition == MatroskaSpecification.OutputSamplingFrequency) { OutputSamplingFrequency = sub.DoubleValue; }
                  else if (sub.Definition == MatroskaSpecification.Channels) { Channels = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.ChannelPositions) { ChannelPositions = (sub as EBMLBinaryElement).Value.ToArray(); }
                  else if (sub.Definition == MatroskaSpecification.BitDepth) { BitDepth = (int)sub.IntValue; }
               }
            }
            else if (child.Definition == MatroskaSpecification.TrackOperation)
            {
               /* ... */
            }
            else if (child.Definition == MatroskaSpecification.ContentEncodings)
            {
               /* ... */
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.TrackEntry, cancellationToken);
         await writer.WriteUnsignedInteger(MatroskaSpecification.TrackNumber, (ulong)TrackNumber, cancellationToken);
         await writer.WriteUnsignedInteger(MatroskaSpecification.TrackUID, TrackUID, cancellationToken);
         await writer.WriteUnsignedInteger(MatroskaSpecification.TrackType, (ulong)TrackType, cancellationToken);
         if (!FlagEnabled) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagEnabled, 0, cancellationToken); }
         if (!FlagDefault) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagDefault, 0, cancellationToken); }
         if (FlagForced) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagForced, 1, cancellationToken); }
         if (FlagHearingImpaired) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagHearingImpaired, 1, cancellationToken); }
         if (FlagVisualImpaired) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagVisualImpaired, 1, cancellationToken); }
         if (FlagTextDescriptions) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagTextDescriptions, 1, cancellationToken); }
         if (FlagOriginal) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagOriginal, 1, cancellationToken); }
         if (FlagCommentary) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagCommentary, 1, cancellationToken); }
         await writer.WriteUnsignedInteger(MatroskaSpecification.FlagLacing, FlagLacing ? 1UL : 0UL, cancellationToken);
         if (MinCache > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.MinCache, (ulong)MinCache, cancellationToken); }
         if (MaxCache > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.MaxCache, (ulong)MaxCache, cancellationToken); }
         if (DefaultDuration > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.DefaultDuration, DefaultDuration, cancellationToken); }
         if (DefaultDecodedFieldDuration > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.DefaultDecodedFieldDuration, DefaultDecodedFieldDuration, cancellationToken); }
         if (MaxBlockAdditionID > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.MaxBlockAdditionID, MaxBlockAdditionID, cancellationToken); }
         if (BlockAdditionMappingEntries != null && BlockAdditionMappingEntries.Count > 0)
         {
            for (int i = 0; i < BlockAdditionMappingEntries.Count; i++)
            {
               var entry = BlockAdditionMappingEntries[i];
               await writer.BeginMasterElement(MatroskaSpecification.BlockAdditionMapping, cancellationToken);
               if (entry.BlockAddIDValue > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.BlockAddIDValue, entry.BlockAddIDValue, cancellationToken); }
               if (entry.BlockAddIDName != null) { await writer.WriteString(MatroskaSpecification.BlockAddIDName, entry.BlockAddIDName, cancellationToken); }
               await writer.WriteUnsignedInteger(MatroskaSpecification.BlockAddIDType, entry.BlockAddIDType, cancellationToken);
               if (entry.BlockAddIDExtraData != null) { await writer.WriteBinary(MatroskaSpecification.BlockAddIDExtraData, entry.BlockAddIDExtraData, cancellationToken); }
               await writer.EndMasterElement(cancellationToken);
            }
         }
         if (Name != null) { await writer.WriteString(MatroskaSpecification.Name, Name, cancellationToken); }
         if (Language != null) { await writer.WriteString(MatroskaSpecification.Language, Language, cancellationToken); }
         if (LanguageIETF != null) { await writer.WriteString(MatroskaSpecification.LanguageIETF, LanguageIETF, cancellationToken); }
         if (CodecID != null) { await writer.WriteString(MatroskaSpecification.CodecID, CodecID, cancellationToken); }
         if (CodecPrivate != null) { await writer.WriteBinary(MatroskaSpecification.CodecPrivate, CodecPrivate, cancellationToken); }
         if (CodecName != null) { await writer.WriteString(MatroskaSpecification.CodecName, CodecName, cancellationToken); }
         if (!CodecDecodeAll) { await writer.WriteUnsignedInteger(MatroskaSpecification.CodecDecodeAll, 0, cancellationToken); }
         if (TrackOverlay != null && TrackOverlay.Count > 0)
         {
            for (int i = 0; i < TrackOverlay.Count; i++)
            {
               await writer.WriteUnsignedInteger(MatroskaSpecification.TrackOverlay, (ulong)TrackOverlay[i], cancellationToken);
            }
         }
         if (CodecDelay > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.CodecDelay, (ulong)CodecDelay, cancellationToken); }
         if (SeekPreRoll > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.SeekPreRoll, (ulong)SeekPreRoll, cancellationToken); }
         if (TrackTranslateEntries != null && TrackTranslateEntries.Count > 0)
         {
            for (int i = 0; i < TrackTranslateEntries.Count; i++)
            {
               var entry = TrackTranslateEntries[i];
               await writer.BeginMasterElement(MatroskaSpecification.TrackTranslate, cancellationToken);
               if (entry.TrackTranslateEditionUID != null) { await writer.WriteBinary(MatroskaSpecification.TrackTranslateEditionUID, entry.TrackTranslateEditionUID, cancellationToken); }
               await writer.WriteUnsignedInteger(MatroskaSpecification.TrackTranslateCodec, (ulong)entry.TrackTranslateCodec, cancellationToken);
               await writer.WriteBinary(MatroskaSpecification.TrackTranslateTrackID, entry.TrackTranslateTrackID, cancellationToken);
               await writer.EndMasterElement(cancellationToken);
            }
         }
         if (HasVideo)
         {
            await writer.BeginMasterElement(MatroskaSpecification.Video, cancellationToken);
            if (FlagInterlaced != VideoInterlacing.Undetermined) { await writer.WriteUnsignedInteger(MatroskaSpecification.FlagInterlaced, (ulong)FlagInterlaced, cancellationToken); }
            if (FieldOrder != VideoInterlacingFieldOrder.Undetermined) { await writer.WriteUnsignedInteger(MatroskaSpecification.FieldOrder, (ulong)FieldOrder, cancellationToken); }
            if (StereoMode > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.StereoMode, (ulong)StereoMode, cancellationToken); }
            if (AlphaMode > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.AlphaMode, (ulong)AlphaMode, cancellationToken); }
            await writer.WriteUnsignedInteger(MatroskaSpecification.PixelWidth, (ulong)PixelWidth, cancellationToken);
            await writer.WriteUnsignedInteger(MatroskaSpecification.PixelHeight, (ulong)PixelHeight, cancellationToken);
            if (PixelCropBottom > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.PixelCropBottom, (ulong)PixelCropBottom, cancellationToken); }
            if (PixelCropTop > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.PixelCropTop, (ulong)PixelCropTop, cancellationToken); }
            if (PixelCropLeft > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.PixelCropLeft, (ulong)PixelCropLeft, cancellationToken); }
            if (PixelCropRight > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.PixelCropRight, (ulong)PixelCropRight, cancellationToken); }
            if (DisplayWidth > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.DisplayWidth, (ulong)DisplayWidth, cancellationToken); }
            if (DisplayHeight > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.DisplayHeight, (ulong)DisplayHeight, cancellationToken); }
            if (DisplayUnit != VideoDisplayUnit.Pixels) { await writer.WriteUnsignedInteger(MatroskaSpecification.DisplayUnit, (ulong)DisplayUnit, cancellationToken); }
            if (AspectRatioType != VideoAspectRatioType.FreeResizing) { await writer.WriteUnsignedInteger(MatroskaSpecification.AspectRatioType, (ulong)AspectRatioType, cancellationToken); }
            if (UncompressedFourCC != null) { await writer.WriteBinary(MatroskaSpecification.UncompressedFourCC, UncompressedFourCC, cancellationToken); }
            if (GammaValue > 0) { await writer.WriteFloat(MatroskaSpecification.GammaValue, GammaValue, cancellationToken); }
            if (FrameRate > 0) { await writer.WriteFloat(MatroskaSpecification.FrameRate, FrameRate, cancellationToken); }
            await writer.EndMasterElement(cancellationToken);
         }
         if (HasAudio)
         {
            await writer.BeginMasterElement(MatroskaSpecification.Audio, cancellationToken);
            await writer.WriteFloat(MatroskaSpecification.SamplingFrequency, SamplingFrequency, cancellationToken);
            if (OutputSamplingFrequency > 0) { await writer.WriteFloat(MatroskaSpecification.OutputSamplingFrequency, OutputSamplingFrequency, cancellationToken); }
            await writer.WriteUnsignedInteger(MatroskaSpecification.Channels, (ulong)Channels, cancellationToken);
            if (ChannelPositions != null) { await writer.WriteBinary(MatroskaSpecification.ChannelPositions, ChannelPositions, cancellationToken); }
            if (BitDepth > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.BitDepth, (ulong)BitDepth, cancellationToken); }
            await writer.EndMasterElement(cancellationToken);
         }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.TrackEntry);
         tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TrackNumber, (ulong)TrackNumber));
         tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TrackUID, TrackUID));
         tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TrackType, (ulong)TrackType));
         if (!FlagEnabled) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagEnabled, 0)); }
         if (!FlagDefault) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagDefault, 0)); }
         if (FlagForced) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagForced, 1)); }
         if (FlagHearingImpaired) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagHearingImpaired, 1)); }
         if (FlagVisualImpaired) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagVisualImpaired, 1)); }
         if (FlagTextDescriptions) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagTextDescriptions, 1)); }
         if (FlagOriginal) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagOriginal, 1)); }
         if (FlagCommentary) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagCommentary, 1)); }
         tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagLacing, FlagLacing ? 1UL : 0UL));
         if (MinCache > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.MinCache, (ulong)MinCache)); }
         if (MaxCache > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.MaxCache, (ulong)MaxCache)); }
         if (DefaultDuration > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.DefaultDuration, DefaultDuration)); }
         if (DefaultDecodedFieldDuration > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.DefaultDecodedFieldDuration, DefaultDecodedFieldDuration)); }
         if (MaxBlockAdditionID > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.MaxBlockAdditionID, MaxBlockAdditionID)); }
         if (BlockAdditionMappingEntries != null && BlockAdditionMappingEntries.Count > 0)
         {
            foreach (var entry in BlockAdditionMappingEntries)
            {
               var mapping = new EBMLMasterElement(MatroskaSpecification.BlockAdditionMapping);
               if (entry.BlockAddIDValue > 0) { mapping.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.BlockAddIDValue, entry.BlockAddIDValue)); }
               if (entry.BlockAddIDName != null) { mapping.AddChild(new EBMLStringElement(MatroskaSpecification.BlockAddIDName, entry.BlockAddIDName)); }
               mapping.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.BlockAddIDType, entry.BlockAddIDType));
               if (entry.BlockAddIDExtraData != null) { mapping.AddChild(new EBMLBinaryElement(MatroskaSpecification.BlockAddIDExtraData, entry.BlockAddIDExtraData)); }
               tracks.AddChild(mapping);
            }
         }
         if (Name != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.Name, Name)); }
         if (Language != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.Language, Language)); }
         if (LanguageIETF != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.LanguageIETF, LanguageIETF)); }
         if (CodecID != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.CodecID, CodecID)); }
         if (CodecPrivate != null) { tracks.AddChild(new EBMLBinaryElement(MatroskaSpecification.CodecPrivate, CodecPrivate)); }
         if (CodecName != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.CodecName, CodecName)); }
         if (!CodecDecodeAll) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CodecDecodeAll, 0)); }
         if (TrackOverlay != null && TrackOverlay.Count > 0)
         {
            foreach (var entry in TrackOverlay)
            {
               tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TrackOverlay, (ulong)entry));
            }
         }
         if (CodecDelay > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CodecDelay, (ulong)CodecDelay)); }
         if (SeekPreRoll > 0) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.SeekPreRoll, (ulong)SeekPreRoll)); }
         if (TrackTranslateEntries != null && TrackTranslateEntries.Count > 0)
         {
            foreach (var entry in TrackTranslateEntries)
            {
               var translate = new EBMLMasterElement(MatroskaSpecification.TrackTranslate);
               if (entry.TrackTranslateEditionUID != null) { translate.AddChild(new EBMLBinaryElement(MatroskaSpecification.TrackTranslateEditionUID, entry.TrackTranslateEditionUID)); }
               translate.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TrackTranslateCodec, (ulong)entry.TrackTranslateCodec));
               translate.AddChild(new EBMLBinaryElement(MatroskaSpecification.TrackTranslateTrackID, entry.TrackTranslateTrackID));
               tracks.AddChild(translate);
            }
         }
         if (HasVideo)
         {
            var video = new EBMLMasterElement(MatroskaSpecification.Video);
            if (FlagInterlaced != VideoInterlacing.Undetermined) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FlagInterlaced, (ulong)FlagInterlaced)); }
            if (FieldOrder != VideoInterlacingFieldOrder.Undetermined) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.FieldOrder, (ulong)FieldOrder)); }
            if (StereoMode > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.StereoMode, (ulong)StereoMode)); }
            if (AlphaMode > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.AlphaMode, (ulong)AlphaMode)); }
            video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelWidth, (ulong)PixelWidth));
            video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelHeight, (ulong)PixelHeight));
            if (PixelCropBottom > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelCropBottom, (ulong)PixelCropBottom)); }
            if (PixelCropTop > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelCropTop, (ulong)PixelCropTop)); }
            if (PixelCropLeft > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelCropLeft, (ulong)PixelCropLeft)); }
            if (PixelCropRight > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.PixelCropRight, (ulong)PixelCropRight)); }
            if (DisplayWidth > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.DisplayWidth, (ulong)DisplayWidth)); }
            if (DisplayHeight > 0) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.DisplayHeight, (ulong)DisplayHeight)); }
            if (DisplayUnit != VideoDisplayUnit.Pixels) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.DisplayUnit, (ulong)DisplayUnit)); }
            if (AspectRatioType != VideoAspectRatioType.FreeResizing) { video.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.AspectRatioType, (ulong)AspectRatioType)); }
            if (UncompressedFourCC != null) { video.AddChild(new EBMLBinaryElement(MatroskaSpecification.UncompressedFourCC, UncompressedFourCC)); }
            if (GammaValue > 0) { video.AddChild(new EBMLFloatElement(MatroskaSpecification.GammaValue, GammaValue)); }
            if (FrameRate > 0) { video.AddChild(new EBMLFloatElement(MatroskaSpecification.FrameRate, FrameRate)); }
            tracks.AddChild(video);
         }
         if (HasAudio)
         {
            var audio = new EBMLMasterElement(MatroskaSpecification.Audio);
            audio.AddChild(new EBMLFloatElement(MatroskaSpecification.SamplingFrequency, SamplingFrequency));
            if (OutputSamplingFrequency > 0) { audio.AddChild(new EBMLFloatElement(MatroskaSpecification.OutputSamplingFrequency, OutputSamplingFrequency)); }
            audio.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.Channels, (ulong)Channels));
            if (ChannelPositions != null) { audio.AddChild(new EBMLBinaryElement(MatroskaSpecification.ChannelPositions, ChannelPositions)); }
            if (BitDepth > 0) { audio.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.BitDepth, (ulong)BitDepth)); }
            tracks.AddChild(audio);
         }
         return tracks;
      }
   }
}
