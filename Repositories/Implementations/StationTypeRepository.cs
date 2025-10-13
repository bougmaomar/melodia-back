using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class StationTypeRepository : IStationTypeRepository
{
    private readonly MelodiaDbContext _db;

    public StationTypeRepository(MelodiaDbContext db) { _db = db; }
    
    public async Task<StationType> CreateStationType(StationType stationType)
    {
        _db.StationTypes.Add(stationType);
        await _db.SaveChangesAsync();
        return stationType;
    }

    public async Task<StationType> FindStationTypeById(long id)
    {
        var stationType = await _db.StationTypes.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id && at.Active);
        if (stationType == null)
            throw new EntityNotFoundException(nameof(StationType), nameof(id), id.ToString());
        return stationType;
    }

    public Task<List<StationType>> GetAllStationTypes()
    {
        return _db.StationTypes.ToListAsync();
    }

    public async Task<StationType> UpdateStationType(StationType stationType)
    {
        if (!_db.StationTypes.Any(at => at.Id == stationType.Id && at.Active))
            throw new EntityNotFoundException(nameof(StationType), nameof(StationType.Id),
                stationType.Id.ToString());
        _db.StationTypes.Update(stationType);
        await _db.SaveChangesAsync();
        return stationType;
    }
    
    public async Task<bool> DesactivateStationType(long stationTypeId)
    {
        var stationType = await _db.StationTypes
            .Include(g => g.RadioStations)
            .FirstOrDefaultAsync(g => g.Id == stationTypeId);
        if (stationType == null) return false;

        stationType.Active = false;
        foreach (var radioStation in stationType.RadioStations)
        {
            radioStation.Active = false;
        }

        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ActivateStationType(long stationTypeId)
    {
        var stationType = await _db.StationTypes
            .Include(g => g.RadioStations)
            .FirstOrDefaultAsync(g => g.Id == stationTypeId);
        if (stationType == null) return false;

        stationType.Active = true;
        foreach (var radioStation in stationType.RadioStations)
        {
            radioStation.Active = true;
        }

        await _db.SaveChangesAsync();
        return true;
    }
        
    public async Task<bool> DeleteStationType(long stationTypeId)
    {
        var stationType = await _db.StationTypes
            .Include(g => g.RadioStations)
            .FirstOrDefaultAsync(g => g.Id == stationTypeId);
        if (stationType == null) return false;

        _db.Stations.RemoveRange(stationType.RadioStations);
        _db.StationTypes.Remove(stationType);

        await _db.SaveChangesAsync();
        return true;
    }
}