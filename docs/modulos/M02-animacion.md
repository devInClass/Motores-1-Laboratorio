# M02 · Animación 2D: clips, Animator y blend trees

**Sesiones 7–10** · 25, 27 y 29 de enero · 3 de febrero de 2027
**Escena:** `Lab_02_Animacion` · **Entregable:** apuntado en 5 direcciones y locomoción suave

## Objetivo

Pasar de "dibujos que se mueven" a un **sistema de estados con mezcla**: el código entrega dos números (`Speed01` y `AimAngle`) y el Animator decide qué pose mostrar, interpolando entre clips en lugar de saltar de uno a otro.

## Conceptos

- **Clip:** secuencia de frames con velocidad y bucle.
- **Animator Controller:** máquina de estados que decide qué clip se reproduce.
- **Blend tree 1D:** mezcla entre clips según **un** parámetro. Aquí: `Speed01` de 0 (idle) a 1 (run).
- **Blend tree 2D:** mezcla según **dos** parámetros. Aquí: `AimAngle` (−90…90) y `Speed01` (0…1).
- **Parámetros:** `float` (`Speed01`, `AimAngle`), `bool` (`Aiming`, `Grounded`, `Crouch`), `trigger` (`Hurt`, `Melee`).
- **Transición:** cuándo se cambia de estado (`exit time`, `duration`).
- **Evento de animación:** llamada a un método en un frame exacto.

## Cómo está armado el Animator del jugador

```
Locomotion (blend tree 1D, Speed01)
   0 ── idle
   1 ── run

Aim (blend tree 2D, X = AimAngle, Y = Speed01)
   ( 90, 0) ── apuntar arriba, de pie
   ( 45, 0) ── diagonal arriba
   (  0, 0) ── al frente
   (-45, 0) ── diagonal abajo
   (-90, 0) ── abajo
   ...y las mismas cinco posiciones con Y = 1 para correr apuntando

Estados: Locomotion ⇄ Aim · Jump · Fall · Crouch · Hurt · Dead
```

El código no dice "reproduce la animación de arriba": dice `AimAngle = 90` y `Aiming = true`. El blend tree hace el resto, y **mezcla** las diagonales si el ángulo está entre dos poses.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 7 | Clips y blend 1D | Idle ↔ Run con `Speed01` |
| 8 | Blend 2D | Cinco direcciones de apuntado |
| 9 | Triggers y eventos | Disparo, cuchillo y daño sincronizados |
| 10 | Revisión y pulido | Ajustar umbrales, posiciones y velocidades |

## Práctica guiada

1. Abre `Lab_02_Animacion` y dispara con `J` mientras pulsas `↑`, `↑+D`, `D`, `↓+D` (en el aire) y `↓`.
2. Abre el `Animator` del `Player` y selecciona el estado `Aim`.
3. En el inspector del blend tree cambia la posición de la pose de "arriba" de `90` a `60` y prueba otra vez.
4. Cambia el `Sample Rate` del clip `player_aim_up_run` de 12 a 6 fps.
5. Selecciona `Locomotion` y cambia el umbral de `run` de `1` a `0.5`: ¿a qué velocidad empieza a correr?
6. Abre `PlayerAnimator.cs` y localiza `SetFloat(AimAngleHash, ...)`.
7. Prueba a poner `_AimAngle` fijo en `45` desde el inspector del Animator (modo *Play*).

## Reto para tu juego

Tu personaje debe apuntar en **5 direcciones** (arriba, diagonal arriba, frente, diagonal abajo, abajo) y mezclar con **correr**. El cambio entre direcciones no debe dar saltos bruscos.

## Casos de prueba

`TC-201` a `TC-207` en `docs/04-casos-de-prueba.md`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| No cambia de estado | Parámetro mal escrito | Usar `Animator.StringToHash` y el mismo nombre. |
| El apuntado no responde | `AimAngle` no se actualiza | Revisar `PlayerAnimator` y `PlayerCombat.AimDirection`. |
| Se ve "fantasma" al mezclar | Dos poses muy distintas mezcladas | Ajustar posiciones del blend tree o usar menos direcciones. |
| El ataque se repite | Clip en bucle | Desmarcar *Loop Time*. |
| El sprite no gira | Se escala el padre | Voltear solo el hijo `Visual`. |
| La animación "salta" al empezar | `exit time` alto | Bajar `duration` a 0.05. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Blend 1D de locomoción funcionando | 25 |
| Blend 2D con 5 direcciones | 35 |
| Transiciones sin parpadeo | 20 |
| Explica qué es un blend tree y qué son sus parámetros | 20 |
