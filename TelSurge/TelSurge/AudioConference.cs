using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumiSoft.Net.UDP;
using LumiSoft.Net.Codec;
using LumiSoft.Media.Wave;
using System.Net;
using System.Net.Sockets;

namespace TelSurge
{
    class AudioConference
    {
        private TelSurgeMain mainForm;
        public bool IsRunning = false;
        private UdpServer audioServer = null;
        private WaveIn audioWaveIn = null;
        private WaveOut audioWaveOut = null;
        int audioPort = 11003;
        private int m_Codec = 0; //Encode Audio  0: ALAW, 1: ULAW
        private string MyIPAddress;
        private List<string> audioDevices;
        public int SelectedDeviceIndex { get; set; }


        public AudioConference()
        {
            SelectedDeviceIndex = 0;
        }
        public List<string> GetAudioDevices()
        {
            List<string> devices = new List<string>();
            foreach (WavInDevice device in WaveIn.Devices)
            {
                devices.Add(device.Name);
            }
            return devices;
        }
        public void Start() 
        {
            IsRunning = false;
            audioServer.Dispose();
            audioServer = null;
            audioWaveOut.Dispose();
            audioWaveOut = null;
            audioWaveIn.Dispose();
            audioWaveIn = null;
        }
        public void Stop() 
        {
            IsRunning = true;
            audioWaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
            audioServer = new UdpServer();
            audioServer.Bindings = new IPEndPoint[] { new IPEndPoint(IPAddress.Parse(MyIPAddress), audioPort) };
            audioServer.PacketReceived += new PacketReceivedHandler(AudioServer_PacketReceived);
            audioServer.Start();

            audioWaveIn = new WaveIn(WaveIn.Devices[SelectedDeviceIndex], 8000, 16, 1, 400);
            audioWaveIn.BufferFull += new BufferFullHandler(audioWaveIn_BufferFull);
            audioWaveIn.Start();
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

            // We just sent buffer to target end point.
            foreach (string addr in connectedClients)
            {
                //Send to all other clients
                if (!addr.Equals(GetMyIP()))
                    audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(addr), audioPort));
            }
            //If client, send to master
            if (!cb_isMaster.Checked)
                audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(tb_ipAddress.Text), audioPort));
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

            // We just play received packet.
            try
            {
                audioWaveOut.Play(decodedData, 0, decodedData.Length);
            }
            catch (Exception ex)
            {
                mainForm.ShowError("Could not play received audio.", ex.Message);
            }
        }
    }
}
