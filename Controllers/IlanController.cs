using InsanK.DTOs;
using InsanK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IlanController : ControllerBase
    {
        private readonly IlanService _ilanService;
        private readonly ILogger<IlanController> _logger;

        public IlanController(IlanService ilanService, ILogger<IlanController> logger)
        {
            _ilanService = ilanService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IlanlariListele([FromQuery] string? arama = null, 
            [FromQuery] string? lokasyon = null, [FromQuery] string? pozisyonTipi = null)
        {
            var ilanlar = await _ilanService.IlanlariListele(arama, lokasyon, pozisyonTipi);
            return Ok(ilanlar);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> IlanDetayGetir(int id)
        {
            int kullaniciId = 0;
            if (User.Identity.IsAuthenticated)
            {
                kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            }
            
            var ilan = await _ilanService.IlanDetayGetir(id, kullaniciId);

            if (ilan == null)
            {
                return NotFound(new { message = "İlan bulunamadı" });
            }

            return Ok(ilan);
        }

        [Authorize]
        [HttpGet("kullanici")]
        public async Task<IActionResult> KullaniciIlanlariniListele()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ilanlar = await _ilanService.KullaniciIlanlariniListele(kullaniciId);
            return Ok(ilanlar);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> IlanOlustur(IlanOlusturDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;

            if (kullaniciTipi != "IsVeren")
            {
                return Forbid();
            }

            var ilan = await _ilanService.IlanOlustur(kullaniciId, model);

            if (ilan == null)
            {
                return BadRequest(new { message = "İlan oluşturulamadı" });
            }

            return Ok(ilan);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> IlanGuncelle(int id, IlanGuncelleDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var ilan = await _ilanService.IlanGuncelle(id, kullaniciId, model);

            if (ilan == null)
            {
                return NotFound(new { message = "İlan bulunamadı veya düzenleme yetkiniz yok" });
            }

            return Ok(ilan);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> IlanSil(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _ilanService.IlanSil(id, kullaniciId);

            if (!sonuc)
            {
                return NotFound(new { message = "İlan bulunamadı veya silme yetkiniz yok" });
            }

            return Ok(new { message = "İlan başarıyla silindi" });
        }
        
        [Authorize]
        [HttpGet("goruntulenme-sayisi")]
        public async Task<IActionResult> IlanGoruntulenmeToplamSayisi()
        {
            try
            {
                var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var kullaniciTipi = User.FindFirst("KullaniciTipi")?.Value;
                
                if (kullaniciTipi != "IsVeren")
                {
                    return Forbid();
                }
                
                var toplamGoruntulenme = await _ilanService.ToplamGoruntulenmeGetir(kullaniciId);
                return Ok(toplamGoruntulenme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"İlan görüntülenme sayısı alınırken hata: {ex.Message}");
                return BadRequest(new { message = "İlan görüntülenme sayısı alınırken bir hata oluştu" });
            }
        }

        [Authorize]
        [HttpPut("{id}/durum")]
        public async Task<IActionResult> IlanDurumGuncelle(int id, [FromQuery] bool aktifMi)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _ilanService.IlanDurumGuncelle(id, kullaniciId, aktifMi);

            if (!sonuc)
            {
                return NotFound(new { message = "İlan bulunamadı veya güncelleme yetkiniz yok" });
            }

            return Ok(new { message = "İlan durumu güncellendi" });
        }
    }
}
