using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Service class for user authentication and management
public static class AuthService
{
    // File path for storing user data
    private const string UsersFilePath = "users.json";
    
    // Currently logged in user
    public static User CurrentUser { get; private set; }
    
    // Collection of all registered users
    private static Dictionary<Guid, User> Users = new Dictionary<Guid, User>();
    
    // Static constructor to load existing users from file
    static AuthService()
    {
        LoadUsers();
    }
    
    // Register a new user
    public static bool Register(string username, string password)
    {
        return Register(username, password, "User");
    }

    // Register a new user with specified role
    public static bool Register(string username, string password, string role)
    {
        // Check if username is already taken
        if (Users.Values.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return false;
        }
        
        // Create new user
        User newUser = new User(username, password);
        newUser.Role = role;
        Users.Add(newUser.Id, newUser);
        SaveUsers();
        Console.WriteLine("Registration successful!");
        return true;
    }
    
    // Log in an existing user
    public static bool Login(string username, string password)
    {
        // Find user by username
        User user = Users.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        if (user != null && user.VerifyPassword(password))
        {
            CurrentUser = user;
            Console.WriteLine($"Login successful! Welcome, {user.Username}.");
            return true;
        }
        
        Console.WriteLine("Invalid username or password.");
        return false;
    }
    
    // Log out the current user
    public static void Logout()
    {
        CurrentUser = null;
        Console.WriteLine("Logged out successfully.");
    }
    
    // Check if a user is currently logged in
    public static bool IsUserLoggedIn()
    {
        return CurrentUser != null;
    }
    
    // Get the current user
    public static User GetCurrentUser()
    {
        return CurrentUser;
    }
    
    // Save users to file
    private static void SaveUsers()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(UsersFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving users: {ex.Message}");
        }
    }
    
    // Load users from file
    private static void LoadUsers()
    {
        try
        {
            if (File.Exists(UsersFilePath))
            {
                string json = File.ReadAllText(UsersFilePath);
                Users = JsonSerializer.Deserialize<Dictionary<Guid, User>>(json) ?? new Dictionary<Guid, User>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users: {ex.Message}");
            Users = new Dictionary<Guid, User>();
        }
    }
}

/*
Usage:
- Call AuthService.Register() to create new user accounts
- Use AuthService.Login() for user authentication
- Access AuthService.CurrentUser to get the logged in user
- User data is persisted to disk using binary serialization

Summary:
This authentication service provides user registration, login, and session management functionality with data persistence.
*/
