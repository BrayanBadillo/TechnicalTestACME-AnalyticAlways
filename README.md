# Sistema de Gestión Escolar ACME - ACME School Management System

<details>
  <summary>  <img src="https://flagcdn.com/w20/es.png" alt="Español" width="20" /> <strong>Español</strong> </summary>

## Descripción
Se desarrollo un sistema de gestión escolar para ACME siguiendo las mejores prácticas de arquitectura, diseño y desarrollo en C#.
Este proyecto es una solución basada en Clean Architecture para gestionar cursos y estudiantes en una escuela. Se han aplicado
principios SOLID, DDD y buenas prácticas de testing para garantizar una base sólida, extensible y lista para escalar.A continuación,
te explico las principales decisiones tomadas y cómo se abordo el problema:
	 
## Funcionalidad
- Registrar estudiantes adultos (mayores de 18 años).
- Registrar cursos con nombre, valor de inscripción y fechas.
- Inscribir estudiantes a cursos tras pagar (simulado).
- Consultar cursos con estudiantes dentro de un rango de fechas.

## Arquitectura

```
ACME.sln
src
├── ACME.Domain          → Entidades, ValueObjects, Excepciones
├── ACME.Application     → DTOs, Interfaces y servicios
├── ACME.Infrastructure  → Repositorios, Persistencia en memoria, extensiones y servicios
└── ACME.Tests           → Tests automatizados con xUnit y Mock
```

He implementado una solución basada en Clean Architecture y Domain-Driven Design (DDD), utilizando principios SOLID y patrones de diseño que permiten mantener el código limpio, extensible y fácilmente testeable, lo cual es ideal para este tipo de sistema por las siguientes razones:

1. Separación clara de responsabilidades - La arquitectura está dividida en capas bien definidas:

- ***Dominio:*** Contiene las entidades, reglas de negocio, excepciones y objetos de valor
- ***Aplicacion:*** Contiene los servicios, interfaces de repositorios y DTOs  
- ***Infraestructura:*** Implementaciones concretas (repositorios en memoria, pasarela de pagos simulada)
- ***Tests:*** Pruebas unitarias exhaustivas con xUnit y Moq

### Modelo de dominio
El modelo de dominio está compuesto por las siguientes entidades y objetos de valor:

- ***Student:*** Representa a un estudiante con nombre y edad (solo adultos).
- ***Course:*** Representa un curso con nombre, tarifa de inscripción, fecha de inicio y fecha de fin.
- ***Enrollment:*** Representa la inscripción de un estudiante en un curso, con fecha de inscripción y referencia al pago (si corresponde).
- ***Money:*** Objeto de valor que representa un monto monetario con cantidad y moneda.

### Patrones implementados
- ***Patron Repositorio:*** Para abstraer la persistencia de datos.
- ***Inyeccion de dependencias:*** Para gestionar las dependencias entre los componentes.
- ***Unidad de Trabajo:*** Para manejar transacciones y garantizar la consistencia de los datos.
- ***Objetos de Valor*** Para representar conceptos inmutables como el dinero.
- ***Modelo de Dominio Rico*** Las entidades contienen la lógica de negocio relacionada con su comportamiento.

### Aplicación de Principios SOLID
- ***S (Single Responsibility):*** Cada clase tiene una única responsabilidad bien definida
- ***O (Open/Closed):*** Las abstracciones permiten extender el comportamiento sin modificar código existente
- ***L (Liskov Substitution):*** Las implementaciones de repositorios pueden intercambiarse sin afectar el funcionamiento
- ***I (Interface Segregation):*** Interfaces específicas para cada tipo de repositorio y servicio
- ***D (Dependency Inversion):*** La dependencia hacia abstracciones permite flexibilidad de implementación

### Aspectos Destacables sobre la adecision de implementación
- ***Patrón Unidad de Trabajo:*** Para mantener la consistencia transaccional (aunque actualmente es una implementación en memoria)
- ***Manejo de errores avanzado:*** Excepciones de dominio específicas para errores de negocio
- ***Tests exhaustivos:***
    - Tests unitarios para entidades del dominio
    - Tests para servicios de aplicación con mocks para simular dependencias
- ***Repositorios en memoria:*** Para este prototipo, se implementaron repositorios en memoria, pero el diseño permite cambiar fácilmente a una base de datos real.
- ***Simulación de pasarela de pago:*** Se implementó una pasarela de pago simulada que siempre devuelve un pago exitoso.
- ***Validaciones en el dominio:*** Todas las reglas de negocio están encapsuladas en las entidades del dominio.
- ***DTOs:*** Se definieron DTOs para transferir datos entre capas sin exponer las entidades del dominio.

### Patrones asíncronos:
Todos los métodos de repositorio y servicios usan Task/async/await Preparado para operaciones I/O reales

### Abstracciones para sistemas externos:
- ***IPaymentGateway*** abstrae la lógica de pagos
- ***Implementación dummy*** para el prototipo, pero preparado para integración real


# Reflexiones del proyecto
### **¿Qué cosas me habría gustado hacer pero no hice?**
1. ***Implementar un sistema de validación más robusto:*** Me habría gustado utilizar FluentValidation para validaciones más complejas.
2. ***Implementar autenticación y autorización:*** Aunque no era parte del alcance, habría sido valioso para un sistema real.
3. ***Implementar un frontend:*** Una interfaz web simple con Razor Pages, Blazor, Angular o React habría sido útil para visualizar el funcionamiento.
4. ***Integrar una base de datos real:*** Aunque el diseño lo permite, no se implementó para mantener la simplicidad.
5. ***Implementar un sistema de notificaciones:*** Para informar a los estudiantes sobre inscripciones, pagos, etc.

### **¿Qué cosas hice pero creo que podrían mejorarse por si el proyecto continua?**
1. ***Manejo de errores:*** El sistema actual maneja los errores básicos, pero podría mejorarse con un sistema más   centralizado.
2. ***Documentación de la API:*** Añadir más documentación XML para los métodos públicos.
3. ***Optimización de consultas:*** Las consultas actuales son sencillas, pero podrían optimizarse para conjuntos de datos grandes.
4. ***Localización:*** El sistema no soporta múltiples idiomas, lo que sería importante para una aplicación internacional.
5. ***Tests de integración:*** Aunque hay buenos tests unitarios, faltan tests de integración para probar el sistema completo.

### **¿Qué librerías de terceros usé y por qué?**
- `Microsoft.Extensions.DependencyInjection:` Para gestionar la inyección de dependencias.
- `xUnit:` Framework para pruebas unitarias.
- `Moq:` Para crear mocks en las pruebas.

### **¿Cuánto tiempo he invertido en el proyecto?**
He invertido aproximadamente 13 horas en este proyecto, distribuidas de la siguiente manera:
- ***Diseño y planificación:*** 3 horas
- ***Implementación del dominio:*** 2 horas
- ***Implementación de la aplicación e infraestructura:*** 3 horas
- ***Implementación de tests:*** 3 horas
- ***Documentación y revisión:*** 2 horas

### **¿Qué cosas tuve que investigar?**
- Mejores prácticas actuales de DDD con .NET 8: Para asegurarme de seguir las prácticas más recientes.
- Patrones de diseño para simulación de pasarelas de pago: Para implementar correctamente la abstracción.
- Técnicas de testing avanzadas con xUnit y Moq: Para crear pruebas más robustas.
- Manejo de fechas en rangos de búsqueda: Para implementar correctamente la consulta de cursos por fechas.
- Configuración óptima de inyección de dependencias: Para asegurar un ciclo de vida adecuado de los componentes.

## Futuras mejoras:
En caso de que el proyecto avance, se podrían realizar las siguientes mejoras:

### Persistencia de datos:
- Implementar repositorios para una base de datos SQL usando Entity Framework Core.
- Utilizar Dapper para consultas optimizadas.
- Considerar opciones NoSQL para escenarios específicos.

### API Web:
- Crear una API RESTful usando ASP.NET Core.
- Implementar autenticación y autorización con Identity o JWT.
- Documentar la API con Swagger/OpenAPI.

### Integración con servicios externos:
- Implementar una pasarela de pago real.
- Servicios de notificaciones (email, SMS).
- Integración con sistemas de calendarios.

### Mejoras de arquitectura:
- Implementar CQRS para separar operaciones de lectura y escritura.
- Utilizar MediatR para implementar el patrón Mediator.
- Implementar Event Sourcing para el historial de cambios.

### Monitoreo y Logging:
- Integrar un sistema de logging (Serilog, NLog).
- Implementar telemetría con Application Insights.
- Añadir health checks para monitorear la salud del sistema.

</details>
  
<details>
 <summary> <img src="https://flagcdn.com/w20/gb.png" alt="English" width="20" /> <strong>English</strong></summary>

## Description  
A school management system was developed for ACME following best practices in architecture, design, and development using C#.  
This project is a Clean Architecture-based solution to manage courses and students in a school. SOLID principles, Domain-Driven Design (DDD), and testing best practices were applied to ensure a solid, extensible, and scalable foundation. Below is an explanation of the key decisions made and how the problem was approached:

## Functionality  
- Register adult students (18 years or older).  
- Register courses with name, enrollment fee, and dates.  
- Enroll students in courses after payment (simulated).  
- Query courses with enrolled students within a date range.  

## Architecture

```
ACME.sln
src
├── ACME.Domain          → Entidades, ValueObjects, Excepciones
├── ACME.Application     → DTOs, Interfaces y servicios
├── ACME.Infrastructure  → Repositorios, Persistencia en memoria, extensiones y servicios
└── ACME.Tests           → Tests automatizados con xUnit y Mock
```

I implemented a solution based on Clean Architecture and Domain-Driven Design (DDD), using SOLID principles and design patterns that help maintain clean, extensible, and testable code ideal for this type of system for the following reasons:

1. Clear separation of concerns – The architecture is divided into well-defined layers:
- ***Domain:*** Contains entities, business rules, exceptions, and value objects  
- ***Application:*** Contains services, repository interfaces, and DTOs  
- ***Infrastructure:*** Concrete implementations (in-memory repositories, simulated payment gateway)  
- ***Tests:*** Thorough unit testing with xUnit and Moq  

### Domain Model  
The domain model consists of the following entities and value objects:
- ***Student:*** Represents a student with name and age (adults only).  
- ***Course:*** Represents a course with a name, enrollment fee, start date, and end date.  
- ***Enrollment:*** Represents a student’s enrollment in a course, with enrollment date and payment reference (if applicable).  
- ***Money:*** A value object representing a monetary amount with value and currency.  

### Implemented Patterns  
- ***Repository Pattern***: To abstract data persistence.  
- ***Dependency Injection***: To manage component dependencies.  
- ***Unit of Work***: To handle transactions and ensure data consistency.  
- ***Value Objects***: To represent immutable concepts like money.  
- ***Rich Domain Model***: Entities encapsulate business logic related to their behavior.  

### Application of SOLID Principles  
- ***S (Single Responsibility)***: Each class has a single, well-defined responsibility.  
- ***O (Open/Closed)***: Abstractions allow behavior to be extended without modifying existing code.  
- ***L (Liskov Substitution)***: Repository implementations can be swapped without affecting functionality.  
- ***I (Interface Segregation)***: Specific interfaces for each type of repository and service.  
- ***D (Dependency Inversion)***: Dependence on abstractions allows flexible implementations.  

### Notable Implementation Decisions  
- ***Unit of Work Pattern***: Ensures transactional consistency (currently in-memory).  
- ***Advanced Error Handling***: Domain-specific exceptions for business errors.  
- ***Extensive Testing***:
  - Unit tests for domain entities  
  - Application service tests with mocked dependencies  
- ***In-memory Repositories***: For this prototype, repositories are in-memory, but the design allows easy transition to a real database.  
- ***Simulated Payment Gateway***: Always returns a successful payment for testing purposes.  
- ***Domain Validations***: All business rules are encapsulated within the domain entities.  
- ***DTOs***: DTOs were defined to transfer data between layers without exposing domain entities.  

### Asynchronous Patterns  
All repository and service methods use `Task/async/await`, ready for real I/O operations.

### Abstractions for External Systems  
- ***IPaymentGateway*** abstracts payment logic.  
- ***Dummy implementation*** for the prototype, but ready for real integration.  

## Project Reflections  

### **What things would I have liked to do but didn't?**

1. ***Implement a more robust validation system:*** I would have liked to use FluentValidation for more complex rules.  
2. ***Implement authentication and authorization:*** Although not in scope, it would have been valuable in a real-world system.  
3. ***Build a frontend:*** A simple web interface using Razor Pages, Blazor, Angular, or React would help visualize the system.  
4. ***Integrate a real database:*** The design supports it, but it was not implemented to keep things simple.  
5. ***Implement a notification system:*** To inform students about enrollments, payments, etc.

### **What things did I do but think could be improved if the project continues?**
1. ***Error handling:*** The current system handles basic errors but could benefit from a centralized error handling mechanism.  
2. ***API documentation:*** Add more XML documentation to public methods.  
3. ***Query optimization:*** Current queries are simple but could be optimized for large datasets.  
4. ***Localization:*** The system does not support multiple languages, which is important for international use.  
5. ***Integration tests:*** While unit tests are solid, integration tests are missing to validate the entire system flow.

### **What third-party libraries did I use and why?**
- `Microsoft.Extensions.DependencyInjection`: To manage dependency injection.  
- `xUnit`: For unit testing.  
- `Moq`: To mock dependencies in tests.  

### **How much time did I invest in the project?**
I invested approximately ***13 hours*** in this project, distributed as follows:

- ***Design and planning:*** 3 hours  
- ***Domain implementation:*** 2 hours  
- ***Application and infrastructure implementation:*** 3 hours  
- ***Test implementation:*** 3 hours  
- ***Documentation and review:*** 2 hours  

### **What did I have to research?**
- Best practices for DDD with .NET 8  
- Design patterns for simulating payment gateways  
- Advanced testing techniques with xUnit and Moq  
- Handling date ranges in queries  
- Optimal dependency injection setup  

## Future Improvements  
If the project evolves, the following enhancements could be made:

### Data Persistence
- Implement repositories for an SQL database using Entity Framework Core  
- Use Dapper for optimized queries  
- Consider NoSQL options for specific scenarios  

### Web API
- Create a RESTful API using ASP.NET Core  
- Implement authentication and authorization with Identity or JWT  
- Document the API with Swagger/OpenAPI  

### Integration with External Services
- Implement a real payment gateway  
- Notification services (email, SMS)  
- Calendar system integration  

### Architectural Enhancements
- Implement CQRS to separate read and write operations  
- Use MediatR to implement the Mediator pattern  
- Implement Event Sourcing for change history tracking  

### Monitoring and Logging
- Integrate a logging system (Serilog, NLog)  
- Add telemetry with Application Insights  
- Include health checks to monitor system status  

</details>
