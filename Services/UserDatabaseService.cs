using System.Security.Cryptography;
using System.Text;
using Lopputoo.Models;
using SQLite;

namespace Lopputoo.Services
{
    public class UserDatabaseService
    {
        private const string DatabaseFileName = "garden_defender.db3";
        private SQLiteAsyncConnection? database;

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (database is not null)
            {
                return database;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
            database = new SQLiteAsyncConnection(databasePath);
            await database.CreateTableAsync<UserAccount>();

            return database;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string confirmPassword)
        {
            var cleanUsername = username.Trim();

            if (string.IsNullOrWhiteSpace(cleanUsername))
            {
                return (false, "Username is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Password is required.");
            }

            if (password.Length < 4)
            {
                return (false, "Password must be at least 4 characters.");
            }

            if (password != confirmPassword)
            {
                return (false, "Passwords do not match.");
            }

            var databaseConnection = await GetDatabaseAsync();
            var existingUser = await databaseConnection.Table<UserAccount>()
                .Where(user => user.Username == cleanUsername)
                .FirstOrDefaultAsync();

            if (existingUser is not null)
            {
                return (false, "That username already exists.");
            }

            var salt = CreateSalt();
            var userAccount = new UserAccount
            {
                Username = cleanUsername,
                PasswordSalt = salt,
                PasswordHash = CreatePasswordHash(password, salt),
                CreatedAt = DateTime.UtcNow
            };

            await databaseConnection.InsertAsync(userAccount);
            return (true, "Account created. You can log in now.");
        }

        public async Task<(bool Success, string Message, UserAccount? User)> LoginAsync(string username, string password)
        {
            var cleanUsername = username.Trim();

            if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Enter username and password.", null);
            }

            var databaseConnection = await GetDatabaseAsync();
            var userAccount = await databaseConnection.Table<UserAccount>()
                .Where(user => user.Username == cleanUsername)
                .FirstOrDefaultAsync();

            if (userAccount is null)
            {
                return (false, "User not found.", null);
            }

            var enteredPasswordHash = CreatePasswordHash(password, userAccount.PasswordSalt);
            if (enteredPasswordHash != userAccount.PasswordHash)
            {
                return (false, "Wrong password.", null);
            }

            return (true, $"Welcome, {userAccount.Username}!", userAccount);
        }

        private static string CreateSalt()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes);
        }

        private static string CreatePasswordHash(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes($"{salt}:{password}");
            var hashBytes = SHA256.HashData(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
