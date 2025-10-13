using melodia.Entities;

namespace melodia_api.Repositories;

public interface IProgramTypeRepository
{
    public Task<ProgramType> CreateProgramType(ProgramType programType);
    public Task<ProgramType> FindProgramTypeById(long id);
    public Task<ProgramType> UpdateProgramType(ProgramType programType);
    public Task<List<ProgramType>> GetAllProgramTypes();
    public Task<bool> DesactivateProgramType(long programTypeId);
    public Task<bool> ActivateProgramType(long programTypeId);
    public Task<bool> DeleteProgramType(long programTypeId);
}