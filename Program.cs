using MiniPI.Data;
using MiniPI.ViewModels;
 
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
 
app.MapGet("/v1/pis", (AppDbContext context) =>
{
    var pis = context.PIs;
    return pis is not null ? Results.Ok(pis) : Results.NotFound();
}); 

app.MapPost("/v1/pis", (AppDbContext context, CreatePIViewModel model) =>
{
    var avaliacao = model.MapTo();
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);
 
    context.PIs.Add(avaliacao);
    context.SaveChanges();
 
    return Results.Created($"/v1/pis/{avaliacao.Id}", avaliacao);
});
 
app.Run();

