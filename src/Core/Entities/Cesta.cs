using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Cesta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        public bool Ativa { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataDesativacao { get; set; }

        public List<ItemCesta> Itens { get; set; } = new List<ItemCesta>();
    }
}
