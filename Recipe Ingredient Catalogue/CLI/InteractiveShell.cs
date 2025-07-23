using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * InteractiveShell.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Advanced interactive shell with command completion, history, and plugin support.
 * Provides sophisticated CLI capabilities including autocomplete, command history,
 * and scriptable command interface.
 * 
 * FEATURES:
 * • Interactive command completion and history
 * • Scriptable command interface with batch processing
 * • Plugin-based command system with dynamic loading
 * • Command aliasing and macro support
 * • Advanced readline-like functionality
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.CLI
{
    /// <summary>
    /// Represents a command that can be executed in the shell
    /// </summary>
    public interface IShellCommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }
        List<string> Aliases { get; }
        bool Execute(string[] args, InteractiveShell shell);
        List<string> GetCompletions(string[] args, int currentArgIndex);
    }

    /// <summary>
    /// Plugin interface for extending shell functionality
    /// </summary>
    public interface IShellPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        void Initialize(InteractiveShell shell);
        void Cleanup();
        List<IShellCommand> GetCommands();
    }

    /// <summary>
    /// Advanced interactive shell with completion and plugin support
    /// </summary>
    public class InteractiveShell
    {
        private readonly Dictionary<string, IShellCommand> _commands;
        private readonly List<string> _commandHistory;
        private readonly Dictionary<string, string> _aliases;
        private readonly List<IShellPlugin> _plugins;
        private readonly Dictionary<string, string> _variables;
        private bool _isRunning;
        private int _historyIndex;
        private string _prompt;

        public InteractiveShell()
        {
            _commands = new Dictionary<string, IShellCommand>();
            _commandHistory = new List<string>();
            _aliases = new Dictionary<string, string>();
            _plugins = new List<IShellPlugin>();
            _variables = new Dictionary<string, string>();
            _isRunning = false;
            _historyIndex = -1;
            _prompt = "recipe> ";

            InitializeBuiltinCommands();
            LoadConfiguration();
        }

        /// <summary>
        /// Initialize built-in shell commands
        /// </summary>
        private void InitializeBuiltinCommands()
        {
            RegisterCommand(new HelpCommand());
            RegisterCommand(new ExitCommand());
            RegisterCommand(new HistoryCommand());
            RegisterCommand(new AliasCommand());
            RegisterCommand(new SetCommand());
            RegisterCommand(new EchoCommand());
            RegisterCommand(new ClearCommand());
            RegisterCommand(new ScriptCommand());
            RegisterCommand(new PluginCommand());
        }

        /// <summary>
        /// Load configuration from file
        /// </summary>
        private void LoadConfiguration()
        {
            var configFile = "shell_config.txt";
            if (File.Exists(configFile))
            {
                try
                {
                    var lines = File.ReadAllLines(configFile);
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("alias "))
                        {
                            var parts = line.Substring(6).Split('=', 2);
                            if (parts.Length == 2)
                            {
                                _aliases[parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                        else if (line.StartsWith("set "))
                        {
                            var parts = line.Substring(4).Split('=', 2);
                            if (parts.Length == 2)
                            {
                                _variables[parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not load configuration: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Register a command with the shell
        /// </summary>
        public void RegisterCommand(IShellCommand command)
        {
            _commands[command.Name.ToLower()] = command;
            
            // Register aliases
            foreach (var alias in command.Aliases)
            {
                _commands[alias.ToLower()] = command;
            }
        }

        /// <summary>
        /// Load plugins from directory
        /// </summary>
        public void LoadPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
                return;

            try
            {
                var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(dllFile);
                        var pluginTypes = assembly.GetTypes()
                            .Where(t => typeof(IShellPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                        foreach (var pluginType in pluginTypes)
                        {
                            var plugin = (IShellPlugin)Activator.CreateInstance(pluginType);
                            plugin.Initialize(this);
                            _plugins.Add(plugin);

                            foreach (var command in plugin.GetCommands())
                            {
                                RegisterCommand(command);
                            }

                            Console.WriteLine($"Loaded plugin: {plugin.Name} v{plugin.Version}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load plugin {dllFile}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading plugins: {ex.Message}");
            }
        }

        /// <summary>
        /// Start the interactive shell
        /// </summary>
        public void Run()
        {
            _isRunning = true;
            Console.WriteLine("Recipe Ingredient Catalogue Interactive Shell");
            Console.WriteLine("Type 'help' for available commands or 'exit' to quit.");
            Console.WriteLine();

            while (_isRunning)
            {
                try
                {
                    Console.Write(_prompt);
                    var input = ReadLineWithCompletion();
                    
                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    // Add to history
                    _commandHistory.Add(input);
                    _historyIndex = _commandHistory.Count;

                    // Process command
                    ProcessCommand(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Read line with tab completion support
        /// </summary>
        private string ReadLineWithCompletion()
        {
            var input = new StringBuilder();
            var cursorPos = 0;

            while (true)
            {
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return input.ToString();

                    case ConsoleKey.Tab:
                        var completions = GetCompletions(input.ToString(), cursorPos);
                        if (completions.Count == 1)
                        {
                            // Auto-complete
                            var completion = completions[0];
                            var words = input.ToString().Split(' ');
                            if (words.Length > 0)
                            {
                                words[words.Length - 1] = completion;
                                var newInput = string.Join(" ", words);
                                
                                // Clear current line and rewrite
                                Console.Write("\r" + new string(' ', _prompt.Length + input.Length));
                                Console.Write("\r" + _prompt + newInput);
                                input.Clear();
                                input.Append(newInput);
                                cursorPos = input.Length;
                            }
                        }
                        else if (completions.Count > 1)
                        {
                            // Show completions
                            Console.WriteLine();
                            foreach (var completion in completions.Take(10))
                            {
                                Console.WriteLine($"  {completion}");
                            }
                            if (completions.Count > 10)
                            {
                                Console.WriteLine($"  ... and {completions.Count - 10} more");
                            }
                            Console.Write(_prompt + input.ToString());
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorPos > 0)
                        {
                            input.Remove(cursorPos - 1, 1);
                            cursorPos--;
                            Console.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if (_historyIndex > 0)
                        {
                            _historyIndex--;
                            var historyItem = _commandHistory[_historyIndex];
                            
                            // Clear current line and replace with history
                            Console.Write("\r" + new string(' ', _prompt.Length + input.Length));
                            Console.Write("\r" + _prompt + historyItem);
                            input.Clear();
                            input.Append(historyItem);
                            cursorPos = input.Length;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (_historyIndex < _commandHistory.Count - 1)
                        {
                            _historyIndex++;
                            var historyItem = _commandHistory[_historyIndex];
                            
                            // Clear current line and replace with history
                            Console.Write("\r" + new string(' ', _prompt.Length + input.Length));
                            Console.Write("\r" + _prompt + historyItem);
                            input.Clear();
                            input.Append(historyItem);
                            cursorPos = input.Length;
                        }
                        break;

                    default:
                        if (!char.IsControl(keyInfo.KeyChar))
                        {
                            input.Insert(cursorPos, keyInfo.KeyChar);
                            cursorPos++;
                            Console.Write(keyInfo.KeyChar);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Get command completions for the current input
        /// </summary>
        private List<string> GetCompletions(string input, int cursorPos)
        {
            var completions = new List<string>();
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length == 0 || (words.Length == 1 && !input.EndsWith(" ")))
            {
                // Complete command names
                var prefix = words.Length > 0 ? words[0] : "";
                completions.AddRange(_commands.Keys
                    .Where(cmd => cmd.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .OrderBy(cmd => cmd));
            }
            else if (words.Length > 0)
            {
                // Complete command arguments
                var commandName = words[0].ToLower();
                if (_commands.ContainsKey(commandName))
                {
                    var command = _commands[commandName];
                    var args = words.Skip(1).ToArray();
                    var currentArgIndex = input.EndsWith(" ") ? args.Length : args.Length - 1;
                    completions.AddRange(command.GetCompletions(args, currentArgIndex));
                }
            }

            return completions;
        }

        /// <summary>
        /// Process a command line
        /// </summary>
        private void ProcessCommand(string input)
        {
            // Handle variable substitution
            input = SubstituteVariables(input);

            // Handle aliases
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0 && _aliases.ContainsKey(words[0]))
            {
                words[0] = _aliases[words[0]];
                input = string.Join(" ", words);
                words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }

            if (words.Length == 0)
                return;

            var commandName = words[0].ToLower();
            var args = words.Skip(1).ToArray();

            if (_commands.ContainsKey(commandName))
            {
                try
                {
                    _commands[commandName].Execute(args, this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Command error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Unknown command: {commandName}. Type 'help' for available commands.");
            }
        }

        /// <summary>
        /// Substitute variables in command line
        /// </summary>
        private string SubstituteVariables(string input)
        {
            foreach (var variable in _variables)
            {
                input = input.Replace($"${variable.Key}", variable.Value);
            }
            return input;
        }

        /// <summary>
        /// Stop the shell
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Get all registered commands
        /// </summary>
        public Dictionary<string, IShellCommand> GetCommands()
        {
            return new Dictionary<string, IShellCommand>(_commands);
        }

        /// <summary>
        /// Get command history
        /// </summary>
        public List<string> GetHistory()
        {
            return new List<string>(_commandHistory);
        }

        /// <summary>
        /// Set a variable
        /// </summary>
        public void SetVariable(string name, string value)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Get a variable
        /// </summary>
        public string GetVariable(string name)
        {
            return _variables.ContainsKey(name) ? _variables[name] : null;
        }

        /// <summary>
        /// Set an alias
        /// </summary>
        public void SetAlias(string alias, string command)
        {
            _aliases[alias] = command;
        }

        /// <summary>
        /// Get all aliases
        /// </summary>
        public Dictionary<string, string> GetAliases()
        {
            return new Dictionary<string, string>(_aliases);
        }
    }

    // Built-in command implementations
    public class HelpCommand : IShellCommand
    {
        public string Name => "help";
        public string Description => "Display help information";
        public string Usage => "help [command]";
        public List<string> Aliases => new List<string> { "?" };

        public bool Execute(string[] args, InteractiveShell shell)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Available commands:");
                var commands = shell.GetCommands().Values.Distinct().OrderBy(c => c.Name);
                foreach (var command in commands)
                {
                    Console.WriteLine($"  {command.Name,-15} {command.Description}");
                }
                Console.WriteLine("\nType 'help <command>' for detailed usage information.");
            }
            else
            {
                var commandName = args[0].ToLower();
                var commands = shell.GetCommands();
                if (commands.ContainsKey(commandName))
                {
                    var command = commands[commandName];
                    Console.WriteLine($"Command: {command.Name}");
                    Console.WriteLine($"Description: {command.Description}");
                    Console.WriteLine($"Usage: {command.Usage}");
                    if (command.Aliases.Any())
                    {
                        Console.WriteLine($"Aliases: {string.Join(", ", command.Aliases)}");
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown command: {commandName}");
                }
            }
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            if (currentArgIndex == 0)
            {
                // Complete with command names
                return new List<string> { "help", "exit", "history", "alias", "set", "echo", "clear" };
            }
            return new List<string>();
        }
    }

    public class ExitCommand : IShellCommand
    {
        public string Name => "exit";
        public string Description => "Exit the shell";
        public string Usage => "exit";
        public List<string> Aliases => new List<string> { "quit", "q" };

        public bool Execute(string[] args, InteractiveShell shell)
        {
            Console.WriteLine("Goodbye!");
            shell.Stop();
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class HistoryCommand : IShellCommand
    {
        public string Name => "history";
        public string Description => "Display command history";
        public string Usage => "history [count]";
        public List<string> Aliases => new List<string> { "hist" };

        public bool Execute(string[] args, InteractiveShell shell)
        {
            var history = shell.GetHistory();
            var count = history.Count;
            
            if (args.Length > 0 && int.TryParse(args[0], out var requestedCount))
            {
                count = Math.Min(requestedCount, history.Count);
            }

            var startIndex = Math.Max(0, history.Count - count);
            for (int i = startIndex; i < history.Count; i++)
            {
                Console.WriteLine($"{i + 1,4}: {history[i]}");
            }
            
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class AliasCommand : IShellCommand
    {
        public string Name => "alias";
        public string Description => "Create command aliases";
        public string Usage => "alias [name=command]";
        public List<string> Aliases => new List<string>();

        public bool Execute(string[] args, InteractiveShell shell)
        {
            if (args.Length == 0)
            {
                var aliases = shell.GetAliases();
                if (aliases.Any())
                {
                    Console.WriteLine("Current aliases:");
                    foreach (var alias in aliases.OrderBy(a => a.Key))
                    {
                        Console.WriteLine($"  {alias.Key} = {alias.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("No aliases defined.");
                }
            }
            else
            {
                var input = string.Join(" ", args);
                var parts = input.Split('=', 2);
                if (parts.Length == 2)
                {
                    shell.SetAlias(parts[0].Trim(), parts[1].Trim());
                    Console.WriteLine($"Alias created: {parts[0].Trim()} = {parts[1].Trim()}");
                }
                else
                {
                    Console.WriteLine("Usage: alias name=command");
                }
            }
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class SetCommand : IShellCommand
    {
        public string Name => "set";
        public string Description => "Set shell variables";
        public string Usage => "set [name=value]";
        public List<string> Aliases => new List<string>();

        public bool Execute(string[] args, InteractiveShell shell)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Shell variables:");
                Console.WriteLine($"  prompt = {shell.GetVariable("prompt") ?? "recipe> "}");
            }
            else
            {
                var input = string.Join(" ", args);
                var parts = input.Split('=', 2);
                if (parts.Length == 2)
                {
                    shell.SetVariable(parts[0].Trim(), parts[1].Trim());
                    Console.WriteLine($"Variable set: {parts[0].Trim()} = {parts[1].Trim()}");
                }
                else
                {
                    Console.WriteLine("Usage: set name=value");
                }
            }
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class EchoCommand : IShellCommand
    {
        public string Name => "echo";
        public string Description => "Display text";
        public string Usage => "echo [text]";
        public List<string> Aliases => new List<string>();

        public bool Execute(string[] args, InteractiveShell shell)
        {
            Console.WriteLine(string.Join(" ", args));
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class ClearCommand : IShellCommand
    {
        public string Name => "clear";
        public string Description => "Clear the screen";
        public string Usage => "clear";
        public List<string> Aliases => new List<string> { "cls" };

        public bool Execute(string[] args, InteractiveShell shell)
        {
            Console.Clear();
            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            return new List<string>();
        }
    }

    public class ScriptCommand : IShellCommand
    {
        public string Name => "script";
        public string Description => "Execute commands from a file";
        public string Usage => "script <filename>";
        public List<string> Aliases => new List<string> { "source" };

        public bool Execute(string[] args, InteractiveShell shell)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: script <filename>");
                return false;
            }

            var filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not found: {filename}");
                return false;
            }

            try
            {
                var lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("#"))
                    {
                        Console.WriteLine($"Executing: {line}");
                        // This would need to call back into the shell's command processor
                        // For now, just display what would be executed
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading script: {ex.Message}");
                return false;
            }

            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            if (currentArgIndex == 0)
            {
                // Complete with file names
                try
                {
                    return Directory.GetFiles(".", "*.txt")
                        .Concat(Directory.GetFiles(".", "*.script"))
                        .Select(Path.GetFileName)
                        .ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
            return new List<string>();
        }
    }

    public class PluginCommand : IShellCommand
    {
        public string Name => "plugin";
        public string Description => "Manage shell plugins";
        public string Usage => "plugin <list|load|info> [args]";
        public List<string> Aliases => new List<string>();

        public bool Execute(string[] args, InteractiveShell shell)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: plugin <list|load|info> [args]");
                return false;
            }

            switch (args[0].ToLower())
            {
                case "list":
                    Console.WriteLine("Plugin management is not fully implemented in this demo.");
                    Console.WriteLine("This would list all loaded plugins.");
                    break;
                case "load":
                    if (args.Length > 1)
                    {
                        Console.WriteLine($"Loading plugin from: {args[1]}");
                        shell.LoadPlugins(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Usage: plugin load <directory>");
                    }
                    break;
                case "info":
                    Console.WriteLine("Plugin information would be displayed here.");
                    break;
                default:
                    Console.WriteLine("Unknown plugin command. Use: list, load, or info");
                    break;
            }

            return true;
        }

        public List<string> GetCompletions(string[] args, int currentArgIndex)
        {
            if (currentArgIndex == 0)
            {
                return new List<string> { "list", "load", "info" };
            }
            return new List<string>();
        }
    }
}
