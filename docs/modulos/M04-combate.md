# M04 · Combate: hitboxes, daño y pooling

**Sesiones 15–18** · 17, 19, 22 y 24 de febrero de 2027
**Escena:** `Lab_04_Combate` · **Entregable:** disparo y cuchillo que dañan sin dañarte

## Objetivo

Separar **quién puede recibir daño** de **quién lo causa**, y entender por qué un juego 2D necesita capas de ataque, invulnerabilidad temporal y reciclaje de balas.

## Conceptos

- **Hurtbox:** el cuerpo que recibe daño (`Damageable`).
- **Hitbox:** la zona que daña (trigger + `LayerMask`).
- **Capas de ataque:** `PlayerAttack` y `EnemyAttack` para no autodañarse.
- **Invulnerabilidad (i-frames):** evita que un enemigo te mate en un frame.
- **Knockback:** empuje que hace legible el golpe.
- **Object pooling:** reutilizar balas en vez de crear/destruir.
- **ScriptableObject:** datos de arma separados del código.

## Plan de clase

| Sesión | Tema | Práctica |
|---|---|---|
| 15 | Hitbox y hurtbox | Cuchillo que solo daña en rango |
| 16 | Daño e i-frames | Recibir dos golpes seguidos |
| 17 | Proyectiles y pool | Disparar 40 balas sin *garbage* |
| 18 | Armas como datos | Crear una segunda arma |

## Práctica guiada

1. Abre `Lab_04_Combate` y dispara a un enemigo: observa el parpadeo y el retroceso.
2. Abre `Prefabs/Bala_Jugador` y revisa `Hit Mask`: debe incluir `Ground` y `Enemy`, **no** `Player`.
3. Abre `PlayerCombat.cs` y localiza `Fire()` y `PerformMelee()`.
4. Cambia `fireInterval` del arma a `0.5` y prueba.
5. Dispara 40 balas y mira la jerarquía: el pool debe reutilizar.
6. Abre `Arma_Ametralladora.asset` y crea una variante con más daño y menos cadencia.

## Reto para tu juego

Tu personaje debe tener un ataque a distancia y uno cuerpo a cuerpo, con daño distinto, y **nunca** debe poder dañarse a sí mismo. Añade una segunda arma configurable.

## Casos de prueba

`TC-401` a `TC-405`. Automatizados: `TC-01`–`TC-13` y `TC-35`.

## Errores frecuentes

| Error | Causa | Solución |
|---|---|---|
| Te quitas vida | Máscara incluye `Player` | Quitar la capa. |
| El enemigo muere de un golpe | Hitbox sin `oncePerTarget` | Activarlo. |
| Las balas no se mueven | Rigidbody *Static* | *Kinematic*. |
| Se dispara al revés | Offset del muzzle sin voltear | Girar el muzzle con `Facing`. |
| Baja el FPS | `Instantiate` por bala | Usar `ProjectilePool`. |

## Rúbrica rápida

| Criterio | Puntos |
|---|---|
| Disparo y melee funcionales | 30 |
| Sin autodaño y con i-frames | 25 |
| Pool reutilizando objetos | 25 |
| Segunda arma como ScriptableObject | 20 |
