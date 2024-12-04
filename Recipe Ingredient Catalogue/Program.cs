using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Recipe_Ingredient_Catalogue.Utilities;

namespace Recipe_Ingredient_Catalogue
{
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
                Console.WriteLine(
                    "Invalid mode. Use 'admin' for full privileges or 'user' for read-only access."
                );
                return;
            }

            Console.WriteLine("Running tests for Ingredient class...");
            Ingredient.RunTests(); // Run tests for Ingredient class

            Console.WriteLine("Running tests for Recipe class...");
            Recipe.RunTests(); // Run tests for Recipe class

            Console.WriteLine("All tests have been executed.");

            // Display a welcome message and instructions for using the program
            Console.WriteLine("Welcome to the Recipe and Ingredients Catalogue!");
            Console.WriteLine("=========================================");
            Console.WriteLine(
                "Use this program to manage your collection of recipes and ingredients."
            );
            Console.WriteLine("Choose an option from the menu below:");

            // Initialise dictionary to store recipes and ingredients
            Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();
            Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();

            // Load recipes and ingredients from a file
            LoadRecipesAndIngredients(recipes, ingredients);

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
    }
}
