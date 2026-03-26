using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
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
    public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
    {
        var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
        var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking
            .GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();

        foreach (var villa in villaList)
        {
            int roomAvailable =
                SD.VillaRoomsAvailable_Count(villa.Id, villaNumberList, checkInDate, nights, bookedVillas);

            Console.WriteLine($"Villa {villa.Id} villaNumberList: {villaNumberList}, available: {roomAvailable}");
            villa.IsAvailable = roomAvailable > 0 ? true : false;
        }

        HomeVM homeVm = new HomeVM()
        {
            CheckInDate = checkInDate,
            VillaList = villaList,
            Nights = nights
        };

        return PartialView("_VillaList", homeVm);
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