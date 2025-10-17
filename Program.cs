using System.Text;
using melodia_api.Controllers;
using melodia_api.Repositories;
using melodia_api.Repositories.Implementations;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Microsoft.Extensions.FileProviders;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Text.Json.Serialization;
using melodia_api.Chat;
using melodia_api.Services.Implementations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<QdrantSettings>(
    builder.Configuration.GetSection("Qdrant"));

builder.Services.AddSingleton<IQdrantRestClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<QdrantSettings>>().Value;
    return new QdrantRestClient(
        baseUrl: settings.BaseUrl,
        apiKey: settings.ApiKey,
        collectionName: settings.CollectionName
    );
});



builder.Services.Configure<DjangoSetting>(
    builder.Configuration.GetSection("Django"));

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000; // 100 Mo
});

builder.Services.AddSignalR();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IArtistAccountRepository, ArtistAccountRepository>();
builder.Services.AddScoped<IAgentAccountRepository, AgentAccountRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IGenreMusicRepository, GenreMusicRepository>();
builder.Services.AddScoped<IMusicFormatRepository, MusicFormatRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IStationTypeRepository, StationTypeRepository>();
builder.Services.AddScoped<IAudioFeatureExtractor, AudioFeatureExtractor>();
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IWriterRepository,  WriterRepository>();
builder.Services.AddScoped<IComposerRepository,  ComposerRepository>();
builder.Services.AddScoped<ICROwnerRepository,  IcrOwnerRepository>();
builder.Services.AddScoped<IPOwnerRepository,  POwnerRepository>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IStationAccountRepository, StationAccountRepository>();
builder.Services.AddScoped<ISongComposerRepository, SongComposerRepository>();
builder.Services.AddScoped<ISongCROwnerRepository, SongCROwnerRepository>();
builder.Services.AddScoped<ISongWriterRepository, SongWriterRepository>();
builder.Services.AddScoped<ISongPOwnerRepository, SongPOwnerRepository>();
builder.Services.AddScoped<IAccessRepository, AccessRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<IProgramTypeRepository, ProgramTypeRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<SongProposalRepository>();
builder.Services.AddHttpClient<GeoDataService>();
builder.Services.AddScoped<GeoDataService>();
builder.Services.AddHttpClient<ISongRepository, SongRepository>();
builder.Services.AddLogging();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.WithOrigins("*") // Allow multiple origins
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services
    .AddIdentity<Account, Role>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<MelodiaDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        MelodiaDbInitializer.Seed(services);

        var geoDataService = services.GetRequiredService<GeoDataService>();
        await geoDataService.ImportGeographicalData();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowOrigin");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
});




app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "uploads/images")),
    RequestPath = "/images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "uploads/audio")),
    RequestPath = "/audio"
});


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();
