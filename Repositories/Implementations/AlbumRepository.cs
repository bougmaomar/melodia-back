using DocumentFormat.OpenXml.Office2010.Excel;
using melodia.Configurations;
using melodia.Entities;
using melodia_api.Models.Album;
using melodia_api.Models.Song;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly MelodiaDbContext _db;
        private readonly string _fileDirectory;

        public AlbumRepository(MelodiaDbContext db, IConfiguration configuration) { _db = db; _fileDirectory = configuration["FileStorage:Directory"]; }
        
        public async Task<List<Album>> FilterAlbums(string title = null, TimeSpan? minTotalDuration = null, TimeSpan? maxTotalDuration = null, long? albumTypeId = null)
        {
            var query = _db.Albums
                .Include(a => a.AlbumType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(a => a.Title.Contains(title));
            }

            if (minTotalDuration.HasValue)
            {
                query = query.Where(a => a.TotalDuration >= minTotalDuration.Value);
            }

            if (maxTotalDuration.HasValue)
            {
                query = query.Where(a => a.TotalDuration <= maxTotalDuration.Value);
            }

            if (albumTypeId.HasValue)
            {
                query = query.Where(a => a.AlbumTypeId == albumTypeId.Value);
            }

            return await query.ToListAsync();
        }
        
        public async Task<Album> CreateAlbum(AlbumCreateDto albumCreateDto)
        {
            var existingAlbum = await _db.Albums.FirstOrDefaultAsync(a => a.Title == albumCreateDto.Title);
            if (existingAlbum != null) throw new Exception($"Album name '{albumCreateDto.Title}' is already taken.");

            Album album = new Album()
            {
                Title = albumCreateDto.Title,
                Active = true,
                Description = albumCreateDto.Description,
                ReleaseDate = albumCreateDto.ReleaseDate,
                AlbumTypeId = albumCreateDto.AlbumTypeId,
            };

            if (albumCreateDto.CoverImage != null)
            {
                album.CoverImage = await UploadFileAsync(albumCreateDto.CoverImage, "images");
            }
            _db.Albums.Add(album);
            await _db.SaveChangesAsync();

            if (albumCreateDto.ArtistIds != null)
            {
                foreach (var artistId in albumCreateDto.ArtistIds)
                {
                    var albumArtist = new AlbumArtist
                    {
                        AlbumId = album.Id,
                        ArtistId = artistId,
                        
                    };
                    _db.AlbumArtists.Add(albumArtist);
                }
                await _db.SaveChangesAsync();
            }
            var createdAlbum = await _db.Albums
                .Include(s => s.Songs)
                .Include(s => s.AlbumType)
                .Include(s => s.AlbumArtists)
                    .ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(s => s.Id == album.Id);
            return createdAlbum;
        }

        private async Task<string> UploadFileAsync(IFormFile file, string subdirectory)
        {
            var uploadDirectory = Path.Combine(_fileDirectory, subdirectory);

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Use forward slashes in the file path
            var relativeFilePath = Path.GetRelativePath(_fileDirectory, filePath).Replace("\\", "/");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return relativeFilePath;
        }
        
        public async Task<Album> UpdateAlbum(Album updatedAlbum, IFormFile newCoverImage)
        {
            var album = await _db.Albums.FindAsync(updatedAlbum.Id);
            if (album == null)
            {
                throw new Exception("Album not found");
            }
            if (newCoverImage != null)
            {
                if (!string.IsNullOrEmpty(album.CoverImage))
                {
                    DeleteFile(album.CoverImage);
                }
                album.CoverImage = await UploadFileAsync(newCoverImage, "images");
            }
            

            _db.Entry(album).CurrentValues.SetValues(updatedAlbum);

           
            await _db.SaveChangesAsync();
            return album;
        }

        private void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_fileDirectory, filePath);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task DeactivateAlbumById(long id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album != null)
            {
                album.Active = false;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ActivateAlbumById(long id)
        {
            var album = await _db.Albums.FindAsync(id);
            if (album != null)
            {
                album.Active = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Album>> GetAllAlbums()
        {
            return await _db.Albums
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .Include(s => s.AlbumType)
                .Include(s => s.Songs)
                .Where(s => s.Active)
                .ToListAsync();
        }
        public async Task<List<Album>> GetAlbumByArtist(long artistId)
        {
            return await _db.Albums
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .Include(s => s.AlbumType)
                .Include(s => s.Songs)
                .Where(s => s.AlbumArtists.Any(aa => aa.ArtistId == artistId))
                .Where(s => s.Active)
                .ToListAsync();

        }
        public async Task<List<AlbumType>> GetAlbumTypes()
        {
            return await _db.AlbumTypes
                .ToListAsync();
        }
        public async Task<List<Album>> GetAlbumsByTypes(long typeId)
        {
            return await _db.Albums
                .Where(s => s.AlbumTypeId == typeId)
                .Include(s => s.AlbumType)
                .ToListAsync();
        }


        public async Task<Album> FindAlbumById(long id)
        {
            var album = await _db.Albums
                .Include(s => s.Songs)
                .Include(s => s.AlbumType)
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

           

            TimeSpan totalDuration = album.Songs
                .Where(s => !string.IsNullOrEmpty(s.Duration))
                .Select(s => TimeSpan.Parse(s.Duration))   
                .Aggregate(TimeSpan.Zero, (sum, duration) => sum.Add(duration));

            album.TotalDuration = totalDuration;

            return album;

        }

        public async Task<List<Album>> GetRelatedAlbums(long albumId)
        {
            var artistIds = await _db.Albums
           .Where(s => s.Id == albumId)
           .SelectMany(s => s.AlbumArtists.Select(sa => sa.ArtistId))
           .ToListAsync();

            var relatedAlbums = await _db.Albums
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .ThenInclude(sa => sa.Account)
                .Include(s => s.Songs)
                .Include(s => s.AlbumType)
                .Where(s => s.Active == true && s.Id != albumId && s.AlbumArtists.Any(sa => artistIds.Contains(sa.ArtistId)))
                .ToListAsync();

            return relatedAlbums;
        }

        public async Task<Album> GetAlbumById(long id)
        {
            return await _db.Albums
                .Include(s => s.AlbumType)
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
