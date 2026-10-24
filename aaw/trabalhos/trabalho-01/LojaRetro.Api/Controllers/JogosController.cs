using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LojaRetro.Api.Controllers;

[ApiController]
[Route("api/jogos")]
public class JogosController : ControllerBase
{
    private readonly JogoRepository _repositorio;

    public JogosController(JogoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    public async Task<ActionResult<List<Jogo>>> Listar()
    {
        var jogos = await _repositorio.ListarAsync();
        return Ok(jogos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Jogo>> BuscarPorId(string id)
    {
        if (!IdValido(id))
            return BadRequest(new { mensagem = "Id inválido." });

        var jogo = await _repositorio.BuscarPorIdAsync(id);
        if (jogo is null)
            return NotFound(new { mensagem = "Jogo não encontrado." });

        return Ok(jogo);
    }

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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
