using AutoMapper;
using melodia_api.Entities;
using melodia_api.Models;
using melodia_api.Models.Access;
using melodia.Entities;
using melodia_api.Models.StationType;
using melodia_api.Models.ArtistAccount;
using melodia_api.Models.AgentAccount;
using melodia_api.Models.GenreMusic;
using melodia_api.Models.Language;
using melodia_api.Models.Song;
using melodia_api.Models.SongWriter;
using melodia_api.Models.SongComposer;
using melodia_api.Models.SongCROwner;
using melodia_api.Models.Album;
using melodia_api.Models.Position;
using melodia_api.Models.Role;
using melodia_api.Models.POwner;
using melodia_api.Models.ProgramType;
using melodia_api.Models.Section;
using melodia_api.Models.SongPOwner;
using melodia_api.Models.SongProposals;
using melodia_api.Models.StationAccount;

namespace melodia.Configurations;

public class AutoMapperInitializer : Profile
{
	public AutoMapperInitializer()
	{
		CreateMap<City, CityViewDto>()
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
            .ReverseMap();
		CreateMap<Country, CountryViewDto>().ReverseMap();
		CreateMap<Proposal, SongProposalsView>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.ArtistId, opt => opt.MapFrom(src => src.ArtistId))
			.ForMember(dest => dest.SongId, opt => opt.MapFrom(src => src.SongId))
			.ForMember(dest => dest.RadioStationId, opt => opt.MapFrom(src => src.RadioStationId))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
			.ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.FirstName + " " + src.Artist.LastName))
			.ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.Title))
			.ForMember(dest => dest.RadioStationName, opt => opt.MapFrom(src => src.RadioStation.StationName));
		CreateMappingsForStationTypes();
		CreateMappingsForProgramTypes();
		CreateMappingsForLanguages();
		CreateMappingsForGenreMusics();
		CreateMappingsForWriters();
        CreateMappingsForComposers();
        CreateMappingsForCROwners();
        CreateMappingsForPOwners();
		CreateMappingsForAlbums();
		CreateMappingsForSongs();
		CreateMappingsForSongComposers();
		CreateMappingsForSongCrOwners();
		CreateMappingsForSongPOwners();
		CreateMappingsForRoles();
		CreateMappingsForAgents();
        CreateMappingsForSongWritters();
		CreateMap<Artist, ArtistDto>().ReverseMap();
		CreateMap<Agent, AgentDto>().ReverseMap();
		CreateMappingForAccess();
		CreateMappingForSections();
		CreateMappingForPositions();
		CreateMappingsForArtists();

	}


	private void CreateMappingsForArtists()
	{
		CreateMap<Artist, ArtistAccountViewDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.ArtistRealName, opt => opt.MapFrom(src => src.FirstName+ ' ' + src.LastName))
			.ForMember(dest => dest.CareerStartDate, opt => opt.MapFrom(src => src.CareerStartDate))
			.ReverseMap();
	}
	private void CreateMappingsForProgramTypes()
	{
		CreateMap<ProgramType, ProgramTypeCreateDto>().ReverseMap();
		CreateMap<ProgramType, ProgramTypeUpdateDto>().ReverseMap();
		CreateMap<ProgramType, ProgramTypeViewDto>().ReverseMap();
	}
	
	private void CreateMappingsForStationTypes()
	{
		CreateMap<StationType, StationTypeCreateDto>().ReverseMap();
		CreateMap<StationType, StationTypeUpdateDto>().ReverseMap();
		CreateMap<StationType, StationTypeViewDto>().ReverseMap();
		CreateMap<Account, ArtistAccountViewDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
			.ReverseMap();
        CreateMap<Account, StationAccountViewDto>();

		CreateMap<RadioStation, StationAccountViewDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
			.ReverseMap();

        CreateMap<Account, AgentAccountViewDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
			.ReverseMap();
		CreateMap<Agent, AgentAccountViewDto>()
			.ForMember(dest => dest.ArtistNames, opt => opt.MapFrom(src => src.ArtistAgents.Select(s => s.Artist.Name)))
			.ReverseMap();

    }

    private void CreateMappingsForSongs()
	{

		CreateMap<SongUpdateLessDto, Song>()
    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
    .ForMember(dest => dest.PlatformReleaseDate, opt => opt.MapFrom(src => src.PlatformReleaseDate))
    .ForMember(dest => dest.GenreMusicId, opt => opt.MapFrom(src => src.GenreMusicId))
    .ForMember(dest => dest.Lyrics, opt => opt.MapFrom(src => src.Lyrics))
    .ForMember(dest => dest.IsMapleMusic, opt => opt.MapFrom(src => src.IsMapleMusic))
    .ForMember(dest => dest.IsMapleArtist, opt => opt.MapFrom(src => src.IsMapleArtist))
    .ForMember(dest => dest.IsMaplePerformance, opt => opt.MapFrom(src => src.IsMaplePerformance))
    .ForMember(dest => dest.IsMapleLyrics, opt => opt.MapFrom(src => src.IsMapleLyrics))
    .ForMember(dest => dest.CodeISRC, opt => opt.MapFrom(src => src.CodeISRC))
    .ForMember(dest => dest.YouTube, opt => opt.MapFrom(src => src.YouTube))
    .ForMember(dest => dest.Spotify, opt => opt.MapFrom(src => src.Spotify))
    .ForMember(dest => dest.SongLanguages, opt => opt.MapFrom(src => src.LanguageIds.Select(id => new SongLanguages { LanguageId = id }).ToList()))
    .ForMember(dest => dest.SongWriters, opt => opt.MapFrom(src => src.WriterIds.Select(id => new SongWriter { WriterId = id }).ToList()))
    .ForMember(dest => dest.SongComposers, opt => opt.MapFrom(src => src.ComposerIds.Select(id => new SongComposer { ComposerId = id }).ToList()))
    .ForMember(dest => dest.SongCrOwners, opt => opt.MapFrom(src => src.CopyrightOwnerIds.Select(id => new SongCROwner { CROwnerId = id }).ToList()))
.ReverseMap();
		CreateMap<SongUpdateDto, SongUpdateLessDto>().ReverseMap();
		CreateMap<SongUpdateCoverLessDto, Song>()
			.ForMember(dest => dest.Mp3FilePath, opt => opt.Ignore())
    .ForMember(dest => dest.WavFilePath, opt => opt.Ignore())
    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
    .ForMember(dest => dest.PlatformReleaseDate, opt => opt.MapFrom(src => src.PlatformReleaseDate))
    .ForMember(dest => dest.GenreMusicId, opt => opt.MapFrom(src => src.GenreMusicId))
    .ForMember(dest => dest.Lyrics, opt => opt.MapFrom(src => src.Lyrics))
    .ForMember(dest => dest.IsMapleMusic, opt => opt.MapFrom(src => src.IsMapleMusic))
    .ForMember(dest => dest.IsMapleArtist, opt => opt.MapFrom(src => src.IsMapleArtist))
    .ForMember(dest => dest.IsMaplePerformance, opt => opt.MapFrom(src => src.IsMaplePerformance))
    .ForMember(dest => dest.IsMapleLyrics, opt => opt.MapFrom(src => src.IsMapleLyrics))
    .ForMember(dest => dest.CodeISRC, opt => opt.MapFrom(src => src.CodeISRC))
    .ForMember(dest => dest.YouTube, opt => opt.MapFrom(src => src.YouTube))
    .ForMember(dest => dest.Spotify, opt => opt.MapFrom(src => src.Spotify))
    .ForMember(dest => dest.SongLanguages, opt => opt.MapFrom(src => src.LanguageIds.Select(id => new SongLanguages { LanguageId = id }).ToList()))
    .ForMember(dest => dest.SongWriters, opt => opt.MapFrom(src => src.WriterIds.Select(id => new SongWriter { WriterId = id }).ToList()))
    .ForMember(dest => dest.SongComposers, opt => opt.MapFrom(src => src.ComposerIds.Select(id => new SongComposer { ComposerId = id }).ToList()))
    .ForMember(dest => dest.SongCrOwners, opt => opt.MapFrom(src => src.CopyrightOwnerIds.Select(id => new SongCROwner { CROwnerId = id }).ToList()))
.ReverseMap();
		CreateMap<SongUpdateDto, SongUpdateCoverLessDto>().ReverseMap();
		CreateMap<SongUpdateAudioLessDto, Song>()
    .ForMember(dest => dest.CoverImagePath, opt => opt.Ignore())
    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
    .ForMember(dest => dest.PlatformReleaseDate, opt => opt.MapFrom(src => src.PlatformReleaseDate))
    .ForMember(dest => dest.GenreMusicId, opt => opt.MapFrom(src => src.GenreMusicId))
    .ForMember(dest => dest.Lyrics, opt => opt.MapFrom(src => src.Lyrics))
    .ForMember(dest => dest.IsMapleMusic, opt => opt.MapFrom(src => src.IsMapleMusic))
    .ForMember(dest => dest.IsMapleArtist, opt => opt.MapFrom(src => src.IsMapleArtist))
    .ForMember(dest => dest.IsMaplePerformance, opt => opt.MapFrom(src => src.IsMaplePerformance))
    .ForMember(dest => dest.IsMapleLyrics, opt => opt.MapFrom(src => src.IsMapleLyrics))
    .ForMember(dest => dest.CodeISRC, opt => opt.MapFrom(src => src.CodeISRC))
    .ForMember(dest => dest.YouTube, opt => opt.MapFrom(src => src.YouTube))
    .ForMember(dest => dest.Spotify, opt => opt.MapFrom(src => src.Spotify))
    .ForMember(dest => dest.SongLanguages, opt => opt.MapFrom(src => src.LanguageIds.Select(id => new SongLanguages { LanguageId = id }).ToList()))
    .ForMember(dest => dest.SongWriters, opt => opt.MapFrom(src => src.WriterIds.Select(id => new SongWriter { WriterId = id }).ToList()))
    .ForMember(dest => dest.SongComposers, opt => opt.MapFrom(src => src.ComposerIds.Select(id => new SongComposer { ComposerId = id }).ToList()))
    .ForMember(dest => dest.SongCrOwners, opt => opt.MapFrom(src => src.CopyrightOwnerIds.Select(id => new SongCROwner { CROwnerId = id }).ToList()))
.ReverseMap();
		CreateMap<SongUpdateDto, SongUpdateAudioLessDto>().ReverseMap();
		CreateMap<Song, SongViewDto>()
			.ForMember(dest => dest.ArtistNames, opt => opt.MapFrom(src => src.SongArtists.Where(sa => sa.Active).Select(sa => sa.Artist.FirstName + ' ' + sa.Artist.LastName)))
			.ForMember(dest => dest.ArtistEmails, opt => opt.MapFrom(src => src.SongArtists.Where(sa => sa.Active).Select(sa => sa.Artist.Account != null ? sa.Artist.Account.Email : null)))
			.ForMember(dest => dest.LanguageLabels, opt => opt.MapFrom(src => src.SongLanguages.Where(sa => sa.Active).Select(sa => sa.Language.Label)))
			.ForMember(dest => dest.GenreMusicName, opt => opt.MapFrom(src => src.GenreMusic.Name))
			.ForMember(dest => dest.ComposersNames, opt => opt.MapFrom(src => src.SongComposers.Where(sc => sc.Active).Select(sc => sc.Composer.Name)))
			.ForMember(dest => dest.WritersNames, opt => opt.MapFrom(src => src.SongWriters.Where(sc => sc.Active).Select(sw => sw.Writer.Name)))
			.ForMember(dest => dest.CROwnersNames, opt => opt.MapFrom(src => src.SongCrOwners.Where(sc => sc.Active).Select(sc => sc.CrOwner.Name)))
			.ForMember(dest => dest.AlbumTitle, opt => opt.MapFrom(src => src.Album != null ? src.Album.Title : null));

		CreateMap<Song, SongView>()
            .ForMember(dest => dest.ArtistIds, opt => opt.MapFrom(src => src.SongArtists.Where(sa => sa.Active).Select(sa => sa.Artist.Id)))
            .ForMember(dest => dest.GenreMusicId, opt => opt.MapFrom(src => src.GenreMusic.Id))
			.ForMember(dest => dest.LanguageIds, opt => opt.MapFrom(src => src.SongLanguages.Where(sc => sc.Active).Select(sc => sc.Language.Id)))
            .ForMember(dest => dest.ComposersIds, opt => opt.MapFrom(src => src.SongComposers.Where(sc => sc.Active).Select(sc => sc.Composer.Id)))
            .ForMember(dest => dest.WritersIds, opt => opt.MapFrom(src => src.SongWriters.Where(sc => sc.Active).Select(sw => sw.Writer.Id)))
            .ForMember(dest => dest.CROwnersIds, opt => opt.MapFrom(src => src.SongCrOwners.Where(sc => sc.Active).Select(sc => sc.CrOwner.Id)))
            .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.Album.Id));

        CreateMap<SongCreateDto, Song>()
			.ForMember(dest => dest.Mp3FilePath, opt => opt.Ignore()) 
			.ForMember(dest => dest.WavFilePath, opt => opt.Ignore()) 
			.ForMember(dest => dest.CoverImagePath, opt => opt.Ignore());

        CreateMap<SongUpdateDto, Song>()
    .ForMember(dest => dest.Mp3FilePath, opt => opt.Ignore())
    .ForMember(dest => dest.WavFilePath, opt => opt.Ignore())
    .ForMember(dest => dest.CoverImagePath, opt => opt.Ignore())
    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
    .ForMember(dest => dest.PlatformReleaseDate, opt => opt.MapFrom(src => src.PlatformReleaseDate))
    .ForMember(dest => dest.GenreMusicId, opt => opt.MapFrom(src => src.GenreMusicId))
    .ForMember(dest => dest.Lyrics, opt => opt.MapFrom(src => src.Lyrics))
    .ForMember(dest => dest.IsMapleMusic, opt => opt.MapFrom(src => src.IsMapleMusic))
    .ForMember(dest => dest.IsMapleArtist, opt => opt.MapFrom(src => src.IsMapleArtist))
    .ForMember(dest => dest.IsMaplePerformance, opt => opt.MapFrom(src => src.IsMaplePerformance))
    .ForMember(dest => dest.IsMapleLyrics, opt => opt.MapFrom(src => src.IsMapleLyrics))
    .ForMember(dest => dest.CodeISRC, opt => opt.MapFrom(src => src.CodeISRC))
    .ForMember(dest => dest.YouTube, opt => opt.MapFrom(src => src.YouTube))
    .ForMember(dest => dest.Spotify, opt => opt.MapFrom(src => src.Spotify))
    .ForMember(dest => dest.SongLanguages, opt => opt.MapFrom(src => src.LanguageIds.Select(id => new SongLanguages { LanguageId = id }).ToList()))
    .ForMember(dest => dest.SongWriters, opt => opt.MapFrom(src => src.WriterIds.Select(id => new SongWriter { WriterId = id }).ToList()))
    .ForMember(dest => dest.SongComposers, opt => opt.MapFrom(src => src.ComposerIds.Select(id => new SongComposer { ComposerId = id }).ToList()))
    .ForMember(dest => dest.SongCrOwners, opt => opt.MapFrom(src => src.CopyrightOwnerIds.Select(id => new SongCROwner { CROwnerId = id }).ToList()));

    }

    private void CreateMappingsForGenreMusics()
	{
		CreateMap<GenreMusic, GenreMusicCreateDto>().ReverseMap();
		CreateMap<GenreMusic, GenreMusicUpdateDto>().ReverseMap();
		CreateMap<GenreMusic, GenreMusicViewDto>().ReverseMap();
	}
	private void CreateMappingsForWriters()
	{
		CreateMap<Writer, WriterViewDto>().ReverseMap();
		CreateMap<Writer, WriterCreateDto>().ReverseMap();
		CreateMap<Writer, WriterUpdateDto>().ReverseMap();
	}
	private void CreateMappingsForComposers()
	{
		CreateMap<Composer, ComposerViewDto>().ReverseMap();
		CreateMap<Composer, ComposerCreateDto>().ReverseMap();
		CreateMap<Composer, ComposerUpdateDto>().ReverseMap();
	}
	private void CreateMappingsForCROwners()
	{
		CreateMap<CROwner, CROwnerViewDto>().ReverseMap();
		CreateMap<CROwner, CROwnerCreateDto>().ReverseMap();
		CreateMap<CROwner, CROwnerUpdateDto>().ReverseMap(); 
		CreateMap<CROwnerCreateDto, CROwnerViewDto>();
    }
	
	private void CreateMappingsForPOwners()
	{
		CreateMap<POwner, POwnerViewDto>().ReverseMap();
		CreateMap<POwner, POwnerCreateDto>().ReverseMap();
		CreateMap<POwner, POwnerUpdateDto>().ReverseMap(); 
		CreateMap<POwnerCreateDto, POwnerViewDto>();
    }

    private void CreateMappingsForLanguages()
	{
		CreateMap<Language, LanguageViewDto>().ReverseMap();
		CreateMap<Language, LanguageCreateDto>().ReverseMap();
		CreateMap<Language, LanguageUpdateDto>().ReverseMap();
	}

	private void CreateMappingForPositions()
	{
		CreateMap<Position, PositionViewDto>().ReverseMap();
		CreateMap<Position, PositionCreateDto>().ReverseMap();
		CreateMap<Position, PositionUpdateDto>().ReverseMap();
	}
	private void CreateMappingsForRoles()
	{
		CreateMap<Role, RoleViewDto>().ReverseMap();
		CreateMap<Role, RoleCreateDto>().ReverseMap();
		CreateMap<Role, RoleUpdateDto>().ReverseMap();
	}

	private void CreateMappingsForAlbums()
	{
		CreateMap<Album, AlbumCreateDto>().ReverseMap();
		CreateMap<Album, AlbumUpdateDto>().ForMember(dest => dest.CoverImage, opt => opt.Ignore());
		CreateMap<Album, AlbumUpdateLessDto>().ReverseMap();
		CreateMap<Album, AlbumUpdateDto>().ReverseMap();
		CreateMap<AlbumUpdateDto, AlbumUpdateLessDto>().ReverseMap();
		CreateMap<AlbumCreateDto, Album>().ReverseMap();
		CreateMap<AlbumUpdateDto, Album>().ReverseMap();
		CreateMap<Album, AlbumViewDto>().ReverseMap();
		CreateMap<Album, AlbumViewDto>()
			.ForMember(dest => dest.AlbumTypeName, opt => opt.MapFrom(src => src.AlbumType.Name))
			.ForMember(dest => dest.ArtistNames, opt => opt.MapFrom(src => src.AlbumArtists.Select(sa => sa.Artist.Name)))
            .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs.ToList()));
        CreateMap<Album, AlbumView>()
            .ForMember(dest => dest.AlbumTypeId, opt => opt.MapFrom(src => src.AlbumType.Id))
            .ForMember(dest => dest.ArtistIds, opt => opt.MapFrom(src => src.AlbumArtists.Select(sa => sa.Artist.Id)));
    }

	private void CreateMappingsForAgents()
	{
		CreateMap<Agent, AgentAccountCreateDto>().ReverseMap();
		CreateMap<Agent, AgentAccountUpdateDto>().ForMember(dest => dest.PhotoProfile, opt => opt.Ignore());
		CreateMap<Agent, AgentAccountUpdateDto>().ReverseMap();
		CreateMap<AgentAccountCreateDto, Agent>().ReverseMap();
		CreateMap<AgentAccountUpdateDto, Agent>().ReverseMap();
		CreateMap<Agent, AgentAccountViewDto>().ReverseMap();
		CreateMap<Agent, AgentAccountViewDto>()
			.ForMember(dest => dest.AgentRealName, opt => opt.MapFrom(src => src.FirstName + src.LastName))
			.ForMember(dest => dest.ArtistNames, opt => opt.MapFrom(src => src.ArtistAgents.Select(sa => sa.Artist.Name)));
    }

    private void CreateMappingsForSongComposers()
	{
		CreateMap<SongComposer, SongComposerCreateDto>().ReverseMap();
		CreateMap<SongComposer, SongComposerUpdateDto>().ReverseMap();
		CreateMap<SongComposer, SongComposerViewDto>().ReverseMap();
		CreateMap<SongComposer, SongComposerViewDto>()
			.ForMember(dest => dest.ComposerName, opt => opt.MapFrom(src => src.Composer.Name))
			.ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.Title));

	}

	private void CreateMappingsForSongCrOwners()
	{
		CreateMap<SongCROwner, SongCROwnerCreateDto>().ReverseMap();
		CreateMap<SongCROwner, SongCROwnerUpdateDto>().ReverseMap();
		CreateMap<SongCROwner, SongCROwnerViewDto>().ReverseMap();
		CreateMap<SongCROwner, SongCROwnerViewDto>()
			.ForMember(dest => dest.CROwnerName, opt => opt.MapFrom(src => src.CrOwner.Name))
			.ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.Title));
	}

	private void CreateMappingsForSongPOwners()
	{
		CreateMap<SongPOwner, SongPOwnerCreateDto>().ReverseMap();
		CreateMap<SongPOwner, SongPOwnerUpdateDto>().ReverseMap();
		CreateMap<SongPOwner, SongPOwnerViewDto>().ReverseMap();
		CreateMap<SongPOwner, SongPOwnerViewDto>()
			.ForMember(dest => dest.POwnerName, opt => opt.MapFrom(src => src.POwner.Name))
			.ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.Title));
	}

	private void CreateMappingsForSongWritters()
	{
		CreateMap<SongWriter, SongWriterCreateDto>().ReverseMap();
		CreateMap<SongWriter, SongWriterUpdateDto>().ReverseMap();
		CreateMap<SongWriter, SongWriterViewDto>().ReverseMap();
		CreateMap<SongWriter, SongWriterViewDto>()
			.ForMember(dest => dest.WritterName, opt => opt.MapFrom(src => src.Writer.Name))
			.ForMember(dest => dest.SongName, opt => opt.MapFrom(src => src.Song.Title));
	}
	
	private void CreateMappingForAccess()
	{
		CreateMap<AccessCreateDto, Access>().ReverseMap();
		CreateMap<AccessUpdateDto, Access>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.RoleId, opt => opt.Ignore())
			.ForMember(dest => dest.SectionId, opt => opt.Ignore());
		CreateMap<AccessViewDto, Access>().ReverseMap();
	}
	
	private void CreateMappingForSections()
	{
		CreateMap<SectionCreateDto, Section>().ReverseMap();
		CreateMap<SectionUpdateDto, Section>().ReverseMap();
		CreateMap<SectionViewDto, Section>().ReverseMap();
	}
}