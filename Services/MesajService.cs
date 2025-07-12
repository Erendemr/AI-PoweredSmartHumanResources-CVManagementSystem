using InsanK.Data;
using InsanK.DTOs;
using InsanK.Models;
using Microsoft.EntityFrameworkCore;

namespace InsanK.Services
{
    public class MesajService
    {
        private readonly ApplicationDbContext _context;

        public MesajService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MesajDetayDTO?> MesajGonder(int gonderenId, MesajGonderDTO model)
        {
            Console.WriteLine($"MesajService: MesajGonder başladı - Gönderen={gonderenId}, Alıcı={model.AliciId}, Konu={model.Konu}");
            
            var gonderen = await _context.Kullanicilar.FindAsync(gonderenId);
            var alici = await _context.Kullanicilar.FindAsync(model.AliciId);

            if (gonderen == null)
            {
                Console.WriteLine($"MesajService: Gönderen kullanıcı (ID: {gonderenId}) bulunamadı");
                return null;
            }
            
            if (alici == null)
            {
                Console.WriteLine($"MesajService: Alıcı kullanıcı (ID: {model.AliciId}) bulunamadı");
                return null;
            }

            if (gonderenId == model.AliciId)
            {
                Console.WriteLine("MesajService: Gönderen ve alıcı aynı kişi olamaz");
                return null;
            }

            var mesaj = new Mesaj
            {
                GonderenId = gonderenId,
                AliciId = model.AliciId,
                Konu = model.Konu,
                Icerik = model.Icerik,
                GondermeTarihi = DateTime.Now,
                Okundu = false
            };

            _context.Mesajlar.Add(mesaj);
            await _context.SaveChangesAsync();

            return await MesajDetayGetir(mesaj.Id, gonderenId);
        }

        public async Task<MesajDetayDTO?> MesajDetayGetir(int id, int kullaniciId)
        {
            var mesaj = await _context.Mesajlar
                .Include(m => m.GonderenKullanici)
                .Include(m => m.AliciKullanici)
                .FirstOrDefaultAsync(m => m.Id == id && (m.GonderenId == kullaniciId || m.AliciId == kullaniciId));

            if (mesaj == null)
            {
                return null;
            }

            if (mesaj.AliciId == kullaniciId && !mesaj.Okundu)
            {
                mesaj.Okundu = true;
                mesaj.OkunmaTarihi = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return new MesajDetayDTO
            {
                Id = mesaj.Id,
                GonderenId = mesaj.GonderenId,
                GonderenKullaniciAdi = mesaj.GonderenKullanici.KullaniciAdi,
                AliciId = mesaj.AliciId,
                AliciKullaniciAdi = mesaj.AliciKullanici.KullaniciAdi,
                Konu = mesaj.Konu,
                Icerik = mesaj.Icerik,
                GondermeTarihi = mesaj.GondermeTarihi,
                Okundu = mesaj.Okundu,
                OkunmaTarihi = mesaj.OkunmaTarihi
            };
        }

        public async Task<List<MesajListeDTO>> GelenMesajlariListele(int kullaniciId)
        {
            return await _context.Mesajlar
                .Include(m => m.GonderenKullanici)
                .Include(m => m.AliciKullanici)
                .Where(m => m.AliciId == kullaniciId)
                .OrderByDescending(m => m.GondermeTarihi)
                .Select(m => new MesajListeDTO
                {
                    Id = m.Id,
                    GonderenId = m.GonderenId,
                    GonderenKullaniciAdi = m.GonderenKullanici.KullaniciAdi,
                    AliciId = m.AliciId,
                    AliciKullaniciAdi = m.AliciKullanici.KullaniciAdi,
                    Konu = m.Konu,
                    GondermeTarihi = m.GondermeTarihi,
                    Okundu = m.Okundu
                })
                .ToListAsync();
        }

        public async Task<List<MesajListeDTO>> GonderilenMesajlariListele(int kullaniciId)
        {
            return await _context.Mesajlar
                .Include(m => m.GonderenKullanici)
                .Include(m => m.AliciKullanici)
                .Where(m => m.GonderenId == kullaniciId)
                .OrderByDescending(m => m.GondermeTarihi)
                .Select(m => new MesajListeDTO
                {
                    Id = m.Id,
                    GonderenId = m.GonderenId,
                    GonderenKullaniciAdi = m.GonderenKullanici.KullaniciAdi,
                    AliciId = m.AliciId,
                    AliciKullaniciAdi = m.AliciKullanici.KullaniciAdi,
                    Konu = m.Konu,
                    GondermeTarihi = m.GondermeTarihi,
                    Okundu = m.Okundu
                })
                .ToListAsync();
        }

        public async Task<bool> MesajSil(int id, int kullaniciId)
        {
            var mesaj = await _context.Mesajlar
                .FirstOrDefaultAsync(m => m.Id == id && (m.GonderenId == kullaniciId || m.AliciId == kullaniciId));

            if (mesaj == null)
            {
                return false;
            }

            _context.Mesajlar.Remove(mesaj);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TumMesajlariOkunduIsaretle(int kullaniciId)
        {
            var okunmamisMesajlar = await _context.Mesajlar
                .Where(m => m.AliciId == kullaniciId && !m.Okundu)
                .ToListAsync();

            if (!okunmamisMesajlar.Any())
            {
                return false;
            }

            foreach (var mesaj in okunmamisMesajlar)
            {
                mesaj.Okundu = true;
                mesaj.OkunmaTarihi = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> OkunmamisMesajSayisi(int kullaniciId)
        {
            return await _context.Mesajlar
                .CountAsync(m => m.AliciId == kullaniciId && !m.Okundu);
        }

        public async Task<List<MesajDetayDTO>> KullaniciMesajlariGetir(int kullaniciId, int digerKullaniciId)
        {
            var mesajlar = await _context.Mesajlar
                .Include(m => m.GonderenKullanici)
                .Include(m => m.AliciKullanici)
                .Where(m => (m.GonderenId == kullaniciId && m.AliciId == digerKullaniciId) ||
                           (m.GonderenId == digerKullaniciId && m.AliciId == kullaniciId))
                .OrderBy(m => m.GondermeTarihi)
                .Select(m => new MesajDetayDTO
                {
                    Id = m.Id,
                    GonderenId = m.GonderenId,
                    GonderenKullaniciAdi = m.GonderenKullanici.KullaniciAdi,
                    AliciId = m.AliciId,
                    AliciKullaniciAdi = m.AliciKullanici.KullaniciAdi,
                    Konu = m.Konu,
                    Icerik = m.Icerik,
                    GondermeTarihi = m.GondermeTarihi,
                    Okundu = m.Okundu,
                    OkunmaTarihi = m.OkunmaTarihi
                })
                .ToListAsync();

            var okunmamisMesajlar = await _context.Mesajlar
                .Where(m => m.AliciId == kullaniciId && m.GonderenId == digerKullaniciId && !m.Okundu)
                .ToListAsync();

            if (okunmamisMesajlar.Any())
            {
                foreach (var mesaj in okunmamisMesajlar)
                {
                    mesaj.Okundu = true;
                    mesaj.OkunmaTarihi = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return mesajlar;
        }
    }
}
