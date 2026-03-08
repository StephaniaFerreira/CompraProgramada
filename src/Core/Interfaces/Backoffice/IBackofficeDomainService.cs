using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Backoffice
{
    public interface IBackofficeDomainService
    {
        void DesativarCesta(Cesta cestaAtual, DateTime agora);
    }
}
