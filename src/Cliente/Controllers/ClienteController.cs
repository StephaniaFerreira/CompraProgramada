using Aplicacao.Interfaces;
using Aplicacao.Models;
using Aplicacao.Models.Cliente.Adesao;
using Aplicacao.Models.Cliente.AlterarValorMensal;
using Aplicacao.Models.Cliente.Carteira;
using Cliente.Models.Saida;
using Core.Exceptions;
using Core.Expections;
using Microsoft.AspNetCore.Mvc;

namespace Cliente.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        [HttpPost("adesao")]
        [ProducesResponseType(typeof(AdesaoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public IActionResult AderirProduto([FromBody] AdesaoRequest request)
        {
            try
            {
                var resultado = _service.Aderir(request);
                return Created(" ",resultado);
            }
            catch(RegraNegocioException ex)
            {
                var erro = new ErroResponse(ex.Message,ex.Codigo);

                return BadRequest(erro);
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpPost("{clienteId}/saida")]
        [ProducesResponseType(typeof(SaidaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
        public IActionResult SairProduto(int clienteId)
        {
            try
            {
                var resultado = _service.Sair(clienteId);
                return Created(" ", resultado);
            }
            catch (RegraNegocioException ex)
            {
                var erro = new ErroResponse(ex.Message, ex.Codigo);

                return BadRequest(erro);
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpPut("{clienteId}/valor-mensal")]
        [ProducesResponseType(typeof(AlterarValorMensalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public IActionResult AlterarValorMensal(int clienteId, [FromBody] AlterarValorMensalRequest request)
        {
            try
            {
                var resultado = _service.AlterarValorMensal(clienteId, request);
                return Ok(resultado);
            }
            catch (RegraNegocioException ex)
            {
                var erro = new ErroResponse(ex.Message, ex.Codigo);

                return BadRequest(erro);
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpPut("{clienteId}/carteira")]
        [ProducesResponseType(typeof(CarteiraResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public IActionResult ConsultarCarteira(int clienteId)
        {
            try
            {
                var resultado = _service.ConsultarCarteira(clienteId);
                return Ok(resultado);
            }
            catch(Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpGet("{clienteId}/rentabilidade")]
        [ProducesResponseType(typeof(RentabilidadeDetalhadaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public IActionResult GetRentabilidade(int clienteId)
        {
            try
            {
                var resultado = _service.ObterRentabilidadeDetalhada(clienteId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }
    }
}