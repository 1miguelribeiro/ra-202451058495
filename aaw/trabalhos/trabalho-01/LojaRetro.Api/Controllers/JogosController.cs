using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LojaRetro.Api.Controllers;

/// <summary>
/// Cadastro de jogos da loja.
/// </summary>
[ApiController]
[Route("api/jogos")]
[Produces("application/json")]
[Tags("Jogos")]
public class JogosController : ControllerBase
{
    private readonly JogoRepository _repositorio;

    public JogosController(JogoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>
    /// Lista todos os jogos cadastrados.
    /// </summary>
    /// <returns>Lista de jogos.</returns>
    /// <response code="200">Lista de jogos (pode vir vazia).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Jogo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Jogo>>> Listar()
    {
        var jogos = await _repositorio.ListarAsync();
        return Ok(jogos);
    }

    /// <summary>
    /// Busca um jogo pelo id.
    /// </summary>
    /// <param name="id">Id do jogo (24 caracteres, gerado pelo MongoDB).</param>
    /// <returns>Dados do jogo encontrado.</returns>
    /// <response code="200">Jogo encontrado.</response>
    /// <response code="400">Id em formato inválido.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Jogo>> BuscarPorId(string id)
    {
        if (!IdValido(id))
            return BadRequest(new { mensagem = "Id inválido." });

        var jogo = await _repositorio.BuscarPorIdAsync(id);
        if (jogo is null)
            return NotFound(new { mensagem = "Jogo não encontrado." });

        return Ok(jogo);
    }

    /// <summary>
    /// Cadastra um jogo novo. O estoque começa em zero.
    /// </summary>
    /// <param name="request">Dados do jogo.</param>
    /// <returns>Jogo recém-criado.</returns>
    /// <response code="201">Jogo criado.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Jogo>> Criar(JogoRequest request)
    {
        var jogo = new Jogo
        {
            Nome = request.Nome,
            Console = request.Console,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            QuantidadeEmEstoque = 0
        };

        await _repositorio.CriarAsync(jogo);
        return CreatedAtAction(nameof(BuscarPorId), new { id = jogo.Id }, jogo);
    }

    /// <summary>
    /// Altera nome, console, ano e preço de um jogo. A quantidade em estoque não muda por aqui.
    /// </summary>
    /// <param name="id">Id do jogo.</param>
    /// <param name="request">Novos dados do jogo.</param>
    /// <returns>Jogo com os dados atualizados.</returns>
    /// <response code="200">Jogo alterado.</response>
    /// <response code="400">Id ou dados inválidos.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Jogo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Jogo>> Atualizar(string id, JogoRequest request)
    {
        if (!IdValido(id))
            return BadRequest(new { mensagem = "Id inválido." });

        var jogo = await _repositorio.BuscarPorIdAsync(id);
        if (jogo is null)
            return NotFound(new { mensagem = "Jogo não encontrado." });

        jogo.Nome = request.Nome;
        jogo.Console = request.Console;
        jogo.AnoLancamento = request.AnoLancamento;
        jogo.Preco = request.Preco;

        await _repositorio.AtualizarAsync(jogo);
        return Ok(jogo);
    }

    /// <summary>
    /// Apaga um jogo.
    /// </summary>
    /// <param name="id">Id do jogo.</param>
    /// <returns>Nenhum conteúdo.</returns>
    /// <response code="204">Jogo apagado.</response>
    /// <response code="400">Id em formato inválido.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        if (!IdValido(id))
            return BadRequest(new { mensagem = "Id inválido." });

        var removido = await _repositorio.RemoverAsync(id);
        if (!removido)
            return NotFound(new { mensagem = "Jogo não encontrado." });

        return NoContent();
    }

    private static bool IdValido(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}
