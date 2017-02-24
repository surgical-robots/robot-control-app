#include "stdafx.h"
#include "Device.h"

namespace ForceDimension
{
	Device::Device()
	{

	}

	Device::Device(char DeviceID)
	{
		this->device = new NativeDevice(DeviceID);
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
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R00"));
		}

		if (device->R01_L != r01_L)
		{
			r01_L = device->R01_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R01"));
		}

		if (device->R02_L != r02_L)
		{
			r02_L = device->R02_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R02"));
		}

		if (device->R10_L != r10_L)
		{
			r10_L = device->R10_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R10"));
		}

		if (device->R11_L != r11_L)
		{
			r11_L = device->R11_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R11"));
		}

		if (device->R12_L != r12_L)
		{
			r12_L = device->R12_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R12"));
		}

		if (device->R20_L != r20_L)
		{
			r20_L = device->R20_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R20"));
		}

		if (device->R21_L != r21_L)
		{
			r21_L = device->R21_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R21"));
		}

		if (device->R22_L != r22_L)
		{
			r22_L = device->R22_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R22"));
		}

		if (device->GripperDeg_L != gripperPosL)
		{
			gripperPosL = device->GripperDeg_L;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("GripperPos"));
		}
	}

	int Device::GetDeviceCount()
	{
		return this->device->GetDeviceCount();
	}

	double Device::X_L::get()
	{
		return xL;
	}

	double Device::Y_L::get()
	{
		return yL;
	}

	double Device::Z_L::get()
	{
		return zL;
	}

	double Device::Theta1_L::get()
	{
		return theta1L;
	}

	double Device::Theta2_L::get()
	{
		return theta2L;
	}

	double Device::Theta3_L::get()
	{
		return theta3L;
	}

	double Device::GripperPos_L::get()
	{
		return gripperPosL;
	}

	double Device::R00_L::get()
	{
		return r00_L;
	}

	double Device::R01_L::get()
	{
		return r01_L;
	}

	double Device::R02_L::get()
	{
		return r02_L;
	}
	
	double Device::R10_L::get()
	{
		return r10_L;
	}
	
	double Device::R11_L::get()
	{
		return r11_L;
	}
	
	double Device::R12_L::get()
	{
		return r12_L;
	}

	double Device::R20_L::get()
	{
		return r20_L;
	}

	double Device::R21_L::get()
	{
		return r21_L;
	}
	
	double Device::R22_L::get()
	{
		return r22_L;
	}

	double Device::X_R::get()
	{
		return xR;
	}

	double Device::Y_R::get()
	{
		return yR;
	}

	double Device::Z_R::get()
	{
		return zR;
	}

	double Device::Theta1_R::get()
	{
		return theta1R;
	}

	double Device::Theta2_R::get()
	{
		return theta2R;
	}

	double Device::Theta3_R::get()
	{
		return theta3R;
	}

	double Device::GripperPos_R::get()
	{
		return gripperPosR;
	}

	double Device::R00_R::get()
	{
		return r00_R;
	}

	double Device::R01_R::get()
	{
		return r01_R;
	}

	double Device::R02_R::get()
	{
		return r02_R;
	}

	double Device::R10_R::get()
	{
		return r10_R;
	}

	double Device::R11_R::get()
	{
		return r11_R;
	}

	double Device::R12_R::get()
	{
		return r12_R;
	}

	double Device::R20_R::get()
	{
		return r20_R;
	}

	double Device::R21_R::get()
	{
		return r21_R;
	}

	double Device::R22_R::get()
	{
		return r22_R;
	}
}