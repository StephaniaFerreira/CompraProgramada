
namespace Aplicacao.Models.Cliente.Adesao
{
    public sealed record AdesaoRequest
    (
         string? Nome, 
         string? Cpf,
         string? Email,
         decimal ValorMensal
    );
    
}
