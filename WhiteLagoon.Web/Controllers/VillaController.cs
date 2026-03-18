using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers;

public class VillaController : Controller
{
    private readonly ApplicationDbContext _db;

    public VillaController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var villas = _db.Villas.ToList();
        return View(villas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Villa obj)
    {
        if (ModelState.IsValid)
        {
            _db.Villas.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index", "Villa");
        }

        return View();
    }
}