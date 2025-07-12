using InsanK.DTOs;
using InsanK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesajController : ControllerBase
    {
        private readonly MesajService _mesajService;

        public MesajController(MesajService mesajService)
        {
            _mesajService = mesajService;
        }

        [HttpPost]
        public async Task<IActionResult> MesajGonder(MesajGonderDTO model)
        {
            Console.WriteLine($"Alınan model: AliciId={model.AliciId}, Konu='{model.Konu}', Icerik Uzunluk={model.Icerik?.Length ?? 0}");
            
            if (!ModelState.IsValid)
            {
                var hatalar = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                Console.WriteLine($"Model doğrulama hataları: {System.Text.Json.JsonSerializer.Serialize(hatalar)}");
                return BadRequest(new { message = "Geçersiz veri formatı", hatalar });
            }

            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var mesaj = await _mesajService.MesajGonder(kullaniciId, model);

            if (mesaj == null)
            {
                return BadRequest(new { message = "Mesaj gönderilemedi" });
            }

            return Ok(mesaj);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> MesajDetayGetir(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            var mesaj = await _mesajService.MesajDetayGetir(id, kullaniciId);

            if (mesaj == null)
            {
                return NotFound(new { message = "Mesaj bulunamadı veya görüntüleme yetkiniz yok" });
            }

            return Ok(mesaj);
        }

        [HttpGet("gelen")]
        public async Task<IActionResult> GelenMesajlariListele()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var mesajlar = await _mesajService.GelenMesajlariListele(kullaniciId);
            return Ok(mesajlar);
        }

        [HttpGet("giden")]
        public async Task<IActionResult> GonderilenMesajlariListele()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var mesajlar = await _mesajService.GonderilenMesajlariListele(kullaniciId);
            return Ok(mesajlar);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> MesajSil(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _mesajService.MesajSil(id, kullaniciId);

            if (!sonuc)
            {
                return NotFound(new { message = "Mesaj bulunamadı veya silme yetkiniz yok" });
            }

            return Ok(new { message = "Mesaj başarıyla silindi" });
        }

        [HttpPost("okundu-isaretle")]
        public async Task<IActionResult> TumMesajlariOkunduIsaretle()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var sonuc = await _mesajService.TumMesajlariOkunduIsaretle(kullaniciId);
            
            if (!sonuc)
            {
                return Ok(new { message = "Okunmamış mesaj bulunmamaktadır" });
            }

            return Ok(new { message = "Tüm mesajlar okundu olarak işaretlendi" });
        }

        [HttpGet("okunmamis-sayisi")]
        public async Task<IActionResult> OkunmamisMesajSayisi()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var sayi = await _mesajService.OkunmamisMesajSayisi(kullaniciId);
            return Ok(sayi);
        }

        [HttpGet("kullanici/{kullaniciId}")]
        public async Task<IActionResult> KullaniciMesajlariGetir(int kullaniciId)
        {
            var mevcutKullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var mesajlar = await _mesajService.KullaniciMesajlariGetir(mevcutKullaniciId, kullaniciId);
            return Ok(mesajlar);
        }
    }
}
