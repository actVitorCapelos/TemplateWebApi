using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostApi.IOptionsModel;
using PostApi.Services;
using PostApi.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddSimpleConsole(opt =>
{
    opt.SingleLine = true;
    opt.TimestampFormat = "dd/MM/yyyy HH:mm:ss ";
});


// Add services to the container.
builder.Services.Configure<LogSettings>(builder.Configuration.GetSection("LogSettings"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<LoggerProvider>();
var app = builder.Build();

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