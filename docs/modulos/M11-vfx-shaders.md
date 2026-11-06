# M11 · VFX y shaders

**Sesión 35** · miércoles 21 de abril de 2027
**Escena:** `Lab_10_VFX` · **Entregable:** un material propio aplicado a tu personaje o enemigo

## Objetivo

Que el alumno entienda que un sprite puede cambiar de aspecto **sin cambiar de textura**: un shader decide cómo se ve cada píxel. Y que las partículas son el "pegamento" visual que hace que un golpe exista.

## Conceptos

- **Shader:** programa que se ejecuta por píxel (o por vértice). Recibe parámetros del material.
- **Material:** instancia de un shader con valores concretos (`_FlashAmount`, `_DissolveAmount`).
- **Flash:** mezclar el color del sprite con blanco cuando recibe daño.
- **Dissolve:** recortar píxeles con un ruido y un umbral (`clip`).
- **Particle System:** emisión, *burst*, tiempo de vida, forma, color y espacio de simulación.
- **Orden de dibujo:** las partículas van en la *sorting layer* `VFX`.

## Plan de la sesión (90 min)

| Minutos | Bloque | Qué pasa |
|---|---|---|
| 0–10 | Repaso | Se golpea a un enemigo y se pregunta: ¿qué cambió visualmente? |
| 10–30 | Shader | Abrir `SpriteFlash.shader` y explicar `_FlashAmount` y `clip`. |
| 30–55 | Material | Cambiar valores en los tres materiales del laboratorio y ver el efecto. |
| 55–75 | Partículas | Ajustar `VFX_Chispas` y `VFX_Explosion` (burst, vida, tamaño). |
| 75–90 | Reto | Aplicar un material propio a su personaje o enemigo. |

## Práctica guiada

1. Abre `Lab_10_VFX` y observa los tres soldados de la izquierda: **mismo sprite**, distinto `Flash Amount`.
2. Selecciona `Blanco_Flash_50` en `Settings/Datos/Materiales` y cambia `_FlashAmount` de `0.5` a `0.9`.
3. Mira el granadero de la derecha: usa `_DissolveAmount = 0.55`. Súbelo a `0.9` y bájalo a `0.1`.
4. Abre `SpriteFlash.shader` y localiza `clip(noise - _DissolveAmount)`. ¿Qué pasa si el valor es 1?
5. Abre el prefab `VFX_Chispas` y cambia el *burst* de 14 a 40 partículas.
6. Abre `VFX_Explosion` y cambia `Start Lifetime` y `Start Size`.
7. Aplica el material a tu personaje y prueba a animar `_FlashAmount` desde código (reto).

## Reto para tu juego

Tu personaje (o tu enemigo) debe **parpadear en blanco al recibir daño** usando el shader, y debe soltar partículas al morir. El efecto no puede durar más de 0.15 s ni tapar la acción.

## Casos de prueba

| ID | Caso | Esperado |
|---|---|---|
| TC-1101 | Flash visible | El soldado con `_FlashAmount = 1` se ve blanco. |
| TC-1102 | Flash sutil | El de `0.5` se ve aclarado pero conserva el dibujo. |
| TC-1103 | Dissolve | El granadero pierde píxeles de forma irregular. |
| TC-1104 | Dissolve total | Con `_DissolveAmount = 1` no queda nada visible. |
| TC-1105 | Partículas | Al entrar a la escena se ven chispas y una explosión. |
| TC-1106 | Orden | Las partículas se dibujan delante del suelo y detrás del HUD. |
| TC-1107 | Rendimiento | La escena mantiene más de 60 FPS con las partículas activas. |

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| El sprite se ve rosa | Shader no compila o material sin shader | Revisar la consola y reasignar el shader. |
| El flash no se nota | `_FlashAmount` en 0 | Subirlo en el material. |
| El dissolve desaparece entero | `_DissolveAmount = 1` | Bajarlo a 0.5. |
| Las partículas no se ven | *Sorting layer* incorrecta o material sin textura | Poner `VFX` y asignar material. |
| Se ve borroso | El material no respeta Point | Usar el filtro del sprite, no tocar el shader. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Material con flash aplicado a un personaje | 30 |
| Dissolve funcionando en una muerte | 25 |
| Partículas ajustadas y legibles | 25 |
| Explica qué hace `clip` y qué es un material | 20 |
