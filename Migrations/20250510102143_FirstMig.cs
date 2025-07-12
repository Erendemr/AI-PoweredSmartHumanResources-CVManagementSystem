using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsanK.Migrations
{
    /// <inheritdoc />
    public partial class FirstMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    KullaniciTipi = table.Column<int>(type: "int", nullable: false),
                    SirketAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SirketAciklamasi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CVDosyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DosyaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DosyaUzantisi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DosyaIcerigi = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IslenmisMetin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IslendiMi = table.Column<bool>(type: "bit", nullable: false),
                    AnalizEdildiMi = table.Column<bool>(type: "bit", nullable: false),
                    HataDetay = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    YuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVDosyalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVDosyalar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CVler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Meslek = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HedefPozisyon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HedefSirket = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Egitim = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Deneyim = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Beceriler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sertifikalar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Diller = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Referanslar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Hobiler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DosyaYolu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AIAnalizi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CVIcerik = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ilanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Lokasyon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PozisyonTipi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmanAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TecrubeSeviyesi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GerekliYetkinlikler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TercihEdilenYetkinlikler = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinMaas = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxMaas = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YayinlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonBasvuruTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    EgitimSeviyesi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SirketLokasyonu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilanlar_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GonderenId = table.Column<int>(type: "int", nullable: false),
                    AliciId = table.Column<int>(type: "int", nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    GondermeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Okundu = table.Column<bool>(type: "bit", nullable: false),
                    OkunmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kullanicilar_AliciId",
                        column: x => x.AliciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kullanicilar_GonderenId",
                        column: x => x.GonderenId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CVAnalizler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVDosyaId = table.Column<int>(type: "int", nullable: false),
                    KisiselBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Egitim = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deneyim = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beceriler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tutarsizliklar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciOnayli = table.Column<bool>(type: "bit", nullable: false),
                    GuvenSkoru = table.Column<int>(type: "int", nullable: false),
                    AnalizTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVAnalizler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVAnalizler_CVDosyalar_CVDosyaId",
                        column: x => x.CVDosyaId,
                        principalTable: "CVDosyalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CVOnerileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OncelikSirasi = table.Column<int>(type: "int", nullable: false),
                    Uygulandimi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVOnerileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVOnerileri_CVler_CVId",
                        column: x => x.CVId,
                        principalTable: "CVler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Basvurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    BasvuruTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OnYazisi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    NotlarGeribildirimi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CVId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basvurular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Basvurular_CVler_CVId",
                        column: x => x.CVId,
                        principalTable: "CVler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Basvurular_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Basvurular_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Kullanicilar",
                columns: new[] { "Id", "Ad", "Email", "IsAdmin", "KayitTarihi", "KullaniciAdi", "KullaniciTipi", "Sifre", "SirketAciklamasi", "SirketAdi", "Soyad", "Telefon" },
                values: new object[] { 1, "Eren", "admin@insankaynaklari.com", true, new DateTime(2025, 5, 10, 13, 21, 43, 360, DateTimeKind.Local).AddTicks(8912), "eren", 2, "123", null, null, "Admin", null });

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_CVId",
                table: "Basvurular",
                column: "CVId");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_IlanId",
                table: "Basvurular",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Basvurular_KullaniciId",
                table: "Basvurular",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CVAnalizler_CVDosyaId",
                table: "CVAnalizler",
                column: "CVDosyaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVDosyalar_KullaniciId",
                table: "CVDosyalar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_CVler_KullaniciId",
                table: "CVler",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVOnerileri_CVId",
                table: "CVOnerileri",
                column: "CVId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_KullaniciId",
                table: "Ilanlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciId",
                table: "Mesajlar",
                column: "AliciId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar",
                column: "GonderenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Basvurular");

            migrationBuilder.DropTable(
                name: "CVAnalizler");

            migrationBuilder.DropTable(
                name: "CVOnerileri");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropTable(
                name: "Ilanlar");

            migrationBuilder.DropTable(
                name: "CVDosyalar");

            migrationBuilder.DropTable(
                name: "CVler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
