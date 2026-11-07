using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using LojaRetro.Api.Services;
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
    /// Lista as movimentações, da mais recente para a mais antiga.
    /// </summary>
    /// <param name="jogoId">Opcional: mostra só as movimentações deste jogo.</param>
    /// <returns>Lista de movimentações.</returns>
    /// <response code="200">Lista de movimentações (pode vir vazia).</response>
    /// <response code="400">Id do jogo em formato inválido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Movimentacao>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<Movimentacao>>> Listar([FromQuery] string? jogoId)
    {
        if (jogoId is null)
            return Ok(await _repositorio.ListarAsync());

        if (!ObjectId.TryParse(jogoId, out _))
            return BadRequest(new { mensagem = "Id do jogo inválido." });

        return Ok(await _repositorio.ListarPorJogoAsync(jogoId));
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
    [HttpPost]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Movimentacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
