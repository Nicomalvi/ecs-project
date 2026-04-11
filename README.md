# Mi forma de practicar C#, ECS, data oriented design...
## ¿Qué es un Entity Component System?
Una arquitectura de programación **orientada a datos** que se apoya más en **composition** antes que inheritance.

Muy utilizada en juegos y simulaciones, lo que la convierte en una caja de arena ideal para ver sus ventajas en 
la práctica de una manera interesante. 

Todo, desde la arquitectura ECS en sí hasta el uso de **sparse sets**, **paginación**, fue implementado **desde cero** 
y comparado con alternativas de las que voy a hablar para poder entenderlo al máximo y lograrlo de una forma modular, reutilizable y óptima.

Más adelante veremos pros y contras de la arquitectura en sí y de las decisiones propias que tomé diferentes a los modelos más comunes.
## ¿Por qué un juego utilizaría ECS?
### Patrón de diseño
Imaginemos que existen 2 entidades en mi juego que hace gran uso de **inheritance**: Un ser humano, una espada. 
El ser humano tiene clase **ALIVE** mientras que la espada tiene clase **ITEM**, ambas clases hijas de la clase **ENTITY**.
¿Qué hago si quiero crear un ser vivo que puedo llevar en mi inventario? ¿Creo otro hijo de **ENTITY** llamado **LIVING_ITEM**?
¿O éste sería un hijo de **ALIVE**? ¿Y si quiero que dichos items puedan perder esa vida? ¿O que los humanos ganen esa portabilidad?

Con ECS esta situación requiere menos atención: una entidad es solo un **ID**. Lo que "es" esa entidad lo definen sus componentes. Un ser 
humano tiene **ALIVE** y **POSITION**. Una espada tiene **ITEM** y **WEIGHT**. Un ser vivo que puedo agarrar tiene **ALIVE**, **POSITION**, 
**ITEM** y **WEIGHT**. 

Comienza a nacer un patrón de diseño más sistémico: Paralizar un enemigo puede ser algo tán básico como quitarle el componente **AI_BEHAVIOUR**,
mientras que darle vida a una puerta es tan simple como sumarle el mismo. Un enemigo se vuelve inmortal al perder el componente **HEALTH**, y así.

### Performance
Otra razón importante es el **rendimiento**. Si trabajamos con miles de entidades, recorrer objetos que están dispersos en la memoria genera 
normalmente varios **cache misses**: la CPU tiene que buscar cada dato en lugares distintos del heap, perdiendo ciclos en el proceso.

ECS agrupa los datos del mismo tipo de forma **contigua en memoria**, minimizando estos cache misses y permitiendo que la CPU 
trabaje de forma mucho más eficiente 

Ésto no es ninguna casualidad: el sistema ECS nace y evoluciona como respuesta al crecimiento de la latencia de 
RAM en comparación con la velocidad de las CPUs a lo largo de los años.

## ¿Cómo funciona un sistema ECS?
### Entities
Simplemente una ID. Representan cualquier **entidad que pueda interactuar con el mundo**, ya sea una pared, una moneda, un grupo de monedas, un vendedor,
un portal a otro nivel... Todos, en principio, son simplemente un número.
### Components
La verdadera diferencia entre cada entidad. Un componente se puede describir como la información que utilizamos para determinar **cómo** una entidad interactúa
con el mundo: Una puerta tiene los componentes **STRUCTURE**, **INTERACTABLE**, **POSITION** mientras que un perro tiene **HEALTH**, **POSITION**,
**AI**, **NAME** ...

Algunos componentes pueden ser simplemente una flag (**INTERACTABLE** es 1 o 0) mientras que otros pueden tener información más compleja (**POSITION** es
una tupla (x,y)).

Cada componente, en principio, es un array de tamaño **MAX_ENTITIES** donde la i-esima posición representa el valor asociado del componente con la entidad i.
(Ésto se puede optimizar de mil maneras distintas y es una de las cosas que más atención necesitó)
### Systems
Las **formas** que tienen los componentes para interactuar con el mundo. Normalmente existe un sistema por componente y el mismo es una función que actualiza los 
valores del componente asociado.

Por ejemplo, analicemos el pseudocódigo de un posible movement_system, que trabaja con los componentes wants_to_move, position, movement:
``` cs
movement_system(bool[] wants_to_move, (int,int)[] position, (int,int)[] movement){
  for (int i = 0; i<MAX_ENTITIES; i++){
    if (wants_to_move[i]){
      position[i] += movement[i];
    }
  }
}
```
