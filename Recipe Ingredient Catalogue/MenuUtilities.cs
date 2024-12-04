using static Recipe_Ingredient_Catalogue.Utilities;

namespace Recipe_Ingredient_Catalogue
{
    public static partial class Utilities
    {
        // Gets the user's menu choice
        public static string GetUserChoice(bool isAdmin)
        {
            string choice;
            while (true)
            {
                Console.Write("Enter your choice: ");
                choice = Console.ReadLine();

                // Validate user input
                if (
                    int.TryParse(choice, out int numChoice)
                    && (
                        isAdmin
                            ? numChoice >= 1 && numChoice <= 14
                            : numChoice >= 1 && numChoice <= 5
                    )
                )
                {
                    return choice;
                }
                else
                {
                    Console.WriteLine(
                        $"Invalid choice. Please enter a number between 1 and {(isAdmin ? 14 : 5)}."
                    );
                }
            }
        }

        // Handles the user's menu choice
        public static void HandleUserChoice(
            string choice,
            Dictionary<string, Recipe> recipes,
            Dictionary<string, Ingredient> ingredients,
            bool isAdmin
        )
        {
            if (!isAdmin && int.TryParse(choice, out int numChoice) && numChoice > 5)
            {
                Console.WriteLine(
                    "You do not have permission to perform this action. Please contact an administrator."
                );
                return;
            }

            switch (choice)
            {
                case "1":
                    DisplayAllRecipes(recipes); // Call function to display all recipes
                    break;
                case "2":
                    DisplayRecipesByCuisine(recipes); // Call function to display recipes by cuisine
                    break;
                case "3":
                    SearchRecipesOrIngredients(recipes, ingredients); // Call function to search recipes or ingredients
                    break;
                case "4":
                    DisplayRecipesByIngredient(recipes, ingredients); // Call function to display recipes by ingredient
                    break;
                case "5":
                    ExitProgram(); // Exit the program
                    break;
                case "6":
                    AddNewRecipe(recipes, ingredients); // Call function to add a new recipe
                    break;
                case "7":
                    AddNewIngredient(ingredients); // Call function to add a new ingredient
                    break;
                case "8":
                    UpdateRecipeOrIngredientInformation(recipes, ingredients); // Call function to update recipe or ingredient information
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
    }
}
