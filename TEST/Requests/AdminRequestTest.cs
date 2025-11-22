using System.Net;
using System.Text;
using System.Text.Json;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.DTOs;
using TEST.Helpers;

namespace TEST.Requests;

[TestClass]
public sealed class AdminRequestTest
{
    [ClassInitialize]

    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]

    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

        //Act
        var response = await Setup.client.PostAsync("/admins/login", content);


        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLoggedIn = JsonSerializer.Deserialize<AdminLoggedIn>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLoggedIn?.Email ?? "");
        Assert.IsNotNull(admLoggedIn?.Profile ?? "");
        Assert.IsNotNull(admLoggedIn?.Token ?? "");

        Console.WriteLine(admLoggedIn?.Token);

    }
}
