// SerialPortPacketTransport.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "SerialPortPacketTransport.h"
#include <msclr\lock.h>
#include <cmath>

SerialPortPacketTransport::SerialPortPacketTransport()
{

}

SerialPortPacketTransport::SerialPortPacketTransport(String^ comPort, Robot^ robotIn)
{
	sppt = this;
	bool wasOpen;
	robot = robotIn;
	SendData = true;

	// Serial sync word is DABAD000
	TxBuffer[0] = 0xDA;
	TxBuffer[1] = 0xBA;
	TxBuffer[2] = 0xD0;
	TxBuffer[3] = 0x00;

	// if setting to same port, do nothing
	if (Port->PortName == comPort)
		return;
	// if port is already open, close port
	if (IsOpen)
	{
		Port->Close();
		serialOpsThread->Suspend();
		wasOpen = true;
	}
	Port->PortName = comPort;
	Port->BaudRate = 230400;
	Port->Parity = Parity::None;
	Port->DataBits = 8;
	Port->StopBits = StopBits::One;
	Port->ReadTimeout = 25;
	Port->WriteTimeout = 25;
	Port->Open();
	serialOpsThread = gcnew Thread(gcnew ThreadStart(this, &SerialPortPacketTransport::SerialOps));
	serialOpsThread->Name = "SerialThread";
	serialOpsThread->IsBackground = true;
	serialOpsThread->Start();
}

void SerialPortPacketTransport::Send(array<unsigned char, 1>^ data)
{
	// check again before we actually send
	if (IsOpen)
	{
		sending = true;

		// Make sure that the message is the right length
		if (data->Length > 28)
		{
			throw gcnew ArgumentException("Data length exceeds 28-byte allowed size");
			for (int i = 0; i < 28; i++)
			{
				TxBuffer[i + 4] = data[i];
			}
		}
		else
		{
			for (int i = 0; i < data->Length; i++)
			{
				TxBuffer[i + 4] = data[i];
			}
		}
		// aquire com lock
		msclr::lock lock1(lockObject);
		try { Port->Write(TxBuffer, 0, (data[0] + 4)); }
		catch (TimeoutException ^) {}
		sending = false;
		lock1.release();
		// release com lock
	}
}

void SerialPortPacketTransport::AddAddress(unsigned int address)
{
	Addresses[controllerCount] = address;
	controllerCount++;
//	controllerCount = robot->Controllers->Values->Count;
}

void SerialPortPacketTransport::RemoveAddress(unsigned int address)
{
	int removeNum;
	bool found = false;

	for (int i = 0; i < controllerCount; i++)
	{
		if (Addresses[i] == address)
		{
			removeNum = i;
			found = true;
			break;
		}
	}
	
	if (found)
	{
		for (int j = removeNum; j < controllerCount; j++)
		{
			Addresses[j] = Addresses[j + 1];
		}
		controllerCount--;
	}
}

void SerialPortPacketTransport::SerialOps()
{
	double setPoint;
	int positionOne;
	int positionTwo;
	int position;
	int byteLimit;
	array<Byte, 1>^ address = gcnew array<Byte, 1>(4);
	array<Byte, 1>^ sendMsg = gcnew array<Byte, 1>(32);
	// Serial sync word is DABAD000
	sendMsg[0] = 0xDA;
	sendMsg[1] = 0xBA;
	sendMsg[2] = 0xD0;
	sendMsg[3] = 0x00;

	msclr::lock lock2(lockObject, 50);
	lock2.release();
	int bytesToRead;
	unsigned int timeout;
	
	while (true)
	{
		if (!SendData && (robot->SerialControllers->Values->Count != NULL))
		{
			for each (Controller^ controller in robot->SerialControllers->Values)
			{
				byteLimit = 4;
				if (controller->GetHalls) byteLimit += 8;
				if (controller->GetPots) byteLimit += 4;
				if (controller->GetCurrent) byteLimit += 4;

				if (controller->SendNum != 0)
				{
					address = BitConverter::GetBytes(controller->Id);
					//setPoint = motor->EncoderClicksPerRevolution / (360) * (motor->Angle - motor->OffsetAngle);
					positionOne = round(controller->Motor1Setpoint);
					positionTwo = round(controller->Motor2Setpoint);
					sendMsg[4] = (Byte)14;
					for (int j = 0; j < 4; j++)
					{
						sendMsg[j + 5] = address[j];
					}
					if (requestData && (controller->GetHalls || controller->GetPots || controller->GetCurrent))
						sendMsg[9] = (Byte)JointCommands::SetPosGetData;
					else
						sendMsg[9] = (Byte)JointCommands::DoubleMoveTo;
					Array::Copy(BitConverter::GetBytes(positionOne), 0, sendMsg, 10, 4);
					Array::Copy(BitConverter::GetBytes(positionTwo), 0, sendMsg, 14, 4);
					lock2.acquire();
					try { Port->Write(sendMsg, 0, (sendMsg[4] + 4)); }
					catch (TimeoutException ^) {}
					if (requestData && (controller->GetHalls || controller->GetPots || controller->GetCurrent))
					{
						waitingForResponse = true;
						timeout = 100000;
						// wait for data
						while ((Port->BytesToRead < byteLimit) && (--timeout > 0));
						while (Port->BytesToRead >= byteLimit)
						{
							Port->Read(RxBuffer, 0, 1);
							if (RxBuffer[0] == 200)
							{
								bytesToRead = Port->BytesToRead > (RxBuffer->Length - 1) ? (RxBuffer->Length - 1) : Port->BytesToRead;
								Port->Read(RxBuffer, 1, bytesToRead);
								DataReceived(RxBuffer);
								waitingForResponse = false;
								break;
							}
						}
					}
					lock2.release();
				}
				else if (requestData && (controller->GetHalls || controller->GetPots || controller->GetCurrent))
				{
					address = BitConverter::GetBytes(controller->Id);
					// message length after DABADOOO
					sendMsg[4] = (Byte)14;
					for (int j = 0; j < 4; j++)
					{
						sendMsg[j + 5] = address[j];
					}
					sendMsg[9] = (Byte)JointCommands::GetStatus;
					// Aquire com lock
					lock2.acquire();
					try { Port->Write(sendMsg, 0, (sendMsg[4] + 4)); }
					catch (TimeoutException ^) {}
					waitingForResponse = true;
					timeout = 100000;
					// wait for data
					while ((Port->BytesToRead < byteLimit) && (--timeout > 0));
					while (Port->BytesToRead >= byteLimit)
					{
						Port->Read(RxBuffer, 0, 1);
						if (RxBuffer[0] == 200)
						{
							bytesToRead = Port->BytesToRead > (RxBuffer->Length - 1) ? (RxBuffer->Length - 1) : Port->BytesToRead;
							Port->Read(RxBuffer, 1, bytesToRead);
							DataReceived(RxBuffer);
							waitingForResponse = false;
							break;
						}
					}
					// release com lock
					lock2.release();
				}
			}
			SendData = true;
			UpdateSetpoints();
		}
		if (Port->BytesToRead > 10)
		{
			while (Port->BytesToRead > 0)
			{
				Port->Read(RxBuffer, 0, 1);
				if (RxBuffer[0] == 200)
				{
					bytesToRead = Port->BytesToRead > (RxBuffer->Length - 1) ? (RxBuffer->Length - 1) : Port->BytesToRead;
					Port->Read(RxBuffer, 1, bytesToRead);
					DataReceived(RxBuffer);
					waitingForResponse = false;
				}
			}
		}
	}
}
