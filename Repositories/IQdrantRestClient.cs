using System.Collections.Generic;
using System.Threading.Tasks;

namespace melodia_api.Repositories
{
    public interface IQdrantRestClient
    {
        Task CreateCollectionAsync(int vectorSize = 4);
        Task<bool> CollectionExistsAsync();
        Task UpsertAsync(int id, float[] vector, object payload = null);
        Task<List<(int Id, float Score, string PayloadJson)>> SearchAsync(float[] queryVector, int limit = 5);
        Task<int> GetMaxIdAsync();
        Task DeletePointAsync(int id);

        Task<float> CompareVectorToPointAsync(float[] queryVector, int targetPointId);

    }
}