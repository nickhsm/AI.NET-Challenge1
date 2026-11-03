using Microsoft.EntityFrameworkCore;
using TicketService.Infrastructure.DbContexts;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();