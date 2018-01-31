#include <ESP8266WiFi.h>

void Connection(char *_ssid, char *_password) {
    WiFi.begin(_ssid, _password);
    
    while (WiFi.status() != WL_CONNECTED) {
      uint8_t mac[6];
      
      WiFi.macAddress(mac);
      Serial.printf("You device with MAC address %02x:%02x:%02x:%02x:%02x:%02x connects to %s failed! Waiting 10 seconds to retry.\r\n",
                  mac[0], mac[1], mac[2], mac[3], mac[4], mac[5], _ssid);
                  
      WiFi.begin(_ssid, _password);
      
      digitalWrite(LED_BUILTIN, HIGH);
      delay(500);    
      digitalWrite(LED_BUILTIN, LOW);
      delay(500);
    }
  
    if (WiFi.status() != WL_CONNECTED) {
      Serial.println("Error: Connection failed");
    } else {
      Serial.println("Connection succeed");
    }
}

void initTime()
{
    time_t epochTime;
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");

    while (true) {
        epochTime = time(NULL);

        if (epochTime == 0) {
            Serial.println("Fetching NTP epoch time failed! Waiting 2 seconds to retry.");
            delay(2000);
        } else {
            Serial.printf("Fetched NTP epoch time is: %lu.\r\n", epochTime);
            break;
        }
    }
}
