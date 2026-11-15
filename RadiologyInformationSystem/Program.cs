using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);


//Repositories
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// Services
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
