'''This file is to test sending output from unity - Without instance'''
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class testbuzz : MonoBehaviour
{
    private SerialPort serialPort;
    //public string portName = "COM3"; // Adjust based on the arduino port
    // public string portName = "COM4"; // Adjust based on the ESP32 port
    public string portName = "COM8"; // Adjust based on the ESP32 bluettoth port
    public int baudRate = 9600;

    void Start()
    {
        // Try to open the serial port
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            serialPort.WriteLine("buzz");
            Debug.Log("Serial port opened successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    void Update()
    {
        // Check if the serial port is open
        if (serialPort.IsOpen)
        {
            try
            {
                // Send a simple test message every second
                if (Time.frameCount % 60 == 0)  // Every 60 frames, send the message
                {
                    serialPort.WriteLine("buzz");
                    Debug.Log("Sent 'buzz' to ESP32");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error writing to serial port: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Ensure the serial port is closed when quitting the application
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
