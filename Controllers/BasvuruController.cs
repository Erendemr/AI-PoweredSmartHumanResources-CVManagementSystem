using InsanK.DTOs;
using InsanK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BasvuruController : ControllerBase
    {
        private readonly BasvuruService _basvuruService;
        private readonly ILogger<BasvuruController> _logger;

        public BasvuruController(BasvuruService basvuruService, ILogger<BasvuruController> logger)
        {
            _basvuruService = basvuruService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> BasvuruOlustur(BasvuruOlusturDTO model)
        {
            try 
            {
                var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

                if (kullaniciTipi != "IsArayan")
                {
                    return Forbid();
                }

                if (model.IlanId <= 0)
                {
                    return BadRequest(new { message = "Geçersiz ilan ID" });
                }
                
                var basvuru = await _basvuruService.BasvuruOlustur(kullaniciId, model);

                if (basvuru == null)
                {
                    return BadRequest(new { message = "Başvuru yapılamadı veya zaten başvuru yapmışsınız" });
                }

                return Ok(basvuru);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Başvuru oluşturma hatası: {ex.Message}");
                
                return BadRequest(new { message = $"Başvuru oluşturulurken bir hata meydana geldi: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BasvuruDetayGetir(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            var basvuru = await _basvuruService.BasvuruDetayGetir(id);

            if (basvuru == null)
            {
                return NotFound(new { message = "Başvuru bulunamadı" });
            }

            if (basvuru.KullaniciId != kullaniciId && !isAdmin)
            {
                return Forbid();
            }

            return Ok(basvuru);
        }

        [HttpGet("ilan/{ilanId}")]
        public async Task<IActionResult> IlanBasvurulariniListele(int ilanId)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            if (kullaniciTipi != "IsVeren" && !isAdmin)
            {
                return Forbid();
            }

            var basvurular = await _basvuruService.IlanBasvurulariniListele(ilanId, kullaniciId);
            return Ok(basvurular);
        }

        [HttpGet("kullanici")]
        public async Task<IActionResult> KullaniciBasvurulariniListele()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var basvurular = await _basvuruService.KullaniciBasvurulariniListele(kullaniciId);
            return Ok(basvurular);
        }

        [HttpGet("toplam-sayisi")]
        public async Task<IActionResult> ToplamBasvuruSayisi()
        {
            try
            {
                var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
                
                if (kullaniciTipi != "IsVeren")
                {
                    return Forbid();
                }
                
                var toplamSayi = await _basvuruService.ToplamBasvuruSayisi(kullaniciId);
                return Ok(toplamSayi);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Toplam başvuru sayısı alınırken hata: {ex.Message}");
                return BadRequest(new { message = "Toplam başvuru sayısı alınırken bir hata oluştu" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> BasvuruDurumGuncelle(int id, BasvuruGuncelleDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            if (kullaniciTipi != "IsVeren" && !isAdmin)
            {
                return Forbid();
            }

            var basvuru = await _basvuruService.BasvuruDurumGuncelle(id, kullaniciId, model);

            if (basvuru == null)
            {
                return NotFound(new { message = "Başvuru bulunamadı veya güncelleme yetkiniz yok" });
            }

            return Ok(basvuru);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> BasvuruIptal(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

            if (kullaniciTipi != "IsArayan")
            {
                return Forbid();
            }

            var sonuc = await _basvuruService.BasvuruIptal(id, kullaniciId);

            if (!sonuc)
            {
                return NotFound(new { message = "Başvuru bulunamadı veya iptal etme yetkiniz yok" });
            }

            return Ok(new { message = "Başvuru başarıyla iptal edildi" });
        }
    }
}
