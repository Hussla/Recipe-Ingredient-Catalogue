using System;
using System.Text.Json.Serialization;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * RefrigeratedIngredient.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Extends PerishableIngredient to represent ingredients that require refrigeration.
 * Demonstrates multiple inheritance levels and specialized storage requirements.
 * 
 * INHERITANCE HIERARCHY:
 * Ingredient → PerishableIngredient → RefrigeratedIngredient
 * 
 * DESIGN PATTERNS:
 * • Template Method: Specialized display and validation logic
 * • Visitor Pattern: Accepts visitor operations for polymorphic behavior
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

/// <summary>
/// Represents an ingredient that requires refrigeration storage
/// </summary>
public class RefrigeratedIngredient : PerishableIngredient
    {
        /// <summary>
        /// Optimal storage temperature in Celsius
        /// </summary>
        public double OptimalTemperature { get; set; }

        /// <summary>
        /// Maximum safe storage temperature in Celsius
        /// </summary>
        public double MaxTemperature { get; set; }

        /// <summary>
        /// Indicates if the ingredient has been temperature-compromised
        /// </summary>
        public bool IsTemperatureCompromised { get; set; }

        /// <summary>
        /// Default constructor for JSON deserialization
        /// </summary>
        [JsonConstructor]
        public RefrigeratedIngredient() : base()
        {
            OptimalTemperature = 4.0; // Default refrigerator temperature
            MaxTemperature = 8.0;
            IsTemperatureCompromised = false;
        }

        /// <summary>
        /// Creates a new refrigerated ingredient with specified parameters
        /// </summary>
        /// <param name="name">Name of the ingredient</param>
        /// <param name="quantity">Available quantity</param>
        /// <param name="expirationDate">Expiration date</param>
        /// <param name="optimalTemperature">Optimal storage temperature</param>
        /// <param name="maxTemperature">Maximum safe temperature</param>
        public RefrigeratedIngredient(string name, int quantity, DateTime expirationDate, 
            double optimalTemperature = 4.0, double maxTemperature = 8.0) 
            : base(name, quantity, expirationDate)
        {
            OptimalTemperature = optimalTemperature;
            MaxTemperature = maxTemperature;
            IsTemperatureCompromised = false;
        }

        /// <summary>
        /// Displays detailed information about the refrigerated ingredient
        /// </summary>
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"  Storage: Refrigerated (Optimal: {OptimalTemperature}°C, Max: {MaxTemperature}°C)");
            Console.WriteLine($"  Temperature Status: {(IsTemperatureCompromised ? "COMPROMISED" : "Safe")}");
            
            if (IsTemperatureCompromised)
            {
                Console.WriteLine("  WARNING: Temperature safety has been compromised!");
            }
        }

        /// <summary>
        /// Checks if the ingredient is safe for consumption based on temperature and expiration
        /// </summary>
        /// <returns>True if safe, false otherwise</returns>
        public override bool IsSafe()
        {
            return base.IsSafe() && !IsTemperatureCompromised;
        }

        /// <summary>
        /// Simulates temperature exposure and updates safety status
        /// </summary>
        /// <param name="exposureTemperature">Temperature the ingredient was exposed to</param>
        /// <param name="exposureHours">Duration of exposure in hours</param>
        public void ExposeToTemperature(double exposureTemperature, double exposureHours)
        {
            if (exposureTemperature > MaxTemperature && exposureHours > 2.0)
            {
                IsTemperatureCompromised = true;
                Console.WriteLine($"WARNING: {Name} has been compromised due to temperature exposure!");
            }
        }


        /// <summary>
        /// Gets the storage requirements as a formatted string
        /// </summary>
        /// <returns>Storage requirements description</returns>
        public virtual string GetStorageRequirements()
        {
            return $"Refrigerate at {OptimalTemperature}°C (max {MaxTemperature}°C)";
        }

        /// <summary>
        /// Runs comprehensive tests for RefrigeratedIngredient functionality
        /// </summary>
        public static new void RunTests()
        {
            Console.WriteLine("Running RefrigeratedIngredient tests...");

            // Test 1: Basic construction and properties
            var milk = new RefrigeratedIngredient("Milk", 1000, DateTime.Now.AddDays(7), 4.0, 8.0);
            System.Diagnostics.Debug.Assert(milk.Name == "Milk", "Name should be set correctly");
            System.Diagnostics.Debug.Assert(milk.Quantity == 1000, "Quantity should be set correctly");
            System.Diagnostics.Debug.Assert(milk.OptimalTemperature == 4.0, "Optimal temperature should be set");
            System.Diagnostics.Debug.Assert(milk.MaxTemperature == 8.0, "Max temperature should be set");
            System.Diagnostics.Debug.Assert(!milk.IsTemperatureCompromised, "Should not be temperature compromised initially");

            // Test 2: Temperature exposure
            milk.ExposeToTemperature(15.0, 3.0); // High temperature for 3 hours
            System.Diagnostics.Debug.Assert(milk.IsTemperatureCompromised, "Should be compromised after high temperature exposure");

            // Test 3: Safety check
            System.Diagnostics.Debug.Assert(!milk.IsSafe(), "Should not be safe when temperature compromised");

            // Test 4: Inheritance chain
            System.Diagnostics.Debug.Assert(milk is Ingredient, "Should be an Ingredient");
            System.Diagnostics.Debug.Assert(milk is PerishableIngredient, "Should be a PerishableIngredient");
            System.Diagnostics.Debug.Assert(milk is RefrigeratedIngredient, "Should be a RefrigeratedIngredient");

            // Test 5: Storage requirements
            string requirements = milk.GetStorageRequirements();
            System.Diagnostics.Debug.Assert(requirements.Contains("4"), "Storage requirements should include optimal temperature");

            Console.WriteLine("RefrigeratedIngredient tests completed successfully.");
        }
    }
