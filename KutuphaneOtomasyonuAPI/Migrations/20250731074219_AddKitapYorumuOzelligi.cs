using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneOtomasyonu.API.Migrations
{
    /// <inheritdoc />
    public partial class AddKitapYorumuOzelligi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KitapYorumlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitapId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    YorumMetni = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    YorumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitapYorumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitapYorumlari_Kitaplar_KitapId",
                        column: x => x.KitapId,
                        principalTable: "Kitaplar",
                        principalColumn: "kitap_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitapYorumlari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "kullanici_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KitapYorumlari_KitapId",
                table: "KitapYorumlari",
                column: "KitapId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapYorumlari_KullaniciId",
                table: "KitapYorumlari",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KitapYorumlari");
        }
    }
}
