'''This file recieves 5 fingers input and also sends output when objects collide with tagged interactable'''
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FullFingerTest : MonoBehaviour
{
    // Transforms for the three joints of each finger
    public Transform indexProximal, indexMiddle, indexDistal; // Index finger joints
    public Transform middleProximal, middleMiddle, middleDistal; // Middle finger joints
    public Transform ringProximal, ringMiddle, ringDistal; // Ring finger joints
    public Transform pinkyProximal, pinkyMiddle, pinkyDistal; // Pinky finger joints
    public Transform thumbProximal, thumbMiddle, thumbDistal; // Thumb finger joints

    public string receivedData;

    void Start()
    {
        Debug.Log("MultipleFingerTestCustom initialized.");
    }

    void Update()
    {
        // Use SerialManagerCustom to get the sensor data
        string sensorData = SerialManager.Instance.ReadFromPort();

        if (sensorData != null)
        {
            ProcessSensorData(sensorData);
        }
    }

    // Process the sensor data and update the finger positions
    void ProcessSensorData(string data)
    {
        try
        {
            string[] dataValues = data.Split(',');

            // Ensure we have five inputs for five fingers
            if (dataValues.Length >= 5)
            {
                // Parse sensor values for each finger
                int indexSensorValue = Mathf.RoundToInt(float.Parse(dataValues[0]));
                int middleSensorValue = Mathf.RoundToInt(float.Parse(dataValues[1]));
                int ringSensorValue = Mathf.RoundToInt(float.Parse(dataValues[2]));
                int pinkySensorValue = Mathf.RoundToInt(float.Parse(dataValues[3]));
                int thumbSensorValue = Mathf.RoundToInt(float.Parse(dataValues[4]));

                // Update each finger's joint rotation based on sensor values
                ApplyFingerRotations(indexSensorValue, middleSensorValue, ringSensorValue, pinkySensorValue, thumbSensorValue);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error processing sensor data: " + e.Message);
        }
    }

    // Apply rotations to all finger joints
    void ApplyFingerRotations(int indexValue, int middleValue, int ringValue, int pinkyValue, int thumbValue)
    {
        // Index finger
        RotateFinger(indexProximal, indexMiddle, indexDistal, indexValue);

        // Middle finger
        RotateFinger(middleProximal, middleMiddle, middleDistal, middleValue);

        // Ring finger
        RotateFinger(ringProximal, ringMiddle, ringDistal, ringValue);

        // Pinky finger
        RotateFinger(pinkyProximal, pinkyMiddle, pinkyDistal, pinkyValue);

        // Thumb finger
        RotateFinger(thumbProximal, thumbMiddle, thumbDistal, thumbValue);
    }

    // Helper function to map values and apply rotation to a finger
    void RotateFinger(Transform joint1, Transform joint2, Transform joint3, int sensorValue)
    {
        float rotation = Map(sensorValue, 0, 1023, 0, 90);

        joint1.localEulerAngles = new Vector3(0, 0, -rotation * 0.5f);
        joint2.localEulerAngles = new Vector3(0, 0, -rotation);
        joint3.localEulerAngles = new Vector3(0, 0, -rotation * 1.5f);
    }

    // Mapping function to convert sensor values to rotation angles
    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return from2 + (to2 - from2) * ((value - from1) / (to1 - from1));
    }
        //On collision Enter to send specific Commands per Fingers
        private void OnCollisionEnter(Collision collision)
    {
        // Check if the object has the "Interactable" tag
        if (collision.gameObject.CompareTag("Interactable")){
            // Check which joint collided and take specific actions based on that
            if (gameObject.name.Contains("index"))
            {
                Debug.Log("Index finger collided with " + collision.gameObject.name);
                SerialManager.Instance.SendCommand("index_haptic\n");
            }
            else if (gameObject.name.Contains("middle"))
            {
                Debug.Log("Middle finger collided with " + collision.gameObject.name);
                SerialManager.Instance.SendCommand("middle_haptic\n");
            }
            else if (gameObject.name.Contains("ring"))
            {
                Debug.Log("Ring finger collided with " + collision.gameObject.name);
                SerialManager.Instance.SendCommand("ring_haptic\n");
            }
            else if (gameObject.name.Contains("pinky"))
            {
                Debug.Log("Pinky finger collided with " + collision.gameObject.name);
                SerialManager.Instance.SendCommand("pinky_haptic\n");
            }
            else if (gameObject.name.Contains("thumb"))
            {
                Debug.Log("Thumb finger collided with " + collision.gameObject.name);
                SerialManager.Instance.SendCommand("thumb_haptic\n");
            }

            // Send a general buzz command for any hand joint collision
            SerialManager.Instance.SendCommand("buzz\n");
            }
    

    }
}
