#include <ESP8266WiFi.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>
#include <AzureIoTUtility.h>

//Defined pins
#define TEMPERATURE_PIN            D4
#define CARDIO_INPUT_PIN           A0
#define CARDIO_LOMIN_PIN           D0
#define CARDIO_LOPLU_PIN           D1
#define PULSE_PIN                  D2

//Cardio defines
#define S_ECG_SIZE 226

//Defined ints
#define TEMPERATURE_ALERT 38
#define INTERVAL 2000
#define EEPROM_SIZE 512
#define SSID_LEN 32
#define PASS_LEN 32
#define CONNECTION_STRING_LEN 256
#define MESSAGE_MAX_LEN 256


//Defined strings
#define DEVICE_ID ""
#define CONNECTION_STRING ""
#define DEVICE_CONNECTION_STRING ""
#define SSID_STRING ""
#define PASS_STRING ""


OneWire oneWire(TEMPERATURE_PIN);
DallasTemperature DS18B20(&oneWire);

//Strings for connection
static char *connectionString;
static char *ssidString;
static char *passString;


//Temperature string
char temperatureString[6];

//Constants for Cardio
int BPM;
unsigned long previousMillis = 0;        // will store last time LED was updated
const long interval_cardio = 10;       
unsigned long oldtime = 0;
unsigned long newtime = 0;
unsigned long beat_time = 0;
unsigned long cek_beat_time = 0;
int data_now, data_old, delta_data;
bool flag_detek = false;
float HR, HR_old;
float BPM_Array[S_ECG_SIZE];

//Message sending and pending
static bool messagePending = false;
static bool messageSending = true;

//Interval for sending and pending
static int interval = INTERVAL;


//IoT Hub client handle
static IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;

//Message counting for sending
static int messageCount = 1;

static unsigned long remessage = 0;



IPAddress apIP(192, 168, 4, 1);

void setup() {
    Serial.begin(115200);
    pinMode(LED_BUILTIN, OUTPUT);

    //Cardio pin
    pinMode(CARDIO_INPUT_PIN, INPUT);

    // leads for electrodes off detection
    pinMode(CARDIO_LOMIN_PIN, INPUT); // Setup for leads off detection LO -
    pinMode(CARDIO_LOPLU_PIN, INPUT); // Setup for leads off detection LO +
    
    //init Time
    initTime();
    
    //Read SSID, Password and Device Connection String
    readCredentials();
    
    //Connection to Wi-Fi
    Connection(ssidString, passString);
    
    //IoT Hub Client init
    iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, MQTT_Protocol);
    if (iotHubClientHandle == NULL) { // if hub client handle is failed to create
          Serial.println("Failed on IoTHubClient_CreateFromConnectionString.");
          while (1);
    }
    //Init standart message call and callback functions for IoT Hub client
    IoTHubClient_LL_SetOption(iotHubClientHandle, "product_info", "NodeMCU_ESP8266");
    IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, receiveMessageCallback, NULL);
    IoTHubClient_LL_SetDeviceMethodCallback(iotHubClientHandle, deviceMethodCallback, NULL);
    IoTHubClient_LL_SetDeviceTwinCallback(iotHubClientHandle, twinCallback, NULL);
    
    //Temperature monitoring start
    DS18B20.begin();
}

void loop() {
    float temperature = getTemperature();
    float pulse = getPulse();
    int cardio = getCardio();

    Serial.printf("QRS: %d, Pulse %lf, Temperature: %lf\n", cardio, pulse, temperature);
    if (remessage < millis()) {
        if (!messagePending && messageSending) {
            char messagePayload[MESSAGE_MAX_LEN];
            bool temperatureAlert = readMessage(messageCount, messagePayload, temperature, pulse, cardio);
            
            sendMessage(iotHubClientHandle, messagePayload, temperatureAlert);
            
            messageCount++;
        }
        remessage = millis() + interval;
        IoTHubClient_LL_DoWork(iotHubClientHandle);
    }
    
    delay(5);
}
