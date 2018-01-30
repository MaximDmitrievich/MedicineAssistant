#include <ESP8266WiFi.h>

#include <DallasTemperature.h>
#include <OneWire.h>


#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>
#include <AzureIoTUtility.h>

#define ONE_WIRE_BUS            D4

#define DEVICE_ID "ESPDevice"

#define TEMPERATURE_ALERT 38

#define CONNECTION_STRING "HostName=MedicineAssistant.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=gwo/rkuLyQeHoT2C0OFrzRTHdP0V6ALR9+tU3Coz0K0="
#define DEVICE_CONNECTION_STRING "HostName=MedicineAssistant.azure-devices.net;DeviceId=ESPDevice;SharedAccessKey=EsQHCMHEZrSbRtES5MwOKWH3DOVw0/q7htQXhL+yJC0="

#define INTERVAL 2000

#define EEPROM_SIZE 512

#define SSID_LEN 32
#define PASS_LEN 32
#define CONNECTION_STRING_LEN 256

#define MESSAGE_MAX_LEN 256

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature DS18B20(&oneWire);

static char *connectionString;

char temperatureString[6];

static bool messagePending = false;
static bool messageSending = true;

static int interval = INTERVAL;

static IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;
static int messageCount = 1;

IPAddress apIP(192, 168, 4, 1);

void setup() {
  Serial.begin(115200);
  Serial.println("");
  Serial.println("Start 1-WiFi");
  pinMode(LED_BUILTIN, OUTPUT);

  initTime();


  //Init WiFi connection
  Connection("MGTS_GPON_5243", "JRQGJKYN");

  //Temperature monitoring
  DS18B20.begin();

  
  readCredentials();

  iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);
  IoTHubClient_LL_SetOption(iotHubClientHandle, "product_info", "ESPDevice");
  IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, receiveMessageCallback, NULL);
  IoTHubClient_LL_SetDeviceMethodCallback(iotHubClientHandle, deviceMethodCallback, NULL);
  IoTHubClient_LL_SetDeviceTwinCallback(iotHubClientHandle, twinCallback, NULL);
}

void loop() {
  float temperature = getTemperature();
  dtostrf(temperature, 2, 2, temperatureString);
  if (!messagePending && messageSending)
    {
        char messagePayload[MESSAGE_MAX_LEN];
        bool temperatureAlert = readMessage(messageCount, messagePayload, temperature);
        sendMessage(iotHubClientHandle, messagePayload, temperatureAlert);
        messageCount++;
    }
    IoTHubClient_LL_DoWork(iotHubClientHandle);
    delay(10);
}
