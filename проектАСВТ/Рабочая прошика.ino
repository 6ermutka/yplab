#include <UIPEthernet.h>
#include <SPI.h>
#include <GyverBME280.h> 

#define analogPin A0
GyverBME280 bme;


float criticalTemp = 0;
float criticalMQ135 = 0;
float criticalHumidity = 0;

byte mac[] = { 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A };
IPAddress server(169, 254, 113, 153);
const uint16_t serverPort = 1234;

EthernetClient client;

const unsigned long NORMAL_INTERVAL = 30000; // 30 секунд
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
  bme.begin(0x76);
  pinMode(analogPin, INPUT);
  Ethernet.begin(mac, IPAddress(169, 254, 113, 154));  
  delay(1000);
  getCriticalValues();
}



void loop() {
  // Проверяем состояние сети
  if (Ethernet.linkStatus() == LinkOFF) {
    Serial.println("Ethernet cable is not connected.");
    delay(1000);
    return;
  }
  float t = bme.readTemperature();  // Температура (°C)
  float pressure_Pa = bme.readPressure()*0.00750062;    // Давление (Па)
  float mq135Value = analogRead(analogPin);
  bool currentTempAlert = (t >= criticalTemp);
  bool currentMQ135Alert = (mq135Value >= criticalMQ135);
  bool currentPressure_PaAlert = (pressure_Pa >= criticalHumidity);
  
  // Определяем, было ли изменение состояния
  bool alertStateChanged = (currentTempAlert != tempAlert) || 
                          (currentMQ135Alert != mq135Alert) || 
                          (currentPressure_PaAlert != humidityAlert);
  
  // Обновляем флаги
  tempAlert = currentTempAlert;
  mq135Alert = currentMQ135Alert;
  humidityAlert = currentPressure_PaAlert;
  
  bool isAlertNow = tempAlert || mq135Alert || humidityAlert;
  
  // Определяем, нужно ли отправлять данные
  bool shouldSendData = alertStateChanged || // Если состояние изменилось
                       isAlertNow ||         // Или сейчас тревога
                       (millis() - lastSend > (isAlertNow ? ALERT_INTERVAL : NORMAL_INTERVAL)); // Или по таймеру

  if (shouldSendData) {
    String plainData = "data:" + String(mq135Value) + "," + String(pressure_Pa) + "," + String(t);

    // Шифруем данные и конвертируем в HEX
    String encryptedHex = xorEncryptToHex(plainData, xorKey);
    
    if (sendToServer(encryptedHex)) {
      Serial.println(encryptedHex);
      lastSend = millis();
    } else {
      Serial.println("Failed to send data");
    }
    
    // Если было превышение, обновляем интервал
    if (isAlertNow) {
      Serial.println("ALERT!");
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

