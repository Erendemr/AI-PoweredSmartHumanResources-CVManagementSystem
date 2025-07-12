using System.ComponentModel.DataAnnotations;

namespace InsanK.DTOs
{
    public class MesajGonderDTO
    {
        [Required]
        public int AliciId { get; set; }

        [Required]
        [StringLength(100)]
        public string Konu { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Icerik { get; set; } = string.Empty;
    }

    public class MesajDetayDTO
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public string GonderenKullaniciAdi { get; set; } = string.Empty;
        public int AliciId { get; set; }
        public string AliciKullaniciAdi { get; set; } = string.Empty;
        public string Konu { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public DateTime GondermeTarihi { get; set; }
        public bool Okundu { get; set; }
        public DateTime? OkunmaTarihi { get; set; }
    }

    public class MesajListeDTO
    {
        public int Id { get; set; }
        public int GonderenId { get; set; }
        public string GonderenKullaniciAdi { get; set; } = string.Empty;
        public int AliciId { get; set; }
        public string AliciKullaniciAdi { get; set; } = string.Empty;
        public string Konu { get; set; } = string.Empty;
        public DateTime GondermeTarihi { get; set; }
        public bool Okundu { get; set; }
    }
}
