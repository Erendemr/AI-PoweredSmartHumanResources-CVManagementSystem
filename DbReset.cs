using InsanK.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InsanK
{
    public class DbReset
    {
        public static async Task ResetDatabaseAsync(IServiceProvider serviceProvider, ILogger<DbReset> logger)
        {
            try
            {
                logger.LogInformation("Veritabanı sıfırlama işlemi başlatılıyor...");

                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await dbContext.Database.EnsureDeletedAsync();
                logger.LogInformation("Veritabanı silindi.");

                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Veritabanı migrasyonları uygulandı.");

                logger.LogInformation("Veritabanı başarıyla sıfırlandı!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Veritabanı sıfırlanırken hata oluştu!");
                throw;
            }
        }
    }
}
