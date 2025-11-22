using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enuns;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestructure.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt").ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
        services.AddAuthorization();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token here: / Insira seu token JWT aqui: "
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
            });
        });


        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))

            );
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            #region Home

            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admins
            string GenerateTokenJwt(Admin admin)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", admin.Email),
                    new Claim("Profile", admin.Profile),
                    new Claim(ClaimTypes.Role, admin.Profile),
                };
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
            {
                var adm = adminService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GenerateTokenJwt(adm);
                    return Results.Ok(new AdminLoggedIn
                    {
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                    });
                }
                else

                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Admins");

            endpoints.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
            {
                var adms = new List<AdminModelView>();
                var admins = adminService.All(page);
                foreach (var admin in admins)
                {
                    adms.Add(new AdminModelView
                    {
                        Id = admin.Id,
                        Email = admin.Email,
                        Profile = admin.Profile
                    });
                }
                return Results.Ok(adms);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Admins");

            endpoints.MapGet("/admins/{id}", ([FromRoute] int id, IAdminService adminService) =>
            {
                var admin = adminService.GetById(id);

                if (admin == null)
                    return Results.NotFound();

                return Results.Ok(new AdminModelView
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Admins");

            endpoints.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
            {
                var validation = new ValidationErrors
                {
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(adminDTO.Email))
                    validation.Messages.Add("The Email field is required./O campo Email é obrigatório.");
                if (string.IsNullOrEmpty(adminDTO.Password))
                    validation.Messages.Add("The Password field is required./O campo Senha é obrigatório.");
                if (adminDTO.Profile == null)
                    validation.Messages.Add("The Profile field is required./O campo Perfil é obrigatório.");
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var admin = new Admin
                {
                    Email = adminDTO.Email,
                    Password = adminDTO.Password,
                    Profile = adminDTO.Profile.ToString() ?? Profile.Editor.ToString()
                };

                adminService.Create(admin);

                return Results.Created($"/admin/{admin.Id}", new AdminModelView
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Admins");
            #endregion

            #region Vehicles

            ValidationErrors validaDTO(VehicleDTO vehicleDTO)
            {
                var validation = new ValidationErrors
                {
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(vehicleDTO.Name))
                    validation.Messages.Add("The Name field is required./O campo Nome é obrigatório.");

                if (string.IsNullOrEmpty(vehicleDTO.Brand))
                    validation.Messages.Add("The Brand field is required./O campo Marca é obrigatório.");

                if (vehicleDTO.Year < 1886 || vehicleDTO.Year > DateTime.Now.Year + 1)
                    validation.Messages.Add(
                        $"The Year field must be between 1886 and {DateTime.Now.Year + 1}./O campo Ano deve estar entre 1886 e {DateTime.Now.Year + 1}.");

                return validation;
            }
            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var validation = validaDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                var vehicle = new Vehicle
                {
                    Name = vehicleDTO.Name,
                    Brand = vehicleDTO.Brand,
                    Year = vehicleDTO.Year
                };
                vehicleService.Create(vehicle);

                return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.All(page);

                return Results.Ok(vehicles);
            }).RequireAuthorization().WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);

                if (vehicle == null)
                    return Results.NotFound();

                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Editor" })
            .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);

                if (vehicle == null)
                    return Results.NotFound();

                var validation = validaDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                    return Results.BadRequest(validation);

                vehicle.Name = vehicleDTO.Name;
                vehicle.Brand = vehicleDTO.Brand;
                vehicle.Year = vehicleDTO.Year;

                vehicleService.Update(vehicle);

                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);

                if (vehicle == null)
                    return Results.NotFound();

                vehicleService.Delete(vehicle);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Vehicles");
            #endregion

        });
    }
}