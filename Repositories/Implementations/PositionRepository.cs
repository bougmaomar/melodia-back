using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class PositionRepository : IPositionRepository
    {
        public readonly MelodiaDbContext _db;
        public PositionRepository(MelodiaDbContext db) {
            _db = db;
        }

        public async Task<Position> CreatePosition(Position position)
        {
            _db.Positions.Add(position);
            await _db.SaveChangesAsync();
            return position;
        }
        
        public async Task<Position> FindPositionById(long id)
        {
            var position = await _db.Positions.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id && at.Active);
            if (position == null) throw new EntityNotFoundException(nameof(Position), nameof(id), id.ToString());
            return position;
        }
        
        public Task<List<Position>> GetAllPositions()
        {
            return _db.Positions.ToListAsync();
        }
        
        public async Task<Position> UpdatePosition(Position position)
        {
            if (!_db.Positions.Any(at => at.Id == position.Id )) throw new EntityNotFoundException(nameof(Position), nameof(position.Id), position.Id.ToString());
            _db.Positions.Update(position);
            await _db.SaveChangesAsync();
            return position;
        }
        
        
        public async Task<bool> DesactivatePosition(long positionId)
        {
            var position = await _db.Positions
                .Include(g => g.Employees)
                .FirstOrDefaultAsync(g => g.Id == positionId);
            if (position == null) return false;

            position.Active = false;
            foreach (var employee in position.Employees)
            {
                position.Active = false;
            }

            await _db.SaveChangesAsync();
            return true;
        }

    
        public async Task<bool> ActivatePosition(long positionId)
        {
            var position = await _db.Positions
                .Include(g => g.Employees)
                .FirstOrDefaultAsync(g => g.Id == positionId);
            if (position == null) return false;

            position.Active = true;
            foreach (var employee in position.Employees)
            {
                employee.Active = true;
            }

            await _db.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeletePosition(long positionId)
        {
            var position = await _db.Positions
                .Include(g => g.Employees)
                .FirstOrDefaultAsync(g => g.Id == positionId);
            if (position == null) return false;

            _db.Employees.RemoveRange(position.Employees);
            _db.Positions.Remove(position);

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
