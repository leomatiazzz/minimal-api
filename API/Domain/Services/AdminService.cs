using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestructure.Db;

namespace MinimalAPI.Domain.Services;

public class AdminService : IAdminService
{
    private readonly DbContexto _context;
    public AdminService(DbContexto contexto)
    {
        _context = contexto;
    }

    public List<Admin> All(int? page)
    {
        var query = _context.Admins.AsQueryable();

        int itemsPerPage = 10;

        if(page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        } 
        return query.ToList();
    }

    public Admin? GetById(int id)
    {
        return _context.Admins.Where(v => v.Id == id).FirstOrDefault();
    }
    
    public Admin Create(Admin admin)
    {
        _context.Admins.Add(admin);
        _context.SaveChanges();

        return admin;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm = _context.Admins.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
}