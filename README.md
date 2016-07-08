# DistributedKeyValueStore-Akka.Net

## TO-DOs

* tamaño máximo para las claves y valores
* múltiples orquestadores, elección de master
* cantidad maxima de claves por nodo
* El nodo orquestador debe esperar a que el nodo de datos elegido para escribir confirme la escritura
	* lo que hacemos hoy es que el nodo de datos le responde directamente al cliente, esto puede ser un problema
* es necesario que el server exponga una API cli/rest? por ahora no lo necesitamos porque ntro cliente es un actor
* refactors
	* no hardcodear la cantidad de routees en SearchValuesActor
	* mejorar la representacion del criterio de busqueda >, >=, =, <, <=
* opcionales
	* réplicas
	* agregar nodos dinamicamente
* pruebas de tolerancia a fallos