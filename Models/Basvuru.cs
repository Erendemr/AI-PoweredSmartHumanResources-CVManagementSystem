using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class Basvuru
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

        [ForeignKey("Ilan")]
        public int IlanId { get; set; }
        public virtual Ilan Ilan { get; set; } = null!;

        public DateTime BasvuruTarihi { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? OnYazisi { get; set; }

        public BasvuruDurumu Durum { get; set; } = BasvuruDurumu.Beklemede;

        [StringLength(500)]
        public string? NotlarGeribildirimi { get; set; }

        [ForeignKey("CV")]
        public int? CVId { get; set; }
        public virtual CV? CV { get; set; }
    }

    public enum BasvuruDurumu
    {
        Beklemede = 1,
        Inceleniyor = 2,
        Mulakat = 3,
        Reddedildi = 4,
        Kabul = 5
    }
}
