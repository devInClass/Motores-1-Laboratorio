"""
Generador de efectos de sonido y música placeholder para Slug Lab.

Todo se sintetiza con numpy y se guarda como WAV mono 22 kHz, para que el
proyecto tenga audio sin depender de archivos externos.

Uso:
    python tools/generar_audio.py
"""

from __future__ import annotations

import os
import wave

import numpy as np

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SFX = os.path.join(ROOT, "Assets", "_SlugLab", "Resources", "SFX")
MUSIC = os.path.join(ROOT, "Assets", "_SlugLab", "Resources", "Musica")

RATE = 22050


def save_wav(name: str, data: np.ndarray, folder: str = SFX) -> None:
    os.makedirs(folder, exist_ok=True)
    data = np.clip(data, -1.0, 1.0)
    pcm = (data * 32767).astype(np.int16)
    with wave.open(os.path.join(folder, name), "wb") as handle:
        handle.setnchannels(1)
        handle.setsampwidth(2)
        handle.setframerate(RATE)
        handle.writeframes(pcm.tobytes())


def timeline(duration: float) -> np.ndarray:
    return np.linspace(0.0, duration, int(RATE * duration), endpoint=False)


def envelope(t: np.ndarray, attack: float, decay: float) -> np.ndarray:
    attack_curve = np.clip(t / max(attack, 1e-4), 0.0, 1.0)
    return attack_curve * np.exp(-t / max(decay, 1e-4))


def square(freq: np.ndarray | float, t: np.ndarray) -> np.ndarray:
    return np.sign(np.sin(2 * np.pi * freq * t))


def sweep(start: float, end: float, duration: float) -> np.ndarray:
    t = timeline(duration)
    freq = np.linspace(start, end, t.size)
    phase = 2 * np.pi * np.cumsum(freq) / RATE
    return np.sin(phase)


def noise(duration: float, seed: int = 0) -> np.ndarray:
    rng = np.random.default_rng(seed)
    return rng.uniform(-1.0, 1.0, int(RATE * duration))


def shot(seed: int, base: float = 520.0) -> np.ndarray:
    dur = 0.16
    t = timeline(dur)
    body = sweep(base, base * 0.35, dur) * envelope(t, 0.002, 0.05)
    crack = noise(dur, seed) * envelope(t, 0.001, 0.02) * 0.6
    return body * 0.7 + crack


def main() -> None:
    # Disparos
    save_wav("disparo.wav", shot(1, 640))
    save_wav("disparo_enemigo.wav", shot(2, 320) * 0.7)

    # Salto y aterrizaje
    t = timeline(0.14)
    save_wav("salto.wav", sweep(280, 760, 0.14) * envelope(t, 0.005, 0.08) * 0.5)

    t = timeline(0.12)
    save_wav("aterrizaje.wav", (noise(0.12, 4) * 0.5 + np.sin(2 * np.pi * 90 * t) * 0.4) * envelope(t, 0.002, 0.04))

    # Golpes
    t = timeline(0.1)
    save_wav("golpe.wav", (noise(0.1, 5) * 0.6 + np.sin(2 * np.pi * 140 * t) * 0.5) * envelope(t, 0.001, 0.035))
    t = timeline(0.22)
    save_wav("dano.wav", square(np.linspace(240, 110, t.size), t) * envelope(t, 0.002, 0.09) * 0.45)

    # Explosión
    t = timeline(0.6)
    rumble = np.sin(2 * np.pi * np.linspace(90, 40, t.size) * t) * 0.5
    save_wav("explosion.wav", (noise(0.6, 6) * 0.8 + rumble) * envelope(t, 0.005, 0.22))

    # Recoger objeto
    t = timeline(0.3)
    notes = [660, 880, 1320]
    pick = np.zeros_like(t)
    for i, freq in enumerate(notes):
        start = int(i * t.size / 3)
        segment = t[: t.size - start]
        pick[start:] += np.sin(2 * np.pi * freq * segment) * envelope(segment, 0.005, 0.08) * 0.35
    save_wav("recoger.wav", pick)

    # Checkpoint y puerta
    t = timeline(0.45)
    chime = np.sin(2 * np.pi * 880 * t) * np.exp(-t / 0.12) + np.sin(2 * np.pi * 1320 * t) * np.exp(-t / 0.2) * 0.6
    save_wav("checkpoint.wav", chime * 0.35)

    t = timeline(0.7)
    rumble = np.sin(2 * np.pi * np.linspace(70, 45, t.size) * t) * envelope(t, 0.05, 0.3)
    save_wav("puerta.wav", (rumble + noise(0.7, 7) * 0.2) * 0.6)

    # Rugido del jefe
    t = timeline(0.9)
    vibrato = np.sin(2 * np.pi * 80 * t) * (1 + 0.15 * np.sin(2 * np.pi * 7 * t))
    save_wav("rugido_jefe.wav", (np.tanh(vibrato * 2) * 0.5) * envelope(t, 0.05, 0.35))

    # Interfaz
    t = timeline(0.06)
    save_wav("click.wav", square(1200, t) * envelope(t, 0.001, 0.02) * 0.25)

    # Música: loop de 8 compases a 120 BPM
    bpm = 120
    beat = 60.0 / bpm
    bars = 8
    duration = beat * 4 * bars
    t = timeline(duration)

    bass_notes = [55, 55, 73.42, 65.41, 55, 55, 82.41, 73.42]
    lead_notes = [440, 523.25, 659.25, 523.25, 392, 523.25, 587.33, 493.88]

    music = np.zeros_like(t)
    for bar in range(bars):
        start = bar * beat * 4
        for step in range(8):
            pos = start + step * beat / 2
            idx = np.searchsorted(t, pos)
            if idx >= t.size:
                continue
            length = min(t.size - idx, int(RATE * beat * 0.45))
            local = np.linspace(0, beat * 0.45, length, endpoint=False)
            freq = lead_notes[(bar + step) % len(lead_notes)]
            music[idx : idx + length] += np.sin(2 * np.pi * freq * local) * np.exp(-local / 0.09) * 0.10

        idx = np.searchsorted(t, start)
        length = min(t.size - idx, int(RATE * beat * 3.5))
        local = np.linspace(0, beat * 3.5, length, endpoint=False)
        music[idx : idx + length] += square(bass_notes[bar % len(bass_notes)], local) * 0.07

        for beat_index in range(4):
            pos = start + beat_index * beat
            idx = np.searchsorted(t, pos)
            if idx >= t.size:
                continue
            length = min(t.size - idx, int(RATE * 0.05))
            local = np.linspace(0, 0.05, length, endpoint=False)
            music[idx : idx + length] += noise(0.05, 20 + beat_index) * np.exp(-local / 0.01) * 0.05

    save_wav("musica_nivel.wav", music * 0.9, MUSIC)

    files = len(os.listdir(SFX)) + len(os.listdir(MUSIC))
    print(f"Audio generado: {files} archivos")


if __name__ == "__main__":
    main()
