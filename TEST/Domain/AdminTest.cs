using MinimalAPI.Domain.Entities;

namespace TEST.Domain.Entities;

[TestClass]
public sealed class AdminTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var adm = new Admin();

        //Act
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Profile = "Admin";

        //Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual("Admin", adm.Profile);
    }
}
