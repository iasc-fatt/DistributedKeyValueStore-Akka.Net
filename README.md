# DistributedKeyValueStore-Akka.Net

## TO-DOs

* El nodo orquestador debe esperar a que el nodo de datos elegido para escribir confirme la escritura
	* lo que hacemos hoy es que el nodo de datos le responde directamente al cliente, esto puede ser un problema
* es necesario que el server exponga una API cli/rest? por ahora no lo necesitamos porque ntro cliente es un actor
* refactors
	* crear una clase para la respuesta del SearchValue (reemplazar el IEnumerable<string>)
	* no hardcodear la cantidad de routees en SearchValuesActor
	* mejorar la representacion del criterio de busqueda >, >=, =, <, <=
	* usar Either en vez de Maybe para el insert
* opcionales
	* réplicas
	* agregar nodos dinamicamente
