#define PULSE A0

int Signal;
int Threshold = 550;


void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(9600);
}

void loop() {
  Signal = analogRead(PULSE);
  Serial.println(Signal);

  if (Signal > Threshold) {
    digitalWrite(LED_BUILTIN, HIGH);
  } else {
    digitalWrite(LED_BUILTIN, LOW);
  }
}


