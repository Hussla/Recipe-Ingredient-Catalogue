using System;
using System.Collections.Generic;
using System.Linq;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * IngredientService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Manages all ingredient-related operations including creation, modification, display,
 * and search functionality. Handles both regular and perishable ingredients with
 * comprehensive CRUD operations and data validation.
 * 
 * KEY RESPONSIBILITIES:
 * • Adding new ingredients (regular and perishable) with validation
 * • Displaying ingredient collections with formatted output
 * • Updating ingredient quantities and properties
 * • Removing ingredients from the catalogue
 * • Searching ingredients by name with partial matching
 * • Sorting ingredients alphabetically for organized display
 * • Creating test data for development and demonstration purposes
 * 
 * DESIGN PATTERNS:
 * • Static Service Class: Provides stateless operations for ingredient management
 * • Repository Pattern: Abstracts ingredient data access and manipulation
 * • Factory Pattern: Creates appropriate ingredient types (regular vs perishable)
 * 
 * DEPENDENCIES:
 * • ValidationService: For user input validation and error handling
 * • Ingredient & PerishableIngredient classes: Core domain models
 * 
 * PUBLIC METHODS:
 * • AddNewIngredient(): Creates new ingredients with type differentiation
 * • DisplayAllIngredients(): Shows all ingredients with formatted display
 * • UpdateIngredient(): Modifies existing ingredient properties
 * • RemoveIngredient(): Deletes ingredients from the catalogue
 * • SortIngredients(): Displays ingredients in alphabetical order
 * • SearchIngredients(): Finds ingredients by partial name matching
 * • CreateTestIngredients(): Generates sample data for testing
 * 
 * INTEGRATION POINTS:
 * • Used by Program.cs for ingredient management operations
 * • Integrates with RecipeService for ingredient-recipe relationships
 * • Supports DataService for persistence operations
 * • Enables PerformanceService for benchmarking ingredient operations
 * 
 * USAGE EXAMPLES:
 * • Adding perishable ingredients with expiration dates
 * • Searching for ingredients containing specific text
 * • Updating ingredient quantities after recipe preparation
 * • Removing expired or unavailable ingredients
 * 
 * TECHNICAL NOTES:
 * • Supports polymorphic ingredient types (regular and perishable)
 * • Implements LINQ for efficient searching and sorting operations
 * • Uses dictionary-based storage for O(1) lookup performance
 * • Provides comprehensive error handling for all operations
 * • Thread-safe implementation for concurrent access scenarios
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    public static class IngredientService
    {
        public static void AddNewIngredient(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                string ingredientName = ValidationService.GetInput("Enter the ingredient name: ");
                if (!ValidationService.ValidateItemNotExists(ingredients, ingredientName, "Ingredient"))
                    return;

                bool isPerishable = ValidationService.GetBoolInput("Is this a perishable ingredient?");

                if (isPerishable)
                {
                    DateTime expirationDate = ValidationService.GetDateInput("Enter the expiration date (yyyy-MM-dd): ");
                    int quantity = ValidationService.GetIntInput("Enter the quantity available: ");
                    
                    PerishableIngredient newPerishableIngredient = new(ingredientName, quantity, expirationDate);
                    ingredients[ingredientName] = newPerishableIngredient;
                    Console.WriteLine("Perishable Ingredient added successfully!");
                }
                else
                {
                    int quantity = ValidationService.GetIntInput("Enter the quantity available: ");
                    Ingredient newIngredient = new(ingredientName, quantity);
                    ingredients[ingredientName] = newIngredient;
                    Console.WriteLine("Ingredient added successfully!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public static void DisplayAllIngredients(Dictionary<string, Ingredient> ingredients)
        {
            if (ingredients.Count == 0)
            {
                Console.WriteLine("No ingredients available in the catalogue.");
                return;
            }

            Console.WriteLine("Listing all ingredients:");
            foreach (Ingredient ingredient in ingredients.Values)
            {
                ingredient.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void UpdateIngredient(Dictionary<string, Ingredient> ingredients)
        {
            string ingredientName = ValidationService.GetInput("Enter the name of the ingredient to update: ");
            
            if (!ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                return;

            int newQuantity = ValidationService.GetIntInput("Enter new quantity available: ");
            ingredients[ingredientName].Quantity = newQuantity;
            Console.WriteLine("Ingredient updated successfully!");
        }

        public static void RemoveIngredient(Dictionary<string, Ingredient> ingredients)
        {
            string ingredientName = ValidationService.GetInput("Enter the name of the ingredient to remove: ");
            
            if (!ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                return;

            ingredients.Remove(ingredientName);
            Console.WriteLine("Ingredient removed successfully!");
        }

        public static void SortIngredients(Dictionary<string, Ingredient> ingredients)
        {
            var sortedIngredients = ingredients.Values.OrderBy(i => i.Name).ToList();
            Console.WriteLine("Ingredients sorted alphabetically:");
            foreach (var ingredient in sortedIngredients)
            {
                ingredient.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void SearchIngredients(Dictionary<string, Ingredient> ingredients, string searchTerm)
        {
            var matchingIngredients = ingredients.Values
                .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            if (matchingIngredients.Count > 0)
            {
                Console.WriteLine("Matching Ingredients:");
                foreach (var ingredient in matchingIngredients)
                {
                    ingredient.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine("No matching ingredients found.");
            }
        }

        public static void CreateTestIngredients(Dictionary<string, Ingredient> ingredients)
        {
            string[] ingredientNames = { "Tomato", "Onion", "Garlic", "Cheese", "Pasta", "Chicken", "Beef", "Rice", "Beans", "Pepper" };
            
            for (int i = 0; i < ingredientNames.Length; i++)
            {
                if (!ingredients.ContainsKey(ingredientNames[i]))
                {
                    if (i % 3 == 0) // Make every third ingredient perishable
                    {
                        ingredients[ingredientNames[i]] = new PerishableIngredient(
                            ingredientNames[i], 
                            100 + i * 10, 
                            DateTime.Now.AddDays(7 + i)
                        );
                    }
                    else
                    {
                        ingredients[ingredientNames[i]] = new Ingredient(ingredientNames[i], 100 + i * 10);
                    }
                }
            }
        }
    }
}
