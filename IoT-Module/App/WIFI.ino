#include <ESP8266WiFi.h>

void Connection(char *_ssid, char *_password) {
  WiFi.mode(WIFI_STA);
  byte tries = 11;
  WiFi.begin(_ssid, _password);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    digitalWrite(LED_BUILTIN, HIGH);
    delay(500);    
    digitalWrite(LED_BUILTIN, LOW);
    delay(500);
  }
  Serial.println("\n");

  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("Error: Connection failed");
  } else {
    Serial.println("Connection succed");
  }
}

void initTime()
{
    time_t epochTime;
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");

    while (true)
    {
        epochTime = time(NULL);

        if (epochTime == 0)
        {
            Serial.println("Fetching NTP epoch time failed! Waiting 2 seconds to retry.");
            delay(2000);
        }
        else
        {
            Serial.printf("Fetched NTP epoch time is: %lu.\r\n", epochTime);
            break;
        }
    }
}
