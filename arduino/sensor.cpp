#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_LSM303_U.h>
#include <Adafruit_L3GD20_U.h>
#include <Adafruit_9DOF.h>

#include "Arduino.h"
#include "RadioFunctions.h"
/* Assign a unique ID to the sensors */
void initSensors();
void setup(void);
void loop(void);

Adafruit_9DOF                dof   = Adafruit_9DOF();
Adafruit_LSM303_Accel_Unified accel = Adafruit_LSM303_Accel_Unified(30301);
Adafruit_LSM303_Mag_Unified   mag   = Adafruit_LSM303_Mag_Unified(30302);
Adafruit_L3GD20_Unified      gyr   = Adafruit_L3GD20_Unified(30303);

/* Update this with the correct SLP for accurate altitude measurements */
float seaLevelPressure = SENSORS_PRESSURE_SEALEVELHPA;

float p_roll;
float p_pitch;
float p_gyro_x;
float p_gyro_y;
float p_gyro_z;
float p_heading;
char buf[8];

/**************************************************************************/
/*!
    @brief  Initialises all the sensors used by this example
*/
/**************************************************************************/
void initSensors()
{
  if(!accel.begin())
  {
    /* There was a problem detecting the LSM303 ... check your connections */
    Serial.println(F("Ooops, no LSM303 detected ... Check your wiring!"));
    while(1);
  }
  if(!mag.begin())
  {
    /* There was a problem detecting the LSM303 ... check your connections */
    Serial.println("Ooops, no LSM303 detected ... Check your wiring!");
    while(1);
  }
  if(!gyr.begin())
  {
    /* There was a problem detecting the LSM303 ... check your connections */
    Serial.println("Ooops, no L3GD20 detected ... Check your wiring!");
    while(1);
  }
}

/**************************************************************************/
/*!

*/
/**************************************************************************/
void setup(void)
{
  Serial.begin(115200);

  rfBegin(11);

  /* Initialise the sensors */
  initSensors();
}

/**************************************************************************/
/*!
    @brief  Constantly check the roll/pitch/heading/altitude/temperature
*/
/**************************************************************************/
void loop(void)
  {

    sensors_event_t accel_event;
    sensors_event_t mag_event;
    sensors_event_t gyr_event;
    sensors_vec_t   orientation;

    /* Calculate pitch and roll from the raw accelerometer data */
    if(!accel.getEvent(&accel_event)) return;
    if (dof.fusionGetOrientation(&accel_event, &mag_event, &orientation))
    {
      /* 'orientation' should have valid .roll and .pitch fields */
      p_roll = orientation.roll; //roll
      //    if(orientation.roll<=0){
      //      Serial.print(-orientation.roll); //roll
      //    }
      //    else{
      //      Serial.print(360-orientation.roll); //roll
      //    }
      p_pitch = orientation.pitch; //pitch
      //    if(orientation.pitch<0){
      //      Serial.print(-orientation.pitch); //pitch
      //    }
      //    else{
      //      Serial.print(360-orientation.pitch); //pitch
      //    }
    }

    /* Calculate x y z from gyroscope */
    if (!gyr.getEvent(&gyr_event)) return;
    else {
      /* 'orientation' should have valid .roll and .pitch fields */
      p_gyro_x = gyr_event.gyro.x; //gyr x
      p_gyro_y = gyr_event.gyro.y; //gyr y
      p_gyro_z = gyr_event.gyro.z; //gyr z

    }

    /* Calculate the heading using the magnetometer */
    if(!mag.getEvent(&mag_event)) return;
    //  dof.magTiltCompensation(SENSOR_AXIS_Z, &mag_event, &accel_event);
    if (dof.fusionGetOrientation(&accel_event, &mag_event, &orientation)) {
      /* 'orientation' should have valid .heading data now */
      p_heading = orientation.heading;
    }




    dtostrf(p_roll, 1, 2, buf);
    rfPrint(buf);
    rfPrint(",");

    dtostrf(p_pitch, 1, 2, buf);
    rfPrint(buf);
    rfPrint(",");

    dtostrf(p_gyro_x, 1, 2, buf);
    rfPrint(buf);
    rfPrint(",");

    dtostrf(p_gyro_y, 1, 2, buf);
    rfPrint(buf);
    rfPrint(",");

    dtostrf(p_gyro_z, 1, 2, buf);
    rfPrint(buf);
    rfPrint(",");

    dtostrf(p_heading, 1, 2, buf);
    rfPrint(buf);
    rfPrint("\n");

    delay(100);
}
