const int flexIndex = 34;  // Analog input pin connected to the flex sensor
const int flexMiddle = 35;
const int flexRing = 32;
const int flexPinky = 33;
const int flexThumb = 25;

#define BTN_PIN 13        // Input pin for the button
#define MOT_PIN 12        // Output pin for the motor

void setup() {
  Serial.begin(9600);  // Initialize serial communication for debugging
  pinMode(BTN_PIN, INPUT);  // Set button pin as input with an internal pull-up resistor
  pinMode(MOT_PIN, OUTPUT);        // Set motor pin as output
  digitalWrite(MOT_PIN, LOW);      // Ensure motor is off initially
}

void loop() {
  // Read sensor values
  int flexValue1 = analogRead(flexIndex);  // Index finger
  int flexValue2 = analogRead(flexMiddle);  // Middle finger
  int flexValue3 = analogRead(flexRing);    // Ring finger
  int flexValue4 = analogRead(flexPinky);   // Pinky finger
  int flexValue5 = analogRead(flexThumb);   // Thumb

  // Print all sensor values on the same line
  Serial.print("Index: ");
  Serial.print(flexValue1);
  Serial.print(", Middle: ");
  Serial.print(flexValue2);
  Serial.print(", Ring: ");
  Serial.print(flexValue3);
  Serial.print(", Pinky: ");
  Serial.print(flexValue4);
  Serial.print(", Thumb: ");
  Serial.println(flexValue5);  // Print newline at the end

  // Check if the button is pressed
  if (digitalRead(BTN_PIN) == HIGH) {  // Button is pressed (active-low button)
    digitalWrite(MOT_PIN, HIGH);       // Turn the motor on
    delay(100);
    digitalWrite(MOT_PIN, LOW);        // Turn the motor off
  } else {
    digitalWrite(MOT_PIN, LOW);       // Ensure motor is off when button is not pressed
  }

  delay(10);  // Delay for stability

}
