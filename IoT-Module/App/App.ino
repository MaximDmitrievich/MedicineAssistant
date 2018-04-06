#include <StaticThreadController.h>
#include <Thread.h>
#include <ThreadController.h>

#include <NTPClient.h>
#include <WiFiUdp.h>
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
#define INTERVAL 1000
#define EEPROM_SIZE 512
#define SSID_LEN 32
#define PASS_LEN 32
#define CONNECTION_STRING_LEN 256
#define MESSAGE_MAX_LEN 768


//Defined strings
#define DEVICE_ID "id1"
#define CONNECTION_STRING "HostName=MedicineAssistant.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=gwo/rkuLyQeHoT2C0OFrzRTHdP0V6ALR9+tU3Coz0K0="
#define DEVICE_CONNECTION_STRING "HostName=MedicineAssistant.azure-devices.net;DeviceId=id1;SharedAccessKey=I/XkMuW2k8F0rc5R6uYUBWeedPcRjx2SauNaA2Qnj08="
#define SSID_STRING "MGTS_GPON_5243"
#define PASS_STRING "JRQGJKYN"

#define MAIL_SSID "MR_Guest"
#define MAIL_PASS "GuestMail"

#define D_SSID "BORWL 34"
#define D_PASSWORD "123school"

#define M_SSID "E34"
#define M_PASSWORD"AlmostEasy"

#define MAI_SSID "mai_8"
#define MAI_PASSWORD "knopkawww719691998"


OneWire oneWire(TEMPERATURE_PIN);
DallasTemperature DS18B20(&oneWire);

//Strings for connection
static char *connectionString;
static char *ssidString;
static char *passString;

WiFiUDP ntpUdp;
NTPClient timeClient(ntpUdp, "europe.pool.ntp.org", 0, 60000);


//Temperature string
char temperatureString[6];

//Constants for Cardio
int BPM;
unsigned long previousMillis = 0;        // will store last time LED was updated
const long interval_cardio = 10;    
const long interval_dots = 50;   
unsigned long oldtime = 0;
unsigned long newtime = 0;
unsigned long beat_time = 0;
unsigned long cek_beat_time = 0;
int data_now, data_old, delta_data;
bool flag_detek = false;
float HR, HR_old;

float BPM_Array[interval_dots];
float Temperature_Array[interval_dots];
float HR_Array[interval_dots];
unsigned long Ticks_Array[interval_dots];
static int increment;

float height_old = 0;
float height_new = 0;
float inByte = 0;

//Message sending and pending
static bool messagePending = false;
static bool messageSending = true;

//Interval for sending and pending
static int interval = INTERVAL;

//IoT Hub client handle
static IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;

//Message counting for sending
static int messageCount = 1;

IPAddress apIP(192, 168, 4, 1);

ThreadController controll = ThreadController();
Thread *reading = new Thread();
Thread *sending = new Thread();

void SEND()
{
    if (!messagePending && messageSending) {
        char messagePayload[MESSAGE_MAX_LEN];
        readMessage(messageCount, messagePayload, Temperature_Array, HR_Array, BPM_Array, Ticks_Array, increment);
        sendMessage(iotHubClientHandle, messagePayload);
        memset(BPM_Array, 0.0, interval_dots);
        memset(HR_Array, 0.0, interval_dots);
        memset(Temperature_Array, 0.0, interval_dots);
        memset(Ticks_Array, 0, interval_dots);
        messageCount++;
        increment = 0;
    }
    IoTHubClient_LL_DoWork(iotHubClientHandle);
}

void READ()
{
    float temperature = getTemperature();
    float hr = getHR();
    float cardio = getCardio();
    timeClient.update();
    if (increment < interval_dots) {
        BPM_Array[increment] = cardio;
        Temperature_Array[increment] = temperature;
        HR_Array[increment] = hr;
        Ticks_Array[increment] = timeClient.getEpochTime();
        increment++;
    }
    Serial.printf("Cardio: %lf,\tHR: %lf,\tT: %lf\tTicks: %lu\n", cardio, hr, temperature, timeClient.getEpochTime());
}


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
    
    timeClient.begin();
    increment = 0;

    reading->onRun(READ);
    reading->setInterval(400);
    
    sending->onRun(SEND);
    sending->setInterval(5000);

    controll.add(reading);
    controll.add(sending);
}

void loop() {
    if (WiFi.status() != WL_CONNECTED) {
      Connection_Non_Pass();
    } else {
        controll.run();
    }
    delay(1);
}
