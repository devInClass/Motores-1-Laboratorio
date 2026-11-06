# PENDIENTES · punto de reanudación

Nota para retomar el trabajo sin releer toda la conversación.

## Estado actual (verificado)

- Repositorio en `slug-lab/`, rama `main`, tag `v1.1.0`.
- Unity **6000.3.10f1** (URP 2D, Input Manager clásico).
- **34/34** pruebas EditMode y **8/8** PlayMode en verde.
- 13 escenas: menú, 2 niveles, 10 laboratorios.
- Animator del jugador con **blend tree 1D** de locomoción y **blend tree 2D**
  de apuntado (5 direcciones × de pie/corriendo).
- Módulo **M11 · VFX y shaders** con `Lab_10_VFX`, shader `SpriteFlash`
  (flash + dissolve) y partículas.
- 4 bugs de la cacería con tests que se activan con `BUG_HUNT`.

## Corregido en esta sesión

1. **Disparo atorado:** las balas se instanciaban como hijas del jugador y
   heredaban su movimiento. Ahora viven en un contenedor en la raíz.
2. **Cartel invisible:** era un canvas World Space detrás de los sprites.
   Ahora es Screen Space Overlay.
3. **Pose de disparo congelada:** el trigger `Shoot` bloqueaba la animación;
   se reemplazó por el blend tree de apuntado.

## Lo que falta (orden sugerido)

1. **Publicar en GitHub** (requiere credenciales del docente):
   ```bash
   cd slug-lab
   git remote add origin https://github.com/TU-USUARIO/slug-lab.git
   git push -u origin main --tags
   ```
   Marcar el repositorio como *Template* para que cada alumno cree su copia.

2. **Probar el blend tree en editor con gráficos** (yo solo pude validarlo con
   pruebas estructurales y en batchmode). Abrir `Lab_02_Animacion`, mantener
   `↑`, `↑+D`, `D`, `↓+D` en el aire y `↓`, y comprobar que la pose mezcla sin
   saltos. Ajustar posiciones del blend tree si alguna diagonal se ve rara.

3. **Revisar el calendario** contra el programa oficial: M11 ocupa la sesión 35
   (21 de abril), el Parcial 2 pasó al viernes 23 de abril y M10 quedó en dos
   sesiones (5 y 7 de mayo).

4. **Build de Windows** y prueba en una máquina sin Unity.

5. **Opcional:** CI con `game-ci` (necesita licencia de Unity como secreto).

6. **Opcional:** arte propio del docente para el juego de referencia.

## Cómo reanudar

- Abrir esta misma sesión desde el historial, **o** iniciar una nueva en el
  mismo workspace y decir: *“Continúa el proyecto slug-lab; lee PENDIENTES.md.”*
- Workspace: `C:\Users\Adrian\Documents\deepseek-harness\default-workspace`
- Proyecto: `slug-lab/`

## Reglas para no romper nada

- No borrar ni mover la carpeta `slug-lab/`.
- No ejecutar `git reset --hard` sin haber commiteado.
- Los `.meta` de Unity se versionan; no ignorarlos.
- Regenerar arte: `python tools/generar_arte.py` (sobrescribe placeholders).
- Regenerar escenas: menú **Slug Lab → 3. Generar juego completo**.

## Comandos de verificación

```bash
Unity.exe -batchmode -nographics -runTests -testPlatform EditMode -testResults edit.xml
Unity.exe -batchmode -nographics -runTests -testPlatform PlayMode -testResults play.xml
```
