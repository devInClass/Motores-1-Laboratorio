# M02 · Animación 2D: clips, Animator y eventos

**Sesiones 7–10** · 25, 27 y 29 de enero · 3 de febrero de 2027
**Escena:** `Lab_02_Animacion` · **Entregable:** set completo de animaciones de tu personaje

## Objetivo

Pasar de "dibujos que se mueven" a un **sistema de estados**: el código dice en qué estado está el personaje y el Animator decide qué se ve.

## Conceptos

- **Clip:** una secuencia de frames con velocidad y bucle.
- **Animator Controller:** la máquina de estados que decide qué clip se reproduce.
- **Parámetros:** `float` (velocidad), `bool` (en suelo), `trigger` (disparo).
- **Transición:** cuándo y cómo se pasa de un estado a otro (`exit time`, `duration`).
- **Evento de animación:** llamada a un método en un frame exacto (ej. crear la bala).

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 7 | Clips y keyframes | Crear Idle y Run desde frames |
| 8 | Parámetros y transiciones | Conectar Idle ↔ Run ↔ Jump ↔ Fall |
| 9 | Triggers y eventos | Disparo y cuchillo sincronizados |
| 10 | Revisión y pulido | Ajustar velocidades y *blend* |

## Práctica guiada

1. Abre `Lab_02_Animacion` y juega: observa qué animación falta o se siente mal.
2. Abre el `Animator` del `Player` y revisa los parámetros: `Speed`, `Grounded`, `VerticalSpeed`, `Crouch`, `Shoot`, `Melee`, `Hurt`, `Dead`.
3. Selecciona la transición `Idle → Run` y cambia el umbral de `0.1` a `2`.
4. Prueba y describe qué cambió.
5. Abre la ventana **Animation** y cambia el `Sample Rate` de `Run` de 14 a 8 fps.
6. Añade una transición nueva `Crouch → Idle` si no existe.

## Reto para tu juego

Tu personaje debe tener **idle, run, jump, fall y attack** con transiciones que no parpadeen. El ataque debe ser un *trigger* de una sola reproducción.

## Casos de prueba

`TC-201` a `TC-205` en el catálogo.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| No cambia de estado | Parámetro mal escrito | Usar `Animator.StringToHash` y el mismo nombre. |
| Parpadea Idle/Run | Umbrales solapados | Dejar histéresis (0.1 y 2). |
| El ataque se repite | Clip en bucle | Desmarcar *Loop Time*. |
| El sprite no gira | Se escala el padre | Voltear solo el hijo `Visual`. |
| La animación "salta" al empezar | `exit time` alto | Bajar `duration` a 0.05. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| 5 clips propios funcionando | 30 |
| Transiciones sin parpadeo | 25 |
| Trigger de ataque de una sola vez | 25 |
| Explica qué es un parámetro y una transición | 20 |
