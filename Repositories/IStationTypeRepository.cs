using melodia.Entities;

namespace melodia_api.Repositories;

public interface IStationTypeRepository
{ 
    public Task<StationType> CreateStationType(StationType stationType);
    public Task<StationType> FindStationTypeById(long id);
    public Task<StationType> UpdateStationType(StationType stationType);
    public Task<List<StationType>> GetAllStationTypes();
    
    public Task<bool> DesactivateStationType(long stationTypeId);
    public Task<bool> ActivateStationType(long stationTypeId);
        
    public Task<bool> DeleteStationType(long stationTypeId);
}