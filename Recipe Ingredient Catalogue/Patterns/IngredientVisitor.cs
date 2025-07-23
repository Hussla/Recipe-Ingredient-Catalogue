using System;
using System.Collections.Generic;
using System.Text;
using RecipeIngredientCatalogue.Interfaces;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * IngredientVisitor.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Implements the Visitor pattern for performing operations on ingredient objects
 * without modifying their classes. Enables adding new operations to the ingredient
 * hierarchy without changing existing code.
 * 
 * DESIGN PATTERNS:
 * • Visitor Pattern: Separates algorithms from object structure
 * • Strategy Pattern: Different visitor implementations for different operations
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Patterns
{
    /// <summary>
    /// Base visitor implementation for ingredient operations
    /// </summary>
    public abstract class IngredientVisitorBase : IIngredientVisitor
    {
        public abstract void Visit(Ingredient ingredient);
        public abstract void Visit(PerishableIngredient perishableIngredient);
        public abstract void Visit(RefrigeratedIngredient refrigeratedIngredient);
        public abstract void Visit(FrozenIngredient frozenIngredient);
    }

    /// <summary>
    /// Visitor for generating detailed ingredient reports
    /// </summary>
    public class IngredientReportVisitor : IngredientVisitorBase
    {
        private readonly StringBuilder _report;
        private int _totalIngredients;
        private int _totalQuantity;

        public IngredientReportVisitor()
        {
            _report = new StringBuilder();
            _totalIngredients = 0;
            _totalQuantity = 0;
        }

        public override void Visit(Ingredient ingredient)
        {
            _report.AppendLine($"📦 Basic Ingredient: {ingredient.Name}");
            _report.AppendLine($"   Quantity: {ingredient.Quantity}");
            _report.AppendLine($"   Storage: Room temperature");
            _report.AppendLine();
            
            _totalIngredients++;
            _totalQuantity += ingredient.Quantity;
        }

        public override void Visit(PerishableIngredient perishableIngredient)
        {
            _report.AppendLine($"⏰ Perishable Ingredient: {perishableIngredient.Name}");
            _report.AppendLine($"   Quantity: {perishableIngredient.Quantity}");
            _report.AppendLine($"   Expires: {perishableIngredient.ExpirationDate:yyyy-MM-dd}");
            _report.AppendLine($"   Days until expiry: {(perishableIngredient.ExpirationDate - DateTime.Now).Days}");
            _report.AppendLine($"   Safe to consume: {(perishableIngredient.IsSafe() ? "Yes" : "No")}");
            _report.AppendLine();
            
            _totalIngredients++;
            _totalQuantity += perishableIngredient.Quantity;
        }

        public override void Visit(RefrigeratedIngredient refrigeratedIngredient)
        {
            _report.AppendLine($"🧊 Refrigerated Ingredient: {refrigeratedIngredient.Name}");
            _report.AppendLine($"   Quantity: {refrigeratedIngredient.Quantity}");
            _report.AppendLine($"   Expires: {refrigeratedIngredient.ExpirationDate:yyyy-MM-dd}");
            _report.AppendLine($"   Storage: {refrigeratedIngredient.GetStorageRequirements()}");
            _report.AppendLine($"   Temperature compromised: {(refrigeratedIngredient.IsTemperatureCompromised ? "Yes" : "No")}");
            _report.AppendLine($"   Safe to consume: {(refrigeratedIngredient.IsSafe() ? "Yes" : "No")}");
            _report.AppendLine();
            
            _totalIngredients++;
            _totalQuantity += refrigeratedIngredient.Quantity;
        }

        public override void Visit(FrozenIngredient frozenIngredient)
        {
            _report.AppendLine($"❄️ Frozen Ingredient: {frozenIngredient.Name}");
            _report.AppendLine($"   Quantity: {frozenIngredient.Quantity}");
            _report.AppendLine($"   Expires: {frozenIngredient.ExpirationDate:yyyy-MM-dd}");
            _report.AppendLine($"   Storage: {frozenIngredient.GetStorageRequirements()}");
            _report.AppendLine($"   Freeze-thaw cycles: {frozenIngredient.FreezeThaWCycles}/{frozenIngredient.MaxFreezeThaWCycles}");
            _report.AppendLine($"   Currently thawed: {(frozenIngredient.HasBeenThawed ? "Yes" : "No")}");
            _report.AppendLine($"   Safe to consume: {(frozenIngredient.IsSafe() ? "Yes" : "No")}");
            _report.AppendLine();
            
            _totalIngredients++;
            _totalQuantity += frozenIngredient.Quantity;
        }

        public string GetReport()
        {
            var summary = new StringBuilder();
            summary.AppendLine("═══════════════════════════════════════");
            summary.AppendLine("         INGREDIENT INVENTORY REPORT");
            summary.AppendLine("═══════════════════════════════════════");
            summary.AppendLine();
            summary.Append(_report.ToString());
            summary.AppendLine("═══════════════════════════════════════");
            summary.AppendLine($"Total Ingredients: {_totalIngredients}");
            summary.AppendLine($"Total Quantity: {_totalQuantity}");
            summary.AppendLine("═══════════════════════════════════════");
            
            return summary.ToString();
        }
    }

    /// <summary>
    /// Visitor for calculating storage costs based on ingredient types
    /// </summary>
    public class StorageCostVisitor : IngredientVisitorBase
    {
        private decimal _totalCost;
        private readonly Dictionary<string, int> _storageTypeCounts;

        // Cost per unit per day for different storage types
        private const decimal ROOM_TEMP_COST = 0.01m;
        private const decimal REFRIGERATED_COST = 0.05m;
        private const decimal FROZEN_COST = 0.10m;

        public StorageCostVisitor()
        {
            _totalCost = 0;
            _storageTypeCounts = new Dictionary<string, int>
            {
                ["Room Temperature"] = 0,
                ["Refrigerated"] = 0,
                ["Frozen"] = 0
            };
        }

        public override void Visit(Ingredient ingredient)
        {
            _totalCost += ingredient.Quantity * ROOM_TEMP_COST;
            _storageTypeCounts["Room Temperature"]++;
        }

        public override void Visit(PerishableIngredient perishableIngredient)
        {
            _totalCost += perishableIngredient.Quantity * ROOM_TEMP_COST;
            _storageTypeCounts["Room Temperature"]++;
        }

        public override void Visit(RefrigeratedIngredient refrigeratedIngredient)
        {
            _totalCost += refrigeratedIngredient.Quantity * REFRIGERATED_COST;
            _storageTypeCounts["Refrigerated"]++;
        }

        public override void Visit(FrozenIngredient frozenIngredient)
        {
            _totalCost += frozenIngredient.Quantity * FROZEN_COST;
            _storageTypeCounts["Frozen"]++;
        }

        public decimal GetTotalCost() => _totalCost;

        public Dictionary<string, int> GetStorageTypeCounts() => new Dictionary<string, int>(_storageTypeCounts);

        public string GetCostReport()
        {
            var report = new StringBuilder();
            report.AppendLine("═══════════════════════════════════════");
            report.AppendLine("         STORAGE COST ANALYSIS");
            report.AppendLine("═══════════════════════════════════════");
            report.AppendLine();
            
            foreach (var kvp in _storageTypeCounts)
            {
                report.AppendLine($"{kvp.Key}: {kvp.Value} ingredients");
            }
            
            report.AppendLine();
            report.AppendLine($"Total Daily Storage Cost: ${_totalCost:F2}");
            report.AppendLine($"Monthly Storage Cost: ${_totalCost * 30:F2}");
            report.AppendLine($"Annual Storage Cost: ${_totalCost * 365:F2}");
            report.AppendLine("═══════════════════════════════════════");
            
            return report.ToString();
        }
    }

    /// <summary>
    /// Visitor for safety inspection of ingredients
    /// </summary>
    public class SafetyInspectionVisitor : IngredientVisitorBase
    {
        private readonly List<string> _safeIngredients;
        private readonly List<string> _unsafeIngredients;
        private readonly List<string> _warnings;

        public SafetyInspectionVisitor()
        {
            _safeIngredients = new List<string>();
            _unsafeIngredients = new List<string>();
            _warnings = new List<string>();
        }

        public override void Visit(Ingredient ingredient)
        {
            _safeIngredients.Add(ingredient.Name);
        }

        public override void Visit(PerishableIngredient perishableIngredient)
        {
            if (perishableIngredient.IsSafe())
            {
                _safeIngredients.Add(perishableIngredient.Name);
                
                // Check for expiring soon
                var daysUntilExpiry = (perishableIngredient.ExpirationDate - DateTime.Now).Days;
                if (daysUntilExpiry <= 2)
                {
                    _warnings.Add($"{perishableIngredient.Name} expires in {daysUntilExpiry} days");
                }
            }
            else
            {
                _unsafeIngredients.Add($"{perishableIngredient.Name} (expired)");
            }
        }

        public override void Visit(RefrigeratedIngredient refrigeratedIngredient)
        {
            if (refrigeratedIngredient.IsSafe())
            {
                _safeIngredients.Add(refrigeratedIngredient.Name);
                
                if (refrigeratedIngredient.IsTemperatureCompromised)
                {
                    _warnings.Add($"{refrigeratedIngredient.Name} has temperature safety concerns");
                }
            }
            else
            {
                var reason = refrigeratedIngredient.IsTemperatureCompromised ? "temperature compromised" : "expired";
                _unsafeIngredients.Add($"{refrigeratedIngredient.Name} ({reason})");
            }
        }

        public override void Visit(FrozenIngredient frozenIngredient)
        {
            if (frozenIngredient.IsSafe())
            {
                _safeIngredients.Add(frozenIngredient.Name);
                
                if (frozenIngredient.FreezeThaWCycles >= frozenIngredient.MaxFreezeThaWCycles - 1)
                {
                    _warnings.Add($"{frozenIngredient.Name} is approaching maximum freeze-thaw cycles");
                }
            }
            else
            {
                _unsafeIngredients.Add($"{frozenIngredient.Name} (too many freeze-thaw cycles or expired)");
            }
        }

        public string GetSafetyReport()
        {
            var report = new StringBuilder();
            report.AppendLine("═══════════════════════════════════════");
            report.AppendLine("         SAFETY INSPECTION REPORT");
            report.AppendLine("═══════════════════════════════════════");
            report.AppendLine();
            
            report.AppendLine($"✅ Safe Ingredients ({_safeIngredients.Count}):");
            foreach (var ingredient in _safeIngredients)
            {
                report.AppendLine($"   • {ingredient}");
            }
            
            report.AppendLine();
            report.AppendLine($"❌ Unsafe Ingredients ({_unsafeIngredients.Count}):");
            foreach (var ingredient in _unsafeIngredients)
            {
                report.AppendLine($"   • {ingredient}");
            }
            
            report.AppendLine();
            report.AppendLine($"⚠️  Warnings ({_warnings.Count}):");
            foreach (var warning in _warnings)
            {
                report.AppendLine($"   • {warning}");
            }
            
            report.AppendLine();
            report.AppendLine("═══════════════════════════════════════");
            
            return report.ToString();
        }

        public List<string> GetSafeIngredients() => new List<string>(_safeIngredients);
        public List<string> GetUnsafeIngredients() => new List<string>(_unsafeIngredients);
        public List<string> GetWarnings() => new List<string>(_warnings);
    }

    /// <summary>
    /// Utility class for applying visitors to ingredient collections
    /// </summary>
    public static class VisitorUtility
    {
        /// <summary>
        /// Applies a visitor to all ingredients in a collection
        /// </summary>
        /// <param name="ingredients">Collection of ingredients</param>
        /// <param name="visitor">Visitor to apply</param>
        public static void ApplyVisitor(IEnumerable<Ingredient> ingredients, IIngredientVisitor visitor)
        {
            foreach (var ingredient in ingredients)
            {
                switch (ingredient)
                {
                    case FrozenIngredient frozen:
                        visitor.Visit(frozen);
                        break;
                    case RefrigeratedIngredient refrigerated:
                        visitor.Visit(refrigerated);
                        break;
                    case PerishableIngredient perishable:
                        visitor.Visit(perishable);
                        break;
                    default:
                        visitor.Visit(ingredient);
                        break;
                }
            }
        }

        /// <summary>
        /// Runs tests for the visitor pattern implementations
        /// </summary>
        public static void RunTests()
        {
            Console.WriteLine("Running Visitor Pattern tests...");

            // Create test ingredients
            var ingredients = new List<Ingredient>
            {
                new Ingredient("Salt", 100),
                new PerishableIngredient("Bread", 2, DateTime.Now.AddDays(3)),
                new RefrigeratedIngredient("Milk", 1000, DateTime.Now.AddDays(7)),
                new FrozenIngredient("Ice Cream", 500, DateTime.Now.AddMonths(6))
            };

            // Test report visitor
            var reportVisitor = new IngredientReportVisitor();
            ApplyVisitor(ingredients, reportVisitor);
            var report = reportVisitor.GetReport();
            System.Diagnostics.Debug.Assert(report.Contains("Salt"), "Report should contain Salt");
            System.Diagnostics.Debug.Assert(report.Contains("Total Ingredients: 4"), "Report should show correct count");

            // Test cost visitor
            var costVisitor = new StorageCostVisitor();
            ApplyVisitor(ingredients, costVisitor);
            var totalCost = costVisitor.GetTotalCost();
            System.Diagnostics.Debug.Assert(totalCost > 0, "Total cost should be greater than 0");

            // Test safety visitor
            var safetyVisitor = new SafetyInspectionVisitor();
            ApplyVisitor(ingredients, safetyVisitor);
            var safeIngredients = safetyVisitor.GetSafeIngredients();
            System.Diagnostics.Debug.Assert(safeIngredients.Count > 0, "Should have safe ingredients");

            Console.WriteLine("Visitor Pattern tests completed successfully.");
        }
    }
}
