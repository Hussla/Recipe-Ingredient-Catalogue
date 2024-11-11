using System;
using System.Collections.Generic;
using System.Diagnostics;

// Class representing a Recipe
public class Recipe
{
    // Member Variables
    private string name; // Stores the name of the recipe
    private string cuisine; // Stores the type of cuisine (e.g., Italian, Mexican)
    private int preparationTime; // Stores the preparation time in minutes
    private List<Ingredient> ingredients; // Stores the list of ingredients for the recipe
    private List<int> ratings; // Stores user ratings for the recipe

    // Constructor
    // Initialises a new instance of the Recipe class with the specified name, cuisine, and preparation time
    public Recipe(string name, string cuisine, int preparationTime)
    {
        this.name = name;
        this.cuisine = cuisine;
        this.preparationTime = preparationTime;
        this.ingredients = new List<Ingredient>();
        this.ratings = new List<int>();
    }

    // Member Functions

    // Returns the name of the recipe
    public string GetName()
    {
        return name;
    }

    // Returns the type of cuisine
    public string GetCuisine()
    {
        return cuisine;
    }

    // Sets a new cuisine type for the recipe
    public void SetCuisine(string cuisine)
    {
        this.cuisine = cuisine;
    }

    // Returns the preparation time in minutes
    public int GetPreparationTime()
    {
        return preparationTime;
    }

    // Sets a new preparation time for the recipe
    public void SetPreparationTime(int preparationTime)
    {
        this.preparationTime = preparationTime;
    }

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
        Console.WriteLine("Recipe Name: " + name);
        Console.WriteLine("Cuisine: " + cuisine);
        Console.WriteLine("Preparation Time: " + preparationTime + " minutes");
        Console.WriteLine("Ingredients:");
        foreach (Ingredient ingredient in ingredients)
        {
            Console.WriteLine("- " + ingredient.GetName() + " (" + ingredient.GetQuantity() + ")");
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

    // Unit Tests
    public static void RunTests()
    {
        Recipe testRecipe = new Recipe("Test Recipe", "Test Cuisine", 30);

        // Test GetName
        Debug.Assert(testRecipe.GetName() == "Test Recipe", "Error: GetName failed.");

        // Test GetCuisine
        Debug.Assert(testRecipe.GetCuisine() == "Test Cuisine", "Error: GetCuisine failed.");

        // Test GetPreparationTime
        Debug.Assert(testRecipe.GetPreparationTime() == 30, "Error: GetPreparationTime failed.");

        // Test SetCuisine
        testRecipe.SetCuisine("Updated Cuisine");
        Debug.Assert(testRecipe.GetCuisine() == "Updated Cuisine", "Error: SetCuisine failed.");

        // Test SetPreparationTime
        testRecipe.SetPreparationTime(45);
        Debug.Assert(testRecipe.GetPreparationTime() == 45, "Error: SetPreparationTime failed.");

        // Test AddIngredient and GetIngredients
        Ingredient testIngredient = new Ingredient("Test Ingredient", "100g");
        testRecipe.AddIngredient(testIngredient);
        Debug.Assert(testRecipe.GetIngredients().Count == 1, "Error: AddIngredient failed.");
        Debug.Assert(testRecipe.GetIngredients()[0] == testIngredient, "Error: GetIngredients failed.");

        // Test AddRating and GetAverageRating
        testRecipe.AddRating(5);
        testRecipe.AddRating(4);
        Debug.Assert(testRecipe.GetAverageRating() == 4.5, "Error: GetAverageRating failed.");
    }
}
