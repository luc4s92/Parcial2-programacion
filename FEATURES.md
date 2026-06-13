# Features y Roadmap de Mejoras

Este documento acompania el README principal y sirve para planificar las iteraciones del proyecto. La idea es registrar que existe hoy, que se esta mejorando y que falta pulir para convertir el juego en un proyecto presentable de portfolio.

## Objetivo del proyecto

Pulir un juego 2D hecho en Unity/C# hasta dejarlo como una muestra jugable que comunique bien:

- Programacion orientada a objetos aplicada a gameplay.
- Patrones de disenio usados en un contexto real.
- Sistemas de audio interactivo.
- Iteracion de game feel, feedback y presentacion.

## Features actuales

### Jugador

- Movimiento horizontal.
- Salto.
- Ataque con espada.
- Sistema de vida mediante componente `Health`.
- Danio por contacto con enemigos y trampas.
- Knockback al recibir danio.
- Animaciones de movimiento, ataque, danio y muerte.
- Modificadores temporales de velocidad mediante items.

### Enemigos

- Clase base `EnemyController`.
- Enemigo melee con persecucion.
- Estrategias de comportamiento con `IEnemyStrategy`.
- Estados de danio y muerte.
- Registro de enemigos derrotados en `GameManager`.
- Sistema de drops aleatorios al morir.

### Items

- Pocion de vida.
- Power-up de velocidad.
- Vino como debuff de velocidad.
- Audio de pickup.

### Audio

- SFX de ataque, danio y muerte.
- SFX de enemigos.
- SFX de items.
- Sonido de portal.
- Voice lines por trigger.
- Mixer principal.
- Transiciones con snapshots.
- Feedback de vida mediante low-pass.

### Flujo de juego

- Menu principal.
- Level1.
- Level2.
- Pantalla de derrota.
- Pantalla de victoria.
- `GameManager` persistente entre escenas.
- Sistema de victoria/derrota.

## Iteraciones de mejora

### Iteracion 1: muerte y audio de derrota

Estado: en progreso.

Objetivo:

- Centralizar la derrota en `GameManager`.
- Evitar cargas duplicadas de `LoserScreen`.
- Permitir que la animacion y el audio de muerte se reproduzcan antes del cambio de escena.
- Preparar un campo especifico para sonido de muerte del jugador.

Cambios esperados:

- `PlayerMovement` solo maneja animacion/audio local de muerte.
- `GameManager` espera antes de cargar `LoserScreen`.
- `PlayerAudio` incluye `deathSFX`.
- Si no hay audio de muerte asignado, se usa un fallback.

Pendiente:

- Asignar un sonido especifico de muerte en el prefab del player.
- Probar muerte en `Level1` y `Level2`.
- Confirmar que no se corta el audio al cambiar de escena.

### Iteracion 2: movimiento del jugador

Estado: pendiente.

Objetivo:

Mejorar el game feel del personaje para que el movimiento sea mas fluido, justo y agradable.

Mejoras propuestas:

- Aceleracion y desaceleracion horizontal.
- Coyote time.
- Jump buffer.
- Salto variable segun cuanto tiempo se mantiene presionado el boton.
- Mejor deteccion de suelo.
- Separar input, movimiento y animacion para reducir acoplamiento.

Criterios de prueba:

- El jugador responde rapido sin sentirse rigido.
- Saltar cerca del borde de una plataforma se siente justo.
- Saltar apenas antes de tocar el piso funciona.
- La animacion acompania el movimiento sin cortes raros.

### Iteracion 3: enemigos y combate

Estado: pendiente.

Objetivo:

Hacer que los enemigos se sientan mas consistentes y que el combate sea mas legible.

Mejoras propuestas:

- Estados mas claros: idle, chase, hitstun, death.
- Mejor persecucion del enemigo melee.
- Evitar empujes o contactos injustos.
- Ajustar knockback del enemigo.
- Mejor feedback al golpear con espada.
- Revisar hitboxes y triggers.

Criterios de prueba:

- El enemigo no se queda pegado de forma rara al player.
- El jugador entiende cuando golpeo y cuando recibio danio.
- La muerte del enemigo no duplica eventos ni drops.

### Iteracion 4: audio y feedback

Estado: pendiente.

Objetivo:

Mejorar la claridad sonora y reforzar el estado del juego mediante audio.

Mejoras propuestas:

- Diferenciar swing de espada y golpe exitoso.
- Balancear volumen entre musica, SFX y voces.
- Revisar snapshots del mixer.
- Ajustar low-pass por vida baja.
- Evitar sonidos duplicados.
- Agregar variaciones de SFX si hace falta.

Criterios de prueba:

- Cada accion importante tiene respuesta sonora clara.
- La musica no tapa el combate.
- El cambio de estado por vida baja se percibe sin molestar.

### Iteracion 5: flujo de escenas y UX

Estado: pendiente.

Objetivo:

Dejar una partida completa estable desde menu hasta victoria o derrota.

Mejoras propuestas:

- Revisar orden de escenas en Build Settings.
- Evitar depender de indices cuando sea riesgoso.
- Agregar/revisar reinicio de partida.
- Volver al menu desde pantallas finales.
- Evaluar pausa.
- Limpiar logs de debug visibles durante gameplay.

Criterios de prueba:

- `Menu -> Level1 -> Level2 -> WinnerScreen` funciona.
- Morir en cualquier nivel lleva a `LoserScreen`.
- Reiniciar o volver al menu no deja estado viejo en `GameManager`.

### Iteracion 6: presentacion para portfolio

Estado: pendiente.

Objetivo:

Preparar el proyecto para mostrarlo publicamente.

Mejoras propuestas:

- Mejorar menu principal.
- Pulir pantallas de victoria y derrota.
- Agregar controles visibles.
- Preparar build jugable.
- Agregar capturas o GIFs.
- Actualizar README principal con informacion final.
- Documentar patrones usados y decisiones tecnicas.

## Backlog tecnico

- Revisar el warning de `PlayerMovement.attackHit` sin uso.
- Auditar referencias serializadas en prefabs.
- Unificar criterios de audio con null checks.
- Evaluar un `AudioManager` si el sistema crece.
- Revisar si `GameManager` debe resetear contadores al reiniciar partida.
- Limpiar comentarios y nombres inconsistentes.
- Revisar ortografia en nombres de assets, por ejemplo `sekeleton_death`.

## Checklist por iteracion

Antes de cerrar cada iteracion:

- Compila sin errores.
- Se prueba en Unity desde `Menu`.
- Se prueba el caso principal de la feature.
- Se revisa que no haya regresiones obvias.
- Se commitea en una rama separada.
- Se mergea a `master` solo despues de probar.
