using System.ComponentModel.DataAnnotations;

namespace InsanK.DTOs
{
    public class CVOlusturDTO
    {
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
        
        [StringLength(100000)]
        public string? CVIcerik { get; set; }
    }

    public class CVGuncelleDTO
    {
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
        
        [StringLength(100000)]
        public string? CVIcerik { get; set; }
    }

    public class CVDetayDTO
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Ozet { get; set; }
        public string? Meslek { get; set; }
        public string? HedefPozisyon { get; set; }
        public string? HedefSirket { get; set; }
        public string? Egitim { get; set; }
        public string? Deneyim { get; set; }
        public string? Beceriler { get; set; }
        public string? Sertifikalar { get; set; }
        public string? Diller { get; set; }
        public string? Referanslar { get; set; }
        public string? Hobiler { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string? DosyaYolu { get; set; }
        public string? AIAnalizi { get; set; }
        public string? CVIcerik { get; set; }
        public List<CVOneriDTO>? Oneriler { get; set; }
    }

    public class CVOneriDTO
    {
        public int Id { get; set; }
        public int CVId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int OncelikSirasi { get; set; }
        public bool Uygulandimi { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }


    
    public class HedefDTO
    {
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
    }
}