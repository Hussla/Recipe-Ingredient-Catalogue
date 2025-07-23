using System;
using System.Security.Cryptography;
using System.Text;

// Class representing a User
[Serializable]
public class User
{
    // Properties with getters and setters
    public Guid Id { get; } = Guid.NewGuid(); // Unique identifier for the user
    public string Username { get; set; } // Stores the username
    public string PasswordHash { get; } // Stores the hashed password
    public string Role { get; set; } = "User"; // Role for access control (default to User)

    // Constructor
    // Initializes a new instance of the User class with the specified username and password
    public User(string username, string password)
    {
        Username = username;
        PasswordHash = HashPassword(password);
    }

    // Method to hash the password using SHA256
    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            // Convert byte array to string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Method to verify the password against the stored hash
    public bool VerifyPassword(string password)
    {
        string hashOfInput = HashPassword(password);
        return PasswordHash.Equals(hashOfInput);
    }
}

/*
Usage:
The User class can be instantiated with a username and password for registration.
The VerifyPassword() method can be used to authenticate users during login.
The Id property provides unique user identification for data association.
The Role property enables basic access control functionality.
*/


