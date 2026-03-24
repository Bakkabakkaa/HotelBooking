using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        HomeVM homeVm = new HomeVM()
        {
            VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
            Nights = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Now)
        };

        return View(homeVm);

    }

    [HttpPost]
    public IActionResult Index(HomeVM homeVm)
    {
        homeVm.VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");

        foreach (var villa in homeVm.VillaList)
        {
            if (villa.Id % 2 == 0)
            {
                villa.IsAvailable = false;
            }
        }

        return View(homeVm);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Error()
    {
        return View();
    }
}