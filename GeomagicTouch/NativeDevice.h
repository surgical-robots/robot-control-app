#pragma once
#include <string>
#include <stdarg.h>
#include <HD/hd.h>
#include <HL\hl.h>
#include <gl\GL.h>
#include <stdint.h>
#include <math.h>
#define _USE_MATH_DEFINES

using namespace std;

struct GetUpdateCallbackData
{
	HHD Handle;
	HDdouble Position[3];
	HDdouble GimbalAngles[3];
	HDdouble Transform[16];
	HDint Buttons;
	HDboolean InkwellSwitch;
	HDdouble Setpoint[3];
	HDboolean ForceEnabled;
};

class NativeDevice
{
public:
	NativeDevice(string DeviceName);
	void Start();
	void Stop();
	void Update();
	void UpdateTransform();
	void UpdateSetpoint();
	void LoadObj(string ObjectPath);
	~NativeDevice();

	bool deviceInitialized;

	double SetpointX;
	double SetpointY;
	double SetpointZ;

	bool SetpointEnabled;

	double X;
	double Y;
	double Z;
	double Theta1;
	double Theta2;
	double Theta3;
	bool Button1;
	bool Button2;
	bool Button3;
	bool Button4;
	bool InkwellSwitch;

	uint8_t Buttons;
private:
	HHD handle;
	HHLRC hapticHandle;
	string name;
	bool servoLoopRegistered = false;
	static HDSchedulerHandle schedulerHandle;
	static int schedulerClients;
	static HDCallbackCode HDCALLBACK GetUpdateCallback(void *);
	GetUpdateCallbackData* CallbackDataObject;
	HDSchedulerHandle servoLoopHandle;
};

