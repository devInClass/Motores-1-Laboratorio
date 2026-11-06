# M07 · Interfaz, pausa y flujo de juego

**Sesiones 28–31** · 5, 7, 9 y 12 de abril de 2027
**Escena:** `Lab_07_UI` · **Entregable:** HUD funcional, pausa y pantalla final

## Objetivo

Que la información del juego sea legible sin invadir la pantalla, y que el jugador siempre sepa en qué estado está: jugando, pausado, muerto o victorioso.

## Conceptos

- **Canvas:** Screen Space Overlay vs World Space.
- **CanvasScaler:** resolución de referencia 1920×1080 y *match* 0.5.
- **Anchors y pivots:** por qué un HUD se rompe al cambiar de resolución.
- **Eventos:** el HUD se suscribe a `Health.Changed`, no pregunta cada frame.
- **`Time.timeScale = 0`:** pausa real (la UI sigue respondiendo).
- **Carga de escenas:** `SceneManager.LoadScene` y build index.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 28 | Canvas y HUD | Corazones, puntos y munición |
| 29 | Pausa | `Esc`, panel y reanudar |
| 30 | Game over / victoria | Pantalla final y reinicio |
| 31 | Guardado | Récord con `PlayerPrefs` |

## Práctica guiada

1. Abre `Lab_07_UI`, recibe daño y observa los corazones.
2. Cambia la resolución del Game view a 4:3 y a 21:9: el HUD debe seguir legible.
3. Pulsa `Esc`: el tiempo se detiene (mira `Time.timeScale` en el inspector).
4. Abre `HUDController.cs` y localiza las suscripciones a eventos.
5. Pierde las 3 vidas y revisa la pantalla final.
6. Añade tu propio texto de récord al HUD.

## Reto para tu juego

HUD con vida, puntos y munición; pausa que detenga el juego; pantalla de victoria y de derrota. Todo debe sobrevivir a un cambio de resolución.

## Casos de prueba

`TC-701` a `TC-705`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| HUD desalineado | Anchors en el centro | Anclar a la esquina. |
| La pausa no detiene la física | `timeScale` sin `FixedUpdate` acorde | Usar `Time.timeScale` y `Update` para UI. |
| El HUD no actualiza | Suscripción tardía | Suscribirse en `Start` y refrescar una vez. |
| Botones no responden | Falta `EventSystem` | Añadirlo a la escena. |
| Se pierde el récord | `PlayerPrefs` sin `Save` | Llamar `PlayerPrefs.Save()`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| HUD legible y escalable | 30 |
| Pausa real | 25 |
| Pantallas de fin de partida | 25 |
| Guardado que persiste | 20 |
