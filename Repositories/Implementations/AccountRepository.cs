using DocumentFormat.OpenXml.Office2010.Excel;
using System.Text;
using melodia.Configurations;
using melodia.Entities;
using melodia_api.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        public readonly MelodiaDbContext _db;
        public readonly UserManager<Account> _userManager;
        public AccountRepository(MelodiaDbContext db, UserManager<Account> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<AccountViewDto>> GetAllAccounts()
        {
            var accounts = await _db.Accounts
           .Where(x => x.Active)
           .Select(a => new AccountViewDto
           {
               Id = a.Id,
               Email = a.Email,
               Active = a.Active,
               RefreshToken = a.RefreshToken,
               AgentId = a.AgentId,
               ArtistId = a.ArtistId,
               RadioStationId  = a.RadioStationId,
               UserName = a.UserName,
               PasswordHash = a.PasswordHash,
               PhoneNumber = a.PhoneNumber
           })
           .ToListAsync();

            return accounts;
        }
        public async Task<Account> GetAccountById(string id)
        {
            var account = await _userManager.FindByIdAsync(id);
            return account;
        }
        public async Task<Account> GetAccountByEmail(string email)
        {
            var account = await _userManager.FindByEmailAsync(email);
            return account;
        }

        public async Task<Account> PutAccountPassword(string id, string newPassword)
        {
            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(account);
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Failed to generate password reset token");
            }
            
            // Validation du mot de passe
            var passwordValidationResult = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, account, newPassword);
            if (!passwordValidationResult.Succeeded)
            {
                throw new Exception("Password validation failed: " + string.Join(", ", passwordValidationResult.Errors.Select(e => e.Description)));
            }

            // Réinitialisation du mot de passe
            var resetResult = await _userManager.ResetPasswordAsync(account, token, newPassword);
            if (!resetResult.Succeeded)
            {
                throw new Exception("Password reset failed: " + string.Join(", ", resetResult.Errors.Select(e => e.Description)));
            }

            // (Optionnel) Logger ou retourner le mot de passe si nécessaire
            Console.WriteLine($"Nouveau mot de passe généré pour {id}: {newPassword}");

            return account;
        }

    }
}
