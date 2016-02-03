using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Timers;
using System.Diagnostics;

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
        GetCurrent
    }

    public delegate void OnPingEventHandler(Controller sender);
    public delegate void OnMotorCurrentChanged(int newValue);
    public delegate void OnPotValueChanged(int newValue);
    public delegate void OnShaftCounterChanged(int newValue);

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

        #endregion

        #region Properties

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
