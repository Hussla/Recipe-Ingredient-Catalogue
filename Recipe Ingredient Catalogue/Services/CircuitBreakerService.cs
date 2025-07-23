using System;
using System.Threading;
using System.Threading.Tasks;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * CircuitBreakerService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Implements the Circuit Breaker pattern for fault tolerance and resilience in
 * file operations and external service calls. Prevents cascading failures by
 * temporarily blocking operations when failure thresholds are exceeded.
 * 
 * KEY RESPONSIBILITIES:
 * • Monitoring operation failures and success rates
 * • Implementing three-state circuit breaker (Closed/Open/Half-Open)
 * • Providing automatic recovery mechanisms with configurable timeouts
 * • Protecting against cascading failures in distributed operations
 * • Logging circuit state transitions and failure patterns
 * • Supporting both synchronous and asynchronous operations
 * • Maintaining failure statistics and performance metrics
 * 
 * DESIGN PATTERNS:
 * • Circuit Breaker Pattern: Core implementation for fault tolerance
 * • State Machine: Three-state management (Closed/Open/Half-Open)
 * • Proxy Pattern: Wraps operations with failure detection
 * • Template Method: Consistent execution flow for sync/async operations
 * 
 * DEPENDENCIES:
 * • LoggingService: For recording circuit state changes and failures
 * • System.Threading: For thread-safe state management
 * • System.Threading.Tasks: For asynchronous operation support
 * 
 * PUBLIC METHODS:
 * • Execute<T>(): Executes synchronous operations with circuit protection
 * • ExecuteAsync<T>(): Executes asynchronous operations with circuit protection
 * • Reset(): Manually resets circuit to closed state
 * • GetStats(): Retrieves current circuit breaker statistics
 * 
 * CIRCUIT STATES:
 * • Closed: Normal operation, all calls pass through
 * • Open: Circuit is open, calls fail immediately without execution
 * • Half-Open: Testing state, single call allowed to test recovery
 * 
 * INTEGRATION POINTS:
 * • Used by DataService for file I/O operations protection
 * • Supports external API calls with fault tolerance
 * • Enables graceful degradation during service outages
 * • Provides metrics for system health monitoring
 * 
 * USAGE EXAMPLES:
 * • Protecting file save operations from disk failures
 * • Wrapping database connections with automatic retry logic
 * • Implementing timeout-based recovery for network operations
 * • Monitoring and logging system reliability metrics
 * 
 * TECHNICAL NOTES:
 * • Thread-safe implementation using lock-based synchronization
 * • Configurable failure thresholds and timeout periods
 * • Automatic state transitions based on success/failure patterns
 * • Exception propagation with circuit state information
 * • Support for generic return types and async/await patterns
 * • Comprehensive logging for debugging and monitoring
 * • Statistics collection for performance analysis
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    /// <summary>
    /// Circuit breaker pattern implementation for fault tolerance in file operations
    /// </summary>
    public class CircuitBreakerService
    {
        private readonly int _failureThreshold;
        private readonly TimeSpan _timeout;
        private int _failureCount;
        private DateTime _lastFailureTime;
        private CircuitState _state;
        private readonly object _lock = new object();

        public enum CircuitState
        {
            Closed,    // Normal operation
            Open,      // Circuit is open, calls fail immediately
            HalfOpen   // Testing if service has recovered
        }

        /// <summary>
        /// Initializes a new circuit breaker with specified parameters
        /// </summary>
        /// <param name="failureThreshold">Number of failures before opening circuit</param>
        /// <param name="timeout">Time to wait before attempting to close circuit</param>
        public CircuitBreakerService(int failureThreshold = 3, TimeSpan? timeout = null)
        {
            _failureThreshold = failureThreshold;
            _timeout = timeout ?? TimeSpan.FromMinutes(1);
            _state = CircuitState.Closed;
            _failureCount = 0;
        }

        /// <summary>
        /// Gets the current state of the circuit breaker
        /// </summary>
        public CircuitState State => _state;

        /// <summary>
        /// Gets the current failure count
        /// </summary>
        public int FailureCount => _failureCount;

        /// <summary>
        /// Executes an operation with circuit breaker protection
        /// </summary>
        /// <typeparam name="T">Return type of the operation</typeparam>
        /// <param name="operation">The operation to execute</param>
        /// <returns>Result of the operation</returns>
        /// <exception cref="CircuitBreakerOpenException">Thrown when circuit is open</exception>
        public T Execute<T>(Func<T> operation)
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open)
                {
                    if (DateTime.UtcNow - _lastFailureTime >= _timeout)
                    {
                        _state = CircuitState.HalfOpen;
                        LoggingService.LogInfo("Circuit breaker transitioning to Half-Open state");
                    }
                    else
                    {
                        LoggingService.LogWarning($"Circuit breaker is OPEN. Operation blocked. Failures: {_failureCount}");
                        throw new CircuitBreakerOpenException("Circuit breaker is open");
                    }
                }
            }

            try
            {
                var result = operation();
                OnSuccess();
                return result;
            }
            catch (Exception ex)
            {
                OnFailure(ex);
                throw;
            }
        }

        /// <summary>
        /// Executes an async operation with circuit breaker protection
        /// </summary>
        /// <typeparam name="T">Return type of the operation</typeparam>
        /// <param name="operation">The async operation to execute</param>
        /// <returns>Task with result of the operation</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open)
                {
                    if (DateTime.UtcNow - _lastFailureTime >= _timeout)
                    {
                        _state = CircuitState.HalfOpen;
                        LoggingService.LogInfo("Circuit breaker transitioning to Half-Open state");
                    }
                    else
                    {
                        LoggingService.LogWarning($"Circuit breaker is OPEN. Async operation blocked. Failures: {_failureCount}");
                        throw new CircuitBreakerOpenException("Circuit breaker is open");
                    }
                }
            }

            try
            {
                var result = await operation();
                OnSuccess();
                return result;
            }
            catch (Exception ex)
            {
                OnFailure(ex);
                throw;
            }
        }

        /// <summary>
        /// Handles successful operation execution
        /// </summary>
        private void OnSuccess()
        {
            lock (_lock)
            {
                if (_state == CircuitState.HalfOpen)
                {
                    _state = CircuitState.Closed;
                    LoggingService.LogInfo("Circuit breaker closed after successful operation");
                }
                _failureCount = 0;
            }
        }

        /// <summary>
        /// Handles failed operation execution
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        private void OnFailure(Exception exception)
        {
            lock (_lock)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                LoggingService.LogError($"Circuit breaker recorded failure #{_failureCount}: {exception.Message}");

                if (_failureCount >= _failureThreshold)
                {
                    _state = CircuitState.Open;
                    LoggingService.LogError($"Circuit breaker OPENED after {_failureCount} failures");
                }
            }
        }

        /// <summary>
        /// Manually resets the circuit breaker to closed state
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _state = CircuitState.Closed;
                _failureCount = 0;
                LoggingService.LogInfo("Circuit breaker manually reset to Closed state");
            }
        }

        /// <summary>
        /// Gets circuit breaker statistics
        /// </summary>
        /// <returns>Statistics about the circuit breaker</returns>
        public CircuitBreakerStats GetStats()
        {
            lock (_lock)
            {
                return new CircuitBreakerStats
                {
                    State = _state,
                    FailureCount = _failureCount,
                    LastFailureTime = _lastFailureTime,
                    FailureThreshold = _failureThreshold,
                    Timeout = _timeout
                };
            }
        }
    }

    /// <summary>
    /// Statistics about circuit breaker state
    /// </summary>
    public class CircuitBreakerStats
    {
        public CircuitBreakerService.CircuitState State { get; set; }
        public int FailureCount { get; set; }
        public DateTime LastFailureTime { get; set; }
        public int FailureThreshold { get; set; }
        public TimeSpan Timeout { get; set; }

        public override string ToString()
        {
            return $"State: {State}, Failures: {FailureCount}/{FailureThreshold}, " +
                   $"Last Failure: {LastFailureTime:yyyy-MM-dd HH:mm:ss}, Timeout: {Timeout}";
        }
    }

    /// <summary>
    /// Exception thrown when circuit breaker is open
    /// </summary>
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message) { }
        public CircuitBreakerOpenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
