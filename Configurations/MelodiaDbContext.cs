using melodia_api.Entities;
using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace melodia.Configurations;

public class MelodiaDbContext : IdentityDbContext<
	Account, Role, string, IdentityUserClaim<string>,
	AccountRole,
	IdentityUserLogin<string>,
	IdentityRoleClaim<string>,
	IdentityUserToken<string>>
{
	public MelodiaDbContext(DbContextOptions<MelodiaDbContext> options) : base(options) { }
	
	public DbSet<MigrationHistory> MigrationHistories { get; set; }
	public DbSet<Access> Accesses { get; set; }
	public DbSet<Section> Sections { get; set; }
	public DbSet<SectionIncluded> SectionIncludeds { get; set; }
	public DbSet<Account> Accounts { get; set; }
	public DbSet<Artist> Artists { get; set; }
	public DbSet<Agent> Agents { get; set; }
	public DbSet<RadioStation> Stations { get; set; }
	public DbSet<Song> Songs { get; set; }
	public DbSet<SongArtist> SongArtists { get; set; }
	public DbSet<ArtistAgent> ArtistAgent {  get; set; }
	public DbSet<StationType> StationTypes { get; set; }
	public DbSet<Position> Positions { get; set; }
	public DbSet<MusicFormat> MusicFormats { get; set; }
	public DbSet<Language> Languages { get; set; }
	public DbSet<GenreMusic> GenreMusics { get; set; }
	public DbSet<ProgramType> ProgramTypes { get; set; }
	public DbSet<Country> Countries { get; set; }
	public DbSet<City> Cities { get; set; }
	public DbSet<Discussion> Discussions { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<POwner> POwners { get; set; }
	public DbSet<SongPOwner> SongPOwners { get; set; }
	public DbSet<Writer> Writers { get; set; }
	public DbSet<SongWriter> SongWriters { get; set; }
	public DbSet<Composer> Composers { get; set; }
	public DbSet<SongComposer> SongComposers { get; set; }
	public DbSet<CROwner> Owners { get; set; }
	public DbSet<SongCROwner> SongCrOwners { get; set; }
	public DbSet<AlbumType> AlbumTypes { get; set; }
	public DbSet<Album> Albums { get; set; }
	public DbSet<AlbumArtist> AlbumArtists { get; set; }
	public DbSet<FavoriteSongs> FavoriteSongs { get; set; }
	public DbSet<Proposal> Proposals { get; set; }
	public DbSet<Programme> Programmes { get; set; }
	public DbSet<Employee> Employees { get; set; }
	public DbSet<Download> Downloads { get; set; }
	public DbSet<Play> Plays { get; set; }
	public DbSet<Visit> Visits { get; set; }
	public DbSet<SongVisit> SongVisits { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

        //foreach (var entity in modelBuilder.Model.GetEntityTypes())
        //{
        //    modelBuilder.Entity(entity.ClrType).ToTable(entity.ClrType.Name);

        //    foreach (var property in entity.GetProperties())
        //    {
        //        property.SetColumnName(property.Name);
        //    }
        //}

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.ClrType.Name);

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name);
            }
        }

		modelBuilder.Entity<Access>().ToTable("accesses");
		modelBuilder.Entity<AccountRole>().ToTable("accountroles");
		modelBuilder.Entity<Account>().ToTable("accounts");
		modelBuilder.Entity<Agent>().ToTable("agents");
		modelBuilder.Entity<Album>().ToTable("albums");
		modelBuilder.Entity<AlbumArtist>().ToTable("albumartists");
		modelBuilder.Entity<AlbumType>().ToTable("albumtypes");
		modelBuilder.Entity<Artist>().ToTable("artists");
		modelBuilder.Entity<ArtistAgent>().ToTable("artistagents");
		modelBuilder.Entity<City>().ToTable("cities");
		modelBuilder.Entity<Composer>().ToTable("composers");
		modelBuilder.Entity<Country>().ToTable("countries");
		modelBuilder.Entity<CROwner>().ToTable("crowners");
		modelBuilder.Entity<Discussion>().ToTable("discussions");
		modelBuilder.Entity<Download>().ToTable("downloads");
		modelBuilder.Entity<Employee>().ToTable("employees");
		modelBuilder.Entity<FavoriteSongs>().ToTable("favoritesongs");
		modelBuilder.Entity<GenreMusic>().ToTable("genremusics");
		modelBuilder.Entity<Language>().ToTable("languages");
		modelBuilder.Entity<Message>().ToTable("messages");
		modelBuilder.Entity<MigrationHistory>().ToTable("migrationhistories");
		modelBuilder.Entity<MusicFormat>().ToTable("musicformats");
		modelBuilder.Entity<Plan>().ToTable("plans");
		modelBuilder.Entity<Play>().ToTable("plays");
		modelBuilder.Entity<Position>().ToTable("positions");
		modelBuilder.Entity<POwner>().ToTable("powners");
		modelBuilder.Entity<Programme>().ToTable("programmes");
		modelBuilder.Entity<ProgramType>().ToTable("programtypes");
		modelBuilder.Entity<Proposal>().ToTable("proposals");
		modelBuilder.Entity<RadioStation>().ToTable("radiostations");
		modelBuilder.Entity<RadioStationLanguage>().ToTable("radiostationlanguages");
		modelBuilder.Entity<RadioStationMusicFormat>().ToTable("radiostationmusicformats");
		modelBuilder.Entity<Role>().ToTable("roles");
		modelBuilder.Entity<Section>().ToTable("sections");
		modelBuilder.Entity<SectionIncluded>().ToTable("sectionincluded");
		modelBuilder.Entity<Song>().ToTable("songs");
		modelBuilder.Entity<SongArtist>().ToTable("songartists");
		modelBuilder.Entity<SongComposer>().ToTable("songcomposers");
		modelBuilder.Entity<SongCROwner>().ToTable("songcrowners");
		modelBuilder.Entity<SongPOwner>().ToTable("songpowners");
		modelBuilder.Entity<SongWriter>().ToTable("songwriters");
		modelBuilder.Entity<StationType>().ToTable("stationtypes");
		modelBuilder.Entity<Visit>().ToTable("visits");
		modelBuilder.Entity<SongVisit>().ToTable("songvisits");
		modelBuilder.Entity<Writer>().ToTable("writers");
		modelBuilder.Entity<SongLanguages>().ToTable("songlanguages");



        base.OnModelCreating(modelBuilder);


		modelBuilder.Entity<Account>()
			.HasKey(a => a.Id);


		modelBuilder.Entity<Agent>()
			.HasOne(a => a.Account)
			.WithOne(b => b.Agent)
			.HasForeignKey<Account>(b => b.AgentId);

		modelBuilder.Entity<Artist>()
			.HasOne(a => a.Account)
			.WithOne(b => b.Artist)
			.HasForeignKey<Account>(b => b.ArtistId);


		modelBuilder.Entity<RadioStation>()
			.HasOne(a => a.Account)
			.WithOne(b => b.RadioStation)
			.HasForeignKey<Account>(b => b.RadioStationId);

		modelBuilder.Entity<Account>(a =>
		{
			a.HasMany(e => e.AccountRoles)
				.WithOne(e => e.Account)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			a.ToTable("accounts");
		});
        modelBuilder.Entity<AccountRole>(ar => ar.ToTable("accountroles"));
        modelBuilder.Entity<Account>()
		.Property(a => a.AgentId)
		.IsRequired(false);
		modelBuilder.Entity<Account>()
		.Property(a => a.ArtistId)
		.IsRequired(false);
		modelBuilder.Entity<Account>()
		.Property(a => a.RadioStationId)
		.IsRequired(false);

		modelBuilder.Entity<AlbumArtist>()
			.HasKey(aa => new { aa.AlbumId, aa.ArtistId });

		modelBuilder.Entity<AlbumArtist>()
			.HasOne(aa => aa.Album)
			.WithMany(a => a.AlbumArtists)
			.HasForeignKey(aa => aa.AlbumId);

		modelBuilder.Entity<AlbumArtist>()
			.HasOne(aa => aa.Artist)
			.WithMany(a => a.AlbumArtists)
			.HasForeignKey(aa => aa.ArtistId);
		
		modelBuilder.Entity<FavoriteSongs>()
			.HasKey(aa => new { aa.UserId, aa.SongId });

		modelBuilder.Entity<FavoriteSongs>()
			.HasOne(aa => aa.Song)
			.WithMany(a => a.FavoriteSongs)
			.HasForeignKey(aa => aa.SongId);

		modelBuilder.Entity<FavoriteSongs>()
			.HasOne(aa => aa.Account)
			.WithMany(a => a.FavoriteSongs)
			.HasForeignKey(aa => aa.UserId);

		modelBuilder.Entity<ArtistAgent>()
			.HasKey(aa => new { aa.AgentId, aa.ArtistId });

		modelBuilder.Entity<ArtistAgent>()
			.HasOne(aa => aa.Agent)
			.WithMany(a => a.ArtistAgents) 
			.HasForeignKey(aa => aa.AgentId);

		modelBuilder.Entity<ArtistAgent>()
			.HasOne(aa => aa.Artist)
			.WithMany(a => a.ArtistAgents)
			.HasForeignKey(aa => aa.ArtistId);

		modelBuilder.Entity<Account>(a =>
        {
            // Each User can have many entries in the UserRole join table
            a.HasMany(e => e.AccountRoles)
                .WithOne(e => e.Account)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            a.ToTable("accounts");
        });
		modelBuilder.Entity<RadioStationLanguage>()
			.HasKey(a => new { a.LanguageId, a.RadioStationId});

        modelBuilder.Entity<RadioStationLanguage>()
            .HasOne(aa => aa.Language)
            .WithMany(a => a.StationLanguages)
            .HasForeignKey(aa => aa.LanguageId);

        modelBuilder.Entity<RadioStationLanguage>()
            .HasOne(aa => aa.RadioStation)
            .WithMany(a => a.StationLanguages)
            .HasForeignKey(aa => aa.RadioStationId);

		modelBuilder.Entity<RadioStationMusicFormat>()
			.HasKey(a => new { a.MusicFormatId, a.RadioStationId});

        modelBuilder.Entity<RadioStationMusicFormat>()
            .HasOne(aa => aa.MusicFormat)
            .WithMany(a => a.StationMusicFormats)
            .HasForeignKey(aa => aa.MusicFormatId);

        modelBuilder.Entity<RadioStationMusicFormat>()
            .HasOne(aa => aa.RadioStation)
            .WithMany(a => a.StationMusicFormats)
            .HasForeignKey(aa => aa.RadioStationId);

        modelBuilder.Entity<Role>(r =>
        {
            // Each Role can have many entries in the UserRole join table
            r.HasMany(e => e.AccountRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            r.ToTable("roles");
        });
        
        
        modelBuilder.Entity<Access>(entity =>
        {
	        entity.HasKey(a => a.Id);

	        entity.HasOne(a => a.Role)
		        .WithMany(r => r.Accesses)
		        .HasForeignKey(a => a.RoleId)
		        .IsRequired();

	        entity.HasOne(a => a.Section)
		        .WithMany()
		        .HasForeignKey(a => a.SectionId)
		        .IsRequired();
        });
        
        modelBuilder.Entity<Section>(entity =>
        {
	        entity.HasKey(s => s.Id);
	        entity.Property(s => s.Label).IsRequired();
	        entity.HasOne(s => s.ParentSection)
		        .WithMany(s => s.SubSections)
		        .HasForeignKey(s => s.ParentSectionId)
		        .IsRequired(false);
        });
        
        modelBuilder.Entity<GenreMusic>()
	        .HasMany(g => g.Songs)
	        .WithOne(s => s.GenreMusic)
	        .HasForeignKey(s => s.GenreMusicId); 
        
        modelBuilder.Entity<Country>()
	        .HasMany(c => c.Cities)
	        .WithOne(c => c.Country)
	        .HasForeignKey(c => c.CountryId);
        
        
        modelBuilder.Entity<Proposal>()
	        .HasOne(p => p.Artist)
	        .WithMany(a => a.Proposals)
	        .HasForeignKey(p => p.ArtistId);

        modelBuilder.Entity<Proposal>()
	        .HasOne(p => p.Song)
	        .WithMany(s => s.Proposals)
	        .HasForeignKey(p => p.SongId);

        modelBuilder.Entity<Proposal>()
	        .HasOne(p => p.RadioStation)
	        .WithMany(rs => rs.Proposals)
	        .HasForeignKey(p => p.RadioStationId);

    }

}