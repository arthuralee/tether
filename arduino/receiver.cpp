#include <Wire.h>

#include "Arduino.h"
#include "RadioFunctions.h"

void setup(void);
void loop(void);

void setup(void)
{
  Serial.begin(115200);

  rfBegin(11);

  Serial.println("Setup");

  /* Initialise the sensors */
  //initSensors();
}

/**************************************************************************/
/*!
    @brief  Constantly check the roll/pitch/heading/altitude/temperature
*/
/**************************************************************************/
void loop() {
  if (rfAvailable()) {
    Serial.print(rfRead());
  }
}
