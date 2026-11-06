# Android y WebGL

El juego está preparado para las tres plataformas del curso: **Windows**, **Android** y **WebGL**. La lógica de movimiento, combate e IA es la misma; lo que cambia es la entrada, la cámara y el empaquetado.

## Lo que ya está resuelto en el proyecto

| Pieza | Archivo | Qué hace |
|---|---|---|
| Entrada virtual | `Scripts/Player/VirtualInput.cs` | Recibe las órdenes del táctil y las suma al teclado. |
| Joystick | `Scripts/Player/TouchJoystick.cs` | Escribe el movimiento en `VirtualInput`. |
| Botones | `Scripts/Player/TouchButton.cs` | Salto, fuego, cuchillo, recarga y pausa. |
| Controles en pantalla | `Scripts/Player/MobileControls.cs` | Construye la UI solo en móvil (o forzada en el editor). |
| Cámara adaptable | `Scripts/Level/CameraFollow2D.cs` | Usa el ancho visible real (`camera.aspect`). |
| Área segura | `Scripts/UI/SafeAreaFitter.cs` | Aparta el HUD del notch y la barra de gestos. |
| Arranque de plataforma | `Scripts/Core/PlatformBootstrap.cs` | 60 FPS, sin vsync, pantalla siempre encendida. |

En el editor los controles táctiles se pueden probar en **`Lab_07_UI`**: aparece un joystick y botones aunque estés en PC.

## Configurar

Menú **Slug Lab → 5. Configurar Android y WebGL**. Deja:

- Orientación **horizontal** (sin rotación a vertical).
- Android **ARMv7 + ARM64**, **IL2CPP**, API mínima 25.
- WebGL con **Brotli** y **Decompression Fallback** activado.

> Si el build de Android falla con `Burst internal compiler error: Acceso denegado`,
> desactiva **Project Settings → Burst AOT Settings → Enable Compilation**. Es un
> problema de permisos del compilador `bcl.exe`, no del proyecto; en un equipo de
> laboratorio limpio suele compilar sin tocarlo.

## Build Android

1. Instala el módulo **Android Build Support** en Unity Hub (ya viene con SDK, NDK y OpenJDK).
2. Menú **Slug Lab → 6. Build Android (APK)**.
3. El archivo queda en `Builds/Android/SlugLab.apk`.
4. Para probarlo en un teléfono:
   ```bash
   adb install -r Builds/Android/SlugLab.apk
   ```
5. Para publicar en Play Store necesitas **keystore** y subir un **AAB** (`Build App Bundle` en Build Settings), no un APK.

> El APK de depuración sirve para clase. Fírmalo solo si vas a distribuir.

## Build WebGL

1. Instala el módulo **WebGL Build Support** en Unity Hub (en este equipo **no está instalado**: el menú te avisará con un error claro).
2. Menú **Slug Lab → 7. Build WebGL**.
3. La carpeta `Builds/WebGL` se sube tal cual a itch.io.
4. En itch.io marca **This file will be played in the browser** y sube un ZIP con `index.html` en la raíz.

### Ajustes importantes para WebGL

- **Decompression Fallback** debe estar activo si no controlas las cabeceras del servidor (itch.io no las configura). Ya viene activado.
- El audio necesita un **gesto del usuario** para arrancar. El botón "Jugar" del menú lo cubre.
- No uses `System.Threading` ni `Application.Quit` como única salida: en el navegador no funcionan igual.
- El build pesa más que el de Windows; revisa `Builds/WebGL` antes de subirlo.

## Qué cambia en los scripts (y qué no)

**No cambia:** `PlayerController`, `PlayerCombat`, `EnemyBrain`, `BossController`, `Weapon`, `Damageable`, el HUD, los menús.

**Sí cambia:** solo `PlayerInput`, que suma teclado + `VirtualInput`. Por eso los botones nuevos no tocan la lógica del juego.

## Resoluciones y aspecto

| Dispositivo | Aspecto | Qué pasa |
|---|---|---|
| Monitor 16:9 | 1.78 | Comportamiento original. |
| Portátil 16:10 | 1.60 | La cámara ve un poco menos de ancho. |
| Teléfono 19.5:9 | 2.17 | La cámara se detiene antes en los bordes. |
| Teléfono 20:9 | 2.22 | Igual, con más margen lateral. |
| Tablet 4:3 | 1.33 | Se ve menos ancho; el nivel sigue centrado. |

Si un nivel es más estrecho que la pantalla, la cámara se centra para no mostrar el vacío.

## Rendimiento

- El juego apunta a **60 FPS** en móvil y desactiva vsync.
- Las balas usan *pool*; las partículas deben mantener un `maxParticles` razonable (120 en los prefabs actuales).
- En Android, revisa el **Profiler** con el dispositivo conectado. Si el arte crece, considera compresión **ASTC 4×4** para sprites grandes; el pixel art pequeño puede quedarse sin compresión.
- Evita `Instantiate`/`Destroy` por frame y `FindObjectOfType` dentro de `Update`.

## Problemas típicos

| Síntoma | Causa | Solución |
|---|---|---|
| El build de WebGL no aparece en el menú | Falta el módulo | Instalarlo desde Unity Hub. |
| WebGL carga y se queda en negro | Compresión sin fallback | Activar *Decompression Fallback*. |
| En Android no se mueve | `MobileControls` no se creó | Comprobar que `Application.isMobilePlatform` es true y que hay `PlayerInput`. |
| El personaje se mueve solo | Entrada virtual sin resetear | `VirtualInput.Reset()` al cargar escena (ya está en `PlatformBootstrap`). |
| El HUD queda bajo el notch | Falta `SafeAreaFitter` | Verificar el contenedor `SafeArea` del canvas. |
| La cámara muestra el vacío en 20:9 | Límites viejos | Regenerar escenas con **Slug Lab → 3**. |
| Android tarda mucho en compilar | IL2CPP + ARM64 | Es normal la primera vez; las siguientes son incrementales. |
| Android falla con "Burst internal compiler error" | Antivirus o permisos bloquean `bcl.exe` | Desactivar Burst AOT en Project Settings. |

## Checklist antes de publicar

- [ ] Probado en un teléfono real (no solo en el editor).
- [ ] Probado en horizontal y con el notch.
- [ ] Joystick y botones se sienten cómodos con los pulgares.
- [ ] Sin errores en consola del dispositivo (`adb logcat`).
- [ ] Build de WebGL subido y probado en el navegador.
- [ ] Créditos y controles visibles en la página de itch.io.
