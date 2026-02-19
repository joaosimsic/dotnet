using PhoneBook.Api.DTOs;
using PhoneBook.Api.Models;
using PhoneBook.Api.Repositories;

namespace PhoneBook.Api.Services;

public class ContactService(IContactRepository repository, IDeletionLogService logService) : IContactService
{
    public async Task<PagedResultDto<ContactDto>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var (contacts, totalCount) = await repository.GetAll(page, pageSize);
        var items = contacts.Select(MapToDto).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<ContactDto>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<ContactDto?> GetByIdAsync(int id)
    {
        var contact = await repository.GetById(id);
        return contact == null ? null : MapToDto(contact);
    }

    public async Task<PagedResultDto<ContactDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        var (contacts, totalCount) = await repository.Search(searchTerm, page, pageSize);
        var items = contacts.Select(MapToDto).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<ContactDto>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        var contact = new Contact
        {
            Name = dto.Name,
            Age = dto.Age,
            Phones = dto.PhoneNumbers.Select(p => new Phone { PhoneNumber = p }).ToList()
        };

        var created = await repository.Create(contact);
        return MapToDto(created);
    }

    public async Task<ContactDto?> UpdateAsync(int id, UpdateContactDto dto)
    {
        var contact = new Contact
        {
            Name = dto.Name,
            Age = dto.Age,
            Phones = dto.PhoneNumbers.Select(p => new Phone { PhoneNumber = p }).ToList()
        };

        var updated = await repository.Update(id, contact);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<ContactDto?> DeleteAsync(int id)
    {
        var deleted = await repository.Delete(id);
        if (deleted == null)
            return null;

        var dto = MapToDto(deleted);
        await logService.LogDeletionAsync(dto);
        return dto;
    }

    private static ContactDto MapToDto(Contact contact) => new(
        contact.Id,
        contact.Name,
        contact.Age,
        contact.CreatedAt,
        contact.UpdatedAt,
        contact.Phones.Select(p => new PhoneDto(p.Id, p.PhoneNumber)).ToList()
    );
}
