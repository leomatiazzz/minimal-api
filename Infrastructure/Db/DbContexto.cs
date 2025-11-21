using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;
namespace MinimalAPI.Infraestructure.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public DbContexto(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Admin> Admins { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = 1,
                Email = "administrador@teste.com",
                Password = "123456",
                Profile = "Admin"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured){
            var stringConnection = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
            if(!string.IsNullOrEmpty(stringConnection))
            {
                optionsBuilder.UseMySql(
                stringConnection, 
                ServerVersion.AutoDetect(stringConnection));
                return;
            }
        }
    }
}