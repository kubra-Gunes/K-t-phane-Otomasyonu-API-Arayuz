// KutuphaneOtomasyonu.API.Interfaces/IOduncKayitRepository.cs
using KutuphaneOtomasyonu.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface IOduncKayitRepository : IRepository<OduncKayit>
    {
        Task<IEnumerable<OduncKayit>> GetBorrowRecordsByUserIdAsync(int userId);

        Task<IEnumerable<OduncKayit>> GetOverdueBorrowRecordsAsync();
    }
}