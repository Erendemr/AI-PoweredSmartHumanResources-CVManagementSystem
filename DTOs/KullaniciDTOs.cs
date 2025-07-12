using InsanK.Models;
using System.ComponentModel.DataAnnotations;

namespace InsanK.DTOs
{
    public class KullaniciKayitDTO
    {
        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Sifre { get; set; } = string.Empty;

        [Required]
        [Compare("Sifre")]
        public string SifreTekrar { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ad { get; set; }

        [StringLength(100)]
        public string? Soyad { get; set; }

        [StringLength(15)]
        public string? Telefon { get; set; }

        [Required]
        public KullaniciTipi KullaniciTipi { get; set; }

        [StringLength(150)]
        public string? SirketAdi { get; set; }

        [StringLength(250)]
        public string? SirketAciklamasi { get; set; }
    }

    public class KullaniciGirisDTO
    {
        [Required]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        public string Sifre { get; set; } = string.Empty;
    }

    public class KullaniciProfilDTO
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Telefon { get; set; }
        public bool IsAdmin { get; set; }
        public KullaniciTipi KullaniciTipi { get; set; }
        public string? SirketAdi { get; set; }
        public string? SirketAciklamasi { get; set; }
        public DateTime KayitTarihi { get; set; }
    }

    public class KullaniciGuncelleDTO
    {
        [StringLength(100)]
        public string? Ad { get; set; }

        [StringLength(100)]
        public string? Soyad { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(15)]
        public string? Telefon { get; set; }

        [StringLength(150)]
        public string? SirketAdi { get; set; }

        [StringLength(250)]
        public string? SirketAciklamasi { get; set; }
    }

    public class SifreGuncelleDTO
    {
        [Required]
        public string MevcutSifre { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string YeniSifre { get; set; } = string.Empty;

        [Required]
        [Compare("YeniSifre")]
        public string YeniSifreTekrar { get; set; } = string.Empty;
    }

public class KullaniciAramaDTO
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string? Ad { get; set; }
    public string? Soyad { get; set; }
    public KullaniciTipi KullaniciTipi { get; set; }
    public string? SirketAdi { get; set; }
}
}
