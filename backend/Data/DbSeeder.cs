using Microsoft.EntityFrameworkCore;
using PhoneBook.Api.Models;

namespace PhoneBook.Api.Data;

public static class DbSeeder
{
  public static async Task SeedAsync(AppDbContext context, ILogger logger)
  {
    var count = await context.Contacts.CountAsync();
    if (count > 0)
    {
      logger.LogInformation("Database already seeded, skipping");
      return;
    }

    logger.LogInformation("Seeding database with initial contacts");

    List<Contact> contacts =
    [
        new()
            {
                Name = "John Smith",
                Age = 32,
                Phones = [
                    new() { PhoneNumber = "+1-555-0101" },
                    new() { PhoneNumber = "+1-555-0102" }
                ]
            },
            new()
            {
                Name = "Jane Doe",
                Age = 28,
                Phones = [new() { PhoneNumber = "+1-555-0201" }]
            },
            new()
            {
                Name = "Robert Johnson",
                Age = 45,
                Phones = [
                    new() { PhoneNumber = "+1-555-0301" },
                    new() { PhoneNumber = "+1-555-0302" },
                    new() { PhoneNumber = "+1-555-0303" }
                ]
            },
            new()
            {
                Name = "Emily Davis",
                Age = 24,
                Phones = [new() { PhoneNumber = "+1-555-0401" }]
            },
            new()
            {
                Name = "Michael Brown",
                Age = 38,
                Phones = [
                    new() { PhoneNumber = "+1-555-0501" },
                    new() { PhoneNumber = "+1-555-0502" }
                ]
            },
            new()
            {
                Name = "Sarah Wilson",
                Age = 31,
                Phones = [new() { PhoneNumber = "+1-555-0601" }]
            },
            new()
            {
                Name = "David Martinez",
                Age = 42,
                Phones = [
                    new() { PhoneNumber = "+1-555-0701" },
                    new() { PhoneNumber = "+1-555-0702" }
                ]
            },
            new()
            {
                Name = "Lisa Anderson",
                Age = 29,
                Phones = [new() { PhoneNumber = "+1-555-0801" }]
            },
            new()
            {
                Name = "James Taylor",
                Age = 55,
                Phones = [
                    new() { PhoneNumber = "+1-555-0901" },
                    new() { PhoneNumber = "+1-555-0902" }
                ]
            },
            new()
            {
                Name = "Jennifer Thomas",
                Age = 33,
                Phones = [new() { PhoneNumber = "+1-555-1001" }]
            },
            new()
            {
                Name = "William Garcia",
                Age = 48,
                Phones = [
                    new() { PhoneNumber = "+1-555-1101" },
                    new() { PhoneNumber = "+1-555-1102" }
                ]
            },
            new()
            {
                Name = "Amanda Rodriguez",
                Age = 26,
                Phones = [new() { PhoneNumber = "+1-555-1201" }]
            }
    ];

    context.Contacts.AddRange(contacts);
    await context.SaveChangesAsync();

    logger.LogInformation("Seeded {Count} contacts", contacts.Count);
  }
}
