using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace RobotControl
{
    public enum JointCommands
    {
        Reserved,
        Ping,
        HomeUp,
        HomeDown,
        MoveTo,
        GetStatus,
        Configure,
        Associate,
        IdentifyLed,
        Jog,
        ResetCounters,
        DoubleMoveTo,
        SetPosGetData,
        GetHallPos,
        GetPots,
        GetCurrent,
        GetConfiguration,
        SetBaudRate
    }

    public delegate void OnPingEventHandler(Controller sender);
    public delegate void OnMotorCurrentChanged(int newValue);
    public delegate void OnPotValueChanged(int newValue);
    public delegate void OnShaftCounterChanged(int newValue);

    public delegate void OnControlModeChange(ControlMode newValue);
    public delegate void OnKpChange(int newValue);
    public delegate void OnClicksPerRevChange(int newValue);
    public delegate void OnSpeedMinChange(int newValue);
    public delegate void OnCurrentMaxChange(int newValue);
    public delegate void OnPotZeroChange(int newValue);
    public delegate void OnDeadbandChange(int newValue);

    [Serializable]
    public class Controller
    {
        #region Events

        public event OnPingEventHandler OnPing;
        public event OnMotorCurrentChanged Motor1CurrentChanged;
        public event OnMotorCurrentChanged Motor2CurrentChanged;
        public event OnPotValueChanged Motor1PotChanged;
        public event OnPotValueChanged Motor2PotChanged;
        public event OnShaftCounterChanged Motor1CounterChanged;
        public event OnShaftCounterChanged Motor2CounterChanged;

        public event OnControlModeChange Motor1ControlModeChanged;
        public event OnControlModeChange Motor2ControlModeChanged;
        public event OnKpChange Motor1KpChanged;
        public event OnKpChange Motor2KpChanged;
        public event OnClicksPerRevChange Motor1ClicksPerRevChanged;
        public event OnClicksPerRevChange Motor2ClicksPerRevChanged;
        public event OnSpeedMinChange Motor1SpeedMinChanged;
        public event OnSpeedMinChange Motor2SpeedMinChanged;
        public event OnCurrentMaxChange Motor1CurrentMaxChanged;
        public event OnCurrentMaxChange Motor2CurrentMaxChanged;
        public event OnPotZeroChange Motor1PotZeroChanged;
        public event OnPotZeroChange Motor2PotZeroChanged;
        public event OnDeadbandChange Motor1DeadbandChanged;
        public event OnDeadbandChange Motor2DeadbandChanged;

        #endregion

        #region Properties

        public bool UpdatingConfig = false;

        private bool identificationLedIsEnabled;

        public bool IdentificationLedIsEnabled
        {
            get { return identificationLedIsEnabled; }
            set {
                if (identificationLedIsEnabled != value)
                {
                    identificationLedIsEnabled = value;
                    if (identificationLedIsEnabled == true)
                        robot.SendCommand(JointCommands.IdentifyLed, this, new byte[] { 1 });
                    else
                        robot.SendCommand(JointCommands.IdentifyLed, this, new byte[] { 0 });
                }
                
            
            }
        }

        public string FriendlyName { get; set; }

        [IgnoreDataMember()]
        [NonSerialized]
        private Robot robot;

        /// <summary>
        /// The <see cref="Robot"/> this joint belongs to
        /// </summary>
        [IgnoreDataMember()]
        public Robot Robot
        {
            get { return robot; }
            set {
                robot = value;
            }
        }

        private uint id;

        public uint Id
        {
            get { return id; }
            set { id = value;  }
        }

        private double motor1Setpoint;

        public double Motor1Setpoint
        {
            get { return motor1Setpoint; }
            set { motor1Setpoint = value; }
        }

        private double motor2Setpoint;

        public double Motor2Setpoint
        {
            get { return motor2Setpoint; }
            set { motor2Setpoint = value; }
        }

        private int sendNum;

        public int SendNum
        {
            get { return sendNum; }
            set { sendNum = value; }
        }

        private bool getHalls;

        public bool GetHalls
        {
            get { return getHalls; }
            set { getHalls = value; }
        }

        private bool getPots;

        public bool GetPots
        {
            get { return getPots; }
            set { getPots = value; }
        }

        private bool getCurrent;

        public bool GetCurrent
        {
            get { return getCurrent; }
            set { getCurrent = value; }
        }

        #endregion

        #region Commands

        public void UpdateConfiguration()
        {
            //byte[] configuration = new byte[] {
            //    (byte)motor1.ControlMode,
            //    (byte)motor2.ControlMode,
            //    motor1.Kp,
            //    motor2.Kp
            //};
            //Robot.SendCommand(JointCommands.Configure, this, configuration);
        }

        public void Ping()
        {
            robot.SendCommand(JointCommands.Ping, this);
        }

        ushort motor1_current;
        ushort motor2_current;
        ushort motor1_pot;
        ushort motor2_pot;
        int motor1_shaftCounter;
        int motor2_shaftCounter;

        uint motor1_kp;
        uint motor2_kp;
        short motor1_clicksPerRev;
        short motor2_clicksPerRev;
        ushort motor1_speedMin;
        ushort motor2_speedMin;
        ushort motor1_currentMax;
        ushort motor2_currentMax;
        ushort motor1_potZero;
        ushort motor2_potZero;
        ushort motor1_deadband;
        ushort motor2_deadband;
        ControlMode motor1_controlMode;
        ControlMode motor2_controlMode;

        public void SetStatus(byte[] response)
        {
            int i = 6;
            if(getHalls)
            {
                int motor1_shaftCounter_new = BitConverter.ToInt32(response, i);
                int motor2_shaftCounter_new = BitConverter.ToInt32(response, (i + 4));
                i += 8;
                if (motor1_shaftCounter_new != motor1_shaftCounter)
                {
                    motor1_shaftCounter = motor1_shaftCounter_new;
                    if (Motor1CounterChanged != null)
                        Motor1CounterChanged(motor1_shaftCounter);
                }
                if (motor2_shaftCounter_new != motor2_shaftCounter)
                {
                    motor2_shaftCounter = motor2_shaftCounter_new;
                    if (Motor2CounterChanged != null)
                        Motor2CounterChanged(motor2_shaftCounter);
                }
            }

            if(getPots)
            {
                ushort motor1_pot_new = BitConverter.ToUInt16(response, i);
                ushort motor2_pot_new = BitConverter.ToUInt16(response, (i + 2));
                i += 4;
                if (motor1_pot_new != motor1_pot)
                {
                    motor1_pot = motor1_pot_new;
                    if (Motor1PotChanged != null)
                        Motor1PotChanged(motor1_pot);
                }
                if (motor2_pot_new != motor2_pot)
                {
                    motor2_pot = motor2_pot_new;
                    if (Motor2PotChanged != null)
                        Motor2PotChanged(motor2_pot);
                }

            }

            if(getCurrent)
            {
                ushort motor1_current_new = BitConverter.ToUInt16(response, i);
                ushort motor2_current_new = BitConverter.ToUInt16(response, (i + 2));

                if (motor1_current_new != motor1_current)
                {
                    motor1_current = motor1_current_new;
                    if (Motor1CurrentChanged != null)
                        Motor1CurrentChanged(motor1_current);
                }
                if (motor2_current_new != motor2_current)
                {
                    motor2_current = motor2_current_new;
                    if (Motor2CurrentChanged != null)
                        Motor2CurrentChanged(motor2_current);
                }
            }
        }

        public void SetHallPos(byte[] response)
        {
            int motor1_shaftCounter_new = BitConverter.ToInt32(response, 6);
            int motor2_shaftCounter_new = BitConverter.ToInt32(response, 10);

            if (motor1_shaftCounter_new != motor1_shaftCounter)
            {
                motor1_shaftCounter = motor1_shaftCounter_new;
                if (Motor1CounterChanged != null)
                    Motor1CounterChanged(motor1_shaftCounter);
            }

            if (motor2_shaftCounter_new != motor2_shaftCounter)
            {
                motor2_shaftCounter = motor2_shaftCounter_new;
                if (Motor2CounterChanged != null)
                    Motor2CounterChanged(motor2_shaftCounter);
            }
        }

        public void SetPots(byte[] response)
        {
            ushort motor1_pot_new = BitConverter.ToUInt16(response, 6);
            ushort motor2_pot_new = BitConverter.ToUInt16(response, 8);
            
            if (motor1_pot_new != motor1_pot)
            {
                motor1_pot = motor1_pot_new;
                if (Motor1PotChanged != null)
                    Motor1PotChanged(motor1_pot);
            }
            
            if (motor2_pot_new != motor2_pot)
            {
                motor2_pot = motor2_pot_new;
                if (Motor2PotChanged != null)
                    Motor2PotChanged(motor2_pot);
            }
        }

        public void SetCurrent(byte[] response)
        {
            ushort motor1_current_new = BitConverter.ToUInt16(response, 6);
            ushort motor2_current_new = BitConverter.ToUInt16(response, 8);

            if (motor1_current_new != motor1_current)
            {
                motor1_current = motor1_current_new;
                if (Motor1CurrentChanged != null)
                    Motor1CurrentChanged(motor1_current);
            }
            if (motor2_current_new != motor2_current)
            {
                motor2_current = motor2_current_new;
                if (Motor2CurrentChanged != null)
                    Motor2CurrentChanged(motor2_current);
            }
        }

        public void SetData(byte[] response)
        {
        }

        public void SetConfig(byte[] response)
        {
            uint newKp = BitConverter.ToUInt32(response, 7);
            short newClicksPerRev = BitConverter.ToInt16(response, 11);
            ushort newSpeedMin = BitConverter.ToUInt16(response, 13);
            ushort newCurrentMax = BitConverter.ToUInt16(response, 15);
            ushort newPotZero = BitConverter.ToUInt16(response, 17);
            ControlMode newControlMode = (ControlMode)response[19];
            ushort newDeadband = BitConverter.ToUInt16(response, 20);

            UpdatingConfig = true;

            switch (response[6])
            {
                case 0:
                    {
                        motor1_kp = newKp;
                        if (Motor1KpChanged != null)
                            Motor1KpChanged(Convert.ToInt32(motor1_kp));
                        motor1_clicksPerRev = newClicksPerRev;
                        if (Motor1ClicksPerRevChanged != null)
                            Motor1ClicksPerRevChanged(Convert.ToInt32(motor1_clicksPerRev));
                        motor1_speedMin = newSpeedMin;
                        if (Motor1SpeedMinChanged != null)
                            Motor1SpeedMinChanged(Convert.ToInt32(motor1_speedMin));
                        motor1_currentMax = newCurrentMax;
                        if (Motor1CurrentMaxChanged != null)
                            Motor1CurrentMaxChanged(Convert.ToInt32(motor1_currentMax));
                        motor1_potZero = newPotZero;
                        if (Motor1PotZeroChanged != null)
                            Motor1PotZeroChanged(Convert.ToInt32(motor1_potZero));
                        motor1_controlMode = newControlMode;
                        if (Motor1ControlModeChanged != null)
                            Motor1ControlModeChanged(motor1_controlMode);
                        motor1_deadband = newDeadband;
                        if (Motor1DeadbandChanged != null)
                            Motor1DeadbandChanged(Convert.ToInt32(motor1_deadband));
                    }
                    break;
                case 1:
                    {
                        motor2_kp = newKp;
                        if (Motor2KpChanged != null)
                            Motor2KpChanged(Convert.ToInt32(motor2_kp));
                        motor2_clicksPerRev = newClicksPerRev;
                        if (Motor2ClicksPerRevChanged != null)
                            Motor2ClicksPerRevChanged(Convert.ToInt32(motor2_clicksPerRev));
                        motor2_speedMin = newSpeedMin;
                        if (Motor2SpeedMinChanged != null)
                            Motor2SpeedMinChanged(Convert.ToInt32(motor2_speedMin));
                        motor2_currentMax = newCurrentMax;
                        if (Motor2CurrentMaxChanged != null)
                            Motor2CurrentMaxChanged(Convert.ToInt32(motor2_currentMax));
                        motor2_potZero = newPotZero;
                        if (Motor2PotZeroChanged != null)
                            Motor2PotZeroChanged(Convert.ToInt32(motor2_potZero));
                        motor2_controlMode = newControlMode;
                        if (Motor2ControlModeChanged != null)
                            Motor2ControlModeChanged(motor2_controlMode);
                        motor2_deadband = newDeadband;
                        if (Motor2DeadbandChanged != null)
                            Motor2DeadbandChanged(Convert.ToInt32(motor2_deadband));
                    }
                    break;
            }
            UpdatingConfig = false;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Call this function with any response data (including the address data) that's received from this joint.
        /// </summary>
        /// <param name="response"></param>
        internal void ParseResponse(byte[] response)
        {
            
        }

        internal void RaiseOnPing()
        {
            if (OnPing != null)
                OnPing(this);
        }
        #endregion
    }
}
