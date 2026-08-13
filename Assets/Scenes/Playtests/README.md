# Escena de prueba metroidvania

`MetroidvaniaMovementTest` es un blockout de gameplay separado de los niveles del
juego. Su objetivo es evaluar movimiento, combate y lectura del recorrido antes de
invertir tiempo en tiles, fondos, iluminacion o decoracion.

## Hipotesis de la prueba

- El salto corto y el salto sostenido permiten controlar aterrizajes de distinta distancia.
- El control aereo alcanza para corregir una trayectoria sin volver trivial cada salto.
- Atacar durante carrera, ascenso y caida no interrumpe la locomocion.
- Abajo + salto permite abandonar una plataforma marcada sin afectar el suelo solido.
- Los encuentros necesitan diferentes dimensiones segun el comportamiento enemigo.
- Una ruta que vuelve cerca del inicio comunica mejor progreso que un corredor lineal.

## Recorrido

1. **Start / Run / Attack:** espacio seguro para acelerar, frenar y atacar corriendo.
2. **Jump Metrics:** cuatro saltos ascendentes con distancia y altura progresivas.
3. **Lower Route:** recuperacion ante un salto fallido y encuentro horizontal con HellHound.
4. **Upper Route:** plataformas separadas y encuentro a distancia con Demon.
5. **Return Shortcut:** descenso escalonado que devuelve al jugador cerca del inicio.

El Skeleton de la primera zona prueba combate terrestre en un espacio amplio. La
ruta inferior no es un pozo de muerte: permite continuar, probar otro encuentro y
reincorporarse al recorrido superior.

## Que registrar

- Tiempo necesario para completar una vuelta.
- Cantidad y ubicacion de saltos fallidos.
- Saltos que exigen una pulsacion demasiado precisa.
- Momentos donde la camara tarda en mostrar el siguiente destino.
- Golpes recibidos sin entender de donde llegaron.
- Retrocesos, dudas o rutas que no se perciben como conectadas.
- Diferencias de sensacion entre teclado y joystick.
- Si el descenso por la plataforma verde responde igual con teclado, D-pad y stick.
- Si invertir la direccion en el aire permite corregir un error sin eliminar el compromiso del salto.
- Si el apice se percibe claro pero no genera una pausa flotante.
- Si aterrizar con una parte pequena del collider sobre un borde conserva `Grounded`.
- Si empujar contra una pared permite deslizarse hacia abajo sin quedar suspendido.
- Si una pulsacion breve y una de 0.2 segundos producen alturas diferentes y previsibles.
- Si la caida mas rapida permite encadenar plataformas sin perder demasiado control aereo.
- Si el shuriken permanece bloqueado antes de recoger el pickup celeste.
- Si cada lanzamiento apaga un indicador del HUD y el pickup amarillo restaura uno.
- Si intentar recoger una recarga con las tres cargas completas deja el pickup disponible.
- Si la zona muerta evita vibraciones al corregir pasos cortos sobre una plataforma.
- Si la anticipacion horizontal muestra suficiente recorrido sin sentirse brusca al girar.
- Si el seguimiento vertical permite leer el aterrizaje sin copiar cada pixel del salto.

Conviene probar primero sin ajustar valores. Despues de una vuelta, cambiar una sola
variable por vez y repetir el mismo recorrido.

## Criterios iniciales

- Una vuelta completa dura entre dos y cuatro minutos durante las primeras pruebas.
- Todos los saltos principales admiten margen de correccion.
- Los bordes no dejan al jugador suspendido con la animacion de caida.
- Fallar la ruta superior conduce a la ruta inferior, no a reiniciar la prueba.
- El Demon puede enfrentarse desde mas de una altura.
- La plataforma verde central se atraviesa con abajo + salto y vuelve a colisionar al caer debajo.
- El HellHound dispone de una recta legible y desaparece al alcanzar una pared.
- El atajo final se reconoce como regreso a una zona ya visitada.

## Regenerar la escena

La escena se puede reconstruir desde:

`Tools > Level Design > Regenerar escena metroidvania`

El comando reemplaza todo el contenido de la escena. Los ajustes que deban
conservarse tienen que incorporarse a `MetroidvaniaTestSceneBuilder`; los cambios
manuales dentro de la escena son temporales.

La escena figura deshabilitada en Build Settings porque es una herramienta de
desarrollo y no forma parte del flujo `Menu -> Level1 -> Level2`.

## Camara

`CameraController` usa seguimiento amortiguado independiente por eje, una zona muerta
y anticipacion horizontal. La referencia busca un comportamiento de metroidvania:
mantener estable el encuadre durante movimientos pequenos y mostrar mas espacio hacia
la direccion de avance.

`PlayerCameraBootstrap` conecta automaticamente toda escena que tenga objetos
etiquetados `MainCamera` y `Player`. Para limitar una habitacion, se puede asignar
un `Collider2D` en `Movement Bounds`; la camara considera su tamano ortografico al
calcular los bordes.

Valores iniciales recomendados:

| Ajuste | Valor |
| --- | ---: |
| Horizontal Smooth Time | `0.16` |
| Vertical Smooth Time | `0.22` |
| Dead Zone | `0.8 x 0.45` |
| Look Ahead Distance | `1.35` |
| Look Ahead Smooth Time | `0.25` |

## Referencias

- [Blockout](https://book.leveldesignbook.com/process/blockout)
- [Metrics](https://book.leveldesignbook.com/process/blockout/metrics)
- [Playtesting](https://book.leveldesignbook.com/process/blockout/playtesting)
