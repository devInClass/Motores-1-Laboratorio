# PENDIENTES · punto de reanudación

Nota para retomar el trabajo sin releer toda la conversación.

## Estado actual

- Repositorio en `slug-lab/`, rama `main`, tag **v1.2.1**.
- Unity **6000.3.10f1** (URP 2D, Input Manager clásico).
- **38/38** pruebas EditMode y **12/12** PlayMode en verde (50 en total).
- 13 escenas: menú, 2 niveles, 10 laboratorios.
- Animator del jugador con blend tree 1D de locomoción y 2D de apuntado.
- Módulo **M11 · VFX y shaders** con `Lab_10_VFX` (flash, dissolve, partículas).
- **Soporte Android y WebGL**: entrada táctil, cámara adaptable, área segura,
  audio Vorbis y menús de build (5, 6 y 7).
- **Pantalla completa en WebGL** (`FullscreenToggle`) y **pausa automática** al
  perder el foco en Android/WebGL.

## Verificado en esta ronda

1. Compila sin errores y regenera las 13 escenas.
2. Suites EditMode (38) y PlayMode (12) en verde, incluida la pausa automática
   y el botón de pantalla completa.
3. La configuración de Android/WebGL se aplica: horizontal, ARMv7 + ARM64,
   IL2CPP, API 25 y Brotli con fallback.
4. **Build de Android: falla en este equipo** con `Burst internal compiler error:
   Acceso denegado` (`bcl.exe`, código 3). Es un bloqueo de permisos del
   compilador Burst, no del proyecto: la build llega hasta Gradle/IL2CPP.
   Probar en un equipo de laboratorio limpio o desactivar *Burst AOT Settings*.
5. **WebGL no se pudo compilar**: el módulo *WebGL Build Support* no está
   instalado en este equipo. El menú muestra un error claro si se intenta.

## Lo que falta

1. **Instalar WebGL Build Support** en Unity Hub y ejecutar
   **Slug Lab → 7. Build WebGL**.
2. **Probar en un teléfono real**: joystick, botones, notch, rendimiento y
   `adb logcat` sin errores. En el editor los controles se prueban en `Lab_07_UI`.
3. **Publicar en GitHub**:
   ```bash
   git remote add origin https://github.com/TU-USUARIO/Motores1.git
   git push -u origin main --tags
   ```
   Crear el repositorio vacío y marcarlo como *Template*.
4. **Revisar el calendario** contra el programa oficial: M11 ocupa la sesión 35
   (21 de abril), el Parcial 2 pasó al viernes 23 de abril y M10 quedó en dos
   sesiones (5 y 7 de mayo).
5. **Opcional:** compresión ASTC para sprites grandes en Android.

## Cómo reanudar

- Abrir esta misma sesión desde el historial, **o** iniciar una nueva en el
  mismo workspace y decir: *“Continúa el proyecto slug-lab; lee PENDIENTES.md.”*
- Workspace: `C:\Users\Adrian\Documents\deepseek-harness\default-workspace`
- Proyecto: `slug-lab/`

## Reglas para no romper nada

- No borrar ni mover la carpeta `slug-lab/`.
- No ejecutar `git reset --hard` sin haber commiteado.
- Los `.meta` de Unity se versionan; no ignorarlos.
- **Cerrar Unity antes de lanzar builds por batch.** Si queda un editor abierto,
  los procesos huérfanos bloquean `Library` y Unity aborta sin mensaje claro.
- Regenerar arte: `python tools/generar_arte.py` (sobrescribe placeholders).
- Regenerar escenas: menú **Slug Lab → 3. Generar juego completo**.

## Comandos de verificación

```bash
Unity.exe -batchmode -nographics -runTests -testPlatform EditMode -testResults edit.xml
Unity.exe -batchmode -nographics -runTests -testPlatform PlayMode -testResults play.xml
```
