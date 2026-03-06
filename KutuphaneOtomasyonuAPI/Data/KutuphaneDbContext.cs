
using KutuphaneOtomasyonu.API.Entities;
using Microsoft.EntityFrameworkCore;



namespace KutuphaneOtomasyonu.API.Data

{
    public class KutuphaneDbContext : DbContext 
    {
        public KutuphaneDbContext(DbContextOptions<KutuphaneDbContext> options) : base(options)

        {

        }


        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Yazar> Yazarlar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<OduncKayit> OduncKayitlari { get; set; }
        public DbSet<KitapYorumu> KitapYorumlari { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kitap>().ToTable("Kitaplar");
            modelBuilder.Entity<Yazar>().ToTable("Yazarlar");
            modelBuilder.Entity<Kategori>().ToTable("Kategoriler");
            modelBuilder.Entity<Kullanici>().ToTable("Kullanicilar");
            modelBuilder.Entity<OduncKayit>().ToTable("OduncKayitlari");
           


            modelBuilder.Entity<Kitap>()

              .Property(k => k.KitapId)

              .HasColumnName("kitap_id")

              .IsRequired();



            modelBuilder.Entity<Yazar>()

              .Property(y => y.YazarId)

              .HasColumnName("yazar_id")

              .IsRequired();



            modelBuilder.Entity<Kitap>()
              .HasOne(k => k.Yazar)
              .WithMany(y => y.Kitaplari)
              .HasForeignKey(k => k.YazarId)
              .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Kitap>()

              .HasOne(k => k.Kategori)

              .WithMany(c => c.Kitaplari)

              .HasForeignKey(k => k.KategoriId)

              .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<OduncKayit>()

              .HasOne(oc => oc.Kullanici)

              .WithMany(u => u.OduncKayitlari)

              .HasForeignKey(oc => oc.KullaniciId)

              .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<OduncKayit>()

              .HasOne(oc => oc.Kitap)

              .WithMany(k => k.OduncKayitlari)

              .HasForeignKey(oc => oc.KitapId)

              .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Kategori>()

              .HasIndex(k => k.KategoriAdi)

              .IsUnique();



            modelBuilder.Entity<Kullanici>()

              .HasIndex(u => u.Email)

              .IsUnique();



            modelBuilder.Entity<Kullanici>()

              .Property(u => u.SifreHash)

              .HasColumnName("sifre_hash")

              .IsRequired()

              .HasMaxLength(255);



            modelBuilder.Entity<Kullanici>()

              .Property(u => u.Rol)

              .HasColumnName("rol")

              .IsRequired()

              .HasMaxLength(50);

            modelBuilder.Entity<KitapYorumu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Puan).IsRequired();
                entity.Property(e => e.YorumMetni).HasMaxLength(500);
                entity.Property(e => e.YorumTarihi).IsRequired();

                // KitapYorumu - Kitap ilişkisi (bir kitapta çok yorum olabilir)
                entity.HasOne(d => d.Kitap)
                      .WithMany(p => p.KitapYorumlari) // Kitap varlığında bir KitapYorumlari koleksiyonu tanımlayacağız
                      .HasForeignKey(d => d.KitapId)
                      .OnDelete(DeleteBehavior.Cascade); // Kitap silinirse yorumları da silinsin

                // KitapYorumu - Kullanici ilişkisi (bir kullanıcının çok yorumu olabilir)
                entity.HasOne(d => d.Kullanici)
                      .WithMany(p => p.KitapYorumlari) // Kullanici varlığında bir KitapYorumlari koleksiyonu tanımlayacağız
                      .HasForeignKey(d => d.KullaniciId)
                      .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silinirse yorumları silinmesin, sadece KullaniciId null olsun veya hata versin. (Kendi tercihinize göre)
            });

     

        }


    }

}