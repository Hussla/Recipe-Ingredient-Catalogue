using System;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * MenuService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Manages user interface navigation and menu display functionality for the application.
 * Provides role-based menu options and input validation for user interactions.
 * 
 * KEY RESPONSIBILITIES:
 * • Displaying appropriate menu options based on user role (admin vs standard user)
 * • Validating user input format and range
 * • Enforcing permission boundaries for restricted operations
 * • Centralizing menu-related UI operations for consistency
 * 
 * DESIGN PATTERNS:
 * • Static Utility Class: Provides stateless helper methods for menu operations
 * • Role-Based Differentiation: Dynamically adjusts UI based on user permissions
 * 
 * DEPENDENCIES:
 * • None (pure UI layer with no direct service dependencies)
 * 
 * PUBLIC METHODS:
 * • DisplayMenu(): Shows appropriate menu options based on user role
 * • GetUserChoice(): Validates and returns user input with range checking
 * • ValidateAdminPermission(): Enforces access control for admin-only operations
 * 
 * INTEGRATION POINTS:
 * • Used by Program.cs to handle user interface navigation
 * • Works with ValidationService for input validation
 * 
 * USAGE EXAMPLES:
 * • Display different menus for admin vs standard users
 * • Prevent non-admin users from accessing restricted operations
 * • Standardize user input handling across the application
 * 
 * TECHNICAL NOTES:
 * • Uses console-based I/O for user interaction
 * • Implements validation loops for robust input handling
 * • Maintains separation between UI and business logic
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

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

                if (int.TryParse(choice, out int numChoice))
                {
                    // For non-admins, validate against their available options (1-7)
                    if (!isAdmin && numChoice >= 1 && numChoice <= 7)
                        return choice;
                    
                    // For admins, validate against their available options (7-20)
                    if (isAdmin && numChoice >= 7 && numChoice <= 20)
                        return choice;
                }

                Console.WriteLine($"Invalid choice. Please enter a number between {(isAdmin ? "7 and 20" : "1 and 7")}.");
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
