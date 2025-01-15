'''This file manages all the input received from serial ports and outputs'''
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
public class SerialManager : MonoBehaviour
{
    private static SerialManager _instance;
    public static SerialManager Instance => _instance;

    private SerialPort serialPort;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject); // Ensure the object persists
        }
    }

    void Start()
    {
        serialPort = new SerialPort("COM8", 9600); // COM8 for Bluetooth gloves
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    public void SendCommand(string command)
    {
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine(command);
            Debug.Log("Buzz command sent - from manager!");
        }
    }

    public string ReadFromPort()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                return serialPort.ReadLine();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error reading from port: " + e.Message);
            }
        }
        return null;
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
