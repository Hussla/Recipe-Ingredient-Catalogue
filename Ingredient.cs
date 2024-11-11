using System;
using System.Collections.Generic;
using System.Diagnostics;

// Class representing an Ingredient
public class Ingredient
{
    // Member Variables
    private string name; // Stores the name of the ingredient
    private string quantity; // Stores the quantity of the ingredient (e.g., 100g, 1 cup)

    // Constructor
    // Initializes a new instance of the Ingredient class with the specified name and quantity
    public Ingredient(string name, string quantity)
    {
        this.name = name;
        this.quantity = quantity;
    }

    // Member Functions
    // Returns the name of the ingredient
    public string GetName()
    {
        return name;
    }

    // Sets a new name for the ingredient
    public void SetName(string name)
    {
        this.name = name;
    }

    // Returns the quantity of the ingredient
    public string GetQuantity()
    {
        return quantity;
    }

    // Sets a new quantity for the ingredient
    public void SetQuantity(string quantity)
    {
        this.quantity = quantity;
    }

    // Displays detailed information about the ingredient, including its name and quantity
    public void DisplayInfo()
    {
        Console.WriteLine($"Ingredient: {name}, Quantity: {quantity}");
    }

    // Unit Tests
    public static void RunTests()
    {
        // Create a test Ingredient instance
        Ingredient testIngredient = new Ingredient("Test Ingredient", "200g");

        // Test GetName
        Debug.Assert(testIngredient.GetName() == "Test Ingredient", "Error: GetName failed.");

        // Test GetQuantity
        Debug.Assert(testIngredient.GetQuantity() == "200g", "Error: GetQuantity failed.");

        // Test SetName
        testIngredient.SetName("Updated Ingredient");
        Debug.Assert(testIngredient.GetName() == "Updated Ingredient", "Error: SetName failed.");

        // Test SetQuantity
        testIngredient.SetQuantity("300g");
        Debug.Assert(testIngredient.GetQuantity() == "300g", "Error: SetQuantity failed.");

        Console.WriteLine("All Ingredient class tests passed.");
    }
}
