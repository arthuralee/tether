default:
	avr-g++ -Larduino/lib -Iarduino/include -Iarduino/libraries/Wire -Iarduino/libraries/Adafruit_Sensor -Iarduino/libraries/Adafruit_9DOF  -Iarduino/libraries/Adafruit_L3GD20_U -Iarduino/libraries/Adafruit_LSM303DLHC -Wall -DF_CPU=16000000L -Os -mmcu=atmega128rfa1 -o main.elf arduino/libraries/Wire/Wire.cpp arduino/libraries/Adafruit_Sensor/Adafruit_Sensor.cpp arduino/libraries/Adafruit_9DOF/Adafruit_9DOF.cpp arduino/libraries/Adafruit_L3GD20_U/Adafruit_L3GD20_U.cpp arduino/libraries/Adafruit_LSM303DLHC/Adafruit_LSM303_U.cpp sensor.cpp -larduino
	avr-objcopy -O ihex -R .eeprom main.elf out.hex

receiver:
	avr-g++ -Larduino/lib -Iarduino/include -Iarduino/libraries/Wire -Wall -DF_CPU=16000000L -Os -mmcu=atmega128rfa1 -o main.elf arduino/libraries/Wire/Wire.cpp receiver.cpp -larduino
	avr-objcopy -O ihex -R .eeprom main.elf out.hex

upload:
	avrdude -c arduino -p atmega128rfa1 -b 57600 -P /dev/tty.usbserial-FTHFFDCT -U flash:w:out.hex

all: default
