using InsanK.DTOs;
using InsanK.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciService _kullaniciService;

        public KullaniciController(KullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        [HttpPost("giris")]
        public async Task<IActionResult> Giris(KullaniciGirisDTO model)
        {
            var (kullanici, token) = await _kullaniciService.Giris(model);

            if (kullanici == null)
            {
                return Unauthorized(new { message = "Kullanıcı adı veya şifre yanlış" });
            }

            return Ok(new
            {
                id = kullanici.Id,
                kullaniciAdi = kullanici.KullaniciAdi,
                email = kullanici.Email,
                ad = kullanici.Ad,
                soyad = kullanici.Soyad,
                isAdmin = kullanici.IsAdmin,
                kullaniciTipi = kullanici.KullaniciTipi,
                token
            });
        }

        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit(KullaniciKayitDTO model)
        {
            var (kullanici, token) = await _kullaniciService.Kayit(model);

            if (kullanici == null)
            {
                return BadRequest(new { message = "Bu kullanıcı adı veya email zaten kullanılıyor" });
            }

            return Ok(new
            {
                id = kullanici.Id,
                kullaniciAdi = kullanici.KullaniciAdi,
                email = kullanici.Email,
                ad = kullanici.Ad,
                soyad = kullanici.Soyad,
                isAdmin = kullanici.IsAdmin,
                kullaniciTipi = kullanici.KullaniciTipi,
                token
            });
        }

        [Authorize]
        [HttpGet("profil/{id}")]
        public async Task<IActionResult> ProfilGetir(int id)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            if (kullaniciId != id && !isAdmin)
            {
                return Forbid();
            }

            var profil = await _kullaniciService.ProfilGetir(id);

            if (profil == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı" });
            }

            return Ok(profil);
        }

        [Authorize]
        [HttpGet("profil")]
        public async Task<IActionResult> KendiProfiliniGetir()
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var profil = await _kullaniciService.ProfilGetir(kullaniciId);

            if (profil == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı" });
            }

            return Ok(profil);
        }

        [Authorize]
        [HttpPut("profil")]
        public async Task<IActionResult> ProfilGuncelle(KullaniciGuncelleDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _kullaniciService.ProfilGuncelle(kullaniciId, model);

            if (!sonuc)
            {
                return BadRequest(new { message = "Profil güncellenemedi" });
            }

            return Ok(new { message = "Profil başarıyla güncellendi" });
        }

        [Authorize]
        [HttpPut("sifre")]
        public async Task<IActionResult> SifreGuncelle(SifreGuncelleDTO model)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sonuc = await _kullaniciService.SifreGuncelle(kullaniciId, model);

            if (!sonuc)
            {
                return BadRequest(new { message = "Şifre güncellenemedi. Mevcut şifre doğru değil." });
            }

            return Ok(new { message = "Şifre başarıyla güncellendi" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("liste")]
        public async Task<IActionResult> KullanicilariListele()
        {
            var kullanicilar = await _kullaniciService.KullanicilariListele();
            return Ok(kullanicilar);
        }

        [Authorize]
        [HttpGet("ara")]
        public async Task<IActionResult> KullaniciAra([FromQuery] string aranan)
        {
            if (string.IsNullOrWhiteSpace(aranan))
            {
                return BadRequest(new { message = "Arama kriteri belirtmelisiniz" });
            }

            var kullanicilar = await _kullaniciService.KullaniciAra(aranan);
            return Ok(kullanicilar);
        }
    }
}
