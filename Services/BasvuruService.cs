using InsanK.Data;
using InsanK.DTOs;
using InsanK.Models;
using Microsoft.EntityFrameworkCore;

namespace InsanK.Services
{
    public class BasvuruService
    {
        private readonly ApplicationDbContext _context;

        public BasvuruService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BasvuruDetayDTO?> BasvuruOlustur(int kullaniciId, BasvuruOlusturDTO model)
        {
            try 
            {
                var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
                if (kullanici == null || kullanici.KullaniciTipi != KullaniciTipi.IsArayan)
                {
                    Console.WriteLine($"Kullanıcı bulunamadı veya iş arayan değil: {kullaniciId}");
                    return null;
                }

                var ilan = await _context.Ilanlar.FindAsync(model.IlanId);
                if (ilan == null || !ilan.AktifMi)
                {
                    Console.WriteLine($"İlan bulunamadı veya aktif değil: {model.IlanId}");
                    return null;
                }

                var mevcutBasvuru = await _context.Basvurular
                    .FirstOrDefaultAsync(b => b.KullaniciId == kullaniciId && b.IlanId == model.IlanId);
                
                if (mevcutBasvuru != null)
                {
                    Console.WriteLine($"Kullanıcı {kullaniciId} zaten {model.IlanId} ID'li ilana başvurmuş");
                    return null;
                }

                if (model.CVId.HasValue)
                {
                    var cv = await _context.CVler
                        .FirstOrDefaultAsync(c => c.Id == model.CVId && c.KullaniciId == kullaniciId);
                    
                    if (cv == null)
                    {
                        Console.WriteLine($"Belirtilen CV bulunamadı veya kullanıcıya ait değil: CV ID {model.CVId}, Kullanıcı ID {kullaniciId}");
                        throw new Exception("Başvuru yapabilmek için geçerli bir CV gereklidir");
                    }
                }
                else 
                {
                    var herhangiCvVar = await _context.CVler.AnyAsync(c => c.KullaniciId == kullaniciId);
                    if (!herhangiCvVar)
                    {
                        Console.WriteLine($"Kullanıcının {kullaniciId} hiç CV kaydı yok");
                        throw new Exception("Başvuru yapabilmek için önce bir CV yüklemelisiniz");
                    }
                    
                    var enSonCV = await _context.CVler
                        .Where(c => c.KullaniciId == kullaniciId)
                        .OrderByDescending(c => c.OlusturmaTarihi)
                        .FirstOrDefaultAsync();
                    
                    if (enSonCV != null)
                    {
                        model.CVId = enSonCV.Id;
                    }
                }

                var basvuru = new Basvuru
                {
                    KullaniciId = kullaniciId,
                    IlanId = model.IlanId,
                    OnYazisi = model.OnYazisi,
                    CVId = model.CVId,
                    BasvuruTarihi = DateTime.Now,
                    Durum = BasvuruDurumu.Beklemede
                };

                _context.Basvurular.Add(basvuru);
                await _context.SaveChangesAsync();

                return await BasvuruDetayGetir(basvuru.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Başvuru oluşturma hatası: {ex.Message}");
                throw;
            }
        }

        public async Task<BasvuruDetayDTO?> BasvuruDurumGuncelle(int id, int kullaniciId, BasvuruGuncelleDTO model)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .FirstOrDefaultAsync(b => b.Id == id && b.Ilan.KullaniciId == kullaniciId);

            if (basvuru == null)
            {
                return null;
            }

            basvuru.Durum = model.Durum;
            basvuru.NotlarGeribildirimi = model.NotlarGeribildirimi;

            await _context.SaveChangesAsync();
            return await BasvuruDetayGetir(id);
        }

        public async Task<BasvuruDetayDTO?> BasvuruDetayGetir(int id)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Kullanici)
                .Include(b => b.Ilan)
                    .ThenInclude(i => i.Kullanici)
                .Include(b => b.CV)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (basvuru == null)
            {
                return null;
            }

            return new BasvuruDetayDTO
            {
                Id = basvuru.Id,
                KullaniciId = basvuru.KullaniciId,
                KullaniciAdi = basvuru.Kullanici.KullaniciAdi,
                KullaniciEmail = basvuru.Kullanici.Email,
                IlanId = basvuru.IlanId,
                IlanBaslik = basvuru.Ilan.Baslik,
                SirketAdi = basvuru.Ilan.Kullanici?.SirketAdi ?? "",
                BasvuruTarihi = basvuru.BasvuruTarihi,
                OnYazisi = basvuru.OnYazisi,
                Durum = basvuru.Durum,
                NotlarGeribildirimi = basvuru.NotlarGeribildirimi,
                CVId = basvuru.CVId,
                CVBaslik = basvuru.CV?.Baslik
            };
        }

        public async Task<List<BasvuruDetayDTO>> IlanBasvurulariniListele(int ilanId, int kullaniciId)
        {
            var ilan = await _context.Ilanlar.FirstOrDefaultAsync(i => i.Id == ilanId && i.KullaniciId == kullaniciId);
            if (ilan == null)
            {
                return new List<BasvuruDetayDTO>();
            }

            return await _context.Basvurular
                .Include(b => b.Kullanici)
                .Include(b => b.Ilan)
                    .ThenInclude(i => i.Kullanici)
                .Include(b => b.CV)
                .Where(b => b.IlanId == ilanId)
                .OrderByDescending(b => b.BasvuruTarihi)
                .Select(b => new BasvuruDetayDTO
                {
                    Id = b.Id,
                    KullaniciId = b.KullaniciId,
                    KullaniciAdi = b.Kullanici.KullaniciAdi,
                    KullaniciEmail = b.Kullanici.Email,
                    IlanId = b.IlanId,
                    IlanBaslik = b.Ilan.Baslik,
                    SirketAdi = b.Ilan.Kullanici.SirketAdi ?? "",
                    BasvuruTarihi = b.BasvuruTarihi,
                    OnYazisi = b.OnYazisi,
                    Durum = b.Durum,
                    NotlarGeribildirimi = b.NotlarGeribildirimi,
                    CVId = b.CVId,
                    CVBaslik = b.CV != null ? b.CV.Baslik : null
                })
                .ToListAsync();
        }

        public async Task<List<BasvuruDetayDTO>> KullaniciBasvurulariniListele(int kullaniciId)
        {
            return await _context.Basvurular
                .Include(b => b.Kullanici)
                .Include(b => b.Ilan)
                    .ThenInclude(i => i.Kullanici)
                .Include(b => b.CV)
                .Where(b => b.KullaniciId == kullaniciId)
                .OrderByDescending(b => b.BasvuruTarihi)
                .Select(b => new BasvuruDetayDTO
                {
                    Id = b.Id,
                    KullaniciId = b.KullaniciId,
                    KullaniciAdi = b.Kullanici.KullaniciAdi,
                    KullaniciEmail = b.Kullanici.Email,
                    IlanId = b.IlanId,
                    IlanBaslik = b.Ilan.Baslik,
                    SirketAdi = b.Ilan.Kullanici.SirketAdi ?? "",
                    BasvuruTarihi = b.BasvuruTarihi,
                    OnYazisi = b.OnYazisi,
                    Durum = b.Durum,
                    NotlarGeribildirimi = b.NotlarGeribildirimi,
                    CVId = b.CVId,
                    CVBaslik = b.CV != null ? b.CV.Baslik : null
                })
                .ToListAsync();
        }

        public async Task<int> ToplamBasvuruSayisi(int isVerenId)
        {
            return await _context.Basvurular
                .Where(b => b.Ilan.KullaniciId == isVerenId)
                .CountAsync();
        }

        public async Task<bool> BasvuruIptal(int id, int kullaniciId)
        {
            var basvuru = await _context.Basvurular
                .FirstOrDefaultAsync(b => b.Id == id && b.KullaniciId == kullaniciId);

            if (basvuru == null)
            {
                return false;
            }

            _context.Basvurular.Remove(basvuru);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
