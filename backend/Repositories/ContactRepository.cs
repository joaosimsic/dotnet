using Microsoft.EntityFrameworkCore;
using PhoneBook.Api.Data;
using PhoneBook.Api.Models;

namespace PhoneBook.Api.Repositories;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public async Task<(List<Contact> Items, int TotalCount)> GetAll(int page, int pageSize)
    {
        var query = context.Contacts.Include(c => c.Phones).OrderBy(c => c.Name);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Contact?> GetById(int id)
    {
        return await context.Contacts
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Contact> Items, int TotalCount)> Search(string searchTerm, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAll(page, pageSize);

        var normalizedTerm = searchTerm.ToUpperInvariant();

        var query = context.Contacts
            .Include(c => c.Phones)
            .Where(c => c.Name.ToUpper().Contains(normalizedTerm) ||
                        c.Phones.Any(p => p.PhoneNumber.ToUpper().Contains(normalizedTerm)))
            .OrderBy(c => c.Name);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Contact> Create(Contact contact)
    {
        contact.CreatedAt = DateTime.UtcNow;
        contact.UpdatedAt = DateTime.UtcNow;

        foreach (var phone in contact.Phones)
        {
            phone.CreatedAt = DateTime.UtcNow;
        }

        context.Contacts.Add(contact);
        await context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact?> Update(int id, Contact contact)
    {
        var existing = await context.Contacts
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (existing == null)
            return null;

        existing.Name = contact.Name;
        existing.Age = contact.Age;
        existing.UpdatedAt = DateTime.UtcNow;

        context.Phones.RemoveRange(existing.Phones);

        foreach (var phone in contact.Phones)
        {
            phone.ContactId = id;
            phone.CreatedAt = DateTime.UtcNow;
            context.Phones.Add(phone);
        }

        await context.SaveChangesAsync();

        return await GetById(id);
    }

    public async Task<Contact?> Delete(int id)
    {
        var contact = await context.Contacts
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null)
            return null;

        context.Contacts.Remove(contact);
        await context.SaveChangesAsync();
        return contact;
    }
}
