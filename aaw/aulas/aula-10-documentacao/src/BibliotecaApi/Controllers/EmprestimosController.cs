using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

// Endpoints de empréstimo exigem login (JWT da Aula 07, pré-pronto).
// É por causa deles que o Swagger precisará do security scheme (Passo 9):
// sem o botão Authorize, o "Try it out" daqui devolve 401.
/// <summary>
/// Empréstimos e devoluções de livros (requer token JWT).
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Empréstimos")]
public class EmprestimosController : ControllerBase
{
    private readonly BibliotecaRepository _repo;

    public EmprestimosController(BibliotecaRepository repo) => _repo = repo;

    /// <summary>
    /// Lista todos os empréstimos.
    /// </summary>
    /// <returns>Lista de empréstimos (ativos, devolvidos e atrasados).</returns>
    /// <response code="200">Retorna a lista de empréstimos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Emprestimo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ObterTodos()
    {
        return Ok(_repo.ObterEmprestimos());
    }

    /// <summary>
    /// Obtém um empréstimo específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do empréstimo.</param>
    /// <returns>Dados do empréstimo encontrado.</returns>
    /// <response code="200">Empréstimo encontrado.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Empréstimo não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Emprestimo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterPorId(int id)
    {
        var emprestimo = _repo.ObterEmprestimoPorId(id);
        if (emprestimo == null)
            return NotFound(new { mensagem = $"Empréstimo com ID {id} não encontrado." });
        return Ok(emprestimo);
    }

    /// <summary>
    /// Realiza um novo empréstimo de livro.
    /// </summary>
    /// <remarks>
    /// O livro deve estar disponível (não emprestado).
    /// O prazo de devolução padrão é de 14 dias.
    ///
    /// Exemplo de requisição:
    ///
    ///     POST /api/emprestimos
    ///     {
    ///         "livroId": 1,
    ///         "nomeUsuario": "João Silva"
    ///     }
    ///
    /// </remarks>
    /// <param name="emprestimo">Dados do empréstimo (livro e usuário).</param>
    /// <returns>Empréstimo recém-criado.</returns>
    /// <response code="201">Empréstimo realizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="409">Livro não encontrado ou indisponível para empréstimo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Emprestimo), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Criar([FromBody] Emprestimo emprestimo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var criado = _repo.CriarEmprestimo(emprestimo);
        if (criado == null)
            return Conflict(new { mensagem = "Livro não encontrado ou não está disponível para empréstimo." });

        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Registra a devolução de um livro emprestado.
    /// </summary>
    /// <remarks>
    /// Marca o empréstimo como devolvido, grava a data de devolução
    /// e deixa o livro disponível novamente.
    /// </remarks>
    /// <param name="id">ID do empréstimo.</param>
    /// <returns>Mensagem de confirmação e o empréstimo atualizado.</returns>
    /// <response code="200">Livro devolvido com sucesso.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="404">Empréstimo não encontrado ou já devolvido.</response>
    [HttpPatch("{id:int}/devolver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Devolver(int id)
    {
        if (!_repo.DevolverLivro(id))
            return NotFound(new { mensagem = "Empréstimo não encontrado ou já devolvido." });

        var emprestimo = _repo.ObterEmprestimoPorId(id);
        return Ok(new { mensagem = "Livro devolvido com sucesso.", emprestimo });
    }
}
