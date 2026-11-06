# M00 · Arranque: herramientas, Git y estructura

**Sesiones 1–3** · 11, 13 y 15 de enero de 2027
**Escena:** ninguna · **Entregable:** repositorio propio con primer commit y escena "Hola Mundo"

## Objetivo

Que cada alumno tenga el proyecto abierto, sepa moverse por Unity y haya hecho su primer commit antes de tocar un solo sprite.

## Conceptos

- Qué es un motor de videojuegos y qué NO hace por ti.
- Proyecto, escena, GameObject, componente, prefab.
- `Transform`, `SpriteRenderer`, `Camera`, `Canvas`.
- Git: `clone`, `add`, `commit`, `push`; qué es un `.meta` y por qué no se borra.
- Estructura de carpetas del curso (`_SlugLab` vs `_Curso`).

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 1 | Presentación, Git, instalación | Clonar el repo, abrirlo, jugar el Nivel 1 |
| 2 | Interfaz y GameObjects | Crear escena propia con sprite y cámara |
| 3 | Prefabs y build settings | Convertir el objeto en prefab y hacer build |

## Práctica guiada

1. Abre `Assets/_SlugLab/Scenes/Juego/Menu.unity` y juega el Nivel 1 completo.
2. Crea `Assets/_Curso/MiJuego/Escenas/HolaMundo.unity`.
3. Añade un `SpriteRenderer` con `Art/Sprites/Personajes/player_idle_0.png`.
4. Añade una `Main Camera` y ajusta el fondo a un color oscuro.
5. Guarda, haz commit y push.
6. `File → Build Settings` y comprueba que tu escena aparece.

## Reto para tu juego

Crea `MiJuego/Escenas/MiNivel.unity` vacía, con cámara y un cartel de texto que diga cómo se llama tu juego. Ese será el lienzo de todo el semestre.

## Casos de prueba

| ID | Caso | Esperado |
|---|---|---|
| TC-001 | El proyecto abre sin errores | Consola limpia |
| TC-002 | El Nivel 1 es jugable | Se puede llegar a la meta |
| TC-003 | Tu escena carga en build | El `.exe` arranca en tu escena |

## Errores frecuentes

- Clonar sin Git LFS cuando hay archivos grandes.
- Subir `Library/` (revisa el `.gitignore`).
- Mover assets con el Explorador de Windows en vez de Unity (rompe los `.meta`).

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Repo clonado y proyecto abierto | 30 |
| Escena propia con sprite y cámara | 30 |
| Primer commit con mensaje correcto | 20 |
| Build de prueba funcionando | 20 |
