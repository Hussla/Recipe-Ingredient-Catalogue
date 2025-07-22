using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeIngredientCatalogue.Services
{
    public static class IngredientService
    {
        public static void AddNewIngredient(Dictionary<string, Ingredient> ingredients)
        {
            try
            {
                string ingredientName = ValidationService.GetInput("Enter the ingredient name: ");
                if (!ValidationService.ValidateItemNotExists(ingredients, ingredientName, "Ingredient"))
                    return;

                bool isPerishable = ValidationService.GetBoolInput("Is this a perishable ingredient?");

                if (isPerishable)
                {
                    DateTime expirationDate = ValidationService.GetDateInput("Enter the expiration date (yyyy-MM-dd): ");
                    int quantity = ValidationService.GetIntInput("Enter the quantity available: ");
                    
                    PerishableIngredient newPerishableIngredient = new(ingredientName, quantity, expirationDate);
                    ingredients[ingredientName] = newPerishableIngredient;
                    Console.WriteLine("Perishable Ingredient added successfully!");
                }
                else
                {
                    int quantity = ValidationService.GetIntInput("Enter the quantity available: ");
                    Ingredient newIngredient = new(ingredientName, quantity);
                    ingredients[ingredientName] = newIngredient;
                    Console.WriteLine("Ingredient added successfully!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public static void DisplayAllIngredients(Dictionary<string, Ingredient> ingredients)
        {
            if (ingredients.Count == 0)
            {
                Console.WriteLine("No ingredients available in the catalogue.");
                return;
            }

            Console.WriteLine("Listing all ingredients:");
            foreach (Ingredient ingredient in ingredients.Values)
            {
                ingredient.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void UpdateIngredient(Dictionary<string, Ingredient> ingredients)
        {
            string ingredientName = ValidationService.GetInput("Enter the name of the ingredient to update: ");
            
            if (!ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                return;

            int newQuantity = ValidationService.GetIntInput("Enter new quantity available: ");
            ingredients[ingredientName].Quantity = newQuantity;
            Console.WriteLine("Ingredient updated successfully!");
        }

        public static void RemoveIngredient(Dictionary<string, Ingredient> ingredients)
        {
            string ingredientName = ValidationService.GetInput("Enter the name of the ingredient to remove: ");
            
            if (!ValidationService.ValidateItemExists(ingredients, ingredientName, "Ingredient"))
                return;

            ingredients.Remove(ingredientName);
            Console.WriteLine("Ingredient removed successfully!");
        }

        public static void SortIngredients(Dictionary<string, Ingredient> ingredients)
        {
            var sortedIngredients = ingredients.Values.OrderBy(i => i.Name).ToList();
            Console.WriteLine("Ingredients sorted alphabetically:");
            foreach (var ingredient in sortedIngredients)
            {
                ingredient.DisplayInfo();
                Console.WriteLine();
            }
        }

        public static void SearchIngredients(Dictionary<string, Ingredient> ingredients, string searchTerm)
        {
            var matchingIngredients = ingredients.Values
                .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

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

        public static void CreateTestIngredients(Dictionary<string, Ingredient> ingredients)
        {
            string[] ingredientNames = { "Tomato", "Onion", "Garlic", "Cheese", "Pasta", "Chicken", "Beef", "Rice", "Beans", "Pepper" };
            
            for (int i = 0; i < ingredientNames.Length; i++)
            {
                if (!ingredients.ContainsKey(ingredientNames[i]))
                {
                    if (i % 3 == 0) // Make every third ingredient perishable
                    {
                        ingredients[ingredientNames[i]] = new PerishableIngredient(
                            ingredientNames[i], 
                            100 + i * 10, 
                            DateTime.Now.AddDays(7 + i)
                        );
                    }
                    else
                    {
                        ingredients[ingredientNames[i]] = new Ingredient(ingredientNames[i], 100 + i * 10);
                    }
                }
            }
        }
    }
}
