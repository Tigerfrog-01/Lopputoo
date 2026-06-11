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
                return (false, LocalizationService.Get("UsernameRequired"));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, LocalizationService.Get("PasswordRequired"));
            }

            if (password.Length < 4)
            {
                return (false, LocalizationService.Get("PasswordTooShort"));
            }

            if (password != confirmPassword)
            {
                return (false, LocalizationService.Get("PasswordsDoNotMatch"));
            }

            var databaseConnection = await GetDatabaseAsync();
            var existingUser = await databaseConnection.Table<UserAccount>()
                .Where(user => user.Username == cleanUsername)
                .FirstOrDefaultAsync();

            if (existingUser is not null)
            {
                return (false, LocalizationService.Get("ThatUsernameExists"));
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
            return (true, LocalizationService.Get("AccountCreated"));
        }

        public async Task<(bool Success, string Message, UserAccount? User)> LoginAsync(string username, string password)
        {
            var cleanUsername = username.Trim();

            if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(password))
            {
                return (false, LocalizationService.Get("EnterUsernamePassword"), null);
            }

            var databaseConnection = await GetDatabaseAsync();
            var userAccount = await databaseConnection.Table<UserAccount>()
                .Where(user => user.Username == cleanUsername)
                .FirstOrDefaultAsync();

            if (userAccount is null)
            {
                return (false, LocalizationService.Get("UserNotFound"), null);
            }

            var enteredPasswordHash = CreatePasswordHash(password, userAccount.PasswordSalt);
            if (enteredPasswordHash != userAccount.PasswordHash)
            {
                return (false, LocalizationService.Get("WrongPassword"), null);
            }

            return (true, LocalizationService.Format("WelcomeFormat", userAccount.Username), userAccount);
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
