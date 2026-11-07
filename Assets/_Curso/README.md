# Assets/_Curso

Tu zona de trabajo. Nada de lo que hagas aquí modifica el juego de referencia.

```
_Curso/
├─ MiJuego/          # tu nivel, tus prefabs, tus scripts, tu arte
│  ├─ Escenas/
│  ├─ Prefabs/
│  ├─ Scripts/
│  ├─ Arte/
│  ├─ Animaciones/
│  └─ Audio/
├─ Entregas/         # tus reportes de laboratorio
└─ Bugs/             # cacería de bugs
   ├─ Scripts/       # aquí corriges
   └─ Tests/         # no tocar: son el juez
```

## Tres reglas

1. **No edites `Assets/_SlugLab/`.** Es el material del curso y el docente puede
   actualizarlo. Si necesitas cambiar un prefab, un script o un valor, **duplícalo**
   dentro de `MiJuego/` y trabaja sobre tu copia.
2. **No edites `Bugs/Tests/`.** Los tests son el criterio de evaluación; solo se
   corrige el código de `Bugs/Scripts/`.
3. **Tu juego vive en `MiJuego/`.** Todo lo que entregas tiene que estar ahí o en
   `Entregas/`.

## Primeros pasos

Están detallados en [`MiJuego/README.md`](MiJuego/README.md):

1. Duplica una escena de laboratorio a `MiJuego/Escenas/`.
2. Duplica el prefab del jugador a `MiJuego/Prefabs/` y reemplaza la instancia.
3. Pon tus dibujos en `MiJuego/Arte/` (se importan solos con PPU 32).
4. Añade tu escena a *Build Settings*.

## Nombres

- Carpetas y archivos en minúsculas, sin espacios: `mi_nivel.unity`, `player_garcia.prefab`.
- Los scripts en `PascalCase`: `MiDisparo.cs`.
- Si trabajas en equipo, añade el nombre al final para no chocar.
