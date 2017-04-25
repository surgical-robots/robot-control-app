// NativeDevice.h

#pragma once

#define _USE_MATH_DEFINES
#include <stdarg.h>
#include <stdint.h>
#include <string>
#include <math.h>
#include <cmath>
#include "sixense.h"
#include <sixense_math.hpp>
#ifdef WIN32
#include <sixense_utils/mouse_pointer.hpp>
#endif
#include <sixense_utils/derivatives.hpp>
#include <sixense_utils/button_states.hpp>
#include <sixense_utils/event_triggers.hpp>
#include <sixense_utils/controller_manager/controller_manager.hpp>

using namespace System;
using namespace System::ComponentModel;

class NativeDevice
{
public:
	NativeDevice();
	NativeDevice(char DeviceIndex);
	void Start();
	void Stop();
	void UpdateDevice();
	//static void controller_manager_setup_callback(sixenseUtils::ControllerManager::setup_step step);
	int GetDeviceCount();
	~NativeDevice();

	bool deviceInitialized;

	sixenseAllControllerData controllerData;
	// left controller
	double X_L;
	double Y_L;
	double Z_L;
	double GripperDeg_L;
	double Theta1_L;
	double Theta2_L;
	double Theta3_L;
	double R00_L;
	double R01_L;
	double R02_L;
	double R10_L;
	double R11_L;
	double R12_L;
	double R20_L;
	double R21_L;
	double R22_L;
	double JoystickX_L;
	double JoystickY_L;
	double Bumper_L;
	double Button1_L;
	double Button2_L;
	double Button3_L;
	double Button4_L;
	// right controller
	double X_R;
	double Y_R;
	double Z_R;
	double GripperDeg_R;
	double Theta1_R;
	double Theta2_R;
	double Theta3_R;
	double R00_R;
	double R01_R;
	double R02_R;
	double R10_R;
	double R11_R;
	double R12_R;
	double R20_R;
	double R21_R;
	double R22_R;
	double JoystickX_R;
	double JoystickY_R;
	double Bumper_R;
	double Button1_R;
	double Button2_R;
	double Button3_R;
	double Button4_R;

	bool IsLeft;
	int DeviceCount;
	int encCount;
	static std::string controller_manager_text_string;
	static bool showInstructions;

private:
	static double x;
	static double y;
	static double z;
	static double theta1;
	static double theta2;
	static double theta3;
	float rotM[3][3];
	static double gripperDeg;
	static char deviceID;
	static int deviceCount;
	bool brakesOn;
};
