using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using melodia_api.Models.AgentAccount;
using melodia_api.Models.Song;
namespace melodia_api.Repositories;

public interface IAgentAccountRepository
{
    public Task<List<AgentAccountViewDto>> GetAllAgentAccounts();
    public Task<List<Song>> GetAllSongByAgent(long id);
    public Task<List<Album>> GetAllAlbumsByAgent(long id);
    public Task<Agent> CreateAgentAndAccount(AgentAccountCreateDto accountCreateDto);
    public Task<Agent> AcceptAgentAccount(long id);
    public Task<Agent> RejectAgentAccount(long id);
    public Task<AgentAccountViewDto> UpdateAgentAndAccountAsync(AgentAccountUpdateDto agentAccountUpdate);
    public Task<IdentityResult> ChangePassword(string email, string oldPassword, string newPassword);
    public Task<AgentAccountViewDto> GetAgentAccountById(long id);
    public Task<AgentAccountViewDto> GetAgentAccountByEmail(string email);
    public Task<bool> DeactivateAgentAccountById(string accountId);
    public Task<bool> ActivateAgentAccountById(string accountId);
}