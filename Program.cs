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

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MelodiaDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Qdrant configuration
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

// Django configuration
builder.Services.Configure<DjangoSetting>(
    builder.Configuration.GetSection("Django"));

// Kestrel configuration for large file uploads
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000; // 100 MB
});

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repository registrations
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
builder.Services.AddScoped<IWriterRepository, WriterRepository>();
builder.Services.AddScoped<IComposerRepository, ComposerRepository>();
builder.Services.AddScoped<ICROwnerRepository, IcrOwnerRepository>();
builder.Services.AddScoped<IPOwnerRepository, POwnerRepository>();
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

// HTTP clients
builder.Services.AddHttpClient<GeoDataService>();
builder.Services.AddScoped<GeoDataService>();
builder.Services.AddHttpClient<ISongRepository, SongRepository>();

// Logging
builder.Services.AddLogging();

// CORS configuration - Azure friendly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000", "http://localhost:4200" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Identity configuration
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

// JWT Authentication
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database seeding (non-blocking)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Attempting database connection...");

        var dbContext = services.GetRequiredService<MelodiaDbContext>();

        // Test connection without crashing the app
        if (await dbContext.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connected successfully");

            // Uncomment if you want to run migrations automatically
            // await dbContext.Database.MigrateAsync();

            MelodiaDbInitializer.Seed(services);
            logger.LogInformation("Database seeding completed");

            var geoDataService = services.GetRequiredService<GeoDataService>();
            await geoDataService.ImportGeographicalData();
            logger.LogInformation("Geographical data import completed");
        }
        else
        {
            logger.LogWarning("Cannot connect to database - skipping initialization");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization: {Message}", ex.Message);
        // Don't throw - let the app start even if seeding fails
    }
}

// Development environment configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// CORS must come first
app.UseCors("AllowOrigin");

// Default static files (wwwroot)
app.UseStaticFiles();

// Configure upload directories safely
var uploadsBasePath = Path.Combine(app.Environment.ContentRootPath, "uploads");

try
{
    var imagesPath = Path.Combine(uploadsBasePath, "images");
    var audioPath = Path.Combine(uploadsBasePath, "audio");

    // Create directories if they don't exist
    Directory.CreateDirectory(imagesPath);
    Directory.CreateDirectory(audioPath);

    // Configure static file serving for uploads
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagesPath),
        RequestPath = "/images"
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(audioPath),
        RequestPath = "/audio"
    });

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Upload directories configured successfully at {Path}", uploadsBasePath);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to configure upload directories. File uploads may not work.");
}

// HTTPS redirection
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.Run();
