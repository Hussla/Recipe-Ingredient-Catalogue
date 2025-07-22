using System;

namespace RecipeIngredientCatalogue.Services
{
    public static class MenuService
    {
        public static void DisplayMenu(bool isAdmin)
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

        public static string GetUserChoice(bool isAdmin)
        {
            string choice;
            while (true)
            {
                Console.Write("Enter your choice: ");
                choice = Console.ReadLine() ?? "";

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

        public static bool ValidateAdminPermission(string choice, bool isAdmin)
        {
            if (!isAdmin && int.TryParse(choice, out int numChoice) && numChoice > 6 && numChoice != 7)
            {
                Console.WriteLine("You do not have permission to perform this action. Please contact an administrator.");
                return false;
            }
            return true;
        }
    }
}
