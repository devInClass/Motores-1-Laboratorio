# M09 · Jefe final: fases, patrones y telegrafía

**Sesiones 37–40** · 26, 28 y 30 de abril · 3 de mayo de 2027
**Escena:** `Lab_08_Jefe` · **Entregable:** jefe con 3 fases y barra de vida

## Objetivo

Cerrar el juego con un combate que se pueda **leer**: el jugador debe entender qué va a pasar antes de que pase, y sentir que mejora entre intento e intento.

## Conceptos

- **Fases por porcentaje de vida:** 100–66 %, 66–33 %, 33–0 %.
- **Patrones:** ráfaga, embestida, invocación.
- **Telegrafía:** animación y sonido antes del ataque.
- **Ventana de castigo:** pausa después del ataque para contraatacar.
- **Barra de jefe:** nombre, fase y relleno.
- **Invencibilidad breve** al cambiar de fase para que el combate no se atasque.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 36 | Fases y patrones | Ajustar umbrales y ataques |
| 37 | Telegrafía | Añadir aviso antes de cada ataque |
| 38 | Barra y feedback | HUD del jefe y cambio de fase |
| 39 | Playtesting | Balance con compañeros |

## Práctica guiada

1. Abre `Lab_08_Jefe` y observa las tres fases.
2. Selecciona `Jefe` y revisa `Phases` en el inspector: umbral, intervalo, ráfaga, `canCharge`, `canSummon`.
3. Baja `actionInterval` de la fase 1 a `0.8` y prueba: ¿es justo?
4. Cambia `volleySpread` y observa la legibilidad.
5. Fuerza la fase 3 y comprueba la invocación de esbirros.
6. Revisa `BossHealthBar`: nombre, fase y relleno.

## Reto para tu juego

Diseña **tu** jefe con 3 fases, al menos 3 patrones distintos y una telegrafía clara por ataque. Debe ser difícil pero superable en 3–5 intentos.

## Casos de prueba

`TC-901` a `TC-905`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| El jefe no cambia de fase | Umbral mal ordenado | Umbrales descendentes 1.0 / 0.66 / 0.33. |
| Ataques imposibles de esquivar | Sin telegrafía | 0.3–0.5 s de aviso. |
| El jefe se queda quieto | Corrutina detenida por muerte | `StopAllCoroutines` solo al morir. |
| La barra no baja | Suscripción a `Health.Changed` | Suscribirse después de `Initialize`. |
| Se puede ganar sin pelear | Meta accesible antes | `requireBossDefeated`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| 3 fases con patrones distintos | 35 |
| Telegrafía legible | 25 |
| Barra de jefe funcional | 20 |
| Balance probado con otra persona | 20 |
