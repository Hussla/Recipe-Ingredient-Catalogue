using System;

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
