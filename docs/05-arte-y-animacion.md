# Arte y animación para el juego

Esta es la especificación que deben cumplir los dibujos para que entren sin fricción al motor. El proyecto ya trae **placeholders generados**; aquí se explica cómo sustituirlos.

## Reglas de oro

1. **32 píxeles = 1 unidad** (Pixels Per Unit = 32). Todo el juego está construido con esa escala.
2. **Filtro Point**, sin compresión, sin mipmaps. Es pixel art, no una foto.
3. **Pivote abajo-centro** en personajes y props; **centro** en tiles, VFX y UI.
4. **Un archivo por frame** (o una hoja con rejilla exacta). Nombres en minúsculas y guion bajo.

## Tamaños de referencia

| Elemento | Tamaño (px) | Unidades | Pivote |
|---|---|---|---|
| Jugador | 32 × 40 | 1 × 1.25 | abajo-centro |
| Enemigo soldado | 32 × 40 | 1 × 1.25 | abajo-centro |
| Jefe | 96 × 96 | 3 × 3 | abajo-centro |
| Tile de suelo | 32 × 32 | 1 × 1 | centro |
| Plataforma | 32 × 16 | 1 × 0.5 | centro |
| Caja / barril | 32 × 32 / 24 × 40 | 1 × 1 / 0.75 × 1.25 | abajo-centro |
| Bala | 12 × 4 | 0.37 × 0.12 | centro |
| Explosión | 40 × 40 | 1.25 × 1.25 | centro |
| Fondo | 512 × 288 | 16 × 9 | centro |

## Convención de nombres

```
player_idle_0.png      player_run_0..7.png
player_jump_0.png      player_fall_0.png
player_shoot_0..1.png  player_crouch_0.png
player_hurt_0.png      player_dead_0.png

soldier_idle_0..1.png  soldier_walk_0..3.png
soldier_shoot_0.png    soldier_dead_0.png

boss_idle_0..1.png     boss_attack_0.png    boss_dead_0.png

tile_ground_top_0.png  tile_ground_mid_0.png
tile_platform.png      tile_metal.png       tile_brick.png

bg_sky.png  bg_far.png  bg_mid.png
```

Carpeta según el tipo:

```
Assets/_SlugLab/Art/Sprites/Personajes/
Assets/_SlugLab/Art/Sprites/Enemigos/
Assets/_SlugLab/Art/Sprites/Jefe/
Assets/_SlugLab/Art/Sprites/Tiles/
Assets/_SlugLab/Art/Sprites/Props/
Assets/_SlugLab/Art/Sprites/VFX/
Assets/_SlugLab/Art/Sprites/UI/
Assets/_SlugLab/Art/Fondos/
```

## Animaciones mínimas del personaje

| Clip | Frames | FPS | Bucle | Notas |
|---|---|---|---|---|
| `idle` | 4 | 8 | sí | Respiración sutil. |
| `run` | 8 | 14 | sí | Ciclo completo de piernas. |
| `jump` | 1–2 | — | no | Piernas recogidas. |
| `fall` | 1–2 | — | no | Piernas abiertas. |
| `crouch` | 1 | — | no | Mitad de altura. |
| `shoot` | 2 | 16 | no | Retroceso del arma. |
| `hurt` | 1 | — | no | Inclinado hacia atrás. |
| `dead` | 1–2 | — | no | En el suelo. |

**Regla de animación:** 8–12 fps para pixel art; 24 fps solo si el dibujo tiene mucho detalle. Más fps no se ve mejor, se ve más caro.

## Exportar desde Aseprite

1. `File → Export Sprite Sheet`.
2. Tipo: **By Rows**, sin *padding*, sin *trim*.
3. `Output: PNG`, `Scale: 100 %`.
4. Si usas la rejilla, cada frame debe medir exactamente lo mismo (32×40).

## Exportar desde Krita / Photoshop

1. Una capa por frame, todas del mismo tamaño.
2. Exportar cada capa con `nombre_0.png`, `nombre_1.png`…
3. Fondo **transparente** (alfa), nunca blanco.

## Reemplazar los placeholders

1. Copia tus PNG a la carpeta correspondiente con el nombre exacto.
2. Vuelve a Unity y espera la importación.
3. Selecciona el sprite y comprueba: **Sprite (2D and UI)**, PPU 32, Point, pivote abajo-centro.
4. Si algo está mal: `Slug Lab → 1. Configurar proyecto` reimporta todo con las reglas del curso.
5. Abre el prefab `Player` y reemplaza los frames en el Animator (ventana **Animation**).

> Los scripts generadores (`tools/generar_arte.py`) **no** se ejecutan en clase. Si los corres, sobrescriben tus dibujos.

## Escala y coherencia

Antes de dibujar el jefe, decide la escala del mundo:

- Un personaje mide **1.25 unidades** de alto.
- La pantalla mide **11.2 unidades** de alto (cámara ortográfica 5.6).
- El jefe debe ocupar entre **3 y 4 unidades**: se ve imponente pero deja espacio para esquivar.

Si tu personaje mide 3 unidades, todo el escenario tendrá que crecer y el juego se sentirá "de juguete".

## Checklist antes de entregar arte

- [ ] PPU 32 en todos los sprites.
- [ ] Filtro Point y compresión None.
- [ ] Pivote correcto (abajo-centro en personajes).
- [ ] Nombres según la convención.
- [ ] Sin píxeles semitransparentes en el borde (halo).
- [ ] Paleta limitada (12–20 colores) y consistente.
- [ ] Animaciones a 8–14 fps.
- [ ] El personaje se lee bien a tamaño real (no solo haciendo zoom).

## Errores visuales típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Sprite borroso | Filtro Bilinear | Cambiar a Point. |
| Personaje "flota" | Pivote en el centro | Pivote abajo-centro. |
| Se ve diminuto | PPU 100 en vez de 32 | Ajustar PPU. |
| Halo blanco al mover | Alfa premultiplicado / fondo blanco | Exportar con alfa real. |
| Animación tiembla | Frames de distinto tamaño | Rejilla fija. |
| Jefe tapa la pantalla | Escala mal calculada | 3–4 unidades máximo. |
