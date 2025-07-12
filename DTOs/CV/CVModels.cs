using System.ComponentModel.DataAnnotations;


//Kullanılmıyor
namespace InsanK.DTOs.CV
{
    public class CVFormDataDTO
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string? Adres { get; set; }
        public DateTime? DogumTarihi { get; set; }
        
        public List<EgitimDTO> Egitim { get; set; } = new List<EgitimDTO>();
        
        public List<DeneyimDTO> Deneyim { get; set; } = new List<DeneyimDTO>();
        
        public string? Beceriler { get; set; }
    }
    
    public class CVAnalizDTO
    {
        public KisiselBilgiDTO KisiselBilgiler { get; set; } = new KisiselBilgiDTO();
        public List<EgitimDTO> Egitim { get; set; } = new List<EgitimDTO>();
        public List<DeneyimDTO> Deneyim { get; set; } = new List<DeneyimDTO>();
        public List<string> Beceriler { get; set; } = new List<string>();
        
        public List<TutarsizlikDTO> Tutarsizliklar { get; set; } = new List<TutarsizlikDTO>();
        
        public int GuvenSkoru { get; set; }
    }
    
    public class TutarsizlikDTO
    {
        public string Alan { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string OnerilenDeger { get; set; } = string.Empty;
    }
    
    public class KisiselBilgiDTO
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string? Adres { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string? LinkedIn { get; set; }
        public string? GitHub { get; set; }
        public string? WebSitesi { get; set; }
    }
    
    public class EgitimDTO
    {
        public string Okul { get; set; } = string.Empty;
        public string? Bolum { get; set; }
        public string? Derece { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool DevamEdiyorMu { get; set; }
        public string? GPA { get; set; }
        public string? Detay { get; set; }
    }
    
    public class DeneyimDTO
    {
        public string Sirket { get; set; } = string.Empty;
        public string Pozisyon { get; set; } = string.Empty;
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool DevamEdiyorMu { get; set; }
        public string? Lokasyon { get; set; }
        public string? Aciklama { get; set; }
    }
    
    public class CVYuklemeDTO
    {
        public int CVDosyaId { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public DateTime YuklemeTarihi { get; set; }
        public bool IslendiMi { get; set; }
        public bool AnalizEdildiMi { get; set; }
        public string? HataDetay { get; set; }
    }
    
    public class CVAnalizIstekDTO
    {
        [Required]
        public int CVDosyaId { get; set; }
        
        public CVFormDataDTO? FormVerisi { get; set; }
    }
}