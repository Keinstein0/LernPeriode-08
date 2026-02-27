using HPlusSportAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSvelte",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // Your frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



var connectionString = builder.Configuration.GetConnectionString("DebugConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<MusicContext>(options =>
{
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MusicContext>();
    dbContext.Database.Migrate(); // Apply all migrations

}


app.UseCors("AllowSvelte");

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
