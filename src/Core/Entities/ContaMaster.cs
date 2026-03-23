using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class ContaMaster
    {
        [Key]
        public int Id { get; set; }
        public string NumeroConta { get; set; } = null!;
        public string Tipo { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<CustodiaMaster> ItensCustodia { get; set; } = new List<CustodiaMaster>();
    }
}
