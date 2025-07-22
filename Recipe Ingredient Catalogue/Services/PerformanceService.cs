using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeIngredientCatalogue.Services
{
    public static class PerformanceService
    {
        public static void RunPerformanceBenchmark(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
        {
            Console.WriteLine("=== Performance Benchmark ===");
            
            // Create test data if needed
            if (recipes.Count == 0)
            {
                Console.WriteLine("Creating test data for benchmarking...");
                CreateTestData(recipes, ingredients);
            }

            Stopwatch stopwatch = new();

            // Benchmark 1: Sequential search
            Console.WriteLine("\n1. Sequential Search Benchmark:");
            stopwatch.Start();
            var sequentialResults = recipes.Values.Where(r => r.Cuisine.Contains("Italian")).ToList();
            stopwatch.Stop();
            Console.WriteLine($"Sequential search found {sequentialResults.Count} results in {stopwatch.ElapsedMilliseconds} ms");

            // Benchmark 2: LINQ operations
            stopwatch.Restart();
            var sortedRecipes = recipes.Values.OrderBy(r => r.Name).ToList();
            stopwatch.Stop();
            Console.WriteLine($"LINQ sorting of {recipes.Count} recipes took {stopwatch.ElapsedMilliseconds} ms");

            // Benchmark 3: Dictionary lookup vs List search
            stopwatch.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var recipe = recipes.ContainsKey("Test Recipe 1");
            }
            stopwatch.Stop();
            Console.WriteLine($"1000 dictionary lookups took {stopwatch.ElapsedTicks} ticks");

            // Benchmark 4: Memory usage profiling
            RunMemoryBenchmark();
        }

        public static void RunParallelProcessingDemo(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
        {
            Console.WriteLine("=== Parallel Processing Demo ===");
            
            // Create test data if needed
            if (recipes.Count < 100)
            {
                Console.WriteLine("Creating test data for parallel processing demo...");
                CreateTestData(recipes, ingredients);
            }

            Stopwatch stopwatch = new();

            // Demo 1: Sequential vs Parallel processing
            RunSequentialVsParallelDemo(recipes, stopwatch);

            // Demo 2: Parallel.ForEach
            RunParallelForEachDemo(recipes, stopwatch);

            // Demo 3: Task-based processing
            RunTaskBasedDemo(recipes, stopwatch);
        }

        private static void RunSequentialVsParallelDemo(Dictionary<string, Recipe> recipes, Stopwatch stopwatch)
        {
            Console.WriteLine("\n1. Sequential vs Parallel Processing:");
            
            // Sequential processing
            stopwatch.Start();
            var sequentialResults = recipes.Values
                .Where(r => r.GetAverageRating() > 3.0)
                .Select(r => r.Name.ToUpper())
                .ToList();
            stopwatch.Stop();
            long sequentialTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Sequential processing: {sequentialTime} ms, {sequentialResults.Count} results");

            // Parallel processing
            stopwatch.Restart();
            var parallelResults = recipes.Values
                .AsParallel()
                .Where(r => r.GetAverageRating() > 3.0)
                .Select(r => r.Name.ToUpper())
                .ToList();
            stopwatch.Stop();
            long parallelTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Parallel processing: {parallelTime} ms, {parallelResults.Count} results");
            
            if (parallelTime > 0)
                Console.WriteLine($"Speedup: {(double)sequentialTime / parallelTime:F2}x");
        }

        private static void RunParallelForEachDemo(Dictionary<string, Recipe> recipes, Stopwatch stopwatch)
        {
            Console.WriteLine("\n2. Parallel.ForEach Demo:");
            var recipeList = recipes.Values.ToList();
            int processedCount = 0;

            stopwatch.Restart();
            Parallel.ForEach(recipeList, recipe =>
            {
                // Simulate some processing work
                Thread.Sleep(1);
                Interlocked.Increment(ref processedCount);
            });
            stopwatch.Stop();
            Console.WriteLine($"Parallel.ForEach processed {processedCount} recipes in {stopwatch.ElapsedMilliseconds} ms");
        }

        private static void RunTaskBasedDemo(Dictionary<string, Recipe> recipes, Stopwatch stopwatch)
        {
            Console.WriteLine("\n3. Task-based Processing:");
            stopwatch.Restart();
            
            var recipeList = recipes.Values.ToList();
            var tasks = new List<Task<int>>();
            var chunks = recipeList.Chunk(recipeList.Count / Environment.ProcessorCount + 1);
            
            foreach (var chunk in chunks)
            {
                var task = Task.Run(() =>
                {
                    int count = 0;
                    foreach (var recipe in chunk)
                    {
                        if (recipe.GetIngredients().Count > 2)
                            count++;
                    }
                    return count;
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            int totalComplexRecipes = tasks.Sum(t => t.Result);
            stopwatch.Stop();
            
            Console.WriteLine($"Task-based processing found {totalComplexRecipes} complex recipes in {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Used {tasks.Count} tasks across {Environment.ProcessorCount} processor cores");
        }

        private static void RunMemoryBenchmark()
        {
            long memoryBefore = GC.GetTotalMemory(false);
            var largeList = new List<Recipe>();
            for (int i = 0; i < 10000; i++)
            {
                largeList.Add(new Recipe($"Recipe {i}", "Test Cuisine", 30));
            }
            long memoryAfter = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory used for 10,000 recipes: {(memoryAfter - memoryBefore) / 1024} KB");

            // Clean up
            largeList.Clear();
            GC.Collect();
        }

        public static void CreateTestData(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
        {
            // Create test ingredients first
            IngredientService.CreateTestIngredients(ingredients);

            // Create test recipes
            string[] cuisines = { "Italian", "Mexican", "Chinese", "Indian", "French" };
            string[] ingredientNames = { "Tomato", "Onion", "Garlic", "Cheese", "Pasta", "Chicken", "Beef", "Rice", "Beans", "Pepper" };
            Random random = new();
            
            for (int i = 0; i < 500; i++)
            {
                string recipeName = $"Test Recipe {i + 1}";
                if (!recipes.ContainsKey(recipeName))
                {
                    string cuisine = cuisines[i % cuisines.Length];
                    Recipe recipe = new(recipeName, cuisine, 30);
                    
                    // Add random ingredients
                    int ingredientCount = random.Next(2, 6);
                    var selectedIngredients = ingredientNames.OrderBy(x => random.Next()).Take(ingredientCount);
                    
                    foreach (var ingredientName in selectedIngredients)
                    {
                        if (ingredients.ContainsKey(ingredientName))
                        {
                            recipe.AddIngredient(ingredients[ingredientName]);
                        }
                    }
                    
                    // Add random ratings
                    int ratingCount = random.Next(1, 6);
                    for (int j = 0; j < ratingCount; j++)
                    {
                        recipe.AddRating(random.Next(1, 6));
                    }
                    
                    recipes[recipeName] = recipe;
                }
            }
            
            Console.WriteLine($"Created {ingredients.Count} test ingredients and {recipes.Count} test recipes.");
        }
    }
}
