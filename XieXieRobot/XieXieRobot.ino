// *** SentandReceive ***

// This example expands the previous Receive example. The Arduino will now send back a status.
// It adds a demonstration of how to:
// - Handle received commands that do not have a function attached
// - Send a command with a parameter to the PC

#include <CmdMessenger.h>  // CmdMessenger
#include <AFMotor.h>

AF_DCMotor motor1(1);
AF_DCMotor motor2(2);
AF_DCMotor motor3(3);
AF_DCMotor motor4(4);
// Blinking led variables 
bool ledState                   = 0;   // Current state of Led
const int kBlinkLed             = 13;  // Pin of internal Led
int speedM1 = 0;
int speedM2 = 0;
int speedM3 = 0;
int speedM4 = 0;

int setSpeed = 0;

int speedOffsetM1 = 0;
int speedOffsetM2 = 0;
int speedOffsetM3 = 0;
int speedOffsetM4 = 0;


// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  kSetLed              , // Command to request led to be set in specific state
  kSetAll         ,
  kSetAllOff,
  kTurnL,
  kTurnR,
  kStop,
  kSetSpeed,
  kStatus             , // Command to report status
};

void OnSetSpeed()
{
  float input = cmdMessenger.readFloatArg();
  setSpeed = (int)input;
  SetAll(1,1,1,1);
  cmdMessenger.sendCmd(kStatus,"Speed set.");
}

void OnSetAll()
{
  float input = cmdMessenger.readFloatArg();

  int newSpeed = (int)input;
  SetAll(newSpeed,newSpeed,newSpeed,newSpeed);
  cmdMessenger.sendCmd(kStatus,"All set to specified.");
}

void OnSetAllOff()
{
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);

  cmdMessenger.sendCmd(kStatus,"All off.");
}

void OnStop()
{
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);

  cmdMessenger.sendCmd(kStatus,"All stopped.");
}

void OnTurnL()
{
   int prevSpeed = setSpeed;
   setSpeed = 255;
   SetAll(1,-1,1,-1);
   
   setSpeed = prevSpeed;

  //speedOffsetM2-=50;
  //speedOffsetM4-=50;
  //speedOffsetM1=0;
  //speedOffsetM3=0;

  //SetAll(speedM1,speedM2,speedM3,speedM4);

  cmdMessenger.sendCmd(kStatus,"Turning left.");
}

void OnTurnR()
{
   setSpeed = 200;
   SetAll(-1,1,-1,1);

  //speedOffsetM1+=50;
  //speedOffsetM3+=50;
  //speedOffsetM2=0;
  //speedOffsetM4=0;

 // SetAll(speedM1,speedM2,speedM3,speedM4);

  cmdMessenger.sendCmd(kStatus,"Turning right.");
}

// Callback function that sets led on or off
void OnSetLed()
{
  // Read led state argument, interpret string as boolean
  ledState = cmdMessenger.readBoolArg();
  // Set led
  digitalWrite(kBlinkLed, ledState?HIGH:LOW);
  // Send back status that describes the led state
  cmdMessenger.sendCmd(kStatus,(int)ledState);
}

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kSetLed, OnSetLed);
  cmdMessenger.attach(kSetAll, OnSetAll);
  cmdMessenger.attach(kSetAllOff, OnSetAllOff);
  cmdMessenger.attach(kTurnL, OnTurnL);
  cmdMessenger.attach(kTurnR, OnTurnR);
  cmdMessenger.attach(kSetSpeed, OnSetSpeed);
  cmdMessenger.attach(kStop, OnStop);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kStatus,"Command without attached callback");
}



// Setup function
void setup() 
{
  // Listen on serial connection for messages from the PC
  Serial.begin(115200); 

  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);

  // Send the status to the PC that says the Arduino has booted
  // Note that this is a good debug function: it will let you also know 
  // if your program had a bug and the arduino restarted  
  cmdMessenger.sendCmd(kStatus,"Arduino has started!");

  // set pin for blink LED
  pinMode(kBlinkLed, OUTPUT);
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
}

void SetAll(int m1, int m2, int m3, int m4)
{
  speedM1 = abs(m1);
  motor1.setSpeed(speedM1 * setSpeed);
  if( m1 <0 ) {
    motor1.run(BACKWARD);
  }
  else
  {
    motor1.run(FORWARD);
  }

  speedM2 = abs(m2);
  motor2.setSpeed(speedM2 * setSpeed);
  if( m2 <0 ) {
    motor2.run(BACKWARD);
  }
  else
  {
    motor2.run(FORWARD);
  }

  speedM3 = abs(m3);
  motor3.setSpeed(speedM3 * setSpeed);
  if( m3 <0 ) {
    motor3.run(BACKWARD);
  }
  else
  {
    motor3.run(FORWARD);
  }

  speedM4 = abs(m4);
  motor4.setSpeed(speedM4 * setSpeed);
  if( m4 <0 ) {
    motor4.run(BACKWARD);
  }
  else
  {
    motor4.run(FORWARD);
  }
}




