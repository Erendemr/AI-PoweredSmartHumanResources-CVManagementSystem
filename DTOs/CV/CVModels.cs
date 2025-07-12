using System.ComponentModel.DataAnnotations;


//Kullanmıyorum
namespace InsanK.DTOs.CV
{
    // Formdan doldurulan bilgiler
    public class CVFormDataDTO
    {
        // Kişisel bilgiler
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string? Adres { get; set; }
        public DateTime? DogumTarihi { get; set; }
        
        // Temel eğitim bilgileri
        public List<EgitimDTO> Egitim { get; set; } = new List<EgitimDTO>();
        
        // Temel iş deneyimi bilgileri
        public List<DeneyimDTO> Deneyim { get; set; } = new List<DeneyimDTO>();
        
        // Beceriler (virgülle ayrılmış)
        public string? Beceriler { get; set; }
    }
    
    // AI analiz sonuçları için model
    public class CVAnalizDTO
    {
        public KisiselBilgiDTO KisiselBilgiler { get; set; } = new KisiselBilgiDTO();
        public List<EgitimDTO> Egitim { get; set; } = new List<EgitimDTO>();
        public List<DeneyimDTO> Deneyim { get; set; } = new List<DeneyimDTO>();
        public List<string> Beceriler { get; set; } = new List<string>();
        
        // Analiz sonucu tespit edilen tutarsızlıklar
        public List<TutarsizlikDTO> Tutarsizliklar { get; set; } = new List<TutarsizlikDTO>();
        
        // AI analiz güven skoru (0-100)
        public int GuvenSkoru { get; set; }
    }
    
    // Tutarsızlık modeli
    public class TutarsizlikDTO
    {
        public string Alan { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string OnerilenDeger { get; set; } = string.Empty;
    }
    
    // Kişisel bilgiler 
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
    
    // Eğitim bilgileri
    public class EgitimDTO
    {
        public string Okul { get; set; } = string.Empty;
        public string? Bolum { get; set; }
        public string? Derece { get; set; } // Lisans, Yüksek Lisans vb.
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool DevamEdiyorMu { get; set; }
        public string? GPA { get; set; }
        public string? Detay { get; set; }
    }
    
    // İş deneyimi bilgileri
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
    
    // CV Yükleme yanıtı
    public class CVYuklemeDTO
    {
        public int CVDosyaId { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public DateTime YuklemeTarihi { get; set; }
        public bool IslendiMi { get; set; }
        public bool AnalizEdildiMi { get; set; }
        public string? HataDetay { get; set; }
    }
    
    // CV Analiz İsteği DTO
    public class CVAnalizIstekDTO
    {
        [Required]
        public int CVDosyaId { get; set; }
        
        // Opsiyonel form verisi - kullanıcının önceden doldurduğu veriler varsa
        public CVFormDataDTO? FormVerisi { get; set; }
    }
}