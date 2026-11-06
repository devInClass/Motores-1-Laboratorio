# M06 · Niveles, cámara y flujo de escena

**Sesiones 24–27** · 10, 12, 17 y 19 de marzo de 2027
**Escena:** `Lab_06_Niveles` · **Entregable:** nivel con suelo, cámara limitada, parallax, checkpoint y puerta

## Objetivo

Montar un nivel que se sienta como un lugar: la cámara no se sale, el fondo da profundidad y el jugador tiene un punto de retorno justo.

## Conceptos

- **Tilemap + Grid + Tile Palette:** pintar colisiones con tiles.
- **Cámara ortográfica:** tamaño, límites y *look ahead*.
- **Parallax:** capas que se mueven a distinta velocidad.
- **Checkpoint:** guardar posición y curar.
- **Puerta / gate:** condición de avance (limpiar una oleada).
- **Meta:** cambiar de escena.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 24 | Tilemap | Pintar suelo, plataformas y huecos |
| 25 | Cámara | Límites y seguimiento suave |
| 26 | Parallax | Tres capas de fondo |
| 27 | Checkpoint y puerta | Nivel completo con retorno |

## Práctica guiada

1. Abre `Lab_06_Niveles` y camina hasta los extremos: la cámara debe detenerse.
2. Selecciona `Main Camera` y revisa `Min Bounds` / `Max Bounds`.
3. Cambia `Smooth Time` a `0.5` y a `0.05`: describe la diferencia.
4. Abre `Nivel1_Cantina` y observa las capas `Fondo`, `Parallax`, `Suelo`, `Props`, `Personajes`.
5. Toca el checkpoint, deja que te maten y comprueba dónde reapareces.
6. Limpia la oleada y mira cómo se abre la puerta.

## Reto para tu juego

Tu nivel debe tener: un hueco que se pueda saltar, una plataforma one-way, un checkpoint a mitad y una puerta que se abra al limpiar enemigos. La cámara nunca debe mostrar el vacío.

## Casos de prueba

`TC-601` a `TC-605`. Automatizados: `TC-31`, `TC-33`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| Cámara muestra el borde | Límites mal calculados | `Min = ancho/2`, `Max = ancho - ancho/2`. |
| Fondo se mueve igual que el suelo | Capas sin `ParallaxLayer` | Añadir el componente con factor 0.15/0.35. |
| Reaparece en el inicio | Checkpoint no guarda | `GameSession.SetCheckpoint`. |
| La puerta no abre | Spawner no detecta muertes | Suscribirse a `Damageable.Died`. |
| Suelo con huecos raros | Tiles sin collider | `Tile Collider Type = Grid`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Tilemap con colisiones correctas | 25 |
| Cámara limitada y suave | 25 |
| Parallax de 2+ capas | 20 |
| Checkpoint y puerta funcionales | 30 |
