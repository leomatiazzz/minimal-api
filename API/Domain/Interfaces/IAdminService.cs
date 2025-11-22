using MinimalAPI.Domain.Entities;
using MinimalAPI.DTOs;

namespace MinimalAPI.Domain.Interfaces;

public interface IAdminService
{
   Admin? Login(LoginDTO loginDTO);
   Admin Create(Admin admin);
   Admin? GetById(int id);
   List<Admin> All(int? page);
}