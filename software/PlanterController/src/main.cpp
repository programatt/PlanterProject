#include <SPI.h>
#include <Arduino.h>
#include <WiFiNINA.h>         
#include <ArduinoHttpClient.h>
#include <Arduino_JSON.h>
#include <RTCZero.h>
#include <Wire.h>
#include <DallasTemperature.h>
#include <Adafruit_SHT31.h>
#include <TMCStepper_UTILITY.h>
#include <TMCStepper.h>
#include <OneWire.h>

// Water Pump Pins
#define EN             6     // Enable
#define DIR            5     // Direction
#define STP            4     // Step
#define CSM            A2    // Chip select
#define SDI            A5    // Software Data In
#define SDO            A4    // Software Data Out
#define SCK            A3    // Software Clock
#define R_SENSE        0.11f // No clue what this is but it works ok
#define CLOCKWISE      true  // rotation direction looking at the motor face on

#define SHT31_I2C_ADDR 0x44

// DS18B20 Temp Sensor Pins
#define ONE_WIRE_BUS   7

// Light Source
#define LIGHT_SOURCE   22

void readSensor();
String getISOTimeString();
void connectToNetwork();
void initialize_peripherals();
void runWaterPump(int steps);
void turnOnLightSource();
void turnOffLightSource();
void printSoilTemperature();
void printAirTemperatureAndHumidity();

const char server[] = "192.168.1.12";
const int port = 5106;
String route = "/devicemessage";
const char contentType[] = "application/json";
const char deviceId[] = "dda5ee71-55c6-48ed-9748-6ce789c36c1b";

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature dt(&oneWire);
TMC2130Stepper driver(CSM, R_SENSE, SDI, SDO, SCK);
Adafruit_SHT31 sht31 = Adafruit_SHT31();
WiFiClient netSocket;
HttpClient client(netSocket, server, port);
RTCZero rtc;
JSONVar body;

long lastRequestTime = 0;
int sendInterval = 1;
unsigned long readingCount = 0;

void setup() {
  SPI.begin();         
  Serial.begin(9600);
  if (!Serial) delay(3000);
  initialize_peripherals();
  rtc.begin();
  // add it to the body JSON for the requests to the server:
  body["id"] = deviceId;
  // attempt to connect to network:
  connectToNetwork();
}

void loop() {
  // if you disconnected from the network, reconnect:
  if (WiFi.status() != WL_CONNECTED) {
    connectToNetwork();
  }
  
  // If the client is not connected:
  if (!client.connected()) {
    // and the request interval has passed:
    if (abs(rtc.getMinutes() - lastRequestTime) >= sendInterval) {
      // read the sensor
      readSensor();
      // print latest reading, for reference:
      Serial.println(JSON.stringify(body));
      // make a post request:
      Serial.println("Making POST To Server");
      client.post(route, contentType, JSON.stringify(body));
      // take note of the time you make your request:
      Serial.println("Finished Post");
      lastRequestTime = rtc.getMinutes();
    }
    // If there is a response available, read it:
  }  else if (client.available()) {
    // read the status code of the response
    int statusCode = client.responseStatusCode();
    Serial.print("Status code: ");
    Serial.println(statusCode);
    // print out the response (this takes a bit longer):
    String response = client.responseBody();
    Serial.print("Response: " );
    Serial.println(response);

    // close the request:
    client.stop();
    // increment the reading count if you got a good response
    if (statusCode == 200) {
      readingCount++;
    }
  }
}

void readSensor() {
  //turnOnLightSource();
  //runWaterPump(5000);
  //printAirTemperatureAndHumidity();
  //printSoilTemperature();
  //turnOffLightSource();

  body["timestamp"] = getISOTimeString();
  body["waterPumpOn"] = true;
  body["lightSourceOn"] = true;
  body["airHumidityPercentage"] = 1000;
  body["airTemperatureCelsius"] = 2250;
  body["soilTemperatureCelsius"] = 2100;
  body["soilMoisturePercentage"] = 6750;
}

// gets an ISO8601-formatted string of the current time:
String getISOTimeString() {
  // ISO8601 string: yyyy-mm-ddThh:mm:ssZ
  String timestamp = "20";
  if (rtc.getYear() <= 9) timestamp += "0";
  timestamp += rtc.getYear();
  timestamp += "-";
  if (rtc.getMonth() <= 9) timestamp += "0";
  timestamp += rtc.getMonth();
  timestamp += "-";
  if (rtc.getDay() <= 9) timestamp += "0";
  timestamp += rtc.getDay();
  timestamp += "T";
  if (rtc.getHours() <= 9) timestamp += "0";
  timestamp += rtc.getHours();
  timestamp += ":";
  if (rtc.getMinutes() <= 9) timestamp += "0";
  timestamp += rtc.getMinutes();
  timestamp += ":";
  if (rtc.getSeconds() <= 9) timestamp += "0";
  timestamp += rtc.getSeconds();
  timestamp += "Z";
  return timestamp;
}

void connectToNetwork() {
  // try to connect to the network:
  while ( WiFi.status() != WL_CONNECTED) {
    Serial.println("Attempting to connect to: " + String("DerpCave"));
    //Connect to WPA / WPA2 network:
    WiFi.begin("DerpCave", "Bonehead1989");
    delay(2000);
  }
  Serial.println("connected.");

  // set the time from the network:
  unsigned long epoch;
  do {
    Serial.println("Attempting to get network time");
    epoch = WiFi.getTime();
    delay(2000);
  } while (epoch == 0);

  // set the RTC:
  rtc.setEpoch(epoch);
  Serial.println(getISOTimeString());
  IPAddress ip = WiFi.localIP();
  Serial.print(ip);
  Serial.print("  Signal Strength: ");
  Serial.println(WiFi.RSSI());
}

void initialize_peripherals(){
  // Light Source
  pinMode(LIGHT_SOURCE, OUTPUT);
  
  // Soil Temp Sensor
  dt.begin();

  // Air Temp/Humid Sensor
  sht31.begin(SHT31_I2C_ADDR);
  
  // Water Pump
  pinMode(EN, OUTPUT);
  pinMode(STP, OUTPUT);
  pinMode(DIR, OUTPUT);
  digitalWrite(EN, LOW);
  driver.begin();                 //  SPI: Init CS pin and SW SPI pins                                
  driver.toff(5);                 // Enables driver in software
  driver.rms_current(600);        // Set motor RMS current
  driver.microsteps(16);          // Set microsteps to 1/16th
  driver.en_pwm_mode(true);       // Toggle stealthChop on TMC2130
  driver.pwm_autoscale(true);
  driver.shaft(CLOCKWISE);
}

void runWaterPump(int steps){
  Serial.println("Running Motor");
  for (uint16_t i = steps; i>0; i--) {
    digitalWrite(STP, HIGH);
    delayMicroseconds(160);
    digitalWrite(STP, LOW);
    delayMicroseconds(160);
  }
}

void turnOnLightSource(){
  digitalWrite(LIGHT_SOURCE, HIGH);
}

void turnOffLightSource(){
  digitalWrite(LIGHT_SOURCE, LOW);
}

void printSoilTemperature(){
  Serial.println("Getting Current Soil Temperature");
  dt.requestTemperatures();
  float soilTemperature = dt.getTempCByIndex(0);
  if(!isnan(soilTemperature)){
    Serial.print("Soil Temp: ");
    Serial.println(soilTemperature);
  }else{
    Serial.println("Unable to read soil temperature");
  }
}

void printAirTemperatureAndHumidity(){
  Serial.println("Getting Current Air Temperature and Humidity");
  float temp = sht31.readTemperature();
  float humidity = sht31.readHumidity();
  if(!isnan(temp)){
    Serial.print("Air Temp: ");
    Serial.println(temp);
  }else{
    Serial.println("Unable to read air temperature");
  }

  if(!isnan(humidity)){
    Serial.print("Air Humidity: ");
    Serial.println(humidity);
  }else{
    Serial.println("Unable to read air humidity");
  }
}