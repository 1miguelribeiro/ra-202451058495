using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using LojaRetro.Api.Models;
using LojaRetro.Api.Repositories;
using LojaRetro.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MongoDb:ConnectionString"]
    ?? throw new InvalidOperationException("Variável MONGODB__CONNECTIONSTRING não configurada. Veja o .env.example.");
var nomeDoBanco = builder.Configuration["MongoDb:Database"] ?? "loja_retro";

var jwt = builder.Configuration.GetSection("Jwt");
var chaveJwt = jwt["Key"];
if (string.IsNullOrWhiteSpace(chaveJwt) || chaveJwt.Length < 32)
    throw new InvalidOperationException("Variável JWT__KEY não configurada ou com menos de 32 caracteres. Veja o .env.example.");

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(nomeDoBanco));
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<EstoqueService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Loja Retrô",
        Description = "API para controle do estoque da loja de jogos retrô.",
        Contact = new OpenApiContact
        {
            Name = "Miguel Ribeiro"
        }
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Faça login em POST /api/auth/login e cole aqui só o token."
    });
    options.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", documento)] = []
    });
});

var app = builder.Build();

await CriarAdminAsync(app);

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Loja Retrô v1");
    options.DocumentTitle = "Documentação - API Loja Retrô";
});

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task CriarAdminAsync(WebApplication app)
{
    var email = app.Configuration["Admin:Email"]?.Trim().ToLower();
    var senha = app.Configuration["Admin:Senha"];
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        return;

    using var escopo = app.Services.CreateScope();
    var usuarios = escopo.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    if (await usuarios.BuscarPorEmailAsync(email) is not null)
        return;

    var admin = new Usuario { Nome = "Administrador", Email = email, Perfil = Perfis.Admin };
    admin.SenhaHash = new PasswordHasher<Usuario>().HashPassword(admin, senha);
    await usuarios.CriarAsync(admin);
}
