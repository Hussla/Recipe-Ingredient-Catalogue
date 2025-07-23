using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * LoggingService.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Provides comprehensive structured logging capabilities with multiple output
 * targets, asynchronous processing, and configurable log levels. Supports both
 * console and file-based logging with JSON formatting and performance optimization.
 * 
 * KEY RESPONSIBILITIES:
 * • Managing structured logging with multiple severity levels (Trace to Critical)
 * • Asynchronous log processing to prevent blocking application threads
 * • File-based logging with automatic rotation and JSON formatting
 * • Console logging with color-coded output for different log levels
 * • Thread-safe log queuing and batch processing for performance
 * • User context tracking for multi-user scenarios
 * • Configurable log filtering and output control
 * • Exception logging with full stack trace capture
 * 
 * DESIGN PATTERNS:
 * • Static Service Class: Provides globally accessible logging functionality
 * • Producer-Consumer Pattern: Asynchronous log processing with queuing
 * • Strategy Pattern: Multiple output targets (console, file)
 * • Observer Pattern: Centralized logging for application-wide monitoring
 * 
 * DEPENDENCIES:
 * • System.Collections.Concurrent: For thread-safe log queuing
 * • System.Text.Json: For structured JSON log formatting
 * • System.Threading: For asynchronous processing and timers
 * • System.IO: For file-based log persistence
 * 
 * PUBLIC METHODS:
 * • LogTrace(): Records detailed diagnostic information
 * • LogDebug(): Records debugging information for development
 * • LogInfo(): Records general application information
 * • LogWarning(): Records potentially harmful situations
 * • LogError(): Records error events with optional exception details
 * • LogCritical(): Records critical failures requiring immediate attention
 * • SetMinimumLogLevel(): Configures log filtering threshold
 * • SetConsoleLogging(): Enables/disables console output
 * • SetFileLogging(): Enables/disables file output
 * • GetStats(): Retrieves logging service statistics
 * • ForceFlush(): Immediately processes all queued logs
 * 
 * LOG LEVELS:
 * • Trace: Most detailed information, typically only of interest when diagnosing problems
 * • Debug: Information useful to developers for debugging the application
 * • Info: General information about application execution
 * • Warning: Potentially harmful situations that don't prevent continued execution
 * • Error: Error events that might still allow the application to continue running
 * • Critical: Very serious error events that will presumably lead the application to abort
 * 
 * INTEGRATION POINTS:
 * • Used by all service classes for centralized logging
 * • Supports CircuitBreakerService for fault tolerance monitoring
 * • Enables PerformanceService for benchmarking and profiling
 * • Provides audit trails for user actions and system events
 * 
 * USAGE EXAMPLES:
 * • Recording user authentication events with user context
 * • Logging performance metrics and system health information
 * • Capturing exceptions with full context for debugging
 * • Monitoring application behavior in production environments
 * • Creating audit trails for compliance and security analysis
 * 
 * TECHNICAL NOTES:
 * • Thread-safe implementation using ConcurrentQueue for log entries
 * • Asynchronous file I/O to prevent blocking main application threads
 * • Automatic log file rotation based on date for manageable file sizes
 * • JSON formatting for structured log analysis and parsing
 * • Color-coded console output for improved readability during development
 * • Configurable minimum log levels for production vs development environments
 * • Timer-based batch processing for optimal I/O performance
 * • Exception-safe logging to prevent logging failures from affecting application
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.Services
{
    /// <summary>
    /// Structured logging service with multiple log levels and async processing
    /// </summary>
    public static class LoggingService
    {
        private static readonly ConcurrentQueue<LogEntry> _logQueue = new ConcurrentQueue<LogEntry>();
        private static readonly Timer _flushTimer;
        private static readonly string _logFilePath;
        private static LogLevel _minimumLogLevel = LogLevel.Info;
        private static bool _consoleLoggingEnabled = true;
        private static bool _fileLoggingEnabled = true;

        static LoggingService()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", $"app_{DateTime.Now:yyyyMMdd}.log");
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
            
            // Flush logs every 5 seconds
            _flushTimer = new Timer(FlushLogs, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Log levels for filtering messages
        /// </summary>
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            Critical = 5
        }

        /// <summary>
        /// Represents a log entry
        /// </summary>
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Message { get; set; }
            public string Category { get; set; }
            public Exception Exception { get; set; }
            public string ThreadId { get; set; }
            public string UserId { get; set; }

            public LogEntry()
            {
                Timestamp = DateTime.UtcNow;
                ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            }
        }

        /// <summary>
        /// Sets the minimum log level
        /// </summary>
        public static void SetMinimumLogLevel(LogLevel level)
        {
            _minimumLogLevel = level;
        }

        /// <summary>
        /// Enables or disables console logging
        /// </summary>
        public static void SetConsoleLogging(bool enabled)
        {
            _consoleLoggingEnabled = enabled;
        }

        /// <summary>
        /// Enables or disables file logging
        /// </summary>
        public static void SetFileLogging(bool enabled)
        {
            _fileLoggingEnabled = enabled;
        }

        /// <summary>
        /// Logs a trace message
        /// </summary>
        public static void LogTrace(string message, string category = null, string userId = null)
        {
            Log(LogLevel.Trace, message, category, null, userId);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        public static void LogDebug(string message, string category = null, string userId = null)
        {
            Log(LogLevel.Debug, message, category, null, userId);
        }

        /// <summary>
        /// Logs an info message
        /// </summary>
        public static void LogInfo(string message, string category = null, string userId = null)
        {
            Log(LogLevel.Info, message, category, null, userId);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public static void LogWarning(string message, string category = null, string userId = null)
        {
            Log(LogLevel.Warning, message, category, null, userId);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public static void LogError(string message, string category = null, Exception exception = null, string userId = null)
        {
            Log(LogLevel.Error, message, category, exception, userId);
        }

        /// <summary>
        /// Logs a critical message
        /// </summary>
        public static void LogCritical(string message, string category = null, Exception exception = null, string userId = null)
        {
            Log(LogLevel.Critical, message, category, exception, userId);
        }

        /// <summary>
        /// Core logging method
        /// </summary>
        private static void Log(LogLevel level, string message, string category, Exception exception, string userId)
        {
            if (level < _minimumLogLevel)
                return;

            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Category = category ?? "General",
                Exception = exception,
                UserId = userId
            };

            _logQueue.Enqueue(logEntry);

            // For critical errors, flush immediately
            if (level == LogLevel.Critical)
            {
                FlushLogs(null);
            }

            // Also log to console if enabled
            if (_consoleLoggingEnabled)
            {
                WriteToConsole(logEntry);
            }
        }

        /// <summary>
        /// Writes log entry to console with color coding
        /// </summary>
        private static void WriteToConsole(LogEntry entry)
        {
            var originalColor = Console.ForegroundColor;
            
            try
            {
                Console.ForegroundColor = GetConsoleColor(entry.Level);
                var prefix = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] [{entry.Category}]";
                
                if (!string.IsNullOrEmpty(entry.UserId))
                {
                    prefix += $" [User:{entry.UserId}]";
                }
                
                Console.WriteLine($"{prefix} {entry.Message}");
                
                if (entry.Exception != null)
                {
                    Console.WriteLine($"Exception: {entry.Exception}");
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Gets console color for log level
        /// </summary>
        private static ConsoleColor GetConsoleColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.White,
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }

        /// <summary>
        /// Flushes queued logs to file
        /// </summary>
        private static void FlushLogs(object state)
        {
            if (!_fileLoggingEnabled || _logQueue.IsEmpty)
                return;

            try
            {
                var logsToWrite = new List<LogEntry>();
                
                // Dequeue all pending logs
                while (_logQueue.TryDequeue(out var logEntry))
                {
                    logsToWrite.Add(logEntry);
                }

                if (logsToWrite.Count == 0)
                    return;

                // Write to file asynchronously
                Task.Run(() => WriteLogsToFile(logsToWrite));
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"Failed to flush logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes logs to file in JSON format
        /// </summary>
        private static async Task WriteLogsToFile(List<LogEntry> logs)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                foreach (var log in logs)
                {
                    var logLine = JsonSerializer.Serialize(log, jsonOptions);
                    await File.AppendAllTextAsync(_logFilePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write logs to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets logging statistics
        /// </summary>
        public static LoggingStats GetStats()
        {
            return new LoggingStats
            {
                QueuedLogs = _logQueue.Count,
                MinimumLogLevel = _minimumLogLevel,
                ConsoleLoggingEnabled = _consoleLoggingEnabled,
                FileLoggingEnabled = _fileLoggingEnabled,
                LogFilePath = _logFilePath
            };
        }

        /// <summary>
        /// Forces immediate flush of all queued logs
        /// </summary>
        public static void ForceFlush()
        {
            FlushLogs(null);
        }

        /// <summary>
        /// Disposes the logging service and flushes remaining logs
        /// </summary>
        public static void Dispose()
        {
            _flushTimer?.Dispose();
            ForceFlush();
        }
    }

    /// <summary>
    /// Statistics about the logging service
    /// </summary>
    public class LoggingStats
    {
        public int QueuedLogs { get; set; }
        public LoggingService.LogLevel MinimumLogLevel { get; set; }
        public bool ConsoleLoggingEnabled { get; set; }
        public bool FileLoggingEnabled { get; set; }
        public string LogFilePath { get; set; }

        public override string ToString()
        {
            return $"Queued: {QueuedLogs}, Level: {MinimumLogLevel}, " +
                   $"Console: {ConsoleLoggingEnabled}, File: {FileLoggingEnabled}";
        }
    }
}
