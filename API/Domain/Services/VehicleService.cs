using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestructure.Db;

namespace MinimalAPI.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DbContexto _context;
    public VehicleService(DbContexto contexto)
    {
        _context = contexto;
    }

    public List<Vehicle> All(int? page = 1, string? name = null, string? brand = null)
    {
        var query = _context.Vehicles.AsQueryable();
         if(!string.IsNullOrEmpty(name))
         {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name.ToLower()}%"));
         }

        int itemsPerPage = 10;

        if(page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        } 
        return query.ToList();
    }

    public void Create(Vehicle vehicles)
    {
        _context.Vehicles.Add(vehicles);
        _context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? GetById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}