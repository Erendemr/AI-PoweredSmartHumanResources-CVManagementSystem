using InsanK.Models;
using Microsoft.EntityFrameworkCore;

namespace InsanK.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=DefaultConnection");
            }
            

            optionsBuilder.UseSqlServer(options => 
            {
                options.CommandTimeout(120);
            });
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<CV> CVler { get; set; }
        public DbSet<CVOneri> CVOnerileri { get; set; }
        public DbSet<Ilan> Ilanlar { get; set; }
        public DbSet<Basvuru> Basvurular { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>()
                .HasOne(k => k.CV)
                .WithOne(c => c.Kullanici)
                .HasForeignKey<CV>(c => c.KullaniciId);

            modelBuilder.Entity<Kullanici>()
                .HasMany(k => k.Ilanlar)
                .WithOne(i => i.Kullanici)
                .HasForeignKey(i => i.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kullanici>()
                .HasMany(k => k.Basvurular)
                .WithOne(b => b.Kullanici)
                .HasForeignKey(b => b.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.GonderenKullanici)
                .WithMany(k => k.GonderilenMesajlar)
                .HasForeignKey(m => m.GonderenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.AliciKullanici)
                .WithMany(k => k.AlinanMesajlar)
                .HasForeignKey(m => m.AliciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ilan>()
                .HasMany(i => i.Basvurular)
                .WithOne(b => b.Ilan)
                .HasForeignKey(b => b.IlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CV>()
                .HasMany(c => c.Oneriler)
                .WithOne(o => o.CV)
                .HasForeignKey(o => o.CVId);

            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici
                {
                    Id = 1,
                    KullaniciAdi = "eren",
                    Email = "admin@insankaynaklari.com",
                    Sifre = "123",
                    Ad = "Eren",
                    Soyad = "Admin",
                    KayitTarihi = DateTime.Now,
                    IsAdmin = true,
                    KullaniciTipi = KullaniciTipi.IsVeren
                }
            );
        }
    }
}