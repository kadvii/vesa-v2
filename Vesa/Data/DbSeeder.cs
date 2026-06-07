using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vesa.Models;

namespace Vesa.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Applicant"))
            await roleManager.CreateAsync(new IdentityRole("Applicant"));

        // 2. Seed Admin User
        if (!await userManager.Users.AnyAsync(u => u.Email == "admin@vesa.iq"))
        {
            var admin = new AppUser
            {
                UserName = "admin@vesa.iq",
                Email = "admin@vesa.iq",
                FullName = "Vesa Admin",
                PhoneNumber = "+9647700000000",
                NationalId = "0000000000",
                DateOfBirth = new DateOnly(1985, 1, 1),
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // 3. Seed Countries and Visa Types
        if (!await db.Countries.AnyAsync())
        {
            var countries = new List<Country>
            {
                new() { Id = Guid.NewGuid(), Name = "Turkey", IsoCode = "TR", FlagEmoji = "🇹🇷", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "United Arab Emirates", IsoCode = "AE", FlagEmoji = "🇦🇪", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Germany", IsoCode = "DE", FlagEmoji = "🇩🇪", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "United Kingdom", IsoCode = "GB", FlagEmoji = "🇬🇧", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Jordan", IsoCode = "JO", FlagEmoji = "🇯🇴", IsActive = true }
            };

            db.Countries.AddRange(countries);

            // Visa types map
            var visaTypes = new List<VisaType>
            {
                // Turkey
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[0].Id,
                    Name = "Tourist Visa",
                    Description = "Single entry tourist visa for Turkey, valid for up to 30 days.",
                    ProcessingDays = 7,
                    FeeAmount = 120000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "BankStatement", "HotelBooking"]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[0].Id,
                    Name = "Business Visa",
                    Description = "Single entry business visa for Turkey, requiring an invitation letter.",
                    ProcessingDays = 5,
                    FeeAmount = 200000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "SponsorLetter"]
                },
                // UAE
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[1].Id,
                    Name = "Tourist Visa (30 Days)",
                    Description = "Tourist visa for UAE valid for 30 days of stay.",
                    ProcessingDays = 3,
                    FeeAmount = 150000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "FlightBooking"]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[1].Id,
                    Name = "Tourist Visa (90 Days)",
                    Description = "Long-term tourist visa for UAE valid for 90 days of stay.",
                    ProcessingDays = 4,
                    FeeAmount = 350000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "FlightBooking", "HotelBooking"]
                },
                // Germany
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[2].Id,
                    Name = "Schengen Tourist Visa",
                    Description = "Schengen visa for tourism purposes in Germany and other Schengen countries.",
                    ProcessingDays = 15,
                    FeeAmount = 135000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "BankStatement", "TravelInsurance", "HotelBooking", "FlightBooking"]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[2].Id,
                    Name = "Schengen Business Visa",
                    Description = "Schengen visa for business meetings and trade fairs.",
                    ProcessingDays = 10,
                    FeeAmount = 135000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "BankStatement", "TravelInsurance", "SponsorLetter"]
                },
                // UK
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[3].Id,
                    Name = "Standard Visitor Visa",
                    Description = "Standard visitor visa for tourism, family visits, or short business trips.",
                    ProcessingDays = 20,
                    FeeAmount = 180000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "BankStatement", "HotelBooking", "FlightBooking"]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[3].Id,
                    Name = "Student Visa",
                    Description = "Visa for students enrolled in verified UK educational institutions.",
                    ProcessingDays = 15,
                    FeeAmount = 500000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "BankStatement", "SponsorLetter"]
                },
                // Jordan
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[4].Id,
                    Name = "Entry Permit",
                    Description = "Standard entry permit for Jordan travel.",
                    ProcessingDays = 2,
                    FeeAmount = 50000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo"]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CountryId = countries[4].Id,
                    Name = "Tourist Visa",
                    Description = "Tourist visa for Jordan travel requiring hotel confirmation.",
                    ProcessingDays = 5,
                    FeeAmount = 80000m,
                    IsActive = true,
                    RequiredDocuments = ["PassportCopy", "Photo", "HotelBooking"]
                }
            };

            db.VisaTypes.AddRange(visaTypes);
            await db.SaveChangesAsync();
        }

        if (!await db.AppointmentSlots.AnyAsync())
        {
            var seededCountries = await db.Countries.ToListAsync();
            if (seededCountries.Any())
            {
                var slots = new List<AppointmentSlot>();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                foreach (var country in seededCountries)
                {
                    slots.Add(new AppointmentSlot
                    {
                        Id = Guid.NewGuid(),
                        CountryId = country.Id,
                        Date = today.AddDays(1),
                        Time = new TimeOnly(10, 0),
                        MaxCapacity = 5,
                        BookedCount = 0,
                        IsActive = true
                    });

                    slots.Add(new AppointmentSlot
                    {
                        Id = Guid.NewGuid(),
                        CountryId = country.Id,
                        Date = today.AddDays(2),
                        Time = new TimeOnly(14, 30),
                        MaxCapacity = 5,
                        BookedCount = 0,
                        IsActive = true
                    });
                }

                db.AppointmentSlots.AddRange(slots);
                await db.SaveChangesAsync();
            }
        }
    }
}
