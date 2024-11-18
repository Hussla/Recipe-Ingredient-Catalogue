using System;
using System.Collections.Generic;
using System.Diagnostics;

// Class representing an Ingredient
public class Ingredient
{
    // Properties with getters and setters
    public string Name { get; set; } // Stores the name of the ingredient
    public string Quantity { get; set; } // Stores the quantity of the ingredient (e.g., 100g, 1 cup)

    // Constructor
    // Initializes a new instance of the Ingredient class with the specified name and quantity
    public Ingredient(string name, string quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    // Member Functions
    // Displays detailed information about the ingredient, including its name and quantity
    public void DisplayInfo()
    {
        Console.WriteLine($"Ingredient: {Name}, Quantity: {Quantity}");
    }

    // Unit Tests
    public static void RunTests()
    {
        // Create a test Ingredient instance
        Ingredient testIngredient = new Ingredient("Test Ingredient", "200g");

        // Test Name property
        Debug.Assert(testIngredient.Name == "Test Ingredient", "Error: Name getter failed.");
        testIngredient.Name = "Updated Ingredient";
        Debug.Assert(testIngredient.Name == "Updated Ingredient", "Error: Name setter failed.");

        // Test Quantity property
        Debug.Assert(testIngredient.Quantity == "200g", "Error: Quantity getter failed.");
        testIngredient.Quantity = "300g";
        Debug.Assert(testIngredient.Quantity == "300g", "Error: Quantity setter failed.");

        Console.WriteLine("All Ingredient class tests passed.");
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