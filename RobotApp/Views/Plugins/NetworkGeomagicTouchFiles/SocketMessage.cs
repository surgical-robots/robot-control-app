using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Views.Plugins.NetworkGeomagicTouchFiles
{
    public class SocketMessage
    {
        public SocketMessage()
        {
        }
        //messageType
        private string _messageType;

        //leftOmni
        private double _xOmniLeft;
        private double _yOmniLeft;
        private double _zOmniLeft;
        private double _gimbal1OmniLeft;
        private double _gimbal2OmniLeft;
        private double _gimbal3OmniLeft;
        private double _buttonsLeft;
        private double _inkwellLeft;
        private double _xForceLeft;
        private double _yForceLeft;
        private double _zForceLeft;

        //rightOmni
        private double _xOmniRight;
        private double _yOmniRight;
        private double _zOmniRight;
        private double _gimbal1OmniRight;
        private double _gimbal2OmniRight;
        private double _gimbal3OmniRight;
        private double _buttonsRight;
        private double _inkwellRight;
        private double _xForceRight;
        private double _yForceRight;
        private double _zForceRight;

        //other message type information
        private string _inControl;
        private string _ipAddress;

        //name of connectee and other connecting info
        private string _name;
        private string _port;

        public string Port
        {
            set { this._port = value; }
            get { return this._port; }
        }
        public string Name
        {
            set { this._name = value; }
            get { return this._name; }
        }

        public double InkwellRight
        {
            set { this._inkwellRight = value; }
            get { return this._inkwellRight; }
        }
        public double ButtonsRight
        {
            set { this._buttonsRight = value; }
            get { return this._buttonsRight; }
        }

        public double InkwellLeft
        {
            set { this._inkwellLeft = value; }
            get { return this._inkwellLeft; }
        }
        public double ButtonsLeft
        {
            set { this._buttonsLeft = value; }
            get { return this._buttonsLeft; }
        }

        public string MessageType
        {
            set { this._messageType = value; }
            get { return this._messageType; }
        }
        public double XOmniLeft
        {
            set { this._xOmniLeft = value; }
            get { return this._xOmniLeft; }
        }
        public double YOmniLeft
        {
            set { this._yOmniLeft = value; }
            get { return this._yOmniLeft; }
        }
        public double ZOmniLeft
        {
            set { this._zOmniLeft = value; }
            get { return this._zOmniLeft; }
        }
        public double XForceLeft
        {
            set { this._xForceLeft = value; }
            get { return this._xForceLeft; }
        }
        public double YForceLeft
        {
            set { this._yForceLeft = value; }
            get { return this._yForceLeft; }
        }
        public double ZForceLeft
        {
            set { this._zForceLeft = value; }
            get { return this._zForceLeft; }
        }

        public double Gimbal1OmniLeft
        {
            set { this._gimbal1OmniLeft = value; }
            get { return this._gimbal1OmniLeft; }
        }
        public double Gimbal2OmniLeft
        {
            set { this._gimbal2OmniLeft = value; }
            get { return this._gimbal2OmniLeft; }
        }
        public double Gimbal3OmniLeft
        {
            set { this._gimbal3OmniLeft = value; }
            get { return this._gimbal3OmniLeft; }
        }
        public double XOmniRight
        {
            set { this._xOmniRight = value; }
            get { return this._xOmniRight; }
        }
        public double YOmniRight
        {
            set { this._yOmniRight = value; }
            get { return this._yOmniRight; }
        }
        public double ZOmniRight
        {
            set { this._zOmniRight = value; }
            get { return this._zOmniRight; }
        }
        public double XForceRight
        {
            set { this._xForceRight = value; }
            get { return this._xForceRight; }
        }
        public double YForceRight
        {
            set { this._yForceRight = value; }
            get { return this._yForceRight; }
        }
        public double ZForceRight
        {
            set { this._zForceRight = value; }
            get { return this._zForceRight; }
        }

        public double Gimbal1OmniRight
        {
            set { this._gimbal1OmniRight = value; }
            get { return this._gimbal1OmniRight; }
        }
        public double Gimbal2OmniRight
        {
            set { this._gimbal2OmniRight = value; }
            get { return this._gimbal2OmniRight; }
        }
        public double Gimbal3OmniRight
        {
            set { this._gimbal3OmniRight = value; }
            get { return this._gimbal3OmniRight; }
        }
        public string InControl
        {
            set { this._inControl = value; }
            get { return this._inControl; }
        }
        public string IpAddress
        {
            set { this._ipAddress = value; }
            get { return this._ipAddress; }
        }
    }
}
