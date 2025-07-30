# Recipe Ingredient Catalogue

## Technical Overview

A C# console application demonstrating object-orientated programming principles, data structures, and software engineering practices through a recipe management system. Built using .NET 8.0 with inheritance hierarchies, design patterns, and performance optimisation techniques.

## System Architecture

### Core Features

**Data Management**
- Recipe creation and ingredient tracking
- User authentication with role-based access
- Multi-format data persistence (JSON, binary)
- Search and filtering capabilities

**Object-Orientated Design**
- Four-level inheritance hierarchy (Ingredient → PerishableIngredient → RefrigeratedIngredient → FrozenIngredient)
- Design patterns implementation (Factory, Strategy, Circuit Breaker, Observer)
- Polymorphic behaviour through virtual methods
- Encapsulation with controlled access methods

**Performance Features**
- Parallel processing with PLINQ
- Performance benchmarking and monitoring
- Collections with optimised data structures
- Circuit breaker pattern for fault tolerance

## Technical Implementation

### Inheritance Hierarchy

The application demonstrates an inheritance structure:

```
Ingredient (base class)
├── Properties: Name, Quantity
├── Methods: DisplayInfo(), RunTests()
└── PerishableIngredient
    ├── Additional: ExpirationDate
    ├── Override: DisplayInfo()
    ├── RefrigeratedIngredient
    │   ├── Additional: OptimalTemp, MaxTemp
    │   └── Override: DisplayInfo()
    └── FrozenIngredient
        ├── Additional: FreezeThawCycles
        └── Override: DisplayInfo()
```

### Service Architecture

**Core Services**
- `DataService`: File I/O and serialisation operations
- `RecipeService`: Recipe management and business logic
- `IngredientService`: Ingredient operations and type handling
- `ValidationService`: Input validation and user interaction

**Additional Services**
- `PerformanceService`: Benchmarking and parallel processing
- `MenuService`: User interface and role-based navigation
- `AuthService`: User authentication and session management

### Design Patterns

**Creational Patterns**
- Factory pattern for ingredient instantiation
- Service locator pattern for dependency management

**Structural Patterns**
- Decorator pattern for enhanced functionality
- Facade pattern for simplified interfaces

**Behavioural Patterns**
- Strategy pattern for algorithm selection
- Observer pattern for event handling
- Circuit Breaker pattern for fault tolerance

## Project Structure

```
Recipe Ingredient Catalogue/
├── Recipe Ingredient Catalogue.sln
├── docs/
│   ├── README.md                          # Project documentation
│   └── usage_guide.md                     # User instructions
├── Recipe Ingredient Catalogue/
│   ├── Program.cs                         # Application entry point
│   ├── Ingredient.cs                      # Base ingredient class
│   ├── Recipe.cs                          # Recipe class with ratings
│   ├── RefrigeratedIngredient.cs          # Temperature-controlled ingredients
│   ├── FrozenIngredient.cs                # Freeze-thaw cycle management
│   ├── Authentication/
│   │   ├── AuthService.cs                 # Authentication service
│   │   └── User.cs                        # User model
│   ├── Services/
│   │   ├── DataService.cs                 # Data persistence
│   │   ├── RecipeService.cs               # Recipe operations
│   │   ├── IngredientService.cs           # Ingredient operations
│   │   ├── ValidationService.cs           # Input validation
│   │   ├── MenuService.cs                 # User interface
│   │   └── PerformanceService.cs          # Performance monitoring
│   └── Patterns/
│       ├── IngredientFactory.cs           # Factory implementation
│       ├── IngredientVisitor.cs           # Visitor pattern
│       └── IngredientProcessors.cs        # Strategy pattern
```

## Key Components

### Domain Models

**Ingredient Class Hierarchy**
- Base `Ingredient` class with polymorphic methods
- `PerishableIngredient` with expiration tracking
- `RefrigeratedIngredient` with temperature monitoring
- `FrozenIngredient` with freeze-thaw cycle management

**Recipe Class**
- Ingredient collection management
- Rating system with statistical analysis
- Validation and testing

**User Authentication**
- Secure password hashing with BCrypt
- Role-based access control (Admin/User)
- Session management and data isolation

### Service Layer

**Validation Service**
- Type-safe input collection
- Range and format validation
- Error handling with retry mechanisms

**Data Service**
- JSON and binary serialisation
- File I/O with error handling
- Data integrity validation

**Performance Service**
- Benchmarking tools with timing analysis
- Parallel processing demonstrations
- Memory usage monitoring

## Programming Concepts Demonstrated

### Collections and Algorithms
- Generic collections (List<T>, Dictionary<TKey, TValue>)
- LINQ operations for data querying
- Search and sorting algorithms
- Data structures implementation

### Command Line Interfaces
- Menu-driven navigation system
- Input validation and error handling
- Role-based interface generation
- User guidance and feedback

### Robustness and Error Handling
- Exception management
- Input validation with retry loops
- Graceful failure recovery
- Circuit breaker pattern implementation

### Object-Orientated Programming
- Inheritance hierarchies with method overriding
- Polymorphism through virtual methods
- Encapsulation with controlled access
- Constructor chaining and initialisation

### Serialisation and Data Persistence
- JSON serialisation with System.Text.Json
- Binary serialisation for compact storage
- File I/O operations with validation
- Data export and import capabilities

### Performance and Parallel Processing
- Multi-threaded operations with PLINQ
- Performance benchmarking tools
- Memory usage analysis
- Parallel algorithm implementation

## System Functionality

### User Operations
- Recipe creation and management
- Ingredient tracking with type specialisation
- Search and filtering capabilities
- Data persistence and retrieval

### Administrative Functions
- User account management
- System configuration
- Performance monitoring
- Data export and reporting

### Authentication Features
- Secure user registration
- Password-protected login
- Role-based access control
- Session management

## Technical Specifications

### Framework Requirements
- .NET 8.0 Runtime
- System.Text.Json library
- BCrypt password hashing

### Data Storage
- JSON format for human-readable data
- Binary format for compact storage
- User-specific data isolation
- Automatic backup capabilities

### Performance Characteristics
- O(1) dictionary lookups
- O(n log n) sorting operations
- Parallel processing utilisation
- Memory-efficient data structures

## Installation and Usage

### Prerequisites
- .NET 8.0 SDK or Runtime
- Command-line interface access

### Running the Application
```bash
cd "Recipe Ingredient Catalogue"
dotnet run
```

### First-Time Setup
1. Execute automatic system tests
2. Register user account
3. Select appropriate user role
4. Begin data entry or management

## Testing Strategy

The application includes unit testing:
- Property validation for all classes
- Method functionality verification
- Edge case handling
- Inheritance chain validation

All tests utilise `Debug.Assert` statements ensuring code correctness and system reliability.

## Development Practices

### Code Quality
- Single Responsibility Principle adherence
- Consistent naming conventions
- Documentation
- Error handling standards

### Architecture Patterns
- Service-oriented design
- Separation of concerns
- Dependency injection principles
- Clean code implementation

This documentation provides technical information for understanding and maintaining the Recipe Ingredient Catalogue application.
