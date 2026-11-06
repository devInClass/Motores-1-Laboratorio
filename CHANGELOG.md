# Changelog

Todas las versiones relevantes del material del curso.

## [1.2.1] · 2026-11-06

### Añadido

- **Botón de pantalla completa para WebGL** (`FullscreenToggle`): se crea solo
  en el navegador, alterna `Screen.fullScreen` y actualiza su etiqueta
  (PANTALLA COMPLETA / VENTANA). En el editor se prueba en `Lab_07_UI`.
- **Pausa automática al perder el foco** en Android y WebGL: `GameFlow` escucha
  `OnApplicationPause` y `OnApplicationFocus`, detiene el tiempo, muestra el
  menú de pausa y silencia el audio. Al volver, el jugador decide cuándo seguir.
- Pruebas TC-45 y TC-46 (50 automatizadas en total).

### Cambiado

- `PauseMenu` expone `Pause()` además de `Toggle()` y controla
  `AudioListener.pause` al pausar y reanudar.

## [1.2.0] · 2026-11-06

### Añadido

- **Soporte Android y WebGL**: capa de entrada virtual (`VirtualInput`),
  joystick y botones táctiles que se crean solo en móvil (`MobileControls`),
  y prueba en escritorio dentro de `Lab_07_UI`.
- **Cámara adaptable**: los límites se calculan con el ancho visible real
  (`camera.aspect`), así un teléfono 20:9 no muestra el vacío del nivel.
- **Área segura** para el HUD (notch y barra de gestos).
- **`PlatformBootstrap`**: 60 FPS, sin vsync y pantalla encendida en móvil.
- Menús **5. Configurar Android y WebGL**, **6. Build Android (APK)** y
  **7. Build WebGL**, con ARM64 + IL2CPP y Brotli con *decompression fallback*.
- Guía [`docs/08-multiplataforma.md`](docs/08-multiplataforma.md).
- Pruebas TC-41 a TC-44 (48 automatizadas en total).

### Cambiado

- `PlayerInput` suma teclado + entrada táctil y desactiva el ratón en móvil,
  para que tocar la pantalla no dispare sin querer.
- El audio de `Resources` pasa de PCM a **Vorbis** (builds más ligeros).

## [1.1.0] · 2026-11-06

### Añadido

- **Blend tree 1D** de locomoción (idle ↔ run por `Speed01`).
- **Blend tree 2D** de apuntado: 5 direcciones (arriba, diagonal arriba, frente,
  diagonal abajo, abajo) × de pie/corriendo, con `AimAngle` y `Speed01`.
- 20 sprites de apuntado generados (`player_aim_*`).
- Módulo **M11 · VFX y shaders** (sesión 35, 21 de abril): laboratorio
  `Lab_10_VFX`, shader `SpriteFlash` con flash de daño y dissolve, y dos
  prefabs de partículas (`VFX_Chispas`, `VFX_Explosion`).
- Pruebas TC-37 (balas), TC-38 (ángulo de apuntado), TC-39 (cartel) y
  TC-40 (estructura del Animator).

### Corregido

- **Balas atoradas o con velocidad irregular:** se instanciaban como hijas del
  jugador y heredaban su movimiento. Ahora viven en un contenedor en la raíz.
- **Cartel de instrucciones invisible:** el canvas estaba en World Space y
  quedaba detrás de los sprites. Ahora es Screen Space Overlay.
- **Personaje "congelado" al disparar:** el disparo usaba un trigger que
  bloqueaba la pose. Ahora el apuntado es un blend tree continuo.

### Cambiado

- El calendario gana una sesión de VFX/shaders; el Parcial 2 pasa al
  viernes 23 de abril y M10 se comprime a dos sesiones.

## [1.0.0] · 2027-01-11

Primera versión lista para el semestre enero–mayo 2027.

### Añadido

- Juego de referencia completo: Menú, `Nivel1_Cantina` y `Nivel2_Jefe`.
- 9 laboratorios por tema (`Lab_01_Sprites` … `Lab_09_Bugs`).
- Framework 2D: jugador con coyote time y salto variable, combate con hitboxes,
  pool de proyectiles, IA de enemigos con FSM, jefe de 3 fases, cámara con
  límites y shake, HUD, pausa, checkpoints y flujo de escenas.
- 36 pruebas automatizadas (EditMode y PlayMode) y catálogo de pruebas manuales.
- Cacería de 4 bugs con tests que se activan con el define `BUG_HUNT`.
- Herramientas de editor: configuración de capas, importador de arte,
  generador de escenas y validador.
- Generadores de arte y audio placeholder en Python.
- Documentación: calendario de 42 sesiones, guía docente, rúbricas,
  especificación de arte, flujo de Git y solución de problemas.

### Notas

- Requiere Unity 6000.3.10f1 (URP 2D, Input Manager clásico).
- El arte es placeholder generado; se espera que cada alumno lo reemplace.
