# Recipe Ingredient Catalogue Documentation

## Key Topics Implemented

### 1. Console IO and Variables
- **Interactive User Interface**: Extensive use of `Console.WriteLine()` for output and `Console.ReadLine()` for input collection
- **Input Validation**: Robust validation loops in helper methods like `GetIntInput()` to ensure valid numeric input
- **Data Storage**: 
  - Dictionaries (`Dictionary<string, Recipe>`, `Dictionary<string, Ingredient>`) for efficient O(1) lookups
  - Lists (`List<Ingredient>`, `List<int>`) for managing collections of ingredients and ratings
- **State Management**: Variables maintain application state across the entire workflow
- **Dynamic Data Handling**: Real-time updates and modifications to recipe and ingredient data


### 2. Command Line Interfaces
- **Menu-Driven Navigation**: Comprehensive CLI menu system with 20 distinct operations for admin mode, 7 for user mode
- **Command Pattern**: Each menu option maps to a dedicated method (e.g., `AddNewRecipe()`, `SearchRecipesOrIngredients()`, `DisplayAllIngredients()`)
- **User Guidance**: Clear prompts and instructions throughout the interface
- **Exit Handling**: Graceful shutdown with `ExitProgram()` method

### 3. Robustness and Error Handling
- **Structured Exception Handling**: 
  - Try-catch blocks in critical operations (file I/O, data validation)
  - Specific error messages for different failure scenarios
- **Input Validation**:
  - Range checks for ratings (1-5)
  - Existence checks for recipe/ingredient operations
- **Debugging Support**: 
  - `Debug.Assert` statements in class test methods
  - Comprehensive error messages for invalid operations

### 4. Encapsulation and Constructors
- **Class Design**:
  - Public properties with getters and setters (e.g., `Name`, `Quantity`, `Cuisine`)
  - Clean property-based architecture for .NET 8.0
- **Constructor Initialization**:
  - Parameterized constructors for proper object creation
  - Base class constructor calls in inheritance chain (`PerishableIngredient : Ingredient`)
- **Method Overriding**: Virtual methods like `DisplayInfo()` for polymorphic behavior

### 5. Object-Oriented Programming (OOP)
- **Inheritance Hierarchy**:
  - Base `Ingredient` class extended by `PerishableIngredient`
  - Method overriding for specialized behavior
- **Polymorphism**:
  - Virtual/override `DisplayInfo()` method
  - Common interface for ingredient operations
- **Static Testing**: `RunTests()` static methods for comprehensive unit testing

## Class Architecture

### 1. Ingredient Class
Base class for all ingredients with core functionality:

**Properties:**
- `Name`: Public property for ingredient name (get/set)
- `Quantity`: Public property for available quantity (get/set)

**Key Methods:**
- `DisplayInfo()`: Virtual method that outputs ingredient details to the console
- `RunTests()`: Static method with comprehensive unit tests using `Debug.Assert`

### 2. PerishableIngredient Class

This class extends `Ingredient` to handle ingredients with expiration dates, demonstrating inheritance:

**Additional Properties:**
- `ExpirationDate`: DateTime value indicating when the ingredient expires

**Key Features:**
- Inherits all functionality from `Ingredient` class
- Overrides `DisplayInfo()` to include expiration date information
- Maintains its own `RunTests()` method that validates all inherited and new functionality

### 3. Recipe Class

The `Recipe` class represents a complete recipe with multiple ingredients and user ratings:

**Properties:**
- `Name`: The recipe name
- `Cuisine`: The type of cuisine (e.g., Italian, Mexican)
- `PreparationTime`: Time required to prepare the recipe in minutes
- `ingredients`: Private list of `Ingredient` objects
- `ratings`: Private list of integer ratings (1-5)

**Key Methods:**
- `AddIngredient(Ingredient ingredient)`: Adds an ingredient to the recipe
- `GetIngredients()`: Returns the complete list of ingredients
- `DisplayInfo()`: Displays comprehensive recipe information including name, cuisine, preparation time, ingredients, and average rating
- `AddRating(int rating)`: Adds a user rating (validated to be between 1-5)
- `GetAverageRating()`: Calculates and returns the average rating
- `HasRatingAboveOrEqual(double rating)`: Checks if average rating meets or exceeds threshold
- `RunTests()`: Comprehensive unit tests using `Debug.Assert` to validate all functionality

## System Functionality

The application provides a rich set of features accessible through a command-line interface:

### User Mode (read-only access)
1. **Display all recipes** with complete details
2. **Display all ingredients** - view all ingredients with quantities and expiration dates
3. **Display recipes by cuisine** type filtering
4. **Search recipes or ingredients** by name
5. **Display recipes by ingredient** - find recipes containing specific ingredients
6. **Load recipes and ingredients** from JSON files
7. **Exit** the program

### Admin Mode (full access - all user features plus)
7. **Add a new recipe** with ingredient management
8. **Add a new ingredient** (regular or perishable with expiration dates)
9. **Display all ingredients** - view all ingredients with quantities and expiration dates
10. **Update recipe or ingredient information** - modify existing data
11. **Save data (JSON)** - export to JSON format
12. **Save data (Binary)** - compact binary serialization
13. **Load data (Binary)** - import from binary files
14. **Remove recipe or ingredient** - delete from catalogue
15. **Rate a recipe** - add 1-5 star ratings
16. **Sort recipes or ingredients** - alphabetical ordering
17. **Export report** - generate formatted text reports
18. **Performance benchmark** - timing and memory profiling tools
19. **Parallel processing demo** - multi-threaded operation demonstrations
20. **Exit** the program

## Implementation Details

### Program Flow
1. **Authentication**: The program accepts a command-line argument (`admin` or `user`) to determine access level
2. **Testing**: All classes run their unit tests upon startup to ensure correctness
3. **Main Loop**: Continuous menu-driven interface that processes user commands
4. **Data Management**: Uses dictionaries to store recipes and ingredients for efficient lookup
5. **File Persistence**: Structured text format for data import/export with validation

### Key Design Patterns
- **Inheritance**: `PerishableIngredient` inherits from `Ingredient`, demonstrating code reuse
- **Encapsulation**: Private member variables with public getter/setter methods
- **Separation of Concerns**: Different methods handle specific responsibilities
- **Error Handling**: Try-catch blocks protect against unexpected input and failures
- **Command Pattern**: Menu options map to dedicated methods for clear operation mapping

### Key Design Patterns
- **Inheritance**: `PerishableIngredient` inherits from `Ingredient`, demonstrating code reuse
- **Encapsulation**: Private member variables with public getter/setter methods
- **Separation of Concerns**: Different methods handle specific responsibilities
- **Error Handling**: Try-catch blocks protect against unexpected input and failures

### Data Persistence
The system supports multiple data storage formats:
- **JSON Serialization**: Human-readable format using System.Text.Json with custom options
- **Binary Serialization**: Compact binary format for efficient storage
- **Load Data**: Imports from both JSON and binary file formats with error handling
- **Export Report**: Generates formatted text reports with sorted recipe and ingredient information

## Usage Instructions

### Running the Application
```bash
dotnet run --project "Recipe Ingredient Catalogue" admin
# or
dotnet run --project "Recipe Ingredient Catalogue" user
```

### File Format for Data Import/Export

#### JSON Format (Primary)
```json
{
  "Recipes": {
    "Pasta Carbonara": {
      "Name": "Pasta Carbonara",
      "Cuisine": "Italian",
      "PreparationTime": 30,
      "ingredients": [
        {
          "Name": "Pasta",
          "Quantity": 500
        },
        {
          "Name": "Eggs",
          "Quantity": 3,
          "ExpirationDate": "2024-01-15T00:00:00"
        }
      ],
      "ratings": [4, 5, 4]
    }
  },
  "Ingredients": {
    "Pasta": {
      "Name": "Pasta",
      "Quantity": 500
    },
    "Eggs": {
      "Name": "Eggs",
      "Quantity": 12,
      "ExpirationDate": "2024-01-15T00:00:00"
    }
  }
}
```

#### Binary Format
Compact binary serialization for efficient storage (not human-readable).

## Testing Strategy

The system includes comprehensive unit tests for all classes:
- **Property validation**: Tests getter and setter methods
- **Method functionality**: Verifies core operations like adding ingredients and calculating ratings
- **Edge cases**: Tests invalid inputs and boundary conditions
- **Inheritance**: Validates that derived classes properly extend base functionality

All tests use `Debug.Assert` statements that will halt execution if any test fails, ensuring code correctness.

## Performance Features

### 6. Performance Optimization and Parallel Processing
- **Benchmarking Tools**: Built-in performance measurement using `Stopwatch` for timing operations
- **Memory Profiling**: GC memory usage tracking and optimization analysis
- **Parallel Processing**: PLINQ (Parallel LINQ) for multi-threaded data operations
- **Task-Based Operations**: `Parallel.ForEach` and `Task.Run` for concurrent processing
- **Multi-Core Utilization**: Automatic processor core detection and workload distribution

#### Performance Benchmark Features (Menu Option 18)
- Sequential vs parallel search comparisons
- LINQ operation timing analysis
- Dictionary lookup vs list search performance tests
- Memory usage profiling for large datasets

#### Parallel Processing Demonstrations (Menu Option 19)
- PLINQ operations with `.AsParallel()` for data filtering
- `Parallel.ForEach` for concurrent recipe processing
- Task-based chunk processing across multiple cores
- Performance speedup calculations and reporting

### Technical Requirements
- **.NET 8.0**: Latest framework with performance optimizations
- **System.Text.Json**: High-performance JSON serialization
- **Extension Methods**: Custom `Chunk()` method for older .NET compatibility
- **Multi-Threading**: Thread-safe operations with `Interlocked` for counters

## Future Enhancements

Planned improvements and feature expansions:

### 1. Data Management Evolution
- **Database Integration**: 
  - Implement SQLite for persistent storage
  - Add Entity Framework support
  - Enable cloud data synchronization
- **Data Relationships**:
  - Create ingredient category hierarchies
  - Implement recipe tagging system
  - Add user-specific data tracking

### 2. Interface Modernization
- **Web Interface**:
  - Develop ASP.NET Core web application
  - Implement responsive design for mobile access
  - Add interactive recipe builder UI
- **Desktop GUI**:
  - Create MAUI cross-platform application
  - Implement drag-and-drop recipe management
  - Add visual ingredient inventory tracking

### 3. Recipe Management Features
- **Scaling System**:
  - Implement automatic ingredient quantity adjustment
  - Add serving size calculator
  - Support unit conversion system
- **Nutritional Analysis**:
  - Integrate nutritional database
  - Calculate recipe calorie counts
  - Add dietary restriction filtering
- **Shopping List Automation**:
  - Generate consolidated ingredient lists
  - Implement store location tracking
  - Add grocery delivery integration

### 4. User Experience Enhancements
- **Authentication System**:
  - Implement multi-user support
  - Add role-based access control
  - Enable user preference persistence
- **Advanced Search**:
  - Add multi-criteria search filters
  - Implement fuzzy matching algorithm
  - Add search result prioritization
- **Media Integration**:
  - Support recipe image uploads
  - Add video tutorial integration
  - Implement step-by-step visual guides
