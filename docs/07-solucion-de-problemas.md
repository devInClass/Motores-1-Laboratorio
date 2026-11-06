# Solución de problemas

## El proyecto no abre

| Síntoma | Causa | Solución |
|---|---|---|
| "Project is already open" | Otra instancia de Unity lo tiene abierto | Cierra el editor anterior o borra `Library/` (se regenera). |
| Se queda en "Importing…" horas | Disco lento o antivirus | Espera; excluye la carpeta del antivirus. |
| "Version mismatch" | Otra versión de Unity | Usa **6000.3.10f1** exactamente. |

## Se ve mal

| Síntoma | Causa | Solución |
|---|---|---|
| Todo rosa | Material URP perdido | `Project Settings → Graphics` → asigna `Assets/_SlugLab/Settings/UniversalRP.asset`. |
| Sprites borrosos | Filtro Bilinear | `Slug Lab → 1. Configurar proyecto` o cambia a Point. |
| Sprites gigantes o diminutos | PPU distinto de 32 | Ajusta *Pixels Per Unit* a 32. |
| Todo negro | Cámara sin `Camera` o sin `AudioListener` | Revisa `Main Camera` en la jerarquía. |
| Nada se dibuja | *Sorting Layer* inexistente | Ejecuta `Slug Lab → 1. Configurar proyecto`. |

## El jugador no responde

| Síntoma | Causa | Solución |
|---|---|---|
| No se mueve | `Active Input Handling` en *Input System Package* | `Project Settings → Player → Input Manager (Old)`. |
| Se mueve solo | Rigidbody con velocidad residual | Revisa `linearVelocity` en el inspector durante Play. |
| Atraviesa el suelo | Tilemap sin `TilemapCollider2D` o capa incorrecta | Revisa que el suelo esté en la capa `Ground` y tenga collider. |
| Vibra sobre el suelo | Collider demasiado grande o `Interpolate` mal | Ajusta el tamaño del `BoxCollider2D`; usa *Interpolate*. |
| Salta infinito | *Coyote time* mal reiniciado | Revisa que `_coyoteUntil` se ponga a 0 al saltar. |

## Animación

| Síntoma | Causa | Solución |
|---|---|---|
| No cambia de estado | Nombre del parámetro distinto | Compara el Animator y el `Animator.StringToHash`. |
| Se queda en el primer frame | El clip no tiene curvas o el *Animator* está en otro objeto | Revisa que el `SpriteRenderer` esté en el mismo objeto que el `Animator`. |
| Parpadea entre estados | Transiciones con condiciones contradictorias | Ajusta los umbrales o añade *exit time*. |
| El sprite no se voltea | Se escala el objeto equivocado | Voltea el hijo `Visual`, no el padre. |

## Combate

| Síntoma | Causa | Solución |
|---|---|---|
| La bala no daña | `hitMask` sin la capa del enemigo | En `Projectile`, marca `Enemy` en `Hit Mask`. |
| Te dañas a ti mismo | Capa del jugador incluida en la máscara | Quita `Player` de `Hit Mask`. |
| El enemigo muere de un golpe | `Hitbox` con `oncePerTarget = false` | Actívalo o desactiva el collider tras el golpe. |
| Las balas no se mueven | Rigidbody en *Static* | Ponlo en *Kinematic*. |

## Pruebas

| Síntoma | Causa | Solución |
|---|---|---|
| "Test assembly not found" | Define `UNITY_INCLUDE_TESTS` ausente | Vuelve a abrir el proyecto; lo añade el Test Framework. |
| Los tests de bugs no aparecen | Falta el define `BUG_HUNT` | `Project Settings → Player → Scripting Define Symbols`. |
| PlayMode falla solo en CI | `-nographics` y física | Ejecuta con `-batchmode` sin `-nographics`, o revisa tiempos. |

## Build

| Síntoma | Causa | Solución |
|---|---|---|
| "Scene not in build settings" | Falta la escena | `Slug Lab → 4. Configurar escenas de compilación`. |
| Build pesa 2 GB | `Library/` incluida | Revisa el `.gitignore` y no copies `Library`. |
| Pantalla negra al abrir el `.exe` | Escena inicial incorrecta | `File → Build Settings` → ordena `Menu` primero. |
| No compila en WebGL | Audio o hilos no soportados | Prueba *Windows* primero; WebGL requiere ajustes. |

## Cuando nada funciona

1. Cierra Unity.
2. Borra `Library/` (no pierdes assets, solo caché).
3. Abre el proyecto y espera la reimportación completa.
4. Ejecuta `Slug Lab → 1. Configurar proyecto` y luego `3. Generar juego completo`.
5. Si sigue roto, `git status` para ver qué cambiaste y `git diff` para revisarlo.

> **Antes de borrar `Library/`**, guarda una copia de tus escenas y prefabs propios. Están en `Assets/`, no en `Library/`, así que no se pierden; pero nunca está de más un commit.
