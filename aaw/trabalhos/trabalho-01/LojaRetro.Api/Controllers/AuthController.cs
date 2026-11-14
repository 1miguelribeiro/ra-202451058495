using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LojaRetro.Api.Controllers;

/// <summary>
/// Cadastro de usuários e login.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Tags("Autenticação")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _hasher = new();

    public AuthController(IUsuarioRepository usuarios, IConfiguration config)
    {
        _usuarios = usuarios;
        _config = config;
    }

    /// <summary>
    /// Cria um usuário com o perfil funcionario.
    /// </summary>
    /// <param name="request">Nome, e-mail e senha.</param>
    /// <returns>Mensagem de confirmação.</returns>
    /// <response code="201">Usuário criado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="409">E-mail já cadastrado.</response>
    [HttpPost("registro")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registrar(RegistroRequest request)
    {
        var email = request.Email.Trim().ToLower();

        if (await _usuarios.BuscarPorEmailAsync(email) is not null)
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = email,
            Perfil = Perfis.Funcionario
        };
        usuario.SenhaHash = _hasher.HashPassword(usuario, request.Senha);

        await _usuarios.CriarAsync(usuario);
        return StatusCode(StatusCodes.Status201Created, new { mensagem = "Usuário criado." });
    }

    /// <summary>
    /// Confere e-mail e senha e devolve um token JWT válido por 1 hora.
    /// </summary>
    /// <param name="request">E-mail e senha.</param>
    /// <returns>Token e dados do usuário.</returns>
    /// <response code="200">Login feito.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">E-mail ou senha incorretos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var usuario = await _usuarios.BuscarPorEmailAsync(request.Email.Trim().ToLower());

        if (usuario is null ||
            _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha) == PasswordVerificationResult.Failed)
            return Unauthorized(new { mensagem = "E-mail ou senha incorretos." });

        return Ok(GerarToken(usuario));
    }

    private LoginResponse GerarToken(Usuario usuario)
    {
        var minutos = int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60");
        var expiraEm = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id!),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil)
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Nome = usuario.Nome,
            Perfil = usuario.Perfil,
            ExpiraEm = expiraEm
        };
    }
}
