# Calendario 2027 · Motores de Videojuegos I

**42 sesiones de 90 minutos.** Lunes, miércoles y viernes. Dos días de examen parcial y la entrega final una semana después de la última clase.

- Inicio: **lunes 11 de enero de 2027**
- Última sesión: **viernes 7 de mayo de 2027**
- Entrega final del proyecto personal: **viernes 14 de mayo de 2027**

> Los huecos del calendario (8 de febrero, 15 de marzo, del 22 de marzo al 2 de abril) corresponden a días festivos y al receso de Semana Santa.

## Mapa rápido

| Bloque | Sesiones | Fechas | Módulo |
|---|---|---|---|
| Arranque | 1–3 | 11, 13 y 15 de enero | M00 |
| Sprites | 4–6 | 18, 20 y 22 de enero | M01 |
| Animación | 7–10 | 25, 27 y 29 de enero · 3 de febrero | M02 |
| Física | 11–14 | 5, 10, 12 y 15 de febrero | M03 |
| Combate | 15–18 | 17, 19, 22 y 24 de febrero | M04 |
| IA de enemigos | 19–22 | 26 de febrero · 1, 3 y 5 de marzo | M05 |
| **Parcial 1** | 23 | **lunes 8 de marzo** | Repaso M00–M05 |
| Niveles y cámara | 24–27 | 10, 12, 17 y 19 de marzo | M06 |
| Interfaz y flujo | 28–31 | 5, 7, 9 y 12 de abril | M07 |
| Audio y *juice* | 32–34 | 14, 16 y 19 de abril | M08 |
| **Parcial 2** | 35 | **miércoles 21 de abril** | Repaso M06–M08 |
| Jefe final | 36–39 | 23, 26, 28 y 30 de abril | M09 |
| Cierre | 40–42 | 3, 5 y 7 de mayo | M10 |
| **Entrega final** | — | **viernes 14 de mayo** | Demo day |

## Detalle sesión por sesión

| # | Fecha | Tema | Entregable de la sesión |
|---|---|---|---|
| 1 | lun 11 ene | Presentación, Git y GitHub. Instalación de Unity. | Repo clonado y primer commit |
| 2 | mié 13 ene | Interfaz de Unity: escenas, GameObjects, componentes. | Escena "Hola Mundo" con sprite |
| 3 | vie 15 ene | Estructura de carpetas, prefabs, Build Settings. | Proyecto ordenado y jugable |
| 4 | lun 18 ene | Importación de sprites: PPU, filtro, compresión. | Hoja de personaje importada |
| 5 | mié 20 ene | Pivotes, recorte de hojas, orden de capas (*sorting*). | Personaje alineado al suelo |
| 6 | vie 22 ene | Atlas y organización de arte para animación. | Carpeta de arte propia |
| 7 | lun 25 ene | Clips de animación y *keyframes*. | Idle y Run de tu personaje |
| 8 | mié 27 ene | Animator: parámetros, transiciones, *exit time*. | Salto y caída conectados |
| 9 | vie 29 ene | Eventos de animación y sincronía con código. | Disparo sincronizado |
| 10 | mié 3 feb | Revisión de animaciones y *blend* de velocidad. | Set completo de animaciones |
| 11 | vie 5 feb | Rigidbody2D, colliders y gravedad. | Personaje que cae y choca |
| 12 | lun 10 feb | Capas, matriz de colisiones y *raycasts*. | Detección de suelo propia |
| 13 | mié 12 feb | Salto con *coyote time* y *jump buffer*. | Salto que se siente bien |
| 14 | lun 15 feb | Plataformas one-way, móviles y pendientes. | Laboratorio de física completo |
| 15 | mié 17 feb | Hitbox y hurtbox. Capas de ataque. | Cuchillo que daña |
| 16 | vie 19 feb | Daño, invulnerabilidad y *knockback*. | Feedback de golpe |
| 17 | lun 22 feb | Proyectiles y *object pooling*. | Disparo sin *garbage* |
| 18 | mié 24 feb | Armas como ScriptableObject. | Dos armas configurables |
| 19 | vie 26 feb | Máquina de estados de enemigo. | Enemigo que patrulla |
| 20 | lun 1 mar | Detección: línea de visión y rango. | Enemigo que persigue |
| 21 | mié 3 mar | Ataque enemigo y telegrafía. | Enemigo que dispara |
| 22 | vie 5 mar | Oleadas y puertas que se abren. | Zona con oleada |
| 23 | **lun 8 mar** | **Parcial 1**: demo jugable M00–M05 | Revisión individual |
| 24 | mié 10 mar | Tilemap y *Tile Palette*. | Suelo y plataformas con tiles |
| 25 | vie 12 mar | Cámara: seguimiento, límites y *look ahead*. | Cámara que no se sale |
| 26 | mié 17 mar | Parallax y composición de fondos. | Fondo en 3 capas |
| 27 | vie 19 mar | Checkpoints, puertas y flujo de nivel. | Nivel con checkpoint |
| 28 | lun 5 abr | Canvas, *anchors* y HUD escalable. | Barra de vida |
| 29 | mié 7 abr | Pausa, *timeScale* y menús. | Menú de pausa |
| 30 | vie 9 abr | Game over, victoria y carga de escenas. | Pantalla final |
| 31 | lun 12 abr | Guardado con `PlayerPrefs` y JSON. | Récord que persiste |
| 32 | mié 14 abr | Audio: fuentes, mezcla y SFX. | Disparo con sonido |
| 33 | vie 16 abr | *Game feel*: shake, hitstop, partículas. | Golpes con impacto |
| 34 | lun 19 abr | Optimización: *pooling*, *profiler*, builds. | Build de prueba |
| 35 | **mié 21 abr** | **Parcial 2**: nivel jugable M06–M08 | Revisión individual |
| 36 | vie 23 abr | Diseño de jefe: fases y patrones. | Jefe con 3 fases |
| 37 | lun 26 abr | Telegrafía y *readability*. | Ataques legibles |
| 38 | mié 28 abr | Barra de jefe y *feedback* de fase. | HUD de jefe |
| 39 | vie 30 abr | *Playtesting* y balance. | Ajustes por pruebas |
| 40 | lun 3 may | Build final, *itch.io* y créditos. | Build publicado |
| 41 | mié 5 may | Portafolio: GIFs, *devlog*, presentación. | Ficha de proyecto |
| 42 | vie 7 may | Ensayo de demo day y revisión cruzada. | Checklist final |
| — | **vie 14 may** | **Entrega final y demo day** | Juego + reporte + presentación |

## Reglas de entrega

1. Cada sesión termina con un **commit** con el formato `M0X: descripción corta`.
2. Los laboratorios se entregan con el **reporte de pruebas** (`plantillas/reporte-pruebas.md`) lleno.
3. Los parciales son **revisiones individuales de 10 minutos** frente a la computadora: se pide jugar, abrir un script y explicar una decisión.
4. La entrega final incluye build jugable, repositorio, reporte de pruebas y un GIF de 15 segundos.
