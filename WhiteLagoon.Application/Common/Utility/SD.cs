using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Utility;

public static class SD
{
    public const string Role_Customer = "Customer";
    public const string Role_Admin = "Admin";
    
    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusCheckedIn = "CheckedIn";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";
    
    public static int VillaRoomsAvailable_Count(
        int villaId,
        List<VillaNumber> villaNumberList,
        DateOnly checkInDate,
        int nights,
        List<Booking> bookings)
    {
        Console.WriteLine($"Всего броней: {bookings.Count}");

        List<int> bookingInDate = new();
        int finalAvailableRoomForAllNights = int.MaxValue;
        var roomsInVilla = villaNumberList.Count(x => x.VillaId == villaId);

        for (int i = 0; i < nights; i++)
        {
            var currentDate = checkInDate.AddDays(i);
            Console.WriteLine($"\nДата: {currentDate}");

            var villasBooked = bookings.Where(u =>
                u.CheckInDate <= currentDate &&
                u.CheckOutDate > currentDate &&
                u.VillaId == villaId);

            foreach (var booking in villasBooked)
            {
                Console.WriteLine($"FOUND: Id={booking.Id}, VillaId={booking.VillaId}");

                if (!bookingInDate.Contains(booking.Id))
                {
                    bookingInDate.Add(booking.Id);
                }
            }

            var totalAvailableRooms = roomsInVilla - bookingInDate.Count;

            if (totalAvailableRooms == 0)
            {
                return 0;
            }

            if (finalAvailableRoomForAllNights > totalAvailableRooms)
            {
                finalAvailableRoomForAllNights = totalAvailableRooms;
            }
        }

        return finalAvailableRoomForAllNights;
    }
}