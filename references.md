# Recipe Ingredient Catalogue - Project Documentation and References

## Project Overview

The Recipe Ingredient Catalogue is a comprehensive C# console application built on .NET 8.0 that manages recipes and ingredients with advanced features including user authentication, data persistence, and sophisticated object-orientated design patterns.

## Main Project Files Analysis

### Core Domain Models

#### 1. Program.cs
**Function**: Main entry point and application orchestrator
- **Purpose**: Manages application lifecycle, user authentication, menu navigation, and coordinates all service operations
- **Key Features**:
  - Command-line argument processing
  - User authentication with role-based access control (Admin vs User)
  - Main application loop with menu-driven navigation
  - Service layer coordination for business logic
  - Automated testing for core domain classes
  - Legacy JSON loading functionality

**C# Features Used**:
- `using` statements for namespace imports
- Static methods and console I/O
- Exception handling with try-catch blocks
- LINQ for data querying
- Async/await patterns for asynchronous operations
- Generic collections (List<T>, Dictionary<TKey, TValue>)

#### 2. Recipe.cs
**Function**: Core domain model representing a recipe entity
- **Purpose**: Manages recipe data including name, cuisine, preparation time, ingredients, and ratings
- **Key Features**:
  - Properties with getters and setters
  - Ingredient collection management
  - Rating system with average calculation
  - Comprehensive unit testing
  - Serialisation support

**C# Features Used**:
- Class definition with properties
- List<T> collections
- LINQ operations (Average())
- Exception handling (ArgumentException)
- Debug.Assert for unit testing
- Serialisable attribute for persistence

#### 3. Ingredient.cs
**Function**: Base class for ingredient entities with inheritance hierarchy
- **Purpose**: Represents basic ingredients with name and quantity, serves as base for specialized ingredient types
- **Key Features**:
  - Virtual methods for polymorphic behaviour
  - Property-based data model
  - Unit testing with Debug.Assert
  - Inheritance foundation for PerishableIngredient

**C# Features Used**:
- Virtual methods for inheritance
- Properties with automatic getters/setters
- Constructor overloading
- Serialisable attribute
- Base class design patterns

#### 4. RefrigeratedIngredient.cs
**Function**: Specialized ingredient requiring refrigerated storage
- **Purpose**: Extends PerishableIngredient with temperature management and storage validation
- **Key Features**:
  - Temperature monitoring (optimal and maximum temperatures)
  - Temperature compromise detection
  - Specialized display and validation logic
  - JSON constructor for deserialisation

**C# Features Used**:
- Class inheritance with `: base()`
- JsonConstructor attribute for JSON deserialisation
- Override methods for polymorphic behaviour
- XML documentation comments

#### 5. FrozenIngredient.cs
**Function**: Specialized ingredient requiring frozen storage
- **Purpose**: Extends RefrigeratedIngredient with freezing requirements and freeze-thaw cycle tracking
- **Key Features**:
  - Freezing temperature management
  - Freeze-thaw cycle counting
  - Quality degradation tracking
  - Visitor pattern support

**C# Features Used**:
- Deep inheritance hierarchy (4 levels)
- Template method pattern implementation
- Visitor pattern acceptance methods
- Complex property validation

### Service Layer

#### 6. DataService.cs
**Function**: Data persistence and serialisation management
- **Purpose**: Handles all data storage operations including JSON and binary serialisation, file I/O, and data validation
- **Key Features**:
  - JSON serialisation using System.Text.Json
  - Binary serialisation for compact storage
  - Comprehensive error handling
  - Data integrity validation
  - Export reporting functionality

**C# Features Used**:
- System.Text.Json for modern JSON handling
- JsonSerializer with custom options
- File I/O operations
- Generic data transfer objects
- Error handling with exception management

#### 7. RecipeService.cs
**Function**: Recipe business logic and operations management
- **Purpose**: Manages all recipe-related CRUD operations, filtering, searching, and relationship management
- **Key Features**:
  - Recipe creation with ingredient associations
  - Filtering by cuisine type
  - Search functionality with partial matching
  - Rating management and calculation
  - Sorting and display operations

**C# Features Used**:
- Static service class pattern
- LINQ queries for filtering and searching
- Collection manipulation methods
- String operations for search functionality

#### 8. IngredientService.cs
**Function**: Ingredient business logic and operations management
- **Purpose**: Manages all ingredient-related CRUD operations, type differentiation, and collection management
- **Key Features**:
  - Ingredient creation (regular and perishable)
  - Search and filtering capabilities
  - Quantity management
  - Test data generation
  - Alphabetical sorting

**C# Features Used**:
- Factory pattern implementation
- Generic type handling
- Collection operations
- Async/Task patterns for future scalability

#### 9. ValidationService.cs
**Function**: Centralised input validation and user interaction
- **Purpose**: Provides consistent validation patterns and type-safe input collection across the application
- **Key Features**:
  - Type-safe input validation (string, int, bool, DateTime)
  - Generic collection validation
  - Validation loops with error feedback
  - Standardized user prompts

**C# Features Used**:
- Generic methods with type constraints
- Validation loops with exception handling
- DateTime parsing and validation
- Boolean conversion methods

#### 10. MenuService.cs
**Function**: User interface navigation and menu management
- **Purpose**: Manages role-based menu display and user input validation for navigation
- **Key Features**:
  - Role-based menu differentiation (Admin vs User)
  - Input validation with range checking
  - Permission boundary enforcement
  - Consistent UI operations

**C# Features Used**:
- Static utility class pattern
- Console I/O operations
- Input validation loops
- Role-based conditional logic

### Authentication System

#### 11. AuthService.cs
**Function**: User authentication and management system
- **Purpose**: Handles user registration, login, password hashing, and role-based access control
- **Key Features**:
  - User registration and login
  - Password hashing for security
  - Role-based access control
  - User data persistence
  - Session management

**C# Features Used**:
- Static class with state management
- Dictionary<Guid, User> for user storage
- LINQ for user queries
- JSON serialisation for persistence
- Guid generation for unique identifiers

#### 12. User.cs (Authentication/User.cs)
**Function**: User entity model
- **Purpose**: Represents user accounts with authentication and authorization data
- **Key Features**:
  - User credentials management
  - Role-based permissions
  - Secure password storage
  - Unique identifier generation

### Command Line Interface

#### 13. CommandLineParser.cs
**Function**: Advanced command-line argument processing
- **Purpose**: Provides sophisticated CLI capabilities with support for flags, options, and complex parameter combinations
- **Key Features**:
  - Flag parsing with short (-v) and long (--verbose) forms
  - Option parsing with values (--output=file.json)
  - Positional argument handling
  - Command validation and help generation
  - Plugin-based command system

**C# Features Used**:
- Enum definitions for argument types
- Complex string parsing algorithms
- Data structures for argument management
- Plugin architecture patterns

#### 14. InteractiveShell.cs
**Function**: Interactive command shell interface
- **Purpose**: Provides an interactive command-line interface for advanced users
- **Key Features**:
  - Command history management
  - Tab completion
  - Interactive help system
  - Command pipelining

### Design Patterns Implementation

#### 15. IngredientFactory.cs (Patterns/IngredientFactory.cs)
**Function**: Factory Method pattern implementation for ingredient creation
- **Purpose**: Creates different types of ingredients without exposing instantiation logic
- **Key Features**:
  - Generic type-safe creation
  - Compile-time type checking
  - Centralised object creation
  - Extensible for new ingredient types

**C# Features Used**:
- Generic constraints with `where T : Ingredient`
- Reflection for type checking
- Generic type parameters
- Factory method pattern

#### 16. IngredientProcessors.cs (Patterns/IngredientProcessors.cs)
**Function**: Strategy pattern implementation for ingredient processing
- **Purpose**: Implements different processing algorithms for ingredients
- **Key Features**:
  - Strategy pattern for processing algorithms
  - Type-specific processing logic
  - Pluggable processing strategies

#### 17. IngredientVisitor.cs (Patterns/IngredientVisitor.cs)
**Function**: Visitor pattern implementation for ingredient operations
- **Purpose**: Enables adding new operations to ingredient classes without modifying them
- **Key Features**:
  - Visitor pattern for polymorphic operations
  - Type-safe operations on ingredient hierarchy
  - Extensible operation framework

#### 18. IIngredientProcessor.cs (Interfaces/IIngredientProcessor.cs)
**Function**: Interface definitions for ingredient processing strategies
- **Purpose**: Defines contracts for ingredient processing operations using Strategy pattern
- **Key Features**:
  - Strategy pattern interface
  - Generic type constraints
  - Visitor pattern interface
  - Interface segregation principle

### Advanced Services

#### 19. PerformanceService.cs
**Function**: Performance monitoring and benchmarking
- **Purpose**: Provides performance analysis and benchmarking capabilities for application operations
- **Key Features**:
  - Operation timing and measurement
  - Performance benchmarking
  - Memory usage monitoring
  - Performance reporting

#### 20. LoggingService.cs
**Function**: Application logging and monitoring
- **Purpose**: Centralised logging functionality for debugging and monitoring
- **Key Features**:
  - Structured logging
  - Log level management
  - File-based logging
  - Error tracking

#### 21. AdvancedCollectionsService.cs
**Function**: Advanced collection operations and algorithms
- **Purpose**: Provides sophisticated collection manipulation and data structure operations
- **Key Features**:
  - Advanced LINQ operations
  - Custom collection algorithms
  - Data structure optimisations
  - Collection performance analysis

#### 22. VectorizedMathService.cs
**Function**: Mathematical operations and vector calculations
- **Purpose**: Provides vectorized mathematical operations for performance-critical calculations
- **Key Features**:
  - Vector mathematics
  - Performance-optimised calculations
  - Mathematical algorithms
  - Numerical analysis

#### 23. CircuitBreakerService.cs
**Function**: Circuit breaker pattern implementation for fault tolerance
- **Purpose**: Implements circuit breaker pattern for handling failures gracefully
- **Key Features**:
  - Fault tolerance mechanisms
  - Automatic failure detection
  - Service degradation handling
  - Recovery mechanisms

### Configuration and Data Files

#### 24. Recipe Ingredient Catalogue.csproj
**Function**: MSBuild project configuration file
- **Purpose**: Defines project structure, dependencies, and build configuration for .NET 8.0
- **Key Features**:
  - Target framework specification (net8.0)
  - Project type configuration (Console Application)
  - Implicit usings and nullable reference types
  - Build output configuration

**MSBuild Features Used**:
- SDK-style project format
- Property groups for configuration
- Target framework specification
- Implicit using statements

#### 25. users.json
**Function**: User authentication data storage
- **Purpose**: Stores user account information including credentials and roles
- **Key Features**:
  - JSON-formatted user data
  - Password hash storage
  - Role-based access information
  - Unique user identification (GUID)

**JSON Structure**:
- User ID (GUID) as key
- Username, password hash, and role data
- Secure credential storage

#### 26. food.js
**Function**: Test data or report output file
- **Purpose**: Contains recipe and ingredient catalogue report data (appears to be output from application)
- **Key Features**:
  - Recipe listings with ratings
  - Cuisine type categorization
  - Average rating calculations
  - Test data for application validation

## Documentation and References

### C# Language and Framework References

#### Core C# Language Features
- **Classes and Objects**: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
- **Inheritance**: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/inheritance
- **Properties**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties
- **Methods**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/methods
- **Constructors**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/constructors
- **Static Classes and Members**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members

#### Advanced C# Features
- **Generics**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/
- **LINQ**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/
- **Async and Await**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/
- **Exception Handling**: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/
- **Attributes**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/
- **Reflection**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection

#### Collections Framework
- **List<T>**: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1
- **Dictionary<TKey,TValue>**: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2
- **IEnumerable<T>**: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1
- **Collection Interfaces**: https://docs.microsoft.com/en-us/dotnet/standard/collections/

#### .NET Framework Components
- **System.Text.Json**: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview
- **System.IO**: https://docs.microsoft.com/en-us/dotnet/api/system.io
- **System.Console**: https://docs.microsoft.com/en-us/dotnet/api/system.console
- **System.DateTime**: https://docs.microsoft.com/en-us/dotnet/api/system.datetime
- **System.Guid**: https://docs.microsoft.com/en-us/dotnet/api/system.guid

#### Design Patterns in C#
- **Factory Pattern**: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/factory
- **Strategy Pattern**: https://refactoring.guru/design-patterns/strategy/csharp/example
- **Visitor Pattern**: https://refactoring.guru/design-patterns/visitor/csharp/example
- **Singleton Pattern**: https://refactoring.guru/design-patterns/singleton/csharp/example

### JSON and Data Serialisation References

#### JSON Standards and Specifications
- **JSON Specification (RFC 7159)**: https://tools.ietf.org/html/rfc7159
- **JSON Schema**: https://json-schema.org/
- **JSON.NET Documentation**: https://www.newtonsoft.com/json/help/html/Introduction.htm

#### System.Text.Json Documentation
- **Getting Started**: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-get-started
- **JsonSerializer Class**: https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer
- **JsonSerializerOptions**: https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions
- **Custom Converters**: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to
- **Attributes**: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-attributes

### JavaScript References (for food.js understanding)

#### Core JavaScript Concepts
- **JavaScript Language Reference**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference
- **Objects and Properties**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Working_with_Objects
- **Arrays**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array
- **JSON in JavaScript**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON

#### Modern JavaScript Features
- **ES6+ Features**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/New_in_JavaScript/ECMAScript_2015_support_in_Mozilla
- **Modules**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules
- **Classes**: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Classes

### .NET 8.0 Specific References

#### .NET 8.0 Documentation
- **.NET 8.0 Overview**: https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8
- **Console Applications**: https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio
- **MSBuild Project Files**: https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview
- **Global Using Directives**: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive
- **Nullable Reference Types**: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references

#### Performance and Optimisation
- **Performance Best Practices**: https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run
- **Memory Management**: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/
- **Benchmarking**: https://benchmarkdotnet.org/

### Security and Authentication References

#### Authentication and Authorization
- **ASP.NET Core Identity**: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity
- **Password Hashing**: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing
- **Security Best Practices**: https://docs.microsoft.com/en-us/dotnet/standard/security/

### Testing and Quality Assurance

#### Unit Testing
- **Unit Testing in .NET**: https://docs.microsoft.com/en-us/dotnet/core/testing/
- **Debug.Assert**: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debug.assert
- **MSTest Framework**: https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest

### Development Tools and IDE

#### Visual Studio and VS Code
- **Visual Studio Documentation**: https://docs.microsoft.com/en-us/visualstudio/
- **VS Code C# Extension**: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp
- **Debugging in VS Code**: https://code.visualstudio.com/docs/editor/debugging

### Project Structure and Architecture

#### Clean Architecture
- **Clean Architecture Principles**: https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- **Dependency Injection**: https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **SOLID Principles**: https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/may/csharp-best-practices-dangers-of-violating-solid-principles-in-csharp

### Command Line and CLI Development

#### Console Applications
- **Console Application Development**: https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio
- **Command Line Parsing**: https://docs.microsoft.com/en-us/dotnet/standard/commandline/
- **System.CommandLine**: https://docs.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial

## Technology Stack Summary

### Primary Technologies
- **.NET 8.0**: Modern cross-platform development framework
- **C# 12**: Latest C# language features and syntax
- **System.Text.Json**: High-performance JSON serialisation
- **Console Application**: Cross-platform command-line interface

### Design Patterns Implemented
- **Factory Method**: For ingredient creation
- **Strategy Pattern**: For ingredient processing
- **Visitor Pattern**: For polymorphic operations
- **Repository Pattern**: For data access abstraction
- **Service Layer Pattern**: For business logic separation
- **Circuit Breaker Pattern**: For fault tolerance

### Development Practices
- **Object-Oriented Programming**: Full OOP implementation with inheritance, polymorphism, and encapsulation
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Unit Testing**: Comprehensive testing with Debug.Assert
- **Error Handling**: Robust exception handling and validation
- **Data Persistence**: Multiple serialisation formats (JSON, Binary)
- **Security**: Password hashing and role-based access control

## Conclusion

The Recipe Ingredient Catalogue demonstrates advanced C# programming concepts, modern .NET development practices, and sophisticated software architecture patterns. The application showcases enterprise-level code organisation, comprehensive error handling, and extensible design principles suitable for production environments.

---

