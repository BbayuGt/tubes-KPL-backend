using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Services;

using tubes_KPL_backend.Services;

var builder = WebApplication.CreateBuilder(args);

//Xendit
builder.Services.Configure<tubes_KPL_backend.Models.XenditSettings>(
    builder.Configuration.GetSection("Xendit")
);

builder.Services.AddHttpClient();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Jwt
var JwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(JwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = JwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    // Tambah header API Key biar gampang di Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Header untuk autorisasi pada endpoint tertentu\n\n" +
                      "Tulis 'Bearer' <spasi> token di input teks.\n\n" +
                      "Contoh: 'Bearer abcd'\n\n",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddScoped<CampaignService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// migrasi database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    dbContext.Database.Migrate();
}

app.Run();
