using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading;

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
            Console.WriteLine("Usage: RecipeCatalogue <mode>"); // for example: dotnet run --project "Recipe Ingredient Catalogue" admin || user

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
        if (isAdmin)
        {
            Console.WriteLine("7 - Add a new Recipe");
            Console.WriteLine("8 - Add a new Ingredient");
            Console.WriteLine("9 - Display all Ingredients");
            Console.WriteLine("10 - Update Recipe or Ingredient Information");
            Console.WriteLine("11 - Save Data (JSON)");
            Console.WriteLine("12 - Save Data (Binary)");
            Console.WriteLine("13 - Load Data (Binary)");
            Console.WriteLine("14 - Remove Recipe or Ingredient");
            Console.WriteLine("15 - Rate a Recipe");
            Console.WriteLine("16 - Sort Recipes or Ingredients");
            Console.WriteLine("17 - Export Report");
            Console.WriteLine("18 - Performance Benchmark");
            Console.WriteLine("19 - Parallel Processing Demo");
            Console.WriteLine("20 - Exit");

        }
        else
        {
            Console.WriteLine("1 - Display all Recipes");
            Console.WriteLine("2 - Display all Ingredients");
            Console.WriteLine("3 - Display Recipes by Cuisine");
            Console.WriteLine("4 - Search Recipes or Ingredients");
            Console.WriteLine("5 - Display Recipes by Ingredient");
            Console.WriteLine("6 - Load Recipes and Ingredients");
            Console.WriteLine("7 - Exit");

        }
        Console.WriteLine("=========================================");
    }

    // Gets the user's menu choice
    static string GetUserChoice(bool isAdmin)
    {
        string choice;
        while (true)
        {
            Console.Write("Enter your choice: ");
            choice = Console.ReadLine();

            // Validate user input
            if (int.TryParse(choice, out int numChoice) && numChoice >= 1 && (numChoice <= 7 || (isAdmin && numChoice <= 20)))
            {
                return choice;
            }
            else
            {
                Console.WriteLine($"Invalid choice. Please enter a number between 1 and {(isAdmin ? "20" : "7")}.");
            }
        }
    }

    // Handles the user's menu choice
    static void HandleUserChoice(string choice, Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, bool isAdmin)
    {
        if (!isAdmin && int.TryParse(choice, out int numChoice) && numChoice > 6 && numChoice != 7)
        {
            Console.WriteLine("You do not have permission to perform this action. Please contact an administrator.");
            return;
        }

        switch (choice)
        {
            case "1":
                DisplayAllRecipes(recipes);
                break;
            case "2":
                if (isAdmin) AddNewIngredient(ingredients); else DisplayAllIngredients(ingredients);
                break;
            case "3":
                if (isAdmin) DisplayAllRecipes(recipes); else DisplayRecipesByCuisine(recipes);
                break;
            case "4":
                if (isAdmin) DisplayRecipesByCuisine(recipes); else SearchRecipesOrIngredients(recipes, ingredients);
                break;
            case "5":
                if (isAdmin) LoadRecipesAndIngredients(recipes, ingredients); else DisplayRecipesByIngredient(recipes, ingredients);
                break;
            case "6":
                if (isAdmin) ExitProgram(); else LoadRecipesAndIngredients(recipes, ingredients);
                break;
            case "7":
                if (isAdmin) AddNewRecipe(recipes, ingredients); else ExitProgram();
                break;
            case "8":
                AddNewIngredient(ingredients);
                break;
            case "9":
                DisplayAllIngredients(ingredients);
                break;
            case "10":
                UpdateRecipeOrIngredientInformation(recipes, ingredients);
                break;
            case "11":
                SaveDataToFile(recipes, ingredients); // JSON Save
                break;
            case "12":
                SaveDataToBinaryFile(recipes, ingredients); // Binary Save
                break;
            case "13":
                LoadDataFromBinaryFile(recipes, ingredients); // Binary Load
                break;
            case "14":
                RemoveRecipeOrIngredient(recipes, ingredients);
                break;
            case "15":
                RateRecipe(recipes);
                break;
            case "16":
                SortRecipesOrIngredients(recipes, ingredients);
                break;
            case "17":
                ExportReport(recipes, ingredients);
                break;
            case "18":
                PerformanceBenchmark(recipes, ingredients);
                break;
            case "19":
                ParallelProcessingDemo(recipes, ingredients);
                break;
            case "20":
                ExitProgram();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    static void AddNewRecipe(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        // Function to add a new recipe to the catalogue
        try
        {
            string recipeName = GetInput("Enter the recipe name: ");
            if (recipes.ContainsKey(recipeName))
            {
                Console.WriteLine("A recipe with this name already exists. Please enter a different name.");
                return;
            }

            string cuisine = GetInput("Enter the cuisine type: ");
            Recipe newRecipe = new Recipe(recipeName, cuisine);

            while (true)
            {
                string ingredientName = GetInput("Enter the ingredient's name (or 'done' to finish): ");
                if (ingredientName.ToLower() == "done")
                    break;

                if (ingredients.ContainsKey(ingredientName))
                {
                    newRecipe.AddIngredient(ingredients[ingredientName]);
                    Console.WriteLine($"Ingredient '{ingredientName}' added to recipe '{recipeName}'.");
                }
                else
                {
                    Console.WriteLine($"Ingredient '{ingredientName}' not found. Please add the ingredient first.");
                }
            }

            recipes[recipeName] = newRecipe;
            Console.WriteLine("Recipe added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message); // Handle unexpected errors
        }
    }

    static void AddNewIngredient(Dictionary<string, Ingredient> ingredients)
    {
        // Function to add a new ingredient to the catalogue
        try
        {
            string ingredientName = GetInput("Enter the ingredient name: ");
            if (ingredients.ContainsKey(ingredientName))
            {
                Console.WriteLine("An ingredient with this name already exists. Please enter a different name.");
                return;
            }

            Console.WriteLine("Is this a perishable ingredient? (yes/no)");
            string isPerishable = Console.ReadLine().ToLower();

            if (isPerishable == "yes")
            {
                DateTime expirationDate;
                while (true)
                {
                    Console.Write("Enter the expiration date (yyyy-MM-dd): ");
                    if (DateTime.TryParse(Console.ReadLine(), out expirationDate))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please try again.");
                    }
                }

                PerishableIngredient newPerishableIngredient = new PerishableIngredient(ingredientName, GetIntInput("Enter the quantity available: "), expirationDate);
                ingredients[ingredientName] = newPerishableIngredient;
                Console.WriteLine("Perishable Ingredient added successfully!");
            }
            else
            {
                Ingredient newIngredient = new Ingredient(ingredientName, GetIntInput("Enter the quantity available: "));
                ingredients[ingredientName] = newIngredient;
                Console.WriteLine("Ingredient added successfully!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message); // Handle unexpected errors
        }
    }

    static void DisplayAllIngredients(Dictionary<string, Ingredient> ingredients)
    {
        // Function to display all ingredients
        if (ingredients.Count == 0)
        {
            Console.WriteLine("No ingredients available in the catalogue.");
        }
        else
        {
            Console.WriteLine("Listing all ingredients:");
            foreach (Ingredient ingredient in ingredients.Values)
            {
                ingredient.DisplayInfo(); // Call DisplayInfo method of Ingredient class to show ingredient details
                Console.WriteLine();
            }
        }
    }

    static void DisplayAllRecipes(Dictionary<string, Recipe> recipes)
    {
        // Function to display all recipes
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available in the catalogue.");
        }
        else
        {
            foreach (Recipe recipe in recipes.Values)
            {
                recipe.DisplayInfo(); // Call DisplayInfo method of Recipe class to show recipe details
                Console.WriteLine();
            }
        }
    }

    static void DisplayRecipesByCuisine(Dictionary<string, Recipe> recipes)
    {
        // Function to display recipes by cuisine type
        string cuisine = GetInput("Enter the cuisine to filter by: ");
        var filteredRecipes = recipes.Values.Where(r => r.GetCuisine().Equals(cuisine, StringComparison.OrdinalIgnoreCase)).ToList();

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
        // Function to search recipes or ingredients by name
        string searchTerm = GetInput("Enter search term (recipe name or ingredient name):");

        var matchingRecipes = recipes.Values.Where(r => r.GetName().ToLower().Contains(searchTerm)).ToList();
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

        var matchingIngredients = ingredients.Values.Where(i => i.GetName().ToLower().Contains(searchTerm)).ToList();
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

    static void UpdateRecipeOrIngredientInformation(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        // Function to update information of a recipe or ingredient
        string choice = GetInput("Do you want to update a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'):").ToLower();

        if (choice == "recipe")
        {
            string recipeName = GetInput("Enter the name of the recipe to update:");
if (recipes.ContainsKey(recipeName))
{
    string newCuisine = GetInput("Enter new cuisine:");
    recipes[recipeName].SetCuisine(newCuisine);
    Console.WriteLine("Recipe updated successfully!");
}
else
{
    Console.WriteLine("Recipe not found.");
}
        }
        else if (choice == "ingredient")
        {
            string ingredientName = GetInput("Enter the name of the ingredient to update:");
if (ingredients.ContainsKey(ingredientName))
{
    int newQuantity = GetIntInput("Enter new quantity available:");
    ingredients[ingredientName].SetQuantity(newQuantity);
    Console.WriteLine("Ingredient updated successfully!");
}
else
{
    Console.WriteLine("Ingredient not found.");
}
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
        }
    }

    static void DisplayRecipesByIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string ingredientName = GetInput("Enter the ingredient name to find recipes:");
        if (ingredients.ContainsKey(ingredientName))
        {
            var recipesWithIngredient = recipes.Values.Where(r => r.GetIngredients().Any(i => i.GetName().Equals(ingredientName, StringComparison.OrdinalIgnoreCase))).ToList();

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
        else
        {
            Console.WriteLine($"Ingredient '{ingredientName}' not found.");
        }
    }

// Class to hold the serialized data structure
public class CatalogueData
{
    public Dictionary<string, Recipe> Recipes { get; set; }
    public Dictionary<string, Ingredient> Ingredients { get; set; }
}

static void SaveDataToFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string filename = GetInput("Enter the filename to save data to (e.g., data.json): ");

    try
    {
        var data = new CatalogueData
        {
            Recipes = recipes,
            Ingredients = ingredients
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filename, json);

        Console.WriteLine("Data saved successfully.");
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred while saving data: " + e.Message);
    }
}

    static void RemoveRecipeOrIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string choice = GetInput("Do you want to remove a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'):").ToLower();

        if (choice == "recipe")
        {
            string recipeName = GetInput("Enter the name of the recipe to remove:");
            if (recipes.ContainsKey(recipeName))
            {
                recipes.Remove(recipeName);
                Console.WriteLine("Recipe removed successfully!");
            }
            else
            {
                Console.WriteLine("Recipe not found.");
            }
        }
        else if (choice == "ingredient")
        {
            string ingredientName = GetInput("Enter the name of the ingredient to remove:");
            if (ingredients.ContainsKey(ingredientName))
            {
                ingredients.Remove(ingredientName);
                Console.WriteLine("Ingredient removed successfully!");
            }
            else
            {
                Console.WriteLine("Ingredient not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
        }
    }

    static void RateRecipe(Dictionary<string, Recipe> recipes)
    {
        string recipeName = GetInput("Enter the name of the recipe to rate:");
        if (recipes.ContainsKey(recipeName))
        {
            int rating = GetIntInput("Enter your rating (1-5): ");
            if (rating >= 1 && rating <= 5)
            {
                recipes[recipeName].AddRating(rating);
                Console.WriteLine("Rating added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            }
        }
        else
        {
            Console.WriteLine("Recipe not found.");
        }
    }

static void SortRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string choice = GetInput("Do you want to sort Recipes or Ingredients? (Enter 'Recipe' or 'Ingredient'):").ToLower();

    if (choice == "recipe")
    {
        var sortedRecipes = recipes.Values.OrderBy(r => r.GetName()).ToList();
        Console.WriteLine("Recipes sorted alphabetically:");
        foreach (var recipe in sortedRecipes)
        {
            recipe.DisplayInfo();
            Console.WriteLine();
        }
    }
    else if (choice == "ingredient")
    {
        var sortedIngredients = ingredients.Values.OrderBy(i => i.GetName()).ToList();
        Console.WriteLine("Ingredients sorted alphabetically:");
        foreach (var ingredient in sortedIngredients)
        {
            ingredient.DisplayInfo();
            Console.WriteLine();
        }
    }
    else
    {
        Console.WriteLine("Invalid choice. Please enter either 'Recipe' or 'Ingredient'.");
    }
}

static void ExportReport(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string filename = GetInput("Enter the filename to export the report to:");

    try
    {
        List<string> reportLines = new List<string>
        {
            "Recipe and Ingredient Catalogue Report",
            "========================================="
        };

        // Add recipe details to the report
        reportLines.Add("\nRecipes:");
        foreach (var recipe in recipes.Values.OrderBy(r => r.GetName()))
        {
            reportLines.Add($"Name: {recipe.GetName()}, Cuisine: {recipe.GetCuisine()}, Average Rating: {recipe.GetAverageRating():F1}");
        }

        // Add ingredient details to the report
        reportLines.Add("\nIngredients:");
        foreach (var ingredient in ingredients.Values.OrderBy(i => i.GetName()))
        {
            reportLines.Add($"Name: {ingredient.GetName()}, Quantity: {ingredient.GetQuantity()}");
        }

        File.WriteAllLines(filename, reportLines);
        Console.WriteLine("Report exported successfully.");
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred while exporting the report: " + e.Message);
    }
}

static void ExitProgram()
{
    Console.WriteLine("Goodbye!");
    Environment.Exit(0);
}

// Helper methods to reduce code duplication and improve readability/maintainability of the program.
// GetInput() and GetIntInput() are used to handle user input and validation.    
static string GetInput(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine();
}

static int GetIntInput(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value))
            return value;
        else
            Console.WriteLine("Invalid input. Please enter a valid integer.");
    }
}

// TOPIC 5: Binary File Serialization Methods
static void SaveDataToBinaryFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string filename = GetInput("Enter the filename to save binary data to (e.g., data.dat): ");

    try
    {
        using (FileStream fs = new FileStream(filename, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // Write recipes count
            writer.Write(recipes.Count);
            foreach (var kvp in recipes)
            {
                writer.Write(kvp.Key); // Recipe name
                writer.Write(kvp.Value.GetName());
                writer.Write(kvp.Value.GetCuisine());
                writer.Write(kvp.Value.GetIngredients().Count);
                foreach (var ingredient in kvp.Value.GetIngredients())
                {
                    writer.Write(ingredient.GetName());
                    writer.Write(ingredient.GetQuantity());
                }
            }

            // Write ingredients count
            writer.Write(ingredients.Count);
            foreach (var kvp in ingredients)
            {
                writer.Write(kvp.Key); // Ingredient name
                writer.Write(kvp.Value.GetName());
                writer.Write(kvp.Value.GetQuantity());
                
                // Check if it's a perishable ingredient
                if (kvp.Value is PerishableIngredient perishable)
                {
                    writer.Write(true); // Is perishable
                    writer.Write(perishable.ExpirationDate.ToBinary());
                }
                else
                {
                    writer.Write(false); // Not perishable
                }
            }
        }

        Console.WriteLine("Binary data saved successfully.");
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred while saving binary data: " + e.Message);
    }
}

static void LoadDataFromBinaryFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    string filename = GetInput("Enter the filename to load binary data from (e.g., data.dat): ");

    if (!File.Exists(filename))
    {
        Console.WriteLine($"File '{filename}' does not exist.");
        return;
    }

    try
    {
        using (FileStream fs = new FileStream(filename, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            // Clear existing data
            recipes.Clear();
            ingredients.Clear();

            // Read ingredients first (needed for recipes)
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

                Recipe recipe = new Recipe(name, cuisine);
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
        }

        Console.WriteLine("Binary data loaded successfully.");
        Console.WriteLine($"Loaded {recipes.Count} recipes and {ingredients.Count} ingredients.");
    }
    catch (Exception e)
    {
        Console.WriteLine("An error occurred while loading binary data: " + e.Message);
    }
}

// TOPIC 6: Performance Profiling and Multiple Processors
static void PerformanceBenchmark(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    Console.WriteLine("=== Performance Benchmark ===");
    
    // Create test data if needed
    if (recipes.Count == 0)
    {
        Console.WriteLine("Creating test data for benchmarking...");
        CreateTestData(recipes, ingredients);
    }

    Stopwatch stopwatch = new Stopwatch();

    // Benchmark 1: Sequential search
    Console.WriteLine("\n1. Sequential Search Benchmark:");
    stopwatch.Start();
    var sequentialResults = recipes.Values.Where(r => r.GetCuisine().Contains("Italian")).ToList();
    stopwatch.Stop();
    Console.WriteLine($"Sequential search found {sequentialResults.Count} results in {stopwatch.ElapsedMilliseconds} ms");

    // Benchmark 2: LINQ operations
    stopwatch.Restart();
    var sortedRecipes = recipes.Values.OrderBy(r => r.GetName()).ToList();
    stopwatch.Stop();
    Console.WriteLine($"LINQ sorting of {recipes.Count} recipes took {stopwatch.ElapsedMilliseconds} ms");

    // Benchmark 3: Dictionary lookup vs List search
    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
        var recipe = recipes.ContainsKey("Test Recipe 1");
    }
    stopwatch.Stop();
    Console.WriteLine($"1000 dictionary lookups took {stopwatch.ElapsedTicks} ticks");

    // Benchmark 4: Memory usage profiling
    long memoryBefore = GC.GetTotalMemory(false);
    var largeList = new List<Recipe>();
    for (int i = 0; i < 10000; i++)
    {
        largeList.Add(new Recipe($"Recipe {i}", "Test Cuisine"));
    }
    long memoryAfter = GC.GetTotalMemory(false);
    Console.WriteLine($"Memory used for 10,000 recipes: {(memoryAfter - memoryBefore) / 1024} KB");

    // Clean up
    largeList.Clear();
    GC.Collect();
}

static void ParallelProcessingDemo(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    Console.WriteLine("=== Parallel Processing Demo ===");
    
    // Create test data if needed
    if (recipes.Count < 100)
    {
        Console.WriteLine("Creating test data for parallel processing demo...");
        CreateTestData(recipes, ingredients);
    }

    Stopwatch stopwatch = new Stopwatch();

    // Demo 1: Sequential vs Parallel processing
    Console.WriteLine("\n1. Sequential vs Parallel Processing:");
    
    // Sequential processing
    stopwatch.Start();
    var sequentialResults = recipes.Values
        .Where(r => r.GetAverageRating() > 3.0)
        .Select(r => r.GetName().ToUpper())
        .ToList();
    stopwatch.Stop();
    long sequentialTime = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Sequential processing: {sequentialTime} ms, {sequentialResults.Count} results");

    // Parallel processing
    stopwatch.Restart();
    var parallelResults = recipes.Values
        .AsParallel()
        .Where(r => r.GetAverageRating() > 3.0)
        .Select(r => r.GetName().ToUpper())
        .ToList();
    stopwatch.Stop();
    long parallelTime = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"Parallel processing: {parallelTime} ms, {parallelResults.Count} results");
    Console.WriteLine($"Speedup: {(double)sequentialTime / parallelTime:F2}x");

    // Demo 2: Parallel.ForEach
    Console.WriteLine("\n2. Parallel.ForEach Demo:");
    var recipeList = recipes.Values.ToList();
    int processedCount = 0;

    stopwatch.Restart();
    Parallel.ForEach(recipeList, recipe =>
    {
        // Simulate some processing work
        Thread.Sleep(1);
        Interlocked.Increment(ref processedCount);
    });
    stopwatch.Stop();
    Console.WriteLine($"Parallel.ForEach processed {processedCount} recipes in {stopwatch.ElapsedMilliseconds} ms");

    // Demo 3: Task-based processing
    Console.WriteLine("\n3. Task-based Processing:");
    stopwatch.Restart();
    
    var tasks = new List<Task<int>>();
    var chunks = recipeList.Chunk(recipeList.Count / Environment.ProcessorCount + 1);
    
    foreach (var chunk in chunks)
    {
        var task = Task.Run(() =>
        {
            int count = 0;
            foreach (var recipe in chunk)
            {
                if (recipe.GetIngredients().Count > 2)
                    count++;
            }
            return count;
        });
        tasks.Add(task);
    }

    Task.WaitAll(tasks.ToArray());
    int totalComplexRecipes = tasks.Sum(t => t.Result);
    stopwatch.Stop();
    
    Console.WriteLine($"Task-based processing found {totalComplexRecipes} complex recipes in {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"Used {tasks.Count} tasks across {Environment.ProcessorCount} processor cores");
}

static void CreateTestData(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
{
    // Create test ingredients
    string[] ingredientNames = { "Tomato", "Onion", "Garlic", "Cheese", "Pasta", "Chicken", "Beef", "Rice", "Beans", "Pepper" };
    for (int i = 0; i < ingredientNames.Length; i++)
    {
        if (!ingredients.ContainsKey(ingredientNames[i]))
        {
            if (i % 3 == 0) // Make every third ingredient perishable
            {
                ingredients[ingredientNames[i]] = new PerishableIngredient(ingredientNames[i], 100 + i * 10, DateTime.Now.AddDays(7 + i));
            }
            else
            {
                ingredients[ingredientNames[i]] = new Ingredient(ingredientNames[i], 100 + i * 10);
            }
        }
    }

    // Create test recipes
    string[] cuisines = { "Italian", "Mexican", "Chinese", "Indian", "French" };
    Random random = new Random();
    
    for (int i = 0; i < 500; i++)
    {
        string recipeName = $"Test Recipe {i + 1}";
        if (!recipes.ContainsKey(recipeName))
        {
            string cuisine = cuisines[i % cuisines.Length];
            Recipe recipe = new Recipe(recipeName, cuisine);
            
            // Add random ingredients
            int ingredientCount = random.Next(2, 6);
            var selectedIngredients = ingredientNames.OrderBy(x => random.Next()).Take(ingredientCount);
            
            foreach (var ingredientName in selectedIngredients)
            {
                if (ingredients.ContainsKey(ingredientName))
                {
                    recipe.AddIngredient(ingredients[ingredientName]);
                }
            }
            
            // Add random ratings
            int ratingCount = random.Next(1, 6);
            for (int j = 0; j < ratingCount; j++)
            {
                recipe.AddRating(random.Next(1, 6));
            }
            
            recipes[recipeName] = recipe;
        }
    }
    
    Console.WriteLine($"Created {ingredients.Count} test ingredients and {recipes.Count} test recipes.");
}
}

// Ingredient Class Definition
public class Ingredient
{
    // Member Variables
    [JsonInclude]
    private string name; // Stores the name of the ingredient
    [JsonInclude]
    private int quantity; // Stores the quantity of the ingredient

    // Constructor
    public Ingredient(string name, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
    }

    // Member Functions
    public string GetName() => name;
    public int GetQuantity() => quantity;
    public void SetQuantity(int quantity) => this.quantity = quantity;
    public void SetName(string name) => this.name = name;
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Ingredient: {name}, Quantity: {quantity}");
    }

    // Unit Tests
    public static void RunTests()
    {
        Ingredient testIngredient = new Ingredient("Sugar", 5);
        Debug.Assert(testIngredient.GetName() == "Sugar", "Error: GetName failed");
        Debug.Assert(testIngredient.GetQuantity() == 5, "Error: GetQuantity failed");
        testIngredient.SetQuantity(10);
        Debug.Assert(testIngredient.GetQuantity() == 10, "Error: SetQuantity failed");
    }
}

// Perishable Ingredient Class Definition
public class PerishableIngredient : Ingredient
{
    // Properties with getters and setters
    public DateTime ExpirationDate { get; set; } // Stores the expiration date of the ingredient

    // Constructor
    // Initializes a new instance of the PerishableIngredient class with the specified name, quantity, and expiration date
    public PerishableIngredient(string name, int quantity, DateTime expirationDate) : base(name, quantity)
    {
        ExpirationDate = expirationDate;
    }

    // Member Functions
    // Displays detailed information about the perishable ingredient, including its name, quantity, and expiration date
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Expiration Date: {ExpirationDate.ToShortDateString()}");
    }

    // Unit Tests
    public static void RunTests()
    {
        PerishableIngredient testPerishableIngredient = new PerishableIngredient("Test Perishable Ingredient", 100, DateTime.Now.AddDays(7));

        // Test Name property
        Debug.Assert(testPerishableIngredient.GetName() == "Test Perishable Ingredient", "Error: Name getter failed.");
        testPerishableIngredient.SetName("Updated Perishable Ingredient");
        Debug.Assert(testPerishableIngredient.GetName() == "Updated Perishable Ingredient", "Error: Name setter failed.");

        // Test Quantity property
        Debug.Assert(testPerishableIngredient.GetQuantity() == 100, "Error: Quantity getter failed.");
        testPerishableIngredient.SetQuantity(200);
        Debug.Assert(testPerishableIngredient.GetQuantity() == 200, "Error: Quantity setter failed.");

        // Test ExpirationDate property
        Debug.Assert(testPerishableIngredient.ExpirationDate.Date == DateTime.Now.AddDays(7).Date, "Error: ExpirationDate getter failed.");
        testPerishableIngredient.ExpirationDate = DateTime.Now.AddDays(14);
        Debug.Assert(testPerishableIngredient.ExpirationDate.Date == DateTime.Now.AddDays(14).Date, "Error: ExpirationDate setter failed.");

        Console.WriteLine("All PerishableIngredient class tests passed.");
    }
}

// Recipe Class Definition
public class Recipe
{
    // Member Variables
    [JsonInclude]
    private string name; // Stores the name of the recipe
    [JsonInclude]
    private string cuisine; // Stores the type of cuisine
    [JsonInclude]
    private List<Ingredient> ingredients; // Stores a list of ingredients
    [JsonInclude]
    private List<int> ratings; // Stores user ratings for the recipe

    // Constructor
    public Recipe(string name, string cuisine)
    {
        this.name = name;
        this.cuisine = cuisine;
        this.ingredients = new List<Ingredient>();
        this.ratings = new List<int>();
    }

    // Member Functions
    public string GetName() => name;
    public string GetCuisine() => cuisine;
    public void SetCuisine(string cuisine) => this.cuisine = cuisine;
    public List<Ingredient> GetIngredients() => ingredients;
    public void AddIngredient(Ingredient ingredient) => ingredients.Add(ingredient);
    public void AddRating(int rating)
    {
        if (rating >= 1 && rating <= 5)
        {
            ratings.Add(rating);
        }
        else
        {
            throw new ArgumentException("Rating must be between 1 and 5.");
        }
    }
    public double GetAverageRating() => ratings.Count == 0 ? 0.0 : ratings.Average();
    public void DisplayInfo()
    {
        Console.WriteLine($"Recipe: {name}, Cuisine: {cuisine}, Average Rating: {GetAverageRating():F1}");
        Console.WriteLine("Ingredients:");
        foreach (var ingredient in ingredients)
        {
            ingredient.DisplayInfo();
        }
    }

    // Unit Tests
    public static void RunTests()
    {
        Recipe testRecipe = new Recipe("Pasta", "Italian");
        Debug.Assert(testRecipe.GetName() == "Pasta", "Error: GetName failed");
        Debug.Assert(testRecipe.GetCuisine() == "Italian", "Error: GetCuisine failed");
        testRecipe.SetCuisine("Mexican");
        Debug.Assert(testRecipe.GetCuisine() == "Mexican", "Error: SetCuisine failed");
        Ingredient testIngredient = new Ingredient("Tomato", 3);
        testRecipe.AddIngredient(testIngredient);
        Debug.Assert(testRecipe.GetIngredients().Count == 1, "Error: AddIngredient failed");
        testRecipe.AddRating(5);
        testRecipe.AddRating(3);
        Debug.Assert(Math.Abs(testRecipe.GetAverageRating() - 4.0) < 0.001, "Error: GetAverageRating failed");
    }
}


 /*
 
    Explanation of Advanced Programming Topics Implemented:
    
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
