

using Microsoft.EntityFrameworkCore;
using FeriaOriente.API.Datos;
using FeriaOriente.API.Interfaces;
using FeriaOriente.API.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FeriaOriente.API.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGestionEmprendimientoService, GestionEmprendimientoService>();
builder.Services.AddScoped<IFeriaService, FeriaService>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger con autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingresa únicamente el token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Configuración de autenticación
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!))
        };
    });

// Conexión a la base de datos
builder.Services.AddDbContext<FeriaOrienteContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionSQL")));

var app = builder.Build();

// Crear o actualizar administrador inicial
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<FeriaOrienteContext>();

    var configuracion = app.Configuration
        .GetSection("AdministradorInicial");

    string correo = configuracion["Correo"]!;
    string contrasena = configuracion["Contrasena"]!;

    var administrador = await context.Administradores
        .FirstOrDefaultAsync(a => a.Correo == correo);

    if (administrador == null)
    {
        administrador = new Administrador
        {
            Nombre = "Administrador",
            Apellido = "FeriaOriente",
            Correo = correo
        };

        context.Administradores.Add(administrador);
    }

    var hasher = new PasswordHasher<Administrador>();

    administrador.ContrasenaHash = hasher.HashPassword(
        administrador,
        contrasena
    );

    await context.SaveChangesAsync();
}

// Configuración de Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

