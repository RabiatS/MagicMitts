#include "BluetoothSerial.h"

BluetoothSerial SerialBT;

const int buttonPin = 13;  // GPIO 13 for the button
bool lastButtonState = LOW;  // To track the previous button state

void setup() {
  SerialBT.begin("ESP32test");  // Initialize Bluetooth with a name
  pinMode(buttonPin, INPUT_PULLUP);  // Set GPIO 13 as input with pull-up resistor
  delay(1000);
}

void loop() {
  // Check for button press
  bool buttonState = digitalRead(buttonPin);
  
  // If button state changes from LOW to HIGH, send a Bluetooth message
  if (buttonState == HIGH && lastButtonState == LOW) {
    SerialBT.println("Button Pressed!");
    delay(50); 
  }
  
  lastButtonState = buttonState;  // Update the last button state

  // Check for incoming Bluetooth data
  if (SerialBT.available()) {
    String inputFromOtherSide = SerialBT.readString();
    SerialBT.println("Entered in command line: ");
    SerialBT.println(inputFromOtherSide);
  }
}
