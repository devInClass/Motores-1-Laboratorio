# Catálogo de casos de prueba

Dos tipos de prueba:

- **A (automatizada):** la ejecuta Unity en el Test Runner. Si el nombre empieza por `TC-`, es un test real.
- **M (manual):** la ejecuta el alumno en la escena y anota el resultado en `plantillas/reporte-pruebas.md`.

Los IDs automáticos coinciden con el nombre del método: `TC07_FirstShot_IsAllowed` → `TC-07`.

---

## Pruebas automatizadas (50)

### Vida y daño · `Tests/EditMode/HealthTests.cs`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-01 | `Damage_ReducesCurrent` | Restar vida correctamente. |
| TC-02 | `Damage_ToZero_RaisesDiedExactlyOnce` | La muerte ocurre una sola vez y la vida no baja de 0. |
| TC-03 | `Damage_AfterDeath_ReturnsFalse` | No se puede dañar a un muerto. |
| TC-04 | `Heal_DoesNotExceedMax` | Curar no pasa del máximo. |
| TC-05 | `ZeroDamage_IsIgnored` | El daño 0 no dispara eventos. |
| TC-06 | `Normalized_ReflectsCurrentHealth` | La fracción 0–1 es correcta. |

### Armas · `Tests/EditMode/WeaponTests.cs`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-07 | `FirstShot_IsAllowed` | El primer disparo sale. |
| TC-08 | `FireRate_BlocksShotsDuringCooldown` | La cadencia bloquea disparos seguidos. |
| TC-09 | `EachShot_ConsumesAmmo` | Cada disparo gasta una bala. |
| TC-10 | `EmptyMagazine_StartsReload` | Al vaciar el cargador empieza la recarga. |
| TC-11 | `Reload_FinishesAfterReloadTime` | La recarga termina en el tiempo indicado. |
| TC-12 | `InfiniteAmmo_NeverReloads` | Munición infinita nunca recarga. |
| TC-13 | `Refill_RestoresMagazine` | Recargar manualmente deja el cargador lleno. |

### Pool y cooldowns · `Tests/EditMode/PoolTests.cs`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-14 | `Prewarm_CreatesObjectsUpFront` | El precalentado crea objetos al inicio. |
| TC-15 | `Get_ReusesReleasedObject` | Se recicla el mismo objeto. |
| TC-16 | `Get_WithoutIdle_CreatesNew` | Si no hay libres, crea uno. |
| TC-17 | `Release_Null_DoesNotThrow` | Liberar `null` no rompe. |
| TC-18 | `Cooldown_BlocksUntilDurationPasses` | El cooldown respeta su duración. |

### Partida · `Tests/EditMode/GameSessionTests.cs`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-19 | `Reset_StartsWithThreeLivesAndZeroScore` | Estado inicial correcto. |
| TC-20 | `AddScore_Accumulates` | Los puntos se suman. |
| TC-21 | `LoseLife_ReturnsFalseWhenNoLivesLeft` | La última vida no permite reaparecer. |
| TC-22 | `SetCheckpoint_MarksHasCheckpoint` | El checkpoint se guarda. |
| TC-23 | `Reset_ClearsCheckpointAndBoss` | Reiniciar limpia el estado. |

### Salud del proyecto · `Tests/EditMode/ProjectSanityTests.cs`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-24 | `RequiredLayersExist` | Están las 8 capas del curso. |
| TC-25 | `RequiredSortingLayersExist` | Están las 7 capas de dibujo. |
| TC-26 | `PlayerTagExists` | Existen los tags `Player` y `Enemy`. |
| TC-27 | `BuildSettings_StartWithMenuAndLevels` | El build empieza por Menú, Nivel 1 y Nivel 2. |
| TC-28 | `AllCourseScenesExist` | Las 10 escenas clave existen. |
| TC-29 | `CharacterSprites_UseCourseImportSettings` | PPU 32, filtro Point, sin compresión. |
| TC-30 | `PrefabsExist` | Los 6 prefabs base existen. |

### Escenas y física · `Tests/PlayMode/`

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-31 | `TodasLasEscenasCarganSinComponentesFaltantes` | Las 13 escenas cargan sin *missing scripts*. |
| TC-32 | `ElJugadorQuedaApoyadoEnElSuelo` | El jugador no atraviesa el tilemap. |
| TC-33 | `ElNivelTieneHUD` | Hay HUD y menú de pausa. |
| TC-34 | `LaGravedadHaceCaerAlJugadorYElSaltoLoSube` | Gravedad y salto funcionan. |
| TC-35 | `LaBalaDelJugadorDaniaAlEnemigo` | Una bala quita vida. |
| TC-36 | `ElEnemigoPatrulla` | La IA se mueve sola. |

### Regresiones y estructura

| ID | Nombre | Qué verifica |
|---|---|---|
| TC-37 | `LasBalasNoHeredanElMovimientoDelJugador` | Las balas viven en la raíz y no se arrastran con el jugador (bug de disparo). |
| TC-38 | `ElAnguloDeApuntadoCubreLasCincoDirecciones` | 90°, 45°, 0°, −45° y −90°. |
| TC-38b | `ElAnguloSeMantieneEntreMenos90Y90` | El ángulo no se sale del rango del blend tree. |
| TC-39 | `ElCartelDeInstruccionesSeDibujaEnOverlay` | El cartel es Screen Space Overlay y tiene fuente (bug de texto invisible). |
| TC-40 | `ElAnimatorDelJugadorUsaDosBlendTrees` | Locomotion 1D y Aim 2D con 10 hijos. |
| TC-40b | `ExistenLosParametrosQueUsaElCodigo` | No falta ningún parámetro del Animator. |
| TC-41 | `LaCamaraSeAjustaAlAnchoVisibleDeCadaPantalla` | La cámara respeta 16:9 y 20:9. |
| TC-41b | `EnUnNivelMasEstrechoQueLaPantallaLaCamaraSeCentra` | Niveles angostos se centran. |
| TC-42 | `LosControlesTactilesSeConstruyenYResponden` | Joystick y botones escriben en `VirtualInput`. |
| TC-43 | `LaEntradaVirtualRegistraUnaSolaPulsacion` | Un toque = un salto. |
| TC-43b | `ResetLimpiaTodo` | Al cambiar de escena no queda entrada pegada. |
| TC-44 | `ElHUDUsaAreaSegura` | El HUD respeta el notch. |
| TC-45 | `ElBotonDePantallaCompletaExisteYResponde` | El botón de WebGL crea la UI y alterna sin errores. |
| TC-46 | `LaPausaAutomaticaDetieneElJuego` | Al perder el foco pausa, silencia el audio y CONTINUAR reanuda. |

---

## Pruebas manuales por módulo

Marca cada una en el reporte con **OK / FALLA / N/A** y una captura.

### M01 · Sprites

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-101 | PPU correcto | Selecciona cualquier sprite en `Art/Sprites/Personajes`. | Inspector muestra *Pixels Per Unit* = 32. |
| TC-102 | Filtro nítido | Haz zoom en la Game view. | Los píxeles se ven cuadrados, sin difuminado. |
| TC-103 | Pivote en los pies | Selecciona `player_idle_0` y mira el pivote azul. | Está en el centro inferior del sprite. |
| TC-104 | Escala coherente | Coloca jugador y enemigo en la misma escena. | Ambos miden ~1 unidad de ancho. |
| TC-105 | Sin compresión | En el inspector del sprite, mira *Compression*. | Dice *None*. |

### M02 · Animación

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-201 | Idle en bucle | Entra a `Lab_02_Animacion` sin tocar nada. | El personaje respira en bucle. |
| TC-202 | Transición a Run | Pulsa `D`. | Pasa de Idle a Run sin salto brusco. |
| TC-203 | Salto y caída | Pulsa `Espacio` y suelta. | Se ve Jump al subir y Fall al bajar. |
| TC-204 | Evento de disparo | Pulsa `J`. | La animación de disparo sale una vez, no en bucle. |
| TC-205 | Volteo | Cambia de dirección. | El sprite mira hacia donde camina y el arma queda delante. |
| TC-206 | Apuntado arriba | Mantén `↑` y dispara. | El brazo apunta arriba sin girar el cuerpo. |
| TC-207 | Correr apuntando | Corre y dispara en diagonal. | Se mezclan carrera y apuntado sin saltos bruscos. |

### M03 · Física

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-301 | Caída | Empieza el nivel. | El jugador cae y se apoya, sin vibrar. |
| TC-302 | Coyote time | Sal de una plataforma y pulsa salto justo después. | El salto se ejecuta. |
| TC-303 | Salto variable | Pulsa salto corto y luego largo. | El salto corto llega menos alto. |
| TC-304 | One-way | Salta desde abajo de una plataforma. | La atraviesas y aterrizas encima. |
| TC-305 | Bajar de plataforma | Sobre una one-way, pulsa `S` + salto. | La atraviesas hacia abajo. |
| TC-306 | Sin atravesar suelo | Corre contra la pared y salta repetidamente. | No atraviesas el tilemap. |

### M04 · Combate

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-401 | Daño con bala | Dispara a un enemigo. | Pierde 1 de vida y parpadea. |
| TC-402 | Invulnerabilidad | Recibe dos golpes seguidos. | Solo el primero quita vida. |
| TC-403 | Cuchillo | Acércate y pulsa `K`. | Daña solo en el rango corto. |
| TC-404 | No autodaño | Dispara pegado a una pared. | No te quitas vida. |
| TC-405 | Pool | Dispara 40 balas seguidas. | En la jerarquía no crecen los objetos indefinidamente. |

### M05 · IA

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-501 | Patrulla | Observa un enemigo lejos. | Camina de un lado a otro sin caer. |
| TC-502 | Detección | Acércate dentro del rango. | Se detiene, te mira y te persigue. |
| TC-503 | Línea de visión | Escóndete detrás de una caja. | Deja de disparar. |
| TC-504 | Ataque | Quédate a rango medio. | Dispara con cadencia, no en ráfaga infinita. |
| TC-505 | Muerte | Vacía su vida. | Se desactiva, da puntos y desaparece. |

### M06 · Niveles

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-601 | Cámara limitada | Camina hasta el borde del nivel. | La cámara se detiene, no muestra el vacío. |
| TC-602 | Parallax | Camina de izquierda a derecha. | El fondo se mueve más lento que el suelo. |
| TC-603 | Checkpoint | Toca la bandera, muere. | Reapareces en la bandera, con vida llena. |
| TC-604 | Puerta | Limpia la oleada. | La barrera desaparece. |
| TC-605 | Meta | Cruza la puerta final. | Se avanza de nivel. |

### M07 · Interfaz

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-701 | Vida en HUD | Recibe un golpe. | Se apaga un corazón. |
| TC-702 | Puntos | Mata un enemigo. | El marcador sube. |
| TC-703 | Pausa | Pulsa `Esc`. | El juego se congela y aparece el menú. |
| TC-704 | Reanudar | Pulsa `Continuar`. | El tiempo vuelve a correr. |
| TC-705 | Game over | Pierde las 3 vidas. | Aparece la pantalla final con puntos. |

### M08 · Audio y juice

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-801 | Disparo con sonido | Dispara varias veces. | Cada bala suena sin cortar la anterior. |
| TC-802 | Impacto | Golpea a un enemigo. | Hay sacudida de cámara breve. |
| TC-803 | Música | Entra al nivel. | La música suena en bucle a volumen bajo. |
| TC-804 | Sin saturación | Muchos sonidos a la vez. | No hay distorsión. |

### M11 · VFX y shaders

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-1101 | Flash visible | Mira al soldado con `_FlashAmount = 1`. | Se ve completamente blanco. |
| TC-1102 | Flash sutil | Mira al de `0.5`. | Se ve aclarado pero conserva el dibujo. |
| TC-1103 | Dissolve | Mira al granadero con `_DissolveAmount = 0.55`. | Pierde píxeles de forma irregular. |
| TC-1104 | Dissolve total | Sube `_DissolveAmount` a 1. | No queda nada visible. |
| TC-1105 | Partículas | Entra a `Lab_10_VFX`. | Se ven chispas y una explosión. |
| TC-1106 | Orden de dibujo | Observa las partículas. | Van delante del suelo y detrás del HUD. |
| TC-1107 | Rendimiento | Mira el Profiler con las partículas activas. | Se mantiene por encima de 60 FPS. |

### M09 · Jefe

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| TC-901 | Barra de jefe | Entra a `Lab_08_Jefe`. | Aparece la barra con nombre y fase. |
| TC-902 | Cambio de fase | Baja su vida al 66 % y 33 %. | Cambia de patrón y se anuncia. |
| TC-903 | Telegrafía | Observa antes de atacar. | Hay una animación/aviso antes del golpe. |
| TC-904 | Invocación | Llega a la fase 3. | Aparecen esbirros. |
| TC-905 | Victoria | Vacía su vida. | Explota, suena y aparece "Misión cumplida". |

---

## Cómo escribir tus propios casos

Formato mínimo:

```
ID: TC-MI-01
Título: El enemigo no dispara mientras salta
Precondición: enemigo con arma y plataforma alta
Pasos: 1) colocarlo en la plataforma 2) esperar 3 s
Esperado: no dispara en el aire
Resultado: OK / FALLA + captura
```

Un buen caso de prueba tiene **un** resultado esperado y se puede repetir sin adivinar.
