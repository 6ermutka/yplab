float criticalTemp = 1000;
float criticalMQ135 = 1000;
float criticalHumidity = 1000;

void setup() {
  Serial.begin(9600);
  // Эмуляция строки от сервера: 3 "квадратика" (непечатаемых символа) + "100,200,300\n"
  String serverResponse = String(char(0x01)) + String(char(0x02)) + String(char(0x03)) + "100,200,300\n";
  Serial.println("Получена строка от сервера:");
  Serial.println(serverResponse); // В мониторе порта могут отображаться квадратики вместо первых символов
  // Парсим значения
  parseServerResponse(serverResponse);
  // Выводим результат
  Serial.println("\nРаспарсенные значения:");
  Serial.print("criticalTemp: "); Serial.println(criticalTemp);
  Serial.print("criticalMQ135: "); Serial.println(criticalMQ135);
  Serial.print("criticalHumidity: "); Serial.println(criticalHumidity);
}

void loop() {
  // Не используется в тесте
}

void parseServerResponse(String response) {
  // Удаляем все нецифровые символы в начале
  int dataStart = 0;
  while (dataStart < response.length() && !isdigit(response.charAt(dataStart))) {
    dataStart++;
  }
  
  if (dataStart >= response.length()) {
    Serial.println("Ошибка: в ответе сервера нет числовых данных");
    return;
  }
  
  // Получаем подстроку, начиная с первой цифры
  String numbersOnly = response.substring(dataStart);
  numbersOnly.trim(); // Удаляем возможные пробелы и \n в конце
  
  // Разбиваем строку по запятым
  int firstComma = numbersOnly.indexOf(',');
  int secondComma = numbersOnly.indexOf(',', firstComma + 1);
  
  if (firstComma == -1 || secondComma == -1) {
    Serial.println("Ошибка: неправильный формат данных");
    return;
  }
  
  // Парсим значения в соответствующие переменные
  criticalTemp = numbersOnly.substring(0, firstComma).toFloat();
  criticalMQ135 = numbersOnly.substring(firstComma + 1, secondComma).toFloat();
  criticalHumidity = numbersOnly.substring(secondComma + 1).toFloat();
}
