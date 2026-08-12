# Progresion del jugador

Esta carpeta contiene el estado de sesion que debe sobrevivir a un cambio de escena,
pero reiniciarse al comenzar una partida nueva.

## Shuriken

- `IShurikenInventory` define el contrato que consumen combate, pickups y UI.
- `ShurikenInventory` contiene las reglas de desbloqueo, consumo y recarga.
- `PlayerProgression` conserva una unica instancia durante la partida y la reinicia
  desde `MenuSystem.Play`.

El jugador no conoce pickups ni HUD. `PlayerShurikenCombat` recibe el contrato del
inventario y solo consume una carga cuando pudo obtener un proyectil del pool. El HUD
se suscribe al evento `Changed`, por lo que no consulta el estado en cada frame.

La capacidad inicial es de tres cargas. Una futura mejora de capacidad debe agregarse
al inventario sin modificar la maquina de estados ni el proyectil.
