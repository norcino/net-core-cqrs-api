# Open Framework
## Introduction
This repository contains a basic framework that leverages multiple design and architectural patterns to offer a solid ground to build up scalable, maintainable and reliable Web Api using the latest .net technologies.

## Key features and benefits

### Command Query pattern
Command Query pattern allow to encapsulate logic in specific handlers improving re usability and separation of concerns.
Each command and query is handled by a dedicated Handler, and dependency injection will be responsible, given a certain command or query, to instantiate the specific Handler, dealing also with the decoration.

### Decorators
Each handler is decorated with multiple decorators, some are always active, others, like the Validation Decorator, is activated when a command or query has the corresponding attribute.
The decorator pattern, allow to easily add cross concern aspects, such logging, exception and transaction handling, and more.
The dependency injection is responsible to decorate each handler with the desired decorators, before returning the requested instance.

### Standardized ordering, filtering, pagination and expansion
This framework allow to query each entity using the OData syntax. The supported operations are listed below.

**Filter**

Allow to filter entities using property values, with operations like equal (eq), ... (TODO complete the list of available filters)

e.g. [https://host/api/entity?$filter=Property eq 'property string value']()

Operation | Command | Notes
---------|----------|---------
 Equal | eq | 
 Greater then | gt | 
 Lesser than | lt | 

**Order**

Allow to order the data by properties, in ascending or descending order

e.g. [https://host/api/entity?$orderBy=Property DESC]()

**Skip**

Allow to skip a certain amount of results from the query, useful to create custom pagination

**Top**

Allow to limit the results to a certain amount of entities returned, useful to create custom pagination

**Count**

Allow to add the count information to the query result

e.g. [https://host/api/entity/$count]() or 
[https://host/api/entity?$filter=Property eq 'property string value'/$count]()

**Expansion**

Allow to request additional relational data to be returned along with the entity/s requested

### Unit Testing made readable and easy to setup
Through the use of builders, the developer can setup the preconditions before running tests, in such way that highlight what are the relevant data, properties and entities, leaving to the Builders, the responsibility to generate default data, just to preserve data integrity.
The fluent notation used by the builders, also improve readability of the tests, which in combination with the use of Shoudly, can really transform a test in text.

### Integration Testing made easy, reliable and fast
To achieve this, a combination of techniques and tools has been used. For example Persisters are used to generate default and custom data which is directly persisted to the database.
Persisters are combined with Builders, offering an easy and clear way to setup preconditions.
The use of Idempotent Tests, allow the developer to always think at the individual test and create or alter the needed data, without caring about alterations made by previous or the current test.
This is achieved using transactions which are rolled back to avoid that modifications affects the preset data.
In addition to the above, the usage of SQLite in memory database, exponentially boost the speed of execution of the Integration Tests, still maintaining a reasonably similar behavior to a relational database like SqlServer.
The base tests are also setup so that changing environment configuration, say CI, instead of using the in memory database, it will use a configured real physical database.

## Conventions

### Projects and namespaces
The projects and namespaces are organised to help the developer understand what is the responsibility of the classes in the namespace, what is the scope of action and where to place new code.

**Application**
The Application namespace is used to prefix all applications that might be part of the solution, an application can be anything which directly uses the below business layer.
The sole responsibility of the application layer, is to manage security, presentation, and everything necessary to make the application ready to be used, by users or clients.

**Business**
Often in a layered architecture you can find a service layer here, well to me business emphasize more the real responsibility of this layer, to contain all the business logic that drives the whole code base.
So, except the business logic that controls the presentation layer behavior and routing, all there rest should be correctly placed in the business layer, mainly in a query or a command handler.
This layer should not know anything about any application, meaning that there should not be any coupling with any application component, so no NuGet packages or projects should be referenced by any Business.* assembly.

**Data**
The data namespace will contain the components responsible to manipulate data, mappings, configurations, abstractions and models. The Data.* assemblies should never reference any other assembly from different namespaces, except Common.*.

**Common**
The common namespace, as the name suggests, contains all components that can be reused across all layers, this normally contains cross cutting concerns such logging and helpers.
The common namespace can be used as root namespace, or sub namespace. Every assembly can reference a common assembly, but these cannot reference any other assembly in any namespace except common.
This is important to guarantee that any component coupled with a Common.* component, are still loosely coupled with other namespaces from the higher leyers of the stack.
For this reason the common namespace, can also be used as sub namespace, to contain within a layer, sharable modules that can be used across the same layer.




