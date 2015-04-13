#include <Servo.h> 
#include <CmdMessenger.h> 
#include <Adafruit_NeoPixel.h>
#include <avr/power.h>
#define PIN            6
#define NUMPIXELS      16
 
Servo panServo;  
                
Servo tiltServo;
 
int posPan = 0;    
int posTilt = 0;


CmdMessenger cmdMessenger = CmdMessenger(Serial);
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);
int delayval = 500; 

enum
{
  kPan              , 
  kTilt         ,
  kLightOn,
  kLightOff,
  kStatus              
}; 

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kPan, OnSetPan);
  cmdMessenger.attach(kTilt, OnSetTilt);
  cmdMessenger.attach(kLightOn, OnSetLightOn);
  cmdMessenger.attach(kLightOff, OnSetLightOff);
  cmdMessenger.attach(kStatus, OnStatus);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kStatus,"Command without attached callback");
}

 
 
void setup() 
{ 
  panServo.attach(9);  // attaches the servo on pin 9 to the servo object 
  tiltServo.attach(10);  // attaches the servo on pin 9 to the servo object 
  
    // Listen on serial connection for messages from the PC
  Serial.begin(115200); 

  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();
  
    // Send the status to the PC that says the Arduino has booted
  // Note that this is a good debug function: it will let you also know 
  // if your program had a bug and the arduino restarted  
  cmdMessenger.sendCmd(kStatus,"Arduino head has started!");
  
   pixels.begin();
  
   
   
} 

void OnSetLightOn()
{
   for(int i=0;i<NUMPIXELS;i++){
      pixels.setPixelColor(i, pixels.Color(0,150,0)); // Moderately bright green color.
      pixels.show(); // This sends the updated pixel color to the hardware.
      delay(delayval); // Delay for a period of time (in milliseconds).
  }
}

void OnSetLightOff()
{
    for(int i=0;i<NUMPIXELS;i++){
      pixels.setPixelColor(i, pixels.Color(0,0,0)); // Moderately bright green color.
      pixels.show(); // This sends the updated pixel color to the hardware.
      delay(delayval); // Delay for a period of time (in milliseconds).
  }
}

void OnStatus()
{
}

void OnSetPan()
{
  float input = cmdMessenger.readFloatArg();
  int pan = (int)input;

  int delta =-1;
 
  
  if(posPan < pan) delta = +1;
  
  int pos  = 0;
  for(pos = posPan; pos < pan; pos += delta)  // goes from 0 degrees to 180 degrees 
  {                                  // in steps of 1 degree 
    panServo.write(pos);              // tell servo to go to position in variable 'pos' 
    delay(30);                       // waits 15ms for the servo to reach the position 
  } 
 
  cmdMessenger.sendCmd(kStatus,"Pan set.");
}

void OnSetTilt()
{
  float input = cmdMessenger.readFloatArg();
  int tilt = (int)input;
  
  int delta =-1;
  
  if(posTilt < tilt) delta = +1;
  
  int pos = 0;
  for(pos = posTilt; pos < tilt; pos += delta)  // goes from 0 degrees to 180 degrees 
  {                                  // in steps of 1 degree 
    tiltServo.write(pos);              // tell servo to go to position in variable 'pos' 
    delay(30);                       // waits 15ms for the servo to reach the position 
  } 
 
  cmdMessenger.sendCmd(kStatus,"Tilt set.");
}
 
 
void loop() 
{ 
   // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
} 
