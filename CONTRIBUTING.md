# Cómo contribuir

Este repositorio es material de clase: se aceptan correcciones, mejoras y nuevos laboratorios, pero con criterio.

## Antes de abrir un issue

- Revisa [`docs/07-solucion-de-problemas.md`](docs/07-solucion-de-problemas.md).
- Comprueba que estás en Unity **6000.3.10f1**.
- Si es un bug del material, usa la plantilla de issue e incluye: escena, pasos, resultado esperado y captura.

## Propuesta de cambio

1. Abre un issue describiendo el problema o la mejora.
2. Crea una rama: `feat/lab-10-sigilo` o `fix/camara-limites`.
3. Cambios pequeños y con un solo propósito.
4. Si tocas código, corre las suites EditMode y PlayMode.
5. Si agregas un laboratorio, incluye: escena, README del módulo y casos de prueba.
6. Abre el PR y enlaza el issue.

## Estilo de código

- Identificadores en **inglés**; comentarios y documentación en **español**.
- Un archivo por clase; nombres en `PascalCase`; métodos en `PascalCase`; campos privados `_camelCase`.
- Nada de valores mágicos: usa `[SerializeField]` con `[Header]` y `[Tooltip]`.
- Lógica pura (sin `MonoBehaviour`) siempre que se pueda: es más fácil de probar.

## Contenido didáctico

- Cada módulo debe poder explicarse en 90 minutos.
- Los TODOs deben tener una pista, no la respuesta.
- Los casos de prueba deben ser reproducibles por alguien que no estuvo en clase.
- Si agregas un bug a la cacería, añade su test y documenta el síntoma.

## Licencia

Al contribuir aceptas que tu aporte se publique bajo la licencia MIT del repositorio.
