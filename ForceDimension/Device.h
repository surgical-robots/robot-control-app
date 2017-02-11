#pragma once
#include "NativeDevice.h"

namespace ForceDimension {
	public ref class Device : INotifyPropertyChanged
	{
	public:
		Device();
		Device(char DeviceID);
		void UpdateDevice();
		void UpdateForces(double fx, double fy, double fz);
		int GetDeviceCount();
		property double X { double get(); }
		property double Y { double get(); }
		property double Z { double get(); }
		property double Theta1 { double get();  }
		property double Theta2 { double get();  }
		property double Theta3 { double get();  }
		property double GripperPos { double get(); }
		property ushort SerialNum { ushort get(); }
		property bool IsLeft { bool get(); }
		property double R00 { double get(); }
		property double R01 { double get(); }
		property double R02 { double get(); }
		property double R10 { double get(); }
		property double R11 { double get(); }
		property double R12 { double get(); }
		property double R20 { double get(); }
		property double R21 { double get(); }
		property double R22 { double get(); }

		virtual event PropertyChangedEventHandler^ PropertyChanged;
		property bool IsInitialized { bool get() { if(device != NULL) return device->deviceInitialized; }}

	private:
		bool isRunning;
		NativeDevice *device;
		double x;
		double y;
		double z;
		double theta1;
		double theta2;
		double theta3;
		double gripperPos;
		double r00;
		double r01;
		double r02;
		double r10;
		double r11;
		double r12;
		double r20;
		double r21;
		double r22;
	};
}
