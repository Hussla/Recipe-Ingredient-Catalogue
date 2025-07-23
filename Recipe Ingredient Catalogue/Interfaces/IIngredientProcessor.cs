using System;
using System.Collections.Generic;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * IIngredientProcessor.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Defines the contract for ingredient processing operations using the Strategy pattern.
 * Enables different processing algorithms to be applied to ingredients without
 * modifying the ingredient classes themselves.
 * 
 * DESIGN PATTERNS:
 * • Strategy Pattern: Defines family of algorithms for ingredient processing
 * • Interface Segregation: Focused interface for specific processing operations
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Interfaces
{
    /// <summary>
    /// Strategy interface for ingredient processing operations
    /// </summary>
    public interface IIngredientProcessor<T> where T : Ingredient
    {
        /// <summary>
        /// Processes an ingredient using the specific strategy
        /// </summary>
        /// <param name="ingredient">The ingredient to process</param>
        /// <returns>Processing result information</returns>
        string Process(T ingredient);
        
        /// <summary>
        /// Validates if the ingredient can be processed by this strategy
        /// </summary>
        /// <param name="ingredient">The ingredient to validate</param>
        /// <returns>True if processable, false otherwise</returns>
        bool CanProcess(T ingredient);
    }

    /// <summary>
    /// Visitor interface for ingredient operations
    /// </summary>
    public interface IIngredientVisitor
    {
        void Visit(Ingredient ingredient);
        void Visit(PerishableIngredient perishableIngredient);
        void Visit(RefrigeratedIngredient refrigeratedIngredient);
        void Visit(FrozenIngredient frozenIngredient);
    }

    /// <summary>
    /// Factory interface for creating ingredients
    /// </summary>
    public interface IIngredientFactory
    {
        T CreateIngredient<T>(string name, int quantity, params object[] additionalParams) where T : Ingredient;
        Ingredient CreateIngredient(string type, string name, int quantity, params object[] additionalParams);
    }

    /// <summary>
    /// Repository interface for ingredient storage operations
    /// </summary>
    public interface IIngredientRepository<T> where T : Ingredient
    {
        void Add(string key, T ingredient);
        T Get(string key);
        bool Remove(string key);
        IEnumerable<T> GetAll();
        bool Exists(string key);
    }
}
