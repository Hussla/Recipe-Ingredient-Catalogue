using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace Recipe_Ingredient_Catalogue
{
    // Class representing Nutritional Information for an ingredient
    public class NutritionalInfo
    {
        public int Calories { get; set; } // Calories per unit
        public int Carbohydrates { get; set; } // Carbohydrates in grams per unit
        public int Proteins { get; set; } // Proteins in grams per unit
        public int Fats { get; set; } // Fats in grams per unit

        public NutritionalInfo(int calories, int carbohydrates, int proteins, int fats)
        {
            Calories = calories;
            Carbohydrates = carbohydrates;
            Proteins = proteins;
            Fats = fats;
        }

        public void DisplayInfo()
        {
            Console.WriteLine(
                $"Calories: {Calories}, Carbohydrates: {Carbohydrates}g, Proteins: {Proteins}g, Fats: {Fats}g"
            );
        }
    }

    // Base class representing an Ingredient
    public class Ingredient
    {
        private string v1;
        private int v2;

        // Properties with getters and setters
        public string Name { get; set; } // Stores the name of the ingredient
        public int Quantity { get; set; } // Stores the quantity of the ingredient (e.g., 100g, 1 cup)
        public string Unit { get; set; } // Stores the unit of measurement (e.g., grams, ml, cups)
        public decimal PricePerUnit { get; set; } // Stores the cost per unit of ingredient
        public DateTime ExpirationDate { get; set; } // Stores the expiration date of the ingredient
        public bool IsAllergen { get; set; } // Indicates if the ingredient is a common allergen
        public NutritionalInfo Nutrition { get; set; } // Stores nutritional information for the ingredient
        public List<string> Tags { get; set; } // Stores tags for categorization

        // Constructor
        // Initializes a new instance of the Ingredient class with the specified parameters
        public Ingredient(
            string name,
            int quantity,
            string unit,
            decimal pricePerUnit,
            DateTime expirationDate,
            bool isAllergen,
            NutritionalInfo nutrition
        )
        {
            Name = name;
            Quantity = quantity;
            Unit = unit;
            PricePerUnit = pricePerUnit;
            ExpirationDate = expirationDate;
            IsAllergen = isAllergen;
            Nutrition = nutrition;
            Tags = new List<string>();
        }

        public Ingredient(string v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        // Member Functions

        // Calculates the total calories based on quantity
        public int CalculateTotalCalories()
        {
            return Quantity * Nutrition.Calories;
        }

        // Calculates the total cost based on quantity
        public decimal GetTotalCost()
        {
            return Quantity * PricePerUnit;
        }

        // Checks if the ingredient is expired
        public bool IsExpired()
        {
            return DateTime.Now > ExpirationDate;
        }

        // Converts the unit of the ingredient to another supported unit
        public void ConvertUnit(string targetUnit)
        {
            if (Unit == "grams" && targetUnit == "kilograms")
            {
                Quantity /= 1000;
                Unit = "kilograms";
            }
            else if (Unit == "kilograms" && targetUnit == "grams")
            {
                Quantity *= 1000;
                Unit = "grams";
            }
            else if (Unit == "milliliters" && targetUnit == "liters")
            {
                Quantity /= 1000;
                Unit = "liters";
            }
            else if (Unit == "liters" && targetUnit == "milliliters")
            {
                Quantity *= 1000;
                Unit = "milliliters";
            }
            else
            {
                Console.WriteLine("Conversion not supported.");
            }
        }

        // Adds a tag to the ingredient
        public void AddTag(string tag)
        {
            Tags.Add(tag);
        }

        // Displays the tags of the ingredient
        public void DisplayTags()
        {
            Console.WriteLine("Tags: " + string.Join(", ", Tags));
        }

        // Virtual Member Function
        // Displays detailed information about the ingredient, including its name, quantity, unit, calories, price, and expiration status
        public virtual void DisplayInfo()
        {
            Console.WriteLine(
                $"Ingredient: {Name}, Quantity: {Quantity} {Unit}, Price per Unit: {PricePerUnit:C}"
            );
            Console.WriteLine($"Total Cost: {GetTotalCost():C}");
            Console.WriteLine(
                $"Expiration Date: {ExpirationDate.ToShortDateString()}, Expired: {(IsExpired() ? "Yes" : "No")}"
            );
            if (IsAllergen)
            {
                Console.WriteLine("Warning: This ingredient is a common allergen.");
            }
            Console.WriteLine("Nutritional Information:");
            Nutrition.DisplayInfo();
            DisplayTags();
        }

        // Serializes the ingredient to JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        // Deserializes an ingredient from JSON
        public static Ingredient FromJson(string json)
        {
            return JsonSerializer.Deserialize<Ingredient>(json);
        }

        // Unit Tests
        public static void RunTests()
        {
            // Create a test NutritionalInfo instance
            NutritionalInfo testNutrition = new NutritionalInfo(50, 10, 5, 2);

            // Create a test Ingredient instance
            Ingredient testIngredient = new Ingredient(
                "Test Ingredient",
                200,
                "grams",
                1.25m,
                DateTime.Now.AddDays(10),
                true,
                testNutrition
            );

            // Test Name property
            Debug.Assert(testIngredient.Name == "Test Ingredient", "Error: Name getter failed.");
            testIngredient.Name = "Updated Ingredient";
            Debug.Assert(testIngredient.Name == "Updated Ingredient", "Error: Name setter failed.");

            // Test Quantity property
            Debug.Assert(testIngredient.Quantity == 200, "Error: Quantity getter failed.");
            testIngredient.Quantity = 300;
            Debug.Assert(testIngredient.Quantity == 300, "Error: Quantity setter failed.");

            // Test CalculateTotalCalories
            Debug.Assert(
                testIngredient.CalculateTotalCalories() == 15000,
                "Error: CalculateTotalCalories failed."
            );

            // Test GetTotalCost
            Debug.Assert(testIngredient.GetTotalCost() == 375.00m, "Error: GetTotalCost failed.");

            // Test IsExpired
            Debug.Assert(testIngredient.IsExpired() == false, "Error: IsExpired failed.");

            // Test Unit Conversion
            testIngredient.ConvertUnit("kilograms");
            Debug.Assert(
                testIngredient.Quantity == 0,
                "Error: Unit conversion to kilograms failed."
            );
            Debug.Assert(testIngredient.Unit == "kilograms", "Error: Unit type conversion failed.");

            // Test Tagging
            testIngredient.AddTag("Organic");
            Debug.Assert(testIngredient.Tags.Contains("Organic"), "Error: Tagging failed.");

            Console.WriteLine("All Ingredient class tests passed.");
        }

        public string GetName()
        {
            return this.Name;
        }
    }

    // Derived class representing a Liquid Ingredient
    public class LiquidIngredient : Ingredient
    {
        // Constructor
        // Initializes a new instance of the LiquidIngredient class with the specified parameters
        public LiquidIngredient(
            string name,
            int quantity,
            string unit,
            decimal pricePerUnit,
            DateTime expirationDate,
            bool isAllergen,
            NutritionalInfo nutrition
        )
            : base(name, quantity, unit, pricePerUnit, expirationDate, isAllergen, nutrition) { }

        // Overridden Member Function
        // Displays detailed information about the liquid ingredient, including its name, quantity, and unit
        public override void DisplayInfo()
        {
            Console.WriteLine($"Liquid Ingredient: {Name}, Quantity: {Quantity} {Unit}");
            base.DisplayInfo();
        }
    }

    // Derived class representing a Solid Ingredient
    public class SolidIngredient : Ingredient, ICookable
    {
        // Additional property for Solid Ingredients
        public string State { get; set; } // Stores the state of the ingredient (e.g., chopped, diced, whole)

        // Constructor
        // Initializes a new instance of the SolidIngredient class with the specified parameters
        public SolidIngredient(
            string name,
            int quantity,
            string unit,
            decimal pricePerUnit,
            DateTime expirationDate,
            bool isAllergen,
            NutritionalInfo nutrition,
            string state
        )
            : base(name, quantity, unit, pricePerUnit, expirationDate, isAllergen, nutrition)
        {
            State = state;
        }

        // Overridden Member Function
        // Displays detailed information about the solid ingredient, including its name, quantity, unit, and state
        public override void DisplayInfo()
        {
            Console.WriteLine($"Solid Ingredient: {Name}, Quantity: {Quantity}, State: {State}");
            base.DisplayInfo();
        }

        void ICookable.Cook()
        {
            throw new NotImplementedException();
        }
    }

    /*
    Detailed Explanation of the Code:
    
    The code defines an inheritance structure for representing different types of ingredients in a recipe.
    It includes a base class called 'Ingredient' and two derived classes called 'LiquidIngredient' and 'SolidIngredient'.
    Inheritance and polymorphism are used to enable flexible and reusable code, allowing for specialized ingredient types to be defined.
    
    1. Ingredient Class (Base Class):
       - Properties:
         - 'Name' (string): Stores the name of the ingredient (e.g., 'Sugar', 'Flour').
         - 'Quantity' (int): Stores the quantity of the ingredient (e.g., 100g, 1 cup).
       - Constructor:
         - Initializes an instance of the 'Ingredient' class with the given name and quantity.
       - Methods:
         - 'DisplayInfo()': A virtual method that displays information about the ingredient. This method can be overridden in derived classes.
         - 'RunTests()': A static method that performs unit testing on the 'Ingredient' class to verify the correctness of its properties.
         - 'GetName()': A simple getter method for the 'Name' property.
       - Usage:
         - This base class can be used to represent any generic ingredient without any specific characteristics (e.g., solid or liquid).
    
    2. LiquidIngredient Class (Derived from Ingredient):
       - Properties:
         - 'Unit' (string): Stores the unit of measurement for the liquid ingredient (e.g., 'ml', 'liters').
       - Constructor:
         - Initializes an instance of the 'LiquidIngredient' class with the given name, quantity, and unit. It calls the base class constructor to initialize common properties.
       - Methods:
         - 'DisplayInfo()': An overridden method that displays detailed information about the liquid ingredient, including its name, quantity, and unit.
       - Usage:
         - This derived class is used to represent liquid ingredients where a specific unit of measurement is required, making it more specialized than the base 'Ingredient' class.
    
    3. SolidIngredient Class (Derived from Ingredient):
       - Properties:
         - 'State' (string): Stores the state of the solid ingredient (e.g., 'chopped', 'diced', 'whole').
       - Constructor:
         - Initializes an instance of the 'SolidIngredient' class with the given name, quantity, and state. It calls the base class constructor to initialize common properties.
       - Methods:
         - 'DisplayInfo()': An overridden method that displays detailed information about the solid ingredient, including its name, quantity, and state.
       - Usage:
         - This derived class is used to represent solid ingredients with additional information about their state, making it more specialized than the base 'Ingredient' class.
    
    4. Object-Oriented Concepts Used:
       - Inheritance:
         - The 'LiquidIngredient' and 'SolidIngredient' classes inherit from the 'Ingredient' class, allowing them to reuse common properties and methods.
       - Polymorphism:
         - The 'DisplayInfo()' method is declared as virtual in the base class and is overridden in the derived classes. This allows for different implementations of the method, providing customized behavior based on the ingredient type.
       - Encapsulation:
         - Properties like 'Name', 'Quantity', 'Unit', and 'State' are encapsulated using getters and setters, providing controlled access to these fields.
    
    Summary:
    This code provides a basic structure for managing different types of ingredients in a recipe application. By using inheritance and polymorphism, it allows for easy extension and specialized behavior for different types of ingredients. The 'DisplayInfo()' method, which demonstrates polymorphism, enables the correct display of ingredient information based on the object's runtime type.
    */


    /*
    Usage:
    The Ingredient class can be instantiated with specific names and quantities for different ingredients.
    The DisplayInfo() method can be called to print the details of each ingredient instance to the console.
    The RunTests() method provides a simple way to verify the correctness of the Ingredient class by running predefined assertions.
    
    Summary:
    The Ingredient.cs file defines an Ingredient class that encapsulates properties for name and quantity, along with methods to display this information and perform basic unit testing.
    This structure is useful in applications where managing and displaying recipe ingredients is required.
    */
}
