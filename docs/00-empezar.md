# Empezar

## 1. Instala lo necesario

| Herramienta | Versión | Dónde |
|---|---|---|
| Unity Hub | última | https://unity.com/download |
| Unity Editor | **6000.3.10f1** | Hub → Installs → Install Editor |
| Git | 2.40+ | https://git-scm.com |
| Editor de código | VS Code o Rider | Se conecta solo con Unity |

Al instalar Unity marca el módulo **Windows Build Support (IL2CPP)** si vas a generar ejecutables.

## 2. Trae el repositorio

```bash
git clone https://github.com/TU-USUARIO/slug-lab.git
cd slug-lab
```

Abre Unity Hub → **Add → Add project from disk** → selecciona la carpeta `slug-lab`.

La primera importación tarda entre 2 y 5 minutos. Es normal ver la barra de progreso en la esquina inferior derecha.

## 3. Comprueba que todo está bien

1. Abre `Assets/_SlugLab/Scenes/Juego/Menu.unity`.
2. Pulsa **Play**.
3. Deberías ver el título **SLUG LAB** y poder entrar al Nivel 1.

Si algo falla:

- **La escena está vacía:** ejecuta `Slug Lab → 3. Generar juego completo` y vuelve a abrirla.
- **Todo se ve rosa:** el proyecto se abrió sin URP. Revisa `Project Settings → Graphics` y que el asset `Assets/_SlugLab/Settings/UniversalRP.asset` esté asignado.
- **El personaje no se mueve:** revisa `Project Settings → Player → Active Input Handling` en **Input Manager (Old)**.

## 4. Recorre el proyecto

| Quiero… | Voy a… |
|---|---|
| Jugar el juego completo | `Scenes/Juego/` |
| Practicar un tema | `Scenes/Laboratorio/Lab_XX` |
| Ver cómo está hecho el jugador | `Scripts/Player/PlayerController.cs` |
| Entender el combate | `Scripts/Combat/` |
| Cambiar el jefe | `Prefabs/Jefe.prefab` + `Scripts/Boss/BossController.cs` |
| Ver qué se evalúa | `Tests/` y `docs/04-casos-de-prueba.md` |
| Trabajar en mi nivel | `Assets/_Curso/MiJuego/` |

## 5. Tu primer cambio (10 minutos)

La regla del curso: **no edites `Assets/_SlugLab/`**. Para experimentar, copia lo que necesites a `Assets/_Curso/MiJuego/`.

1. Duplica `Assets/_SlugLab/Prefabs/Player.prefab` y muévelo a `Assets/_Curso/MiJuego/Prefabs/Player_MiJuego.prefab`.
2. Duplica `Assets/_SlugLab/Scenes/Laboratorio/Lab_03_Fisica.unity` en `Assets/_Curso/MiJuego/Escenas/MiNivel.unity`.
3. En `MiNivel`, borra el `Player` y arrastra tu prefab.
4. En el componente `Player Controller` de **tu** prefab, cambia `Run Speed` de `6.5` a `10`.
5. Pulsa Play: deberías notar la diferencia.
6. Guarda y haz commit:

```bash
git add .
git commit -m "M03: mi escena y mi prefab de jugador"
git push
```

> Este es el flujo de todo el semestre: el material del curso se lee, no se toca; tu juego crece en `MiJuego/`.

## 6. Cómo se usan los laboratorios

Cada `Lab_XX` tiene un cartel con el objetivo. El flujo de trabajo es siempre el mismo:

1. Lee el README del módulo en `docs/modulos/`.
2. Abre la escena y juega.
3. Localiza lo que está incompleto o mal (el README te dice dónde mirar).
4. Corrige, prueba y **anota el caso de prueba** en `plantillas/reporte-pruebas.md`.
5. Haz commit.

## 7. Ejecutar las pruebas

En Unity: **Window → General → Test Runner → EditMode → Run All**.

En consola (CI o para revisar rápido):

```bash
Unity.exe -batchmode -nographics -runTests -testPlatform EditMode ^
  -projectPath . -testResults resultados.xml
```

Si un test falla, el mensaje te dice qué se esperaba. Los IDs (`TC-01`, `TC-32`…) están en el catálogo.

## 8. La cacería de bugs

1. `Project Settings → Player → Scripting Define Symbols` → añade `BUG_HUNT`.
2. Espera la recompilación y abre el Test Runner.
3. Verás 4 tests en rojo (`BUG01`…`BUG04`).
4. Corrige los archivos de `Assets/_Curso/Bugs/Scripts/` **sin tocar los tests**.
5. Cuando los 4 estén en verde, quita el define y documenta la solución.

## 9. Problemas frecuentes

Están todos en [`07-solucion-de-problemas.md`](07-solucion-de-problemas.md). Los tres más comunes:

- **"No se puede abrir el proyecto":** otra versión de Unity lo tiene abierto. Cierra el editor anterior.
- **"Los sprites se ven borrosos":** el filtro no es *Point*. Ejecuta `Slug Lab → 1. Configurar proyecto`.
- **"Missing script":** abriste una escena antes de que terminara la importación. Espera y reabre.
