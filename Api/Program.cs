using Core.Extensions;
using Infrastructure.Extension;
using Microsoft.Extensions.Options;
using Namespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

InfrastructureServiceCollection.SetupServiceCollection(builder.Services, builder.Configuration);
CoreServiceCollection.SetupServiceCollection(builder.Services, builder.Configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ISummonerService, SummonerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UsePathBase(new PathString("/api"));
app.UseRouting();

app.MapControllers();

app.Run();
