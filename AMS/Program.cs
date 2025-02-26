using BAMS.Implemetations;
using BAMS.Interface;
using DAMS.Implementations;
using DAMS.Interface;
using DAMS.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtSecretKey = jwtSettings["SecretKey"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

// ✅ Add Controllers
builder.Services.AddControllers();

// ✅ Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true
        };
    });

// ✅ Add Authorization
builder.Services.AddAuthorization();

// ✅ Add Swagger with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // 🔹 Configure Swagger to accept Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}' below. Example: 'Bearer abc123'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// ✅ Add HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<PasswordHelper>();
// ✅ Register Services (Dependency Injection)
builder.Services.AddScoped<IUserBussinesService, UserBussinesService>();
builder.Services.AddScoped<IEmailBussinesService, EmailBussinesService>();
builder.Services.AddScoped<IAuthBussinesService, AuthBussinesService>();
builder.Services.AddScoped<IAmazonBussinessService, AmazonBussinessService>();
builder.Services.AddScoped<ICurrencyExchangeBussines, CurrencyExchangeBussines>();

// DAMS Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAmazonS3Service, AmazonS3Service>();
builder.Services.AddScoped<ICurrencyExchange, CurrencyExchange>();

builder.Services.AddHostedService<JobsScheduler>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ✅ Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();         // ✅ Use session middleware
app.UseAuthentication();  // ✅ Ensure Authentication Middleware is added
app.UseAuthorization();   // ✅ Ensure Authorization Middleware is added

app.MapControllers();     // ✅ Map Controllers

app.Run();
