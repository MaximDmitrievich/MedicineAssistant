#include <ArduinoJson.h>

bool readMessage(int messageId, char *payload, float temperature, float hr, float *cardio) {
    StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    JsonObject &root = jsonBuffer.createObject();
    
    root["deviceId"] = DEVICE_ID;
    root["messageId"] = messageId;
    
    bool temperatureAlert = false;

    root["temperature"] = temperature;
    root["hr"] = hr;
    root["cardio"] = cardio;
    
    if (temperature > TEMPERATURE_ALERT) {
        temperatureAlert = true;
    }
    
    root.printTo(payload, MESSAGE_MAX_LEN);
    
    return temperatureAlert;
}

void parseTwinMessage(char *message) {
    StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    JsonObject &root = jsonBuffer.parseObject(message);
    
    if (!root.success()) {
        Serial.printf("Parse %s failed.\r\n", message);
        return;
    }

    if (root["desired"]["interval"].success()) {
        interval = root["desired"]["interval"];
    } else if (root.containsKey("interval")) {
        interval = root["interval"];
    }
}
