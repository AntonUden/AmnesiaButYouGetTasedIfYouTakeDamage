const int relayPin = 2;

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(relayPin, OUTPUT);
  
  Serial.begin(9600);
  Serial.setTimeout(100);

  blink();
}

void blink() {
  for(int i = 0; i < 5; i++) {
    digitalWrite(LED_BUILTIN, HIGH);
    delay(100);
    digitalWrite(LED_BUILTIN, LOW);
    delay(100);
  }
}

void loop() {
  if (Serial.available() > 0) {
    String received = Serial.readString();

    received.trim();

    if(received == "test") {
      blink();
    }

    if(received == "zap") {
      digitalWrite(LED_BUILTIN, HIGH);
      digitalWrite(relayPin, HIGH);
      delay(200);
      digitalWrite(LED_BUILTIN, LOW);
      digitalWrite(relayPin, LOW);
    }
  }
}
