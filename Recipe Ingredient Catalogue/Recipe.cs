using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Recipe_Ingredient_Catalogue
{
    /// <summary>
    /// Contains information regarding Recipes and includes getters and setters to access public properties
    /// Demonstrates inheritance and polymorphism for different types of recipes.
    /// </summary>
    public class Recipe
    {
        // Properties with getters and setters
        public string Name { get; set; } // Stores the name of the recipe
        public string Cuisine { get; set; } // Stores the type of cuisine (e.g., Italian, Mexican)
        public int PreparationTime { get; set; } // Stores the preparation time in minutes
        protected List<Ingredient> ingredients; // Stores the list of ingredients for the recipe
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

        // Virtual method to display detailed information about the recipe
        public virtual void DisplayInfo()
        {
            Console.WriteLine("Recipe Name: " + Name);
            Console.WriteLine("Cuisine: " + Cuisine);
            Console.WriteLine("Preparation Time: " + PreparationTime + " minutes");
            Console.WriteLine("Ingredients:");
            foreach (Ingredient ingredient in ingredients)
            {
                ingredient.DisplayInfo();
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
            Debug.Assert(
                testRecipe.GetIngredients()[0] == testIngredient,
                "Error: GetIngredients failed."
            );

            // Test AddRating and GetAverageRating
            testRecipe.AddRating(5);
            testRecipe.AddRating(4);
            Debug.Assert(testRecipe.GetAverageRating() == 4.5, "Error: GetAverageRating failed.");

            Console.WriteLine("All Recipe class tests passed.");
        }
    }

    // Derived class representing a Dessert Recipe
    public class DessertRecipe : Recipe
    {
        public bool IsSweet { get; set; } // Indicates if the dessert is sweet

        // Constructor
        public DessertRecipe(string name, string cuisine, int preparationTime, bool isSweet)
            : base(name, cuisine, preparationTime)
        {
            IsSweet = isSweet;
        }

        // Overridden method to display detailed information about the dessert recipe
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Is Sweet: " + (IsSweet ? "Yes" : "No"));
        }
    }

    // Derived class representing a Main Course Recipe
    public class MainCourseRecipe : Recipe
    {
        public bool IsVegetarian { get; set; } // Indicates if the main course is vegetarian

        // Constructor
        public MainCourseRecipe(string name, string cuisine, int preparationTime, bool isVegetarian)
            : base(name, cuisine, preparationTime)
        {
            IsVegetarian = isVegetarian;
        }

        // Overridden method to display detailed information about the main course recipe
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Is Vegetarian: " + (IsVegetarian ? "Yes" : "No"));
        }
    }

    /*
    Detailed Explanation of the Code:
    
    The code defines a recipe management system that uses object-oriented programming concepts, specifically inheritance and polymorphism, to represent different types of recipes.
    
    1. Recipe Class (Base Class):
       - **Properties**:
         - `Name` (string): Stores the name of the recipe (e.g., "Spaghetti Bolognese").
         - `Cuisine` (string): Stores the type of cuisine (e.g., "Italian", "Mexican").
         - `PreparationTime` (int): Stores the preparation time in minutes.
         - `ingredients` (List<Ingredient>): A protected list that stores the ingredients for the recipe.
         - `ratings` (List<int>): A private list that stores user ratings for the recipe.
       - **Constructor**: Initializes the recipe with the specified name, cuisine, and preparation time, and initializes the lists for ingredients and ratings.
       - **Methods**:
         - `AddIngredient(Ingredient ingredient)`: Adds an ingredient to the recipe.
         - `GetIngredients()`: Returns the list of ingredients in the recipe.
         - `DisplayInfo()`: A virtual method that displays the detailed information about the recipe, including name, cuisine, preparation time, and ingredients.
         - `AddRating(int rating)`: Adds a rating to the recipe, validating that the rating is between 1 and 5.
         - `GetAverageRating()`: Calculates and returns the average rating of the recipe. Returns 0.0 if no ratings are available.
         - `RunTests()`: A static method that runs unit tests to verify the correctness of the Recipe class properties and methods.
       - **Usage**: The `Recipe` class serves as the base class for any generic recipe and includes common properties and methods applicable to all recipes.
    
    2. DessertRecipe Class (Derived from Recipe):
       - **Property**:
         - `IsSweet` (bool): Indicates whether the dessert is sweet.
       - **Constructor**: Initializes the dessert recipe with the specified name, cuisine, preparation time, and sweetness.
       - **Methods**:
         - `DisplayInfo()`: Overrides the base class method to include additional information about whether the dessert is sweet.
       - **Usage**: This class is used to represent dessert recipes, extending the base `Recipe` class to include a specific attribute for desserts.
    
    3. MainCourseRecipe Class (Derived from Recipe):
       - **Property**:
         - `IsVegetarian` (bool): Indicates whether the main course is vegetarian.
       - **Constructor**: Initializes the main course recipe with the specified name, cuisine, preparation time, and vegetarian status.
       - **Methods**:
         - `DisplayInfo()`: Overrides the base class method to include additional information about whether the main course is vegetarian.
       - **Usage**: This class is used to represent main course recipes, extending the base `Recipe` class to include a specific attribute for main courses.
    
    4. Object-Oriented Concepts Used:
       - **Inheritance**: The `DessertRecipe` and `MainCourseRecipe` classes inherit from the `Recipe` class, allowing them to reuse common properties and methods.
       - **Polymorphism**: The `DisplayInfo()` method in the base class is declared as virtual and is overridden in the derived classes (`DessertRecipe` and `MainCourseRecipe`). This allows each derived class to provide its own implementation of the method, ensuring that the correct version is called based on the runtime type of the object.
       - **Encapsulation**: The properties like `Name`, `Cuisine`, and `PreparationTime` use getters and setters to control access, while the `ratings` list is kept private to manage rating data securely.
    
    Summary:
    The code demonstrates a well-structured approach to modeling a recipe catalog using object-oriented principles. The base `Recipe` class provides a template for all recipes, while derived classes (`DessertRecipe` and `MainCourseRecipe`) extend the functionality to represent specific types of recipes with additional attributes. This approach ensures scalability, reusability, and easy maintenance, making it possible to add more specialized types of recipes in the future.
    */

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
}
