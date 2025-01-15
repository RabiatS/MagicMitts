//This is a future file for when we receive the PCB and the full glove is installed
#include "BluetoothSerial.h"


BluetoothSerial SerialBT;

// Pin Definitions from Schematic
// Flex Sensor Inputs
const int FLEX_PINKY = 36;    // GPIO36
const int FLEX_RING = 39;     // GPIO39
const int FLEX_MIDDLE = 34;   // GPIO34
const int FLEX_INDEX = 35;    // GPIO35
const int FLEX_THUMB = 32;    // GPIO32

// PWM Outputs for Electromagnets (1kHz 50% Duty Cycle)
const int PWM_PINKY = 21;     // GPIO21
const int PWM_RING = 19;      // GPIO19
const int PWM_MIDDLE = 18;    // GPIO18
const int PWM_INDEX = 5;      // GPIO5
const int PWM_THUMB = 17;     // GPIO17

// Vibration Actuator Outputs
const int VIB_PINKY = 33;     // GPIO33
const int VIB_RING = 25;      // GPIO25
const int VIB_MIDDLE = 26;    // GPIO26
const int VIB_INDEX = 4;      // GPIO4
const int VIB_THUMB = 13;     // GPIO13

// Thresholds for flex sensors
const int thresholdIndex = 2200;
const int thresholdMiddle = 2200;
const int thresholdRing = 2100;
const int thresholdPinky = 2000;
const int thresholdThumb = 2000;

// PWM properties
const int PWM_FREQ = 1000;    // 1kHz frequency
const int PWM_RES = 8;        // 8-bit resolution (0-255)
const int PWM_CHANNELS = 5;   // Number of PWM channels needed

void setup() {
    // Initialize both Serial and Bluetooth
    Serial.begin(9600);
    SerialBT.begin("ESP32_MagicMitts");
    
    // Configure ADC for flex sensors
    analogSetWidth(12);  // 12-bit ADC resolution
    
    // Configure PWM channels for electromagnets
    ledcSetup(0, PWM_FREQ, PWM_RES);  // Channel 0 - Pinky
    ledcSetup(1, PWM_FREQ, PWM_RES);  // Channel 1 - Ring
    ledcSetup(2, PWM_FREQ, PWM_RES);  // Channel 2 - Middle
    ledcSetup(3, PWM_FREQ, PWM_RES);  // Channel 3 - Index
    ledcSetup(4, PWM_FREQ, PWM_RES);  // Channel 4 - Thumb
    
    // Attach PWM channels to GPIO pins
    ledcAttachPin(PWM_PINKY, 0);
    ledcAttachPin(PWM_RING, 1);
    ledcAttachPin(PWM_MIDDLE, 2);
    ledcAttachPin(PWM_INDEX, 3);
    ledcAttachPin(PWM_THUMB, 4);
    
    // Setup vibration actuator pins
    pinMode(VIB_PINKY, OUTPUT);
    pinMode(VIB_RING, OUTPUT);
    pinMode(VIB_MIDDLE, OUTPUT);
    pinMode(VIB_INDEX, OUTPUT);
    pinMode(VIB_THUMB, OUTPUT);

    Serial.println("System Ready. Sending data in CSV format.");
    SerialBT.println("ESP32 Glove Bluetooth Started.");
}
void loop() {
    // Read flex sensors
    int indexFlex = analogRead(FLEX_INDEX);
    int middleFlex = analogRead(FLEX_MIDDLE);
    int ringFlex = analogRead(FLEX_RING);
    int pinkyFlex = analogRead(FLEX_PINKY);
    int thumbFlex = analogRead(FLEX_THUMB);
    
    // Send values over both Serial and Bluetooth
    String dataString = String(indexFlex) + "," + 
                       String(middleFlex) + "," + 
                       String(ringFlex) + "," + 
                       String(pinkyFlex) + "," + 
                       String(thumbFlex);
    
    Serial.println(dataString);
    SerialBT.println(dataString);

    // Check for commands from both Serial and Bluetooth
    checkCommands(Serial);
    checkCommands(SerialBT);
    
    delay(20); // 50Hz update rate
}

void checkCommands(Stream &port) {
    if (port.available()) {
        String command = port.readStringUntil('\n');
        handleCommand(command);
    }
}

void handleCommand(String command) {
    // Match commands from GrabDetector.cs in unity - to receive
    if (command.startsWith("thumb_haptic")) {
        digitalWrite(VIB_THUMB, HIGH);
        delay(50);
        digitalWrite(VIB_THUMB, LOW);
    }
    else if (command.startsWith("index_haptic")) {
        digitalWrite(VIB_INDEX, HIGH);
        delay(50);
        digitalWrite(VIB_INDEX, LOW);
    }
    else if (command.startsWith("middle_haptic")) {
        digitalWrite(VIB_MIDDLE, HIGH);
        delay(50);
        digitalWrite(VIB_MIDDLE, LOW);
    }
    else if (command.startsWith("ring_haptic")) {
        digitalWrite(VIB_RING, HIGH);
        delay(50);
        digitalWrite(VIB_RING, LOW);
    }
    else if (command.startsWith("pinky_haptic")) {
        digitalWrite(VIB_PINKY, HIGH);
        delay(50);
        digitalWrite(VIB_PINKY, LOW);
    }
    // Brake commands
    else if (command.startsWith("brake_thumb")) ledcWrite(4, 255);
    else if (command.startsWith("brake_index")) ledcWrite(3, 255);
    else if (command.startsWith("brake_middle")) ledcWrite(2, 255);
    else if (command.startsWith("brake_ring")) ledcWrite(1, 255);
    else if (command.startsWith("brake_pinky")) ledcWrite(0, 255);
    // Release command
    else if (command.startsWith("release")) {
        // Turn off all electromagnets
        for(int i = 0; i < PWM_CHANNELS; i++) {
            ledcWrite(i, 0);
        }
        // Turn off all vibration actuators
        digitalWrite(VIB_THUMB, LOW);
        digitalWrite(VIB_INDEX, LOW);
        digitalWrite(VIB_MIDDLE, LOW);
        digitalWrite(VIB_RING, LOW);
        digitalWrite(VIB_PINKY, LOW);
    }
    // General buzz command
    else if (command.startsWith("buzz")) {
        // Activate all vibration actuators briefly
        digitalWrite(VIB_THUMB, HIGH);
        digitalWrite(VIB_INDEX, HIGH);
        digitalWrite(VIB_MIDDLE, HIGH);
        digitalWrite(VIB_RING, HIGH);
        digitalWrite(VIB_PINKY, HIGH);
        delay(50);
        digitalWrite(VIB_THUMB, LOW);
        digitalWrite(VIB_INDEX, LOW);
        digitalWrite(VIB_MIDDLE, LOW);
        digitalWrite(VIB_RING, LOW);
        digitalWrite(VIB_PINKY, LOW);
    }
}