// NativeDevice.h

#pragma once

#define _USE_MATH_DEFINES
#include <stdarg.h>
#include <stdint.h>
#include <string>
#include <math.h>
#include <cmath>
#include "dhdc.h"

using namespace System;
using namespace System::ComponentModel;

class NativeDevice
{
public:
	NativeDevice();
	NativeDevice(char DeviceIndex);
	void UpdateDevice();
	void UpdateForces(double fx, double fy, double fz);
	int GetDeviceCount();
	~NativeDevice();

	bool deviceInitialized;

	bool ForceEnabled;

	double X;
	double Y;
	double Z;
	double GripperDeg;
	double Theta1;
	double Theta2;
	double Theta3;
	double R00;
	double R01;
	double R02;
	double R10;
	double R11;
	double R12;
	double R20;
	double R21;
	double R22;
	char DeviceID;
	ushort SerialNum;
	bool IsLeft;
	int DeviceCount;
	int encCount;

private:
	static double x;
	static double y;
	static double z;
	static double theta1;
	static double theta2;
	static double theta3;
	double rotM[3][3];
	static double gripperDeg;
	static double forceX;
	static double forceY;
	static double forceZ;
	static char deviceID;
	static int deviceCount;
};
