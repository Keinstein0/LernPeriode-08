using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MusicBackend.Models;
using MusicBackend.Services;
using System.Reflection; // Wichtig für Assembly.GetExecutingAssembly()
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load("../../../.env");
if (!File.Exists("../../../.env"))
{
    throw new Exception(".env not found");
}
var f = File.ReadAllText("../../../.env");



// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSvelte",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
                    
        });
});



var connectionString = builder.Configuration.GetConnectionString("DebugConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<MusicContext>(options =>
{
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddScoped<IPasswordHash, BCryptHasher>();
builder.Services.AddScoped<ITokenGenerator, JWTTokenGenerator>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music Backend", Version = "v1" });



    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddHealthChecks();

//Authentication service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{// JwtConfig:Key ist der symetrische Schlüssel für die Verschlüsselung
    var key = builder.Configuration.GetSection("JwtConfig:Key").Value;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false, // Optional: Validierung des Issuer
        ValidateAudience = false,
        ValidateLifetime = true, // Zeitstempel überprüfen
        ClockSkew = TimeSpan.Zero // Optional: Pufferzeit deaktivieren – siehe «Zusätzliche Angaben zum Auftrag» <-- What is that??
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MusicContext>();
    dbContext.Database.Migrate(); // Apply all migrations

}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseCors("AllowSvelte");


// Configure the HTTP request pipeline.


//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MusicContext>();
    await DbPopulator.InitializeAsync(context);
}

app.Run();
