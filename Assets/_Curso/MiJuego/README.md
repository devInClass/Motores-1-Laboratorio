# MiJuego

Tu juego vive aquí. Esta carpeta está **fuera** del material del curso: puedes
editarla, romperla y rehacerla sin afectar los laboratorios.

## Primeros 20 minutos

### 1. Crea tu escena

1. En el Project, ve a `Assets/_SlugLab/Scenes/Laboratorio/`.
2. Clic derecho en `Lab_03_Fisica` → **Duplicate**.
3. Renómbrala `MiNivel` y muévela a `Assets/_Curso/MiJuego/Escenas/`.

### 2. Crea tu prefab de jugador

1. Ve a `Assets/_SlugLab/Prefabs/Player.prefab`.
2. Clic derecho → **Duplicate**.
3. Muévelo a `Assets/_Curso/MiJuego/Prefabs/` y llámalo `Player_MiJuego`.
4. En `MiNivel`, borra el `Player` de la jerarquía y arrastra tu prefab.

> **Por qué:** si editas el prefab original, cambias todos los laboratorios y
> chocas con las actualizaciones del curso. Tu copia es tuya.

### 3. Pon tu arte

1. Copia tus PNG a `Assets/_Curso/MiJuego/Arte/`.
2. Unity los importa solos con **PPU 32**, filtro Point y pivote abajo-centro
   (usa subcarpetas `Fondos/`, `UI/`, `VFX/` o `Tiles/` si quieres pivote centrado).
3. Abre tu `Player_MiJuego`, selecciona el hijo `Visual` y asigna tus sprites.
4. Reemplaza los frames en el Animator.

### 4. Añade tu escena al build

`File → Build Settings → Add Open Scenes`. Sin esto, tu escena no existe en el
juego compilado.

### 5. Guarda y sube

```bash
git add .
git commit -m "M03: mi escena y mi personaje"
git push
```

## Estructura

| Carpeta | Qué va |
|---|---|
| `Escenas/` | `MiNivel.unity`, `MiJefe.unity` |
| `Prefabs/` | tus copias de Player, enemigos, jefe, balas |
| `Scripts/` | tus scripts propios |
| `Arte/` | tus sprites (PPU 32, pivote abajo-centro) |
| `Animaciones/` | clips y controllers propios |
| `Audio/` | SFX y música |

## Checklist antes de entregar

- [ ] El nivel se puede terminar.
- [ ] La cámara no muestra el vacío (ajusta `Level Min X` / `Level Max X`).
- [ ] El HUD refleja la vida real.
- [ ] La pausa funciona.
- [ ] Hay pantalla de victoria y de derrota.
- [ ] Todos los sprites son propios o están acreditados.
- [ ] El build corre fuera de Unity.

> No borres los placeholders del proyecto base: si los necesitas de referencia,
> están en `Assets/_SlugLab/Art/`.
