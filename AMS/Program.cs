using BAMS.Implemetations;
using BAMS.Interface;
using DAMS.Implementations;
using DAMS.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// BAMS Services
builder.Services.AddScoped<IUserBussinesService, UserBussinesService>();
builder.Services.AddScoped<IEmailBussinesService, EmailBussinesService>();
builder.Services.AddScoped<IAuthBussinesService, AuthBussinesService>();

// DAMS Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddTransient<JobsSchedular, JobsSchedular>();
//JAYANT
// Add JobsScheduler as a Hosted Service
builder.Services.AddHostedService<JobsScheduler>();


// IHttpContextAccessor for accessing session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add Session Middleware
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession(); // Use session middleware
app.UseAuthorization();
app.MapControllers();

app.Run();
