# PENDIENTES · punto de reanudación

Nota para retomar el trabajo sin releer toda la conversación.

## Estado actual (verificado)

- Repositorio en `slug-lab/`, rama `main`, árbol limpio, tag `v1.0.0`.
- Unity **6000.3.10f1** (URP 2D, Input Manager clásico).
- **30/30** pruebas EditMode y **6/6** PlayMode en verde, comprobado también sobre un clon limpio.
- 2 niveles jugables + menú + 9 laboratorios + 4 bugs con tests.
- Documentación completa: calendario de 42 sesiones, guías, rúbricas y catálogo de pruebas.

## Lo que falta (orden sugerido)

1. **Publicar en GitHub** (requiere credenciales del docente):
   ```bash
   cd slug-lab
   git remote add origin https://github.com/TU-USUARIO/slug-lab.git
   git push -u origin main --tags
   ```
   Marcar el repositorio como *Template* para que cada alumno cree su copia.

2. **Revisar el calendario** contra el programa oficial de la materia y ajustar el reparto de sesiones en `docs/01-calendario.md`.

3. **Calibrar el andamiaje**: si el grupo tiene programación casi cero, convertir los retos de los laboratorios en TODOs dentro del código (hoy son retos descritos en el README, no huecos en los scripts).

4. **Build de Windows** y prueba en una máquina sin Unity.

5. **Opcional**: CI con `game-ci` (necesita licencia de Unity como secreto), y workflow de pruebas.

6. **Opcional**: arte propio del docente para el juego de referencia (hoy es placeholder generado).

## Cómo reanudar

- Abrir esta misma sesión desde el historial del GUI, **o** iniciar una nueva en el mismo workspace y decir:
  *“Continúa el proyecto slug-lab; lee PENDIENTES.md.”*
- Workspace: `C:\Users\Adrian\Documents\deepseek-harness\default-workspace`
- Proyecto: `slug-lab/`

## Reglas para no romper nada

- No borrar ni mover la carpeta `slug-lab/`.
- No ejecutar `git reset --hard` sin haber commiteado.
- Los `.meta` de Unity se versionan; no ignorarlos.
- Regenerar arte/audio sobrescribe placeholders: `python tools/generar_arte.py`.
- Regenerar escenas: menú **Slug Lab → 3. Generar juego completo** (o `-executeMethod SlugLab.EditorTools.SceneBuilder.BuildEverything`).

## Comandos de verificación

```bash
Unity.exe -batchmode -nographics -runTests -testPlatform EditMode -testResults edit.xml
Unity.exe -batchmode -nographics -runTests -testPlatform PlayMode -testResults play.xml
```
