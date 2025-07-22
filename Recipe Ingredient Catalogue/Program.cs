using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RecipeIngredientCatalogue.Services;

// Extension method for Chunk functionality (for older .NET versions)
public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
    {
        if (size <= 0) throw new ArgumentException("Size must be greater than 0", nameof(size));
        
        var list = source.ToList();
        for (int i = 0; i < list.Count; i += size)
        {
            yield return list.Skip(i).Take(size);
        }
    }
}


// Main Program Class
class Program
{
    static void Main(string[] args)
    {
        // User authentication mode setup
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: RecipeCatalogue <mode>"); // for example: dotnet run --project "Recipe Ingredient Catalogue/Recipe Ingredient Catalogue.csproj" admin || dotnet run --project "Recipe Ingredient Catalogue/Recipe Ingredient Catalogue.csproj" user

                                                                                


            Console.WriteLine("<mode> should be either 'admin' or 'user'");
            return;
        }

        string mode = args[0].ToLower();
        bool isAdmin = mode == "admin";

        if (!isAdmin && mode != "user")
        {
            Console.WriteLine("Invalid mode. Use 'admin' for full privileges or 'user' for read-only access.");
            return;
        }

        Console.WriteLine("Running tests for Ingredient class...");
        Ingredient.RunTests(); // Run tests for Ingredient class

        Console.WriteLine("Running tests for Recipe class...");
        Recipe.RunTests(); // Run tests for Recipe class

        Console.WriteLine("All tests have been executed.");

        // Display a welcome message and instructions for using the program
        Console.WriteLine("\nWelcome to the Recipe and Ingredients Catalogue!");
        Console.WriteLine("=========================================");
        Console.WriteLine("Use this program to manage your collection of recipes and ingredients.");
        Console.WriteLine("Choose an option from the menu below:");

        // Initialise dictionary to store recipes and ingredients
        Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
        Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();

        // Start the main program loop
        while (true)
        {
            // Display the menu options to the user
            DisplayMenu(isAdmin);
            // Get the user's menu choice
            string choice = GetUserChoice(isAdmin);
            // Handle the user's menu choice
            HandleUserChoice(choice, recipes, ingredients, isAdmin);
        }
    }

    // Displays the menu options to the user
    static void DisplayMenu(bool isAdmin)
    {
        MenuService.DisplayMenu(isAdmin);
    }

    // Gets the user's menu choice
    static string GetUserChoice(bool isAdmin)
    {
        return MenuService.GetUserChoice(isAdmin);
    }

    // Handles the user's menu choice
    static void HandleUserChoice(string choice, Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, bool isAdmin)
    {
        if (!MenuService.ValidateAdminPermission(choice, isAdmin))
            return;

        switch (choice)
        {
            case "1":
                RecipeService.DisplayAllRecipes(recipes);
                break;
            case "2":
                if (isAdmin) IngredientService.AddNewIngredient(ingredients); else IngredientService.DisplayAllIngredients(ingredients);
                break;
            case "3":
                if (isAdmin) RecipeService.DisplayAllRecipes(recipes); else RecipeService.DisplayRecipesByCuisine(recipes);
                break;
            case "4":
                if (isAdmin) RecipeService.DisplayRecipesByCuisine(recipes); else SearchRecipesOrIngredients(recipes, ingredients);
                break;
            case "5":
                if (isAdmin) LoadRecipesAndIngredients(recipes, ingredients); else RecipeService.DisplayRecipesByIngredient(recipes, ingredients);
                break;
            case "6":
                if (isAdmin) ExitProgram(); else LoadRecipesAndIngredients(recipes, ingredients);
                break;
            case "7":
                if (isAdmin) RecipeService.AddNewRecipe(recipes, ingredients); else ExitProgram();
                break;
            case "8":
                IngredientService.AddNewIngredient(ingredients);
                break;
            case "9":
                IngredientService.DisplayAllIngredients(ingredients);
                break;
            case "10":
                UpdateRecipeOrIngredientInformation(recipes, ingredients);
                break;
            case "11":
                DataService.SaveDataToJsonFile(recipes, ingredients, ValidationService.GetInput("Enter the filename to save data to (e.g., data.json): "));
                break;
            case "12":
                DataService.SaveDataToBinaryFile(recipes, ingredients, ValidationService.GetInput("Enter the filename to save binary data to (e.g., data.dat): "));
                break;
            case "13":
                DataService.LoadDataFromBinaryFile(recipes, ingredients, ValidationService.GetInput("Enter the filename to load binary data from (e.g., data.dat): "));
                break;
            case "14":
                RemoveRecipeOrIngredient(recipes, ingredients);
                break;
            case "15":
                RecipeService.RateRecipe(recipes);
                break;
            case "16":
                SortRecipesOrIngredients(recipes, ingredients);
                break;
            case "17":
                DataService.ExportReport(recipes, ingredients, ValidationService.GetInput("Enter the filename to export the report to: "));
                break;
            case "18":
                PerformanceService.RunPerformanceBenchmark(recipes, ingredients);
                break;
            case "19":
                PerformanceService.RunParallelProcessingDemo(recipes, ingredients);
                break;
            case "20":
                ExitProgram();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }


static void LoadRecipesAndIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    // Function to load recipes and ingredients from a JSON file
    string filename = GetInput("What file would you like to load? (e.g., data.json): ");

    if (File.Exists(filename))
    {
        try
        {
            Console.WriteLine($"Loading data from '{filename}'...");
            string json = File.ReadAllText(filename);

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            var data = JsonSerializer.Deserialize<CatalogueData>(json, options);
            
            if (data != null)
            {
                // Clear existing data
                recipes.Clear();
                ingredients.Clear();

                // Load recipes
                if (data.Recipes != null)
                {
                    foreach (var kvp in data.Recipes)
                    {
                        recipes[kvp.Key] = kvp.Value;
                        Console.WriteLine($"Loaded Recipe: {kvp.Key}");
                    }
                }

                // Load ingredients
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
            Console.WriteLine($"Stack trace: {e.StackTrace}");
        }
    }
    else
    {
        Console.WriteLine($"Sorry, '{filename}' does not exist.");
    }
}

    static void SearchRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string searchTerm = ValidationService.GetInput("Enter search term (recipe name or ingredient name): ");
        RecipeService.SearchRecipes(recipes, searchTerm);
        IngredientService.SearchIngredients(ingredients, searchTerm);
    }

    static void UpdateRecipeOrIngredientInformation(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string choice = ValidationService.GetInput("Do you want to update a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'): ").ToLower();

        if (choice == "recipe")
        {
            RecipeService.UpdateRecipe(recipes);
        }
        else if (choice == "ingredient")
        {
            IngredientService.UpdateIngredient(ingredients);
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
        }
    }

// Class to hold the serialized data structure
public class CatalogueData
{
    public Dictionary<string, Recipe> Recipes { get; set; }
    public Dictionary<string, Ingredient> Ingredients { get; set; }
}

    static void RemoveRecipeOrIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string choice = ValidationService.GetInput("Do you want to remove a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'): ").ToLower();

        if (choice == "recipe")
        {
            RecipeService.RemoveRecipe(recipes);
        }
        else if (choice == "ingredient")
        {
            IngredientService.RemoveIngredient(ingredients);
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
        }
    }


static void SortRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string choice = ValidationService.GetInput("Do you want to sort Recipes or Ingredients? (Enter 'Recipe' or 'Ingredient'): ").ToLower();

    if (choice == "recipe")
    {
        RecipeService.SortRecipes(recipes);
    }
    else if (choice == "ingredient")
    {
        IngredientService.SortIngredients(ingredients);
    }
    else
    {
        Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
    }
}

    static void ExitProgram()
    {
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }

    // Helper method that's still used in LoadRecipesAndIngredients
    static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}



 /*
 
    Explanation of Systems Programming Topics Implemented:
    
    1. Console IO and Variables:
        - Extensive use of Console.WriteLine() and Console.ReadLine() for interactive user interface
        - Robust input validation with helper methods like GetIntInput() for numeric input
        - Dictionary and List data structures for efficient O(1) lookups and dynamic collections
        - State management variables maintaining application workflow across operations

    2. Command Line Interfaces:
        - Sophisticated menu-driven navigation with 20 admin operations and 6 user operations
        - Command pattern implementation mapping menu options to dedicated methods
        - User authentication system with admin/user mode differentiation
        - Comprehensive user guidance and graceful exit handling

    3. Robustness and Error Handling:
        - Structured exception handling with try-catch blocks in critical operations
        - Input validation including range checks (ratings 1-5) and existence verification
        - Specific error messages for different failure scenarios
        - Debug.Assert statements for comprehensive unit testing

    4. Object-Oriented Programming (OOP):
        - Inheritance hierarchy with base Ingredient class extended by PerishableIngredient
        - Polymorphism through virtual/override DisplayInfo() methods
        - Encapsulation with private fields and controlled access through public methods
        - Static testing methods for comprehensive class validation
        - Clean class design with public properties and proper getter/setter methods
        - Parameterized constructors for proper object initialization
        - Base class constructor calls demonstrating inheritance chain (PerishableIngredient : Ingredient)
        - Virtual method implementation for polymorphic behavior

    5. Data Persistence and Serialization:
        - Dual serialization support: JSON (human-readable) and Binary (compact storage)
        - System.Text.Json implementation with custom JsonSerializerOptions
        - File I/O operations with comprehensive error handling and validation
        - CatalogueData class for structured data export/import operations

    6. Performance Optimization and Parallel Processing:
        - Built-in performance benchmarking using Stopwatch for timing analysis
        - Memory profiling with GC.GetTotalMemory() for optimization insights
        - PLINQ (Parallel LINQ) implementation with .AsParallel() for multi-threaded operations
        - Parallel.ForEach and Task-based processing for concurrent recipe handling
        - Multi-core utilization with Environment.ProcessorCount detection
        - Custom extension methods (Chunk) for .NET compatibility
        - Multiple file format support (JSON, Binary, Text reports)
        - Structured report generation with sorted data presentation
        - File existence validation and error recovery mechanisms
        - Comprehensive logging and user feedback during operations

*/
