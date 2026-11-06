# Evaluación

La nota final combina tres evidencias. No hay examen teórico: se evalúa lo que el alumno puede **hacer y explicar** frente a la computadora.

| Componente | Peso | Cuándo |
|---|---|---|
| Escenarios de práctica (9 laboratorios) | 25 % | A lo largo del curso |
| Cacería de bugs | 15 % | Módulo 9 (y recuperable hasta el final) |
| Parcial 1 (M00–M05) | 15 % | Sesión 23 · lunes 8 de marzo |
| Parcial 2 (M06–M08 + M11) | 15 % | Sesión 36 · viernes 23 de abril |
| Juego propio + reporte + demo | 30 % | Viernes 14 de mayo |

Para aprobar se necesita **al menos 60 % en el juego propio** y **los 4 bugs corregidos**. Los laboratorios se pueden recuperar hasta la entrega final.

---

## 1. Rúbrica de laboratorios (25 %)

Cada `Lab_XX` vale lo mismo. Se entrega el `reporte-pruebas.md` con los casos del módulo marcados y una captura o GIF por caso.

| Nivel | Descripción | Puntos |
|---|---|---|
| Excelente | Todos los casos pasan; el alumno explica qué cambió y por qué. | 100 |
| Bien | Todos los casos pasan, pero la explicación es superficial. | 85 |
| Suficiente | Pasan 2 de 3 casos; el resto está intentado y documentado. | 70 |
| Insuficiente | Menos de la mitad de los casos, o sin reporte. | 40 |
| No entregado | — | 0 |

---

## 2. Rúbrica de cacería de bugs (15 %)

Se activa el define `BUG_HUNT` y se corre el Test Runner. Los 4 tests deben quedar en verde **sin modificar los tests**.

| Criterio | Puntos |
|---|---|
| `BUG01` vida | 25 |
| `BUG02` cadencia | 25 |
| `BUG03` pool | 25 |
| `BUG04` puntaje | 25 |

Cada bug corregido suma solo si el alumno puede decir **cuál era el síntoma** y **qué línea lo causaba**. Se permite mirar apuntes; no se permite copiar la solución de un compañero sin explicación.

---

## 3. Parciales (15 % + 15 %)

Revisión individual de 10 minutos. El docente pide un cambio en vivo.

| Criterio | Puntos |
|---|---|
| El juego corre sin errores en consola | 20 |
| Juega y llega al objetivo del nivel | 20 |
| Localiza el script correcto cuando se le pide un cambio | 25 |
| Hace el cambio y lo prueba | 25 |
| Explica una decisión de diseño (por qué ese valor, esa capa, ese estado) | 10 |

**Parcial 1** cubre: sprites, animación, física, combate e IA.
**Parcial 2** cubre: tilemap, cámara, HUD, pausa y audio.

---

## 4. Juego propio (30 %)

Entrega: build jugable (`.zip` o itch.io), repositorio, `reporte-pruebas.md`, GIF de 15 s y ficha de proyecto.

### 4.1 Jugabilidad (10 pts)

| Nivel | Descripción |
|---|---|
| 10 | Se puede jugar de principio a fin: menú → nivel → jefe → victoria, sin bloqueos. |
| 8 | Llega al final pero hay algún momento confuso o sin *feedback*. |
| 6 | Se puede jugar, pero el jefe o el final no está cerrado. |
| 3 | Solo se puede caminar y disparar. |
| 0 | No compila o no arranca. |

### 4.2 Arte y animación propios (8 pts)

| Nivel | Descripción |
|---|---|
| 8 | Personaje, enemigos y jefe dibujados por el alumno, con idle/run/attack y coherencia de escala. |
| 6 | La mayoría es propio; alguna animación falta o está sin pulir. |
| 4 | Mezcla de placeholders y arte propio, sin criterio de escala. |
| 2 | Todo placeholder. |

Se valora la **coherencia** (PPU, pivotes, paleta) por encima del detalle. Un personaje simple bien animado vale más que uno complejo que se ve borroso.

Un *flash* de daño o un *dissolve* bien aplicado suma en este apartado; partículas sin criterio restan.

### 4.3 Código y arquitectura (6 pts)

| Nivel | Descripción |
|---|---|
| 6 | Separa datos (ScriptableObjects), lógica y presentación; sin `Instantiate` por bala. |
| 4 | Código funcional con alguna clase demasiado grande. |
| 2 | Todo en un solo script; valores mágicos por todas partes. |

### 4.4 Pruebas y documentación (4 pts)

| Nivel | Descripción |
|---|---|
| 4 | Reporte completo, casos propios además de los del curso, bugs documentados. |
| 3 | Reporte completo con los casos del curso. |
| 1 | Reporte incompleto. |

### 4.5 Presentación / demo day (2 pts)

2 minutos: qué es, qué se puede hacer, qué fue lo más difícil, qué harías con una semana más.

---

## 5. Entregables y fechas

| Entregable | Fecha | Formato |
|---|---|---|
| Laboratorio 1 | Sesión 6 | `reporte-pruebas.md` + commit |
| Laboratorio 3 | Sesión 14 | idem |
| Laboratorio 5 | Sesión 22 | idem |
| **Parcial 1** | Lun 8 mar | En vivo |
| Laboratorio 6 | Sesión 27 | idem |
| Laboratorio 7 | Sesión 31 | idem |
| **Parcial 2** | Vie 23 abr | En vivo |
| Cacería de bugs | Sesión 40 | 4 tests en verde |
| **Juego final** | **Vie 14 may** | Build + repo + reporte + GIF |

## 6. Política de uso de IA

Permitido: usar asistentes para entender un error, explicar una API o generar *boilerplate*.
No permitido: entregar un juego que no puedes explicar. En el parcial se pregunta por cualquier línea.

Si el alumno usa IA, debe anotarlo en el reporte: **qué pidió, qué obtuvo y qué cambió**. Eso también se evalúa (criterio de criterio propio).
