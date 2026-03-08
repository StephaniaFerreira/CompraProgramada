using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class ClienteCadastro
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal ValorMensal { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataAdesao { get; set; }
        public DateTime? DataSaida { get; set; }

        public ContaGrafica ContaGrafica { get; set; } = null!;
    }
}
