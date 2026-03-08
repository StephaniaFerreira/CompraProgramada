using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class ArquivoLido
    {
        [Key]
        public int Id { get; set; }
        public string NomeArquivo { get; set; } = null!;
        public DateTime DataLeitura { get; set; }
    }
}
