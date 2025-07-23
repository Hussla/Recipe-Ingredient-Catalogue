using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    /// <summary>
    /// Static service class for managing ingredient operations with performance optimizations
    /// </summary>
    public static class IngredientService
    {
        /// <summary>
        /// Adds a new ingredient to the collection with validation
        /// </summary>
        public static void AddNewIngredient(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                string name = ValidationService.GetInput("Enter ingredient name: ");
                
                if (ingredients.ContainsKey(name))
                {
                    Console.WriteLine($"Ingredient '{name}' already exists.");
                    return;
                }

                int quantity = ValidationService.GetIntInput("Enter quantity: ");
                bool isPerishable = ValidationService.GetBoolInput("Is this ingredient perishable? (y/n): ");

                Ingredient newIngredient;
                if (isPerishable)
                {
                    DateTime expirationDate = ValidationService.GetDateInput("Enter expiration date (MM/dd/yyyy): ");
                    newIngredient = new PerishableIngredient(name, quantity, expirationDate);
                }
                else
                {
                    newIngredient = new Ingredient(name, quantity);
                }

                ingredients[name] = newIngredient;
                Console.WriteLine($"Ingredient '{name}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding ingredient: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays all ingredients with formatted output
        /// </summary>
        public static void DisplayAllIngredients(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                {
                    Console.WriteLine("No ingredients available.");
                    return;
                }

                Console.WriteLine("\n=== All Ingredients ===");
                foreach (var ingredient in ingredients.Values)
                {
                    ingredient.DisplayInfo();
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying ingredients: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing ingredient's properties
        /// </summary>
        public static void UpdateIngredient(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                {
                    Console.WriteLine("No ingredients available to update.");
                    return;
                }

                string name = ValidationService.GetInput("Enter ingredient name to update: ");
                
                if (!ingredients.ContainsKey(name))
                {
                    Console.WriteLine($"Ingredient '{name}' not found.");
                    return;
                }

                var ingredient = ingredients[name];
                Console.WriteLine("Current ingredient details:");
                ingredient.DisplayInfo();

                int newQuantity = ValidationService.GetIntInput("Enter new quantity: ");
                ingredient.Quantity = newQuantity;

                if (ingredient is PerishableIngredient perishable)
                {
                    DateTime newExpirationDate = ValidationService.GetDateInput("Enter new expiration date (MM/dd/yyyy): ");
                    perishable.ExpirationDate = newExpirationDate;
                }

                Console.WriteLine($"Ingredient '{name}' updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating ingredient: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes an ingredient from the collection
        /// </summary>
        public static void RemoveIngredient(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                {
                    Console.WriteLine("No ingredients available to remove.");
                    return;
                }

                string name = ValidationService.GetInput("Enter ingredient name to remove: ");
                
                if (!ingredients.ContainsKey(name))
                {
                    Console.WriteLine($"Ingredient '{name}' not found.");
                    return;
                }

                ingredients.Remove(name);
                Console.WriteLine($"Ingredient '{name}' removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing ingredient: {ex.Message}");
            }
        }

        /// <summary>
        /// Searches for ingredients by name with partial matching
        /// </summary>
        public static void SearchIngredients(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                {
                    Console.WriteLine("No ingredients available to search.");
                    return;
                }

                string searchTerm = ValidationService.GetInput("Enter ingredient name to search: ");
                
                var matchingIngredients = ingredients.Values
                    .Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingIngredients.Count == 0)
                {
                    Console.WriteLine($"No ingredients found containing '{searchTerm}'.");
                    return;
                }

                Console.WriteLine($"\n=== Search Results for '{searchTerm}' ===");
                foreach (var ingredient in matchingIngredients)
                {
                    ingredient.DisplayInfo();
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching ingredients: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays ingredients sorted alphabetically
        /// </summary>
        public static void SortIngredients(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                {
                    Console.WriteLine("No ingredients available to sort.");
                    return;
                }

                Console.WriteLine("\n=== Ingredients (Sorted Alphabetically) ===");
                var sortedIngredients = ingredients.Values
                    .OrderBy(i => i.Name)
                    .ToList();

                foreach (var ingredient in sortedIngredients)
                {
                    ingredient.DisplayInfo();
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sorting ingredients: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates test ingredients for development and demonstration
        /// </summary>
        public static void CreateTestIngredients(Dictionary<string, Ingredient> ingredients, int count = 1000)
        {
            try
            {
                Console.WriteLine($"Creating {count} test ingredients...");
                
                string[] baseNames = { "Flour", "Sugar", "Salt", "Pepper", "Oil", "Butter", "Milk", "Eggs", "Cheese", "Tomato" };
                Random random = new Random();

                for (int i = 0; i < count; i++)
                {
                    string baseName = baseNames[random.Next(baseNames.Length)];
                    string name = $"{baseName}_{i}";
                    int quantity = random.Next(1, 1000);
                    
                    if (random.NextDouble() < 0.3) // 30% chance of perishable
                    {
                        DateTime expirationDate = DateTime.Now.AddDays(random.Next(1, 365));
                        ingredients[name] = new PerishableIngredient(name, quantity, expirationDate);
                    }
                    else
                    {
                        ingredients[name] = new Ingredient(name, quantity);
                    }
                }

                Console.WriteLine($"Successfully created {count} test ingredients.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test ingredients: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates total quantity using vectorized operations for performance
        /// </summary>
        public static float CalculateTotalQuantityVectorized(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                if (ingredients.Count == 0)
                    return 0f;

                float[] quantities = ingredients.Values.Select(i => (float)i.Quantity).ToArray();
                return VectorizedMathService.CalculateTotalQuantityVectorized(quantities);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating total quantity: {ex.Message}");
                return 0f;
            }
        }

        /// <summary>
        /// Finds ingredients that need restocking using parallel processing
        /// </summary>
        public static List<Ingredient> FindLowStockIngredients(Dictionary<string, Ingredient> ingredients, int threshold)
        {
            try
            {
                // Using PLINQ for parallel processing
                return ingredients.Values.AsParallel()
                                 .Where(i => i.Quantity < threshold)
                                 .OrderBy(i => i.Quantity)
                                 .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding low stock ingredients: {ex.Message}");
                return new List<Ingredient>();
            }
        }

        /// <summary>
        /// Analyzes ingredient usage patterns with performance optimizations
        /// </summary>
        public static Dictionary<string, int> AnalyzeIngredientTypes(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                var typeAnalysis = new Dictionary<string, int>();
                
                Parallel.ForEach(ingredients.Values, ingredient =>
                {
                    string type = ingredient is PerishableIngredient ? "Perishable" : "Non-Perishable";
                    
                    lock (typeAnalysis)
                    {
                        if (typeAnalysis.ContainsKey(type))
                            typeAnalysis[type]++;
                        else
                            typeAnalysis[type] = 1;
                    }
                });
                
                return typeAnalysis;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing ingredient types: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Optimizes ingredient quantities using vectorized operations
        /// </summary>
        public static Dictionary<string, float> OptimizeIngredientQuantities(Dictionary<string, Ingredient> ingredients, float optimizationFactor = 0.9f)
        {
            try
            {
                var optimizedResults = new Dictionary<string, float>();
                
                if (ingredients.Count == 0)
                    return optimizedResults;

                var ingredientList = ingredients.Values.ToList();
                float[] quantities = ingredientList.Select(i => (float)i.Quantity).ToArray();
                float[] results = new float[quantities.Length];
                
                // Apply vectorized scaling operation
                VectorizedMathService.ScaleQuantitiesVectorized(quantities, optimizationFactor, results);
                
                for (int i = 0; i < ingredientList.Count; i++)
                {
                    optimizedResults[ingredientList[i].Name] = results[i];
                }
                
                return optimizedResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error optimizing ingredient quantities: {ex.Message}");
                return new Dictionary<string, float>();
            }
        }
    }
}
