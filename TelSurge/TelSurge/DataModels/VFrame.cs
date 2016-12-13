using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TelSurge.DataModels
{
    class SubFrame
    {
        public static int MaxSubFrameSize = 50000;
        public int VFrameID { get; set; }
        public int ID { get; set; } // 1 indexed
        public int VFrameSize { get; set; }
        public int Size { get; set; }
        public byte[] data { get; set; }

        public SubFrame()
        {
            VFrameID = 0;
            ID = 0;
            VFrameSize = 0;
            Size = MaxSubFrameSize;
        }
        public SubFrame(int ID, int Size)
        {
            VFrameID = 0;
            this.ID = ID;
            VFrameSize = 0;
            this.Size = Size;
            data = new byte[Size];
        }
    }


    class VFrame
    {
        private Queue<byte[]> recQueue = new Queue<byte[]>();
        private int ID;

        //Chuck large input data into smaller arrays
        private List<SubFrame> chunkData(byte[] data)
        {
            List<SubFrame> imgChunks = new List<SubFrame>();
            int count = 1;
            for(int i = 0; i < data.Length; i += SubFrame.MaxSubFrameSize)
            {
                int chunkLen = SubFrame.MaxSubFrameSize - 1;
                // check if this is the last chunk
                if ((data.Length - i) < SubFrame.MaxSubFrameSize)
                    chunkLen = (data.Length - i);
                // add chunk to virtual frame
                // add subFrame ID as a tag at the end of each chunk, ID size = 1Byte
                SubFrame chunk = new SubFrame(count, chunkLen);
                Array.Copy(data, i, chunk.data, 0, chunkLen);
                imgChunks.Add(chunk);
                count++;
            }
            //Update VFrame ID and size for every subFrame
            Random rand = new Random();
            ID = rand.Next();
            foreach(SubFrame chunk in imgChunks)
            {
                chunk.VFrameID = ID;
                chunk.VFrameSize = count;
            }
            return imgChunks;
        }

        //Send VFrame in small chunks
        public void Send(byte[] data, string Address, int Port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            List<SubFrame> imgChunks = chunkData(data);
            foreach (SubFrame chunk in imgChunks)
            {
                s.SendTo(SocketData.SerializeObject<SubFrame>(chunk), new IPEndPoint(IPAddress.Parse(Address), Port));
            }
        }

        public void BufferSubFrame(byte[] subFrame)
        {
            /*Logic for subFrame buffer:
                * Add subFrame
                * Check for complete VFrame
                * If possible display VFrame and remove corresponding subFrames
                * If buffer gets full - dump it and start over
            */
        }
    }
}
