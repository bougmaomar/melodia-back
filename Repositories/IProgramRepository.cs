namespace melodia_api.Repositories;
using melodia.Entities;

public interface IProgramRepository
{
    public Task<Programme> CreateProgramme(Programme programme);
    public Task<Programme> FindProgrammeById(long id);
    public Task<Programme> UpdateProgramme(Programme programme);
    public Task<List<Programme>> GetAllProgrammes();
    
    public Task<List<Programme>> GetProgrammesByStationId(long stationId);
    
    public Task<bool> DesactivateProgramme(long programmeId);
    public Task<bool> ActivateProgramme(long programmeId);

    public Task<bool> DeleteProgramme(long programmeId);
}