using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class Mesaj
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("GonderenKullanici")]
        public int GonderenId { get; set; }
        public virtual Kullanici GonderenKullanici { get; set; } = null!;

        [ForeignKey("AliciKullanici")]
        public int AliciId { get; set; }
        public virtual Kullanici AliciKullanici { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Konu { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Icerik { get; set; } = string.Empty;

        public DateTime GondermeTarihi { get; set; } = DateTime.Now;

        public bool Okundu { get; set; } = false;

        public DateTime? OkunmaTarihi { get; set; }
    }
}
