using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class OduncIslemDto
    {
        public int OduncIslemId { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciEmail { get; set; }
        public string KitapId { get; set; }
        public string KitapAdi { get; set; }
        public DateTime OduncAlmaTarihi { get; set; }
        public DateTime SonTeslimTarihi { get; set; }
        public bool TeslimEdildi { get; set; }
        public DateTime? TeslimTarihi { get; set; } 

        private bool _isOverdue;
        public bool IsOverdue
        {
            get { return _isOverdue; }
            set
            {
                if (_isOverdue != value)
                {
                    _isOverdue = value;
                    OnPropertyChanged(nameof(IsOverdue));
                }
            }
        }

        // INotifyPropertyChanged uygulaması
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
