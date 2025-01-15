#include "BluetoothSerial.h" 


BluetoothSerial SerialBT;

// Analog input pin for flex sensor
const int flexIndex = 34; 
const int flexMiddle = 35;
const int flexRing = 32;
const int flexPinky = 33;
const int flexThumb = 25;

#define BTN_PIN 13  // Input pin for the button
#define MOT_PIN 12  // Output pin for the motor

// Thresholds for each finger (can be tuned based on sensor range)
const int thresholdIndex = 2200;
const int thresholdMiddle = 2200;
const int thresholdRing = 2100;
const int thresholdPinky = 2000;
const int thresholdThumb = 2000;

void setup() {
  Serial.begin(9600);  // Initialize serial communication for Unity
  SerialBT.begin("ESP32_MagicMitts");  // Bluetooth device name
  pinMode(BTN_PIN, INPUT);
  pinMode(MOT_PIN, OUTPUT);  // Set actuator pin as output
  digitalWrite(MOT_PIN, LOW);  // motor is off initially

  Serial.println("System Ready. Sending data in CSV format.");
  SerialBT.println("ESP32 Glove Bluetooth Started.");
}

void loop() {

  // Read sensor values from all flex sensors
  int flexValue1 = analogRead(flexIndex);  // Index finger
  int flexValue2 = analogRead(flexMiddle);  // Middle finger
  int flexValue3 = analogRead(flexRing);    // Ring finger
  int flexValue4 = analogRead(flexPinky);   // Pinky finger
  int flexValue5 = analogRead(flexThumb);   // Thumb

  // Send values over serial as CSV (for Unity)
  Serial.print(flexValue1); Serial.print(",");
  Serial.print(flexValue2); Serial.print(",");
  Serial.print(flexValue3); Serial.print(",");
  Serial.print(flexValue4); Serial.print(",");
  Serial.println(flexValue5);  // Print newline at the end

    // Send values over Bluetooth Serial
  SerialBT.print(flexValue1); SerialBT.print(",");
  SerialBT.print(flexValue2); SerialBT.print(",");
  SerialBT.print(flexValue3); SerialBT.print(",");
  SerialBT.print(flexValue4); SerialBT.print(",");
  SerialBT.println(flexValue5);

  // Check if the button is pressed
  if (digitalRead(BTN_PIN) == HIGH) {
    activateMotor();  
  }

  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');
    if (command == "buzz") {  // add to haptic buzz 
      activateMotor(); 
    }
  }
  if (SerialBT.available() > 0) {
    String command = SerialBT.readStringUntil('\n');
    if (command == "buzz") {  // add to haptic buzz 
      activateMotor(); 
    }
  }

  delay(10);  // Stability delay
}

// Function to turn on the haptic motor for feedback
void activateMotor() {
  digitalWrite(MOT_PIN, HIGH);  
  delay(100);                   
  digitalWrite(MOT_PIN, LOW);  
  // delay(300); 
}
