using System.Text;
using api.Features.Auth.Login;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using domain.Entity;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Seeders;
using System.Threading.RateLimiting;
using domain.Interfaces;
using Infrastructure.Repositries;
using api.Features.Livre;
using api.Features.Nouveautes;
using api.Features.Emprunt;
using api.Features.Parametre;
using api.Features.Sanctions;
using Npgsql;
using api.Features.Auth.ForgetPassword;
using api.Features.Profile;
using Infrastructure.Repositories;
using LibraryManagement.Features.Dashboard.Repositories;
using LibraryManagement.Features.Dashboard.Services;
using Infrastructure.SignalR;
using api.Features.Membres;
using domain.Entity.Enum;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("postgres"));

NpgsqlConnection.GlobalTypeMapper.MapEnum<Raison_sanction>("raison_sanction");
NpgsqlConnection.GlobalTypeMapper.MapEnum<etat_liv>("etat_liv");
NpgsqlConnection.GlobalTypeMapper.MapEnum<Statut_emp>("statut_emp");
NpgsqlConnection.GlobalTypeMapper.MapEnum<Statut_liv>("statut_liv");
NpgsqlConnection.GlobalTypeMapper.MapEnum<StatutMemb>("statut_memb");
NpgsqlConnection.GlobalTypeMapper.MapEnum<TypeMemb>("type_memb");
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<BiblioDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});



builder.Services.AddIdentity<Bibliothecaire, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
}).AddEntityFrameworkStores<BiblioDbContext>().AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        NameClaimType = "sub",
        RoleClaimType = "role",
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
    };
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5)
            }));
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();



// Add these services
builder.Services.AddSignalR();

builder.Services.AddScoped<IRepository<Nouveaute>, Repository<Nouveaute>>();
builder.Services.AddScoped<IRepository<Sanction>, Repository<Sanction>>();
builder.Services.AddScoped<IRepository<Membre>, Repository<Membre>>();

builder.Services.AddScoped<Repository<Sanction>>();
builder.Services.AddScoped<Repository<Membre>>();
builder.Services.AddScoped<Repository<Nouveaute>>();


builder.Services.AddScoped<ILivresRepository, LivresRepository>();
builder.Services.AddScoped<IFichierRepository, FichierRepository>();
builder.Services.AddScoped<IEmpruntsRepository, EmpruntsRepository>();
builder.Services.AddScoped<IParametreRepository, ParametreRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddScoped<LivresHandler>();
builder.Services.AddScoped<MembreHandler>();
builder.Services.AddScoped<EmpruntHandler>();
builder.Services.AddScoped<ParametreHandler>();
builder.Services.AddScoped<SanctionHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<NouveauteHandler>();
builder.Services.AddScoped<ForgotPasswordHandler>();
builder.Services.AddScoped<ProfileHandler>();
builder.Services.AddScoped<DashboardService>();




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); //  l’authentification par cookie
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Appel du seeding au démarrage de l'application
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BiblioDbContext>();
    var services = scope.ServiceProvider;
    using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
        await DataSeeder.SeedAllDataAsync(services);
        await transaction.CommitAsync();
        Console.WriteLine("✅ Seeding process completed successfully!");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.WriteLine($"❌ Error during seeding: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}
//order imporatnt routing rouetr first controller last  authentification then autorization 
app.UseRouting();// After app.UseRouting()
app.MapHub<dashboardHub>("/dashboardHub");
app.MapHub<NotificationHub>("/notificationHub");

app.UseCors("AllowAngularDevClient");


app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
