using melodia_api.Entities;
using melodia.Entities;
using melodia_api.Models.StationAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace melodia_api.Repositories
{
    public interface IStationAccountRepository
    {
        public Task<List<StationAccountViewDto>> GetAllStationAccounts();
        public Task<RadioStation> CreateStationAndAccount(StationAccountCreateDto accountCreateDto);

        public Task<StationAccountViewDto> UpdateStationAndAccountAsync(StationAccountUpdateDto stationAccountUpdate);
        public Task<IdentityResult> ChangePassword(string email, string oldPassword, string newPassword);
        public Task<Account> GetStationAccountById(string accountId);
        public Task<StationAccountViewDto> GetStationAccountByEmail(string email);
        public Task<bool> DeactivateStationAccountById(string accountId);
        public Task<bool> ActivateStationAccountById(string accountId);
        public Task<bool> SendToAccepted(Artist artist, Song song, RadioStation radioStation);

        public Task<RadioStation> GetRadioStationById(long id);
        public Task<bool> AcceptSong(long radioStationId, long songId);
        public Task<bool> RejectSong(long radioStationId, long songId);
        public Task<List<Proposal>> GetProposalsByRadioStationIdAsync(long radioStationId, ProposalStatus? status = null);
        public Task<List<Proposal>> GetAllAcceptedProposalsByArtist(long artistId);
        public Task<List<Proposal>> GetAllAcceptedProposalsByAgent(long agentId);

        public Task<string> GetTopStation();
        public Task<List<Proposal>> GetAcceptedProposalsByRadioStation(long radioStationId); 
        public Task<bool> RadioStationExists(long radioStationId);
        
        public Task<RadioStation> GetRadioStationByAccountId(string accountId);
        
        public Task<RadioStation> AcceptStationAccount(long id);
        public Task<RadioStation> RejectStationAccount(long id);
    }
}
