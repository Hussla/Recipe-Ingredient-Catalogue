using System;
using System.Collections.Generic;
using System.Diagnostics;

// Class representing an Ingredient
public class Ingredient
{
    // Properties with getters and setters
    public string Name { get; set; } // Stores the name of the ingredient
    public int Quantity { get; set; } // Stores the quantity of the ingredient

    // Constructor
    // Initializes a new instance of the Ingredient class with the specified name and quantity
    public Ingredient(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    // Member Functions
    // Displays detailed information about the ingredient, including its name and quantity
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Ingredient: {Name}, Quantity: {Quantity}");
    }

    // Unit Tests
    public static void RunTests()
    {
        // Create a test Ingredient instance
        Ingredient testIngredient = new Ingredient("Test Ingredient", 200);

        // Test Name property
Debug.Assert(testIngredient.Name == "Test Ingredient", "Error: Name getter failed.");
testIngredient.Name = "Updated Ingredient";
Debug.Assert(testIngredient.Name == "Updated Ingredient", "Error: Name setter failed.");

// Test Quantity property
Debug.Assert(testIngredient.Quantity == 200, "Error: Quantity getter failed.");
testIngredient.Quantity = 300;
Debug.Assert(testIngredient.Quantity == 300, "Error: Quantity setter failed.");

        Console.WriteLine("All Ingredient class tests passed.");
    }
}

// Class representing a Perishable Ingredient
public class PerishableIngredient : Ingredient
{
    // Properties with getters and setters
    public DateTime ExpirationDate { get; set; } // Stores the expiration date of the ingredient

    // Constructor
    // Initializes a new instance of the PerishableIngredient class with the specified name, quantity, and expiration date
    public PerishableIngredient(string name, int quantity, DateTime expirationDate) : base(name, quantity)
    {
        ExpirationDate = expirationDate;
    }

    // Member Functions
    // Displays detailed information about the perishable ingredient, including its name, quantity, and expiration date
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Expiration Date: {ExpirationDate.ToShortDateString()}");
    }

    // Unit Tests
public new static void RunTests()
{
    // Create a test PerishableIngredient instance
    PerishableIngredient testPerishableIngredient = new PerishableIngredient("Test Perishable Ingredient", 100, DateTime.Now.AddDays(7));

    // Test Name property
    Debug.Assert(testPerishableIngredient.Name == "Test Perishable Ingredient", "Error: Name getter failed.");
    testPerishableIngredient.Name = "Updated Perishable Ingredient";
    Debug.Assert(testPerishableIngredient.Name == "Updated Perishable Ingredient", "Error: Name setter failed.");

    // Test Quantity property
    Debug.Assert(testPerishableIngredient.Quantity == 100, "Error: Quantity getter failed.");
    testPerishableIngredient.Quantity = 200;
    Debug.Assert(testPerishableIngredient.Quantity == 200, "Error: Quantity setter failed.");

    // Test ExpirationDate property
    Debug.Assert(testPerishableIngredient.ExpirationDate.Date == DateTime.Now.AddDays(7).Date, "Error: ExpirationDate getter failed.");
    testPerishableIngredient.ExpirationDate = DateTime.Now.AddDays(14);
    Debug.Assert(testPerishableIngredient.ExpirationDate.Date == DateTime.Now.AddDays(14).Date, "Error: ExpirationDate setter failed.");

    Console.WriteLine("All PerishableIngredient class tests passed.");
}
    
}

/*
Usage:
The Ingredient class can be instantiated with specific names and quantities for different ingredients.
The DisplayInfo() method can be called to print the details of each ingredient instance to the console.
The RunTests() method provides a simple way to verify the correctness of the Ingredient class by running predefined assertions.

Summary:
The Ingredient.cs file defines an Ingredient class that encapsulates properties for name and quantity, along with methods to display this information and perform basic unit testing. 
This structure is useful in applications where managing and displaying recipe ingredients is required.
*/
