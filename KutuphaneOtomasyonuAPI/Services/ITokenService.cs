
using KutuphaneOtomasyonu.API.Entities;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Kullanici user);
    }
}