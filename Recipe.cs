using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Runtime.Serialization;

// Class representing a Recipe
[Serializable]
public class Recipe
{
    // Properties with getters and setters
    public string Name { get; set; } // Stores the name of the recipe
    public string Cuisine { get; set; } // Stores the type of cuisine (e.g., Italian, Mexican)
    public int PreparationTime { get; set; } // Stores the preparation time in minutes
    private List<Ingredient> ingredients; // Stores the list of ingredients for the recipe
    private List<int> ratings; // Stores user ratings for the recipe

    // Constructor
    // Initialises a new instance of the Recipe class with the specified name, cuisine, and preparation time
    public Recipe(string name, string cuisine, int preparationTime)
    {
        Name = name;
        Cuisine = cuisine;
        PreparationTime = preparationTime;
        ingredients = new List<Ingredient>();
        ratings = new List<int>();
    }

    // Member Functions

    // Adds an ingredient to the recipe
    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
    }

    // Returns the list of ingredients in the recipe
    public List<Ingredient> GetIngredients()
    {
        return ingredients;
    }

    // Displays detailed information about the recipe, including its name, cuisine, preparation time, and ingredients
    public void DisplayInfo()
    {
        Console.WriteLine("Recipe Name: " + Name);
        Console.WriteLine("Cuisine: " + Cuisine);
        Console.WriteLine("Preparation Time: " + PreparationTime + " minutes");
        Console.WriteLine("Ingredients:");
        foreach (Ingredient ingredient in ingredients)
        {
            ingredient.DisplayInfo(); // Call DisplayInfo method of Ingredient class to show ingredient details
        }
        Console.WriteLine("Average Rating: " + GetAverageRating());
    }

    // Adds a rating to the recipe
    public void AddRating(int rating)
    {
        if (rating >= 1 && rating <= 5)
        {
            ratings.Add(rating);
        }
        else
        {
            throw new ArgumentException("Rating must be between 1 and 5.");
        }
    }

    // Calculates and returns the average rating of the recipe
    public double GetAverageRating()
    {
        if (ratings.Count == 0)
        {
            return 0.0; // No ratings available
        }
        else
        {
            return ratings.Average();
        }
    }

    // Checks if the recipe's average rating is above or equal to the specified rating
    public bool HasRatingAboveOrEqual(double rating)
    {
        return GetAverageRating() >= rating;
    }

    // Unit Tests
    public static void RunTests()
    {
        Recipe testRecipe = new Recipe("Test Recipe", "Test Cuisine", 30);

        // Test Name property
        Debug.Assert(testRecipe.Name == "Test Recipe", "Error: Name getter failed.");
        testRecipe.Name = "Updated Recipe";
        Debug.Assert(testRecipe.Name == "Updated Recipe", "Error: Name setter failed.");

        // Test Cuisine property
        Debug.Assert(testRecipe.Cuisine == "Test Cuisine", "Error: Cuisine getter failed.");
        testRecipe.Cuisine = "Updated Cuisine";
        Debug.Assert(testRecipe.Cuisine == "Updated Cuisine", "Error: Cuisine setter failed.");

        // Test PreparationTime property
        Debug.Assert(testRecipe.PreparationTime == 30, "Error: PreparationTime getter failed.");
        testRecipe.PreparationTime = 45;
        Debug.Assert(testRecipe.PreparationTime == 45, "Error: PreparationTime setter failed.");

        // Test AddIngredient and GetIngredients
        Ingredient testIngredient = new Ingredient("Test Ingredient", 100);
        testRecipe.AddIngredient(testIngredient);
        Debug.Assert(testRecipe.GetIngredients().Count == 1, "Error: AddIngredient failed.");
        Debug.Assert(testRecipe.GetIngredients()[0] == testIngredient, "Error: GetIngredients failed.");

        // Test AddRating and GetAverageRating
        testRecipe.AddRating(5);
        testRecipe.AddRating(4);
        Debug.Assert(Math.Abs(testRecipe.GetAverageRating() - 4.5) < 0.001, "Error: GetAverageRating failed.");

        // Test HasRatingAboveOrEqual
        Debug.Assert(testRecipe.HasRatingAboveOrEqual(4.0), "Error: HasRatingAboveOrEqual failed for rating 4.0");
        Debug.Assert(!testRecipe.HasRatingAboveOrEqual(5.0), "Error: HasRatingAboveOrEqual failed for rating 5.0");

        Console.WriteLine("All Recipe class tests passed.");
    }
}
/*
Usage:
The Recipe class can be instantiated with specific names, cuisines, and preparation times for different recipes.
The DisplayInfo() method can be called to print the details of each recipe instance to the console.
The AddIngredient() method can be used to add ingredients to the recipe.                                    
The GetAverageRating() method can be used to calculate the average rating of the recipe.
The RunTests() method provides a simple way to verify the correctness of the Recipe class by running predefined assertions.         

Summary:
The Recipe.cs file defines a Recipe class that encapsulates properties for name, cuisine, preparation time, ingredients, and ratings, along with methods to display this information, add ingredients, calculate the average rating, and perform basic unit testing. 
This structure is useful in applications where managing and displaying recipe information is required.
*/
