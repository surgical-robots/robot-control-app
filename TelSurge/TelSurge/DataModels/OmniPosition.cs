using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeomagicTouch;

namespace TelSurge.DataModels
{
    public class OmniPosition
    {
        public double LeftX { get; set; }
        public double LeftY { get; set; }
        public double LeftZ { get; set; }
        public double Gimbal1Left { get; set; }
        public double Gimbal2Left { get; set; }
        public double Gimbal3Left { get; set; }
        public double R00Left { get; set; }
        public double R01Left { get; set; }
        public double R02Left { get; set; }
        public double ButtonsLeft { get; set; }
        public double InkwellLeft { get; set; }
        public double RightX { get; set; }
        public double RightY { get; set; }
        public double RightZ { get; set; }
        public double R00Right { get; set; }
        public double R01Right { get; set; }
        public double R02Right { get; set; }
        public double Gimbal1Right { get; set; }
        public double Gimbal2Right { get; set; }
        public double Gimbal3Right { get; set; }
        public double ButtonsRight { get; set; }
        public double InkwellRight { get; set; }
        public bool[] ExtraButtons { get; set; }
        
        public OmniPosition()
        {
            LeftX = 0;
            LeftY = 0;
            LeftZ = 0;
            Gimbal1Left = 0;
            Gimbal2Left = 0;
            Gimbal3Left = 0;
            ButtonsLeft = 0;
            InkwellLeft = 0;
            R00Left = 0;
            R01Left = 0;
            R02Left = 0;

            RightX = 0;
            RightY = 0;
            RightZ = 0;
            Gimbal1Right = 0;
            Gimbal2Right = 0;
            Gimbal3Right = 0;
            R00Right = 0;
            R01Right = 0;
            R02Right = 0;
            ButtonsRight = 0;
            InkwellRight = 0;

            ExtraButtons = new bool[0];
        }
        public OmniPosition(double leftx, double lefty, double leftz, double rightx, double righty, double rightz)
        {
            this.LeftX = leftx;
            this.LeftY = lefty;
            this.LeftZ = leftz;
            this.RightX = rightx;
            this.RightY = righty;
            this.RightZ = rightz;
            ExtraButtons = new bool[0];
        }

        public OmniPosition(double leftX, double leftY, double leftZ, double gimbal1Left, double gimbal2Left, double gimbal3Left, double buttonsLeft, double inkwellLeft, double rightX, double rightY, double rightZ, double gimbal1Right, double gimbal2Right, double gimbal3Right, double buttonsRight, double inkwellRight, bool[] extraButtons)
        {
            LeftX = leftX;
            LeftY = leftY;
            LeftZ = leftZ;
            Gimbal1Left = gimbal1Left;
            Gimbal2Left = gimbal2Left;
            Gimbal3Left = gimbal3Left;
            //R00Left = r00Left;
            //R01Left = r01Left;
            //R02Left = r02Left;
            ButtonsLeft = buttonsLeft;
            InkwellLeft = inkwellLeft;

            RightX = rightX;
            RightY = rightY;
            RightZ = rightZ;
            Gimbal1Right = gimbal1Right;
            Gimbal2Right = gimbal2Right;
            Gimbal3Right = gimbal3Right;
            //R00Right = r00Right;
            //R01Right = r01Right;
            //R02Right = r02Right;
            ButtonsRight = buttonsRight;
            InkwellRight = inkwellRight;
            ExtraButtons = extraButtons;
        }

        public OmniPosition(double[] leftPos, double[] rightPos)
        {
            LeftX = leftPos[0];
            LeftY = leftPos[1];
            LeftZ = leftPos[2];
            Gimbal1Left = leftPos[3];
            Gimbal2Left = leftPos[4];
            Gimbal3Left = leftPos[5];
            ButtonsLeft = leftPos[6];
            InkwellLeft = leftPos[7];

            RightX = rightPos[0];
            RightY = rightPos[1];
            RightZ = rightPos[2];
            Gimbal1Right = rightPos[3];
            Gimbal2Right = rightPos[4];
            Gimbal3Right = rightPos[5];
            ButtonsRight = rightPos[6];
            InkwellRight = rightPos[7];
            ExtraButtons = new bool[0];
        }

        public OmniPosition(Device Left, Device Right)
        {
            LeftX = Left.X;
            LeftY = Left.Y;
            LeftZ = Left.Z;
            Gimbal1Left = Left.Theta1;// *(180 / Math.PI);
            Gimbal2Left = Left.Theta2;// * (180 / Math.PI);
            Gimbal3Left = Left.Theta3;// * (180 / Math.PI);
            R00Left = Left.R22;
            R01Left = Left.R02;
            R02Left = Left.R12;
            ButtonsLeft = 0;
            if (Left.Button1)
                ButtonsLeft = 1;
            else if (Left.Button2)
                ButtonsLeft = 2;
            InkwellLeft = Convert.ToDouble(Left.IsInInkwell);

            RightX = Right.X;
            RightY = Right.Y;
            RightZ = Right.Z;
            Gimbal1Right = Right.Theta1;// * (180 / Math.PI);
            Gimbal2Right = Right.Theta2;// * (180 / Math.PI);
            Gimbal3Right = Right.Theta3;// * (180 / Math.PI);
            R00Right = Right.R22;
            R01Right = Right.R02;
            R02Right = Right.R12;
            ButtonsRight = 0;
            if (Right.Button1)
                ButtonsRight = 1;
            else if (Right.Button2)
                ButtonsRight = 2;
            InkwellRight = Convert.ToDouble(Right.IsInInkwell);
            ExtraButtons = new bool[0];
        }

        public OmniPosition Add(OmniPosition pos)
        {
           return new OmniPosition(
                LeftX + pos.LeftX,
                LeftY + pos.LeftY,
                LeftZ + pos.LeftZ,
                Gimbal1Left + pos.Gimbal1Left,
                Gimbal2Left + pos.Gimbal2Left,
                Gimbal3Left + pos.Gimbal3Left,
                ButtonsLeft,
                InkwellLeft,
                RightX + pos.RightX,
                RightY + pos.RightY,
                RightZ + pos.RightZ,
                Gimbal1Right + pos.Gimbal1Right,
                Gimbal2Right + pos.Gimbal2Right,
                Gimbal3Right + pos.Gimbal3Right,
                ButtonsRight,
                InkwellRight,
                ExtraButtons
            );
        }
        public OmniPosition Subtract(OmniPosition pos)
        {
            return new OmniPosition(
                LeftX - pos.LeftX,
                LeftY - pos.LeftY,
                LeftZ - pos.LeftZ,
                Gimbal1Left - pos.Gimbal1Left,
                Gimbal2Left - pos.Gimbal2Left,
                Gimbal3Left - pos.Gimbal3Left,
                ButtonsLeft,
                InkwellLeft,
                RightX - pos.RightX,
                RightY - pos.RightY,
                RightZ - pos.RightZ,
                Gimbal1Right - pos.Gimbal1Right,
                Gimbal2Right - pos.Gimbal2Right,
                Gimbal3Right - pos.Gimbal3Right,
                ButtonsRight,
                InkwellRight,
                ExtraButtons
            );
        }
    }
}
