#include "stdafx.h"
#include "Device.h"

namespace RazerHydra
{
	Device::Device()
	{
		this->device = new NativeDevice();
	}

	void Device::Start()
	{
		this->device->Start();
	}

	void Device::Stop()
	{
		this->device->Stop();
	}

	void Device::UpdateDevice()
	{
		this->device->UpdateDevice();

		// LEFT CONTROLLER
		if (device->X_L != xL)
		{
			xL = device->X_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("X_L"));
		}

		if (device->Y_L != yL)
		{
			yL = device->Y_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Y_L"));
		}

		if (device->Z_L != zL)
		{
			zL = device->Z_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Z_L"));
		}

		if (device->Theta1_L != theta1L)
		{
			theta1L = device->Theta1_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta1_L"));
		}

		if (device->Theta2_L != theta2L)
		{
			theta2L = device->Theta2_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta2_L"));
		}

		if (device->Theta3_L != theta3L)
		{
			theta3L = device->Theta3_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta3_L"));
		}

		if (device->R00_L != r00_L)
		{
			r00_L = device->R00_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R00_L"));
		}

		if (device->R01_L != r01_L)
		{
			r01_L = device->R01_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R01_L"));
		}

		if (device->R02_L != r02_L)
		{
			r02_L = device->R02_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R02_L"));
		}

		if (device->R10_L != r10_L)
		{
			r10_L = device->R10_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R10_L"));
		}

		if (device->R11_L != r11_L)
		{
			r11_L = device->R11_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R11_L"));
		}

		if (device->R12_L != r12_L)
		{
			r12_L = device->R12_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R12_L"));
		}

		if (device->R20_L != r20_L)
		{
			r20_L = device->R20_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R20_L"));
		}

		if (device->R21_L != r21_L)
		{
			r21_L = device->R21_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R21_L"));
		}

		if (device->R22_L != r22_L)
		{
			r22_L = device->R22_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R22_L"));
		}

		if (device->GripperDeg_L != gripperPosL)
		{
			gripperPosL = device->GripperDeg_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("GripperPos_L"));
		}

		if (device->JoystickX_L != joystickX_L)
		{
			joystickX_L = device->JoystickX_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("JoystickX_L"));
		}

		if (device->JoystickY_L != joystickY_L)
		{
			joystickY_L = device->JoystickY_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("JoystickY_L"));
		}

		if (device->Bumper_L != bumper_L)
		{
			bumper_L = device->Bumper_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Bumper_L"));
		}

		if (device->Button1_L != button1_L)
		{
			button1_L = device->Button1_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button1_L"));
		}

		if (device->Button2_L != button2_L)
		{
			button2_L = device->Button2_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button2_L"));
		}

		if (device->Button3_L != button3_L)
		{
			button3_L = device->Button3_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button3_L"));
		}

		if (device->Button4_L != button4_L)
		{
			button4_L = device->Button4_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button4_L"));
		}

		// RIGHT CONTROLLER
		if (device->X_R != xR)
		{
			xR = device->X_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("X_R"));
		}

		if (device->Y_R != yR)
		{
			yR = device->Y_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Y_R"));
		}

		if (device->Z_R != zR)
		{
			zR = device->Z_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Z_R"));
		}

		if (device->Theta1_R != theta1R)
		{
			theta1R = device->Theta1_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta1_R"));
		}

		if (device->Theta2_R != theta2R)
		{
			theta2R = device->Theta2_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta2_R"));
		}

		if (device->Theta3_R != theta3R)
		{
			theta3R = device->Theta3_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta3_R"));
		}

		if (device->R00_R != r00_R)
		{
			r00_R = device->R00_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R00_R"));
		}

		if (device->R01_R != r01_R)
		{
			r01_R = device->R01_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R01_R"));
		}

		if (device->R02_R != r02_R)
		{
			r02_R = device->R02_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R02_R"));
		}

		if (device->R10_R != r10_R)
		{
			r10_R = device->R10_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R10_R"));
		}

		if (device->R11_R != r11_R)
		{
			r11_R = device->R11_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R11_R"));
		}

		if (device->R12_R != r12_R)
		{
			r12_R = device->R12_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R12_R"));
		}

		if (device->R20_R != r20_R)
		{
			r20_R = device->R20_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R20_R"));
		}

		if (device->R21_R != r21_R)
		{
			r21_R = device->R21_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R21_R"));
		}

		if (device->R22_R != r22_R)
		{
			r22_R = device->R22_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R22_R"));
		}

		if (device->GripperDeg_R != gripperPosR)
		{
			gripperPosR = device->GripperDeg_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("GripperPos_R"));
		}

		if (device->JoystickX_R != joystickX_R)
		{
			joystickX_R = device->JoystickX_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("JoystickX_R"));
		}

		if (device->JoystickY_R != joystickY_R)
		{
			joystickY_R = device->JoystickY_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("JoystickY_R"));
		}

		if (device->Bumper_R != bumper_R)
		{
			bumper_R = device->Bumper_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Bumper_R"));
		}

		if (device->Button1_R != button1_R)
		{
			button1_R = device->Button1_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button1_R"));
		}

		if (device->Button2_R != button2_R)
		{
			button2_R = device->Button2_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button2_R"));
		}

		if (device->Button3_R != button3_R)
		{
			button3_R = device->Button3_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button3_R"));
		}

		if (device->Button4_R != button4_R)
		{
			button4_R = device->Button4_R;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button4_R"));
		}
	}

	int Device::GetDeviceCount() {	return this->device->GetDeviceCount();	}

	double Device::X_L::get() {	return xL;	}
	double Device::Y_L::get() {	return yL;	}
	double Device::Z_L::get() {	return zL;	}
	double Device::Theta1_L::get() { return theta1L; }
	double Device::Theta2_L::get() { return theta2L; }
	double Device::Theta3_L::get() { return theta3L; }
	double Device::GripperPos_L::get() { return gripperPosL; }
	double Device::R00_L::get()	{ return r00_L; }
	double Device::R01_L::get()	{ return r01_L;	}
	double Device::R02_L::get()	{ return r02_L;	}
	double Device::R10_L::get()	{ return r10_L;	}
	double Device::R11_L::get()	{ return r11_L;	}
	double Device::R12_L::get()	{ return r12_L;	}
	double Device::R20_L::get()	{ return r20_L;	}
	double Device::R21_L::get()	{ return r21_L;	}
	double Device::R22_L::get()	{ return r22_L;	}
	double Device::JoystickX_L::get() { return joystickX_L; }
	double Device::JoystickY_L::get() { return joystickY_L; }
	double Device::Bumper_L::get() { return bumper_L; }
	double Device::Button1_L::get() { return button1_L; }
	double Device::Button2_L::get() { return button2_L; }
	double Device::Button3_L::get() { return button3_L; }
	double Device::Button4_L::get() { return button4_L; }


	double Device::X_R::get() { return xR; }
	double Device::Y_R::get() { return yR; }
	double Device::Z_R::get() { return zR; }
	double Device::Theta1_R::get() { return theta1R; }
	double Device::Theta2_R::get() { return theta2R; }
	double Device::Theta3_R::get() { return theta3R; }
	double Device::GripperPos_R::get() { return gripperPosR; }
	double Device::R00_R::get()	{ return r00_R; }
	double Device::R01_R::get()	{ return r01_R; }
	double Device::R02_R::get()	{ return r02_R; }
	double Device::R10_R::get()	{ return r10_R; }
	double Device::R11_R::get()	{ return r11_R; }
	double Device::R12_R::get()	{ return r12_R; }
	double Device::R20_R::get()	{ return r20_R; }
	double Device::R21_R::get()	{ return r21_R; }
	double Device::R22_R::get()	{ return r22_R; }
	double Device::JoystickX_R::get() { return joystickX_R; }
	double Device::JoystickY_R::get() { return joystickY_R; }
	double Device::Bumper_R::get() { return bumper_R; }
	double Device::Button1_R::get() { return button1_R; }
	double Device::Button2_R::get() { return button2_R; }
	double Device::Button3_R::get() { return button3_R; }
	double Device::Button4_R::get() { return button4_R; }
	
}