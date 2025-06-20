#include "EtherCard.h"

static byte mymac[] = {0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A }; // MAC Address
static byte myip[] = {169, 254, 190, 103}; // ВОТ тут надо заменить IP на IP который выдал маршутизатор на +1 в конце(допустим у Паши 101 в конце то мы делаем 102 или 110 но не 101)
byte Ethernet::buffer[1200]; BufferFiller bfill;

int LedPins[] = {2, 3, 4, 5, 6, 7, 8, 9};
boolean PinStatus[] = {1, 2, 3, 4, 5, 6, 7, 8};

const char http_OK[] PROGMEM =
  "HTTP/1.0 200 OK\r\n"
  "Content-Type: text/html\r\n"
  "Pragma: no-cache\r\n\r\n";

const char http_Found[] PROGMEM =
  "HTTP/1.0 302 Found\r\n"
  "Location: /\r\n\r\n";

const char http_Unauthorized[] PROGMEM =
  "HTTP/1.0 401 Unauthorized\r\n"
  "Content-Type: text/html\r\n\r\n"
  "<h1>401 Unauthorized</h1>";

// оформление Web страницы
void homePage() {
  bfill.emit_p(PSTR("$F"
  "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>"
  "<meta name='viewport' content='width=device-width, initial-scale=1.0'>"
  "<title>Управление Ардуино УНО</title>"
  "<h1 style='color:#0ea6f2'>Управление Ардуино</h1>"
  "<font size='3em'>"
  "<font style='display:none;'>Светодиод 0: <a href='?ArduinoPIN1=$F'>$F</a></font>"
  "Светодиод 1: <a href='?ArduinoPIN2=$F'>$F</a><br /><br />"
  "Светодиод 2: <a href='?ArduinoPIN3=$F'>$F</a><br /><br />"
  "Светодиод 3: <a href='?ArduinoPIN4=$F'>$F</a><br /><br />"
  "</font>"),

  http_OK,
  PinStatus[1] ? PSTR("off") : PSTR("on"),
  PinStatus[1] ? PSTR("<font color='green'>ON</font>") : PSTR("<font color='red'>OFF</font>"),
  PinStatus[2] ? PSTR("off") : PSTR("on"),
  PinStatus[2] ? PSTR("<font color='green'>ON</font>") : PSTR("<font color='red'>OFF</font>"),
  PinStatus[3] ? PSTR("off") : PSTR("on"),
  PinStatus[3] ? PSTR("<font color='green'>ON</font>") : PSTR("<font color='red'>OFF</font>"),
  PinStatus[4] ? PSTR("off") : PSTR("on"),
  PinStatus[4] ? PSTR("<font color='green'>ON</font>") : PSTR("<font color='red'>OFF</font>"));
}

void setup() {
  Serial.begin(9600);
  ether.staticSetup(myip);
  ether.printIp("My SET IP: ", ether.myip);



  if (ether.begin(sizeof Ethernet::buffer, mymac, 10) == 0);

  for (int i = 2; i <= 5; i++) {
    pinMode(LedPins[i], OUTPUT);
    PinStatus[i] = false;
  }
}

void loop() {
  delay(1);
  word len = ether.packetReceive();
  word pos = ether.packetLoop(len);

  if (pos) {
    bfill = ether.tcpOffset();
    char *data = (char *) Ethernet::buffer + pos;
    if (strncmp("GET /", data, 5) != 0) {
      bfill.emit_p(http_Unauthorized);
    }
    else {

      data += 5;
      if (data[0] == ' ') {
        homePage(); // если обнаружены изменения на станице, запускаем функцию
        for (int i = 2; i <= 5; i++) { digitalWrite(LedPins[i], PinStatus[i + 1]); }
      }

      // "16" = количество символов в строке "?ArduinoPIN1=on "

      else if (strncmp("?ArduinoPIN1=on ", data, 16) == 0) {
        PinStatus[1] = true;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN2=on ", data, 16) == 0) {
        PinStatus[2] = true;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN3=on ", data, 16) == 0) {
        PinStatus[3] = true;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN4=on ", data, 16) == 0) {
        PinStatus[4] = true;
        bfill.emit_p(http_Found);
      }

      // "17" = количество символов в строке "?ArduinoPIN1=off "

      else if (strncmp("?ArduinoPIN1=off ", data, 17) == 0) {
        PinStatus[1] = false;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN2=off ", data, 17) == 0) {
        PinStatus[2] = false;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN3=off ", data, 17) == 0) {
        PinStatus[3] = false;
        bfill.emit_p(http_Found);
      }
      else if (strncmp("?ArduinoPIN4=off ", data, 17) == 0) {
        PinStatus[4] = false;
        bfill.emit_p(http_Found);
      }

      else { bfill.emit_p(http_Unauthorized); }
    }
    ether.httpServerReply(bfill.position());
  }
}
