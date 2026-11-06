# M08 · Audio y *game feel*

**Sesiones 32–34** · 14, 16 y 19 de abril de 2027
**Escena:** juego propio · **Entregable:** golpes con impacto y sonido

## Objetivo

Que el alumno entienda que "se siente bien" no es magia: es una suma de detalles pequeños (sonido, sacudida, pausa, partículas) que se pueden medir y ajustar.

## Conceptos

- **AudioSource y AudioListener:** uno por escena, muchas fuentes.
- **Voces:** un pool de fuentes para que los SFX no se corten.
- **Mezcla:** volumen maestro, música al 0.2, SFX al 0.7.
- **Camera shake:** desplazamiento breve y decreciente.
- **Hitstop:** congelar el tiempo 50–80 ms en un impacto.
- **Partículas / VFX:** feedback visual inmediato.
- **Profiler:** comprobar que no generamos *garbage*.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 32 | SFX y mezcla | Disparo, salto y golpe con volumen adecuado |
| 33 | Shake, hitstop y VFX | Hacer que el golpe "se sienta" |
| 34 | Optimización | Profiler y build de prueba |

## Práctica guiada

1. Abre el Nivel 1 y escucha: disparo, impacto, música.
2. Abre `AudioManager.cs` y mira el pool de voces (`voiceCount`).
3. Dispara 10 veces rápido: los sonidos no deben cortarse.
4. Cambia `screenShake` del arma a `0.4` y siente la diferencia (y el abuso).
5. Abre `CameraFollow2D.Shake` y ajusta duración.
6. Abre el **Profiler** mientras disparas: vigila *GC Alloc*.

## Reto para tu juego

Añade sonido a disparo, salto, daño y muerte; una sacudida breve al recibir daño y *hitstop* al golpear al jefe. El volumen debe permitir escuchar la música.

## Casos de prueba

`TC-801` a `TC-804`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| Un solo sonido a la vez | Una única `AudioSource` | Pool de fuentes. |
| Todo suena saturado | Volúmenes sumados | Bajar maestro y usar `PlayOneShot`. |
| La cámara tiembla demasiado | Amplitud alta | 0.05–0.15 para impactos. |
| El juego se siente lento | `GC Alloc` por frame | Pool y evitar `new` en `Update`. |
| Sin audio en build | `AudioListener` duplicado | Uno solo por escena. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| 4 SFX integrados y mezclados | 30 |
| Shake e hitstop medidos | 25 |
| Sin cortes de audio | 20 |
| Profiler sin allocaciones por frame | 25 |
