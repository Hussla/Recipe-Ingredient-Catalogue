using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientCatalogue.Interfaces;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * IngredientProcessors.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Implements the Strategy pattern for different ingredient processing algorithms.
 * Provides interchangeable processing strategies without modifying ingredient classes.
 * 
 * DESIGN PATTERNS:
 * • Strategy Pattern: Family of algorithms with interchangeable implementations
 * • Generic Constraints: Type-safe processing with compile-time checking
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Patterns
{
    /// <summary>
    /// Strategy for nutritional analysis of ingredients
    /// </summary>
    public class NutritionalAnalysisProcessor<T> : IIngredientProcessor<T> where T : Ingredient
    {
        private readonly Dictionary<string, NutritionalInfo> _nutritionalDatabase;

        public NutritionalAnalysisProcessor()
        {
            _nutritionalDatabase = new Dictionary<string, NutritionalInfo>
            {
                ["Milk"] = new NutritionalInfo { CaloriesPerUnit = 0.42, Protein = 3.4, Fat = 1.0, Carbs = 5.0 },
                ["Cheese"] = new NutritionalInfo { CaloriesPerUnit = 4.0, Protein = 25.0, Fat = 33.0, Carbs = 1.3 },
                ["Bread"] = new NutritionalInfo { CaloriesPerUnit = 2.65, Protein = 9.0, Fat = 3.2, Carbs = 49.0 },
                ["Ice Cream"] = new NutritionalInfo { CaloriesPerUnit = 2.07, Protein = 3.5, Fat = 11.0, Carbs = 23.0 },
                ["Salt"] = new NutritionalInfo { CaloriesPerUnit = 0.0, Protein = 0.0, Fat = 0.0, Carbs = 0.0 }
            };
        }

        public bool CanProcess(T ingredient)
        {
            return _nutritionalDatabase.ContainsKey(ingredient.Name);
        }

        public string Process(T ingredient)
        {
            if (!CanProcess(ingredient))
            {
                return $"Nutritional information not available for {ingredient.Name}";
            }

            var info = _nutritionalDatabase[ingredient.Name];
            var totalCalories = info.CaloriesPerUnit * ingredient.Quantity;
            var totalProtein = info.Protein * ingredient.Quantity / 100;
            var totalFat = info.Fat * ingredient.Quantity / 100;
            var totalCarbs = info.Carbs * ingredient.Quantity / 100;

            return $"Nutritional Analysis for {ingredient.Name} ({ingredient.Quantity}g):\n" +
                   $"  Total Calories: {totalCalories:F1} kcal\n" +
                   $"  Protein: {totalProtein:F1}g\n" +
                   $"  Fat: {totalFat:F1}g\n" +
                   $"  Carbohydrates: {totalCarbs:F1}g";
        }

        private class NutritionalInfo
        {
            public double CaloriesPerUnit { get; set; }
            public double Protein { get; set; }
            public double Fat { get; set; }
            public double Carbs { get; set; }
        }
    }

    /// <summary>
    /// Strategy for allergen detection in ingredients
    /// </summary>
    public class AllergenDetectionProcessor<T> : IIngredientProcessor<T> where T : Ingredient
    {
        private readonly Dictionary<string, List<string>> _allergenDatabase;

        public AllergenDetectionProcessor()
        {
            _allergenDatabase = new Dictionary<string, List<string>>
            {
                ["Milk"] = new List<string> { "Dairy", "Lactose" },
                ["Cheese"] = new List<string> { "Dairy", "Lactose" },
                ["Bread"] = new List<string> { "Gluten", "Wheat" },
                ["Ice Cream"] = new List<string> { "Dairy", "Lactose", "Eggs" },
                ["Eggs"] = new List<string> { "Eggs" },
                ["Nuts"] = new List<string> { "Tree Nuts" },
                ["Peanuts"] = new List<string> { "Peanuts" },
                ["Fish"] = new List<string> { "Fish" },
                ["Shellfish"] = new List<string> { "Shellfish" },
                ["Soy"] = new List<string> { "Soy" }
            };
        }

        public bool CanProcess(T ingredient)
        {
            return _allergenDatabase.ContainsKey(ingredient.Name);
        }

        public string Process(T ingredient)
        {
            if (!CanProcess(ingredient))
            {
                return $"No known allergens for {ingredient.Name}";
            }

            var allergens = _allergenDatabase[ingredient.Name];
            return $"Allergen Alert for {ingredient.Name}:\n" +
                   $"  Contains: {string.Join(", ", allergens)}\n" +
                   $"  Severity: {GetSeverityLevel(allergens)}";
        }

        private string GetSeverityLevel(List<string> allergens)
        {
            var highRiskAllergens = new[] { "Peanuts", "Tree Nuts", "Shellfish" };
            return allergens.Any(a => highRiskAllergens.Contains(a)) ? "HIGH RISK" : "Medium Risk";
        }
    }

    /// <summary>
    /// Strategy for cost calculation of ingredients
    /// </summary>
    public class CostCalculationProcessor<T> : IIngredientProcessor<T> where T : Ingredient
    {
        private readonly Dictionary<string, decimal> _priceDatabase;

        public CostCalculationProcessor()
        {
            _priceDatabase = new Dictionary<string, decimal>
            {
                ["Milk"] = 0.001m,      // $0.001 per ml
                ["Cheese"] = 0.015m,    // $0.015 per gram
                ["Bread"] = 0.003m,     // $0.003 per gram
                ["Ice Cream"] = 0.008m, // $0.008 per gram
                ["Salt"] = 0.0001m,     // $0.0001 per gram
                ["Eggs"] = 0.25m,       // $0.25 per egg
                ["Butter"] = 0.012m,    // $0.012 per gram
                ["Sugar"] = 0.002m      // $0.002 per gram
            };
        }

        public bool CanProcess(T ingredient)
        {
            return _priceDatabase.ContainsKey(ingredient.Name);
        }

        public string Process(T ingredient)
        {
            if (!CanProcess(ingredient))
            {
                return $"Price information not available for {ingredient.Name}";
            }

            var pricePerUnit = _priceDatabase[ingredient.Name];
            var totalCost = pricePerUnit * ingredient.Quantity;
            var costCategory = GetCostCategory(totalCost);

            return $"Cost Analysis for {ingredient.Name}:\n" +
                   $"  Quantity: {ingredient.Quantity}\n" +
                   $"  Price per unit: ${pricePerUnit:F4}\n" +
                   $"  Total cost: ${totalCost:F2}\n" +
                   $"  Cost category: {costCategory}";
        }

        private string GetCostCategory(decimal totalCost)
        {
            return totalCost switch
            {
                < 1.00m => "Budget",
                < 5.00m => "Standard",
                < 15.00m => "Premium",
                _ => "Luxury"
            };
        }
    }

    /// <summary>
    /// Strategy for shelf life optimization
    /// </summary>
    public class ShelfLifeOptimizationProcessor<T> : IIngredientProcessor<T> where T : Ingredient
    {
        public bool CanProcess(T ingredient)
        {
            return ingredient is PerishableIngredient;
        }

        public string Process(T ingredient)
        {
            if (!(ingredient is PerishableIngredient perishable))
            {
                return $"{ingredient.Name} is not perishable - no shelf life optimization needed";
            }

            var daysRemaining = (perishable.ExpirationDate - DateTime.Now).Days;
            var recommendations = GenerateRecommendations(perishable, daysRemaining);

            return $"Shelf Life Optimization for {ingredient.Name}:\n" +
                   $"  Days until expiration: {daysRemaining}\n" +
                   $"  Current safety status: {(perishable.IsSafe() ? "Safe" : "Expired")}\n" +
                   $"  Recommendations:\n{recommendations}";
        }

        private string GenerateRecommendations(PerishableIngredient ingredient, int daysRemaining)
        {
            var recommendations = new List<string>();

            if (daysRemaining <= 0)
            {
                recommendations.Add("    ❌ DISCARD IMMEDIATELY - Expired");
            }
            else if (daysRemaining <= 1)
            {
                recommendations.Add("    🔥 USE TODAY - Critical expiration");
                recommendations.Add("    📦 Consider freezing if possible");
            }
            else if (daysRemaining <= 3)
            {
                recommendations.Add("    USE SOON - Plan meals around this ingredient");
                recommendations.Add("    🥘 Consider batch cooking");
            }
            else if (daysRemaining <= 7)
            {
                recommendations.Add("    📅 MONITOR CLOSELY - Check daily");
                recommendations.Add("    🍽️ Plan usage within the week");
            }
            else
            {
                recommendations.Add("    ✅ GOOD CONDITION - Normal usage");
                recommendations.Add("    Monitor for quality changes");
            }

            // Add storage-specific recommendations
            if (ingredient is RefrigeratedIngredient refrigerated)
            {
                recommendations.Add($"    Maintain temperature at {refrigerated.OptimalTemperature}°C");
                if (refrigerated.IsTemperatureCompromised)
                {
                    recommendations.Add("    WARNING: Temperature compromised - use immediately");
                }
            }

            if (ingredient is FrozenIngredient frozen)
            {
                recommendations.Add($"    Keep frozen at {frozen.FreezingTemperature}°C");
                if (frozen.FreezeThaWCycles > 0)
                {
                    recommendations.Add($"    🔄 Freeze-thaw cycles: {frozen.FreezeThaWCycles}/{frozen.MaxFreezeThaWCycles}");
                }
            }

            return string.Join("\n", recommendations);
        }
    }

    /// <summary>
    /// Context class for managing ingredient processing strategies
    /// </summary>
    public class IngredientProcessingContext<T> where T : Ingredient
    {
        private IIngredientProcessor<T> _processor;

        public IngredientProcessingContext(IIngredientProcessor<T> processor)
        {
            _processor = processor;
        }

        public void SetProcessor(IIngredientProcessor<T> processor)
        {
            _processor = processor;
        }

        public string ProcessIngredient(T ingredient)
        {
            if (_processor.CanProcess(ingredient))
            {
                return _processor.Process(ingredient);
            }
            else
            {
                return $"Current processor cannot handle {ingredient.Name}";
            }
        }

        /// <summary>
        /// Processes ingredient with all available strategies
        /// </summary>
        public string ProcessWithAllStrategies(T ingredient)
        {
            var processors = new List<IIngredientProcessor<T>>
            {
                new NutritionalAnalysisProcessor<T>(),
                new AllergenDetectionProcessor<T>(),
                new CostCalculationProcessor<T>(),
                new ShelfLifeOptimizationProcessor<T>()
            };

            var results = new List<string>();
            foreach (var processor in processors)
            {
                if (processor.CanProcess(ingredient))
                {
                    results.Add(processor.Process(ingredient));
                }
            }

            return string.Join("\n\n" + new string('=', 50) + "\n\n", results);
        }
    }

    /// <summary>
    /// Utility class for testing strategy pattern implementations
    /// </summary>
    public static class ProcessorUtility
    {
        public static void RunTests()
        {
            Console.WriteLine("Running Strategy Pattern tests...");

            // Create test ingredients
            var milk = new RefrigeratedIngredient("Milk", 1000, DateTime.Now.AddDays(5));
            var bread = new PerishableIngredient("Bread", 500, DateTime.Now.AddDays(2));

            // Test nutritional processor
            var nutritionalProcessor = new NutritionalAnalysisProcessor<RefrigeratedIngredient>();
            System.Diagnostics.Debug.Assert(nutritionalProcessor.CanProcess(milk), "Should be able to process milk");
            var nutritionalResult = nutritionalProcessor.Process(milk);
            System.Diagnostics.Debug.Assert(nutritionalResult.Contains("Calories"), "Should contain calorie information");

            // Test allergen processor
            var allergenProcessor = new AllergenDetectionProcessor<RefrigeratedIngredient>();
            var allergenResult = allergenProcessor.Process(milk);
            System.Diagnostics.Debug.Assert(allergenResult.Contains("Dairy"), "Should detect dairy allergen");

            // Test context with strategy switching
            var context = new IngredientProcessingContext<PerishableIngredient>(new ShelfLifeOptimizationProcessor<PerishableIngredient>());
            var shelfLifeResult = context.ProcessIngredient(bread);
            System.Diagnostics.Debug.Assert(shelfLifeResult.Contains("Days until expiration"), "Should contain expiration info");

            // Test processing with all strategies
            var allResults = context.ProcessWithAllStrategies(bread);
            System.Diagnostics.Debug.Assert(allResults.Length > 100, "Should have comprehensive results");

            Console.WriteLine("Strategy Pattern tests completed successfully.");
        }
    }
}
