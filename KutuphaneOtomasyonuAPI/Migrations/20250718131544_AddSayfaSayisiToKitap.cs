using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KutuphaneOtomasyonu.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSayfaSayisiToKitap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sayfa_sayisi", // Veritabanı sütun adı
                table: "Kitaplar",      // Hangi tabloya eklenecek
                type: "int",            // Sütun veri tipi
                nullable: false,        // NULL değer alabilir mi?
                defaultValue: 0);       // Varsayılan değer (eğer nullable=false ise ve mevcut verilerde değer yoksa)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sayfa_sayisi",
                table: "Kitaplar");
        }
    }
}