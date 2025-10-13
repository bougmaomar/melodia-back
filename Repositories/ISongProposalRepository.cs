using melodia_api.Entities;

namespace melodia_api.Repositories;

public interface ISongProposalRepository
{
    public Task<bool> ProposeSongsToRadioStation(long artistId, long songId, long radioStationId);
    public Task<List<Proposal>> GetAllProposalsByArtist(long artistId);
    public Task<int[]> GetMensualProposalsByArtist(long artistId);
    public Task<int[]> GetAnnualProposalsByArtist(long artistId);
    public Task<int> GetAllAcceptedProposalsCount();
    public Task<int> GetAllProposalsCount();
    public Task<int> GetProposalsStatistics();

}