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

	void Device::UpdateDevice()
	{
		this->device->UpdateDevice();

		if (device->X != x)
		{
			x = device->X;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("X"));
		}

		if (device->Y != y)
		{
			y = device->Y;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Y"));
		}

		if (device->Z != z)
		{
			z = device->Z;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Z"));
		}

		if (device->Theta1 != theta1)
		{
			theta1 = device->Theta1;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta1"));
		}

		if (device->Theta2 != theta2)
		{
			theta2 = device->Theta2;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta2"));
		}

		if (device->Theta3 != theta3)
		{
			theta3 = device->Theta3;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Theta3"));
		}

		if (device->R00 != r00)
		{
			r00 = device->R00;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R00"));
		}

		if (device->R01 != r01)
		{
			r01 = device->R01;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R01"));
		}

		if (device->R02 != r02)
		{
			r02 = device->R02;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R02"));
		}

		if (device->R10 != r10)
		{
			r10 = device->R10;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R10"));
		}

		if (device->R11 != r11)
		{
			r11 = device->R11;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R11"));
		}

		if (device->R12 != r12)
		{
			r12 = device->R12;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R12"));
		}

		if (device->R20 != r20)
		{
			r20 = device->R20;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R20"));
		}

		if (device->R21 != r21)
		{
			r21 = device->R21;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R21"));
		}

		if (device->R22 != r22)
		{
			r22 = device->R22;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("R22"));
		}

		if (device->GripperDeg != gripperPos)
		{
			gripperPos = device->GripperDeg;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("GripperPos"));
		}
	}

	void Device::UpdateForces(double fx, double fy, double fz)
	{
		this->device->UpdateForces(fx, fy, fz);
	}

	int Device::GetDeviceCount()
	{
		return this->device->GetDeviceCount();
	}

	double Device::X::get()
	{
		return x;
	}

	double Device::Y::get()
	{
		return y;
	}

	double Device::Z::get()
	{
		return z;
	}

	double Device::Theta1::get()
	{
		return theta1;
	}

	double Device::Theta2::get()
	{
		return theta2;
	}

	double Device::Theta3::get()
	{
		return theta3;
	}

	double Device::GripperPos::get()
	{
		return gripperPos;
	}

	double Device::R00::get()
	{
		return r00;
	}

	double Device::R01::get()
	{
		return r01;
	}

	double Device::R02::get()
	{
		return r02;
	}
	
	double Device::R10::get()
	{
		return r10;
	}
	
	double Device::R11::get()
	{
		return r11;
	}
	
	double Device::R12::get()
	{
		return r12;
	}

	double Device::R20::get()
	{
		return r20;
	}

	double Device::R21::get()
	{
		return r21;
	}
	
	double Device::R22::get()
	{
		return r22;
	}
	
	ushort Device::SerialNum::get()
	{
		return device->SerialNum;
	}

	bool Device::IsLeft::get()
	{
		return device->IsLeft;
	}
}