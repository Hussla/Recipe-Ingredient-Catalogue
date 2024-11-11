using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

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
            Console.WriteLine("6 - Add a new Recipe");
            Console.WriteLine("7 - Add a new Ingredient");
            Console.WriteLine("8 - Update Recipe or Ingredient Information");
            Console.WriteLine("9 - Save Data");
            Console.WriteLine("10 - Remove Recipe or Ingredient");
            Console.WriteLine("11 - Rate a Recipe");
            Console.WriteLine("12 - Sort Recipes or Ingredients");
            Console.WriteLine("13 - Export Report");
            Console.WriteLine("14 - Exit");

        }
        else
        {
            Console.WriteLine("1 - Display all Recipes");
            Console.WriteLine("2 - Display Recipes by Cuisine");
            Console.WriteLine("3 - Search Recipes or Ingredients");
            Console.WriteLine("4 - Display Recipes by Ingredient");
            Console.WriteLine("5 - Exit");

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
            if (int.TryParse(choice, out int numChoice) && numChoice >= 1 && numChoice <= 14)
            {
                return choice;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a number between 1 and 14.");
            }
        }
    }

    // Handles the user's menu choice
    static void HandleUserChoice(string choice, Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients, bool isAdmin)
    {
        if (!isAdmin && int.TryParse(choice, out int numChoice) && numChoice > 5)
        {
            Console.WriteLine("You do not have permission to perform this action. Please contact an administrator.");
            return;
        }

        switch (choice)
        {
            case "1":
                AddNewRecipe(recipes, ingredients); // Call function to add a new recipe
                break;
            case "2":
                AddNewIngredient(ingredients); // Call function to add a new ingredient
                break;
            case "3":
                DisplayAllRecipes(recipes); // Call function to display all recipes
                break;
            case "4":
                DisplayRecipesByCuisine(recipes); // Call function to display recipes by cuisine
                break;
            case "5":
                LoadRecipesAndIngredients(recipes, ingredients); // Call function to load recipes and ingredients
                break;
            case "6":
                SearchRecipesOrIngredients(recipes, ingredients); // Call function to search recipes or ingredients
                break;
            case "7":
                UpdateRecipeOrIngredientInformation(recipes, ingredients); // Call function to update recipe or ingredient information
                break;
            case "8":
                DisplayRecipesByIngredient(recipes, ingredients); // Call function to display recipes by ingredient
                break;
            case "9":
                SaveDataToFile(recipes, ingredients); // Save data
                break;
            case "10":
                RemoveRecipeOrIngredient(recipes, ingredients); // Remove recipe or ingredient
                break;
            case "11":
                RateRecipe(recipes); // Rate a recipe
                break;
            case "12":
                SortRecipesOrIngredients(recipes, ingredients); // Sort recipes or ingredients
                break;
            case "13":
                ExportReport(recipes, ingredients); // Export report
                break;
            case "14":
                ExitProgram(); // Exit the program
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again."); // Handle invalid menu choices
                break;
        }
    }

     // Additional Functions Placeholder
   static void AddNewRecipe(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        // Function to add a new recipe to the catalogue
        try
        {
            Console.Write("Enter the recipe name: ");
            string recipeName = Console.ReadLine();

            // Check if the recipe already exists
            if (recipes.ContainsKey(recipeName))
            {
                Console.WriteLine("A recipe with this name already exists. Please enter a different name.");
                return;
            }

            Console.Write("Enter the cuisine type: ");
            string cuisine = Console.ReadLine();

            // Create a new Recipe object and add it to the recipes dictionary
            Recipe newRecipe = new Recipe(recipeName, cuisine);

            // Optionally add ingredients to the recipe
            Console.Write("Do you want to add ingredients to this recipe? (yes/no): ");
            string addIngredientsResponse = Console.ReadLine().ToLower();
            while (addIngredientsResponse == "yes")
            {
                Console.Write("Enter the ingredient's name: ");
                string ingredientName = Console.ReadLine();

                if (ingredients.ContainsKey(ingredientName))
                {
                    newRecipe.AddIngredient(ingredients[ingredientName]);
                    Console.WriteLine($"Ingredient '{ingredientName}' added to recipe '{recipeName}'.");
                }
                else
                {
                    Console.WriteLine($"Ingredient '{ingredientName}' not found. Please add the ingredient first.");
                }

                Console.Write("Do you want to add another ingredient to this recipe? (yes/no): ");
                addIngredientsResponse = Console.ReadLine().ToLower();
            }

            recipes[recipeName] = newRecipe;
            Console.WriteLine("Recipe added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message); // Handle unexpected errors
        }
    }

    // Additional Functions Placeholder
    static void AddNewIngredient(Dictionary<string, Ingredient> ingredients) {
        // Function to add a new ingredient to the catalogue
        try
        {
            Console.Write("Enter the ingredient name: ");
            string ingredientName = Console.ReadLine();

            // Check if the ingredient already exists
            if (ingredients.ContainsKey(ingredientName))
            {
                Console.WriteLine("An ingredient with this name already exists. Please enter a different name.");
                return;
            }

            Console.Write("Enter the quantity available: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Invalid input. Quantity must be a number.");
                return;
            }

            // Create a new Ingredient object and add it to the ingredients dictionary
            Ingredient newIngredient = new Ingredient(ingredientName, quantity);
            ingredients[ingredientName] = newIngredient;
            Console.WriteLine("Ingredient added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message); // Handle unexpected errors
        }
    }

    static void DisplayAllRecipes(Dictionary<string, Recipe> recipes) {
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

    static void DisplayRecipesByCuisine(Dictionary<string, Recipe> recipes) {
        // Function to display recipes by cuisine type
       Console.Write("Enter the cuisine to filter by: ");
        string cuisine = Console.ReadLine();

        // Use LINQ to filter recipes by the specified cuisine
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

    static void LoadRecipesAndIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients) {
        // Function to load recipes and ingredients from a file
         Console.WriteLine("What file would you like to load?");
        string filename = Console.ReadLine();

        if (File.Exists(filename))
        {
            Console.WriteLine($"Loading data from '{filename}'...");
            List<string> lines = File.ReadAllLines(filename).ToList();

            bool isRecipeSection = false;
            bool isIngredientSection = false;

            foreach (string line in lines)
            {
                if (line.Trim() == "Recipes:")
                {
                    // Start parsing the Recipes section
                    isRecipeSection = true;
                    isIngredientSection = false;
                    Console.WriteLine("Found Recipes section");
                }
                else if (line.Trim() == "Ingredients:")
                {
                    // Start parsing the Ingredients section
                    isRecipeSection = false;
                    isIngredientSection = true;
                    Console.WriteLine("Found Ingredients section");
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (isRecipeSection)
                    {
                        try
                        {
                            // Split the line by comma to extract recipe details
                            var parts = line.Split(",");
                            if (parts.Length >= 2)
                            {
                                string name = parts[0].Trim();
                                string cuisine = parts[1].Trim();
                                Recipe newRecipe = new Recipe(name, cuisine);
                                recipes[name] = newRecipe;
                                Console.WriteLine($"Loaded Recipe: {name}");
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect recipe format: {line}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error parsing recipe: {line}. Error: {e.Message}");
                        }
                    }
                    else if (isIngredientSection)
                    {
                        try
                        {
                            // Split the line by comma to extract ingredient details
                            var parts = line.Split(",");
                            if (parts.Length >= 2)
                            {
                                string name = parts[0].Trim();
                                if (int.TryParse(parts[1].Trim(), out int quantity))
                                {
                                    Ingredient newIngredient = new Ingredient(name, quantity);
                                    ingredients[name] = newIngredient;
                                    Console.WriteLine($"Loaded Ingredient: {name}");
                                }
                                else
                                {
                                    Console.WriteLine($"Incorrect ingredient format: {line}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect ingredient format: {line}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error parsing ingredient: {line}. Error: {e.Message}");
                        }
                    }
                }
                }

            // Display a success message after loading data
            Console.WriteLine("Data loaded successfully.");
        }
        else
        {
            // Inform the user if the specified file does not exist
            Console.WriteLine($"Sorry, '{filename}' does not exist.");
        }
        
    }

    static void SearchRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients) {
        // Function to search recipes or ingredients by name
        Console.WriteLine("Enter search term (recipe name or ingredient name):");
        string searchTerm = Console.ReadLine().ToLower();

        // Search for recipes that match the search term
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

        // Search for ingredients that match the search term
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

    static void UpdateRecipeOrIngredientInformation(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients) {
        // Function to update information of a recipe or ingredient
         Console.WriteLine("Do you want to update a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'):");
        string choice = Console.ReadLine().ToLower();

        if (choice == "recipe")
        {
            Console.Write("Enter the name of the recipe to update: ");
            string recipeName = Console.ReadLine();

            if (recipes.ContainsKey(recipeName))
            {
                Console.Write("Enter new cuisine: ");
                string newCuisine = Console.ReadLine();

                // Update recipe details
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
            Console.Write("Enter the name of the ingredient to update: ");
            string ingredientName = Console.ReadLine();

            if (ingredients.ContainsKey(ingredientName))
            {
                Console.Write("Enter new quantity available: ");
                if (!int.TryParse(Console.ReadLine(), out int newQuantity))
                {
                    Console.WriteLine("Invalid input. Quantity must be a number.");
                    return;
                }

                // Update ingredient details
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

    // Displays recipes that contain a specific ingredient
    static void DisplayRecipesByIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        Console.Write("Enter the ingredient name to find recipes: ");
        string ingredientName = Console.ReadLine();

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

    // Saves recipes and ingredients to a file
    static void SaveDataToFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        Console.WriteLine("Enter the filename to save data to:");
        string filename = Console.ReadLine();

        try
        {
            List<string> lines = new List<string>();

            // Write Recipes section
            lines.Add("Recipes:");
            foreach (Recipe recipe in recipes.Values)
            {
                lines.Add($"{recipe.GetName()}, {recipe.GetCuisine()}");
            }

            // Write Ingredients section
            lines.Add("Ingredients:");
            foreach (Ingredient ingredient in ingredients.Values)
            {
                lines.Add($"{ingredient.GetName()}, {ingredient.GetQuantity()}");
            }

            // Write all lines to the specified file
            File.WriteAllLines(filename, lines);

            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while saving data: " + e.Message);
        }
    }

    // Removes a recipe or ingredient from the catalogue
    static void RemoveRecipeOrIngredient(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        Console.Write("Do you want to remove a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'): ");
        string choice = Console.ReadLine().ToLower();

        if (choice == "recipe")
        {
            Console.Write("Enter the name of the recipe to remove: ");
            string recipeName = Console.ReadLine();

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
            Console.Write("Enter the name of the ingredient to remove: ");
            string ingredientName = Console.ReadLine();

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

    // Rates a recipe
    static void RateRecipe(Dictionary<string, Recipe> recipes)
    {
        Console.Write("Enter the name of the recipe to rate: ");
        string recipeName = Console.ReadLine();

        if (recipes.ContainsKey(recipeName))
        {
            Console.Write("Enter your rating (1-5): ");
            if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
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

    // Sorts recipes or ingredients
    static void SortRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        Console.Write("Do you want to sort Recipes or Ingredients? (Enter 'Recipe' or 'Ingredient'): ");
        string choice = Console.ReadLine().ToLower();

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

    // Exports the recipe and ingredient catalogue to a text file
    static void ExportReport(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        Console.Write("Enter the filename to export the report to: ");
        string filename = Console.ReadLine();

        try
        {
            List<string> reportLines = new List<string>
            {
                "Recipe and Ingredient Catalogue Report",
                "========================================="
            };

            // Add recipe details to the report
            reportLines.Add("\nRecipes:");
            foreach (var recipe in recipes.Values)
            {
                reportLines.Add($"Name: {recipe.GetName()}, Cuisine: {recipe.GetCuisine()}, Average Rating: {recipe.GetAverageRating()}");
            }

            // Add ingredient details to the report
            reportLines.Add("\nIngredients:");
            foreach (var ingredient in ingredients.Values)
            {
                reportLines.Add($"Name: {ingredient.GetName()}, Quantity: {ingredient.GetQuantity()}");
            }

            // Write report to the specified file
            File.WriteAllLines(filename, reportLines);

            Console.WriteLine("Report exported successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while exporting the report: " + e.Message);
        }
    }

    // Exits the program
    static void ExitProgram()
    {
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
}



// Ingredient Class Definition
public class Ingredient
{
    // Member Variables
    private string name; // Stores the name of the ingredient
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
    public void DisplayInfo()
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

// Recipe Class Definition
public class Recipe
{
    // Member Variables
    private string name; // Stores the name of the recipe
    private string cuisine; // Stores the type of cuisine
    private List<Ingredient> ingredients; // Stores a list of ingredients
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



    // Additional functions like SearchRecipesOrIngredients, UpdateRecipeOrIngredientInformation, etc., will be implemented here

    // Explanation of Topics Included So Far
    /*
    1. Console IO and Variables:
        - The program makes extensive use of Console.WriteLine() and Console.ReadLine() for user interaction.
        - Variables are used to store user input and data like recipes and ingredients.

    2. Command Line Interfaces:
        - The program provides a command-line interface that lets users enter commands to interact with the catalogue.
        - This allows for user-friendly navigation and data manipulation.

    3. Robustness:
        - Error handling is implemented throughout the program to make it more robust and handle unexpected inputs or failures.
        - `try-catch` blocks are used to catch exceptions during operations like adding ingredients or reading from files.

    4. Encapsulation and Constructors:
        - The `Recipe` and `Ingredient` classes use constructors to initialize their objects with the appropriate values.
        - Encapsulation ensures that class properties are kept private, accessed, and modified through public methods only.

    5. Object-Oriented Programming (OOP):
        - Classes like `Recipe` and `Ingredient` represent real-world entities and encapsulate their properties and behaviors.
        - This program also demonstrates inheritance through base classes (if needed).

    6. Data Persistence:
        - The program allows for saving and loading of data through file operations, providing data persistence across different runs.
        - This includes reading and writing to text files.

    7. Writing Fast Code:
        - LINQ queries are used to filter recipes and ingredients, offering a concise and readable way to work with collections.
        - Optimization techniques can be further employed to improve performance in larger datasets.
    */

