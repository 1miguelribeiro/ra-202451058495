using EscolaApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaApi.Controllers;

[ApiController]
[Route("api/v1/alunos")]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext db;
    public AlunosController(AppDbContext db) { this.db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAlunos([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
    {
        if (pagina < 1) pagina = 1;
        if (tamanho < 1 || tamanho > 100) tamanho = 10;

        var total = await db.Alunos.CountAsync();

        var items = await db.Alunos
            .Include(a => a.Matriculas)
            .OrderBy(a => a.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();

        return Ok(new
        {
            pagina,
            tamanho,
            total,
            totalpaginas = (int)Math.Ceiling(total / (double)tamanho),
            items,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlunoPorId(int id)
    {
        var aluno = await db.Alunos
            .Include(a => a.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aluno is null)
            return NotFound();

        return Ok(aluno);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarAluno(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null)
            return NotFound();

        db.Alunos.Remove(aluno);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{alunoId}/matriculas")]
    public async Task<IActionResult> GetMatriculas(int alunoId)
    {
        var existe = await db.Alunos.AnyAsync(a => a.Id == alunoId);
        if (!existe)
            return Problem(detail: "Aluno não encontrado.", statusCode: 404);

        var matriculas = await db.Matriculas
            .Where(m => m.AlunoId == alunoId)
            .ToListAsync();

        return Ok(matriculas);
    }

    [HttpGet("{alunoId}/matriculas/{matriculaId}")]
    public async Task<IActionResult> GetMatricula(int alunoId, int matriculaId)
    {
        var matricula = await db.Matriculas
            .FirstOrDefaultAsync(m => m.Id == matriculaId && m.AlunoId == alunoId);

        if (matricula is null)
            return NotFound();

        return Ok(matricula);
    }
}
