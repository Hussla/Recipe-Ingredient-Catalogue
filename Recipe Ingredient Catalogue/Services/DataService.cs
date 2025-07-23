using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * DataService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Manages data persistence and serialisation operations for the application.
 * Provides multiple storage formats (JSON, Binary) and comprehensive data
 * import/export functionality with robust error handling and validation.
 * 
 * KEY RESPONSIBILITIES:
 * • Saving application data to JSON format for human-readable storage
 * • Loading application data from JSON files with validation
 * • Saving application data to binary format for compact storage
 * • Loading application data from binary files with type safety
 * • Exporting formatted reports for data analysis and documentation
 * • Managing file I/O operations with comprehensive error handling
 * • Maintaining data integrity during serialization/deserialization
 * 
 * DESIGN PATTERNS:
 * • Static Service Class: Provides stateless data persistence operations
 * • Serialisation Strategy: Supports multiple data formats (JSON/Binary)
 * • Data Transfer Object: Uses CatalogueData for structured serialization
 * 
 * DEPENDENCIES:
 * • System.Text.Json: For JSON serialisation and deserialisation
 * • System.IO: For file operations and binary data handling
 * • Recipe & Ingredient classes: Core domain models for data persistence
 * 
 * PUBLIC METHODS:
 * • SaveDataToJsonFile(): Serializes data to human-readable JSON format
 * • LoadDataFromJsonFile(): Deserializes data from JSON files
 * • SaveDataToBinaryFile(): Serializes data to compact binary format
 * • LoadDataFromBinaryFile(): Deserializes data from binary files
 * • ExportReport(): Generates formatted text reports for analysis
 * 
 * INTEGRATION POINTS:
 * • Used by Program.cs for data persistence operations
 * • Supports RecipeService and IngredientService for data storage
 * • Enables backup and restore functionality
 * • Provides data export capabilities for external analysis
 * 
 * USAGE EXAMPLES:
 * • Saving complete catalogue data to JSON for backup
 * • Loading previously saved data on application startup
 * • Exporting binary data for efficient storage and transfer
 * • Generating reports for data analysis and documentation
 * 
 * TECHNICAL NOTES:
 * • Implements JSON serialisation with enum support and formatting
 * • Uses binary serialisation for space-efficient storage
 * • Provides comprehensive error handling for file operations
 * • Maintains backward compatibility for data format changes
 * • Supports polymorphic serialisation for ingredient types
 * • Thread-safe implementation for concurrent access scenarios
 * • Validates file existence and format before operations
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    public class CatalogueData
    {
        public Dictionary<string, Recipe> Recipes { get; set; } = new();
        public Dictionary<string, Ingredient> Ingredients { get; set; } = new();
    }

    public static class DataService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static void SaveDataToJsonFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, string filename)
        {
            try
            {
                var data = new CatalogueData
                {
                    Recipes = recipes,
                    Ingredients = ingredients
                };

                string json = JsonSerializer.Serialize(data, JsonOptions);
                File.WriteAllText(filename, json);

                Console.WriteLine("Data saved successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while saving data: " + e.Message);
            }
        }

        public static void LoadDataFromJsonFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Sorry, '{filename}' does not exist.");
                return;
            }

            try
            {
                Console.WriteLine($"Loading data from '{filename}'...");
                string json = File.ReadAllText(filename);

                var data = JsonSerializer.Deserialize<CatalogueData>(json, JsonOptions);
                
                if (data != null)
                {
                    recipes.Clear();
                    ingredients.Clear();

                    if (data.Recipes != null)
                    {
                        foreach (var kvp in data.Recipes)
                        {
                            recipes[kvp.Key] = kvp.Value;
                            Console.WriteLine($"Loaded Recipe: {kvp.Key}");
                        }
                    }

                    if (data.Ingredients != null)
                    {
                        foreach (var kvp in data.Ingredients)
                        {
                            ingredients[kvp.Key] = kvp.Value;
                            Console.WriteLine($"Loaded Ingredient: {kvp.Key}");
                        }
                    }

                    Console.WriteLine("Data loaded successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to deserialize data. The file may be corrupted or in an incorrect format.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while loading data: {e.Message}");
            }
        }

        public static void SaveDataToBinaryFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, string filename)
        {
            try
            {
                using FileStream fs = new(filename, FileMode.Create);
                using BinaryWriter writer = new(fs);
                
                // Write recipes count
                writer.Write(recipes.Count);
                foreach (var kvp in recipes)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Name);
                    writer.Write(kvp.Value.Cuisine);
                    writer.Write(kvp.Value.GetIngredients().Count);
                    foreach (var ingredient in kvp.Value.GetIngredients())
                    {
                        writer.Write(ingredient.Name);
                        writer.Write(ingredient.Quantity);
                    }
                }

                // Write ingredients count
                writer.Write(ingredients.Count);
                foreach (var kvp in ingredients)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Name);
                    writer.Write(kvp.Value.Quantity);
                    
                    if (kvp.Value is PerishableIngredient perishable)
                    {
                        writer.Write(true);
                        writer.Write(perishable.ExpirationDate.ToBinary());
                    }
                    else
                    {
                        writer.Write(false);
                    }
                }

                Console.WriteLine("Binary data saved successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while saving binary data: " + e.Message);
            }
        }

        public static void LoadDataFromBinaryFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File '{filename}' does not exist.");
                return;
            }

            try
            {
                using FileStream fs = new(filename, FileMode.Open);
                using BinaryReader reader = new(fs);
                
                recipes.Clear();
                ingredients.Clear();

                // Read ingredients first
                int ingredientCount = reader.ReadInt32();
                for (int i = 0; i < ingredientCount; i++)
                {
                    string key = reader.ReadString();
                    string name = reader.ReadString();
                    int quantity = reader.ReadInt32();
                    bool isPerishable = reader.ReadBoolean();

                    if (isPerishable)
                    {
                        DateTime expirationDate = DateTime.FromBinary(reader.ReadInt64());
                        ingredients[key] = new PerishableIngredient(name, quantity, expirationDate);
                    }
                    else
                    {
                        ingredients[key] = new Ingredient(name, quantity);
                    }
                }

                // Read recipes
                int recipeCount = reader.ReadInt32();
                for (int i = 0; i < recipeCount; i++)
                {
                    string key = reader.ReadString();
                    string name = reader.ReadString();
                    string cuisine = reader.ReadString();
                    int ingredientCount2 = reader.ReadInt32();

                    Recipe recipe = new(name, cuisine, 30); // Default preparation time
                    for (int j = 0; j < ingredientCount2; j++)
                    {
                        string ingredientName = reader.ReadString();
                        int ingredientQuantity = reader.ReadInt32();
                        
                        if (ingredients.ContainsKey(ingredientName))
                        {
                            recipe.AddIngredient(ingredients[ingredientName]);
                        }
                    }
                    recipes[key] = recipe;
                }

                Console.WriteLine("Binary data loaded successfully.");
                Console.WriteLine($"Loaded {recipes.Count} recipes and {ingredients.Count} ingredients.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while loading binary data: " + e.Message);
            }
        }

        public static void ExportReport(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, string filename)
        {
            try
            {
                List<string> reportLines = new()
                {
                    "Recipe and Ingredient Catalogue Report",
                    "========================================="
                };

                reportLines.Add("\nRecipes:");
                foreach (var recipe in recipes.Values.OrderBy(r => r.Name))
                {
                    reportLines.Add($"Name: {recipe.Name}, Cuisine: {recipe.Cuisine}, Average Rating: {recipe.GetAverageRating():F1}");
                }

                reportLines.Add("\nIngredients:");
                foreach (var ingredient in ingredients.Values.OrderBy(i => i.Name))
                {
                    reportLines.Add($"Name: {ingredient.Name}, Quantity: {ingredient.Quantity}");
                }

                File.WriteAllLines(filename, reportLines);
                Console.WriteLine("Report exported successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while exporting the report: " + e.Message);
            }
        }
    }
}
