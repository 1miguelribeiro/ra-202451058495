using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using LojaRetro.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LojaRetro.Api.Controllers;

/// <summary>
/// Entradas e saídas do estoque.
/// </summary>
[ApiController]
[Route("api/movimentacoes")]
[Produces("application/json")]
[Tags("Movimentações")]
public class MovimentacoesController : ControllerBase
{
    private readonly IMovimentacaoRepository _repositorio;
    private readonly EstoqueService _estoque;

    public MovimentacoesController(IMovimentacaoRepository repositorio, EstoqueService estoque)
    {
        _repositorio = repositorio;
        _estoque = estoque;
    }

    /// <summary>
    /// Lista as movimentações, da mais recente para a mais antiga, por página.
    /// </summary>
    /// <param name="jogoId">Opcional: mostra só as movimentações deste jogo.</param>
    /// <param name="pagina">Número da página</param>
    /// <param name="tamanho">Itens por página</param>
    /// <returns>Uma página de movimentações.</returns>
    /// <response code="200">Página de movimentações (pode vir vazia).</response>
    /// <response code="400">Id do jogo, página ou tamanho inválido.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagina<Movimentacao>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pagina<Movimentacao>>> Listar(
        [FromQuery] string? jogoId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 10)
    {
        if (jogoId is not null && !ObjectId.TryParse(jogoId, out _))
            return BadRequest(new { mensagem = "Id do jogo inválido." });

        if (pagina < 1 || tamanho < 1 || tamanho > 50)
            return BadRequest(new { mensagem = "A página começa em 1 e o tamanho deve ser de 1 a 50." });

        return Ok(await _repositorio.ListarAsync(jogoId, pagina, tamanho));
    }

    /// <summary>
    /// Busca uma movimentação pelo id.
    /// </summary>
    /// <param name="id">Id da movimentação.</param>
    /// <returns>Dados da movimentação encontrada.</returns>
    /// <response code="200">Movimentação encontrada.</response>
    /// <response code="400">Id em formato inválido.</response>
    /// <response code="404">Movimentação não encontrada.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movimentacao>> BuscarPorId(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest(new { mensagem = "Id inválido." });

        var movimentacao = await _repositorio.BuscarPorIdAsync(id);
        if (movimentacao is null)
            return NotFound(new { mensagem = "Movimentação não encontrada." });

        return Ok(movimentacao);
    }

    /// <summary>
    /// Registra uma entrada ou saída e atualiza o estoque do jogo.
    /// </summary>
    /// <remarks>
    /// Entrada soma no estoque; saída subtrai. Não é possível vender mais do que há em estoque.
    /// </remarks>
    /// <param name="request">Dados da movimentação.</param>
    /// <returns>Movimentação registrada.</returns>
    /// <response code="201">Movimentação registrada.</response>
    /// <response code="400">Quantidade inválida, id inválido ou estoque insuficiente.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="401">Sem token ou token inválido.</response>
    [HttpPost]
    [Authorize(Roles = Perfis.Admin + "," + Perfis.Funcionario)]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Movimentacao>> Registrar(MovimentacaoRequest request)
    {
        try
        {
            var movimentacao = await _estoque.RegistrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = movimentacao.Id }, movimentacao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Corrige uma movimentação e recalcula o estoque do jogo.
    /// </summary>
    /// <remarks>
    /// O efeito antigo é desfeito e o novo é aplicado. O estoque não pode ficar negativo.
    /// </remarks>
    /// <param name="id">Id da movimentação.</param>
    /// <param name="request">Novos dados da movimentação.</param>
    /// <returns>Movimentação corrigida.</returns>
    /// <response code="200">Movimentação corrigida.</response>
    /// <response code="400">Id inválido, quantidade inválida ou estoque insuficiente.</response>
    /// <response code="404">Movimentação ou jogo não encontrado.</response>
    /// <response code="401">Sem token ou token inválido.</response>
    /// <response code="403">Perfil sem permissão (somente admin).</response>
    [HttpPut("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Movimentacao>> Atualizar(string id, AtualizarMovimentacaoRequest request)
    {
        try
        {
            var movimentacao = await _estoque.AtualizarAsync(id, request);
            return Ok(movimentacao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma movimentação e desfaz o efeito dela no estoque.
    /// </summary>
    /// <param name="id">Id da movimentação.</param>
    /// <returns>Nenhum conteúdo.</returns>
    /// <response code="204">Movimentação removida.</response>
    /// <response code="400">Id inválido ou o estoque ficaria negativo.</response>
    /// <response code="404">Movimentação ou jogo não encontrado.</response>
    /// <response code="401">Sem token ou token inválido.</response>
    /// <response code="403">Perfil sem permissão (somente admin).</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Remover(string id)
    {
        try
        {
            await _estoque.RemoverAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
