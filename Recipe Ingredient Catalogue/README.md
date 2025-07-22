# Recipe Ingredient Catalogue

This project models a recipe management system using object-oriented programming principles in C#. It consists of two main classes: `Recipe` and `Ingredient`.

## Recipe Class

The `Recipe` class represents a recipe with the following properties:
- `Name`: The name of the recipe
- `Cuisine`: The type of cuisine (e.g., Italian, Mexican)
- `PreparationTime`: The preparation time in minutes
- `ingredients`: A private list of `Ingredient` objects
- `ratings`: A private list of integer ratings

### Key Methods

- **AddIngredient(Ingredient ingredient)**: Adds an ingredient to the recipe
- **GetIngredients()**: Returns the list of ingredients in the recipe
- **DisplayInfo()**: Displays detailed information about the recipe including name, cuisine, preparation time, ingredients, and average rating
- **AddRating(int rating)**: Adds a rating (1-5) to the recipe
- **GetAverageRating()**: Calculates and returns the average rating of the recipe
- **HasRatingAboveOrEqual(double rating)**: Checks if the recipe's average rating meets or exceeds the specified value

The class includes comprehensive unit tests in the `RunTests()` method that verify all functionality using `Debug.Assert` statements.

## Ingredient Class

The `Ingredient` class (defined in Ingredient.cs) represents a food ingredient with:
- `Name`: The name of the ingredient
- `Quantity`: The quantity of the ingredient

It includes methods to display ingredient information and supports being added to recipes.

## Usage

The system can be used to:
1. Create recipe instances with specific names, cuisines, and preparation times
2. Add ingredients to recipes
3. Rate recipes (1-5 stars)
4. Display comprehensive recipe information
5. Filter recipes based on rating thresholds

The `Program.cs` file demonstrates how to instantiate and use these classes together in a console application.

## Testing

Both classes include unit tests (`RunTests()` method) that validate all functionality through assertions. Running these tests ensures the correctness of the implementation.
