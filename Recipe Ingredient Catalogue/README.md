# Recipe Ingredient Catalogue

This project implements a comprehensive recipe and ingredient management system using object-oriented programming principles in C#. The system provides both administrative and user modes, allowing for full management of recipes and ingredients with different levels of access.

## Architecture Overview

The system is built around three core classes that work together to manage recipe and ingredient data:

## Project File Structure

The project follows a standard C# console application structure with the following organization:

```
Recipe Ingredient Catalogue/
├── Recipe Ingredient Catalogue.csproj          # Project configuration file
├── Program.cs                                  # Main application entry point with menu system
├── Recipe.cs                                   # Recipe class definition
├── Ingredient.cs                               # Ingredient and PerishableIngredient class definitions
├── README.md                                   # Project documentation
├── bin/                                        # Compiled binaries and dependencies
│   └── Debug/
│       └── net8.0/
├── obj/                                        # Intermediate build files
│   └── Debug/
│       └── net8.0/
└── Recipe Ingredient Catalogue.sln             # Solution file for Visual Studio
```

**Key Files:**
- `Recipe Ingredient Catalogue.csproj`: Contains project metadata, target framework (.NET 8.0), and package references
- `Program.cs`: Implements the main application logic, user interface, and data management
- `Recipe.cs`: Defines the Recipe class with properties for name, cuisine, preparation time, ingredients, and ratings
- `Ingredient.cs`: Contains both the base Ingredient class and the derived PerishableIngredient class
- `README.md`: Comprehensive documentation of the system architecture and functionality
- `Recipe Ingredient Catalogue.sln`: Solution file that organizes the project for development in Visual Studio or other IDEs

### 1. Ingredient Class

The `Ingredient` class represents a basic food ingredient with essential properties and functionality:

**Properties:**
- `Name`: The name of the ingredient (string)
- `Quantity`: The available quantity of the ingredient (integer)

**Key Methods:**
- `DisplayInfo()`: Outputs ingredient details to the console
- `GetName()`, `GetQuantity()`: Accessor methods for properties
- `SetName()`, `SetQuantity()`: Mutator methods for properties
- `RunTests()`: Comprehensive unit tests to verify class functionality

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

### User Mode (read-only)
- **Display all recipes** with complete details
- **Filter recipes by cuisine** type
- **Search recipes or ingredients** by name
- **Find recipes containing specific ingredients**
- **Load recipes and ingredients** from external files

### Admin Mode (full access)
- **Add new recipes** and ingredients
- **Update existing recipe or ingredient information**
- **Remove recipes or ingredients** from the catalogue
- **Rate recipes** (1-5 stars)
- **Sort recipes or ingredients** alphabetically
- **Save data** to files for persistence
- **Export comprehensive reports** in text format
- **Exit the program**

## Implementation Details

### Program Flow
1. **Authentication**: The program accepts a command-line argument (`admin` or `user`) to determine access level
2. **Testing**: All classes run their unit tests upon startup to ensure correctness
3. **Main Loop**: Continuous menu-driven interface that processes user commands
4. **Data Management**: Uses dictionaries to store recipes and ingredients for efficient lookup

### Key Design Patterns
- **Inheritance**: `PerishableIngredient` inherits from `Ingredient`, demonstrating code reuse
- **Encapsulation**: Private member variables with public getter/setter methods
- **Separation of Concerns**: Different methods handle specific responsibilities
- **Error Handling**: Try-catch blocks protect against unexpected input and failures

### Data Persistence
The system supports saving and loading data through file operations:
- **Save Data**: Exports recipes and ingredients to text files in a structured format
- **Load Data**: Imports data from text files, parsing recipes and ingredients sections
- **Export Report**: Generates formatted reports with sorted recipe and ingredient information

## Usage Instructions

### Running the Application
```bash
dotnet run --project "Recipe Ingredient Catalogue" admin
# or
dotnet run --project "Recipe Ingredient Catalogue" user
```

### File Format for Data Import/Export
```
Recipes:
Recipe Name 1, Cuisine Type
Recipe Name 2, Cuisine Type

Ingredients:
Ingredient Name 1, Quantity
Ingredient Name 2, Quantity
```

## Testing Strategy

The system includes comprehensive unit tests for all classes:
- **Property validation**: Tests getter and setter methods
- **Method functionality**: Verifies core operations like adding ingredients and calculating ratings
- **Edge cases**: Tests invalid inputs and boundary conditions
- **Inheritance**: Validates that derived classes properly extend base functionality

All tests use `Debug.Assert` statements that will halt execution if any test fails, ensuring code correctness.


## Future Enhancements

Potential improvements to the system could include:
- Database integration for more robust data storage
- Web or GUI interface for improved user experience
- Recipe scaling functionality to adjust ingredient quantities
- Nutritional information tracking
- Shopping list generation
- Image support for recipes
- User authentication system
- Advanced search with multiple criteria
