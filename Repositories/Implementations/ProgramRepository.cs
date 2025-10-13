using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class ProgramRepository : IProgramRepository
{
    private readonly MelodiaDbContext _db;
    
    public ProgramRepository(MelodiaDbContext db) { _db = db; }
    
    public async Task<Programme> CreateProgramme(Programme programme)
    {
        _db.Programmes.Add(programme);
        await _db.SaveChangesAsync();
        return programme;
    }
    
    public async Task<Programme> FindProgrammeById(long id)
    {
        var programme = await _db.Programmes.SingleOrDefaultAsync(programme => programme.Id == id);
        if (programme == null) throw new EntityNotFoundException(nameof(Programme), nameof(id), id.ToString());
        return programme;
    }
    
    public async Task<Programme> UpdateProgramme(Programme programme)
    {
        if (!_db.Programmes.Any(t => t.Id == programme.Id))
            throw new EntityNotFoundException(nameof(Programme), nameof(programme.Id), programme.Id.ToString());
        var existedProgramme = await _db.Programmes.FirstOrDefaultAsync(p => p.Id == programme.Id);
        programme.RadioStationId = existedProgramme.RadioStationId;
        _db.ChangeTracker.Clear();
        _db.Programmes.Update(programme);
        await _db.SaveChangesAsync();
        return programme;
    }
    
    public Task<List<Programme>> GetAllProgrammes()
    {
        return _db.Programmes.Where(programme => programme.Active).ToListAsync();
    }

    public Task<List<Programme>> GetProgrammesByStationId(long stationId)
    {
        return _db.Programmes.Where(programme => programme.RadioStationId == stationId).ToListAsync();
    }
    
    public async Task<bool> DeleteProgramme(long programmeId)
    {
        var programme = await _db.Programmes.FindAsync(programmeId);
        if (programme == null) return false;
        
        _db.Programmes.Remove(programme);
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DesactivateProgramme(long programmeId)
    {
        var programme = await _db.Programmes.FirstOrDefaultAsync(g => g.Id == programmeId);
        if (programme == null) return false;

        programme.Active = false;
        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ActivateProgramme(long programmeId)
    {
        var programme = await _db.Programmes
            .FirstOrDefaultAsync(g => g.Id == programmeId);
        if (programme == null) return false;

        programme.Active = true;
        await _db.SaveChangesAsync();
        return true;
    }
}