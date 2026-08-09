# Sistema del jugador

Esta carpeta contiene el movimiento, input, animacion, audio y estados de gameplay
del jugador. El objetivo de la arquitectura es mantener separadas las decisiones de
gameplay de la implementacion de fisica y de la presentacion visual.

## Flujo general

```mermaid
flowchart LR
    InputSystem[Input System] --> InputReader[PlayerInputReader]
    PlayerMovement[PlayerMovement] --> StateMachine[StateMachine]
    StateMachine --> CurrentState[Estado actual]
    InputReader --> CurrentState
    CurrentState --> Locomotion[PlayerLocomotion]
    Locomotion --> Physics[PlayerMovementPhysics]
    Physics --> Rigidbody[Rigidbody2D]
    CurrentState --> Animation[PlayerAnimationController]
    Animation --> Animator[Animator]
```

`PlayerMovement` es el punto de entrada utilizado por Unity. Durante `Awake()` crea
los servicios y estados, conecta sus dependencias y construye la maquina de estados.

## Responsabilidades

| Script | Responsabilidad |
| --- | --- |
| `PlayerMovement` | Coordina los componentes de Unity, crea las dependencias y solicita cambios de estado. |
| `PlayerInputReader` | Traduce Input System a intenciones como mover, saltar y atacar. |
| `PlayerLocomotion` | Contiene las reglas compartidas de movimiento y salto. |
| `PlayerMovementPhysics` | Modifica el `Rigidbody2D`, gravedad, velocidad y deteccion de suelo. |
| `PlayerAnimationController` | Encapsula parametros y estados del Animator. |
| `PlayerDamageReaction` | Ejecuta feedback, knockback y muerte. |
| `PlayerSpeedModifier` | Mantiene la velocidad base y modificadores temporales. |
| `PlayerAudio` | Reproduce los sonidos locales del jugador. |
| `StateMachine` | Mantiene un unico estado activo y ejecuta su ciclo de vida. |

## Ejecucion por frame

`PlayerMovement.Update()` realiza este orden:

1. Actualiza la duracion de los modificadores de velocidad.
2. Comprueba el suelo mediante `PlayerLocomotion.UpdateGroundState()`.
3. Ejecuta `StateMachine.Tick()`.
4. El estado actual decide que comportamiento ejecutar y si debe cambiar de estado.
5. Actualiza el parametro de suelo del Animator.

Solo el estado actual recibe `Tick()`. Las reglas de `Grounded`, `Jump` y `Fall` no
se ejecutan simultaneamente.

## Ciclo de un estado

Todos los estados implementan `IState`:

```csharp
internal interface IState
{
    void Enter();
    void Tick();
    void Exit();
}
```

Al cambiar de estado, `StateMachine` ejecuta:

```text
estado actual.Exit()
estado actual = estado nuevo
estado nuevo.Enter()
```

Los estados no conocen directamente a la maquina. Reciben callbacks `Action`, como
`requestJump` o `requestFall`, que apuntan a metodos privados de `PlayerMovement`.
Esto reduce el acoplamiento entre cada estado y el coordinador.

## Estados actuales

### Grounded

- Restaura la gravedad normal al entrar.
- Ejecuta movimiento horizontal terrestre.
- Actualiza el jump buffer.
- Cambia a `Jump` cuando la fisica acepta el salto.
- Cambia a `Fall` cuando pierde el suelo.
- Permite comenzar un ataque.

### Jump

- Reproduce la animacion `jump` al entrar.
- Mantiene control horizontal aereo.
- Aplica salto variable segun si el boton sigue presionado.
- Cambia a `Fall` cuando la velocidad vertical llega a cero o se vuelve negativa.

### Fall

- Reproduce la animacion `fall` al entrar.
- Aplica gravedad de caida y limita la velocidad maxima.
- Mantiene control horizontal aereo.
- Permite saltar mediante coyote time o jump buffer.
- Cambia a `Grounded` al detectar el suelo.

### Attack

- Limpia el jump buffer.
- Activa el parametro de ataque y reproduce el sonido de espada.
- Frena horizontalmente al jugador.
- Termina mediante un Animation Event.
- Al terminar vuelve a `Grounded` o `Fall` segun la posicion del jugador.

### Knockback

- Detiene el movimiento anterior y aplica un impulso.
- Espera la duracion configurada.
- Al terminar vuelve a `Grounded` o `Fall`.

### Dead

- Activa la animacion y el sonido de muerte.
- Restaura la gravedad y detiene el movimiento.
- No ejecuta comportamiento durante `Tick()`.

## Transiciones principales

```mermaid
stateDiagram-v2
    [*] --> Grounded
    Grounded --> Jump: salto aceptado
    Grounded --> Fall: pierde el suelo
    Grounded --> Attack: ataque
    Jump --> Fall: velocidad vertical <= 0
    Fall --> Jump: coyote time o jump buffer
    Fall --> Grounded: aterriza
    Attack --> Grounded: termina en suelo
    Attack --> Fall: termina en aire
    Knockback --> Grounded: termina en suelo
    Knockback --> Fall: termina en aire
```

La maquina de estados de gameplay es la fuente de verdad. Actualmente `Jump` y
`Fall` reproducen directamente sus estados del Animator mediante `Animator.Play()`.
Por ese motivo no necesitan una transicion grafica entre ambos en el Animator.

## Salto asistido

`PlayerMovementPhysics` mantiene dos contadores:

- `coyoteCounter`: permite saltar durante un instante despues de abandonar el suelo.
- `jumpBufferCounter`: recuerda una pulsacion realizada poco antes de aterrizar.

`TryJump()` solo aplica el impulso cuando ambos contadores permiten el salto. Al
saltar, consume los contadores para evitar saltos duplicados.

El salto variable utiliza dos ajustes:

- `jumpCutMultiplier`: reduce la velocidad vertical cuando se suelta el boton.
- `lowJumpGravityMultiplier`: aumenta la gravedad si no se mantiene el salto.

La caida utiliza `fallGravityMultiplier` y limita la velocidad mediante
`maxFallSpeed`.

## Composicion

**Composicion** es el sustantivo y **compose** significa "componer" en ingles. No es
una clase ni un metodo especial de C#.

Este sistema usa composicion porque `PlayerMovement` construye objetos pequenos y
los conecta para formar el comportamiento completo del jugador. Por ejemplo:

```text
PlayerMovement
  contiene StateMachine
  contiene PlayerLocomotion
  contiene PlayerAnimationController
  contiene PlayerDamageReaction
```

Ninguna de esas clases hereda de `PlayerMovement`. Cada una aporta una capacidad y
el resultado final se obtiene combinandolas.

`PlayerMovement.Awake()` tambien funciona como **Composition Root**: es el lugar
donde se crean los objetos y se conectan sus dependencias.

## Principios de mantenimiento

- Los estados deciden transiciones, pero no modifican directamente el Rigidbody.
- `PlayerMovementPhysics` no lee input ni decide estados.
- `PlayerInputReader` no contiene reglas de gameplay.
- `PlayerAnimationController` no decide fisica.
- La locomocion compartida no debe copiarse dentro de cada estado.
- Los valores ajustables deben permanecer serializados en `PlayerMovement` y pasar a `Settings`.
- Las clases auxiliares permanecen `internal` porque son detalles de implementacion del juego.
- `IState` y `StateMachine` viven en `Core/StateMachine` y se reutilizan por composicion.

## Agregar un estado

1. Crear una clase `internal sealed` que implemente `IState`.
2. Inyectar solamente las dependencias que necesita mediante el constructor.
3. Recibir callbacks `Action` para solicitar transiciones.
4. Crear el estado en `PlayerMovement.Awake()`.
5. Agregar metodos privados de cambio de estado en `PlayerMovement`.
6. Delegar fisica, animacion y audio a sus clases correspondientes.
7. Probar entrada, salida, interrupciones y regreso a locomocion.

Un estado nuevo solo se justifica cuando agrega reglas de gameplay diferentes. Una
fase puramente visual puede resolverse dentro del Animator sin crear otro estado.
