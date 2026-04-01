using Microsoft.AspNetCore.Hosting;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation;

public class VillaService : IVillaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const string VillaImageFolder = "images/VillaImage";

    public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public void CreateVilla(Villa villa)
    {
        if (villa.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "VillaImage");
            Directory.CreateDirectory(imagePath);

            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            villa.Image.CopyTo(fileStream);

            villa.ImageUrl = $"/{VillaImageFolder}/{fileName}";
        }
        else
        {
            villa.ImageUrl = "https://placehold.co/600x400";
        }

        _unitOfWork.Villa.Add(villa);
        _unitOfWork.Save();
        // Примечание: После создания виллы необходимо добавить хотя бы один 
        // VillaNumber через админ-панель, иначе вилла будет отображаться как "Sold Out"
        // Это связано с тем, что доступность рассчитывается по количеству номеров виллы
    }

    public bool DeleteVilla(int id)
    {
        try
        {
            Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == id);
            if (objFromDb is not null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                {
                    var oldImagePath =
                        Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\', '/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Villa.Remove(objFromDb);
                _unitOfWork.Save();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IEnumerable<Villa> GetAllVillas()
    {
        return _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");
    }
    public Villa GetVillaById(int id)
    {
        return _unitOfWork.Villa.Get(u => u.Id == id, includeProperties: "VillaAmenity");
    }
    
    public IEnumerable<Villa> GetVillasAvailabilityByDate(int nights, DateOnly checkInDate)
    {
        var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
        var villaNumbersList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
                                                           u.Status == SD.StatusCheckedIn).ToList();


        foreach (var villa in villaList)
        {
            int roomAvailable = SD.VillaRoomsAvailable_Count
                (villa.Id, villaNumbersList, checkInDate, nights, bookedVillas);

            villa.IsAvailable = roomAvailable > 0 ? true : false;
        }

        return villaList;
    }

    public void UpdateVilla(Villa villa)
    {
        if (villa.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "VillaImage");
            Directory.CreateDirectory(imagePath);

            if (!string.IsNullOrEmpty(villa.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\', '/'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            villa.Image.CopyTo(fileStream);

            villa.ImageUrl = $"/{VillaImageFolder}/{fileName}";
        }

        _unitOfWork.Villa.Update(villa);
        _unitOfWork.Save();
    }
    
    public bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate)
    {
        var villaNumbersList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
            u.Status == SD.StatusCheckedIn).ToList();

        int roomAvailable = SD.VillaRoomsAvailable_Count
            (villaId, villaNumbersList, checkInDate, nights, bookedVillas);

        return roomAvailable > 0;
    }
}