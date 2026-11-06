# Flujo de trabajo con Git

El objetivo no es solo "guardar el proyecto", sino que cada alumno aprenda el flujo real de un estudio: ramas, commits pequeños y *pull requests*.

## Lo mínimo por sesión

```bash
git pull                       # traer cambios del repositorio base
git add .
git commit -m "M03: coyote time funcionando"
git push
```

Formato de mensaje: `M0X: qué hiciste`. Sin mensajes como "cambios", "update" o "final final".

## 1. Crea tu copia

**Opción A — fork (recomendada):** en GitHub pulsa **Fork** y clona *tu* fork:

```bash
git clone https://github.com/TU-USUARIO/slug-lab.git
cd slug-lab
git remote add upstream https://github.com/PROFE/slug-lab.git
```

**Opción B — plantilla:** el docente marca el repo como *Template*; pulsa **Use this template** y clona el tuyo.

## 2. Una rama por módulo

```bash
git checkout -b M03-fisica
# trabajar...
git push -u origin M03-fisica
```

Al terminar, abre un *pull request* hacia tu propia `main` y descríbelo:

```
## Qué hice
- Implementé coyote time y jump buffer.
- Ajusté la gravedad de caída a 1.9.

## Cómo probarlo
1. Abrir Lab_03_Fisica.
2. Saltar desde el borde de la plataforma.

## Casos de prueba
- [x] TC-302
- [x] TC-303
```

Aunque nadie revise el PR, el hábito importa: obliga a explicar el cambio.

## 3. Etiquetas de avance

Al terminar cada módulo:

```bash
git tag M03-terminado
git push --tags
```

La entrega final se etiqueta `entrega-final`.

## 4. Qué NO se sube

El `.gitignore` ya excluye `Library/`, `Temp/`, `Logs/`, builds y archivos de IDE. Si ves `Library/` en `git status`, algo está mal: no lo subas, pesa gigabytes y se regenera solo.

Sí se sube:

- `Assets/` completo (incluidos `.meta`).
- `ProjectSettings/`.
- `Packages/manifest.json` y `packages-lock.json`.
- `docs/`, `tools/`, `README.md`.

## 5. Los `.meta` son obligatorios

Unity identifica cada asset por un GUID que vive en el `.meta`. Si borras o ignoras los `.meta`, las referencias se rompen y las escenas pierden componentes.

> Nunca commitees una carpeta `Assets` sin sus `.meta`. Si Unity no los generó, abre el proyecto una vez y espera.

## 6. Conflictos en escenas

Los archivos `.unity` son YAML y se conflictúan feo. Reglas:

1. **Nunca** dos personas editan la misma escena a la vez.
2. Trabaja en escenas separadas (`MiJuego/NivelPropio.unity`).
3. Si hay conflicto: no lo resuelvas a mano. Elige una versión, vuelve a abrir Unity y rehaz el cambio pequeño.
4. Para evitar dolores: activa **Force Text** en `Edit → Project Settings → Editor → Asset Serialization` (ya viene así).

## 7. Arte grande y Git LFS

Si subes PSD o videos, instala Git LFS antes de agregarlos:

```bash
git lfs install
git lfs track "*.psd" "*.mp4" "*.wav"
git add .gitattributes
```

Los PNG de pixel art pesan poco; no necesitan LFS.

## 8. Recuperar el trabajo

| Situación | Comando |
|---|---|
| Quiero ver qué cambié | `git status` / `git diff` |
| Quiero deshacer un archivo | `git restore ruta/al/archivo` |
| Quiero volver a un commit anterior | `git log --oneline` y luego `git checkout <hash>` |
| Borré algo sin commitear | En Unity: `Edit → Undo`; en el sistema, busca en la papelera |
| Rompí todo y quiero empezar de nuevo | `git reset --hard origin/main` (¡pierdes lo no commiteado!) |

## 9. Checklist de entrega

- [ ] `main` actualizada y sin errores en consola.
- [ ] Todos los `.meta` incluidos.
- [ ] `reporte-pruebas.md` lleno.
- [ ] Tag `entrega-final`.
- [ ] Build o enlace a itch.io en el README de tu repo.
