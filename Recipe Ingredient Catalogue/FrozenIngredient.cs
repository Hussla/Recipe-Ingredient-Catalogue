using System;
using System.Text.Json.Serialization;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * FrozenIngredient.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Extends RefrigeratedIngredient to represent ingredients that require freezing.
 * Demonstrates deep inheritance hierarchy and specialized storage requirements.
 * 
 * INHERITANCE HIERARCHY:
 * Ingredient → PerishableIngredient → RefrigeratedIngredient → FrozenIngredient
 * 
 * DESIGN PATTERNS:
 * • Template Method: Specialized display and validation logic
 * • Visitor Pattern: Accepts visitor operations for polymorphic behavior
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

/// <summary>
/// Represents an ingredient that requires frozen storage
/// </summary>
public class FrozenIngredient : RefrigeratedIngredient
    {
        /// <summary>
        /// Freezing temperature in Celsius
        /// </summary>
        public double FreezingTemperature { get; set; }

        /// <summary>
        /// Indicates if the ingredient has been thawed
        /// </summary>
        public bool HasBeenThawed { get; set; }

        /// <summary>
        /// Number of freeze-thaw cycles the ingredient has undergone
        /// </summary>
        public int FreezeThaWCycles { get; set; }

        /// <summary>
        /// Maximum safe freeze-thaw cycles before quality degradation
        /// </summary>
        public int MaxFreezeThaWCycles { get; set; }

        /// <summary>
        /// Default constructor for JSON deserialization
        /// </summary>
        [JsonConstructor]
        public FrozenIngredient() : base()
        {
            FreezingTemperature = -18.0; // Standard freezer temperature
            HasBeenThawed = false;
            FreezeThaWCycles = 0;
            MaxFreezeThaWCycles = 3;
        }

        /// <summary>
        /// Creates a new frozen ingredient with specified parameters
        /// </summary>
        /// <param name="name">Name of the ingredient</param>
        /// <param name="quantity">Available quantity</param>
        /// <param name="expirationDate">Expiration date</param>
        /// <param name="freezingTemperature">Freezing storage temperature</param>
        /// <param name="maxFreezeThaWCycles">Maximum safe freeze-thaw cycles</param>
        public FrozenIngredient(string name, int quantity, DateTime expirationDate, 
            double freezingTemperature = -18.0, int maxFreezeThaWCycles = 3) 
            : base(name, quantity, expirationDate, freezingTemperature, freezingTemperature + 5)
        {
            FreezingTemperature = freezingTemperature;
            HasBeenThawed = false;
            FreezeThaWCycles = 0;
            MaxFreezeThaWCycles = maxFreezeThaWCycles;
        }

        /// <summary>
        /// Displays detailed information about the frozen ingredient
        /// </summary>
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"  Storage: Frozen at {FreezingTemperature}°C");
            Console.WriteLine($"  Freeze-Thaw Cycles: {FreezeThaWCycles}/{MaxFreezeThaWCycles}");
            Console.WriteLine($"  Thaw Status: {(HasBeenThawed ? "Previously Thawed" : "Never Thawed")}");
            
            if (FreezeThaWCycles >= MaxFreezeThaWCycles)
            {
                Console.WriteLine("  WARNING: Maximum freeze-thaw cycles exceeded!");
            }
        }

        /// <summary>
        /// Checks if the ingredient is safe for consumption
        /// </summary>
        /// <returns>True if safe, false otherwise</returns>
        public override bool IsSafe()
        {
            return base.IsSafe() && FreezeThaWCycles < MaxFreezeThaWCycles;
        }

        /// <summary>
        /// Simulates thawing the ingredient
        /// </summary>
        public void Thaw()
        {
            if (!HasBeenThawed)
            {
                HasBeenThawed = true;
                FreezeThaWCycles++;
                Console.WriteLine($"THAWED: {Name} has been thawed (Cycle {FreezeThaWCycles})");
                
                if (FreezeThaWCycles >= MaxFreezeThaWCycles)
                {
                    Console.WriteLine($"WARNING: {Name} has exceeded safe freeze-thaw cycles!");
                }
            }
        }

        /// <summary>
        /// Simulates refreezing the ingredient
        /// </summary>
        public void Refreeze()
        {
            if (HasBeenThawed)
            {
                HasBeenThawed = false;
                Console.WriteLine($"FROZEN: {Name} has been refrozen");
            }
        }


        /// <summary>
        /// Gets the storage requirements as a formatted string
        /// </summary>
        /// <returns>Storage requirements description</returns>
        public override string GetStorageRequirements()
        {
            return $"Keep frozen at {FreezingTemperature}°C or below";
        }

        /// <summary>
        /// Runs comprehensive tests for FrozenIngredient functionality
        /// </summary>
        public static new void RunTests()
        {
            Console.WriteLine("Running FrozenIngredient tests...");

            // Test 1: Basic construction and properties
            var iceCream = new FrozenIngredient("Ice Cream", 500, DateTime.Now.AddMonths(6), -18.0, 2);
            System.Diagnostics.Debug.Assert(iceCream.Name == "Ice Cream", "Name should be set correctly");
            System.Diagnostics.Debug.Assert(iceCream.Quantity == 500, "Quantity should be set correctly");
            System.Diagnostics.Debug.Assert(iceCream.FreezingTemperature == -18.0, "Freezing temperature should be set");
            System.Diagnostics.Debug.Assert(iceCream.MaxFreezeThaWCycles == 2, "Max freeze-thaw cycles should be set");
            System.Diagnostics.Debug.Assert(!iceCream.HasBeenThawed, "Should not be thawed initially");
            System.Diagnostics.Debug.Assert(iceCream.FreezeThaWCycles == 0, "Should have zero freeze-thaw cycles initially");

            // Test 2: Thawing and refreezing
            iceCream.Thaw();
            System.Diagnostics.Debug.Assert(iceCream.HasBeenThawed, "Should be thawed after thaw operation");
            System.Diagnostics.Debug.Assert(iceCream.FreezeThaWCycles == 1, "Should have one freeze-thaw cycle");

            iceCream.Refreeze();
            System.Diagnostics.Debug.Assert(!iceCream.HasBeenThawed, "Should not be thawed after refreeze");

            // Test 3: Multiple freeze-thaw cycles
            iceCream.Thaw();
            iceCream.Refreeze();
            System.Diagnostics.Debug.Assert(iceCream.FreezeThaWCycles == 2, "Should have two freeze-thaw cycles");

            // Test 4: Safety check after exceeding cycles
            iceCream.Thaw(); // This should exceed the limit
            System.Diagnostics.Debug.Assert(!iceCream.IsSafe(), "Should not be safe after exceeding freeze-thaw cycles");

            // Test 5: Inheritance chain
            System.Diagnostics.Debug.Assert(iceCream is Ingredient, "Should be an Ingredient");
            System.Diagnostics.Debug.Assert(iceCream is PerishableIngredient, "Should be a PerishableIngredient");
            System.Diagnostics.Debug.Assert(iceCream is RefrigeratedIngredient, "Should be a RefrigeratedIngredient");
            System.Diagnostics.Debug.Assert(iceCream is FrozenIngredient, "Should be a FrozenIngredient");

            // Test 6: Storage requirements
            string requirements = iceCream.GetStorageRequirements();
            System.Diagnostics.Debug.Assert(requirements.Contains("-18"), "Storage requirements should include freezing temperature");

            Console.WriteLine("FrozenIngredient tests completed successfully.");
        }
    }
