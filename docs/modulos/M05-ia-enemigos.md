# M05 · Enemigos e IA: máquinas de estado

**Sesiones 19–22** · 26 de febrero · 1, 3 y 5 de marzo de 2027
**Escena:** `Lab_05_IA` · **Entregable:** enemigo que patrulla, detecta, persigue y ataca

## Objetivo

Que el alumno deje de escribir `if` sueltos y piense en **estados**: un enemigo está en una situación a la vez y cambia cuando se cumple una condición.

## Conceptos

- **Máquina de estados finita (FSM):** `Patrol → Alert → Chase → Attack → Stagger → Dead`.
- **Detección:** distancia + línea de visión (`Raycast` contra el suelo).
- **Detección de borde:** raycast hacia abajo delante del enemigo.
- **Telegrafía:** pausa breve antes de atacar para que el jugador pueda reaccionar.
- **Oleadas:** aparición por grupos y condición de "zona limpia".

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 19 | FSM y patrulla | Enemigo que va y viene sin caerse |
| 20 | Detección y persecución | Línea de visión y rango |
| 21 | Ataque y telegrafía | Disparo con aviso |
| 22 | Oleadas y puerta | Limpiar la zona para avanzar |

## Práctica guiada

1. Abre `Lab_05_IA` y observa a los enemigos: ¿qué hacen cuando te ven? ¿Y cuando te escondes?
2. Abre `EnemyBrain.cs` y localiza el `switch (CurrentState)`.
3. Cambia `sightRange` del `Enemigo_Soldado` de 9 a 4 y prueba.
4. Escóndete detrás de una caja y comprueba que deja de disparar.
5. Fuerza el estado `Stagger` recibiendo un golpe: observa la pausa.
6. Abre `EnemySpawner` en `Lab_06_Niveles` y cambia la cantidad de enemigos.

## Reto para tu juego

Diseña **un** enemigo con al menos 4 estados y una telegrafía clara. Debe poder perder al jugador de vista y volver a patrullar.

## Casos de prueba

`TC-501` a `TC-505`. Automatizado: `TC-36`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| Cae del mapa | Falta detección de borde | Raycast delante + abajo. |
| Dispara a través de paredes | No hay línea de visión | Raycast contra `Ground`. |
| Nunca ataca | Cadencia mal inicializada | Reiniciar el `Cooldown` en `Start`. |
| Se queda trabado | Estado sin salida | Toda transición debe tener salida. |
| Todos disparan a la vez | Misma semilla de tiempo | Añadir retardo aleatorio. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| FSM con 4+ estados y transiciones claras | 35 |
| Detección con línea de visión | 25 |
| Ataque con telegrafía | 20 |
| Oleada que abre la puerta | 20 |
