using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using melodia_api.Models.ArtistAccount;
using melodia_api.Entities;
namespace melodia_api.Repositories;

public interface IArtistAccountRepository
{
    public Task<List<ArtistAccountViewDto>> GetAllArtistAccounts();
    public Task<Artist> CreateArtistAndAccount(ArtistAccountCreateDto accountCreateDto);
    public Task<Artist> CreateArtistByAgent(ArtistCreateByAgentDto accountCreateByAgentDto);
    public Task<Artist> UpdateArtistByAgent(ArtistUpdateByAgentDto accountUpdateByAgentDto);
    public Task<ArtistAccountViewDto> UpdateArtistAndAccountAsync(ArtistAccountUpdateDto artistAccountUpdate);
    public Task<IdentityResult> ChangePassword(string email, string oldPassword, string newPassword);
    public Task<ArtistAccountViewDto> GetArtistAccountById(long id);
    public Task<ArtistAccountViewDto> GetArtistAccountByEmail(string email);
    public Task<List<ArtistAccountViewDto>> GetArtistsByAgent(long id);
    public Task<bool> DeactivateArtistAccountById(string accountId);
    public Task<bool> ActivateArtistAccountById(string accountId);
    public Task<Artist> GetArtistById(long id);
    public Task<bool> SendProposal(Artist artist, Song song, RadioStation radioStation, string description);
    public Task RecordVisitAsync(long artistId, long radioStationId);
    public Task<List<Visit>> GetAllVisitsByArtists(long artistId);
    public Task<int[]> GetMensualVisitsByArtist(long artistId);
    public Task<int[]> GetAnnualVisitsByArtist(long artistId);

    public Task<List<ArtistPlayComparaison>> GetArtistComparison(long artistId, long comparedId);
}