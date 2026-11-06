# M01 · Sprites: importar, recortar y ordenar

**Sesiones 4–6** · 18, 20 y 22 de enero de 2027
**Escena:** `Lab_01_Sprites` · **Entregable:** hoja de personaje importada y alineada

## Objetivo

Entender que un sprite no es "una imagen": es una imagen más una configuración (PPU, pivote, filtro, compresión) que determina cómo se ve y cómo se comporta en el mundo.

## Conceptos

- **Pixels Per Unit (PPU):** cuántos píxeles hay en una unidad del mundo. En el curso: **32**.
- **Pivote:** el punto de anclaje. Personajes: abajo-centro. Tiles: centro.
- **Filtro:** Point (pixel art nítido) vs Bilinear (borroso).
- **Compresión:** sin compresión para pixel art.
- **Sorting Layer / Order in Layer:** qué se dibuja encima de qué.
- **Hoja de sprites:** una imagen con varios frames en rejilla.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 4 | Importación y PPU | Importar `hoja_jugador_idle.png` y comparar PPU 32 vs 100 |
| 5 | Pivotes y recorte | Recortar la hoja en 4 frames y alinear al suelo |
| 6 | Orden de capas y atlas | Ordenar fondo, suelo, personaje y VFX |

## Práctica guiada

1. Abre `Lab_01_Sprites` y observa la fila de personajes a distintas escalas.
2. Selecciona `player_idle_0` en el Project y revisa el inspector: PPU, filtro, compresión.
3. Abre `Art/Referencias/hoja_jugador_idle.png` y usa **Sprite Editor → Slice → Grid** con 32×40.
4. Aplica los 4 frames y compáralos con los individuales.
5. Cambia el `Sorting Layer` del personaje a `Personajes` y el fondo a `Fondo`.
6. Ejecuta `Slug Lab → 1. Configurar proyecto` y verifica que todo vuelve a estar bien.

## Reto para tu juego

Dibuja a tu personaje en una hoja de 4 frames de idle (32×40) y sustitúyelo en el prefab `Player`. Debe quedar apoyado en el suelo, sin flotar ni hundirse.

## Casos de prueba

`TC-101` a `TC-105` en `docs/04-casos-de-prueba.md`.

## Errores frecuentes

| Error | Por qué pasa | Cómo se corrige |
|---|---|---|
| Sprite borroso | Filtro Bilinear | Point. |
| Personaje flotando | Pivote centrado | Abajo-centro. |
| Se ve enorme | PPU 100 | PPU 32. |
| Marco blanco | Fondo opaco | Exportar con alfa. |
| Un frame "salta" | Frames de distinto tamaño | Rejilla fija. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Sprite propio importado con PPU 32 y Point | 30 |
| Pivote correcto y alineado al suelo | 30 |
| Hoja recortada en frames | 20 |
| Orden de capas coherente | 20 |
