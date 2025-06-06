Что мы используем:

1. Датчик газа(MQ135)
2. Датчик температуры и давления(BME280)
3. Arduino UNO
4. Ethernet shield (ENC28J60)

Схема подключения:
1. Ethernet Shield:

    ENC28J60 |  Arduino UNO
       VCC          5V
       GND	        GND
       SCK          D13
       SO	          D12
       SI	          D11
       CS           D10


2. MQ135:
      MQ135 |  Arduino UNO
       VCC          5V
       GND	        GND
       A0           A0

3. BME280
      MQ135 |  Arduino UNO
       VCC          3.3V
       GND	        GND
       SCL          A5
       SDA          A4


