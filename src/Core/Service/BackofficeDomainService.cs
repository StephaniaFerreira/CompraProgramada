using Core.Entities;
using Core.Interfaces.Backoffice;


namespace Core.Service
{
    public class BackofficeDomainService : IBackofficeDomainService
    {
        public void DesativarCesta(Cesta cestaAtual, DateTime agora)
        {
            cestaAtual!.Ativa = false;
            cestaAtual.DataDesativacao = agora;
        }
    }
}
