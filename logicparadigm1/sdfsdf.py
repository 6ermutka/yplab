#!/usr/bin/env python3
import pyautogui
from PIL import Image
import time

# Настройки
target_color = (128, 0, 128)  # Фиолетовый цвет (R, G, B)
threshold = 30  # Допустимое отклонение цвета
check_interval = 0.1  # Проверять каждые 0.1 секунды
center_region = 20  # Размер области в центре экрана (20x20 пикселей)

# Для безопасности - возможность экстренного выхода
pyautogui.PAUSE = 0.1
pyautogui.FAILSAFE = True

print("Скрипт запущен. Для выхода переместите курсор в верхний левый угол.")

def color_match(c1, c2, threshold):
    return all(abs(c1[i] - c2[i]) < threshold for i in range(3))

try:
    while True:
        # Получаем скриншот центра экрана
        screen = pyautogui.screenshot()
        width, height = screen.size
        center_x, center_y = width // 2, height // 2
        
        # Область в центре экрана
        region = (
            center_x - center_region // 2,
            center_y - center_region // 2,
            center_x + center_region // 2,
            center_y + center_region // 2
        )
        
        crop = screen.crop(region)
        
        # Проверяем все пиксели в области
        found = False
        for x in range(crop.width):
            for y in range(crop.height):
                pixel = crop.getpixel((x, y))
                if color_match(pixel, target_color, threshold):
                    found = True
                    break
            if found:
                break
        
        # Если нашли фиолетовый цвет - кликаем
        if found:
            pyautogui.click(button='left')
            print("Фиолетовый цвет обнаружен! Сделан клик.")
            time.sleep(0.5)  # Задержка после клика
        
        time.sleep(check_interval)

except KeyboardInterrupt:
    print("\nСкрипт остановлен.")
except Exception as e:
    print(f"Произошла ошибка: {e}")
