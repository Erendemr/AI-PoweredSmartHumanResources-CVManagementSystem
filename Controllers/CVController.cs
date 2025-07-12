using InsanK.DTOs;
using System.IO;
using InsanK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVController : ControllerBase
    {
        private readonly CVService _cvService;
        private readonly ILogger<CVController> _logger;

        public CVController(CVService cvService, ILogger<CVController> logger)
        {
            _cvService = cvService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CVOlustur(CVOlusturDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

            if (kullaniciTipi != "IsArayan")
            {
                return Forbid();
            }

            var cv = await _cvService.CVOlustur(kullaniciId, model);

            if (cv == null)
            {
                return BadRequest(new { message = "CV oluşturulamadı" });
            }

            return Ok(cv);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CVGuncelle(int id, CVGuncelleDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var cv = await _cvService.CVGuncelle(id, kullaniciId, model);

            if (cv == null)
            {
                return NotFound(new { message = "CV bulunamadı veya düzenleme yetkiniz yok" });
            }

            return Ok(cv);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CVDetayGetir(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            var cv = await _cvService.CVDetayGetir(id);

            if (cv == null)
            {
                return NotFound(new { message = "CV bulunamadı" });
            }

            if (cv.KullaniciId != kullaniciId && !isAdmin)
            {
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
                
                if (kullaniciTipi != "IsVeren")
                {
                    return Forbid();
                }
            }

            return Ok(cv);
        }

        [HttpGet("icerik/{id}")]
        public async Task<IActionResult> GetCVIcerik(int id)
        {
            try
            {
                var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

                var cv = await _cvService.CVDetayGetir(id);

                if (cv == null)
                {
                    return NotFound(new { message = "CV bulunamadı" });
                }

                if (cv.KullaniciId != kullaniciId && !isAdmin)
                {
                    var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
                    
                    if (kullaniciTipi != "IsVeren")
                    {
                        return Forbid();
                    }
                }

                var icerik = await _cvService.GetCVIcerik(id, cv.KullaniciId);
                return Ok(new { CVIcerik = icerik, Id = cv.Id, KullaniciId = cv.KullaniciId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "CV içeriği alınırken bir hata oluştu" });
            }
        }

        [HttpGet("kullanici/{kullaniciId}")]
        public async Task<IActionResult> KullaniciCVleriGetir(int kullaniciId)
        {
            try
            {
                var istekYapanKullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

                if (istekYapanKullaniciId != kullaniciId && !isAdmin && kullaniciTipi != "IsVeren")
                {
                    return Forbid();
                }

                var cvler = await _cvService.KullaniciCVleriGetir(kullaniciId);
                return Ok(cvler);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "CV listesi alınırken bir hata oluştu" });
            }
        }

        [HttpGet("benim")]
        public async Task<IActionResult> KendiCVleriniGetir()
        {
            try
            {
                var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var cvler = await _cvService.KullaniciCVleriGetir(kullaniciId);
                return Ok(cvler);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "CV listeniz alınırken bir hata oluştu" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CVSil(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _cvService.CVSil(id, kullaniciId);

            if (!sonuc)
            {
                return NotFound(new { message = "CV bulunamadı veya silme yetkiniz yok" });
            }

            return Ok(new { message = "CV başarıyla silindi" });
        }

        [HttpPost("analiz")]
        public async Task<IActionResult> CVAnaliz(CVAnalizDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

            if (kullaniciTipi != "IsArayan")
            {
                return Forbid();
            }
            
            if (model.CVId <= 0)
            {
                return BadRequest(new { message = "Geçersiz CV ID. Lütfen geçerli bir CV seçin." });
            }

            try
            {
                var oneriler = await _cvService.CVAnaliz(model.CVId, kullaniciId, model.Soru ?? "");

                if (oneriler.Count == 0)
                {
                    return BadRequest(new { message = "CV analizi yapılamadı veya öneriler üretilemedi" });
                }

                return Ok(oneriler);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("CV içeriği olmadığı için") || 
                                                ex.Message.Contains("CV için çok sık analiz") || 
                                                ex.Message.Contains("CV'de önemli bir değişiklik") ||
                                                ex.Message.Contains("CV bulunamadı"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ApplicationException ex) when (ex.InnerException != null && 
                                                ex.InnerException is DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "CV analizi için veri güncellemesi yapılırken bir sorun oluştu. Lütfen tekrar deneyin." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CV analizi sırasında beklenmeyen bir hata: {Message}", ex.Message);
                return BadRequest(new { message = $"API hatası: {ex.Message}. Lütfen farklı bir API anahtarı kullanmayı veya API ayarlarını değiştirmeyi deneyin." });
            }
        }

        [HttpPut("oneri/{oneriId}")]
        public async Task<IActionResult> CVOneriGuncelle(int oneriId, [FromQuery] bool uygulandimi)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _cvService.CVOneriGuncelle(oneriId, kullaniciId, uygulandimi);

            if (!sonuc)
            {
                return NotFound(new { message = "Öneri bulunamadı veya güncelleme yetkiniz yok" });
            }

            return Ok(new { message = "Öneri durumu güncellendi" });
        }

        [HttpGet("dosya/{id}")]
        public async Task<IActionResult> DosyaIndir(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            var cv = await _cvService.CVDetayGetir(id);

            if (cv == null)
            {
                return NotFound(new { message = "CV bulunamadı" });
            }

            if (cv.KullaniciId != kullaniciId && !isAdmin)
            {
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
                
                if (kullaniciTipi != "IsVeren")
                {
                    return Forbid();
                }
            }

            if (string.IsNullOrEmpty(cv.DosyaYolu) || !System.IO.File.Exists(cv.DosyaYolu))
            {
                return NotFound(new { message = "Dosya bulunamadı" });
            }

            var dosyaAdi = Path.GetFileName(cv.DosyaYolu);
            var dosyaIcerigi = await System.IO.File.ReadAllBytesAsync(cv.DosyaYolu);
            var contentType = Path.GetExtension(cv.DosyaYolu).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            return File(dosyaIcerigi, contentType, dosyaAdi);
        }

        
        [HttpPost("tercih")]
        public async Task<IActionResult> KariyerTercihKaydet([FromBody] HedefDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

            if (kullaniciTipi != "IsArayan")
            {
                return Forbid();
            }

            var cvler = await _cvService.KullaniciCVleriGetir(kullaniciId);
            if (cvler == null || cvler.Count == 0)
            {
                return BadRequest(new { message = "Kullanıcıya ait CV bulunamadı" });
            }

            var cv = cvler[0];

            var guncelleDTO = new CVGuncelleDTO
            {
                Baslik = cv.Baslik,
                Ozet = cv.Ozet,
                Meslek = cv.Meslek,
                HedefPozisyon = model.JobTitle,
                HedefSirket = model.CompanyName,
                Egitim = cv.Egitim,
                Deneyim = cv.Deneyim,
                Beceriler = cv.Beceriler,
                Sertifikalar = cv.Sertifikalar,
                Diller = cv.Diller,
                Referanslar = cv.Referanslar,
                Hobiler = cv.Hobiler
            };

            var guncellenenCv = await _cvService.CVGuncelle(cv.Id, kullaniciId, guncelleDTO);
            if (guncellenenCv == null)
            {
                return BadRequest(new { message = "CV güncellenirken bir hata oluştu" });
            }

            return Ok(guncellenenCv);
        }
    }
}