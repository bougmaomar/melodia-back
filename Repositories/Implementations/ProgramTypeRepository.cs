using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class ProgramTypeRepository : IProgramTypeRepository
{
  private readonly MelodiaDbContext _db;

    public ProgramTypeRepository(MelodiaDbContext db) { _db = db; }
    
    public async Task<ProgramType> CreateProgramType(ProgramType programType)
    {
        _db.ProgramTypes.Add(programType);
        await _db.SaveChangesAsync();
        return programType;
    }

    public async Task<ProgramType> FindProgramTypeById(long id)
    {
        var programType = await _db.ProgramTypes.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id);
        if (programType == null)
            throw new EntityNotFoundException(nameof(ProgramType), nameof(id), id.ToString());
        return programType;
    }

    public Task<List<ProgramType>> GetAllProgramTypes()
    {
        return _db.ProgramTypes.ToListAsync();
    }

    public async Task<ProgramType> UpdateProgramType(ProgramType programType)
    {
        if (!_db.ProgramTypes.Any(at => at.Id == programType.Id && at.Active)) throw new EntityNotFoundException(nameof(ProgramType), nameof(ProgramType.Id),
                programType.Id.ToString());
        _db.ProgramTypes.Update(programType);
        await _db.SaveChangesAsync();
        return programType;
    }
    
    public async Task<bool> DesactivateProgramType(long programTypeId)
    {
        var programType = await _db.ProgramTypes
            .Include(g => g.Programmes)
            .FirstOrDefaultAsync(g => g.Id == programTypeId);
        if (programType == null) return false;

        programType.Active = false;
        foreach (var programees in programType.Programmes)
        {
            programees.Active = false;
        }

        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ActivateProgramType(long programTypeId)
    {
        var programType = await _db.ProgramTypes
            .Include(g => g.Programmes)
            .FirstOrDefaultAsync(g => g.Id == programTypeId);
        if (programType == null) return false;

        programType.Active = true;
        foreach (var programme in programType.Programmes)
        {
            programme.Active = true;
        }

        await _db.SaveChangesAsync();
        return true;
    }
        
    public async Task<bool> DeleteProgramType(long stationTypeId)
    {
        var programType = await _db.ProgramTypes
            .Include(g => g.Programmes)
            .FirstOrDefaultAsync(g => g.Id == stationTypeId);
        if (programType == null) return false;

        _db.Programmes.RemoveRange(programType.Programmes);
        _db.ProgramTypes.Remove(programType);

        await _db.SaveChangesAsync();
        return true;
    }
}