using System.ComponentModel.DataAnnotations;

namespace Aplicacao.Models.Cesta
{
    public record CestaRequest(
    [Required] string Nome,
    [Required, MinLength(5), MaxLength(5)] List<ItemRequest> Itens
    );

    public record ItemRequest(
        [Required] string Ticker,
        [Required] decimal Percentual
    );
}
