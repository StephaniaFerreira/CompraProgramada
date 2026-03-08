using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ContaGrafica
    {
        [Key]
        public int Id { get; set; }

        public  string? NumeroConta { get; set; }
        public string? Tipo { get; set; }
        public DateTime DataCriacao { get; set; }

        [ForeignKey("ClienteCadastro")]
        public int ClienteId { get; set; }

        public ClienteCadastro? Cliente { get; set; }
    }
}
