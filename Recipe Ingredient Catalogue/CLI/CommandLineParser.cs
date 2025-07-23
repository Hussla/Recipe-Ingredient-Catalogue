using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * CommandLineParser.cs - Recipe Ingredient Catalogue
 * ═══════════════════════════════════════════════════════════════════════════════
 * 
 * PURPOSE:
 * Advanced command-line argument parsing with support for flags, options, and
 * complex parameter combinations. Provides sophisticated CLI capabilities
 * beyond basic argument processing.
 * 
 * FEATURES:
 * • Complex flag parsing with short (-v) and long (--verbose) forms
 * • Option parsing with values (--output=file.json)
 * • Positional argument handling
 * • Command validation and help generation
 * • Plugin-based command system
 * 
 * ═══════════════════════════════════════════════════════════════════════════════
 */

namespace RecipeIngredientCatalogue.CLI
{
    /// <summary>
    /// Represents a parsed command line argument
    /// </summary>
    public class ParsedArgument
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ArgumentType Type { get; set; }
        public bool IsPresent { get; set; }
    }

    /// <summary>
    /// Types of command line arguments
    /// </summary>
    public enum ArgumentType
    {
        Flag,           // --verbose, -v
        Option,         // --output=file.json, -o file.json
        Positional,     // recipe.json (no prefix)
        Command         // add, remove, list
    }

    /// <summary>
    /// Definition of a command line argument
    /// </summary>
    public class ArgumentDefinition
    {
        public string Name { get; set; }
        public string ShortForm { get; set; }
        public string LongForm { get; set; }
        public ArgumentType Type { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public List<string> ValidValues { get; set; } = new List<string>();
    }

    /// <summary>
    /// Advanced command line parser with comprehensive flag and option support
    /// </summary>
    public class CommandLineParser
    {
        private readonly List<ArgumentDefinition> _definitions;
        private readonly Dictionary<string, ParsedArgument> _parsedArguments;
        private readonly List<string> _positionalArguments;
        private readonly List<string> _errors;

        public CommandLineParser()
        {
            _definitions = new List<ArgumentDefinition>();
            _parsedArguments = new Dictionary<string, ParsedArgument>();
            _positionalArguments = new List<string>();
            _errors = new List<string>();
            
            InitializeStandardArguments();
        }

        /// <summary>
        /// Initialize standard command line arguments
        /// </summary>
        private void InitializeStandardArguments()
        {
            // Help flag
            AddArgument(new ArgumentDefinition
            {
                Name = "help",
                ShortForm = "-h",
                LongForm = "--help",
                Type = ArgumentType.Flag,
                Description = "Display help information"
            });

            // Verbose flag
            AddArgument(new ArgumentDefinition
            {
                Name = "verbose",
                ShortForm = "-v",
                LongForm = "--verbose",
                Type = ArgumentType.Flag,
                Description = "Enable verbose output"
            });

            // Version flag
            AddArgument(new ArgumentDefinition
            {
                Name = "version",
                ShortForm = "-V",
                LongForm = "--version",
                Type = ArgumentType.Flag,
                Description = "Display version information"
            });

            // Output file option
            AddArgument(new ArgumentDefinition
            {
                Name = "output",
                ShortForm = "-o",
                LongForm = "--output",
                Type = ArgumentType.Option,
                Description = "Specify output file path"
            });

            // Input file option
            AddArgument(new ArgumentDefinition
            {
                Name = "input",
                ShortForm = "-i",
                LongForm = "--input",
                Type = ArgumentType.Option,
                Description = "Specify input file path"
            });

            // Format option
            AddArgument(new ArgumentDefinition
            {
                Name = "format",
                ShortForm = "-f",
                LongForm = "--format",
                Type = ArgumentType.Option,
                Description = "Specify output format",
                DefaultValue = "json",
                ValidValues = new List<string> { "json", "xml", "csv", "binary" }
            });

            // User role option
            AddArgument(new ArgumentDefinition
            {
                Name = "role",
                ShortForm = "-r",
                LongForm = "--role",
                Type = ArgumentType.Option,
                Description = "Specify user role",
                DefaultValue = "user",
                ValidValues = new List<string> { "admin", "user", "guest" }
            });

            // Batch mode flag
            AddArgument(new ArgumentDefinition
            {
                Name = "batch",
                ShortForm = "-b",
                LongForm = "--batch",
                Type = ArgumentType.Flag,
                Description = "Run in batch mode (non-interactive)"
            });

            // Configuration file option
            AddArgument(new ArgumentDefinition
            {
                Name = "config",
                ShortForm = "-c",
                LongForm = "--config",
                Type = ArgumentType.Option,
                Description = "Specify configuration file path"
            });

            // Log level option
            AddArgument(new ArgumentDefinition
            {
                Name = "log-level",
                LongForm = "--log-level",
                Type = ArgumentType.Option,
                Description = "Set logging level",
                DefaultValue = "info",
                ValidValues = new List<string> { "debug", "info", "warn", "error" }
            });

            // Plugin directory option
            AddArgument(new ArgumentDefinition
            {
                Name = "plugin-dir",
                LongForm = "--plugin-dir",
                Type = ArgumentType.Option,
                Description = "Specify plugin directory path"
            });
        }

        /// <summary>
        /// Add a new argument definition
        /// </summary>
        public void AddArgument(ArgumentDefinition definition)
        {
            _definitions.Add(definition);
        }

        /// <summary>
        /// Parse command line arguments
        /// </summary>
        public bool Parse(string[] args)
        {
            _parsedArguments.Clear();
            _positionalArguments.Clear();
            _errors.Clear();

            // Initialize all arguments as not present
            foreach (var def in _definitions)
            {
                _parsedArguments[def.Name] = new ParsedArgument
                {
                    Name = def.Name,
                    Type = def.Type,
                    IsPresent = false,
                    Value = def.DefaultValue
                };
            }

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("--"))
                {
                    // Long form argument
                    ParseLongFormArgument(arg, args, ref i);
                }
                else if (arg.StartsWith("-") && arg.Length > 1)
                {
                    // Short form argument(s)
                    ParseShortFormArgument(arg, args, ref i);
                }
                else
                {
                    // Positional argument
                    _positionalArguments.Add(arg);
                }
            }

            // Validate required arguments
            ValidateRequiredArguments();

            return _errors.Count == 0;
        }

        /// <summary>
        /// Parse long form arguments (--argument or --argument=value)
        /// </summary>
        private void ParseLongFormArgument(string arg, string[] args, ref int index)
        {
            string name, value = null;

            if (arg.Contains("="))
            {
                var parts = arg.Split('=', 2);
                name = parts[0];
                value = parts[1];
            }
            else
            {
                name = arg;
            }

            var definition = _definitions.FirstOrDefault(d => d.LongForm == name);
            if (definition == null)
            {
                _errors.Add($"Unknown argument: {name}");
                return;
            }

            if (definition.Type == ArgumentType.Flag)
            {
                _parsedArguments[definition.Name].IsPresent = true;
                _parsedArguments[definition.Name].Value = "true";
            }
            else if (definition.Type == ArgumentType.Option)
            {
                if (value == null && index + 1 < args.Length && !args[index + 1].StartsWith("-"))
                {
                    value = args[++index];
                }

                if (value == null)
                {
                    _errors.Add($"Option {name} requires a value");
                    return;
                }

                if (definition.ValidValues.Any() && !definition.ValidValues.Contains(value))
                {
                    _errors.Add($"Invalid value '{value}' for {name}. Valid values: {string.Join(", ", definition.ValidValues)}");
                    return;
                }

                _parsedArguments[definition.Name].IsPresent = true;
                _parsedArguments[definition.Name].Value = value;
            }
        }

        /// <summary>
        /// Parse short form arguments (-a or -abc for multiple flags)
        /// </summary>
        private void ParseShortFormArgument(string arg, string[] args, ref int index)
        {
            for (int i = 1; i < arg.Length; i++)
            {
                var shortForm = "-" + arg[i];
                var definition = _definitions.FirstOrDefault(d => d.ShortForm == shortForm);

                if (definition == null)
                {
                    _errors.Add($"Unknown argument: {shortForm}");
                    continue;
                }

                if (definition.Type == ArgumentType.Flag)
                {
                    _parsedArguments[definition.Name].IsPresent = true;
                    _parsedArguments[definition.Name].Value = "true";
                }
                else if (definition.Type == ArgumentType.Option)
                {
                    string value = null;

                    // If this is the last character and there's a next argument
                    if (i == arg.Length - 1 && index + 1 < args.Length && !args[index + 1].StartsWith("-"))
                    {
                        value = args[++index];
                    }
                    // If there are more characters, use the rest as the value
                    else if (i < arg.Length - 1)
                    {
                        value = arg.Substring(i + 1);
                        i = arg.Length; // Break out of the loop
                    }

                    if (value == null)
                    {
                        _errors.Add($"Option {shortForm} requires a value");
                        continue;
                    }

                    if (definition.ValidValues.Any() && !definition.ValidValues.Contains(value))
                    {
                        _errors.Add($"Invalid value '{value}' for {shortForm}. Valid values: {string.Join(", ", definition.ValidValues)}");
                        continue;
                    }

                    _parsedArguments[definition.Name].IsPresent = true;
                    _parsedArguments[definition.Name].Value = value;
                }
            }
        }

        /// <summary>
        /// Validate that all required arguments are present
        /// </summary>
        private void ValidateRequiredArguments()
        {
            foreach (var definition in _definitions.Where(d => d.Required))
            {
                if (!_parsedArguments[definition.Name].IsPresent)
                {
                    _errors.Add($"Required argument missing: {definition.Name}");
                }
            }
        }

        /// <summary>
        /// Check if an argument is present
        /// </summary>
        public bool HasArgument(string name)
        {
            return _parsedArguments.ContainsKey(name) && _parsedArguments[name].IsPresent;
        }

        /// <summary>
        /// Get the value of an argument
        /// </summary>
        public string GetArgumentValue(string name)
        {
            return _parsedArguments.ContainsKey(name) ? _parsedArguments[name].Value : null;
        }

        /// <summary>
        /// Get all positional arguments
        /// </summary>
        public List<string> GetPositionalArguments()
        {
            return new List<string>(_positionalArguments);
        }

        /// <summary>
        /// Get parsing errors
        /// </summary>
        public List<string> GetErrors()
        {
            return new List<string>(_errors);
        }

        /// <summary>
        /// Generate help text
        /// </summary>
        public string GenerateHelp(string programName = "RecipeCatalogue")
        {
            var help = new StringBuilder();
            help.AppendLine($"Usage: {programName} [OPTIONS] [COMMAND] [ARGUMENTS]");
            help.AppendLine();
            help.AppendLine("A comprehensive recipe and ingredient management system with advanced CLI capabilities.");
            help.AppendLine();
            help.AppendLine("OPTIONS:");

            var maxWidth = _definitions.Max(d => GetArgumentDisplayName(d).Length);

            foreach (var def in _definitions.OrderBy(d => d.Name))
            {
                var displayName = GetArgumentDisplayName(def);
                var padding = new string(' ', maxWidth - displayName.Length + 2);
                help.AppendLine($"  {displayName}{padding}{def.Description}");

                if (def.ValidValues.Any())
                {
                    var valuePadding = new string(' ', maxWidth + 4);
                    help.AppendLine($"{valuePadding}Valid values: {string.Join(", ", def.ValidValues)}");
                }

                if (!string.IsNullOrEmpty(def.DefaultValue))
                {
                    var valuePadding = new string(' ', maxWidth + 4);
                    help.AppendLine($"{valuePadding}Default: {def.DefaultValue}");
                }
            }

            help.AppendLine();
            help.AppendLine("EXAMPLES:");
            help.AppendLine($"  {programName} --role=admin --verbose");
            help.AppendLine($"  {programName} -r admin -v -o output.json");
            help.AppendLine($"  {programName} --batch --input=recipes.json --format=csv");
            help.AppendLine($"  {programName} --help");

            return help.ToString();
        }

        /// <summary>
        /// Get display name for an argument (combines short and long forms)
        /// </summary>
        private string GetArgumentDisplayName(ArgumentDefinition def)
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrEmpty(def.ShortForm))
                parts.Add(def.ShortForm);
            
            if (!string.IsNullOrEmpty(def.LongForm))
                parts.Add(def.LongForm);

            var name = string.Join(", ", parts);

            if (def.Type == ArgumentType.Option)
                name += "=VALUE";

            return name;
        }

        /// <summary>
        /// Generate version information
        /// </summary>
        public string GenerateVersionInfo()
        {
            return "Recipe Ingredient Catalogue v2.0.0\n" +
                   "Advanced CLI with multi-user support\n" +
                   "Built with .NET 8.0\n" +
                   "Copyright (c) 2024";
        }

        /// <summary>
        /// Run comprehensive tests for the command line parser
        /// </summary>
        public static void RunTests()
        {
            Console.WriteLine("Running CommandLineParser tests...");

            var parser = new CommandLineParser();

            // Test 1: Basic flag parsing
            var args1 = new[] { "--verbose", "--help" };
            System.Diagnostics.Debug.Assert(parser.Parse(args1), "Should parse basic flags");
            System.Diagnostics.Debug.Assert(parser.HasArgument("verbose"), "Should detect verbose flag");
            System.Diagnostics.Debug.Assert(parser.HasArgument("help"), "Should detect help flag");

            // Test 2: Short form flags
            parser = new CommandLineParser();
            var args2 = new[] { "-vh" };
            System.Diagnostics.Debug.Assert(parser.Parse(args2), "Should parse combined short flags");
            System.Diagnostics.Debug.Assert(parser.HasArgument("verbose"), "Should detect verbose from combined flags");
            System.Diagnostics.Debug.Assert(parser.HasArgument("help"), "Should detect help from combined flags");

            // Test 3: Options with values
            parser = new CommandLineParser();
            var args3 = new[] { "--output=test.json", "-r", "admin" };
            System.Diagnostics.Debug.Assert(parser.Parse(args3), "Should parse options with values");
            System.Diagnostics.Debug.Assert(parser.GetArgumentValue("output") == "test.json", "Should get correct output value");
            System.Diagnostics.Debug.Assert(parser.GetArgumentValue("role") == "admin", "Should get correct role value");

            // Test 4: Invalid values
            parser = new CommandLineParser();
            var args4 = new[] { "--format=invalid" };
            System.Diagnostics.Debug.Assert(!parser.Parse(args4), "Should fail on invalid format value");
            System.Diagnostics.Debug.Assert(parser.GetErrors().Count > 0, "Should have validation errors");

            // Test 5: Help generation
            var help = parser.GenerateHelp();
            System.Diagnostics.Debug.Assert(help.Contains("Usage:"), "Help should contain usage information");
            System.Diagnostics.Debug.Assert(help.Contains("--verbose"), "Help should list verbose option");

            Console.WriteLine("CommandLineParser tests completed successfully.");
        }
    }
}
