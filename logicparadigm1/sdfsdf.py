#!/usr/bin/env python3
import time
import threading
import sys
import keyboard
import numpy as np
from mss import mss
from Xlib import display

def exiting():
    sys.exit()

class TriggerBot:
    def __init__(self):
        self.sct = mss()
        self.triggerbot = False
        self.triggerbot_toggle = True
        self.exit_program = False
        self.toggle_lock = threading.Lock()

        # Получаем разрешение экрана
        d = display.Display().screen()
        self.WIDTH, self.HEIGHT = d.width_in_pixels, d.height_in_pixels
        
        # Зона проверки (5x5 пикселей в центре)
        self.ZONE = 5
        self.GRAB_ZONE = (
            int(self.WIDTH / 2 - self.ZONE),
            int(self.HEIGHT / 2 - self.ZONE),
            int(self.WIDTH / 2 + self.ZONE),
            int(self.HEIGHT / 2 + self.ZONE),
        )

        # Жестко заданные параметры (аналогично вашему конфигу)
        self.trigger_hotkey = 'shift'  # 0xA0 - это код левого Shift
        self.base_delay = 0.01
        self.trigger_delay = 40
        self.color_tolerance = 70
        self.always_enabled = False
        self.R, self.G, self.B = (250, 100, 250)  # Фиолетовый цвет

    def cooldown(self):
        time.sleep(0.1)
        with self.toggle_lock:
            self.triggerbot_toggle = True
            # Звуковые сигналы
            print("\a", end='')  # Первый beep
            time.sleep(0.075)
            print("\a", end='')  # Второй beep (высокий или низкий в зависимости от состояния)

    def search_color(self):
        img = np.array(self.sct.grab({
            'left': self.GRAB_ZONE[0],
            'top': self.GRAB_ZONE[1],
            'width': self.ZONE * 2,
            'height': self.ZONE * 2
        }))

        pmap = np.array(img)
        pixels = pmap.reshape(-1, 4)
        color_mask = (
            (pixels[:, 0] > self.R - self.color_tolerance) & (pixels[:, 0] < self.R + self.color_tolerance) &
            (pixels[:, 1] > self.G - self.color_tolerance) & (pixels[:, 1] < self.G + self.color_tolerance) &
            (pixels[:, 2] > self.B - self.color_tolerance) & (pixels[:, 2] < self.B + self.color_tolerance)
        )
        matching_pixels = pixels[color_mask]
        
        if self.triggerbot and len(matching_pixels) > 0:
            delay_percentage = self.trigger_delay / 100.0
            actual_delay = self.base_delay + self.base_delay * delay_percentage
            time.sleep(actual_delay)
            keyboard.press_and_release('k')

    def toggle(self):
        if keyboard.is_pressed(self.trigger_hotkey):
            with self.toggle_lock:
                if self.triggerbot_toggle:
                    self.triggerbot = not self.triggerbot
                    print(f"TriggerBot {'ON' if self.triggerbot else 'OFF'}")
                    self.triggerbot_toggle = False
                    threading.Thread(target=self.cooldown).start()

        if keyboard.is_pressed('ctrl+shift+x'):
            self.exit_program = True
            exiting()

    def hold(self):
        while True:
            if keyboard.is_pressed(self.trigger_hotkey):
                self.triggerbot = True
                self.search_color()
            else:
                time.sleep(0.1)
            
            if keyboard.is_pressed('ctrl+shift+x'):
                self.exit_program = True
                exiting()

    def start(self):
        print("TriggerBot started")
        print(f"Hotkey: {self.trigger_hotkey} (hold to activate)")
        print("Press Ctrl+Shift+X to exit")
        
        while not self.exit_program:
            if self.always_enabled:
                self.toggle()
                self.search_color() if self.triggerbot else time.sleep(0.1)
            else:
                self.hold()

if __name__ == "__main__":
    try:
        TriggerBot().start()
    except KeyboardInterrupt:
        exiting()
