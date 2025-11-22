using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.DTOs;

namespace TEST.Mocks;

public class AdminServiceMock : IAdminService
{
    private static List<Admin> admins = new List<Admin>()
    {
        new Admin
        {
            Id = 1,
            Email = "adm@teste.com",
            Password = "123456",
            Profile = "Admin"
        },
        new Admin
        {
            Id = 2,
            Email = "editor@teste.com",
            Password = "123456",
            Profile = "Editor"
        }
    };
    public List<Admin> All(int? page)
    {
        return admins;
    }

    public Admin Create(Admin admin)
    {
        admin.Id = admins.Count() + 1;
        admins.Add(admin);

        return admin;
    }

    public Admin? GetById(int id)
    {
        return admins.Find(a => a.Id == id);
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        return admins.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }
}