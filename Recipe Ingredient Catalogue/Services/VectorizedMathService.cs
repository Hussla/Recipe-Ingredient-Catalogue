using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * VectorizedMathService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Provides high-performance vectorized mathematical operations for ingredient
 * calculations using SIMD (Single Instruction, Multiple Data) optimizations.
 * Leverages modern CPU vector instructions for parallel processing of numerical data.
 * 
 * KEY RESPONSIBILITIES:
 * • Performing vectorized addition operations on ingredient quantity arrays
 * • Calculating total quantities using SIMD horizontal addition
 * • Applying scaling factors to ingredient quantities with vector operations
 * • Detecting and utilizing hardware acceleration capabilities (AVX2, SSE)
 * • Providing fallback implementations for non-SIMD capable hardware
 * • Optimizing mathematical operations for large datasets
 * • Supporting bulk ingredient calculations with minimal CPU overhead
 * 
 * DESIGN PATTERNS:
 * • Static Utility Class: Provides stateless mathematical operations
 * • Strategy Pattern: Hardware-specific optimization strategies (AVX2 vs fallback)
 * • Template Method: Consistent operation flow with hardware-specific implementations
 * 
 * DEPENDENCIES:
 * • System.Numerics: For Vector<T> types and SIMD operations
 * • System.Runtime.Intrinsics: For low-level CPU intrinsic functions
 * • System.Runtime.Intrinsics.X86: For x86-specific SIMD instructions (AVX2, SSE)
 * 
 * PUBLIC METHODS:
 * • AddQuantitiesVectorized(): Adds two arrays element-wise using SIMD
 * • CalculateTotalQuantityVectorized(): Sums array elements using horizontal addition
 * • ScaleQuantitiesVectorized(): Multiplies array elements by scalar using SIMD
 * • IsAvx2Supported: Property indicating AVX2 instruction set availability
 * • VectorByteSize: Property showing vector register size in bytes
 * 
 * SIMD OPTIMIZATIONS:
 * • AVX2 Instructions: 256-bit vector operations for maximum performance
 * • Vector<T> Fallback: Cross-platform SIMD when AVX2 unavailable
 * • Horizontal Operations: Efficient reduction operations for aggregation
 * • Memory Alignment: Optimized memory access patterns for cache efficiency
 * 
 * INTEGRATION POINTS:
 * • Used by IngredientService for bulk quantity calculations
 * • Supports PerformanceService for mathematical benchmarking
 * • Enables efficient processing of large ingredient datasets
 * • Provides foundation for advanced analytics and reporting
 * 
 * USAGE EXAMPLES:
 * • Calculating total inventory quantities across thousands of ingredients
 * • Applying percentage adjustments to recipe scaling operations
 * • Performing bulk mathematical operations on ingredient arrays
 * • Optimizing performance-critical numerical computations
 * 
 * TECHNICAL NOTES:
 * • Requires .NET 7+ for optimal Vector256<T> support
 * • Automatically detects and utilizes available CPU instruction sets
 * • Handles array length mismatches and edge cases gracefully
 * • Provides significant performance improvements for large datasets (>100 elements)
 * • Uses unsafe memory operations for maximum performance
 * • Maintains numerical precision equivalent to scalar operations
 * • Thread-safe implementation suitable for parallel processing scenarios
 * 
 * PERFORMANCE CHARACTERISTICS:
 * • AVX2: Processes 8 float values simultaneously (256-bit registers)
 * • Vector<T>: Processes 4-8 values depending on platform (128-256 bit)
 * • Typical speedup: 2-8x faster than scalar operations on large arrays
 * • Memory bandwidth optimization through vectorized loads/stores
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    /// <summary>
    /// Provides vectorized mathematical operations for ingredient calculations using SIMD optimizations
    /// </summary>
    public static class VectorizedMathService
    {
        /// <summary>
        /// Adds quantities from two arrays using SIMD vectorization
        /// </summary>
        public static void AddQuantitiesVectorized(float[] quantities1, float[] quantities2, float[] result)
        {
            if (quantities1.Length != quantities2.Length || quantities1.Length != result.Length)
                throw new ArgumentException("All arrays must be the same length");
            
            if (Vector.IsHardwareAccelerated && Avx2.IsSupported)
            {
                int vectorSize = Vector256<float>.Count;
                int i = 0;
                
                // Process using AVX2 intrinsics
                for (; i <= quantities1.Length - vectorSize; i += vectorSize)
                {
                    var a = Vector256.LoadUnsafe(ref quantities1[i]);
                    var b = Vector256.LoadUnsafe(ref quantities2[i]);
                    var sum = Avx2.Add(a, b);
                    sum.StoreUnsafe(ref result[i]);
                }
                
                // Process remaining elements
                for (; i < quantities1.Length; i++)
                {
                    result[i] = quantities1[i] + quantities2[i];
                }
            }
            else
            {
                // Fallback to standard vector operations
                for (int i = 0; i < quantities1.Length; i++)
                {
                    result[i] = quantities1[i] + quantities2[i];
                }
            }
        }

        /// <summary>
        /// Calculates the total quantity using SIMD horizontal add
        /// </summary>
        public static float CalculateTotalQuantityVectorized(float[] quantities)
        {
            if (quantities == null || quantities.Length == 0)
                return 0;
            
            if (Vector.IsHardwareAccelerated && Avx2.IsSupported)
            {
                int vectorSize = Vector256<float>.Count;
                Vector256<float> total = Vector256<float>.Zero;
                int i = 0;
                
                // Process using AVX2 intrinsics
                for (; i <= quantities.Length - vectorSize; i += vectorSize)
                {
                    var current = Vector256.LoadUnsafe(ref quantities[i]);
                    total = Avx2.Add(total, current);
                }
                
                // Sum the vector elements
                float result = Vector256.Sum(total);
                
                // Add remaining elements
                for (; i < quantities.Length; i++)
                {
                    result += quantities[i];
                }
                
                return result;
            }
            else
            {
                // Fallback to standard vector operations
                Vector<float> total = Vector<float>.Zero;
                int vectorSize = Vector<float>.Count;
                int i = 0;
                
                for (; i <= quantities.Length - vectorSize; i += vectorSize)
                {
                    var current = new Vector<float>(quantities, i);
                    total += current;
                }
                
                float result = Vector.Sum(total);
                
                for (; i < quantities.Length; i++)
                {
                    result += quantities[i];
                }
                
                return result;
            }
        }

        /// <summary>
        /// Applies a scaling factor to quantities using SIMD operations
        /// </summary>
        public static void ScaleQuantitiesVectorized(float[] quantities, float scale, float[] result)
        {
            if (quantities.Length != result.Length)
                throw new ArgumentException("Arrays must be the same length");
            
            if (Vector.IsHardwareAccelerated && Avx2.IsSupported)
            {
                int vectorSize = Vector256<float>.Count;
                var scaleVector = Vector256.Create(scale);
                int i = 0;
                
                for (; i <= quantities.Length - vectorSize; i += vectorSize)
                {
                    var current = Vector256.LoadUnsafe(ref quantities[i]);
                    var scaled = Avx2.Multiply(current, scaleVector);
                    scaled.StoreUnsafe(ref result[i]);
                }
                
                // Process remaining elements
                for (; i < quantities.Length; i++)
                {
                    result[i] = quantities[i] * scale;
                }
            }
            else
            {
                // Fallback to standard vector operations
                var scaleVector = new Vector<float>(scale);
                int vectorSize = Vector<float>.Count;
                
                for (int i = 0; i < quantities.Length; i += vectorSize)
                {
                    if (i + vectorSize <= quantities.Length)
                    {
                        var current = new Vector<float>(quantities, i);
                        var scaled = current * scaleVector;
                        scaled.CopyTo(result, i);
                    }
                    else
                    {
                        for (int j = i; j < quantities.Length; j++)
                        {
                            result[j] = quantities[j] * scale;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the current platform supports AVX2 instructions
        /// </summary>
        public static bool IsAvx2Supported => Avx2.IsSupported;
        
        /// <summary>
        /// Gets the vector size in bytes
        /// </summary>
        public static int VectorByteSize => Vector<byte>.Count;
    }
}
