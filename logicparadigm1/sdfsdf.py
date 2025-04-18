#!/usr/bin/env python3
import time
import numpy as np
from mss import mss
import subprocess
import sys

class TriggerBot:
    def __init__(self):
        self.sct = mss()
        self.active = True  # Всегда активно
        self.WIDTH, self.HEIGHT = 1280, 800  # Разрешение Steam Deck
        self.ZONE = 5  # Зона проверки (5x5 пикселей)
        
        self.GRAB_ZONE = {
            'left': int(self.WIDTH/2 - self.ZONE),
            'top': int(self.HEIGHT/2 - self.ZONE),
            'width': self.ZONE*2,
            'height': self.ZONE*2
        }
        
        # Фиолетовый цвет (R, G, B) и допуск
        self.TARGET_COLOR = (250, 100, 250)
        self.COLOR_TOLERANCE = 70
        
        # Задержки
        self.BASE_DELAY = 0.01
        self.TRIGGER_DELAY = 40  # 40%

    def press_key(self):
        """Эмулирует нажатие клавиши 'k' через xdotool"""
        try:
            subprocess.run(['xdotool', 'key', 'k'], check=True)
        except:
            print("Ошибка: xdotool не работает. Нажмите 'k' вручную!")

    def check_color(self):
        """Проверяет наличие целевого цвета в центре экрана"""
        img = np.array(self.sct.grab(self.GRAB_ZONE))
        pixels = img.reshape(-1, 4)[:, :3]  # Берем только RGB (без альфа-канала)
        
        # Создаем маску для целевого цвета
        lower = np.array([c - self.COLOR_TOLERANCE for c in self.TARGET_COLOR])
        upper = np.array([c + self.COLOR_TOLERANCE for c in self.TARGET_COLOR])
        color_mask = np.all((pixels >= lower) & (pixels <= upper), axis=1)
        
        return np.any(color_mask)

    def run(self):
        print("TriggerBot запущен для Steam Deck")
        print(f"Отслеживание цвета {self.TARGET_COLOR} в центре экрана")
        print("Нажмите Ctrl+C для выхода")
        
        try:
            while True:
                if self.active and self.check_color():
                    delay = self.BASE_DELAY * (1 + self.TRIGGER_DELAY/100)
                    time.sleep(delay)
                    self.press_key()
                time.sleep(0.01)  # Небольшая задержка для CPU
                
        except KeyboardInterrupt:
            print("\nЗавершение работы...")

if __name__ == "__main__":
    bot = TriggerBot()
    bot.run()
