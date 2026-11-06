"""
Genera la imagen de portada del repositorio (docs/img/nivel1.png).

Es un montaje con el arte placeholder del propio juego, no una captura del
editor: sirve como portada del README y como referencia visual del estilo.

Uso:
    python tools/generar_banner.py
"""

from __future__ import annotations

import os

from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, "Assets", "_SlugLab", "Art")
OUT = os.path.join(ROOT, "docs", "img")

W, H = 1280, 720
SCALE = 3


def load(path: str) -> Image.Image:
    return Image.open(path).convert("RGBA")


def scaled(img: Image.Image, factor: float) -> Image.Image:
    return img.resize((int(img.width * factor), int(img.height * factor)), Image.NEAREST)


def tint(img: Image.Image, color: tuple[int, int, int, int]) -> Image.Image:
    copy = img.copy()
    pixels = copy.load()
    for y in range(copy.height):
        for x in range(copy.width):
            r, g, b, a = pixels[x, y]
            if a == 0:
                continue
            pixels[x, y] = (min(255, int(r * color[0] / 255)), min(255, int(g * color[1] / 255)), min(255, int(b * color[2] / 255)), a)
    return copy


def main() -> None:
    os.makedirs(OUT, exist_ok=True)
    canvas = Image.new("RGBA", (W, H), (20, 22, 34, 255))

    sky = scaled(load(os.path.join(ART, "Fondos", "bg_sky.png")), W / 512)
    canvas.alpha_composite(sky, (0, 0))

    far = scaled(load(os.path.join(ART, "Fondos", "bg_far.png")), W / 512)
    canvas.alpha_composite(far, (0, H - far.height))

    mid = scaled(load(os.path.join(ART, "Fondos", "bg_mid.png")), W / 512)
    canvas.alpha_composite(mid, (0, H - mid.height - 20))

    ground_top = scaled(load(os.path.join(ART, "Sprites", "Tiles", "tile_ground_top_0.png")), 3)
    ground_mid = scaled(load(os.path.join(ART, "Sprites", "Tiles", "tile_ground_mid_0.png")), 3)
    for x in range(0, W, 96):
        canvas.alpha_composite(ground_mid, (x, H - 96))
        canvas.alpha_composite(ground_top, (x, H - 192))
    for x in range(0, W, 96):
        canvas.alpha_composite(ground_mid, (x, H - 288))

    platform = scaled(load(os.path.join(ART, "Sprites", "Tiles", "tile_platform.png")), 3)
    for i in range(3):
        canvas.alpha_composite(platform, (300 + i * 96, 330))
    for i in range(3):
        canvas.alpha_composite(platform, (820 + i * 96, 250))

    crate = scaled(load(os.path.join(ART, "Sprites", "Props", "crate.png")), 3)
    canvas.alpha_composite(crate, (140, H - 192 - 96))

    player = scaled(load(os.path.join(ART, "Sprites", "Personajes", "player_shoot_0.png")), 3)
    canvas.alpha_composite(player, (250, H - 192 - player.height + 10))

    soldier = scaled(load(os.path.join(ART, "Sprites", "Enemigos", "soldier_shoot_0.png")), 3)
    canvas.alpha_composite(soldier, (700, H - 192 - soldier.height + 10))

    grenadier = tint(soldier, (180, 200, 255, 255))
    canvas.alpha_composite(grenadier, (760, 250 - grenadier.height + 10))

    boss = scaled(load(os.path.join(ART, "Sprites", "Jefe", "boss_idle_0.png")), 2.4)
    canvas.alpha_composite(boss, (990, H - 192 - boss.height + 6))

    explosion = scaled(load(os.path.join(ART, "Sprites", "VFX", "explosion_2.png")), 2)
    canvas.alpha_composite(explosion, (880, H - 192 - 120))

    bullet = scaled(load(os.path.join(ART, "Sprites", "VFX", "bullet_player.png")), 2)
    for offset in range(3):
        canvas.alpha_composite(bullet, (420 + offset * 70, H - 192 - 95))

    draw = ImageDraw.Draw(canvas)
    draw.rectangle([0, 0, W, 150], fill=(10, 12, 20, 170))
    draw.rectangle([0, 150, W, 154], fill=(120, 220, 190, 255))

    try:
        title_font = ImageFont.load_default(size=86)
        sub_font = ImageFont.load_default(size=30)
    except TypeError:
        title_font = ImageFont.load_default()
        sub_font = ImageFont.load_default()

    draw.text((48, 24), "SLUG LAB", font=title_font, fill=(242, 240, 214, 255))
    draw.text((50, 116), "MOTORES DE VIDEOJUEGOS I  ·  UNITY 2D", font=sub_font, fill=(150, 220, 210, 255))

    canvas.convert("RGB").save(os.path.join(OUT, "nivel1.png"))
    print("Portada generada en docs/img/nivel1.png")


if __name__ == "__main__":
    main()
