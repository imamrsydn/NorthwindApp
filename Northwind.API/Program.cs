using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Northwind.DbAccess.Models;
using Northwind.Provider.Implementations;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Implementations;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Northwind.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Read JwtSettings from appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

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

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var responseObj = new
                        {
                            statusCode = 401,
                            message = "Akses ditolak. Token tidak valid atau tidak ditemukan.",
                            error = context.ErrorDescription ?? "Unauthorized"
                        };

                        var jsonResponse = JsonSerializer.Serialize(responseObj);
                        await context.Response.WriteAsync(jsonResponse);
                    }
                };
            });

            // Swagger Setup (Gaya sintaks asli Visual Studio kamu yang kompetibel dengan paket yang terinstall)
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Northwind_3 Rest API Documentation",
                    Description = "contoh penggunaan Swagger documentation"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    swagger.IncludeXmlComments(xmlPath);
                }

                var swaggerSecurityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                };
                swagger.AddSecurityDefinition("Bearer", swaggerSecurityScheme);
                swagger.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            builder.Services.AddOpenApi();

            // Register DbContext
            builder.Services.AddDbContext<Northwind3Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Load MailSettings dari appsettings.json
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            // Menambahkan service CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocal", policy =>
                {
                    //.WithOrigins("http://127.0.0.1:5500")
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()   // Mengizinkan GET, POST, PUT, DELETE, dll.
                    .AllowAnyHeader();  // Mengizinkan semua header
                });
            });

            // Register Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IShipperRepository, ShipperRepository>();
            builder.Services.AddScoped<ITerritoryRepository, TerritoryRepository>();
            builder.Services.AddScoped<IRegionRepository, RegionRepository>();
            builder.Services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            // Register Providers
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthProvider, AuthProvider>();
            builder.Services.AddScoped<ICategoryProvider, CategoryProvider>();
            builder.Services.AddScoped<ISupplierProvider, SupplierProvider>();
            builder.Services.AddScoped<IShipperProvider, ShipperProvider>();
            builder.Services.AddScoped<ITerritoryProvider, TerritoryProvider>();
            builder.Services.AddScoped<IRegionProvider, RegionProvider>();
            builder.Services.AddScoped<ICustomerTypeProvider, CustomerTypeProvider>();
            builder.Services.AddScoped<IProductProvider, ProductProvider>();
            builder.Services.AddScoped<ICustomerProvider, CustomerProvider>();
            builder.Services.AddScoped<IEmployeeProvider, EmployeeProvider>();
            builder.Services.AddScoped<IOrderProvider, OrderProvider>();
            builder.Services.AddScoped<IDashboardProvider, DashboardProvider>();
            builder.Services.AddScoped<IReportProvider, ReportProvider>();
            builder.Services.AddScoped<IUserAccountProvider, UserAccountProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Mengaktifkan Middleware CORS
            app.UseCors("AllowLocal");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}