using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BirraRepo>(opt => opt.UseInMemoryDatabase("BirraList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/ping", () => "pong");

app.MapGet("/birras", (BirraRepo repo) => repo.Birras.ToList());

app.MapGet("/birras/{id}", (int id, BirraRepo repo) => 
{
    Birra? birra = repo.Birras.Find(id);
    return birra is not null 
        ? Results.Ok(birra) 
        : Results.NotFound();
});

app.MapGet("/birras/byprice/{price}", (float price, BirraRepo repo) => 
{
    return repo.Birras.Where(x => x.Price <= price);
}).AddEndpointFilter(async (context, next) =>
{
    float price = context.GetArgument<float>(0);
    if (price <= 0) return Results.Problem("El precio tiene que ser mayor a 0, rey.");
    if (price >= 2000) return Results.Problem("Todavia no existen birras tan caras padre. Ingresa un valor menor a 2mil");

    return await next(context);
});

app.MapPost("/birras", (Birra unaBirra, BirraRepo repo) => 
{
    repo.Birras.Add(unaBirra);
    repo.SaveChanges();
    return Results.Created("Birra agregada", unaBirra);
});

app.MapPut("/birras/{id}", (int id, Birra unaBirra, BirraRepo repo) =>
{
    Birra? birra = repo.Birras.Find(id);
    if (birra is null) return Results.NotFound();

    birra.Name = unaBirra.Name;
    birra.Price = unaBirra.Price;
    repo.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("birras/{id}", (int id, BirraRepo repo) => 
{
    Birra? birra = repo.Birras.Find(id);
    if (birra is null) return Results.NotFound();

    repo.Birras.Remove(birra);
    repo.SaveChanges();
    return Results.Ok(birra);
});

app.Run();
