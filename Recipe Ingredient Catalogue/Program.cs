﻿using System;
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

            int quantity = GetIntInput("Enter the quantity available: ");
            Ingredient newIngredient = new Ingredient(ingredientName, quantity);
            ingredients[ingredientName] = newIngredient;
            Console.WriteLine("Ingredient added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message); // Handle unexpected errors
        }
    }

    static void DisplayAllRecipes(Dictionary<string, Recipe> recipes)
    {
        // Function to display all recipes
        if (recipes.Count() == 0)
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

        if (filteredRecipes.Count() == 0)
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
        // Function to load recipes and ingredients from a file
        string filename = GetInput("What file would you like to load?");

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
                            var parts = line.Split(",");
                            if (parts.Length >= 2)
                            {
                                string name = parts[0].Trim();
                                int quantity = GetIntInput($"Enter the quantity for '{name}': ");
                                Ingredient newIngredient = new Ingredient(name, quantity);
                                ingredients[name] = newIngredient;
                                Console.WriteLine($"Loaded Ingredient: {name}");
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

    static void SearchRecipesOrIngredients(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        // Function to search recipes or ingredients by name
        string searchTerm = GetInput("Enter search term (recipe name or ingredient name):");

        var matchingRecipes = recipes.Values.Where(r => r.GetName().ToLower().Contains(searchTerm)).ToList();
        if (matchingRecipes.Count() > 0)
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
        if (matchingIngredients.Count() > 0)
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

            if (recipesWithIngredient.Count() > 0)
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

    static void SaveDataToFile(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        string filename = GetInput("Enter the filename to save data to:");

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

            File.WriteAllLines(filename, lines);
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
}

 
// Additional functions like SearchRecipesOrIngredients, UpdateRecipeOrIngredientInformation, etc., will be implemented here
 /*
    
    Explanation of Topics Included So Far
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
