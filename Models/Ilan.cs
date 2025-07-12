using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class Ilan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

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

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinMaas { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxMaas { get; set; }

        public DateTime YayinlanmaTarihi { get; set; } = DateTime.Now;
        public DateTime? SonBasvuruTarihi { get; set; }

        public bool AktifMi { get; set; } = true;

        [StringLength(50)]
        public string? EgitimSeviyesi { get; set; }

        [StringLength(500)]
        public string? SirketLokasyonu { get; set; }

        public virtual ICollection<Basvuru>? Basvurular { get; set; }
    }
}
