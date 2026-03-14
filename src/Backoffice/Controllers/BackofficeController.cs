using Aplicacao.Interfaces;
using Aplicacao.Models;
using Aplicacao.Models.Cesta;
using Aplicacao.Services;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace BackOffice.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class BackofficeController : ControllerBase
    {
        private readonly IBackofficeService _service;
        private readonly ICotacaoService _cotacaoService;

        public BackofficeController(IBackofficeService service, ICotacaoService cotacaoService)
        {
            _service = service;
            _cotacaoService = cotacaoService;
        }

        [HttpPost("cesta")]
        [ProducesResponseType(typeof(CestaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public IActionResult CadastrarOuAlterar([FromBody] CestaRequest request)
        {
            try
            {
                var response = _service.CadastrarOuAlterar(request);
                return CreatedAtAction(nameof(ConsultarAtual), new { }, response);
            }
            catch (CestaException ex)
            {
                return BadRequest(new ErroResponse(ex.Mensagem, ex.Codigo));
            }
        }

        [HttpGet("cesta/atual")]
        [ProducesResponseType(typeof(CestaAtualResponse), StatusCodes.Status200OK)]
        public IActionResult ConsultarAtual()
        {
            try
            {
                var cesta = _service.ConsultarAtual();
                return Ok(cesta);
            }
            catch (CestaException ex)
            {
                return BadRequest(new ErroResponse(ex.Mensagem, ex.Codigo));
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpGet("cesta/historico")]
        [ProducesResponseType(typeof(HistoricoCestaResponse), StatusCodes.Status200OK)]
        public IActionResult Historico()
        {
            try
            {
                var historico = _service.Historico();
                return Ok(historico);
            }
            catch (CestaException ex)
            {
                return BadRequest(new ErroResponse(ex.Mensagem, ex.Codigo));
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpGet("conta-master/custodia")]
        public ActionResult<CustodiaMasterResponse> GetCustodia()
        {
            try
            {
                var result = _service.ConsultarCustodiaMaster();
                return Ok(result);
            }
            catch (CestaException ex)
            {
                return BadRequest(new ErroResponse(ex.Mensagem, ex.Codigo));
            }
            catch (Exception ex)
            {
                var erro = new ErroResponse("Erro inesperado ao processar a solicitação.", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, erro);
            }
        }

        [HttpPost("api/motor/executar-compra")]
        public ActionResult<ExecucaoCompraResponse> ExecutarCompraManual([FromBody] ExecucaoCompraRequest request)
        {
            try
            {
                var response = _service.ExecutarCompraMotor(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
        #region ENDPOINT COTACAO
        [HttpPost("registra/cotacao")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarCotacao([FromBody] CotacaoRequest request)
        {
            try
            {

                await _cotacaoService.ExecutarRegistroArquivo(request.Data);

                var response = new
                {
                    mensagem = "Cotação registrada com sucesso",
                    data = request.Data
                };

                return Created("", response);
            }
            catch (CestaException ex)
            {
                return BadRequest(new ErroResponse(ex.Mensagem, ex.Codigo));
            }
        }

        public class CotacaoRequest
        {
            public DateTime Data { get; set; }
        }
        #endregion
    }
}