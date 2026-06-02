using ApiPeliculas;
using ApiPeliculas.endpoints;
using ApiPeliculas.Repository;
using ApiPeliculas.Services;
using ApiPeliculas.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ApiPeliculas.Utilidades;

var builder = WebApplication.CreateBuilder(args);

//Areglo de servicios

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

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

builder.Services.AddAuthentication().AddJwtBearer()
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = Llaves.IssuerPropio,
    ValidAudience = Llaves.IssuerPropio,
    //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(),
    IssuerSigningKeys = Llaves.ObtenerTodasLlaves(builder.Configuration, Llaves.IssuerPropio),
    ClockSkew = TimeSpan.Zero
 });


builder.Services.AddAuthorization();


//Fin area Servicios
var app = builder.Build();

//Areglo de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => {
   
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;
    var error = new Error();
    
    //error.Id = Guid.NewGuid();
    error.Fecha = DateTime.UtcNow;
    error.Mensaje = exception.Message;
    error.StackTrace = exception.StackTrace ?? string.Empty;

    var repositorio = context.RequestServices.GetRequiredService<IRepositoryErrores>();
    await repositorio.Crear(error);
    
    await TypedResults.BadRequest(
        new {tipo="error", mensaje = "Ocurrió un error en el servidor" , status=500}
        ).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();
app.UseCors();
app.UseOutputCache();
app.UseAuthentication();
//Fin Middlewares

app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => {
    throw new InvalidOperationException("Error de prueba");
});
app.MapGroup("/Generos").MapGeneros();
app.MapGroup("/Actores").MapActores();
app.MapGroup("/Peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();



app.Run();
