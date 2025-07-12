using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsanK.Models
{
    public class CVOneri
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CV")]
        public int CVId { get; set; }
        public virtual CV CV { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Aciklama { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Kategori { get; set; } = string.Empty;

        public int OncelikSirasi { get; set; }

        public bool Uygulandimi { get; set; } = false;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}
