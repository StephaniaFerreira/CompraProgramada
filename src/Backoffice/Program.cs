using Infraestrutura;
using Microsoft.EntityFrameworkCore;
using Core.MotorCompra;
using System.Text.Json.Serialization;
using Infraestrutura.Kafka;
using Aplicacao.Interfaces;
using Aplicacao.Services;
using Infraestrutura.Persistencia;
using Core.Interfaces;
using Core.Interfaces.Backoffice;
using Core.Service;
using Core.Repositories;
using Core.Interfaces.MotorCompra;
using Core.Interfaces.Cotacoes;
using Infraestrutura.Arquivo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBackofficeService, BackofficeService>();
builder.Services.AddScoped<IMotorCompraService, MotorCompraService>();
builder.Services.AddScoped<IImpostoService, ImpostoService>();
builder.Services.AddScoped<ICotacaoService, CotacaoService>();

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MySQLDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        b => b.MigrationsAssembly("Infraestrutura")
    ));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<MySQLDbContext>());

builder.Services.AddSingleton<IKafkaProducer>(sp =>
    new KafkaProducer("localhost:9092"));

builder.Services.AddScoped<IBackofficeRepository, BackofficeRepository>();
builder.Services.AddScoped<IBackofficeDomainService, BackofficeDomainService>();
builder.Services.AddScoped<IBackofficeService, BackofficeService>();
builder.Services.AddScoped<IMotorCompraDomainService, MotorCompraDomainService>();
builder.Services.AddScoped<IMotorCompraRepository, MotorCompraRepository>();
builder.Services.AddScoped<ICotacaoRepository, CotacaoRepository>();
builder.Services.AddScoped<ICotacaoArquivoReader, CotacaoArquivoReader>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.ApplyMigrations();
await app.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
