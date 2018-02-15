using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using MathNet.Numerics.LinearAlgebra;

namespace RobotApp.Views.Plugins
{
    public class Calibration
    {
        public byte[] CalibSerialNumber;
        public byte[] CalibPartNumber;
        public byte[] CalibFamilyId;
        public byte[] CalibTime;
        public float[,] BasicMatrix;
        public byte ForceUnits;
        public byte TorqueUnits;
        public float[] MaxRating;
        public int CountsPerForce;
        public int CountsPerTorque;
        public ushort[] GageGain;
        public ushort[] GageOffset;
    }

    public enum ModCommand
    {
        ReadCoils = 0x01,
        ReadDiscreteInputs,
        ReadHoldingRegisters,
        ReadInputRegisters,
        WriteSingleCoil,
        WriteSingleRegister,
        ReadExceptionStatus,
        Diagnostics,
        WriteMultipleRegisters = 0x10,
        ReportServerID,
        ReadFileRecord = 0x14,
        WriteFileRecord,
        MaskWriteRegister,
        ReadWriteMultipleRegisters,
        ReadFIFOQueue,
        UnlockStorage = 0x6A,
        StartDataStream = 0x46
    }

    /// <summary>
    /// Interaction logic for FTSensorATI.xaml
    /// </summary>
    ///
    public partial class FTSensorATI : PluginBase
    {
        private BackgroundWorker workerThread;
        private SerialPort connectPort;
        private int baudRate = 1250000;
        private byte slaveAddress = 10;

        private bool connected = false;
        private bool listen = false;
        private int portCount = 0;
        private int[] buttonNum = new int[5] { 0, 0, 0, 0, 0 };
        private int outputNum;
        private float filterThreshold = 10;
        private int sampleSize = 32;
        private Calibration calib = new Calibration();
        private Matrix<float> basicMatrix = Matrix<float>.Build.Dense(6, 6);
        private Matrix<float> gaugeValues = Matrix<float>.Build.Dense(6, 1);
        private Matrix<float> lastGauge = Matrix<float>.Build.Dense(6, 1);
        private Matrix<float> outValues = Matrix<float>.Build.Dense(6, 1);
        private Matrix<float> offset = Matrix<float>.Build.Dense(6, 1);
        private Matrix<float> diff = Matrix<float>.Build.Dense(6, 1);


        public FTSensorATI()
        {
            this.TypeName = "ATI FT Sensor";
            InitializeComponent();

            Outputs.Add("Fx", new OutputSignalViewModel("Force X"));
            Outputs.Add("Fy", new OutputSignalViewModel("Force Y"));
            Outputs.Add("Fz", new OutputSignalViewModel("Force Z"));
            Outputs.Add("Tx", new OutputSignalViewModel("Torque X"));
            Outputs.Add("Ty", new OutputSignalViewModel("Torque Y"));
            Outputs.Add("Tz", new OutputSignalViewModel("Torque Z"));

            workerThread = new BackgroundWorker();
            workerThread.WorkerSupportsCancellation = true;
            workerThread.DoWork += workerThread_DoWork;

            FindPorts();
        }

        void startStreaming()
        {
            byte[] payload = new byte[5];

            payload[0] = slaveAddress;
            payload[1] = (byte)ModCommand.StartDataStream;
            payload[2] = 0x55;

            byte[] sendCrc = new byte[payload.Length - 2];
            Array.Copy(payload, sendCrc, payload.Length - 2);
            Array.Copy(Mod_CRC(sendCrc, sendCrc.Length), 0, payload, payload.Length - 2, 2);

            connectPort.DiscardInBuffer();

            if (connectPort.IsOpen)
                connectPort.Write(payload, 0, payload.Length);

            Task t = Task.Run(() => { Listen(); });
        }

        void readCalibStructure()
        {
            byte[] payload = new byte[8];
            byte[] receiveBuffer = new byte[255];
            int byteCount = 0;
            payload[0] = slaveAddress;
            payload[1] = (byte)ModCommand.ReadHoldingRegisters;  // read multiple registers
            payload[2] = 0x00;
            payload[3] = 0xe3;
            payload[4] = 0x00;
            payload[5] = 0x7d;

            byte[] sendCrc = new byte[payload.Length - 2];
            Array.Copy(payload, sendCrc, payload.Length - 2);
            Array.Copy(Mod_CRC(sendCrc, sendCrc.Length), 0, payload, payload.Length - 2, 2);

            if (connectPort.IsOpen)
            {
                try
                {
                    connectPort.Write(payload, 0, payload.Length);
                }
                catch(TimeoutException ex)
                {
                    Debug.Print("Write timeout!");
                    return;
                }
            }
            Thread.Sleep(20);
            for(int i = 0; i < 255; i++)
            {
                try
                {
                    receiveBuffer[byteCount] = (byte)connectPort.ReadByte();
                    byteCount++;
                }
                catch (TimeoutException ex)
                {
                    Debug.Print("Read Timeout!");
                }
            }
            calib.CalibSerialNumber = new byte[8];
            Array.Copy(receiveBuffer, 3, calib.CalibSerialNumber, 0, 8);
            calib.CalibPartNumber = new byte[32];
            Array.Copy(receiveBuffer, 11, calib.CalibPartNumber, 0, 32);
            calib.CalibFamilyId = new byte[4];
            Array.Copy(receiveBuffer, 43, calib.CalibFamilyId, 0, 4);
            calib.CalibTime = new byte[20];
            Array.Copy(receiveBuffer, 47, calib.CalibTime, 0, 20);
            calib.BasicMatrix = new float[6, 6];
            int k = 0;
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    calib.BasicMatrix[i, j] = ToFloat(receiveBuffer, 67 + (k * 4));
                    basicMatrix[i, j] = ToFloat(receiveBuffer, 67 + (k * 4));
                    k++;
                }
            }
            calib.ForceUnits = receiveBuffer[100];
            calib.TorqueUnits = receiveBuffer[101];

            ConnectText = "Connected to : " + System.Text.Encoding.Default.GetString(calib.CalibSerialNumber).TrimEnd() + "... Click to Disconnect";
        }

        void changeBaudRate()
        {
            byte[] payload = new byte[11];

            payload[0] = slaveAddress;
            payload[1] = (byte)ModCommand.WriteMultipleRegisters;
            payload[2] = 0x00;
            payload[3] = 0x1e;
            payload[4] = 0x00;
            payload[5] = 0x01;
            payload[6] = 0x02;
            payload[7] = 0x00;
            payload[8] = 0x00;

            byte[] sendCrc = new byte[payload.Length - 2];
            Array.Copy(payload, sendCrc, payload.Length - 2);
            Array.Copy(Mod_CRC(sendCrc, sendCrc.Length), 0, payload, payload.Length - 2, 2);

            if (connectPort.IsOpen)
            {
                connectPort.Write(payload, 0, payload.Length);
            }

            payload[0] = slaveAddress;
            payload[1] = (byte)ModCommand.WriteMultipleRegisters;
            payload[2] = 0x00;
            payload[3] = 0x1F;
            payload[4] = 0x00;
            payload[5] = 0x01;
            payload[6] = 0x02;
            payload[7] = 0x00;
            payload[8] = 0x02;

            Array.Copy(payload, sendCrc, payload.Length - 2);
            Array.Copy(Mod_CRC(sendCrc, sendCrc.Length), 0, payload, payload.Length - 2, 2);

            //payload[0] = slaveAddress;
            //payload[1] = (byte)ModCommand.WriteMultipleRegisters;
            //payload[2] = 0x00;
            //payload[3] = 0x1f;
            //payload[4] = 0x00;
            //payload[5] = 0x01;
            //payload[6] = 0x02;
            //payload[7] = 0x00;
            //payload[8] = 0x00;
            //payload[9] = 0xd7;
            //payload[10] = 0x0f;

            if (connectPort.IsOpen)
            {
                connectPort.Write(payload, 0, payload.Length);
            }

            connectPort.Close();
            connectPort.BaudRate = 115200;
            connectPort.Open();

            payload = new byte[13];

            payload[0] = slaveAddress;
            payload[1] = (byte)ModCommand.WriteMultipleRegisters;
            payload[2] = 0x00;
            payload[3] = 0x20;
            payload[4] = 0x00;
            payload[5] = 0x02;
            payload[6] = 0x04;
            payload[7] = 0x00;
            payload[8] = 0x07;
            payload[9] = 0xC4;
            payload[10] = 0x6D;

            Array.Copy(payload, sendCrc, payload.Length - 2);
            Array.Copy(Mod_CRC(sendCrc, sendCrc.Length), 0, payload, payload.Length - 2, 2);

            if (connectPort.IsOpen)
            {
                connectPort.Write(payload, 0, payload.Length);
            }
        }

        static float ToFloat(byte[] input, int offset)
        {
            byte[] newArray = new[] { input[offset + 3], input[offset + 2], input[offset + 1], input[offset] };
            return BitConverter.ToSingle(newArray, 0);
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            Listen();
        }

        public void FindPorts()
        {
            int removeCount = 0;
            int i = 0;
            SerialPort currentPort;
            string[] ports = SerialPort.GetPortNames();
            PortList = new List<string>(ports);
        }

        private RelayCommand<string> detectCOMsCommand;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> DetectCOMsCommand
        {
            get
            {
                return detectCOMsCommand
                    ?? (detectCOMsCommand = new RelayCommand<string>(
                    p =>
                    {
                        FindPorts();
                    }));
            }
        }

        private RelayCommand<string> connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand<string> ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand<string>(
                    p =>
                    {
                        if(!connected)
                        {
                            if (!connectPort.IsOpen)
                                connectPort.Open();
                            readCalibStructure();
                            startStreaming();
                            connected = true;
                            listen = true;
                        }
                        else
                        {
                            listen = false;
                            Thread.Sleep(100);
                            if (connectPort.IsOpen)
                                connectPort.Close();
                            ConnectText = "Connect to Selected Device";
                            connected = false;
                        }
                    }));
            }
        }

        public void Listen()
        {
            Queue<byte> receiveQueue = new Queue<byte>();
            double avgFx = 0;
            double avgFy = 0;
            double avgFz = 0;
            double avgTx = 0;
            double avgTy = 0;
            double avgTz = 0;
            double oldAvgFx = 0;
            double oldAvgFy = 0;
            double oldAvgFz = 0;
            double oldAvgTx = 0;
            double oldAvgTy = 0;
            double oldAvgTz = 0;
            bool isGood;
            bool initialized = false;

            while(listen)
            {
                Thread.Sleep(15);
                while (connectPort.BytesToRead > 0)
                {
                    receiveQueue.Enqueue((byte)connectPort.ReadByte());

                    if (receiveQueue.Count == 13)
                    {
                        byte[] testArray = receiveQueue.ToArray();
                        if(CheckSample(testArray))
                        {
                            float[] sampleValues = new float[6];
                            for (int i = 0; i < 6; i++)
                            {
                                 sampleValues[i] = ToShort(testArray, i * 2);
                            }

                            gaugeValues[0, 0] = sampleValues[0];
                            gaugeValues[1, 0] = sampleValues[3];
                            gaugeValues[2, 0] = sampleValues[1];
                            gaugeValues[3, 0] = sampleValues[4];
                            gaugeValues[4, 0] = sampleValues[2];
                            gaugeValues[5, 0] = sampleValues[5];

                            outValues = basicMatrix.Multiply(gaugeValues).Multiply((float)0.000001).Subtract(offset);

                            avgFx = ((oldAvgFx * sampleSize) + (outValues[0, 0] - oldAvgFx)) / sampleSize;
                            avgFy = ((oldAvgFy * sampleSize) + (outValues[1, 0] - oldAvgFy)) / sampleSize;
                            avgFz = ((oldAvgFz * sampleSize) + (outValues[2, 0] - oldAvgFz)) / sampleSize;
                            avgTx = ((oldAvgTx * sampleSize) + (outValues[3, 0] - oldAvgTx)) / sampleSize;
                            avgTy = ((oldAvgTy * sampleSize) + (outValues[4, 0] - oldAvgTy)) / sampleSize;
                            avgTz = ((oldAvgTz * sampleSize) + (outValues[5, 0] - oldAvgTz)) / sampleSize;

                            oldAvgFx = avgFx;
                            oldAvgFy = avgFy;
                            oldAvgFz = avgFz;
                            oldAvgTx = avgTx;
                            oldAvgTy = avgTy;
                            oldAvgTz = avgTz;

                            RobotApp.App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                Outputs["Fx"].Value = avgFx;
                                Outputs["Fy"].Value = avgFy;
                                Outputs["Fz"].Value = avgFz;
                                Outputs["Tx"].Value = avgTx;
                                Outputs["Ty"].Value = avgTy;
                                Outputs["Tz"].Value = avgTz;
                            });

                            receiveQueue.Clear();
                        }
                        else
                        {
                            receiveQueue.Dequeue();
                        }
                    }
                }
            }
        }

        bool CheckSample(byte[] sampleBytes)
        {
            int checkSum = 0;
            for (int i = 0; i < 12; i++)
            {
                checkSum = checkSum + sampleBytes[i];
            }

            if ((checkSum & 0x7f) == (sampleBytes[12] & 0x7f))
                return true; /* checksum matches. */
            else
                return false; /* checksum does not match. */
        }

        // Compute the MODBUS CRC
        byte[] Mod_CRC(byte[] buf, int len)
        {
            ushort crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (ushort)buf[pos];        // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                { // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    { // If the LSB is set
                        crc >>= 1;              // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                        // Else LSB is not set
                        crc >>= 1;              // Just shift right
                }
            }

            byte[] crcBytes = new byte[2];
            crcBytes = BitConverter.GetBytes(crc);
            return crcBytes;
        }

        static short ToShort(byte[] input, int offset)
        {
            byte[] newArray = new[] { input[offset + 1], input[offset] };
            return BitConverter.ToInt16(newArray, 0);
        }

        /// <summary>
        /// The <see cref="PortList" /> property's name.
        /// </summary>
        public const string PortListPropertyName = "PortList";

        private List<string> portList = null;

        /// <summary>
        /// Sets and gets the PortList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<string> PortList
        {
            get
            {
                return portList;
            }

            set
            {
                if (portList == value)
                {
                    return;
                }

                portList = value;
                RaisePropertyChanged(PortListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedPort" /> property's name.
        /// </summary>
        public const string SelectedPortPropertyName = "SelectedPort";

        private string selectedPort = null;

        /// <summary>
        /// Sets and gets the SelectedPort property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedPort
        {
            get
            {
                return selectedPort;
            }

            set
            {
                if (selectedPort == value)
                {
                    return;
                }

                selectedPort = value;

                if(selectedPort != null)
                {
                    connectPort = new SerialPort(selectedPort, baudRate);
                    connectPort.Parity = Parity.Even;
                    connectPort.DataBits = 8;
                    connectPort.StopBits = StopBits.One;
                    connectPort.ReadBufferSize = 36;
                    connectPort.WriteTimeout = 10;
                    connectPort.ReadTimeout = 10;
                    RaisePropertyChanged(SelectedPortPropertyName);
                }
            }
        }

        /// <summary>
        /// The <see cref="ConnectText" /> property's name.
        /// </summary>
        public const string ConnectTextPropertyName = "ConnectText";

        private string connectText = "Connect to Selected Device";

        /// <summary>
        /// Sets and gets the ConnectText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectText
        {
            get
            {
                return connectText;
            }

            set
            {
                if (connectText == value)
                {
                    return;
                }

                connectText = value;
                RaisePropertyChanged(ConnectTextPropertyName);
            }
        }

        private RelayCommand<string> zeroCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand<string> ZeroCommand
        {
            get
            {
                return zeroCommand
                    ?? (zeroCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (connected)
                        {
                            offset[0, 0] = (float)Outputs["Fx"].Value;
                            offset[1, 0] = (float)Outputs["Fy"].Value;
                            offset[2, 0] = (float)Outputs["Fz"].Value;
                            offset[3, 0] = (float)Outputs["Tx"].Value;
                            offset[4, 0] = (float)Outputs["Ty"].Value;
                            offset[5, 0] = (float)Outputs["Tz"].Value;
                        }
                    }));
            }
        }
    }
}
