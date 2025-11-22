using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infraestructure.Db;

namespace TEST.Domain.Entities;

[TestClass]
public sealed class AdminServiceTest
{
    private DbContexto CreateTestContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }
    [TestMethod]
    public void GetByIdTest()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins;");

        var adm = new Admin();
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Profile = "Admin";

        var adminService = new AdminService(context);

        //Act
        adminService.Create(adm);
        var admFromDB = adminService.GetById(adm.Id);

        //Assert
        //Assert.AreEqual(1, adminService.All(1).Count());
        Assert.AreEqual(1, admFromDB?.Id);
    }
}
