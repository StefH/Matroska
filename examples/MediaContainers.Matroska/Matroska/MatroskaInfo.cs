using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaInfo
   {
      public byte[] SegmentUID { get; set; }
      public string SegmentFilename { get; set; }
      public byte[] PrevUID { get; set; }
      public string PrevFilename { get; set; }
      public byte[] NextUID { get; set; }
      public string NextFilename { get; set; }
      public byte[] SegmentFamily { get; set; }
      public List<ChapterTranslate> ChapterTranslateEntries { get; set; }
      public ulong TimestampScale { get; set; } = 1000000;
      public double Duration { get; set; }
      public DateTime? DateUTC { get; set; }
      public string Title { get; set; }
      public string MuxingApp { get; set; } = "mkvdotnet_1.0";
      public string WritingApp { get; set; } = "mkvdotnet_1.0";

      public class ChapterTranslate
      {
         public byte[] ChapterTranslateEditionUID { get; set; }
         public int ChapterTranslateCodec { get; set; }
         public byte[] ChapterTranslateID { get; set; }
      }

      public void CopyTo(MatroskaInfo info, bool shallow = false, bool copyAppName = false)
      {
         info.SegmentUID = SegmentUID;
         info.SegmentFilename = SegmentFilename;
         info.PrevUID = PrevUID;
         info.PrevFilename = PrevFilename;
         info.NextUID = NextUID;
         info.NextFilename = NextFilename;
         info.SegmentFamily = SegmentFamily;
         if (shallow) { info.ChapterTranslateEntries = ChapterTranslateEntries; }
         else
         {
            info.ChapterTranslateEntries = null;
            if (ChapterTranslateEntries != null)
            {
               info.ChapterTranslateEntries = ChapterTranslateEntries.Select(x => new ChapterTranslate()
               {
                  ChapterTranslateEditionUID = x.ChapterTranslateEditionUID,
                  ChapterTranslateCodec = x.ChapterTranslateCodec,
                  ChapterTranslateID = x.ChapterTranslateID
               })
               .ToList();
            }
         }
         info.TimestampScale = TimestampScale;
         info.Duration = Duration;
         info.DateUTC = DateUTC;
         info.Title = Title;
         if (copyAppName)
         {
            info.MuxingApp = MuxingApp;
            info.WritingApp = WritingApp;
         }
      }

      public void ReadFrom(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition != MatroskaSpecification.Info) { return; }
         foreach (var child in element.Children)
         {
            if (child.Definition == MatroskaSpecification.SegmentUID) { SegmentUID = (child as EBMLBinaryElement).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.SegmentFilename) { SegmentFilename = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.PrevUID) { PrevUID = (child as EBMLBinaryElement).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.PrevFilename) { PrevFilename = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.NextUID) { NextUID = (child as EBMLBinaryElement).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.NextFilename) { NextFilename = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.SegmentFamily) { SegmentFamily = (child as EBMLBinaryElement).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.TimestampScale) { TimestampScale = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.Duration) { Duration = child.DoubleValue; }
            else if (child.Definition == MatroskaSpecification.DateUTC) { DateUTC = child.DateValue; }
            else if (child.Definition == MatroskaSpecification.Title) { Title = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.MuxingApp) { MuxingApp = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.WritingApp) { WritingApp = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.ChapterTranslate)
            {
               var entry = new ChapterTranslate();
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.ChapterTranslateID) { entry.ChapterTranslateID = (sub as EBMLBinaryElement).Value.ToArray(); }
                  else if (sub.Definition == MatroskaSpecification.ChapterTranslateCodec) { entry.ChapterTranslateCodec = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.ChapterTranslateEditionUID) { entry.ChapterTranslateEditionUID = (sub as EBMLBinaryElement).Value.ToArray(); }
               }
               if (ChapterTranslateEntries == null) { ChapterTranslateEntries = new List<ChapterTranslate>(); }
               ChapterTranslateEntries.Add(entry);
            }
         }
      }

      public async ValueTask<EBMLWriter.RelativeOffset> Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         EBMLWriter.RelativeOffset durationMark = default;
         await writer.BeginMasterElement(MatroskaSpecification.Info, cancellationToken);
         if (SegmentUID != null) { await writer.WriteBinary(MatroskaSpecification.SegmentUID, SegmentUID, cancellationToken); }
         if (SegmentFilename != null) { await writer.WriteString(MatroskaSpecification.SegmentFilename, SegmentFilename, cancellationToken); }
         if (PrevUID != null) { await writer.WriteBinary(MatroskaSpecification.PrevUID, PrevUID, cancellationToken); }
         if (PrevFilename != null) { await writer.WriteString(MatroskaSpecification.PrevFilename, SegmentFilename, cancellationToken); }
         if (NextUID != null) { await writer.WriteBinary(MatroskaSpecification.NextUID, NextUID, cancellationToken); }
         if (NextFilename != null) { await writer.WriteString(MatroskaSpecification.NextFilename, NextFilename, cancellationToken); }
         if (SegmentFamily != null) { await writer.WriteBinary(MatroskaSpecification.SegmentFamily, SegmentFamily, cancellationToken); }
         if (ChapterTranslateEntries != null && ChapterTranslateEntries.Count > 0)
         {
            await writer.BeginMasterElement(MatroskaSpecification.ChapterTranslate, cancellationToken);
            for (int i = 0; i < ChapterTranslateEntries.Count; i++)
            {
               var item = ChapterTranslateEntries[i];
               if (item.ChapterTranslateID != null) { await writer.WriteBinary(MatroskaSpecification.ChapterTranslateID, item.ChapterTranslateID, cancellationToken); }
               await writer.WriteUnsignedInteger(MatroskaSpecification.ChapterTranslateCodec, (ulong)item.ChapterTranslateCodec, cancellationToken);
               await writer.WriteBinary(MatroskaSpecification.ChapterTranslateEditionUID, item.ChapterTranslateEditionUID, cancellationToken);
            }
            await writer.EndMasterElement(cancellationToken);
         }
         await writer.WriteUnsignedInteger(MatroskaSpecification.TimestampScale, TimestampScale, cancellationToken);
         if (Duration > 0)
         {
            durationMark = writer.MarkRelativeOffset(null);
            await writer.WriteFloat(MatroskaSpecification.Duration, Duration, cancellationToken);
         }
         if (DateUTC != null) { await writer.WriteDate(MatroskaSpecification.DateUTC, DateUTC.Value, cancellationToken); }
         if (Title != null) { await writer.WriteString(MatroskaSpecification.Title, Title, cancellationToken); }
         await writer.WriteString(MatroskaSpecification.MuxingApp, MuxingApp, cancellationToken);
         await writer.WriteString(MatroskaSpecification.WritingApp, WritingApp, cancellationToken);
         await writer.EndMasterElement(cancellationToken);
         return durationMark;
      }

      public EBMLMasterElement ToElement()
      {
         var info = new EBMLMasterElement(MatroskaSpecification.Info);
         if (SegmentUID != null) { info.AddChild(new EBMLBinaryElement(MatroskaSpecification.SegmentUID, SegmentUID)); }
         if (SegmentFilename != null) { info.AddChild(new EBMLStringElement(MatroskaSpecification.SegmentFilename, SegmentFilename)); }
         if (PrevUID != null) { info.AddChild(new EBMLBinaryElement(MatroskaSpecification.PrevUID, PrevUID)); }
         if (PrevFilename != null) { info.AddChild(new EBMLStringElement(MatroskaSpecification.PrevFilename, PrevFilename)); }
         if (NextUID != null) { info.AddChild(new EBMLBinaryElement(MatroskaSpecification.NextUID, NextUID)); }
         if (NextFilename != null) { info.AddChild(new EBMLStringElement(MatroskaSpecification.NextFilename, NextFilename)); }
         if (SegmentFamily != null) { info.AddChild(new EBMLBinaryElement(MatroskaSpecification.SegmentFamily, SegmentFamily)); }
         if (ChapterTranslateEntries != null && ChapterTranslateEntries.Count > 0)
         {
            var chap = new EBMLMasterElement(MatroskaSpecification.ChapterTranslate);
            foreach (var item in ChapterTranslateEntries)
            {
               if (item.ChapterTranslateID != null) { info.AddChild(new EBMLBinaryElement(MatroskaSpecification.ChapterTranslateID, item.ChapterTranslateID)); }
               info.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.ChapterTranslateCodec, (ulong)item.ChapterTranslateCodec));
               info.AddChild(new EBMLBinaryElement(MatroskaSpecification.ChapterTranslateEditionUID, item.ChapterTranslateEditionUID));
            }
            info.AddChild(chap);
         }
         info.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TimestampScale, TimestampScale));
         if (Duration > 0) { info.AddChild(new EBMLFloatElement(MatroskaSpecification.Duration, Duration)); }
         if (DateUTC != null) { info.AddChild(new EBMLDateElement(MatroskaSpecification.DateUTC, DateUTC.Value)); }
         if (Title != null) { info.AddChild(new EBMLStringElement(MatroskaSpecification.Title, Title)); }
         info.AddChild(new EBMLStringElement(MatroskaSpecification.MuxingApp, MuxingApp));
         info.AddChild(new EBMLStringElement(MatroskaSpecification.WritingApp, WritingApp));
         return info;
      }
   }
}
