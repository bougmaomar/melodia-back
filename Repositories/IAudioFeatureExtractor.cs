using System.Threading.Tasks;

namespace melodia_api.Repositories
{
    public interface IAudioFeatureExtractor
    {
        Task<float[]> ExtractVectorAsync(string filePath);
    }
}