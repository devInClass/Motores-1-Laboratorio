# M03 · Física 2D y movimiento del jugador

**Sesiones 11–14** · 5, 10, 12 y 15 de febrero de 2027
**Escena:** `Lab_03_Fisica` · **Entregable:** personaje que se mueve y salta "rico"

## Objetivo

Que el alumno entienda la diferencia entre **mover un objeto** y **simular física**, y que aprenda los tres trucos que hacen que un salto se sienta bien: gravedad asimétrica, *coyote time* y *jump buffer*.

## Conceptos

- `Rigidbody2D`: masa, gravedad, tipo (Dynamic/Kinematic/Static), interpolación.
- `Collider2D`: caja, cápsula, trigger, `Composite`.
- `Update` vs `FixedUpdate`: input en el primero, física en el segundo.
- Capas y `LayerMask`: qué colisiona con qué.
- `Physics2D.OverlapBox` para detectar suelo; `Raycast` para paredes y bordes.
- **Coyote time:** margen para saltar después de salir de una plataforma.
- **Jump buffer:** margen para registrar un salto antes de tocar el suelo.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 11 | Rigidbody y colliders | Hacer que el jugador caiga y choque |
| 12 | Capas y raycasts | Detectar suelo con `OverlapBox` |
| 13 | Coyote y buffer | Ajustar los tres valores del salto |
| 14 | Plataformas | One-way, móviles y huecos |

## Práctica guiada

1. Abre `Lab_03_Fisica` y cruza los huecos. ¿Qué se siente mal?
2. Selecciona el `Player` y revisa `Run Speed`, `Jump Velocity`, `Fall Gravity Multiplier`, `Coyote Time`, `Jump Buffer Time`.
3. Pon `Coyote Time` en `0` y prueba a saltar al borde. Después ponlo en `0.15` y compara.
4. Pon `Jump Buffer Time` en `0` y pulsa salto justo antes de aterrizar.
5. Sube a la plataforma móvil y comprueba que te lleva.
6. Abre `PlayerController.cs` y localiza `UpdateGrounded()` y `TryJump()`.

## Reto para tu juego

Ajusta los valores de salto para que **tú** puedas cruzar tu nivel sin frustrarte. Documenta los valores finales y por qué los elegiste.

## Casos de prueba

`TC-301` a `TC-306` en el catálogo. Automatizados: `TC-32` y `TC-34`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| Atraviesa el suelo | Mover el transform en `Update` | Usar `linearVelocity` en `FixedUpdate`. |
| Salto flotante | Gravedad baja | Subir `Fall Gravity Multiplier`. |
| Salto infinito | Coyote mal reseteado | `_coyoteUntil = 0` al saltar. |
| Se queda pegado a la pared | Fricción del material | Crear `PhysicsMaterial2D` con fricción 0. |
| La plataforma lo deja atrás | No se parenta | `OnCollisionEnter2D` + `SetParent`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Movimiento y salto funcionales | 30 |
| Coyote time y jump buffer explicados | 25 |
| Plataformas one-way y móvil | 25 |
| Sin errores de física en consola | 20 |
