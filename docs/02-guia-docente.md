# Guía docente

Esta guía asume sesiones de **90 minutos** con alumnado de Animación y **poca o ninguna experiencia en programación**. El repositorio ya trae el 80 % del código resuelto: la clase no consiste en teclear, sino en entender, probar y ajustar.

## Estructura de una sesión tipo

| Minutos | Bloque | Qué pasa |
|---|---|---|
| 0–10 | Repaso | Se proyecta el juego funcionando y se pregunta qué creen que cambió. |
| 10–25 | Concepto | Explicación corta con la escena del laboratorio abierta. Máximo 15 min de pizarra. |
| 25–65 | Práctica guiada | Todos abren `Lab_XX` y resuelven los retos del README. El docente circula. |
| 65–80 | Casos de prueba | Cada alumno ejecuta los casos del módulo y anota resultados. |
| 80–90 | Cierre | Commit, duda común, adelanto de la siguiente sesión. |

**Regla de oro:** nada de teoría sin una escena abierta. Si no se puede señalar en el *Game view*, no va en la clase.

## Cómo usar las escenas

Cada `Lab_XX` es un **banco de pruebas**, no un nivel para ganar. Los carteles indican qué mirar. Los alumnos deben:

1. Jugar la escena y describir qué se siente mal.
2. Abrir el inspector o el script implicado.
3. Cambiar **una cosa** y volver a jugar.
4. Anotar el antes/después en el reporte.

El juego de referencia (`Scenes/Juego/`) se usa como *meta*: al inicio de cada módulo se muestra la parte correspondiente y se pregunta "¿cómo está hecho esto?".

## Qué se espera en cada módulo

| Módulo | El alumno debe poder explicar | Señal de que no lo entendió |
|---|---|---|
| M01 Sprites | Por qué 32 PPU y por qué el pivote importa | Cambia el PPU hasta que "se vea bien" |
| M02 Animación | Qué es un parámetro, una transición y un blend tree | Copia el Animator sin poder justificar los estados |
| M03 Física | Diferencia entre `Update` y `FixedUpdate` | Mueve el Rigidbody en `Update` |
| M04 Combate | Hitbox ≠ hurtbox y para qué sirve la invulnerabilidad | Pone el daño en el `OnCollisionEnter` del jugador |
| M05 IA | Qué estados tiene su enemigo y cuándo cambia | Un solo `if` gigante dentro de `Update` |
| M06 Niveles | Qué hace la cámara y qué el parallax | Fondo pegado a la cámara, sin límites |
| M07 UI | Cómo se comunica el HUD con la vida | HUD que busca al jugador cada frame |
| M08 Audio | Por qué un SFX necesita voz propia y volumen | Un único `AudioSource` que se corta |
| M11 VFX | Qué hace un shader y qué un material | Cambia el color del sprite en vez del material |
| M09 Jefe | Qué es una fase y qué una telegrafía | Ataques instantáneos e ilegibles |
| M10 Cierre | Cómo se publica y se presenta un juego | Build sin créditos ni README |

## Los parciales

Son **revisiones individuales de 10 minutos**, no exámenes escritos:

1. El alumno abre su juego y juega 2 minutos.
2. El docente pide un cambio en vivo: "haz que el salto sea más corto", "agrega un enemigo aquí".
3. El alumno muestra dónde está el código, lo cambia y lo prueba.
4. Se califica con la rúbrica de `03-evaluacion.md`.

Esto evalúa comprensión real y evita el "funciona pero no sé por qué".

**Parcial 1** cubre M00–M05: sprites, animación, física, combate e IA.
**Parcial 2** cubre M06–M08 y M11: niveles, interfaz, audio y VFX/shaders.

## Errores que verás una y otra vez

| Síntoma | Causa habitual | Cómo guiarlo |
|---|---|---|
| El personaje atraviesa el suelo | Falta collider en el tilemap o el Rigidbody es kinematic | Preguntar: "¿quién tiene el collider, el dibujo o la física?" |
| La animación no cambia | El parámetro del Animator no coincide con el del script | Abrir Animator y comparar nombres letra por letra |
| Las balas se disparan hacia atrás | `transform.localScale.x` negativo + offset del cañón | Dibujar el pivote del muzzle en la escena |
| El enemigo cae del mapa | No hay detección de borde | Mostrar el `Gizmo` del raycast |
| El HUD no actualiza | Se suscribió al evento después de que ocurriera | Poner un `Debug.Log` en el evento |
| Todo se ve rosa | Material URP perdido | Reasignar `UniversalRP.asset` |
| El juego va lento | `Instantiate`/`Destroy` por cada bala | Abrir el Profiler y mostrar el *garbage* |
| El texto no se lee | Canvas en World Space detrás de los sprites | Usar Screen Space Overlay para los carteles |

## Cómo evaluar con el repositorio

- **Automático:** `Test Runner` da una nota objetiva de la lógica y la configuración.
- **Manual:** cada módulo tiene casos de prueba con pasos y resultado esperado (`04-casos-de-prueba.md`).
- **Revisión de código:** el alumno debe poder señalar qué línea cambió y por qué.

No califiques solo que el juego "se vea bonito". Un nivel feo con física sólida vale más que uno bonito que se rompe.

## Consejos de laboratorio

- Ten **dos escenas abiertas** como máximo; más confunde.
- Proyecta siempre el **Game view** y el **Inspector**, no el código.
- Si alguien se atora más de 10 minutos, pídele que explique en voz alta lo que cree que pasa: casi siempre se responde solo.
- Guarda 5 minutos finales para que **todos** hagan commit. Un alumno sin commit es un alumno sin entrega.
- Ten a mano una copia del proyecto en la versión "todo roto" para demostrar errores en vivo.

## Adaptación a 8 o 12 sesiones

Si el curso se recorta, conserva en este orden: M01 → M03 → M04 → M05 → M09. Lo que se puede recortar sin perder el hilo es M08 (audio) y parte de M10.
