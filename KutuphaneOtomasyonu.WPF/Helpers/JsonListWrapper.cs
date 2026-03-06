using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Helpers
{
    // JSON'daki "$id" ve "$values" yapısını temsil eden yardımcı sınıf
    public class JsonListWrapper<T>
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        [JsonPropertyName("$values")]
        public List<T> Values { get; set; }
    }
}