# Plataformas atravesables

`OneWayPlatform.prefab` permite:

- saltar a traves de la plataforma desde abajo;
- aterrizar sobre su superficie desde arriba;
- descender manteniendo abajo y presionando salto.

## Uso

1. Arrastrar `OneWayPlatform.prefab` a la escena.
2. Alinear su posicion con la grilla de `1 x 1`.
3. Cambiar `Width In Tiles` en `ResizablePlatform2D` para ajustar el ancho.
4. Mantener la escala del `Transform` en `(1, 1, 1)`.

El componente sincroniza automaticamente el ancho del `SpriteRenderer` y del
`BoxCollider2D`. La superficie fisica es delgada y queda alineada con el borde
superior del tile.

La separacion vertical recomendada entre recorridos es de `6` tiles. Puede usarse
menos cuando golpear el techo o limitar el salto sea una decision intencional del
diseno.
