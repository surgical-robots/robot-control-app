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

	// Init sixense
	if (sixenseInit() == SIXENSE_SUCCESS)
	{
		if (sixenseGetNumActiveControllers > 0)
		{
			deviceInitialized = true;
			//sixenseSetActiveBase(0);
			//sixenseUtils::getTheControllerManager()->setGameType(sixenseUtils::ControllerManager::ONE_PLAYER_TWO_CONTROLLER);
			//sixenseUtils::getTheControllerManager()->registerSetupCallback(controller_manager_setup_callback);
		}
	}

}

void NativeDevice::Start()
{
	if (deviceInitialized)
	{
		sixenseUtils::getTheControllerManager()->setGameType(sixenseUtils::ControllerManager::ONE_PLAYER_TWO_CONTROLLER);
		//sixenseUtils::getTheControllerManager()->registerSetupCallback(controller_manager_setup_callback);
	}
}

void NativeDevice::Stop()
{
	sixenseExit();
}

void NativeDevice::UpdateDevice()
{
	if (sixenseGetAllNewestData(&controllerData) == SIXENSE_FAILURE)
		return;

	sixenseControllerData LeftController = controllerData.controllers[0];
	sixenseControllerData RightController = controllerData.controllers[1];

	// Left Controller Data
	X_L = LeftController.pos[0];
	Y_L = LeftController.pos[1];
	Z_L = LeftController.pos[2];

	R00_L = LeftController.rot_mat[0][0];
	R01_L = LeftController.rot_mat[0][1];
	R02_L = LeftController.rot_mat[0][2];
	R10_L = LeftController.rot_mat[1][0];
	R11_L = LeftController.rot_mat[1][1];
	R12_L = LeftController.rot_mat[1][2];
	R20_L = LeftController.rot_mat[2][0];
	R21_L = LeftController.rot_mat[2][1];
	R22_L = LeftController.rot_mat[2][2];

	double sy = sqrt(R00_L * R00_L + R10_L * R10_L);

	bool singular = sy < 1e-6;

	if (!singular)
	{
		Theta1_L = atan2(R21_L, R22_L) * 180 / M_PI;
		Theta2_L = atan2(-R20_L, sy) * 180 / M_PI;
		Theta3_L = atan2(R10_L, R00_L) * 180 / M_PI;
	}
	else
	{
		Theta1_L = atan2(-R12_L, R11_L) * 180 / M_PI;
		Theta2_L = atan2(-R20_L, sy) * 180 / M_PI;
		Theta3_L = 0;
	}

	// Right Controller Data
	X_R = RightController.pos[0];
	Y_R = RightController.pos[1];
	Z_R = RightController.pos[2];

	R00_R = RightController.rot_mat[0][0];
	R01_R = RightController.rot_mat[0][1];
	R02_R = RightController.rot_mat[0][2];
	R10_R = RightController.rot_mat[1][0];
	R11_R = RightController.rot_mat[1][1];
	R12_R = RightController.rot_mat[1][2];
	R20_R = RightController.rot_mat[2][0];
	R21_R = RightController.rot_mat[2][1];
	R22_R = RightController.rot_mat[2][2];

	sy = sqrt(R00_L * R00_L + R10_L * R10_L);

	singular = sy < 1e-6;

	if (!singular)
	{
		Theta1_R = atan2(R21_R, R22_R) * 180 / M_PI;
		Theta2_R = atan2(-R20_R, sy) * 180 / M_PI;
		Theta3_R = atan2(R10_R, R00_R) * 180 / M_PI;
	}
	else
	{
		Theta1_R = atan2(-R12_R, R11_R) * 180 / M_PI;
		Theta2_R = atan2(-R20_R, sy) * 180 / M_PI;
		Theta3_R = 0;
	}
}

int NativeDevice::GetDeviceCount()
{
	return sixenseGetNumActiveControllers();
}

//void NativeDevice::controller_manager_setup_callback(sixenseUtils::ControllerManager::setup_step step) {
//
//	if (sixenseUtils::getTheControllerManager()->isMenuVisible()) {
//
//		// Turn on the flag that tells the graphics system to draw the instruction screen instead of the controller information. The game
//		// should be paused at this time.
//		NativeDevice::showInstructions = true;
//
//		// Ask the controller manager what the next instruction string should be.
//		NativeDevice::controller_manager_text_string = sixenseUtils::getTheControllerManager()->getStepString();
//
//		// We could also load the supplied controllermanager textures using the filename: sixenseUtils::getTheControllerManager()->getTextureFileName();
//
//	}
//	else {
//
//		// We're done with the setup, so hide the instruction screen.
//		NativeDevice::showInstructions = false;
//
//	}
//
//}

NativeDevice::~NativeDevice()
{
	Stop();
}