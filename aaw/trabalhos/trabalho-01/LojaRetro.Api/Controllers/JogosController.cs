using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

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
}
