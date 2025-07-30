# Recipe Ingredient Catalogue - Usage Guide

## Application Overview

The Recipe Ingredient Catalogue is a C# console application that manages recipes and ingredients through a command-line interface. The system supports multi-user authentication with role-based access control and data persistence capabilities.

## System Requirements

- .NET 8.0 Runtime
- Command-line interface access
- Sufficient storage space for user data files

## Installation and Setup

### Running the Application

1. Navigate to the project directory:
```bash
cd "Recipe Ingredient Catalogue"
```

2. Execute the application:
```bash
dotnet run
```

### Initial System Tests

Upon startup, the application automatically executes unit tests for all core classes:

```
Running tests for Ingredient class...
All Ingredient class tests passed.
Running tests for PerishableIngredient class...
All PerishableIngredient class tests passed.
Running tests for RefrigeratedIngredient class...
Running RefrigeratedIngredient tests...
WARNING: Milk has been compromised due to temperature exposure!
RefrigeratedIngredient tests completed successfully.
Running tests for FrozenIngredient class...
Running FrozenIngredient tests...
Ice Cream has been thawed (Cycle 1)
Ice Cream has been refrozen
Ice Cream has been thawed (Cycle 2)
WARNING: Ice Cream has exceeded safe freeze-thaw cycles!
FrozenIngredient tests completed successfully.
Running tests for Recipe class...
All Recipe class tests passed.
All tests have been executed.
```

## Authentication System

### User Registration

The application presents an authentication menu upon successful initialisation:

```
Welcome to the Recipe and Ingredients Catalogue!
Multi-User Recipe Management System
Please log in or register to continue.

=== Authentication ===
1. Login
2. Register
3. Exit
Choose an option (1-3):
```

To create a new user account:

1. Select option `2` (Register)
2. Enter a unique username
3. Create a secure password
4. Select user role (Admin for full access, User for standard access)

System behaviour:
- Passwords are secured using BCrypt hashing
- Role-based access control determines available features
- User accounts are stored in JSON format

### User Login

To access the system:

1. Select option `1` (Login)
2. Enter username credentials
3. Enter password credentials

The system validates credentials and establishes a user session with appropriate permissions.

## Core Functionality

### Menu System

Upon successful authentication, users access a role-based menu system:

#### Standard User Menu (Options 1-6)
- Display all recipes
- Display all ingredients  
- Display recipes by cuisine
- Search recipes or ingredients
- Display recipes by ingredient
- Load recipes and ingredients
- Exit

#### Administrator Menu (Options 1-20)
All standard user options plus:
- Add new recipe
- Add new ingredient
- Update recipe or ingredient information
- Save data (JSON format)
- Save data (Binary format)
- Load data (Binary format)
- Remove recipe or ingredient
- Rate recipes
- Sort recipes or ingredients
- Export system reports
- Performance benchmarking
- Parallel processing demonstrations

### Data Management Operations

#### Recipe Management

**Adding Recipes:**
- Enter recipe name, cuisine type, and preparation time
- Add ingredients with quantities
- Specify perishable ingredients with expiration dates
- System validates input and maintains data relationships

**Searching Recipes:**
- Search by recipe name (case-insensitive)
- Filter by cuisine type
- Find recipes containing specific ingredients

#### Ingredient Management

**Ingredient Types:**
1. **Regular Ingredients:** Basic items with name and quantity
2. **Perishable Ingredients:** Items with expiration dates
3. **Refrigerated Ingredients:** Temperature-controlled storage requirements
4. **Frozen Ingredients:** Freeze-thaw cycle management

**Adding Ingredients:**
- Select ingredient type
- Enter name and quantity
- Provide additional requirements based on type (expiration dates, temperature thresholds, etc.)

### Data Persistence

#### JSON Serialisation
- Human-readable data format
- User-specific data files
- Error handling
- Data validation on import

#### Binary Serialisation
- Compact storage format
- Efficient data transfer
- Type-safe deserialisation
- Reduced file sizes

#### Report Generation
- Formatted text reports
- Timestamped file names
- Data summaries
- Statistical information

### Performance Features

#### Benchmarking Tools
- Operation timing measurements
- Memory usage analysis
- Performance comparison reports
- System capability assessment

#### Parallel Processing
- Multi-threaded operations
- PLINQ implementation
- Performance optimisation demonstrations
- System resource utilisation

## User Interface Guidelines

### Input Validation
- Range checking for numerical inputs
- Format validation for dates
- Existence verification for operations
- Type safety for all user inputs

### Error Handling
- Exception management
- User-friendly error messages
- Graceful failure recovery
- Input retry mechanisms

### Data Integrity
- Automatic data validation
- Relationship consistency checking
- Backup and recovery capabilities
- Transaction safety

## System Architecture

### Service Layer
The application utilises a modular service architecture:

- **MenuService:** User interface navigation
- **ValidationService:** Input validation and user interaction
- **DataService:** File I/O and serialisation operations
- **RecipeService:** Recipe management operations
- **IngredientService:** Ingredient management operations
- **PerformanceService:** Benchmarking and analysis

### Domain Models
- **Ingredient:** Base class with inheritance hierarchy
- **Recipe:** Recipe entity with ingredient relationships
- **User:** Authentication and authorisation model

### Design Patterns
- Factory Method for object creation
- Strategy Pattern for algorithm selection
- Circuit Breaker for fault tolerance
- Repository Pattern for data access

## Technical Specifications

### Framework
- .NET 8.0 Console Application
- System.Text.Json for serialisation
- BCrypt for password security

### Data Structures
- Generic collections (List<T>, Dictionary<TKey, TValue>)
- LINQ for data querying
- Custom data structures for specialised operations

### Object-Oriented Features
- Four-level inheritance hierarchy
- Polymorphism through virtual methods
- Encapsulation with controlled access
- Constructor chaining and initialisation

This usage guide provides instructions for operating the Recipe Ingredient Catalogue application effectively and securely.
