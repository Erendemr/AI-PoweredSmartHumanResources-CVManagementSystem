using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsanK.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CVAnalizler");

            migrationBuilder.DropTable(
                name: "CVDosyalar");

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "KayitTarihi",
                value: new DateTime(2025, 6, 17, 21, 54, 56, 170, DateTimeKind.Local).AddTicks(8858));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CVDosyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    AnalizEdildiMi = table.Column<bool>(type: "bit", nullable: false),
                    DosyaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DosyaIcerigi = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DosyaUzantisi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HataDetay = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IslendiMi = table.Column<bool>(type: "bit", nullable: false),
                    IslenmisMetin = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "CVAnalizler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVDosyaId = table.Column<int>(type: "int", nullable: false),
                    AnalizTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Beceriler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deneyim = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Egitim = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuvenSkoru = table.Column<int>(type: "int", nullable: false),
                    KisiselBilgiler = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciOnayli = table.Column<bool>(type: "bit", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tutarsizliklar = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                column: "KayitTarihi",
                value: new DateTime(2025, 5, 10, 13, 21, 43, 360, DateTimeKind.Local).AddTicks(8912));

            migrationBuilder.CreateIndex(
                name: "IX_CVAnalizler_CVDosyaId",
                table: "CVAnalizler",
                column: "CVDosyaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVDosyalar_KullaniciId",
                table: "CVDosyalar",
                column: "KullaniciId");
        }
    }
}
