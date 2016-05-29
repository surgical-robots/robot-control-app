#include "stdafx.h"
#include "NativeDevice.h"
#include <Windows.h>
#include <HD/hd.h>
#include <HL\hl.h>
#include <gl\GL.h>
#include <stdio.h>
#include <string>
#include <vector>
#include <glm\glm.hpp>
#include <math.h>

using namespace std;

int NativeDevice::schedulerClients = 0;

NativeDevice::NativeDevice(string DeviceName)
{
	name = DeviceName;
	deviceInitialized = false;
	while(HD_DEVICE_ERROR(hdGetError()));
	handle = hdInitDevice(name.c_str());
	servoLoopHandle = NULL;
	HDErrorInfo error;
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		HDstring stringError = hdGetErrorString(error.errorCode);
		printf("Error in Initialization");
	}
	else {
		deviceInitialized = true;
		CallbackDataObject = new GetUpdateCallbackData();
		CallbackDataObject->Handle = this->handle;
	}

	hdEnable(HD_FORCE_OUTPUT);
	hdEnable(HD_FORCE_RAMPING);
	hdEnable(HD_MAX_FORCE_CLAMPING);
	SetpointEnabled = false;
}

void NativeDevice::Start()
{
	if (!deviceInitialized)
		return;

	servoLoopHandle = hdScheduleAsynchronous(GetUpdateCallback, CallbackDataObject, HD_DEFAULT_SCHEDULER_PRIORITY);
	servoLoopRegistered = true;


	// we're the first one here! start the scheduler
	if (schedulerClients == 0) 
	{
		hdSetSchedulerRate(500);
		HDErrorInfo error;
		if (HD_DEVICE_ERROR(error = hdGetError()))
		{
			HDstring stringError = hdGetErrorString(error.errorCode);
			printf("Error in Initialization");
		}

		hdStartScheduler();
		if (HD_DEVICE_ERROR(error = hdGetError()))
		{
			HDstring stringError = hdGetErrorString(error.errorCode);
			printf("Error in Initialization");
		}
	}
	schedulerClients++;

	//this->hapticHandle = hlCreateContext(this->handle);
	//hlMakeCurrent(this->hapticHandle);

	//HLuint viscosity = hlGenEffects(1);
	//hlBeginFrame();
	//hlEffectd(HL_EFFECT_PROPERTY_GAIN, 0.7);
	//hlEffectd(HL_EFFECT_PROPERTY_MAGNITUDE, 2);
	//hlStartEffect(HL_EFFECT_VISCOUS, viscosity);
	//hlEndFrame();
}

/// This is the synchronous callback that's used to get the current position/angles/buttons
/// that we'll push out of the class.
///
/// Remarks: This is a static function that's "shared" by any instance of this.
/// DO NOT attempt to read/write any instance variables directly! Instead, add them to
/// the CallbackData struct, which will get passed to this function.
HDCallbackCode HDCALLBACK NativeDevice::GetUpdateCallback(void *pUserData)
{
	GetUpdateCallbackData *Data = static_cast<GetUpdateCallbackData *>(pUserData);
	hdBeginFrame(Data->Handle);
	HDErrorInfo error;
	while (HD_DEVICE_ERROR(error = hdGetError()))
	{
		HDstring stringError = hdGetErrorString(error.errorCode);
		printf("Error in Initialization");
	}
	hdMakeCurrentDevice(Data->Handle);

	double oldX = Data->Position[0];
	double oldY = Data->Position[1];
	double oldZ = Data->Position[2];

	while (HD_DEVICE_ERROR(error = hdGetError()))
	{
		HDstring stringError = hdGetErrorString(error.errorCode);
		printf("Error in Initialization");
	}
	hdGetDoublev(HD_CURRENT_POSITION, Data->Position);
	hdGetDoublev(HD_CURRENT_GIMBAL_ANGLES, Data->GimbalAngles);
	hdGetDoublev(HD_CURRENT_TRANSFORM, Data->Transform);
	hdGetBooleanv(HD_CURRENT_INKWELL_SWITCH, &Data->InkwellSwitch);
	hdGetIntegerv(HD_CURRENT_BUTTONS, &Data->Buttons);

	if (Data->ForceEnabled)
	{
		double kd = 0.6;
	    // calculate force setpoints from Data
		double xSetpoint = Data->Setpoint[0];
		double ySetpoint = Data->Setpoint[1];
		double zSetpoint = Data->Setpoint[2];

		double forceX = xSetpoint - ( Data->Position[0] - oldX ) * kd;
		double forceY = ySetpoint - ( Data->Position[1] - oldY ) * kd;
		double forceZ = zSetpoint - ( Data->Position[2] - oldZ ) * kd;

		double forces[3] = { forceX, forceY, forceZ };
		// ...
		//hdSetDoublev(HD_CURRENT_FORCE, Data->Setpoint);
		hdSetDoublev(HD_CURRENT_FORCE, forces);
	}
	else
	{
		double noForce[3] = { 0, 0, 0 };
		hdSetDoublev(HD_CURRENT_FORCE, noForce);
	}
		
	hdEndFrame(Data->Handle);

	while (HD_DEVICE_ERROR(error = hdGetError()))
	{
		HDstring stringError = hdGetErrorString(error.errorCode);
		printf("Error in Initialization");
	}

	return HD_CALLBACK_CONTINUE;
}

void NativeDevice::UpdateSetpoint()
{
	CallbackDataObject->Setpoint[0] = SetpointX;
	CallbackDataObject->Setpoint[1] = SetpointY;
	CallbackDataObject->Setpoint[2] = SetpointZ;

	CallbackDataObject->ForceEnabled = SetpointEnabled;
}

void NativeDevice::Update()
{
	if (!deviceInitialized) return;
	if (!servoLoopRegistered) return;
	X = CallbackDataObject->Position[0];
	Y = CallbackDataObject->Position[1];
	Z = CallbackDataObject->Position[2];
	Theta1 = CallbackDataObject->GimbalAngles[0];
	Theta2 = CallbackDataObject->GimbalAngles[1];
	Theta3 = CallbackDataObject->GimbalAngles[2];
	Button1 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_1;
	Button2 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_2;
	Button3 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_3;
	Button4 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_4;
	InkwellSwitch = CallbackDataObject->InkwellSwitch;
}

void NativeDevice::UpdateTransform()
{
	if (!deviceInitialized) return;
	if (!servoLoopRegistered) return;
	HDdouble theta1, theta2, chi1, chi2, phi1, phi2;
	X = CallbackDataObject->Transform[12];
	Y = CallbackDataObject->Transform[13];
	Z = CallbackDataObject->Transform[14];

	//if (abs(CallbackDataObject->Transform[2]) != 1)
	//{
	//	theta1 = -asin(CallbackDataObject->Transform[2]);
	//	theta2 = M_PI - theta1;
	//	chi1 = atan2(CallbackDataObject->Transform[6] / cos(theta1), CallbackDataObject->Transform[10] / cos(theta1));
	//	chi2 = atan2(CallbackDataObject->Transform[6] / cos(theta2), CallbackDataObject->Transform[10] / cos(theta2));
	//	phi1 = atan2(CallbackDataObject->Transform[1] / cos(theta1), CallbackDataObject->Transform[0] / cos(theta1));
	//	phi2 = atan2(CallbackDataObject->Transform[1] / cos(theta2), CallbackDataObject->Transform[0] / cos(theta2));

	//	Theta1 = theta1;
	//	Theta2 = chi1;
	//	Theta3 = phi1;
	//}
	//else
	//{
	//	Theta3 = 0;
	//	if (CallbackDataObject->Transform[2] == -1)
	//	{
	//		Theta1 = M_PI_2;
	//		Theta2 = atan2(CallbackDataObject->Transform[4], CallbackDataObject->Transform[8]);
	//	}
	//	else
	//	{
	//		Theta1 = -M_PI_2;
	//		Theta2 = atan2(-CallbackDataObject->Transform[4], -CallbackDataObject->Transform[8]);
	//	}
	//}

	//Theta1 = atan2(CallbackDataObject->Transform[6], CallbackDataObject->Transform[10]);
	//Theta2 = atan2(-CallbackDataObject->Transform[2], sqrt(pow(CallbackDataObject->Transform[6], 2) + pow(CallbackDataObject->Transform[10], 2)));
	//Theta3 = atan2(CallbackDataObject->Transform[1], CallbackDataObject->Transform[0]);

	Theta1 = atan2(CallbackDataObject->Transform[9], CallbackDataObject->Transform[10]);
	Theta2 = atan2(-CallbackDataObject->Transform[8], sqrt(pow(CallbackDataObject->Transform[0], 2) + pow(CallbackDataObject->Transform[4], 2)));
	Theta3 = atan2(sin(Theta1)*CallbackDataObject->Transform[2] - cos(Theta1)*CallbackDataObject->Transform[1], cos(Theta1)*CallbackDataObject->Transform[5] - sin(Theta1)*CallbackDataObject->Transform[6]);

	Button1 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_1;
	Button2 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_2;
	Button3 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_3;
	Button4 = CallbackDataObject->Buttons & HD_DEVICE_BUTTON_4;
	InkwellSwitch = CallbackDataObject->InkwellSwitch;
}

void NativeDevice::Stop()
{
	if (servoLoopHandle != NULL)
		hdUnschedule(servoLoopHandle);

	schedulerClients--;
	if (schedulerClients == 0) // we're the last one here.
		hdStopScheduler();
	
	hdDisableDevice(this->handle);
}

void NativeDevice::LoadObj(string ObjectPath)
{
	std::vector< unsigned int > vertexIndices, normalIndices;
	std::vector< glm::vec3 > temp_vertices;
	std::vector< glm::vec3 > temp_normals;
	std::vector< glm::vec3 > out_vertices;

	hlTouchableFace(HL_FRONT);
	hlTouchModel(HL_CONTACT);

	const char * filePath = ObjectPath.c_str();

	FILE * file = fopen(filePath, "r");

	while (1){

		char lineHeader[128];
		// read the first word of the line
		int res = fscanf(file, "%s", lineHeader);
		if (res == EOF)
			break; // EOF = End Of File. Quit the loop.
		if (strcmp(lineHeader, "v") == 0)
		{
			glm::vec3 vertex;
			fscanf(file, "%f %f %f\n", &vertex.x, &vertex.y, &vertex.z);
			temp_vertices.push_back(vertex);
		}
		else if (strcmp(lineHeader, "f") == 0)
		{
			std::string vertex1, vertex2, vertex3;
			unsigned int vertexIndex[3], uvIndex[3], normalIndex[3];
			int matches = fscanf(file, "%d//%d %d//%d %d//%d\n", &vertexIndex[0], &normalIndex[0], &vertexIndex[1], &normalIndex[1], &vertexIndex[2], &normalIndex[2]);
			vertexIndices.push_back(vertexIndex[0]);
			vertexIndices.push_back(vertexIndex[1]);
			vertexIndices.push_back(vertexIndex[2]);
			normalIndices.push_back(normalIndex[0]);
			normalIndices.push_back(normalIndex[1]);
			normalIndices.push_back(normalIndex[2]);
		}
	}

	for (unsigned int i = 0; i < vertexIndices.size(); i++)
	{
		unsigned int vertexIndex = vertexIndices[i];
		glm::vec3 vertex = temp_vertices[vertexIndex - 1];
		out_vertices.push_back(vertex);
	}

	int VertexNum = out_vertices.size();
	int FaceNum = vertexIndices.size();

	hlBeginFrame();
	HLuint object = hlGenShapes(1);
	hlHinti(HL_SHAPE_FEEDBACK_BUFFER_VERTICES, VertexNum);
	hlBeginShape(HL_SHAPE_FEEDBACK_BUFFER, object);
	glBegin(GL_TRIANGLES);
	// insert triangles here
	for (int i = 0; i < FaceNum; i++)
	{
		glVertex3f(out_vertices[vertexIndices[i]].x, out_vertices[vertexIndices[i]].y, out_vertices[vertexIndices[i]].z);
	}
	glEnd();
	hlEndShape();
	hlEndFrame();
}

NativeDevice::~NativeDevice()
{

}
