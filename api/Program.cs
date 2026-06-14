using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Application.Security;
using MyTCGBinder.Application.Services;
using MyTCGBinder.Application.UseCases;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;
using MyTCGBinder.Infrastructure.Middlewares;
using MyTCGBinder.Infrastructure.Repositories;
using MyTCGBinder.Infrastructure.TcgApi;
using System.Text.Json.Serialization;
using MyTCGBinder.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',')
    ?? ["http://localhost:4200", "http://localhost:4201"];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & Unit of Work
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCardRepository, UserCardRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<ITCGCardRepository, TCGCardRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<PasswordHasher<User>>();

// Use cases - Auth
builder.Services.AddScoped<IAuthenticateUseCase, AuthenticateUseCase>();
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddScoped<ISendEmailForgotPasswordUseCase, SendEmailForgotPasswordUseCase>();
builder.Services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
builder.Services.AddScoped<IDeleteUserDataUseCase, DeleteUserDataUseCase>();

// Use cases - Cards
builder.Services.AddScoped<IAddCardUseCase, AddCardUseCase>();
builder.Services.AddScoped<IDeleteCardUseCase, DeleteCardUseCase>();
builder.Services.AddScoped<IGetCollectionUseCase, GetCollectionUseCase>();
builder.Services.AddScoped<IGetCollectionCountUseCase, GetCollectionCountUseCase>();
builder.Services.AddScoped<IUpdateCardQuantityUseCase, UpdateCardQuantityUseCase>();
builder.Services.AddScoped<ISearchCardsUseCase, SearchCardsUseCase>();
builder.Services.AddScoped<IGetSetsUseCase, GetSetsUseCase>();

// Jobs - TCG
builder.Services.AddHttpClient("tcg", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TcgApi:BaseUrl"]!);

    var apiKey = builder.Configuration["TcgApi:Key"];
    if (!string.IsNullOrEmpty(apiKey))
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
});

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddHostedService<SeedTcgCardsJob>();


var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var cookie = ctx.Request.Cookies["MyTCGBinder_token"];
                if (!string.IsNullOrEmpty(cookie))
                    ctx.Token = cookie;
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("register", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("forgot-password", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(5);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("reset-password", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(5);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Context>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
