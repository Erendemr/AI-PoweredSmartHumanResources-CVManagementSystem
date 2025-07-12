using InsanK.Data;
using InsanK.DTOs;
using InsanK.Helpers;
using InsanK.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace InsanK.Services
{
    public class CVService
    {
        private readonly ApplicationDbContext _context;
        private readonly Helpers.CVAIService _aiService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CVService> _logger;
        private readonly PDFExtractor _pdfExtractor;

        public CVService(ApplicationDbContext context, Helpers.CVAIService aiService, IWebHostEnvironment environment, 
            ILogger<CVService> logger, PDFExtractor pdfExtractor)
        {
            _context = context;
            _aiService = aiService;
            _environment = environment;
            _logger = logger;
            _pdfExtractor = pdfExtractor;
        }

        public async Task<CVDetayDTO?> CVOlustur(int kullaniciId, CVOlusturDTO model)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
            {
                return null;
            }

            var mevcutCV = await _context.CVler
                .FirstOrDefaultAsync(c => c.KullaniciId == kullaniciId);

            if (mevcutCV != null)
            {
                _logger.LogInformation("Bu kullanıcı için zaten bir CV mevcut (ID: {CVId}). Güncelleme yapılacak.", mevcutCV.Id);
                
                mevcutCV.Baslik = model.Baslik;
                mevcutCV.Ozet = model.Ozet;
                mevcutCV.Meslek = model.Meslek;
                mevcutCV.HedefPozisyon = model.HedefPozisyon;
                mevcutCV.HedefSirket = model.HedefSirket;
                mevcutCV.Egitim = model.Egitim;
                mevcutCV.Deneyim = model.Deneyim;
                mevcutCV.Beceriler = model.Beceriler;
                mevcutCV.Sertifikalar = model.Sertifikalar;
                mevcutCV.Diller = model.Diller;
                mevcutCV.Referanslar = model.Referanslar;
                mevcutCV.Hobiler = model.Hobiler;
                mevcutCV.CVIcerik = model.CVIcerik;
                mevcutCV.GuncellemeTarihi = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return await CVDetayGetir(mevcutCV.Id);
            }

            var cv = new CV
            {
                KullaniciId = kullaniciId,
                Baslik = model.Baslik,
                Ozet = model.Ozet,
                Meslek = model.Meslek,
                HedefPozisyon = model.HedefPozisyon,
                HedefSirket = model.HedefSirket,
                Egitim = model.Egitim,
                Deneyim = model.Deneyim,
                Beceriler = model.Beceriler,
                Sertifikalar = model.Sertifikalar,
                Diller = model.Diller,
                Referanslar = model.Referanslar,
                Hobiler = model.Hobiler,
                CVIcerik = model.CVIcerik,
                OlusturmaTarihi = DateTime.Now
            };

            _context.CVler.Add(cv);
            await _context.SaveChangesAsync();

            return await CVDetayGetir(cv.Id);
        }

        public async Task<CVDetayDTO?> CVGuncelle(int id, int kullaniciId, CVGuncelleDTO model)
        {
            var cv = await _context.CVler
                .FirstOrDefaultAsync(c => c.Id == id && c.KullaniciId == kullaniciId);

            if (cv == null)
            {
                return null;
            }

            cv.Baslik = model.Baslik;
            cv.Ozet = model.Ozet;
            cv.Meslek = model.Meslek;
            cv.HedefPozisyon = model.HedefPozisyon;
            cv.HedefSirket = model.HedefSirket;
            cv.Egitim = model.Egitim;
            cv.Deneyim = model.Deneyim;
            cv.Beceriler = model.Beceriler;
            cv.Sertifikalar = model.Sertifikalar;
            cv.Diller = model.Diller;
            cv.Referanslar = model.Referanslar;
            cv.Hobiler = model.Hobiler;
            cv.CVIcerik = model.CVIcerik;
            cv.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return await CVDetayGetir(id);
        }

        public async Task<CVDetayDTO?> CVDetayGetir(int id)
        {
            try
            {
                _logger.LogInformation("CVDetayGetir için sorgu yapılıyor. CVId: {CVId}", id);
                
                var cv = await _context.CVler
                    .Include(c => c.Oneriler)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cv == null)
                {
                    _logger.LogWarning("CVDetayGetir: CV bulunamadı. CVId: {CVId}", id);
                    return null;
                }

                _logger.LogInformation("CVDetayGetir başarılı. CVId: {CVId}, KullaniciId: {KullaniciId}", id, cv.KullaniciId);
                
                var cvModel = new CVDetayDTO
                {
                    Id = cv.Id,
                    KullaniciId = cv.KullaniciId,
                    Baslik = cv.Baslik,
                    Ozet = cv.Ozet,
                    Meslek = cv.Meslek,
                    HedefPozisyon = cv.HedefPozisyon,
                    HedefSirket = cv.HedefSirket,
                    Egitim = cv.Egitim,
                    Deneyim = cv.Deneyim,
                    Beceriler = cv.Beceriler,
                    Sertifikalar = cv.Sertifikalar,
                    Diller = cv.Diller,
                    Referanslar = cv.Referanslar,
                    Hobiler = cv.Hobiler,
                    OlusturmaTarihi = cv.OlusturmaTarihi,
                    GuncellemeTarihi = cv.GuncellemeTarihi,
                    DosyaYolu = cv.DosyaYolu,
                    AIAnalizi = cv.AIAnalizi,
                    CVIcerik = cv.CVIcerik,
                    Oneriler = cv.Oneriler?.Select(o => new CVOneriDTO
                    {
                        Id = o.Id,
                        CVId = o.CVId,
                        Baslik = o.Baslik,
                        Aciklama = o.Aciklama,
                        Kategori = o.Kategori,
                        OncelikSirasi = o.OncelikSirasi,
                        Uygulandimi = o.Uygulandimi,
                        OlusturmaTarihi = o.OlusturmaTarihi
                    }).ToList()
                };
                
                return cvModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CVDetayGetir işlemi sırasında hata oluştu. CVId: {CVId}", id);
                throw;
            }
        }

        public async Task<List<CVDetayDTO>> KullaniciCVleriGetir(int kullaniciId)
        {
            try
            {
                _logger.LogInformation("KullaniciCVleriGetir için sorgu yapılıyor. KullaniciId: {KullaniciId}", kullaniciId);
                
                var cvler = await _context.CVler
                    .Where(c => c.KullaniciId == kullaniciId)
                    .Select(cv => new CVDetayDTO
                    {
                        Id = cv.Id,
                        KullaniciId = cv.KullaniciId,
                        Baslik = cv.Baslik,
                        Ozet = cv.Ozet,
                        Meslek = cv.Meslek,
                        HedefPozisyon = cv.HedefPozisyon,
                        HedefSirket = cv.HedefSirket,
                        Egitim = cv.Egitim,
                        Deneyim = cv.Deneyim,
                        Beceriler = cv.Beceriler,
                        Sertifikalar = cv.Sertifikalar,
                        Diller = cv.Diller,
                        Referanslar = cv.Referanslar,
                        Hobiler = cv.Hobiler,
                        OlusturmaTarihi = cv.OlusturmaTarihi,
                        GuncellemeTarihi = cv.GuncellemeTarihi,
                        DosyaYolu = cv.DosyaYolu,
                        AIAnalizi = cv.AIAnalizi,
                        CVIcerik = cv.CVIcerik 
                    })
                    .ToListAsync();
                    
                _logger.LogInformation("KullaniciCVleriGetir başarılı. KullaniciId: {KullaniciId}, CV Sayısı: {CVSayisi}", kullaniciId, cvler.Count);
                return cvler;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "KullaniciCVleriGetir işlemi sırasında hata oluştu. KullaniciId: {KullaniciId}", kullaniciId);
                throw;
            }
        }

        public async Task<string?> GetCVIcerik(int cvId, int kullaniciId)
        {
            try
            {
                _logger.LogInformation("GetCVIcerik metodu çağrıldı. CVId: {CVId}, KullaniciId: {KullaniciId}", cvId, kullaniciId);
                
                var icerik = await _context.CVler
                    .Where(c => c.Id == cvId && c.KullaniciId == kullaniciId)
                    .Select(c => c.CVIcerik)
                    .FirstOrDefaultAsync();
                    
                return icerik;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCVIcerik işlemi sırasında hata oluştu. CVId: {CVId}, KullaniciId: {KullaniciId}", cvId, kullaniciId);
                throw;
            }
        }

        public async Task<bool> CVSil(int id, int kullaniciId)
        {
            var cv = await _context.CVler
                .FirstOrDefaultAsync(c => c.Id == id && c.KullaniciId == kullaniciId);

            if (cv == null)
            {
                return false;
            }

            _context.CVler.Remove(cv);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CVOneriDTO>> CVAnaliz(int cvId, int kullaniciId, string soru = "")
        {
            try {
                var cv = await _context.CVler
                    .FirstOrDefaultAsync(c => c.Id == cvId && c.KullaniciId == kullaniciId);

                if (cv == null)
                {
                    throw new ApplicationException("CV bulunamadı veya erişim yetkiniz yok.");
                }
                
                List<CVOneri> oneriler;
                
                try {
                    oneriler = await _aiService.AnalizEtVeOneriGetir(cv, soru);
                } catch (Exception ex) {
                    _logger.LogError(ex, "AI servisi hatası: {Message}", ex.Message);
                    throw;
                }
                
                try {
                    var eskiOneriler = await _context.CVOnerileri.Where(o => o.CVId == cvId).ToListAsync();
                    if (eskiOneriler.Any())
                    {
                        _context.CVOnerileri.RemoveRange(eskiOneriler);
                    }
                    
                    foreach (var oneri in oneriler)
                    {
                        oneri.Id = 0;
                            

                        if (oneri.Kategori.Length > 50)
                        {
                            _logger.LogWarning($"Kategori metin uzunluğu ({oneri.Kategori.Length}) sınırı aşıyor. Kısaltılıyor.");
                            oneri.Kategori = oneri.Kategori.Substring(0, 47) + "...";
                        }
                        
                        if (oneri.Baslik.Length > 100)
                        {
                            _logger.LogWarning($"Başlık metin uzunluğu ({oneri.Baslik.Length}) sınırı aşıyor. Kısaltılıyor.");
                            oneri.Baslik = oneri.Baslik.Substring(0, 97) + "...";
                        }
                        
                        if (oneri.Aciklama.Length > 1000)
                        {
                            _logger.LogWarning($"Açıklama metin uzunluğu ({oneri.Aciklama.Length}) sınırı aşıyor. Kısaltılıyor.");
                            oneri.Aciklama = oneri.Aciklama.Substring(0, 997) + "...";
                        }
                    }
                    
                    _context.CVOnerileri.AddRange(oneriler);
                        
                    cv.AIAnalizi = "CV analizi tamamlandı. " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    cv.GuncellemeTarihi = DateTime.Now;

                    await _context.SaveChangesAsync();
                } 
                catch (DbUpdateConcurrencyException ex) 
                {
                    _logger.LogError(ex, "CV analizi sırasında concurrency hatası oluştu. CVId: {CVId}", cvId);
                    return oneriler.Select(o => new CVOneriDTO
                    {
                        Id = o.Id,
                        CVId = o.CVId,
                        Baslik = o.Baslik,
                        Aciklama = o.Aciklama,
                        Kategori = o.Kategori,
                        OncelikSirasi = o.OncelikSirasi,
                        Uygulandimi = o.Uygulandimi,
                        OlusturmaTarihi = o.OlusturmaTarihi
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CV analiz sonuçları kaydedilirken hata oluştu. CVId: {CVId}", cvId);
                    throw new ApplicationException("CV analiz sonuçları kaydedilirken bir hata oluştu.", ex);
                }

                return oneriler.Select(o => new CVOneriDTO
                {
                    Id = o.Id,
                    CVId = o.CVId,
                    Baslik = o.Baslik,
                    Aciklama = o.Aciklama,
                    Kategori = o.Kategori,
                    OncelikSirasi = o.OncelikSirasi,
                    Uygulandimi = o.Uygulandimi,
                    OlusturmaTarihi = o.OlusturmaTarihi
                }).ToList();
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("CV analizi sırasında bir hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task<bool> CVOneriGuncelle(int oneriId, int kullaniciId, bool uygulandimi)
        {
            var oneri = await _context.CVOnerileri
                .Include(o => o.CV)
                .FirstOrDefaultAsync(o => o.Id == oneriId && o.CV.KullaniciId == kullaniciId);

            if (oneri == null)
            {
                return false;
            }

            oneri.Uygulandimi = uygulandimi;
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<object> CVOneriOlustur(int kullaniciId, int ilanId)
        {
            try
            {
                var cv = await _context.CVler
                    .FirstOrDefaultAsync(c => c.KullaniciId == kullaniciId);
                    
                if (cv == null)
                {
                    throw new ApplicationException("Öneri oluşturmak için CV'niz bulunmalıdır.");
                }
                
                var ilan = await _context.Ilanlar
                    .Include(i => i.Kullanici)
                    .FirstOrDefaultAsync(i => i.Id == ilanId);
                    
                if (ilan == null)
                {
                    throw new ApplicationException("Belirtilen ilan bulunamadı.");
                }
                
                string oneri = await _aiService.CVIlanOneriOlustur(cv, ilan);
                
                return new { oneri = oneri };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CV öneri oluşturma sırasında hata: KullaniciId={KullaniciId}, IlanId={IlanId}", kullaniciId, ilanId);
                throw new ApplicationException("CV öneri oluşturma sırasında hata: " + ex.Message, ex);
            }
        }
    }
}