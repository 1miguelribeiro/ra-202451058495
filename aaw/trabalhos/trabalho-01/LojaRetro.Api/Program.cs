using System.Reflection;
using LojaRetro.Api.Repositories;
using Microsoft.OpenApi;
using MongoDB.Driver;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MongoDb:ConnectionString"]
    ?? throw new InvalidOperationException("Variável MONGODB__CONNECTIONSTRING não configurada. Veja o .env.example.");
var nomeDoBanco = builder.Configuration["MongoDb:Database"] ?? "loja_retro";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(nomeDoBanco));
builder.Services.AddScoped<JogoRepository>();

builder.Services.AddControllers();

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
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Loja Retrô v1");
    options.DocumentTitle = "Documentação - API Loja Retrô";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
