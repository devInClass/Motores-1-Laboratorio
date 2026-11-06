"""
Generador de arte placeholder para Slug Lab.

Dibuja todos los sprites con Pillow en estilo pixel art para que el proyecto
sea jugable desde el primer minuto y los alumnos solo tengan que sustituirlos
por sus propios dibujos.

Uso:
    python tools/generar_arte.py

Los PNG se escriben en Assets/_SlugLab/Art/. Un AssetPostprocessor de Unity
aplica automáticamente Pixels Per Unit = 32, filtro Point y pivote inferior.
"""

from __future__ import annotations

import math
import os
import random

from PIL import Image, ImageDraw

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, "Assets", "_SlugLab", "Art")

OUTLINE = (26, 28, 44, 255)
SKIN = (242, 177, 121, 255)
SKIN_SHADE = (214, 146, 96, 255)
WHITE = (255, 255, 255, 255)
EYE = (26, 28, 44, 255)

PLAYER = {
    "helmet": (122, 158, 63, 255),
    "helmet_shade": (85, 112, 43, 255),
    "vest": (74, 109, 58, 255),
    "vest_shade": (52, 78, 41, 255),
    "pants": (51, 64, 92, 255),
    "boots": (42, 47, 62, 255),
    "gun": (111, 119, 135, 255),
    "skin": SKIN,
    "skin_shade": SKIN_SHADE,
}

ENEMY = {
    "helmet": (163, 59, 59, 255),
    "helmet_shade": (122, 42, 42, 255),
    "vest": (122, 42, 42, 255),
    "vest_shade": (90, 30, 30, 255),
    "pants": (58, 54, 62, 255),
    "boots": (36, 34, 40, 255),
    "gun": (90, 94, 104, 255),
    "skin": (214, 158, 112, 255),
    "skin_shade": (176, 126, 88, 255),
}

BOSS = {
    "armor": (138, 75, 58, 255),
    "armor_dark": (104, 54, 42, 255),
    "metal": (90, 95, 110, 255),
    "metal_dark": (66, 70, 82, 255),
    "eye": (255, 64, 64, 255),
    "teeth": (236, 226, 196, 255),
}


def ensure(*parts: str) -> str:
    path = os.path.join(ART, *parts)
    os.makedirs(path, exist_ok=True)
    return path


def canvas(width: int, height: int) -> Image.Image:
    return Image.new("RGBA", (width, height), (0, 0, 0, 0))


def block(draw: ImageDraw.ImageDraw, x: int, y: int, w: int, h: int, color, outline=OUTLINE):
    """Rectángulo con contorno de 1 px: la base del look pixel art."""
    if w <= 0 or h <= 0:
        return
    draw.rectangle([x - 1, y - 1, x + w, y + h], fill=outline)
    draw.rectangle([x, y, x + w - 1, y + h - 1], fill=color)


def save(image: Image.Image, *parts: str, name: str):
    path = os.path.join(ensure(*parts), name)
    image.save(path)
    return path


# --------------------------------------------------------------------------
# Personajes
# --------------------------------------------------------------------------

def soldier(palette: dict, pose: str, phase: float = 0.0) -> Image.Image:
    """Soldado de 32x40 con el pivote en los pies (abajo al centro)."""
    img = canvas(32, 40)
    d = ImageDraw.Draw(img)

    if pose == "dead":
        block(d, 4, 30, 24, 7, palette["vest"])
        block(d, 8, 24, 8, 8, palette["skin"])
        block(d, 6, 22, 12, 4, palette["helmet"])
        block(d, 26, 30, 4, 7, palette["boots"])
        return img

    crouch = pose == "crouch"
    body_top = 20 if crouch else 16
    hip = 32 if crouch else 28
    head_y = 8 if crouch else 5

    # Piernas
    if pose == "jump":
        block(d, 11, hip, 5, 8, palette["pants"])
        block(d, 16, hip - 2, 5, 7, palette["pants"])
        block(d, 10, hip + 8, 7, 4, palette["boots"])
        block(d, 16, hip + 5, 7, 4, palette["boots"])
    elif pose == "fall":
        block(d, 10, hip, 5, 9, palette["pants"])
        block(d, 17, hip, 5, 9, palette["pants"])
        block(d, 9, hip + 9, 7, 4, palette["boots"])
        block(d, 16, hip + 9, 7, 4, palette["boots"])
    elif pose == "run":
        swing = math.sin(phase * math.tau)
        front = 3 if swing > 0 else 0
        back = 0 if swing > 0 else 3
        block(d, 12, hip, 5, 10 - front, palette["pants"])
        block(d, 17, hip, 5, 10 - back, palette["pants"])
        block(d, 10 - front, hip + 8, 8, 4, palette["boots"])
        block(d, 16 + back, hip + 8, 8, 4, palette["boots"])
    else:
        block(d, 12, hip, 5, 10, palette["pants"])
        block(d, 17, hip, 5, 10, palette["pants"])
        block(d, 11, hip + 10, 7, 4, palette["boots"])
        block(d, 17, hip + 10, 7, 4, palette["boots"])

    # Torso
    block(d, 11, body_top, 11, hip - body_top, palette["vest"])
    d.rectangle([12, body_top + 2, 15, body_top + 3], fill=palette["vest_shade"])
    d.rectangle([12, body_top + 6, 15, body_top + 7], fill=palette["vest_shade"])

    # Cabeza
    block(d, 11, head_y, 10, 10, palette["skin"])
    d.rectangle([12, head_y + 6, 20, head_y + 8], fill=palette["skin_shade"])
    block(d, 10, head_y - 3, 12, 5, palette["helmet"])
    d.rectangle([10, head_y + 1, 22, head_y + 2], fill=palette["helmet_shade"])
    d.rectangle([17, head_y + 4, 19, head_y + 5], fill=EYE)

    # Brazo y arma
    if pose in ("shoot", "aim"):
        block(d, 20, body_top + 3, 7, 4, palette["vest"])
        block(d, 25, body_top + 3, 8, 3, palette["gun"])
        if pose == "shoot":
            d.rectangle([31, body_top + 1, 33, body_top + 6], fill=(255, 222, 120, 255))
    else:
        block(d, 20, body_top + 2, 4, 9, palette["vest"])
        block(d, 21, body_top + 9, 4, 8, palette["gun"])

    return img


def boss_frame(pose: str, phase: float = 0.0) -> Image.Image:
    """Jefe de 96x96, grande y pesado."""
    img = canvas(96, 96)
    d = ImageDraw.Draw(img)
    bob = 1 if math.sin(phase * math.tau) > 0 else 0

    if pose == "dead":
        block(d, 10, 66, 76, 22, BOSS["armor"])
        block(d, 30, 52, 36, 16, BOSS["metal_dark"])
        block(d, 40, 58, 16, 4, BOSS["eye"])
        return img

    # Piernas
    block(d, 30, 70, 14, 22, BOSS["metal_dark"])
    block(d, 54, 70, 14, 22, BOSS["metal_dark"])
    block(d, 26, 88, 20, 6, BOSS["metal"])
    block(d, 52, 88, 20, 6, BOSS["metal"])

    # Torso
    block(d, 24, 34 + bob, 50, 40, BOSS["armor"])
    d.rectangle([30, 44 + bob, 68, 48 + bob], fill=BOSS["armor_dark"])
    d.rectangle([30, 56 + bob, 68, 60 + bob], fill=BOSS["armor_dark"])
    d.rectangle([44, 40 + bob, 54, 62 + bob], fill=BOSS["metal_dark"])

    # Cabeza
    block(d, 34, 14 + bob, 30, 22, BOSS["metal"])
    block(d, 38, 20 + bob, 22, 10, (58, 44, 40, 255))
    d.rectangle([42, 23 + bob, 47, 26 + bob], fill=BOSS["eye"])
    d.rectangle([52, 23 + bob, 57, 26 + bob], fill=BOSS["eye"])
    # Cuernos
    block(d, 28, 8 + bob, 8, 14, BOSS["teeth"])
    block(d, 62, 8 + bob, 8, 14, BOSS["teeth"])
    # Dientes
    for i in range(5):
        d.rectangle([40 + i * 4, 30 + bob, 42 + i * 4, 33 + bob], fill=BOSS["teeth"])

    # Brazos
    if pose == "attack":
        block(d, 4, 40 + bob, 24, 12, BOSS["armor"])
        block(d, 0, 44 + bob, 12, 12, BOSS["metal_dark"])
        d.rectangle([0, 47 + bob, 7, 53 + bob], fill=BOSS["eye"])
        block(d, 70, 42 + bob, 22, 12, BOSS["armor"])
    else:
        block(d, 6, 44 + bob, 20, 14, BOSS["armor"])
        block(d, 70, 44 + bob, 20, 14, BOSS["armor"])
        block(d, 4, 58 + bob, 18, 10, BOSS["metal_dark"])
        block(d, 74, 58 + bob, 18, 10, BOSS["metal_dark"])

    return img


# --------------------------------------------------------------------------
# Escenario
# --------------------------------------------------------------------------

def tile_ground(top: bool, variant: int) -> Image.Image:
    img = canvas(32, 32)
    d = ImageDraw.Draw(img)
    random.seed(1000 + variant + (0 if top else 50))

    dirt = (109, 78, 56, 255)
    dirt_dark = (84, 58, 42, 255)
    grass = (108, 154, 70, 255)
    grass_dark = (78, 118, 52, 255)

    d.rectangle([0, 0, 31, 31], fill=dirt)
    for _ in range(26):
        x = random.randint(0, 30)
        y = random.randint(0, 30)
        d.rectangle([x, y, x + 1, y + 1], fill=dirt_dark)

    if top:
        d.rectangle([0, 0, 31, 6], fill=grass)
        d.rectangle([0, 6, 31, 7], fill=grass_dark)
        for x in range(0, 32, 4):
            d.rectangle([x, 5, x + 1, 8], fill=grass_dark)

    return img


def tile_platform() -> Image.Image:
    img = canvas(32, 16)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 31, 15], fill=OUTLINE)
    d.rectangle([1, 1, 30, 12], fill=(150, 104, 66, 255))
    d.rectangle([1, 1, 30, 3], fill=(186, 138, 92, 255))
    for x in (5, 15, 25):
        d.rectangle([x, 5, x + 1, 11], fill=(112, 76, 48, 255))
    return img


def tile_metal() -> Image.Image:
    img = canvas(32, 32)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 31, 31], fill=(74, 78, 92, 255))
    d.rectangle([0, 0, 31, 2], fill=(104, 110, 128, 255))
    d.rectangle([0, 29, 31, 31], fill=(52, 55, 66, 255))
    for cx, cy in ((7, 7), (24, 7), (7, 24), (24, 24)):
        d.rectangle([cx, cy, cx + 2, cy + 2], fill=(140, 146, 164, 255))
    return img


def tile_brick() -> Image.Image:
    img = canvas(32, 32)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 31, 31], fill=(58, 48, 58, 255))
    for row in range(4):
        y = row * 8
        offset = 0 if row % 2 == 0 else 8
        for col in range(-1, 4):
            x = col * 16 + offset
            d.rectangle([x, y, x + 14, y + 7], fill=(76, 62, 74, 255))
    return img


def background_sky() -> Image.Image:
    w, h = 512, 288
    img = canvas(w, h)
    d = ImageDraw.Draw(img)
    top = (36, 44, 84)
    bottom = (214, 130, 104)
    for y in range(h):
        t = y / (h - 1)
        color = tuple(int(top[i] + (bottom[i] - top[i]) * t) for i in range(3)) + (255,)
        d.line([(0, y), (w, y)], fill=color)
    d.ellipse([380, 60, 452, 132], fill=(255, 214, 140, 255))
    d.ellipse([392, 72, 440, 120], fill=(255, 240, 196, 255))
    return img


def background_far() -> Image.Image:
    w, h = 512, 288
    img = canvas(w, h)
    d = ImageDraw.Draw(img)
    random.seed(7)
    color = (44, 40, 66, 255)
    for i in range(9):
        x = i * 62 - 20
        peak = random.randint(70, 150)
        d.polygon([(x, h), (x + 40, h - peak), (x + 90, h)], fill=color)
    return img


def background_mid() -> Image.Image:
    w, h = 512, 288
    img = canvas(w, h)
    d = ImageDraw.Draw(img)
    random.seed(11)
    base = 250
    for i in range(10):
        x = i * 54
        bw = random.randint(38, 54)
        bh = random.randint(60, 130)
        d.rectangle([x, base - bh, x + bw, base], fill=(52, 40, 52, 255))
        d.rectangle([x - 2, base - bh - 6, x + bw + 2, base - bh], fill=(40, 30, 40, 255))
        for wx in range(x + 6, x + bw - 6, 12):
            for wy in range(base - bh + 10, base - 8, 16):
                if random.random() < 0.5:
                    d.rectangle([wx, wy, wx + 4, wy + 5], fill=(214, 168, 96, 255))
    return img


def prop_crate() -> Image.Image:
    img = canvas(32, 32)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 31, 31], fill=OUTLINE)
    d.rectangle([2, 2, 29, 29], fill=(150, 104, 66, 255))
    d.rectangle([2, 2, 29, 6], fill=(186, 138, 92, 255))
    d.line([(4, 4), (27, 27)], fill=(112, 76, 48, 255), width=2)
    d.line([(27, 4), (4, 27)], fill=(112, 76, 48, 255), width=2)
    return img


def prop_barrel() -> Image.Image:
    img = canvas(24, 40)
    d = ImageDraw.Draw(img)
    block(d, 2, 2, 20, 36, (86, 108, 120, 255))
    for y in (8, 20, 32):
        d.rectangle([2, y, 21, y + 2], fill=(56, 72, 82, 255))
    d.rectangle([7, 6, 16, 8], fill=(200, 214, 220, 255))
    return img


def prop_flag(active: bool) -> Image.Image:
    img = canvas(16, 40)
    d = ImageDraw.Draw(img)
    d.rectangle([6, 4, 8, 39], fill=(60, 64, 78, 255))
    color = (92, 226, 128, 255) if active else (120, 120, 130, 255)
    d.polygon([(9, 6), (16, 10), (9, 16)], fill=color)
    return img


def prop_health() -> Image.Image:
    img = canvas(16, 16)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 15, 15], fill=OUTLINE)
    d.rectangle([1, 1, 14, 14], fill=(238, 238, 244, 255))
    d.rectangle([6, 4, 9, 11], fill=(206, 64, 72, 255))
    d.rectangle([4, 6, 11, 9], fill=(206, 64, 72, 255))
    return img


def projectile_player() -> Image.Image:
    img = canvas(12, 4)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 11, 3], fill=(255, 196, 64, 255))
    d.rectangle([6, 1, 11, 2], fill=(255, 255, 230, 255))
    return img


def projectile_enemy() -> Image.Image:
    img = canvas(10, 10)
    d = ImageDraw.Draw(img)
    d.ellipse([0, 0, 9, 9], fill=(255, 96, 64, 255))
    d.ellipse([2, 2, 7, 7], fill=(255, 214, 120, 255))
    return img


def muzzle_flash(scale: int = 0) -> Image.Image:
    img = canvas(16, 16)
    d = ImageDraw.Draw(img)
    d.polygon([(0, 8), (7, 5), (9, 0), (11, 5), (15, 4), (12, 9), (15, 14), (10, 11), (8, 15), (6, 11), (1, 13)], fill=(255, 226, 120, 255))
    if scale == 0:
        d.ellipse([5, 5, 10, 10], fill=(255, 255, 255, 255))
    return img


def impact_frame(frame: int) -> Image.Image:
    img = canvas(16, 16)
    d = ImageDraw.Draw(img)
    random.seed(40 + frame)
    for _ in range(6 + frame * 3):
        angle = random.uniform(0, math.tau)
        length = random.uniform(2, 7 - frame)
        x = 8 + math.cos(angle) * length
        y = 8 + math.sin(angle) * length
        color = (255, 240, 180, 255) if frame < 2 else (255, 160, 90, 255)
        d.line([(8, 8), (x, y)], fill=color, width=2)
    return img


def explosion_frame(frame: int) -> Image.Image:
    img = canvas(40, 40)
    d = ImageDraw.Draw(img)
    random.seed(80 + frame)
    radius = 6 + frame * 5
    d.ellipse([20 - radius, 20 - radius, 20 + radius, 20 + radius], fill=(255, 140, 60, 255))
    inner = max(2, radius - 5)
    d.ellipse([20 - inner, 20 - inner, 20 + inner, 20 + inner], fill=(255, 232, 160, 255))
    for _ in range(4 + frame * 2):
        angle = random.uniform(0, math.tau)
        dist = radius + random.uniform(0, 8)
        x = 20 + math.cos(angle) * dist
        y = 20 + math.sin(angle) * dist
        d.ellipse([x - 2, y - 2, x + 2, y + 2], fill=(255, 190, 90, 255))
    return img


def ui_pip(filled: bool) -> Image.Image:
    img = canvas(16, 16)
    d = ImageDraw.Draw(img)
    color = (226, 64, 72, 255) if filled else (74, 62, 70, 255)
    d.rectangle([0, 0, 15, 15], fill=OUTLINE)
    d.rectangle([1, 1, 14, 14], fill=color)
    if filled:
        d.rectangle([3, 3, 6, 5], fill=(255, 168, 168, 255))
    return img


def ui_panel() -> Image.Image:
    img = canvas(32, 32)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 31, 31], fill=(20, 20, 32, 210))
    d.rectangle([1, 1, 30, 30], outline=(120, 200, 180, 255), width=1)
    return img


def ui_button() -> Image.Image:
    img = canvas(64, 24)
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, 63, 23], fill=(38, 52, 60, 255))
    d.rectangle([1, 1, 62, 22], outline=(120, 200, 180, 255), width=1)
    return img


# --------------------------------------------------------------------------

def main() -> None:
    characters = ensure("Sprites", "Personajes")
    enemies = ensure("Sprites", "Enemigos")
    boss_dir = ensure("Sprites", "Jefe")
    tiles = ensure("Sprites", "Tiles")
    props = ensure("Sprites", "Props")
    vfx = ensure("Sprites", "VFX")
    ui = ensure("Sprites", "UI")
    backgrounds = ensure("Fondos")

    # Jugador
    for i in range(4):
        soldier(PLAYER, "idle", i / 4).save(os.path.join(characters, f"player_idle_{i}.png"))
    for i in range(8):
        soldier(PLAYER, "run", i / 8).save(os.path.join(characters, f"player_run_{i}.png"))
    soldier(PLAYER, "jump").save(os.path.join(characters, "player_jump_0.png"))
    soldier(PLAYER, "fall").save(os.path.join(characters, "player_fall_0.png"))
    soldier(PLAYER, "crouch").save(os.path.join(characters, "player_crouch_0.png"))
    for i in range(2):
        soldier(PLAYER, "shoot", i / 2).save(os.path.join(characters, f"player_shoot_{i}.png"))
    soldier(PLAYER, "shoot", 0).save(os.path.join(characters, "player_aim_0.png"))
    soldier(PLAYER, "dead").save(os.path.join(characters, "player_dead_0.png"))
    soldier(PLAYER, "idle", 0.5).save(os.path.join(characters, "player_hurt_0.png"))

    # Hoja de sprites para el ejercicio de recorte (128x40, 4 frames de 32x40)
    sheet = canvas(32 * 4, 40)
    for i in range(4):
        sheet.paste(soldier(PLAYER, "idle", i / 4), (i * 32, 0))
    sheet.save(os.path.join(ensure("Referencias"), "hoja_jugador_idle.png"))

    # Enemigo soldado
    for i in range(2):
        soldier(ENEMY, "idle", i / 2).save(os.path.join(enemies, f"soldier_idle_{i}.png"))
    for i in range(4):
        soldier(ENEMY, "run", i / 4).save(os.path.join(enemies, f"soldier_walk_{i}.png"))
    soldier(ENEMY, "shoot", 0).save(os.path.join(enemies, "soldier_shoot_0.png"))
    soldier(ENEMY, "dead").save(os.path.join(enemies, "soldier_dead_0.png"))

    # Enemigo granadero (mismo dibujo, otra paleta para distinguirlo)
    grenadier = dict(ENEMY)
    grenadier["helmet"] = (72, 96, 140, 255)
    grenadier["helmet_shade"] = (52, 70, 104, 255)
    grenadier["vest"] = (72, 96, 140, 255)
    grenadier["vest_shade"] = (52, 70, 104, 255)
    for i in range(2):
        soldier(grenadier, "idle", i / 2).save(os.path.join(enemies, f"grenadier_idle_{i}.png"))

    # Jefe
    for i in range(2):
        boss_frame("idle", i / 2).save(os.path.join(boss_dir, f"boss_idle_{i}.png"))
    boss_frame("attack", 0).save(os.path.join(boss_dir, "boss_attack_0.png"))
    boss_frame("dead", 0).save(os.path.join(boss_dir, "boss_dead_0.png"))

    # Tiles
    for variant in range(3):
        tile_ground(True, variant).save(os.path.join(tiles, f"tile_ground_top_{variant}.png"))
        tile_ground(False, variant).save(os.path.join(tiles, f"tile_ground_mid_{variant}.png"))
    tile_platform().save(os.path.join(tiles, "tile_platform.png"))
    tile_metal().save(os.path.join(tiles, "tile_metal.png"))
    tile_brick().save(os.path.join(tiles, "tile_brick.png"))

    # Props
    prop_crate().save(os.path.join(props, "crate.png"))
    prop_barrel().save(os.path.join(props, "barrel.png"))
    prop_flag(False).save(os.path.join(props, "flag_off.png"))
    prop_flag(True).save(os.path.join(props, "flag_on.png"))
    prop_health().save(os.path.join(props, "health_pickup.png"))

    # VFX
    projectile_player().save(os.path.join(vfx, "bullet_player.png"))
    projectile_enemy().save(os.path.join(vfx, "bullet_enemy.png"))
    muzzle_flash(0).save(os.path.join(vfx, "muzzle_0.png"))
    muzzle_flash(1).save(os.path.join(vfx, "muzzle_1.png"))
    for i in range(3):
        impact_frame(i).save(os.path.join(vfx, f"impact_{i}.png"))
    for i in range(4):
        explosion_frame(i).save(os.path.join(vfx, f"explosion_{i}.png"))

    # Fondos
    background_sky().save(os.path.join(backgrounds, "bg_sky.png"))
    background_far().save(os.path.join(backgrounds, "bg_far.png"))
    background_mid().save(os.path.join(backgrounds, "bg_mid.png"))

    # UI
    ui_pip(True).save(os.path.join(ui, "pip_full.png"))
    ui_pip(False).save(os.path.join(ui, "pip_empty.png"))
    ui_panel().save(os.path.join(ui, "panel.png"))
    ui_button().save(os.path.join(ui, "button.png"))

    total = sum(len(files) for _, _, files in os.walk(ART))
    print(f"Arte generado. Archivos en Art/: {total}")


if __name__ == "__main__":
    main()
