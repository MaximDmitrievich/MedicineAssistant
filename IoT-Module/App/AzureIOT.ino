#include <EEPROM.h>

void readCredentials() //connection string
{
  int ssidAddr = 0;
  int passAddr = ssidAddr + SSID_LEN;
  int connectionStringAddr = passAddr + SSID_LEN;
  connectionString = (char *)malloc(CONNECTION_STRING_LEN);
  strcpy(connectionString, "HostName=MedicineAssistant.azure-devices.net;DeviceId=tempdevice;SharedAccessKey=8E4cl+KVX39FvhrfWLiQqCYd3GyGfasXs4EQI2o6tMY=");
  int connectionStringLength = EEPROMread(connectionStringAddr, connectionString);
  EEPROMWrite(connectionStringAddr, connectionString, strlen(connectionString));
}

#define EEPROM_END 0
#define EEPROM_START 1

void EEPROMWrite(int addr, char *data, int size) //Write EEPROM
{
    EEPROM.begin(EEPROM_SIZE);
    // write the start marker
    EEPROM.write(addr, EEPROM_START);
    addr++;
    for (int i = 0; i < size; i++)
    {
        EEPROM.write(addr, data[i]);
        addr++;
    }
    EEPROM.write(addr, EEPROM_END);
    EEPROM.commit();
    EEPROM.end();
}

int EEPROMread(int addr, char *buf)
{
    EEPROM.begin(EEPROM_SIZE);
    int count = -1;
    char c = EEPROM.read(addr);
    addr++;
    if (c != EEPROM_START)
    {
        return 0;
    }
    while (c != EEPROM_END && count < EEPROM_SIZE)
    {
        c = (char)EEPROM.read(addr);
        count++;
        addr++;
        buf[count] = c;
    }
    EEPROM.end();
    return count;
}





