#include <ArduinoJson.h>

void readMessage(int messageId, char *payload, float temperature, float hr, float *cardio, int inc) {
    StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    JsonObject &root = jsonBuffer.createObject();
    
    root["deviceId"] = DEVICE_ID;
    root["messageId"] = messageId;
    root["temperature"] = temperature;
    root["hr"] = hr;
    
    JsonArray &cardiojson = root.createNestedArray("cardio");
    
    for (int i = 0; i < inc; i++) {
        cardiojson.add(cardio[i]);
    }
    
    root.printTo(payload, MESSAGE_MAX_LEN);
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
