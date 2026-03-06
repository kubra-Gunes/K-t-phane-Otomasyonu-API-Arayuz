using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneOtomasyonu.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    kategori_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kategori_adi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.kategori_id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    kullanici_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sifre_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    kayit_tarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.kullanici_id);
                });

            migrationBuilder.CreateTable(
                name: "Yazarlar",
                columns: table => new
                {
                    yazar_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    biyografi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yazarlar", x => x.yazar_id);
                });

            migrationBuilder.CreateTable(
                name: "Kitaplar",
                columns: table => new
                {
                    kitap_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    kitap_adi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    yazar_id = table.Column<int>(type: "int", nullable: false),
                    kategori_id = table.Column<int>(type: "int", nullable: false),
                    stok_adedi = table.Column<int>(type: "int", nullable: false),
                    mevcut_adet = table.Column<int>(type: "int", nullable: false),
                    yayin_yili = table.Column<int>(type: "int", nullable: false),
                    aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitaplar", x => x.kitap_id);
                    table.ForeignKey(
                        name: "FK_Kitaplar_Kategoriler_kategori_id",
                        column: x => x.kategori_id,
                        principalTable: "Kategoriler",
                        principalColumn: "kategori_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kitaplar_Yazarlar_yazar_id",
                        column: x => x.yazar_id,
                        principalTable: "Yazarlar",
                        principalColumn: "yazar_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OduncKayitlari",
                columns: table => new
                {
                    odunc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kullanici_id = table.Column<int>(type: "int", nullable: false),
                    kitap_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    odunc_alma_tarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    son_teslim_tarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    iade_tarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    durum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OduncKayitlari", x => x.odunc_id);
                    table.ForeignKey(
                        name: "FK_OduncKayitlari_Kitaplar_kitap_id",
                        column: x => x.kitap_id,
                        principalTable: "Kitaplar",
                        principalColumn: "kitap_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OduncKayitlari_Kullanicilar_kullanici_id",
                        column: x => x.kullanici_id,
                        principalTable: "Kullanicilar",
                        principalColumn: "kullanici_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_kategori_adi",
                table: "Kategoriler",
                column: "kategori_adi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitaplar_kategori_id",
                table: "Kitaplar",
                column: "kategori_id");

            migrationBuilder.CreateIndex(
                name: "IX_Kitaplar_yazar_id",
                table: "Kitaplar",
                column: "yazar_id");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_email",
                table: "Kullanicilar",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OduncKayitlari_kitap_id",
                table: "OduncKayitlari",
                column: "kitap_id");

            migrationBuilder.CreateIndex(
                name: "IX_OduncKayitlari_kullanici_id",
                table: "OduncKayitlari",
                column: "kullanici_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OduncKayitlari");

            migrationBuilder.DropTable(
                name: "Kitaplar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Yazarlar");
        }
    }
}
