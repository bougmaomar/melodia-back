using melodia.Entities;

namespace melodia_api.Repositories
{
    public interface IMusicFormatRepository
    {

        public Task<MusicFormat> FindMusicFormatById(long id);
        public Task<MusicFormat> CreateMusicFormat(MusicFormat musicFormat);
        public Task<MusicFormat> UpdateMusicFormat(MusicFormat musicFormat);
        public Task<List<MusicFormat>> GetAllMusicFormats();
        public Task DesactivateMusicFormatById(long id);
    }
}
