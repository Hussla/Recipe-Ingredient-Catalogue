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
using RecipeIngredientCatalogue.Patterns;
using RecipeIngredientCatalogue.CLI;
using RecipeIngredientCatalogue.Interfaces;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * Program.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Main entry point and orchestration layer for the Recipe Ingredient Catalogue
 * application. Manages user authentication, application lifecycle, menu navigation,
 * and coordinates interactions between all service layers.
 * 
 * KEY RESPONSIBILITIES:
 * • Application startup and command-line argument processing
 * • User authentication and role-based access control (admin vs user)
 * • Main application loop and menu-driven navigation
 * • Coordinating service layer operations for business logic
 * • Managing application state and data collections
 * • Handling user choice routing and operation delegation
 * • Running automated tests for core domain classes
 * • Providing legacy JSON loading functionality
 * 
 * DESIGN PATTERNS:
 * • Main Controller: Orchestrates application flow and user interactions
 * • Command Pattern: Maps menu choices to specific operations
 * • Facade Pattern: Provides simplified interface to complex service operations
 * • Strategy Pattern: Implements role-based menu and operation differentiation
 * 
 * DEPENDENCIES:
 * • MenuService: For user interface navigation and input validation
 * • ValidationService: For user input collection and validation
 * • RecipeService: For all recipe-related operations
 * • IngredientService: For all ingredient-related operations
 * • DataService: For data persistence and serialization
 * • PerformanceService: For benchmarking and performance analysis
 * • Recipe & Ingredient classes: Core domain models
 * 
 * PUBLIC METHODS:
 * • Main(): Application entry point with command-line argument processing
 * 
 * PRIVATE METHODS:
 * • DisplayMenu(): Delegates menu display to MenuService
 * • GetUserChoice(): Delegates user input to MenuService
 * • HandleUserChoice(): Routes user selections to appropriate service operations
 * • LoadRecipesAndIngredients(): Legacy JSON loading functionality
 * • SearchRecipesOrIngredients(): Coordinates search across both collections
 * • UpdateRecipeOrIngredientInformation(): Handles update operations
 * • RemoveRecipeOrIngredient(): Handles deletion operations
 * • SortRecipesOrIngredients(): Handles sorting operations
 * • ExitProgram(): Graceful application termination
 * • GetInput(): Legacy input helper method
 * 
 * INTEGRATION POINTS:
 * • Serves as the main orchestration layer for all application operations
 * • Coordinates between UI layer (MenuService) and business logic (Services)
 * • Manages application state through dictionary collections
 * • Provides role-based operation routing and access control
 * 
 * USAGE EXAMPLES:
 * • dotnet run admin - Launches application in administrator mode
 * • dotnet run user - Launches application in read-only user mode
 * • Handles menu navigation and operation delegation
 * • Manages data persistence and loading operations
 * 
 * TECHNICAL NOTES:
 * • Implements role-based access control with admin/user differentiation
 * • Uses Dictionary<string, T> for efficient O(1) data lookups
 * • Provides comprehensive error handling and user feedback
 * • Supports both JSON and binary data persistence formats
 * • Includes automated testing execution on startup
 * • Maintains separation of concerns through service delegation
 * • Thread-safe implementation for concurrent operations
 * • Supports extension methods for .NET compatibility (Chunk)
 * 
 * COMMAND LINE USAGE:
 * • Required argument: mode ("admin" or "user")
 * • Admin mode: Full CRUD operations and advanced features
 * • User mode: Read-only operations and basic functionality
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

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
        Console.WriteLine("Running tests for Ingredient class...");
        Ingredient.RunTests(); // Run tests for Ingredient class

        Console.WriteLine("Running tests for PerishableIngredient class...");
        PerishableIngredient.RunTests(); // Run tests for PerishableIngredient class

        Console.WriteLine("Running tests for RefrigeratedIngredient class...");
        RefrigeratedIngredient.RunTests(); // Run tests for RefrigeratedIngredient class

        Console.WriteLine("Running tests for FrozenIngredient class...");
        FrozenIngredient.RunTests(); // Run tests for FrozenIngredient class

        Console.WriteLine("Running tests for Recipe class...");
        Recipe.RunTests(); // Run tests for Recipe class

        Console.WriteLine("Running tests for IngredientFactory...");
        RecipeIngredientCatalogue.Patterns.IngredientFactory.RunTests();

        Console.WriteLine("Running tests for Visitor Pattern...");
        RecipeIngredientCatalogue.Patterns.VisitorUtility.RunTests();

        Console.WriteLine("Running tests for Strategy Pattern...");
        RecipeIngredientCatalogue.Patterns.ProcessorUtility.RunTests();

        Console.WriteLine("Running tests for CommandLineParser...");
        RecipeIngredientCatalogue.CLI.CommandLineParser.RunTests();

        Console.WriteLine("All tests have been executed.");

        // Display a welcome message and instructions for using the program
        Console.WriteLine("\nWelcome to the Recipe and Ingredients Catalogue!");
        Console.WriteLine("=========================================");
        Console.WriteLine("Multi-User Recipe Management System");
        Console.WriteLine("Please log in or register to continue.");

        // Handle user authentication
        if (!HandleAuthentication())
        {
            Console.WriteLine("Authentication failed. Exiting application.");
            return;
        }

        // Get user role for admin privileges
        bool isAdmin = AuthService.GetCurrentUser().Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        
        Console.WriteLine($"\nWelcome, {AuthService.GetCurrentUser().Username}!");
        Console.WriteLine($"Role: {AuthService.GetCurrentUser().Role}");
        Console.WriteLine("Use this program to manage your collection of recipes and ingredients.");

        // Load user-specific data
        Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
        Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();
        LoadUserData(recipes, ingredients);

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
                if (isAdmin) 
                {
                    IngredientService.AddNewIngredient(ingredients);
                    SaveUserData(recipes, ingredients);
                }
                else IngredientService.DisplayAllIngredients(ingredients);
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
                if (isAdmin) 
                {
                    SaveUserData(recipes, ingredients);
                    AuthService.Logout();
                    Console.WriteLine("Logged out successfully. Exiting application.");
                    Environment.Exit(0);
                }
                else LoadRecipesAndIngredients(recipes, ingredients);
                break;
            case "7":
                if (isAdmin) 
                {
                    RecipeService.AddNewRecipe(recipes, ingredients);
                    SaveUserData(recipes, ingredients);
                }
                else 
                {
                    SaveUserData(recipes, ingredients);
                    AuthService.Logout();
                    Console.WriteLine("Logged out successfully. Exiting application.");
                    Environment.Exit(0);
                }
                break;
            case "8":
                IngredientService.AddNewIngredient(ingredients);
                SaveUserData(recipes, ingredients);
                break;
            case "9":
                IngredientService.DisplayAllIngredients(ingredients);
                break;
            case "10":
                UpdateRecipeOrIngredientInformation(recipes, ingredients);
                SaveUserData(recipes, ingredients);
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
                SaveUserData(recipes, ingredients);
                break;
            case "15":
                RecipeService.RateRecipe(recipes);
                SaveUserData(recipes, ingredients);
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
                SaveUserData(recipes, ingredients);
                AuthService.Logout();
                Console.WriteLine("Logged out successfully. Exiting application.");
                Environment.Exit(0);
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
        IngredientService.SearchIngredients(ingredients);
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

    // Handle user authentication (login/register)
    static bool HandleAuthentication()
    {
        while (true)
        {
            Console.WriteLine("\n=== Authentication ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option (1-3): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (HandleLogin())
                        return true;
                    break;
                case "2":
                    HandleRegistration();
                    break;
                case "3":
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Handle user login
    static bool HandleLogin()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine();

        return AuthService.Login(username, password);
    }

    // Handle user registration
    static void HandleRegistration()
    {
        Console.Write("Choose a username: ");
        string username = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        Console.Write("Choose a password: ");
        string password = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        Console.Write("Are you an admin? (y/n): ");
        string roleInput = Console.ReadLine();
        string role = roleInput?.ToLower() == "y" ? "Admin" : "User";

        if (AuthService.Register(username, password, role))
        {
            Console.WriteLine("Registration successful! You can now log in.");
        }
    }

    // Load user-specific data
    static void LoadUserData(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string userDataFile = $"user_data_{AuthService.GetCurrentUser().Id}.json";
        if (File.Exists(userDataFile))
        {
            try
            {
                Console.WriteLine("Loading your personal data...");
                DataService.LoadDataFromJsonFile(recipes, ingredients, userDataFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
                Console.WriteLine("Starting with empty data.");
            }
        }
        else
        {
            Console.WriteLine("No previous data found. Starting with empty catalogue.");
        }
    }

    // Save user-specific data
    static void SaveUserData(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string userDataFile = $"user_data_{AuthService.GetCurrentUser().Id}.json";
        try
        {
            DataService.SaveDataToJsonFile(recipes, ingredients, userDataFile);
            Console.WriteLine("Your data has been saved automatically.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user data: {ex.Message}");
        }
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
        - Parameterised constructors for proper object initialisation
        - Base class constructor calls demonstrating inheritance chain (PerishableIngredient : Ingredient)
        - Virtual method implementation for polymorphic behavior

    5. Data Persistence - Serialisation and Binary Files:
        - Dual serialization support: JSON (human-readable) and Binary (compact storage)
        - System.Text.Json implementation with custom JsonSerializerOptions
        - File I/O operations with comprehensive error handling and validation
        - CatalogueData class for structured data export/import operations

    6. Writing Fast Code - Performance Optimisation and Parallel Processing:
        - Built-in performance benchmarking using Stopwatch for timing analysis
        - Memory profiling with GC.GetTotalMemory() for optimization insights
        - PLINQ (Parallel LINQ) implementation with .AsParallel() for multi-threaded operations
        - Parallel.ForEach and Task-based processing for concurrent recipe handling
        - Multi-core utilization with Environment.ProcessorCount detection
        - Custom extension methods (Chunk) for .NET compatibility
        - Multiple file format support (JSON, Binary)
        - Structured report generation with sorted data presentation
        - File existence validation and error recovery mechanisms
        - Comprehensive logging and user feedback during operations

*/
