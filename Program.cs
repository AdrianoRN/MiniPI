using MiniPI.Data;
using MiniPI.ViewModels;
using MiniPI.Settings;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniPI.Models;
using MiniPI.Repositories;
using Shop.Services;

var builder = WebApplication.CreateBuilder(args);
var Key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(x =>
{	
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("manager"));
    options.AddPolicy("Employeer", policy =>
        policy.RequireRole("employeer"));
});


builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();


 
app.MapGet("/v1/pis", (AppDbContext context) =>
{
    var pis = context.PIs;
    return pis is not null ? Results.Ok(pis) : Results.NotFound();
}); 

app.MapPost("/login", (User model) =>
{
    var user = UserRepository.Get(model.Username, model.Password);
    if (user == null)
        return Results.NotFound(new{message = "Usuário Invalido"});

    var token = TokenService.GenerateToken(user);

    user.Password = "";

    return Results.Ok(new
    {
        user = user,
        token = token
    });
   
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

//https://localhost:7292/swagger/index.html

