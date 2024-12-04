using static Recipe_Ingredient_Catalogue.Utilities;

namespace Recipe_Ingredient_Catalogue
{
    public static partial class Utilities
    {
        // Gets the user's menu choice

        public static void AddNewRecipe(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            // Function to add a new recipe to the catalogue
            try
            {
                string recipeName = GetInput("Enter the recipe name: ");
                if (recipes.ContainsKey(recipeName))
                {
                    Console.WriteLine(
                        "A recipe with this name already exists. Please enter a different name."
                    );
                    return;
                }

                string cuisine = GetInput("Enter the cuisine type: ");
                int preparationTime = GetIntInput("Enter the preparation time in minutes: ");
                Recipe newRecipe = new Recipe(recipeName, cuisine, preparationTime);

                while (true)
                {
                    string ingredientName = GetInput(
                        "Enter the ingredient's name (or 'done' to finish): "
                    );
                    if (ingredientName.ToLower() == "done")
                        break;

                    if (ingredients.ContainsKey(ingredientName))
                    {
                        newRecipe.AddIngredient(ingredients[ingredientName]);
                        Console.WriteLine(
                            $"Ingredient '{ingredientName}' added to recipe '{recipeName}'."
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Ingredient '{ingredientName}' not found. Please add the ingredient first."
                        );
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

        public static void AddNewIngredient(Dictionary<string, Ingredient> ingredients)
        {
            // Function to add a new ingredient to the catalogue
            try
            {
                string ingredientName = GetInput("Enter the ingredient name: ");
                if (ingredients.ContainsKey(ingredientName))
                {
                    Console.WriteLine(
                        "An ingredient with this name already exists. Please enter a different name."
                    );
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

        public static void DisplayAllRecipes(Dictionary<string, Recipe> recipes)
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

        public static void DisplayRecipesByCuisine(Dictionary<string, Recipe> recipes)
        {
            // Function to display recipes by cuisine type
            string cuisine = GetInput("Enter the cuisine to filter by: ");
            var filteredRecipes = recipes
                .Values.Where(r => r.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase))
                .ToList();

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

        public static void LoadRecipesAndIngredients(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
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
                                if (parts.Length >= 3)
                                {
                                    string name = parts[0].Trim();
                                    string cuisine = parts[1].Trim();
                                    if (int.TryParse(parts[2].Trim(), out int preparationTime))
                                    {
                                        Recipe newRecipe = new Recipe(
                                            name,
                                            cuisine,
                                            preparationTime
                                        );
                                        recipes[name] = newRecipe;
                                        Console.WriteLine($"Loaded Recipe: {name}");
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            $"Incorrect preparation time format for recipe: {line}"
                                        );
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Incorrect recipe format: {line}");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(
                                    $"Error parsing recipe: {line}. Error: {e.Message}"
                                );
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
                                    if (int.TryParse(parts[1].Trim(), out int quantity))
                                    {
                                        Ingredient newIngredient = new Ingredient(name, quantity);
                                        ingredients[name] = newIngredient;
                                        Console.WriteLine($"Loaded Ingredient: {name}");
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            $"Incorrect quantity format for ingredient: {line}"
                                        );
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Incorrect ingredient format: {line}");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(
                                    $"Error parsing ingredient: {line}. Error: {e.Message}"
                                );
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

        public static void SearchRecipesOrIngredients(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            // Function to search recipes or ingredients by name
            string searchTerm = GetInput("Enter search term (recipe name or ingredient name):");

            var matchingRecipes = recipes
                .Values.Where(r => r.Name.ToLower().Contains(searchTerm))
                .ToList();
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

            var matchingIngredients = ingredients
                .Values.Where(i => i.Name.ToLower().Contains(searchTerm))
                .ToList();
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

        public static void UpdateRecipeOrIngredientInformation(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            // Function to update information of a recipe or ingredient
            string choice = GetInput(
                    "Do you want to update a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'):"
                )
                .ToLower();

            if (choice == "recipe")
            {
                string recipeName = GetInput("Enter the name of the recipe to update:");
                if (recipes.ContainsKey(recipeName))
                {
                    string newCuisine = GetInput("Enter new cuisine:");
                    recipes[recipeName].Cuisine = newCuisine;
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
                    ingredients[ingredientName].Quantity = newQuantity;
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

        public static void DisplayRecipesByIngredient(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            string ingredientName = GetInput("Enter the ingredient name to find recipes:");
            if (ingredients.ContainsKey(ingredientName))
            {
                var recipesWithIngredient = recipes
                    .Values.Where(r =>
                        r.GetIngredients()
                            .Any(i =>
                                i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase)
                            )
                    )
                    .ToList();

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
                    Console.WriteLine(
                        $"No recipes found containing the ingredient '{ingredientName}'."
                    );
                }
            }
            else
            {
                Console.WriteLine($"Ingredient '{ingredientName}' not found.");
            }
        }

        public static void SaveDataToFile(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            string filename = GetInput("Enter the filename to save data to:");

            try
            {
                List<string> lines = new List<string>();

                // Write Recipes section
                lines.Add("Recipes:");
                foreach (Recipe recipe in recipes.Values)
                {
                    lines.Add($"{recipe.Name}, {recipe.Cuisine}, {recipe.PreparationTime}");
                }

                // Write Ingredients section
                lines.Add("Ingredients:");
                foreach (Ingredient ingredient in ingredients.Values)
                {
                    lines.Add($"{ingredient.Name}, {ingredient.Quantity}");
                }

                File.WriteAllLines(filename, lines);
                Console.WriteLine("Data saved successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while saving data: " + e.Message);
            }
        }

        public static void RemoveRecipeOrIngredient(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            string choice = GetInput(
                    "Do you want to remove a Recipe or an Ingredient? (Enter 'Recipe' or 'Ingredient'):"
                )
                .ToLower();

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

        public static void RateRecipe(Dictionary<string, Recipe> recipes)
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

        public static void SortRecipesOrIngredients(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            string choice = GetInput(
                    "Do you want to sort Recipes or Ingredients? (Enter 'Recipe' or 'Ingredient'):"
                )
                .ToLower();

            if (choice == "recipe")
            {
                var sortedRecipes = recipes.Values.OrderBy(r => r.Name).ToList();
                Console.WriteLine("Recipes sorted alphabetically:");
                foreach (var recipe in sortedRecipes)
                {
                    recipe.DisplayInfo();
                    Console.WriteLine();
                }
            }
            else if (choice == "ingredient")
            {
                var sortedIngredients = ingredients.Values.OrderBy(i => i.Name).ToList();
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

        public static void ExportReport(
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients
        )
        {
            string filename = GetInput("Enter the filename to export the report to:");

            try
            {
                List<string> reportLines = new List<string>
                {
                    "Recipe and Ingredient Catalogue Report",
                    "=========================================",
                };

                // Add recipe details to the report
                reportLines.Add("\nRecipes:");
                foreach (var recipe in recipes.Values)
                {
                    reportLines.Add(
                        $"Name: {recipe.Name}, Cuisine: {recipe.Cuisine}, Average Rating: {recipe.GetAverageRating()}"
                    );
                }

                // Add ingredient details to the report
                reportLines.Add("Ingredients:");
                foreach (var ingredient in ingredients.Values)
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

        public static void ExitProgram()
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
}
