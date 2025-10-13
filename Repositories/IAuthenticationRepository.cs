using melodia.Entities;
using melodia_api.Models;
using melodia_api.Models.Account;

namespace melodia_api.Repositories;

public interface IAuthenticationRepository
{
    public Task<Tokens> Authenticate(string login, string password);
    public Task<Tokens> Reauthenticate(Tokens tokens);

}