using MinimalAPI.Domain.Entities;
using MinimalAPI.DTOs;

namespace MinimalAPI.Domain.Interfaces;

public interface IVehicleService
{
   List<Vehicle> All(int? page = 1, string? name = null, string? brand = null);
   Vehicle? GetById(int id);
   void Create(Vehicle vehicles);
   void Update(Vehicle vehicle);
   void Delete(Vehicle vehicle);
}