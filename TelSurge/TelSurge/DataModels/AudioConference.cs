using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumiSoft.Media.Wave;
using LumiSoft.Net.UDP;
using LumiSoft.Net.Codec;
using System.Net;

namespace TelSurge
{
    public class AudioConference
    {
        private TelSurgeMain Main;
        private WaveIn audioWaveIn = null;
        private WaveOut audioWaveOut = null;
        private UdpServer audioServer = null;
        private int audioPort;
        private int m_Codec = 0; //Encode Audio  0: ALAW, 1: ULAW
        public bool InConference { get; set; }

        public AudioConference(TelSurgeMain MainForm, int AudioPort)
        {
            this.Main = MainForm;
            this.audioPort = AudioPort;
        }
        public void JoinConference(int MicIndex) 
        {
            if (!InConference)
            {
                audioWaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
                audioServer = new UdpServer();
                audioServer.Bindings = new IPEndPoint[] { new IPEndPoint(IPAddress.Parse(Main.User.MyIPAddress), audioPort) };
                audioServer.PacketReceived += new PacketReceivedHandler(AudioServer_PacketReceived);
                audioServer.Start();

                audioWaveIn = new WaveIn(WaveIn.Devices[MicIndex], 8000, 16, 1, 400);
                audioWaveIn.BufferFull += new BufferFullHandler(audioWaveIn_BufferFull);
                audioWaveIn.Start();
                InConference = true;
            }
        }
        public void LeaveConference() 
        {
            if (InConference)
            {
                audioServer.Stop();
                audioServer.Dispose();
                audioWaveOut.Dispose();
                audioWaveIn.Dispose();
                InConference = false;
            }
        }
        private void AudioServer_PacketReceived(UdpPacket_eArgs e)
        {
            // Decompress data.
            byte[] decodedData = null;
            if (m_Codec == 0)
            {
                decodedData = G711.Decode_aLaw(e.Data, 0, e.Data.Length);
            }
            else if (m_Codec == 1)
            {
                decodedData = G711.Decode_uLaw(e.Data, 0, e.Data.Length);
            }

            // just play received packet
            audioWaveOut.Play(decodedData, 0, decodedData.Length);
        }
        private void audioWaveIn_BufferFull(byte[] buffer)
        {
            // Compress data. 
            byte[] encodedData = null;
            if (m_Codec == 0)
            {
                encodedData = G711.Encode_aLaw(buffer, 0, buffer.Length);
            }
            else if (m_Codec == 1)
            {
                encodedData = G711.Encode_uLaw(buffer, 0, buffer.Length);
            }

            //Send to all other clients
            foreach (User usr in Main.Surgery.ConnectedClients)
            {
                if (!usr.MyName.Equals(Main.User.MyName))
                    audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(usr.MyIPAddress), audioPort));
            }
            //If client, send to master
            if (!Main.User.IsMaster)
                audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(Main.Surgery.Master.MyIPAddress), audioPort));
        }
        public List<string> GetAvailableInputDevices()
        {
            List<string> devices = new List<string>();
            foreach (WavInDevice device in WaveIn.Devices)
            {
                devices.Add(device.Name);
            }
            return devices;
        }
        

    }
}
