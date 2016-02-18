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
    class AudioConference
    {
        private TelSurgeMain Main;
        private User User;
        private Surgery Surgery;
        private WaveIn audioWaveIn = null;
        private WaveOut audioWaveOut = null;
        private UdpServer audioServer = null;
        private int audioPort;
        private int m_Codec = 0; //Encode Audio  0: ALAW, 1: ULAW
        public bool InConference { get; set; }

        public AudioConference(TelSurgeMain MainForm, User User, Surgery Surgery, int AudioPort)
        {
            this.Main = MainForm;
            this.User = User;
            this.Surgery = Surgery;
            this.audioPort = AudioPort;
        }
        public void JoinConference(int MicIndex) 
        {
            try
            {
                if (!InConference)
                {
                    audioWaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
                    audioServer = new UdpServer();
                    audioServer.Bindings = new IPEndPoint[] { new IPEndPoint(IPAddress.Parse(User.MyIPAddress), audioPort) };
                    audioServer.PacketReceived += new PacketReceivedHandler(AudioServer_PacketReceived);
                    audioServer.Start();

                    audioWaveIn = new WaveIn(WaveIn.Devices[MicIndex], 8000, 16, 1, 400);
                    audioWaveIn.BufferFull += new BufferFullHandler(audioWaveIn_BufferFull);
                    audioWaveIn.Start();
                    InConference = true;
                }
            }
            catch (Exception ex)
            {
                Main.ShowError("Could not join audio conference. Check input/output devices.", ex.Message);
            }
        }
        public void LeaveConference() 
        {
            try
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
            catch (Exception ex)
            {
                Main.ShowError("Could not leave audio conference.", ex.Message);
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
            try
            {
                audioWaveOut.Play(decodedData, 0, decodedData.Length);
            }
            catch (Exception ex)
            {
                Main.ShowError("Could not play received audio.", ex.Message);
            }
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
            foreach (User usr in Surgery.ConnectedClients)
            {
                if (!usr.MyName.Equals(User.MyName))
                    audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(usr.MyIPAddress), audioPort));
            }
            //If client, send to master
            if (!User.IsMaster)
                audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(Surgery.Master.MyIPAddress), audioPort));
        }
        public List<string> GetAvailableInputDevices()
        {
            List<string> devices = new List<string>();
            try
            {
                foreach (WavInDevice device in WaveIn.Devices)
                {
                    devices.Add(device.Name);
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
            return devices;
        }
        

    }
}
