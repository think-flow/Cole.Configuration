using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("appsettings.yml", true, true);
builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yml", true, true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<Model.Receipt>().Bind(builder.Configuration.GetSection("TestObj"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/yaml/config", (IConfiguration config) => config["Logging:LogLevel:Microsoft.AspNetCore"]);

app.MapGet("/yaml/option", (IOptionsSnapshot<Model.Receipt> options) => options.Value);

app.Run();
