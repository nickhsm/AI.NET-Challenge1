using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;
using TicketService.API.Services;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Seeders;
using TicketService.Infrastructure.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    // Voeg de Bearer token definitie toe
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Plak hier je token."
    });

    // Zorg dat Swagger weet dat het token nodig is
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<TicketServiceDbContext>(dbContectOption =>
    dbContectOption.UseNpgsql(
        builder.Configuration.GetConnectionString("Docker")));

// HttpClient for FaceRecognitionService
builder.Services.AddHttpClient<IFaceRecognitionService, FaceRecognitionService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Face:Endpoint"]!);
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", builder.Configuration["Face:ApiKey"]);
    }
);

// Repositories
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IConductorRepository, ConductorRepository>();

// Seeder
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

// Password hash
builder.Services.AddScoped<IPasswordHasher<IUser>, PasswordHasher<IUser>>();

// Creates JWT tokens at login
builder.Services.AddScoped<ITokenService, TokenService>();

// Check JWT tokens on every request
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        };
    });

// Check who has access to what
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();