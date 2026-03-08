using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class CustodiaMaster
    {
        [Key] 
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal ValorAtual { get; set; }
        public string Origem { get; set; } = null!;
        public int ContaMasterId { get; set; }
        public virtual ContaMaster ContaMaster { get; set; } = null!;
    }
}
