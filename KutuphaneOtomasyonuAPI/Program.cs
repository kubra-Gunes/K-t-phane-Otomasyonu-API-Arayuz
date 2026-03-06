// Program.cs
using KutuphaneOtomasyonu.API.Data;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyonu.API.Interfaces;
using KutuphaneOtomasyonu.API.Repositories;
using KutuphaneOtomasyonu.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// API controller'larýný ekle ve JSON ayarlarýný yap
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Döngüsel referans hatasýný önlemek için bu ayar zorunludur.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        // Opsiyonel: JSON çýktýsýnýn daha okunabilir olmasý için Indented özelliði
        options.JsonSerializerOptions.WriteIndented = true;
    });

// API Explorer ve Swagger ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabaný baðlantýsýný ve Context'i ekle
builder.Services.AddDbContext<KutuphaneDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KutuphaneDb")));

// Dependency Injection için Repository ve Service'leri ekle
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IKitapRepository, KitapRepository>();
builder.Services.AddScoped<IYazarRepository, YazarRepository>();
builder.Services.AddScoped<IKategoriRepository, KategoriRepository>();
builder.Services.AddScoped<IKullaniciRepository, KullaniciRepository>();
builder.Services.AddScoped<IOduncKayitRepository, OduncKayitRepository>();

builder.Services.AddScoped<ITokenService, TokenService>(); // TokenService'i ekle

// JWT Kimlik Doðrulama servisini ekle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Token'ý imzalayan anahtarýn doðrulanmasýný saðlar.
            ValidateIssuerSigningKey = true,
            // appsettings.json'dan aldýðýmýz anahtarý kullanýr.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

            // Token'ýn kaynaðýný (Issuer) doðrular.
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Token'ýn hedef kitlesini (Audience) doðrular.
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Token'ýn geçerlilik süresini doðrular.
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Token'ýn geçerlilik süresi için esneklik tanýmaz.

            // Bu ayar, token'daki rol bilgisini doðru bir þekilde okumasýný saðlar.
            // Zaten varsayýlan olarak doðru, ancak açýkça belirtmek iyi bir pratiktir.
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

// Yetkilendirme servisini ekle
builder.Services.AddAuthorization();


var app = builder.Build();

// HTTP istek iþlem hattýný yapýlandýr
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Kimlik doðrulama middleware'ini ekle. Bu, UseAuthorization'dan önce GELMELÝDÝR.
app.UseAuthentication();

// Yetkilendirme middleware'ini ekle
app.UseAuthorization();

app.MapControllers();

app.Run();
