using InsanK.Data;
using InsanK.DTOs;
using InsanK.Helpers;
using InsanK.Models;
using Microsoft.EntityFrameworkCore;

namespace InsanK.Services
{
    public class KullaniciService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public KullaniciService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<(Kullanici? kullanici, string token)> Giris(KullaniciGirisDTO model)
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi);

            if (kullanici == null || kullanici.Sifre != model.Sifre)
            {
                return (null, string.Empty);
            }

            var token = _jwtService.GenerateJwtToken(kullanici);
            return (kullanici, token);
        }

        public async Task<(Kullanici? kullanici, string token)> Kayit(KullaniciKayitDTO model)
        {
            var kullaniciVar = await _context.Kullanicilar
                .AnyAsync(k => k.KullaniciAdi == model.KullaniciAdi || k.Email == model.Email);

            if (kullaniciVar)
            {
                return (null, string.Empty);
            }

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = model.KullaniciAdi,
                Email = model.Email,
                Sifre = model.Sifre,
                Ad = model.Ad,
                Soyad = model.Soyad,
                Telefon = model.Telefon,
                KullaniciTipi = model.KullaniciTipi,
                SirketAdi = model.SirketAdi,
                SirketAciklamasi = model.SirketAciklamasi,
                KayitTarihi = DateTime.Now
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateJwtToken(yeniKullanici);
            return (yeniKullanici, token);
        }

        public async Task<KullaniciProfilDTO?> ProfilGetir(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                return null;
            }

            return new KullaniciProfilDTO
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.KullaniciAdi,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Telefon = kullanici.Telefon,
                IsAdmin = kullanici.IsAdmin,
                KullaniciTipi = kullanici.KullaniciTipi,
                SirketAdi = kullanici.SirketAdi,
                SirketAciklamasi = kullanici.SirketAciklamasi,
                KayitTarihi = kullanici.KayitTarihi
            };
        }

        public async Task<bool> ProfilGuncelle(int id, KullaniciGuncelleDTO model)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                return false;
            }

            if (model.Email != null && model.Email != kullanici.Email)
            {
                var emailVar = await _context.Kullanicilar
                    .AnyAsync(k => k.Email == model.Email && k.Id != id);

                if (emailVar)
                {
                    return false;
                }

                kullanici.Email = model.Email;
            }

            kullanici.Ad = model.Ad ?? kullanici.Ad;
            kullanici.Soyad = model.Soyad ?? kullanici.Soyad;
            kullanici.Telefon = model.Telefon ?? kullanici.Telefon;

            if (kullanici.KullaniciTipi == KullaniciTipi.IsVeren)
            {
                kullanici.SirketAdi = model.SirketAdi ?? kullanici.SirketAdi;
                kullanici.SirketAciklamasi = model.SirketAciklamasi ?? kullanici.SirketAciklamasi;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SifreGuncelle(int id, SifreGuncelleDTO model)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null || kullanici.Sifre != model.MevcutSifre)
            {
                return false;
            }

            kullanici.Sifre = model.YeniSifre;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<KullaniciProfilDTO>> KullanicilariListele()
        {
            return await _context.Kullanicilar
                .Select(k => new KullaniciProfilDTO
                {
                    Id = k.Id,
                    KullaniciAdi = k.KullaniciAdi,
                    Email = k.Email,
                    Ad = k.Ad,
                    Soyad = k.Soyad,
                    Telefon = k.Telefon,
                    IsAdmin = k.IsAdmin,
                    KullaniciTipi = k.KullaniciTipi,
                    SirketAdi = k.SirketAdi,
                    SirketAciklamasi = k.SirketAciklamasi,
                    KayitTarihi = k.KayitTarihi
                })
                .ToListAsync();
        }

        public async Task<List<KullaniciAramaDTO>> KullaniciAra(string aranan)
        {
            var arananLower = aranan.ToLower();

            return await _context.Kullanicilar
                .Where(k => k.KullaniciAdi.ToLower().Contains(arananLower) ||
                           (k.Ad != null && k.Ad.ToLower().Contains(arananLower)) ||
                           (k.Soyad != null && k.Soyad.ToLower().Contains(arananLower)) ||
                           (k.SirketAdi != null && k.SirketAdi.ToLower().Contains(arananLower)))
                .Select(k => new KullaniciAramaDTO
                {
                    Id = k.Id,
                    KullaniciAdi = k.KullaniciAdi,
                    Ad = k.Ad,
                    Soyad = k.Soyad,
                    KullaniciTipi = k.KullaniciTipi,
                    SirketAdi = k.SirketAdi
                })
                .Take(10)
                .ToListAsync();
        }
    }
}
