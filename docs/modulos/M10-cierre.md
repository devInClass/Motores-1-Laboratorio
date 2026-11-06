# M10 · Cierre: build, publicación y portafolio

**Sesiones 40–42** · 3, 5 y 7 de mayo de 2027
**Escena:** juego propio · **Entregable:** build publicado + ficha de proyecto

## Objetivo

Que el juego salga de Unity y llegue a alguien. Un proyecto que solo corre en el editor no cuenta como entregado.

## Conceptos

- **Build Settings:** escenas, plataforma, il2cpp vs Mono.
- **Resolución y calidad:** ventana, pantalla completa, vsync.
- **itch.io:** página, portada, GIF, créditos.
- **Portafolio:** qué mostrar y en qué orden.
- **Presentación:** 2 minutos, demo en vivo, qué falló y qué aprendiste.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 40 | Build | Generar `.exe` y probarlo en otra máquina |
| 41 | Publicación | Subir a itch.io con portada y GIF |
| 42 | Ensayo | Demo day y revisión cruzada |

## Práctica guiada

1. `File → Build Settings` → Windows → Build.
2. Abre el `.exe` fuera de Unity y juega de principio a fin.
3. Comprueba que el build no pesa más de lo necesario (`Library` no va incluida).
4. Crea la página de itch.io: título, descripción, controles, créditos.
5. Graba un GIF de 15 segundos con los tres momentos clave: correr, combatir, jefe.
6. Ensaya la presentación con un compañero que no conozca el juego.

## Reto final

Entrega: build jugable, repositorio limpio, `reporte-pruebas.md`, GIF y ficha de proyecto (`plantillas/entrega-final.md`).

## Casos de prueba

| ID | Caso | Esperado |
|---|---|---|
| TC-1001 | Build arranca en otra máquina | Menú y nivel jugables |
| TC-1002 | Resolución | Se ve bien en 1920×1080 y 1280×720 |
| TC-1003 | Sin errores | Consola vacía al jugar |
| TC-1004 | Cierre completo | Menú → nivel → jefe → victoria |

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| El build no arranca | Escena `Menu` fuera del build | Configurar Build Settings. |
| Pantalla negra | Cámara sin `Camera` o escena vacía | Revisar escena inicial. |
| Va lento en otra PC | Il2cpp sin optimizar o muchos objetos | Probar *Release*. |
| itch.io rechaza el ZIP | Archivo muy grande | Comprimir y quitar `Library`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Build reproducible | 30 |
| Página publicada con créditos | 25 |
| GIF y ficha de proyecto | 25 |
| Presentación de 2 minutos | 20 |
