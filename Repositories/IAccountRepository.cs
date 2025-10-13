using melodia.Entities;
using melodia_api.Models.Account;

namespace melodia_api.Repositories
{
    public interface IAccountRepository
    {
        public Task<List<AccountViewDto>> GetAllAccounts();
        public Task<Account> GetAccountById(string id);
        public Task<Account> GetAccountByEmail(string email);
        public Task<Account> PutAccountPassword(string id, string newPassword);
    }
}
