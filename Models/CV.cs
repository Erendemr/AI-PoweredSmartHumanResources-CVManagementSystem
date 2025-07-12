using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class CV
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Ozet { get; set; }

        [StringLength(100)]
        public string? Meslek { get; set; }

        [StringLength(50)]
        public string? HedefPozisyon { get; set; }

        [StringLength(100)]
        public string? HedefSirket { get; set; }

        [StringLength(2000)]
        public string? Egitim { get; set; }

        [StringLength(2000)]
        public string? Deneyim { get; set; }

        [StringLength(500)]
        public string? Beceriler { get; set; }

        [StringLength(500)]
        public string? Sertifikalar { get; set; }

        [StringLength(500)]
        public string? Diller { get; set; }

        [StringLength(500)]
        public string? Referanslar { get; set; }

        [StringLength(500)]
        public string? Hobiler { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellemeTarihi { get; set; }

        [StringLength(500)]
        public string? DosyaYolu { get; set; }

        [StringLength(1000)]
        public string? AIAnalizi { get; set; }

        [StringLength(100000)]
        public string? CVIcerik { get; set; }

        public virtual ICollection<CVOneri>? Oneriler { get; set; }
    }
}