// This is the main DLL file.
#define _USE_MATH_DEFINES
#include <cmath>
#include "stdafx.h"
#include "NativeDevice.h"

NativeDevice::NativeDevice()
{

}

NativeDevice::NativeDevice(char DeviceIndex)
{
	DeviceID = DeviceIndex;
	deviceInitialized = false;

	// open the first available device
	if (dhdOpenID(DeviceID) < 0) 
	{
		printf("error: cannot open device (%s)\n", dhdErrorGetLastStr());
		dhdSleep(2.0);
		return;
	}
	else
		deviceInitialized = true;

	dhdEnableExpertMode();

	brakesOn = false;
	dhdGetSerialNumber(&SerialNum, DeviceID);
	IsLeft = dhdIsLeftHanded(DeviceID);
	dhdClose(DeviceID);
	//dhdEnableForce(DHD_ON, DeviceID);
	//dhdSetForceAndTorqueAndGripperForce(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
	//dhdSetGravityCompensation(DHD_ON, DeviceID);

	switch (dhdGetSystemType()) {
	case DHD_DEVICE_DELTA3:
	case DHD_DEVICE_OMEGA3:
	case DHD_DEVICE_FALCON:
		encCount = 3;
		break;
	case DHD_DEVICE_DELTA6:
	case DHD_DEVICE_OMEGA33:
	case DHD_DEVICE_OMEGA33_LEFT:
		encCount = 6;
		break;
	case DHD_DEVICE_OMEGA331:
	case DHD_DEVICE_OMEGA331_LEFT:
		encCount = 7;
		break;
	case DHD_DEVICE_CONTROLLER:
	case DHD_DEVICE_CONTROLLER_HR:
	default:
		encCount = 8;
		break;
	}
}

void NativeDevice::Start()
{
	dhdOpenID(DeviceID);
	dhdEnableForce(DHD_ON, DeviceID);
	dhdSetForceAndTorqueAndGripperForce(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
	ForceEnabled = true;
}

void NativeDevice::Stop()
{
	dhdClose(DeviceID);
}

void NativeDevice::Brake()
{
	if (brakesOn)
		dhdSetBrakes(DHD_OFF, DeviceID);
	else
		dhdSetBrakes(DHD_ON, DeviceID);
}

void NativeDevice::UpdateDevice()
{
	if (dhdGetPosition(&X, &Y, &Z, DeviceID) < DHD_NO_ERROR)
		printf("error: cannot read device\n");
	if (dhdGetOrientationFrame(rotM, DeviceID) < DHD_NO_ERROR)
		printf("error: cannot read device\n");
	if (dhdGetGripperAngleDeg(&GripperDeg, DeviceID) < DHD_NO_ERROR)
		printf("error: cannot read device");

	R00 = rotM[0][0];
	R01 = rotM[0][1];
	R02 = rotM[0][2];
	R10 = rotM[1][0];
	R11 = rotM[1][1];
	R12 = rotM[1][2];
	R20 = rotM[2][0];
	R21 = rotM[2][1];
	R22 = rotM[2][2];

	double sy = sqrt(rotM[0][0] * rotM[0][0] + rotM[1][0] * rotM[1][0]);

	bool singular = sy < 1e-6;

	if (!singular)
	{
		Theta1 = atan2(rotM[2][1], rotM[2][2]) * 180 / M_PI;
		Theta2 = atan2(-rotM[2][0], sy) * 180 / M_PI;
		Theta3 = atan2(rotM[1][0], rotM[0][0]) * 180 / M_PI;
	}
	else
	{
		Theta1 = atan2(-rotM[1][2], rotM[1][1]) * 180 / M_PI;
		Theta2 = atan2(-rotM[2][0], sy) * 180 / M_PI;
		Theta3 = 0;
	}
}

void NativeDevice::UpdateForces(double fx, double fy, double fz)
{
	//if (ForceEnabled)
	//{
		dhdSetForce(fx, fy, fz, DeviceID);
	//}
}

int NativeDevice::GetDeviceCount()
{
	return dhdGetDeviceCount();
}

NativeDevice::~NativeDevice()
{
	dhdClose(DeviceID);
}