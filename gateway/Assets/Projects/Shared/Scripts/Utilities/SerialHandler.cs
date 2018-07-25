using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SerialHandler 
{
	public delegate void SerialDataReceivedEventHandler(string message);
	public event SerialDataReceivedEventHandler OnDataReceived;

	public string portName = "";
	public int baudRate    = 9600;

	private SerialPort serialPort_;
	private Thread thread_;
	private bool isRunning_ = false;

	private string message_;
	private bool isNewMessageReceived_ = false;

	public bool ThreadedReading=true;
	
	public void Update()
	{
		if (isNewMessageReceived_) {
			OnDataReceived(message_);
		}
	}


	public void Open(bool readEnabled)
	{
		serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
		serialPort_.Open();

		isRunning_ = true;

        if (readEnabled)
        {
            thread_ = new Thread(Read);
            thread_.Start();
        }
	}

	public void Close()
	{
		isRunning_ = false;

		if (serialPort_ != null && serialPort_.IsOpen) {
			serialPort_.Close();
			serialPort_.Dispose();
		}
		if (thread_ != null && thread_.IsAlive) {
			thread_.Join();
		}

	}

	private void Read()
	{
		while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
			try {
				//if (serialPort_.BytesToRead > 0) {
					message_ = serialPort_.ReadLine();
					isNewMessageReceived_ = true;
				//}
			} catch (System.Exception e) {
				//Debug.LogWarning(e.Message);
			}
		}
	}

	public void Write(string message)
	{
		try {
			serialPort_.Write(message);
		} catch (System.Exception e) {
			Debug.LogWarning(e.Message);
		}
	}
	public void WriteBytes(byte[]data)
	{
		try {
			serialPort_.Write(data,0,data.Length);
		} catch (System.Exception e) {
			Debug.LogWarning(e.Message);
		}
	}
}