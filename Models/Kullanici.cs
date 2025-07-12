using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Sifre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ad { get; set; }

        [StringLength(100)]
        public string? Soyad { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        [StringLength(15)]
        public string? Telefon { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required]
        public KullaniciTipi KullaniciTipi { get; set; }

        public virtual CV? CV { get; set; }

        [StringLength(150)]
        public string? SirketAdi { get; set; }

        [StringLength(250)]
        public string? SirketAciklamasi { get; set; }

        public virtual ICollection<Ilan>? Ilanlar { get; set; }
        public virtual ICollection<Basvuru>? Basvurular { get; set; }
        public virtual ICollection<Mesaj>? GonderilenMesajlar { get; set; }
        public virtual ICollection<Mesaj>? AlinanMesajlar { get; set; }
    }

    public enum KullaniciTipi
    {
        IsArayan = 1,
        IsVeren = 2
    }
}
