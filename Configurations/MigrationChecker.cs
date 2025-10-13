using melodia_api.Entities;

namespace melodia.Configurations;

public static class MigrationChecker
{
    public static bool HasMigrationBeenApplied(MelodiaDbContext context, string migrationId)
    {
        return context.MigrationHistories.Any(m => m.MigrationId == migrationId);
    }

    public static void MarkMigrationAsApplied(MelodiaDbContext context, string migrationId)
    {
        var migrationHistory = new MigrationHistory
        {
            MigrationId = migrationId,
            AppliedOn = DateTime.UtcNow
        };
        context.MigrationHistories.Add(migrationHistory);
        context.SaveChanges();
    }
}

