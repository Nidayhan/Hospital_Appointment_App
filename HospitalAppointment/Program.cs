using HospitalAppointment_core.Interfaces;
using HospitalAppointment_core.Interfaces.RepositoryInterfaces;
using HospitalAppointment_core.Services;
using HospitalAppointment_Infrastructure.Data;
using HospitalAppointment_Infrastructure.Repositories;
using HospitalAppointment_Infrastructure.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using HospitalAppointment_domain.Entities;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
            
// Add services to the container.
// Add global authorization policy: require authenticated user by default.
builder.Services.AddControllers(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(defaultPolicy));
});
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to support Bearer authentication
builder.Services.AddSwaggerGen(options =>
{
    // Basic swagger doc
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HospitalAppointment API", Version = "v1" });

    // Define the security scheme for Bearer (use ApiKey to allow pasting "Bearer <token>" if you prefer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the full value including the 'Bearer ' prefix. Example: \"Bearer eyJhbGciOi...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Require Bearer token for all operations by default (can be overridden per-operation)
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    };
    options.AddSecurityRequirement(securityRequirement);
});


// Core services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Token service
builder.Services.AddScoped<ITokenService, TokenService>();

// Auth service
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection.GetValue<string>("Key");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            // ensure role claim is mapped correctly
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                // log or inspect ctx.Exception.Message
                Console.WriteLine("JWT auth failed: " + ctx.Exception?.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("Token validated for: " + ctx.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

// Authorization policies (role-based and claim examples)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));

    // Example claim-based policy — demonstration only
    options.AddPolicy("UserHasIdClaim", policy => policy.RequireClaim("userId"));
});

var app = builder.Build();

// Seed a single admin user from configuration (or defaults) if not present
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userRepo = services.GetRequiredService<IUserRepository>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        var configuration = services.GetRequiredService<IConfiguration>();

        var adminTc = configuration["Admin:Tc"] ?? "11111111111";
        var adminPassword = configuration["Admin:Password"] ?? "Admin@12345";
        var adminFirst = configuration["Admin:FirstName"] ?? "System";
        var adminLast = configuration["Admin:LastName"] ?? "Admin";

        var existing = userRepo.GetByTcAsync(adminTc).GetAwaiter().GetResult();
        if (existing == null)
        {
            var hasher = new PasswordHasher<object>();
            var adminUser = new User
            {
                TcKimlikNo = adminTc,
                FirstName = adminFirst,
                LastName = adminLast,
                PhoneNumber = string.Empty,
                Role = "Admin",
                IsActive = true
            };
            adminUser.PasswordHash = hasher.HashPassword(null!, adminPassword);

            userRepo.AddAsync(adminUser).GetAwaiter().GetResult();
            unitOfWork.CommitAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Seeded admin user with TC: {adminTc}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while seeding admin user: " + ex.Message);
    }
}

// Always enable Swagger UI (or keep the IsDevelopment() check if you prefer)
// Serve Swagger UI at /swagger instead of root to avoid route conflicts
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "HospitalAppointment API v1");
    options.RoutePrefix = "swagger"; // UI available at /swagger
});

// existing pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
