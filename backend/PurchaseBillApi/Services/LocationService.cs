using Microsoft.EntityFrameworkCore;
using PurchaseBillApi.Data;
using PurchaseBillApi.DTOs;

namespace PurchaseBillApi.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetLocationsForUserAsync(string username);
}

/// <summary>Reads back the locations that were saved during login (Task 1 / used by Task 2's Batch dropdown).</summary>
public class LocationService : ILocationService
{
    private readonly AppDbContext _db;

    public LocationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<LocationDto>> GetLocationsForUserAsync(string username)
    {
        return await _db.LocationDetails
            .Where(l => l.Username == username)
            .OrderBy(l => l.Location_Name)
            .Select(l => new LocationDto
            {
                Location_Code = l.Location_Code,
                Location_Name = l.Location_Name
            })
            .ToListAsync();
    }
}
