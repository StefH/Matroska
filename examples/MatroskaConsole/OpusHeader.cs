using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matroska
{
    class OpusHeader
    {
        /*
         * typedef struct {
   int version;
   int channels; // Number of channels: 1..255
        int preskip;
        ogg_uint32_t input_sample_rate;
        int gain; // in dB S7.8 should be zero whenever possible 
        int channel_mapping;
        // The rest is only used if channel_mapping != 0 
        int nb_streams;
        int nb_coupled;
        unsigned char stream_map[255];
        unsigned char dmatrix[OPUS_DEMIXING_MATRIX_SIZE_MAX];
    }
    OpusHeader;*/
        public string ID = "OpusHead";
        public string OpusTags = "OpusTags";
        public byte Version;
        public byte OutputChannelCount;
        public ushort PreSkip;
        public uint InputSampleRate;
        public short OutputGain;
        public byte ChannelMappingFamily;

        public byte StreamCount;
        public byte CoupledStreamCount;
        public byte[] ChannelMapping;

        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(Version);
            w.Write(OutputChannelCount);
            w.Write(PreSkip);
            w.Write(InputSampleRate);
            w.Write(OutputGain);
            w.Write(ChannelMappingFamily);
            // w.Write(StreamCount);
            //w.Write(CoupledStreamCount);
            //w.Write(ChannelMapping);

            w.Write(Encoding.ASCII.GetBytes(OpusTags));
            w.Write(0L);
        }
    }
}