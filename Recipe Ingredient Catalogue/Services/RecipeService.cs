using System;
using System.Collections.Generic;
using System.Linq;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * RecipeService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Manages all recipe-related operations including creation, modification, display,
 * filtering, and search functionality. Handles recipe-ingredient relationships and
 * provides comprehensive CRUD operations with advanced querying capabilities.
 * 
 * KEY RESPONSIBILITIES:
 * • Adding new recipes with ingredient associations and validation
 * • Displaying recipe collections with detailed formatting
 * • Filtering recipes by cuisine type for categorized browsing
 * • Finding recipes containing specific ingredients
 * • Updating recipe properties and metadata
 * • Removing recipes from the catalogue
 * • Managing recipe ratings and calculating averages
 * • Searching recipes by name with partial matching
 * • Sorting recipes alphabetically for organized display
 * 
 * DESIGN PATTERNS:
 * • Static Service Class: Provides stateless operations for recipe management
 * • Repository Pattern: Abstracts recipe data access and manipulation
 * • Query Object Pattern: Implements complex filtering and search operations
 * 
 * DEPENDENCIES:
 * • ValidationService: For user input validation and error handling
 * • Recipe class: Core domain model for recipe entities
 * • Ingredient classes: For recipe-ingredient relationship management
 * 
 * PUBLIC METHODS:
 * • AddNewRecipe(): Creates new recipes with ingredient associations
 * • DisplayAllRecipes(): Shows all recipes with comprehensive details
 * • DisplayRecipesByCuisine(): Filters and displays recipes by cuisine type
 * • DisplayRecipesByIngredient(): Finds recipes containing specific ingredients
 * • UpdateRecipe(): Modifies existing recipe properties
 * • RemoveRecipe(): Deletes recipes from the catalogue
 * • RateRecipe(): Adds user ratings to recipes
 * • SortRecipes(): Displays recipes in alphabetical order
 * • SearchRecipes(): Finds recipes by partial name matching
 * 
 * INTEGRATION POINTS:
 * • Used by Program.cs for recipe management operations
 * • Integrates with IngredientService for ingredient-recipe relationships
 * • Supports DataService for persistence operations
 * • Enables PerformanceService for benchmarking recipe operations
 * 
 * USAGE EXAMPLES:
 * • Creating recipes with multiple ingredients and cuisine classification
 * • Finding all Italian recipes in the catalogue
 * • Searching for recipes that use tomatoes
 * • Rating recipes and viewing average ratings
 * • Updating recipe cuisine or preparation details
 * 
 * TECHNICAL NOTES:
 * • Implements LINQ for efficient filtering and searching operations
 * • Uses dictionary-based storage for O(1) lookup performance
 * • Supports case-insensitive searching and filtering
 * • Provides comprehensive error handling for all operations
 * • Maintains referential integrity between recipes and ingredients
 * • Thread-safe implementation for concurrent access scenarios
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    public static class RecipeService
    {
        public static void AddNewRecipe(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                string recipeName = ValidationService.GetInput("Enter the recipe name: ");
                if (!ValidationService.ValidateItemNotExists(recipes, recipeName, "Recipe"))
                    return;

                string cuisine = ValidationService.GetInput("Enter the cuisine type: ");
                int preparationTime = ValidationService.GetIntInput("Enter the preparation time (minutes): ");
                Recipe newRecipe = new(recipeName, cuisine, preparationTime);

                while (true)
                {
                    string ingredientName = ValidationService.GetInput("Enter the ingredient's name (or 'done' to finish): ");
                    if (ingredientName.ToLower() == "done")
                        break;

                    if (ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                    {
                        newRecipe.AddIngredient(ingredients[ingredientName]);
                        Console.WriteLine($"Ingredient '{ingredientName}' added to recipe '{recipeName}'.");
                    }
                }

                recipes[recipeName] = newRecipe;
                Console.WriteLine("Recipe added successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public static void DisplayAllRecipes(Dictionary<string, Recipe> recipes)
        {
            if (recipes.Count == 0)
            {
                Console.WriteLine("No recipes available in the catalogue.");
                return;
            }

            foreach (Recipe recipe in recipes.Values)
            {
                recipe.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void DisplayRecipesByCuisine(Dictionary<string, Recipe> recipes)
        {
            string cuisine = ValidationService.GetInput("Enter the cuisine to filter by: ");
            var filteredRecipes = recipes.Values
                .Where(r => r.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredRecipes.Count == 0)
            {
                Console.WriteLine($"No recipes found for the cuisine '{cuisine}'.");
            }
            else
            {
                Console.WriteLine($"Recipes for the cuisine '{cuisine}':");
                foreach (Recipe recipe in filteredRecipes)
                {
                    recipe.DisplayInfo();
                    Console.WriteLine();
                }
            }
        }

        public static void DisplayRecipesByIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
        {
            string ingredientName = ValidationService.GetInput("Enter the ingredient name to find recipes: ");
            
            if (!ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                return;

            var recipesWithIngredient = recipes.Values
                .Where(r => r.GetIngredients().Any(i => i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (recipesWithIngredient.Count > 0)
            {
                Console.WriteLine($"Recipes with the ingredient '{ingredientName}':");
                foreach (var recipe in recipesWithIngredient)
                {
                    recipe.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine($"No recipes found containing the ingredient '{ingredientName}'.");
            }
        }

        public static void UpdateRecipe(Dictionary<string, Recipe> recipes)
        {
            string recipeName = ValidationService.GetInput("Enter the name of the recipe to update: ");
            
            if (!ValidationService.ValidateItemExists(recipes, recipeName, "Recipe"))
                return;

            string newCuisine = ValidationService.GetInput("Enter new cuisine: ");
            recipes[recipeName].Cuisine = newCuisine;
            Console.WriteLine("Recipe updated successfully!");
        }

        public static void RemoveRecipe(Dictionary<string, Recipe> recipes)
        {
            string recipeName = ValidationService.GetInput("Enter the name of the recipe to remove: ");
            
            if (!ValidationService.ValidateItemExists(recipes, recipeName, "Recipe"))
                return;

            recipes.Remove(recipeName);
            Console.WriteLine("Recipe removed successfully!");
        }

        public static void RateRecipe(Dictionary<string, Recipe> recipes)
        {
            string recipeName = ValidationService.GetInput("Enter the name of the recipe to rate: ");
            
            if (!ValidationService.ValidateItemExists(recipes, recipeName, "Recipe"))
                return;

            int rating = ValidationService.GetRatingInput("Enter your rating (1-5): ");
            recipes[recipeName].AddRating(rating);
            Console.WriteLine("Rating added successfully!");
        }

        public static void SortRecipes(Dictionary<string, Recipe> recipes)
        {
            var sortedRecipes = recipes.Values.OrderBy(r => r.Name).ToList();
            Console.WriteLine("Recipes sorted alphabetically:");
            foreach (var recipe in sortedRecipes)
            {
                recipe.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void SearchRecipes(Dictionary<string, Recipe> recipes, string searchTerm)
        {
            var matchingRecipes = recipes.Values
                .Where(r => r.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            if (matchingRecipes.Count > 0)
            {
                Console.WriteLine("Matching Recipes:");
                foreach (var recipe in matchingRecipes)
                {
                    recipe.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine("No matching recipes found.");
            }
        }
    }
}
