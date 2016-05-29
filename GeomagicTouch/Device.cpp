#include "stdafx.h"
#include "Device.h"
#include <string>

#include <msclr\marshal_cppstd.h>
using namespace System; 
using namespace System::Configuration;

namespace GeomagicTouch {

	Device::Device(String^ DeviceName)
	{
		deviceName = DeviceName;
		msclr::interop::marshal_context context;
		std::string nativeString = context.marshal_as<std::string>(DeviceName);
		this->device = new NativeDevice(nativeString);
	}

	void Device::Start()
	{
		if (isRunning) return;
		this->device->Start();
		isRunning = true;
	}

	void Device::LoadObj(String^ ObjectPath)
	{
		if (!isRunning) return;
		msclr::interop::marshal_context context;
		std::string nativeString = context.marshal_as<std::string>(ObjectPath);
		this->device->LoadObj(nativeString);
	}

	void Device::Stop()
	{
		this->device->Stop();
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

	bool Device::IsInInkwell::get()
	{
		return isInInkwell;
	}

	bool Device::Button1::get()
	{
		return button1;
	}

	bool Device::Button2::get()
	{
		return button2;
	}

	bool Device::Button3::get()
	{
		return button3;
	}

	bool Device::Button4::get()
	{
		return button4;
	}

	double Device::SetpointX::get()
	{
		return device->SetpointX;
	}

	void Device::SetpointX::set(double value)
	{
		// Don't update anything if the value hasn't changed
		if (device->SetpointX == value)
			return;

		device->SetpointX = value;
		// Whenever we change the setpoint, call the Update function to update the internal callback structure
		device->UpdateSetpoint();
	}

	double Device::SetpointY::get()
	{
		return device->SetpointY;
	}

	void Device::SetpointY::set(double value)
	{
		// Don't update anything if the value hasn't changed
		if (device->SetpointY == value)
			return;

		device->SetpointY = value;
		// Whenever we change the setpoint, call the Update function to update the internal callback structure
		device->UpdateSetpoint();
	}

	double Device::SetpointZ::get()
	{
		return device->SetpointZ;
	}

	void Device::SetpointZ::set(double value)
	{
		// Don't update anything if the value hasn't changed
		if (device->SetpointZ == value)
			return;

		device->SetpointZ = value;
		// Whenever we change the setpoint, call the Update function to update the internal callback structure
		device->UpdateSetpoint();
	}

	bool Device::SetpointEnabled::get()
	{
		return device->SetpointEnabled;
	}

	void Device::SetpointEnabled::set(bool value)
	{
		// Don't update anything if the value hasn't changed
		if (device->SetpointEnabled == value)
			return;

		device->SetpointEnabled = value;
		// Whenever we change the setpoint, call the Update function to update the internal callback structure
		device->UpdateSetpoint();
	}

	void Device::Update()
	{
		device->Update();
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

		if (device->InkwellSwitch == isInInkwell)
		{
			isInInkwell = !device->InkwellSwitch;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("IsInInkwell"));
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

		if (device->Button1 != button1)
		{
			button1 = device->Button1;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button1"));
		}

		if (device->Button2 != button2)
		{
			button2 = device->Button2;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button2"));
		}

		if (device->Button3 != button3)
		{
			button3 = device->Button3;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button3"));
		}

		if (device->Button4 != button4)
		{
			button4 = device->Button4;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button4"));
		}


	}

	void Device::UpdateTransform()
	{
		device->UpdateTransform();
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

		if (device->InkwellSwitch == isInInkwell)
		{
			isInInkwell = !device->InkwellSwitch;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("IsInInkwell"));
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

		if (device->Button1 != button1)
		{
			button1 = device->Button1;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button1"));
		}

		if (device->Button2 != button2)
		{
			button2 = device->Button2;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button2"));
		}

		if (device->Button3 != button3)
		{
			button3 = device->Button3;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button3"));
		}

		if (device->Button4 != button4)
		{
			button4 = device->Button4;
			this->PropertyChanged(this, gcnew PropertyChangedEventArgs("Button4"));
		}
	}
}