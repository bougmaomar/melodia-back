using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia.Configurations;

public abstract class MelodiaDbInitializer
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var context = new MelodiaDbContext(serviceProvider.GetRequiredService<DbContextOptions<MelodiaDbContext>>());
        context.Database.EnsureCreated();

        context.Database.ExecuteSqlRaw(File.ReadAllText("Data/InitialData.sql"));
        
        InitializeAdminUserAndRole(serviceProvider).Wait();
        InitializeRadioStationUserAndRole(serviceProvider).Wait();
    }

    private static async Task InitializeAdminUserAndRole(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var adminRole = new Role
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            await roleManager.CreateAsync(adminRole);
        }

        var adminEmail = "admin@melodia.com";
        var adminName = "admin";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new Account
            {
                UserName = adminName,
                Email = adminEmail,
                EmailConfirmed = true,
                LastLogin = DateTime.UtcNow,
                Active = true,
                IsApproved = true,
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            var result = await userManager.CreateAsync(newAdmin, "Admin.123@");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
            else
            {
                throw new Exception("Failed to create default admin user");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

private static async Task InitializeRadioStationUserAndRole(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    var dbContext = serviceProvider.GetRequiredService<MelodiaDbContext>();

    // Ensure the role exists
    if (!await roleManager.RoleExistsAsync("Station"))
    {
        var radioStationRole = new Role
        {
            Name = "Station",
            NormalizedName = "STATION"
        };
        await roleManager.CreateAsync(radioStationRole);
    }

    // Ensure the station type exists
    var defaultStationType = await dbContext.StationTypes.FirstOrDefaultAsync(st => st.Name == "Default");
    if (defaultStationType == null)
    {
        defaultStationType = new StationType
        {
            Name = "Default"
        };
        dbContext.StationTypes.Add(defaultStationType);
        await dbContext.SaveChangesAsync();
    }

    var radioStationEmail = "radio@station.com";
        var radioStationName = "radiostation";
    var radioStationUser = await userManager.FindByEmailAsync(radioStationEmail);

    if (radioStationUser == null)
    {
        var newRadioStationUser = new Account
        {
            UserName = radioStationName,
            Email = radioStationEmail,
            EmailConfirmed = true,
            LastLogin = DateTime.UtcNow,
            Active = true,
            IsApproved = true,
            RefreshToken = Guid.NewGuid().ToString(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        var result = await userManager.CreateAsync(newRadioStationUser, "Radio.123@");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newRadioStationUser, "Station");

            var newRadioStation = new RadioStation
            {
                StationName = "Default Radio Station",
                Frequency = "101.1 FM",
                StationOwner = "Default Owner",
                PhoneNumber = "0507989805",
                FoundationDate = DateTime.UtcNow,
                Account = newRadioStationUser,
                StationType = defaultStationType,
                Active = true
            };

            dbContext.Stations.Add(newRadioStation);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Failed to create default radio station user");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(radioStationUser, "Station"))
        {
            await userManager.AddToRoleAsync(radioStationUser, "Station");
        }

        var existingRadioStation = await dbContext.Stations
            .FirstOrDefaultAsync(rs => rs.Account.Id == radioStationUser.Id);

        if (existingRadioStation == null)
        {
            var newRadioStation = new RadioStation
            {
                StationName = "Default Radio Station",
                Frequency = "101.1 FM",
                StationOwner = "Default Owner",
                PhoneNumber = "0507898905",
                FoundationDate = DateTime.UtcNow,
                Account = radioStationUser,
                StationType = defaultStationType,
                Active = true
            };

            dbContext.Stations.Add(newRadioStation);
            await dbContext.SaveChangesAsync();
        }
    }
}

}

