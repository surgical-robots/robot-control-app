#pragma once
#include "NativeDevice.h"

namespace ForceDimension {
	public ref class Device : INotifyPropertyChanged
	{
	public:
		Device();
		Device(char DeviceID);
		void Start();
		void Stop();
		void UpdateDevice();
		int GetDeviceCount();
		property double X_L { double get(); }
		property double Y_L { double get(); }
		property double Z_L { double get(); }
		property double Theta1_L { double get();  }
		property double Theta2_L { double get();  }
		property double Theta3_L { double get();  }
		property double GripperPos_L { double get(); }
		property double R00_L { double get(); }
		property double R01_L { double get(); }
		property double R02_L { double get(); }
		property double R10_L { double get(); }
		property double R11_L { double get(); }
		property double R12_L { double get(); }
		property double R20_L { double get(); }
		property double R21_L { double get(); }
		property double R22_L { double get(); }

		property double X_R { double get(); }
		property double Y_R { double get(); }
		property double Z_R { double get(); }
		property double Theta1_R { double get();  }
		property double Theta2_R { double get();  }
		property double Theta3_R { double get();  }
		property double GripperPos_R { double get(); }
		property double R00_R { double get(); }
		property double R01_R { double get(); }
		property double R02_R { double get(); }
		property double R10_R { double get(); }
		property double R11_R { double get(); }
		property double R12_R { double get(); }
		property double R20_R { double get(); }
		property double R21_R { double get(); }
		property double R22_R { double get(); }

		virtual event PropertyChangedEventHandler^ PropertyChanged;
		property bool IsInitialized { bool get() { if(device != NULL) return device->deviceInitialized; }}

	private:
		bool isRunning;
		NativeDevice *device;
		double xL;
		double yL;
		double zL;
		double theta1L;
		double theta2L;
		double theta3L;
		double gripperPosL;
		double r00_L;
		double r01_L;
		double r02_L;
		double r10_L;
		double r11_L;
		double r12_L;
		double r20_L;
		double r21_L;
		double r22_L;
	
		double xR;
		double yR;
		double zR;
		double theta1R;
		double theta2R;
		double theta3R;
		double gripperPosR;
		double r00_R;
		double r01_R;
		double r02_R;
		double r10_R;
		double r11_R;
		double r12_R;
		double r20_R;
		double r21_R;
		double r22_R;
	};
}
