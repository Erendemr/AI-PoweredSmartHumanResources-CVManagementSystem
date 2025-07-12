using InsanK.Models;
using System.ComponentModel.DataAnnotations;

namespace InsanK.DTOs
{
    public class IlanOlusturDTO
    {
        [Required]
        [StringLength(100)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Aciklama { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Lokasyon { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PozisyonTipi { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DepartmanAdi { get; set; }

        [StringLength(50)]
        public string? TecrubeSeviyesi { get; set; }

        [StringLength(500)]
        public string? GerekliYetkinlikler { get; set; }

        [StringLength(500)]
        public string? TercihEdilenYetkinlikler { get; set; }

        public decimal? MinMaas { get; set; }
        public decimal? MaxMaas { get; set; }
        public DateTime? SonBasvuruTarihi { get; set; }
        public string? EgitimSeviyesi { get; set; }
        public string? SirketLokasyonu { get; set; }
    }

    public class IlanGuncelleDTO
    {
        [Required]
        [StringLength(100)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Aciklama { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Lokasyon { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PozisyonTipi { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DepartmanAdi { get; set; }

        [StringLength(50)]
        public string? TecrubeSeviyesi { get; set; }

        [StringLength(500)]
        public string? GerekliYetkinlikler { get; set; }

        [StringLength(500)]
        public string? TercihEdilenYetkinlikler { get; set; }

        public decimal? MinMaas { get; set; }
        public decimal? MaxMaas { get; set; }
        public DateTime? SonBasvuruTarihi { get; set; }
        public bool AktifMi { get; set; }
        public string? EgitimSeviyesi { get; set; }
        public string? SirketLokasyonu { get; set; }
    }

    public class IlanDetayDTO
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string? SirketAdi { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? Lokasyon { get; set; }
        public string? PozisyonTipi { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? TecrubeSeviyesi { get; set; }
        public string? GerekliYetkinlikler { get; set; }
        public string? TercihEdilenYetkinlikler { get; set; }
        public decimal? MinMaas { get; set; }
        public decimal? MaxMaas { get; set; }
        public DateTime YayinlanmaTarihi { get; set; }
        public DateTime? SonBasvuruTarihi { get; set; }
        public bool AktifMi { get; set; }
        public string? EgitimSeviyesi { get; set; }
        public string? SirketLokasyonu { get; set; }
        public bool BasvurulmusMu { get; set; } = false;
    }

    public class IlanListeDTO
    {
        public int Id { get; set; }
        public string? SirketAdi { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Lokasyon { get; set; } = string.Empty;
        public string PozisyonTipi { get; set; } = string.Empty;
        public string? TecrubeSeviyesi { get; set; }
        public DateTime YayinlanmaTarihi { get; set; }
        public int BasvuruSayisi { get; set; }
        public bool AktifMi { get; set; }
        public DateTime? SonBasvuruTarihi { get; set; }
    }

    public class BasvuruOlusturDTO
    {
        [Required]
        public int IlanId { get; set; }

        [StringLength(1000)]
        public string? OnYazisi { get; set; }

        public int? CVId { get; set; }
    }

    public class BasvuruGuncelleDTO
    {
        [Required]
        public BasvuruDurumu Durum { get; set; }

        [StringLength(500)]
        public string? NotlarGeribildirimi { get; set; }
    }

    public class BasvuruDetayDTO
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string KullaniciEmail { get; set; } = string.Empty;
        public int IlanId { get; set; }
        public string IlanBaslik { get; set; } = string.Empty;
        public string SirketAdi { get; set; } = string.Empty;
        public DateTime BasvuruTarihi { get; set; }
        public string? OnYazisi { get; set; }
        public BasvuruDurumu Durum { get; set; }
        public string? NotlarGeribildirimi { get; set; }
        public int? CVId { get; set; }
        public string? CVBaslik { get; set; }
    }
}
