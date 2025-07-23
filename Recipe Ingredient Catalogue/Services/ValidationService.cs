using System;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * ValidationService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Centralized input validation and user interaction management for the application.
 * Provides consistent validation patterns and type-safe input collection.
 * 
 * KEY RESPONSIBILITIES:
 * • Collecting and validating user input of various types (string, int, bool, DateTime)
 * • Implementing validation loops with error feedback for robust input handling
 * • Generic collection validation (item existence checks)
 * • Standardizing user prompts and error messages
 * 
 * DESIGN PATTERNS:
 * • Static Utility Class: Provides reusable validation methods across the application
 * • Generic Methods: Type-safe collection validation with generic constraints
 * • Validation Loops: Robust input validation with retry capability
 * 
 * DEPENDENCIES:
 * • None (standalone validation utility)
 * 
 * PUBLIC METHODS:
 * • GetInput(): Collects string input with prompt
 * • GetIntInput(): Collects and validates integer input
 * • GetBoolInput(): Collects and validates boolean input
 * • GetDateInput(): Collects and validates date input
 * • GetRatingInput(): Collects and validates 1-5 rating input
 * • ValidateItemExists(): Generic dictionary key existence check
 * • ValidateItemNotExists(): Generic dictionary key non-existence check
 * 
 * INTEGRATION POINTS:
 * • Used by all service classes for user input collection
 * • Provides validation for RecipeService and IngredientService operations
 * • Supports DataService for file operations
 * • Enables PerformanceService for user interaction
 * 
 * USAGE EXAMPLES:
 * • Collecting recipe name input with validation
 * • Ensuring ingredient quantity is a valid integer
 * • Checking if a recipe already exists before adding
 * • Validating user confirmation for critical operations
 * 
 * TECHNICAL NOTES:
 * • Thread-safe implementation for concurrent access
 * • Implements validation loops with clear error feedback
 * • Uses generic type parameters for flexible collection validation
 * • Provides consistent user experience through standardized prompts
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    public static class ValidationService
    {
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        public static int GetIntInput(string prompt)
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

        public static bool GetBoolInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " (yes/no): ");
                string input = Console.ReadLine()?.ToLower() ?? "";
                
                if (input == "yes" || input == "y")
                    return true;
                else if (input == "no" || input == "n")
                    return false;
                else
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
        }

        public static DateTime GetDateInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    return date;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please try again (yyyy-MM-dd).");
                }
            }
        }

        public static int GetRatingInput(string prompt)
        {
            while (true)
            {
                int rating = GetIntInput(prompt);
                if (rating >= 1 && rating <= 5)
                {
                    return rating;
                }
                else
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                }
            }
        }

        public static bool ValidateItemExists<T>(Dictionary<string, T> collection, string itemName, string itemType)
        {
            if (!collection.ContainsKey(itemName))
            {
                Console.WriteLine($"{itemType} '{itemName}' not found.");
                return false;
            }
            return true;
        }

        public static bool ValidateItemNotExists<T>(Dictionary<string, T> collection, string itemName, string itemType)
        {
            if (collection.ContainsKey(itemName))
            {
                Console.WriteLine($"A {itemType.ToLower()} with this name already exists. Please enter a different name.");
                return false;
            }
            return true;
        }
    }
}
