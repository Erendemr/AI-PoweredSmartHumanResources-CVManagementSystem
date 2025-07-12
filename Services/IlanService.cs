using InsanK.Data;
using InsanK.DTOs;
using InsanK.Models;
using Microsoft.EntityFrameworkCore;

namespace InsanK.Services
{
    public class IlanService
    {
        private readonly ApplicationDbContext _context;

        public IlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IlanDetayDTO?> IlanOlustur(int kullaniciId, IlanOlusturDTO model)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null || kullanici.KullaniciTipi != KullaniciTipi.IsVeren)
            {
                return null;
            }

            var ilan = new Ilan
            {
                KullaniciId = kullaniciId,
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Lokasyon = model.Lokasyon,
                PozisyonTipi = model.PozisyonTipi,
                DepartmanAdi = model.DepartmanAdi,
                TecrubeSeviyesi = model.TecrubeSeviyesi,
                GerekliYetkinlikler = model.GerekliYetkinlikler,
                TercihEdilenYetkinlikler = model.TercihEdilenYetkinlikler,
                MinMaas = model.MinMaas,
                MaxMaas = model.MaxMaas,
                YayinlanmaTarihi = DateTime.Now,
                SonBasvuruTarihi = model.SonBasvuruTarihi,
                AktifMi = true,
                EgitimSeviyesi = model.EgitimSeviyesi,
                SirketLokasyonu = model.SirketLokasyonu
            };

            _context.Ilanlar.Add(ilan);
            await _context.SaveChangesAsync();

            return await IlanDetayGetir(ilan.Id, kullaniciId);
        }

        public async Task<IlanDetayDTO?> IlanGuncelle(int id, int kullaniciId, IlanGuncelleDTO model)
        {
            var ilan = await _context.Ilanlar
                .FirstOrDefaultAsync(i => i.Id == id && (i.KullaniciId == kullaniciId || 
                                        _context.Kullanicilar.Any(k => k.Id == kullaniciId && k.IsAdmin)));

            if (ilan == null)
            {
                return null;
            }

            ilan.Baslik = model.Baslik;
            ilan.Aciklama = model.Aciklama;
            ilan.Lokasyon = model.Lokasyon;
            ilan.PozisyonTipi = model.PozisyonTipi;
            ilan.DepartmanAdi = model.DepartmanAdi;
            ilan.TecrubeSeviyesi = model.TecrubeSeviyesi;
            ilan.GerekliYetkinlikler = model.GerekliYetkinlikler;
            ilan.TercihEdilenYetkinlikler = model.TercihEdilenYetkinlikler;
            ilan.MinMaas = model.MinMaas;
            ilan.MaxMaas = model.MaxMaas;
            ilan.SonBasvuruTarihi = model.SonBasvuruTarihi;
            ilan.AktifMi = model.AktifMi;
            ilan.EgitimSeviyesi = model.EgitimSeviyesi;
            ilan.SirketLokasyonu = model.SirketLokasyonu;

            await _context.SaveChangesAsync();
            return await IlanDetayGetir(id, kullaniciId);
        }

        public async Task<IlanDetayDTO?> IlanDetayGetir(int id, int kullaniciId = 0)
        {
            var ilan = await _context.Ilanlar
                .Include(i => i.Kullanici)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ilan == null)
            {
                return null;
            }

            bool basvurulmusMu = false;
            if (kullaniciId > 0)
            {
                basvurulmusMu = await _context.Basvurular
                    .AnyAsync(b => b.IlanId == id && b.KullaniciId == kullaniciId);
            }

            return new IlanDetayDTO
            {
                Id = ilan.Id,
                KullaniciId = ilan.KullaniciId,
                SirketAdi = ilan.Kullanici?.SirketAdi ?? "",
                Baslik = !string.IsNullOrEmpty(ilan.Baslik) ? ilan.Baslik : "İsimsiz İlan",
                Aciklama = ilan.Aciklama ?? "",
                Lokasyon = ilan.Lokasyon ?? "",
                PozisyonTipi = ilan.PozisyonTipi ?? "",
                DepartmanAdi = ilan.DepartmanAdi ?? "",
                TecrubeSeviyesi = ilan.TecrubeSeviyesi ?? "",
                GerekliYetkinlikler = ilan.GerekliYetkinlikler ?? "",
                TercihEdilenYetkinlikler = ilan.TercihEdilenYetkinlikler ?? "",
                MinMaas = ilan.MinMaas,
                MaxMaas = ilan.MaxMaas,
                YayinlanmaTarihi = ilan.YayinlanmaTarihi,
                SonBasvuruTarihi = ilan.SonBasvuruTarihi,
                AktifMi = ilan.AktifMi,
                EgitimSeviyesi = ilan.EgitimSeviyesi ?? "",
                SirketLokasyonu = ilan.SirketLokasyonu ?? "",
                BasvurulmusMu = basvurulmusMu
            };
        }

        public async Task<List<IlanListeDTO>> IlanlariListele(string? arama = null, string? lokasyon = null, string? pozisyonTipi = null)
        {
            var query = _context.Ilanlar
                .Include(i => i.Kullanici)
                .Include(i => i.Basvurular)
                .Where(i => i.AktifMi);

            if (!string.IsNullOrEmpty(arama))
            {
                query = query.Where(i => i.Baslik.Contains(arama) || 
                                      i.Aciklama.Contains(arama) || 
                                      i.GerekliYetkinlikler.Contains(arama) ||
                                      i.TercihEdilenYetkinlikler.Contains(arama));
            }

            if (!string.IsNullOrEmpty(lokasyon))
            {
                query = query.Where(i => i.Lokasyon.Contains(lokasyon));
            }

            if (!string.IsNullOrEmpty(pozisyonTipi))
            {
                query = query.Where(i => i.PozisyonTipi == pozisyonTipi);
            }

            return await query
                .OrderByDescending(i => i.YayinlanmaTarihi)
                .Select(i => new IlanListeDTO
                {
                    Id = i.Id,
                    SirketAdi = i.Kullanici.SirketAdi,
                    Baslik = i.Baslik,
                    Lokasyon = i.Lokasyon,
                    PozisyonTipi = i.PozisyonTipi,
                    TecrubeSeviyesi = i.TecrubeSeviyesi ?? "",
                    YayinlanmaTarihi = i.YayinlanmaTarihi,
                    BasvuruSayisi = i.Basvurular.Count,
                    AktifMi = i.AktifMi,
                    SonBasvuruTarihi = i.SonBasvuruTarihi
                })
                .ToListAsync();
        }

        public async Task<List<IlanListeDTO>> KullaniciIlanlariniListele(int kullaniciId)
        {
            return await _context.Ilanlar
                .Include(i => i.Kullanici)
                .Include(i => i.Basvurular)
                .Where(i => i.KullaniciId == kullaniciId)
                .OrderByDescending(i => i.YayinlanmaTarihi)
                .Select(i => new IlanListeDTO
                {
                    Id = i.Id,
                    SirketAdi = i.Kullanici.SirketAdi,
                    Baslik = i.Baslik,
                    Lokasyon = i.Lokasyon,
                    PozisyonTipi = i.PozisyonTipi,
                    TecrubeSeviyesi = i.TecrubeSeviyesi ?? "",
                    YayinlanmaTarihi = i.YayinlanmaTarihi,
                    BasvuruSayisi = i.Basvurular.Count,
                    AktifMi = i.AktifMi,
                    SonBasvuruTarihi = i.SonBasvuruTarihi
                })
                .ToListAsync();
        }

        public async Task<bool> IlanSil(int id, int kullaniciId)
        {
            var ilan = await _context.Ilanlar
                .FirstOrDefaultAsync(i => i.Id == id && (i.KullaniciId == kullaniciId || 
                                        _context.Kullanicilar.Any(k => k.Id == kullaniciId && k.IsAdmin)));

            if (ilan == null)
            {
                return false;
            }

            _context.Ilanlar.Remove(ilan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IlanDurumGuncelle(int id, int kullaniciId, bool aktifMi)
        {
            var ilan = await _context.Ilanlar
                .FirstOrDefaultAsync(i => i.Id == id && (i.KullaniciId == kullaniciId || 
                                        _context.Kullanicilar.Any(k => k.Id == kullaniciId && k.IsAdmin)));

            if (ilan == null)
            {
                return false;
            }

            ilan.AktifMi = aktifMi;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<int> ToplamGoruntulenmeGetir(int kullaniciId)
        {

            var ilanSayisi = await _context.Ilanlar
                .CountAsync(i => i.KullaniciId == kullaniciId);
                
            Random random = new Random();
            return ilanSayisi * random.Next(10, 50);
        }
    }
}
