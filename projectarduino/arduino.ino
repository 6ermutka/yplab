#include <UIPEthernet.h>
#include <SPI.h>
#include <DHT.h>
DHT dht(2, DHT11);
#define analogPin A0

float criticalTemp = 200;
float criticalMQ135 = 200;
float criticalHumidity = 200;

byte mac[] = { 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A };
IPAddress server(169, 254, 57, 122);
const uint16_t serverPort = 1234;

EthernetClient client;

const unsigned long NORMAL_INTERVAL = 1000; // 30 секунд
const unsigned long ALERT_INTERVAL = 1000;   // 1 секунда 
unsigned long lastSend = 0;
const char* xorKey = "asdfqwer";

bool tempAlert = false;
bool mq135Alert = false;
bool humidityAlert = false;
bool wasInAlert = false; // Флаг, что было превышение

String xorEncryptToHex(const String &input, const String &key) {
  String output;
  int keyLength = key.length();
  int inputLength = input.length();
  
  for (int i = 0; i < inputLength; i++) {
    char encryptedChar = input[i] ^ key[i % keyLength];
    // Конвертируем в HEX (два символа на байт)
    char hexBuf[3];
    sprintf(hexBuf, "%02X", (unsigned char)encryptedChar);
    output += hexBuf;
  }
  
  return output;
}

void setup() {
  Serial.begin(9600);
  dht.begin();
  pinMode(analogPin, INPUT);
  Serial.println("Initializing Ethernet...");
  Ethernet.begin(mac, IPAddress(169, 254, 57, 123));  
  Serial.print("Arduino IP: ");
  Serial.println(Ethernet.localIP());
  delay(1000);
  getCriticalValues();
  delay(1000);
}

void loop() {
  // Проверяем состояние сети
  if (Ethernet.linkStatus() == LinkOFF) {
    Serial.println("Ethernet cable is not connected.");
    delay(1000);
    return;
  }

  // Читаем данные с датчиков
  float h = dht.readHumidity();
  float t = dht.readTemperature();
  float mq135Value = analogRead(analogPin);

  // Проверяем критические значения
  bool currentTempAlert = (t >= criticalTemp);
  bool currentMQ135Alert = (mq135Value >= criticalMQ135);
  bool currentHumidityAlert = (h >= criticalHumidity);
  
  // Определяем, было ли изменение состояния
  bool alertStateChanged = (currentTempAlert != tempAlert) || 
                          (currentMQ135Alert != mq135Alert) || 
                          (currentHumidityAlert != humidityAlert);
  
  // Обновляем флаги
  tempAlert = currentTempAlert;
  mq135Alert = currentMQ135Alert;
  humidityAlert = currentHumidityAlert;
  
  bool isAlertNow = tempAlert || mq135Alert || humidityAlert;
  
  // Определяем, нужно ли отправлять данные
  bool shouldSendData = alertStateChanged || // Если состояние изменилось
                       isAlertNow ||         // Или сейчас тревога
                       (millis() - lastSend > (isAlertNow ? ALERT_INTERVAL : NORMAL_INTERVAL)); // Или по таймеру

  if (shouldSendData) {
    String plainData = "data:" + String(mq135Value) + "," + String(h) + "," + String(t) + "," +
                      (tempAlert ? "1" : "0") + "," + 
                      (mq135Alert ? "1" : "0") + "," + 
                      (humidityAlert ? "1" : "0");

    // Шифруем данные и конвертируем в HEX
    String encryptedHex = xorEncryptToHex(plainData, xorKey);
    
    if (sendToServer(encryptedHex)) {
      Serial.print("Plain data: ");
      Serial.println(plainData);
      Serial.print("Encrypted HEX sent: ");
      Serial.println(encryptedHex);
      lastSend = millis();
    } else {
      Serial.println("Failed to send data");
    }
    
    // Если было превышение, обновляем интервал
    if (isAlertNow) {
      Serial.println("ALERT STATE! Increased send frequency");
    }
  }
  
  delay(100); // Короткая задержка для стабильности
}

bool sendToServer(String data) {
  if (client.connect(server, serverPort)) {
    client.println(data);
    
    // Ждем ответа
    unsigned long start = millis();
    while (client.connected() && millis() - start < 2000) {
      if (client.available()) {
        String response = client.readStringUntil('\n');
        response.trim();
        Serial.print("Server: ");
        Serial.println(response);
        break;
      }
      delay(10);
    }
    
    client.stop();
    return true;
  }
  return false;
}

void getCriticalValues() {
  if (client.connect(server, serverPort)) {
    client.println("GET_CRITICAL");
    unsigned long start = millis();
    while (client.connected() && millis() - start < 2000) {
      if (client.available()) {
        String response = client.readStringUntil('\n');
        response.trim();
        
        int firstComma = response.indexOf(',');
        int secondComma = response.indexOf(',', firstComma + 1);
        
        if (firstComma != -1 && secondComma != -1) {
          criticalTemp = response.substring(0, firstComma).toFloat();
          criticalMQ135 = response.substring(firstComma + 1, secondComma).toFloat();
          criticalHumidity = response.substring(secondComma + 1).toFloat();
          
          Serial.print("Critical values updated: ");
          Serial.print("Temp="); Serial.print(criticalTemp);
          Serial.print(", MQ135="); Serial.print(criticalMQ135);
          Serial.print(", Hum="); Serial.println(criticalHumidity);
        }
        break;
      }
      delay(10);
    }
    client.stop();
  }
}
