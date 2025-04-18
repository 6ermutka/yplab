#!/usr/bin/env python3
import time
import threading
import sys
import numpy as np
from mss import mss
from pynput import keyboard
import subprocess

class TriggerBot:
    def __init__(self):
        self.sct = mss()
        self.triggerbot = False
        self.triggerbot_toggle = True
        self.toggle_lock = threading.Lock()
        
        # Параметры экрана Steam Deck (1280x800)
        self.WIDTH, self.HEIGHT = 1280, 800
        self.ZONE = 5
        self.GRAB_ZONE = {
            'left': int(self.WIDTH/2 - self.ZONE),
            'top': int(self.HEIGHT/2 - self.ZONE),
            'width': self.ZONE*2,
            'height': self.ZONE*2
        }

        # Целевой цвет (фиолетовый) и параметры
        self.R, self.G, self.B = 250, 100, 250
        self.color_tolerance = 70
        self.base_delay = 0.01
        self.trigger_delay = 40

        # Настройка слушателя клавиш
        self.listener = keyboard.Listener(
            on_press=self.on_press,
            on_release=self.on_release)
        self.listener.start()

    def on_press(self, key):
        if key == keyboard.Key.shift:
            with self.toggle_lock:
                if self.triggerbot_toggle:
                    self.triggerbot = not self.triggerbot
                    print(f"TriggerBot {'ON' if self.triggerbot else 'OFF'}")
                    self.triggerbot_toggle = False
                    threading.Thread(target=self.cooldown).start()

    def on_release(self, key):
        if key == keyboard.Key.shift:
            with self.toggle_lock:
                self.triggerbot_toggle = True

    def cooldown(self):
        time.sleep(0.1)
        with self.toggle_lock:
            self.triggerbot_toggle = True

    def search_color(self):
        img = np.array(self.sct.grab(self.GRAB_ZONE))
        pixels = img.reshape(-1, 4)
        
        color_mask = (
            (pixels[:, 0] > self.R - self.color_tolerance) &
            (pixels[:, 0] < self.R + self.color_tolerance) &
            (pixels[:, 1] > self.G - self.color_tolerance) &
            (pixels[:, 1] < self.G + self.color_tolerance) &
            (pixels[:, 2] > self.B - self.color_tolerance) &
            (pixels[:, 2] < self.B + self.color_tolerance)
        )
        
        if self.triggerbot and np.any(color_mask):
            delay = self.base_delay * (1 + self.trigger_delay/100)
            time.sleep(delay)
            subprocess.run(['xdotool', 'key', 'k'])

    def start(self):
        print("TriggerBot для Steam Deck")
        print("Удерживайте Shift для активации")
        print("Нажмите Ctrl+C для выхода")
        
        try:
            while True:
                self.search_color()
                time.sleep(0.01)
        except KeyboardInterrupt:
            print("\nВыход...")
            self.listener.stop()

if __name__ == "__main__":
    bot = TriggerBot()
    bot.start()
