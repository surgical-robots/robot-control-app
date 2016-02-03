#pragma once
#using <System.dll>

using namespace System;
using namespace System::Configuration;
using namespace RobotControl;
using namespace System::IO::Ports;
using namespace System::Threading;

namespace RobotControl {

public ref class SerialPortPacketTransport :
public IPacketTransport
{
public:
	 SerialPortPacketTransport();
	 SerialPortPacketTransport(String^ comPort, Robot^ robotIn);
	 SerialPort^ Port = gcnew SerialPort();
	 virtual property bool IsOpen { bool get() sealed { return Port->IsOpen; } void set(bool val) { } }
	 virtual property bool RequestData { bool get() sealed{ return requestData; } void set(bool val) { requestData = val; }};
	 virtual property bool SendData { bool get() sealed{ return sendData; } void set(bool val) { sendData = val; }};
	 virtual event IPacketTransportDataReceived^ DataReceived;
	 virtual event IPacketTransportUpdateSetpoints^ UpdateSetpoints;
	 virtual void Send(array<unsigned char, 1>^ data);
	 virtual void SerialOps();
	 static array<unsigned int, 1>^ Addresses = gcnew array<unsigned int, 1>(10);
	 virtual void AddAddress(unsigned int address);
	 virtual void RemoveAddress(unsigned int address);
	 static Object^ lockObject = gcnew Object;
//	 virtual property long TimeoutCount { long get() sealed { return timeoutCount; } void set(long val) sealed { } }
	 Thread^ serialOpsThread;
	 Robot^ robot;
private:
	static array<unsigned char, 1>^ TxBuffer = gcnew array<unsigned char, 1>(32);
	static array<unsigned char, 1>^ RxBuffer = gcnew array<unsigned char, 1>(32);
	static SerialPortPacketTransport^ sppt;
	static int controllerCount = 0;
	static bool sending = false;
	static bool requestData = false;
	static bool sendData = false;
	static bool waitingForResponse = false;
//	static long timeoutCount = 0;
};

}

