using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InsanK.Data;
using InsanK.Helpers;
using InsanK.Services;
using InsanK;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Configure form options for high-quality photo uploads (up to 50MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o => {
    o.ValueLengthLimit = 52428800; // 50MB for high-quality images
    o.MultipartBodyLengthLimit = 52428800; // 50MB total
    o.MemoryBufferThreshold = 2097152; // 2MB buffer
    o.BufferBody = true;
    o.BufferBodyLengthLimit = 52428800; // 50MB buffer limit
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "İnsan Kaynakları API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
            },
            new string[] { }
        }
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<InsanK.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("GeminiClient", client => {
    client.Timeout = TimeSpan.FromSeconds(90);
    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
    client.DefaultRequestHeaders.ConnectionClose = false;
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler {
    PooledConnectionLifetime = TimeSpan.FromMinutes(15),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    UseCookies = false,
    AllowAutoRedirect = true,
    MaxConnectionsPerServer = 20,
    SslOptions = new System.Net.Security.SslClientAuthenticationOptions
    {
        RemoteCertificateValidationCallback = delegate { return true; }
    }
});

builder.Services.AddHttpClient();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<InsanK.Helpers.JwtSettings>(jwtSettings);

var aiSettings = builder.Configuration.GetSection("AISettings");
builder.Services.Configure<InsanK.Helpers.AISettings>(aiSettings);

var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtSettings["Issuer"],
    ValidAudience = jwtSettings["Audience"],
    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
        System.Text.Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]))
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddScoped<InsanK.Helpers.JwtService>();
builder.Services.AddScoped<InsanK.Helpers.CVAIService>();
builder.Services.AddScoped<InsanK.Helpers.PDFExtractor>();
builder.Services.AddScoped<InsanK.Services.KullaniciService>();
builder.Services.AddScoped<InsanK.Services.CVService>();
builder.Services.AddScoped<InsanK.Services.IlanService>();
builder.Services.AddScoped<InsanK.Services.BasvuruService>();
builder.Services.AddScoped<InsanK.Services.MesajService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Veritabanını sıfırla
        // DbReset.ResetDatabaseAsync(services, services.GetRequiredService<ILogger<DbReset>>()).Wait();
        
        var context = services.GetRequiredService<InsanK.Data.ApplicationDbContext>();
        context.Database.Migrate();
        
        logger.LogInformation("Veritabanı başarıyla oluşturuldu.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı oluşturulurken hata oluştu.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
