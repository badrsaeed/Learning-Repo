using ApiCashingApp.Data;
using ApiCashingApp.Models;
using ApiCashingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCashingApp.Controllers;

[ApiController]
[Route("[controller]")]

public class DriversController : Controller
{
    private readonly ApiDbContext _context;
    private readonly ICacheService _cacheService;

    private string driversKey = "drivers";

    public DriversController(
        ApiDbContext context,
        ICacheService cacheService
    )
    {
        _context = context;
        _cacheService = cacheService;
    }


    [HttpGet("drivers")]
    public async Task<IActionResult> Get()
    {
        var cachedDrivers = _cacheService.GetData<IEnumerable<Driver>>(driversKey);
        if(cachedDrivers is not null && cachedDrivers.Any())
            return Ok(cachedDrivers);

        var drivers = await _context.Drivers.ToListAsync();
        var isCached = _cacheService.SetData<IEnumerable<Driver>>(driversKey, drivers, DateTimeOffset.Now.AddMinutes(2));

        return Ok(drivers);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Driver driver)
    {

        try
        {
            await _context.Drivers.AddAsync(driver);
            await _context.SaveChangesAsync();
            return Ok(driver);
        }catch(Exception ex){
            throw;
        }

    }
}