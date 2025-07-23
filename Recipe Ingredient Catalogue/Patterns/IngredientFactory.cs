using System;
using RecipeIngredientCatalogue.Interfaces;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * IngredientFactory.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Implements the Factory Method pattern for creating different types of ingredients.
 * Provides a centralized way to create ingredient objects without exposing
 * instantiation logic to client code.
 * 
 * DESIGN PATTERNS:
 * • Factory Method: Creates objects without specifying exact classes
 * • Generic Constraints: Type-safe creation with compile-time checking
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Patterns
{
    /// <summary>
    /// Factory implementation for creating ingredient objects
    /// </summary>
    public class IngredientFactory : IIngredientFactory
    {
        /// <summary>
        /// Creates an ingredient of the specified generic type
        /// </summary>
        /// <typeparam name="T">Type of ingredient to create</typeparam>
        /// <param name="name">Name of the ingredient</param>
        /// <param name="quantity">Quantity of the ingredient</param>
        /// <param name="additionalParams">Additional parameters for specialized ingredients</param>
        /// <returns>Created ingredient instance</returns>
        public T CreateIngredient<T>(string name, int quantity, params object[] additionalParams) where T : Ingredient
        {
            var type = typeof(T);
            
            if (type == typeof(Ingredient))
            {
                return (T)(object)new Ingredient(name, quantity);
            }
            else if (type == typeof(PerishableIngredient))
            {
                var expirationDate = additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddDays(7);
                return (T)(object)new PerishableIngredient(name, quantity, expirationDate);
            }
            else if (type == typeof(RefrigeratedIngredient))
            {
                var expirationDate = additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddDays(7);
                var optimalTemp = additionalParams.Length > 1 ? (double)additionalParams[1] : 4.0;
                var maxTemp = additionalParams.Length > 2 ? (double)additionalParams[2] : 8.0;
                return (T)(object)new RefrigeratedIngredient(name, quantity, expirationDate, optimalTemp, maxTemp);
            }
            else if (type == typeof(FrozenIngredient))
            {
                var expirationDate = additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddMonths(6);
                var freezingTemp = additionalParams.Length > 1 ? (double)additionalParams[1] : -18.0;
                var maxCycles = additionalParams.Length > 2 ? (int)additionalParams[2] : 3;
                return (T)(object)new FrozenIngredient(name, quantity, expirationDate, freezingTemp, maxCycles);
            }
            
            throw new ArgumentException($"Unsupported ingredient type: {type.Name}");
        }

        /// <summary>
        /// Creates an ingredient based on string type specification
        /// </summary>
        /// <param name="type">String representation of ingredient type</param>
        /// <param name="name">Name of the ingredient</param>
        /// <param name="quantity">Quantity of the ingredient</param>
        /// <param name="additionalParams">Additional parameters for specialized ingredients</param>
        /// <returns>Created ingredient instance</returns>
        public Ingredient CreateIngredient(string type, string name, int quantity, params object[] additionalParams)
        {
            return type.ToLower() switch
            {
                "ingredient" or "basic" => new Ingredient(name, quantity),
                "perishable" => new PerishableIngredient(name, quantity, 
                    additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddDays(7)),
                "refrigerated" => new RefrigeratedIngredient(name, quantity,
                    additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddDays(7),
                    additionalParams.Length > 1 ? (double)additionalParams[1] : 4.0,
                    additionalParams.Length > 2 ? (double)additionalParams[2] : 8.0),
                "frozen" => new FrozenIngredient(name, quantity,
                    additionalParams.Length > 0 ? (DateTime)additionalParams[0] : DateTime.Now.AddMonths(6),
                    additionalParams.Length > 1 ? (double)additionalParams[1] : -18.0,
                    additionalParams.Length > 2 ? (int)additionalParams[2] : 3),
                _ => throw new ArgumentException($"Unknown ingredient type: {type}")
            };
        }

        /// <summary>
        /// Creates an ingredient with automatic type detection based on parameters
        /// </summary>
        /// <param name="name">Name of the ingredient</param>
        /// <param name="quantity">Quantity of the ingredient</param>
        /// <param name="expirationDate">Optional expiration date</param>
        /// <param name="requiresRefrigeration">Whether ingredient needs refrigeration</param>
        /// <param name="requiresFreezing">Whether ingredient needs freezing</param>
        /// <returns>Appropriate ingredient type based on requirements</returns>
        public Ingredient CreateIngredientAuto(string name, int quantity, DateTime? expirationDate = null, 
            bool requiresRefrigeration = false, bool requiresFreezing = false)
        {
            if (requiresFreezing)
            {
                return new FrozenIngredient(name, quantity, expirationDate ?? DateTime.Now.AddMonths(6));
            }
            else if (requiresRefrigeration)
            {
                return new RefrigeratedIngredient(name, quantity, expirationDate ?? DateTime.Now.AddDays(7));
            }
            else if (expirationDate.HasValue)
            {
                return new PerishableIngredient(name, quantity, expirationDate.Value);
            }
            else
            {
                return new Ingredient(name, quantity);
            }
        }

        /// <summary>
        /// Runs tests for the IngredientFactory
        /// </summary>
        public static void RunTests()
        {
            Console.WriteLine("Running IngredientFactory tests...");
            var factory = new IngredientFactory();

            // Test 1: Generic creation
            var basicIngredient = factory.CreateIngredient<Ingredient>("Salt", 100);
            System.Diagnostics.Debug.Assert(basicIngredient.Name == "Salt", "Basic ingredient creation failed");
            System.Diagnostics.Debug.Assert(basicIngredient.GetType() == typeof(Ingredient), "Wrong type created");

            // Test 2: Perishable ingredient creation
            var perishable = factory.CreateIngredient<PerishableIngredient>("Milk", 500, DateTime.Now.AddDays(5));
            System.Diagnostics.Debug.Assert(perishable is PerishableIngredient, "Perishable ingredient creation failed");

            // Test 3: String-based creation
            var refrigerated = factory.CreateIngredient("refrigerated", "Cheese", 200, DateTime.Now.AddDays(14));
            System.Diagnostics.Debug.Assert(refrigerated is RefrigeratedIngredient, "String-based creation failed");

            // Test 4: Auto creation
            var autoFrozen = factory.CreateIngredientAuto("Ice Cream", 300, DateTime.Now.AddMonths(3), false, true);
            System.Diagnostics.Debug.Assert(autoFrozen is FrozenIngredient, "Auto creation failed");

            Console.WriteLine("IngredientFactory tests completed successfully.");
        }
    }
}
