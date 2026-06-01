using ApiPeliculas;
using ApiPeliculas.endpoints;
using ApiPeliculas.Repository;
using ApiPeliculas.Services;
using ApiPeliculas.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

//Areglo de servicios

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddCors(options =>
{
    
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IRepositoryGeneros, RepositoryGeneros>();
builder.Services.AddScoped<IRepositoryActores, RepositoryActores>();
builder.Services.AddScoped<IRepositoryPeliculas, RepositoryPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IAlmacenadorArchivos,AlmacenadorArchivosLocal>();
builder.Services.AddScoped<IRepositoryErrores, RepositoryErrores>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//Fin area Servicios
var app = builder.Build();

//Areglo de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => {
    await TypedResults.BadRequest(
        new {tipo="error", mensaje = "Ocurrió un error en el servidor" , status=500}
        ).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();
app.UseCors();
app.UseOutputCache();

app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => {
    throw new InvalidOperationException("Error de prueba");
});
app.MapGroup("/Generos").MapGeneros();
app.MapGroup("/Actores").MapActores();
app.MapGroup("/Peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();



app.Run();
