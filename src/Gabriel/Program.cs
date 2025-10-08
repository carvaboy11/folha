using Microsoft.EntityFrameworkCore;
using Gabriel.Models;
using Gabriel.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Enzo_Gabriel.db"));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.Migrate();
}

// POST api/funcionario/cadastrar
app.MapPost("/api/funcionario/cadastrar", async (FuncionarioDto dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Cpf))
        return Results.BadRequest(new { message = "Nome e CPF são obrigatórios." });

    var exists = await db.Funcionarios.AnyAsync(f => f.Cpf == dto.Cpf);
    if (exists) return Results.Conflict(new { message = "Funcionario com esse CPF já existe." });

    var f = new Funcionario
    {
        Nome = dto.Nome.Trim(),
        Cpf = dto.Cpf.Trim(),
        ValorHora = dto.ValorHora,
        HorasTrabalhadas = dto.HorasTrabalhadas
    };
    db.Funcionarios.Add(f);
    await db.SaveChangesAsync();
    return Results.Created($"/api/funcionario/{f.Id}", f);
});

// GET api/funcionario/listar
app.MapGet("/api/funcionario/listar", async (AppDbContext db) =>
{
    var list = await db.Funcionarios.ToListAsync();
    return Results.Ok(list);
});

// POST api/folha/cadastrar
app.MapPost("/api/folha/cadastrar", async (FolhaInputDto input, AppDbContext db) =>
{
    // Find funcionario by CPF
    var funcionario = await db.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == input.Cpf);
    if (funcionario is null) return Results.NotFound(new { message = "Funcionario não encontrado." });

    // Calculate Salario Bruto
    var salarioBruto = funcionario.HorasTrabalhadas * funcionario.ValorHora;

    // Calculate INSS
    decimal descontoINSS;
    if (salarioBruto <= 1693.72m) descontoINSS = salarioBruto * 0.08m;
    else if (salarioBruto <= 2822.90m) descontoINSS = salarioBruto * 0.09m;
    else if (salarioBruto <= 5645.80m) descontoINSS = salarioBruto * 0.11m;
    else descontoINSS = 621.03m;

    // Calculate IR (after INSS)
    var baseIR = salarioBruto - descontoINSS;
    decimal aliquotaIR = 0m;
    decimal parcelaDeduzir = 0m;

    if (baseIR <= 1903.98m) { aliquotaIR = 0m; parcelaDeduzir = 0m; }
    else if (baseIR <= 2826.65m) { aliquotaIR = 0.075m; parcelaDeduzir = 142.80m; }
    else if (baseIR <= 3751.05m) { aliquotaIR = 0.15m; parcelaDeduzir = 354.80m; }
    else if (baseIR <= 4664.68m) { aliquotaIR = 0.225m; parcelaDeduzir = 636.13m; }
    else { aliquotaIR = 0.275m; parcelaDeduzir = 869.36m; }

    var descontoIR = Math.Max(0, (double)((baseIR * aliquotaIR) - parcelaDeduzir));
    decimal descontoIRdec = Convert.ToDecimal(descontoIR);

    // FGTS = 8% do bruto
    var fgts = salarioBruto * 0.08m;

    // Salario liquido = bruto - INSS - IR
    var salarioLiquido = salarioBruto - descontoINSS - descontoIRdec;

    var folha = new FolhaPagamento
    {
        FuncionarioId = funcionario.Id,
        Mes = input.Mes,
        Ano = input.Ano,
        SalarioBruto = decimal.Round(salarioBruto, 2),
        DescontoINSS = decimal.Round(descontoINSS, 2),
        DescontoIR = decimal.Round(descontoIRdec, 2),
        ValorFGTS = decimal.Round(fgts, 2),
        SalarioLiquido = decimal.Round(salarioLiquido, 2)
    };

    db.Folhas.Add(folha);
    await db.SaveChangesAsync();

    return Results.Created($"/api/folha/{folha.Id}", folha);
});

// GET api/folha/listar
app.MapGet("/api/folha/listar", async (AppDbContext db) =>
{
    var folhas = await db.Folhas.Include(f => f.Funcionario).ToListAsync();
    if (folhas.Count == 0) return Results.NotFound();
    return Results.Ok(folhas);
});

// GET api/folha/buscar/{cpf}/{mes}/{ano}
app.MapGet("/api/folha/buscar/{cpf}/{mes:int}/{ano:int}", async (string cpf, int mes, int ano, AppDbContext db) =>
{
    var funcionario = await db.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == cpf);
    if (funcionario is null) return Results.NotFound();

    var folha = await db.Folhas.Include(f => f.Funcionario)
        .FirstOrDefaultAsync(f => f.FuncionarioId == funcionario.Id && f.Mes == mes && f.Ano == ano);

    if (folha is null) return Results.NotFound();

    return Results.Ok(folha);
});

app.Run();
