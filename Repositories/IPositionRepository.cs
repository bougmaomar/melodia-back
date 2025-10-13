using melodia.Entities;

namespace melodia_api.Repositories
{
    public interface IPositionRepository
    {
        public Task<Position> FindPositionById(long id);
        public Task<Position> CreatePosition(Position position);
        public Task<Position> UpdatePosition(Position position);
        public Task<List<Position>> GetAllPositions();
        public Task<bool> DesactivatePosition(long positionId);
        public Task<bool> ActivatePosition(long positionId);
        
        public Task<bool> DeletePosition(long positionId);
    }
}
