# 🧙‍♂️ 2D Action Game – Unity Project

Este es un juego 2D desarrollado en **Unity (C#)** como parte de un proyecto académico.  
El objetivo principal fue **aplicar conceptos de Programación Orientada a Objetos (POO)** junto con **patrones de diseño** y el uso de **estructuras de datos** dentro de un contexto de videojuego.

---

## 🎮 Características del juego

- Control de personaje (movimiento, salto, ataque).
- Enemigos **melee** y **ranged** con comportamientos diferenciados.
- Sistema de **items** y **power-ups** (pociones de vida, potenciadores de velocidad, vino que reduce la velocidad).
- Sistema de **vida** con eventos para UI y control de estados.
- **Trampas** que detectan al jugador y aplican daño.
- Pantalla de **Game Over** y **Victory**.
- Sistema de **drops** aleatorios en enemigos derrotados.

---

## 🏗️ Estructura del Proyecto

### 📌 Herencia

Se utilizó herencia para **reutilizar código** en los enemigos:

- `EnemyController` → clase base con lógica común (vida, animaciones, detección).
- `MeleeEnemy` y `RangedEnemy` → heredan de `EnemyController` y definen sus comportamientos específicos (persecución, disparo de proyectiles).

👉 Esto permite **extender fácilmente** el juego con nuevos tipos de enemigos sin duplicar código.

---

### 📌 Interfaces

Se diseñaron varias interfaces para favorecer el **desacoplamiento**:

- `IDamageable` → implementada por Player y enemigos para recibir daño.
- `IEnemyContext` → proporciona información necesaria para que las estrategias de IA funcionen sin depender de la implementación concreta del enemigo.
- `IEnemyStrategy` → define cómo debe ejecutarse una estrategia de comportamiento.

👉 Esto permite que tanto enemigos como el jugador interactúen de forma **genérica y flexible**.

---

### 📌 Patrones de Diseño

1. **Strategy Pattern**

   - Implementado en el comportamiento de enemigos (`ChaseStrategy`, `IdleStrategy`, `ShootStrategy`).
   - Cada enemigo selecciona qué estrategia usar, evitando `if/else` gigantes.
   - Ejemplo: un enemigo melee persigue (`ChaseStrategy`) mientras que un enemigo ranged se queda quieto y dispara (`ShootStrategy`).

2. **Observer Pattern**

   - Implementado en el sistema de vida con la clase `Health`.
   - Dispara eventos (`OnLifeChanged`, `OnDeath`) que son escuchados por:
     - `PlayerMovement` → activa animaciones de daño/muerte.
     - `GameManager` → cambia el estado del juego.
     - UI → actualiza la barra de vida.

3. **Singleton Pattern**
   - Implementado en `GameManager` para tener un único punto de control del estado del juego.
   - Persiste entre escenas y se accede con `GameManager.Instance`.

---

### 📌 Estructuras de Datos

- **List\<T\>**:
  - En `GameManager`: para llevar registro de enemigos activos y contar los enemigos derrotados.
  - En enemigos (`possibleDrops`): para manejar ítems que pueden soltar al morir.

👉 Esto permitió manejar colecciones dinámicas y aplicar selección aleatoria para drops.

---

## ⚔️ Jugador

- Implementa `IDamageable`.
- Puede moverse, saltar y atacar.
- Recibe daño con knockback y animaciones.
- Aplica power-ups de velocidad con corrutinas (`ApplySpeedModifier`) que extienden duración si se toman consecutivamente.

---

## 🧩 Diagrama UML

![UML](./uml_diagram.png)  
_(Ejemplo de la relación entre EnemyController, MeleeEnemy, RangedEnemy y las estrategias)_

---

## 🚀 Cómo jugar

1. Descargar el repositorio.
2. Abrir con **Unity 2022.x o superior**.
3. Ejecutar la escena principal (`MainScene`).
4. Controles:
   - ⬅️ ➡️ → Moverse.
   - ␣ (Space) → Saltar.
   - `Z` → Atacar.

---

## 🏆 Condiciones de victoria y derrota

- **Game Over** → si el jugador pierde toda la vida.
- **Victory** → al cruzar el último portal (WinTrigger), mostrando en consola la cantidad de enemigos derrotados.

---

## 📖 Tecnologías y conceptos aplicados

- Lenguaje: **C#**
- Motor: **Unity**
- POO: **Herencia, Polimorfismo, Interfaces**
- Patrones de diseño: **Strategy, Observer, Singleton**
- Estructuras de datos: **List\<T\>**
- Corrutinas y físicas de Unity.

---

## ✨ Autor

Proyecto desarrollado por **[Tu Nombre]**  
👨‍💻 [Tu GitHub](https://github.com/tuusuario)
