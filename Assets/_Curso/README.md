# Assets/_Curso

Aquí vive tu trabajo. Nada de lo que hagas en esta carpeta modifica el juego de referencia.

```
_Curso/
├─ MiJuego/          # tu nivel, tus scripts, tu arte
│  ├─ Escenas/
│  ├─ Scripts/
│  ├─ Arte/
│  ├─ Animaciones/
│  └─ Audio/
├─ Bugs/             # cacería de bugs (Módulo 9)
│  ├─ Scripts/       # aquí corriges
│  └─ Tests/         # no tocar: son el juez
└─ Plantillas/
```

## Reglas

1. **No edites** `Assets/_SlugLab/` para tu entrega: es el material base. Si necesitas un cambio, haz una copia en `MiJuego/`.
2. Tus escenas siempre en `MiJuego/Escenas/`.
3. Tus sprites, con PPU 32 y pivote abajo-centro.
4. Nombres en minúsculas y sin espacios.

## Empezar tu nivel

1. Duplica `Assets/_SlugLab/Scenes/Laboratorio/Lab_03_Fisica.unity`.
2. Guárdala como `MiJuego/Escenas/MiNivel.unity`.
3. Añádela a `File → Build Settings`.
4. Reemplaza el `Player` por una copia con tu arte.
