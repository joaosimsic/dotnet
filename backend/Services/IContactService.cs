using PhoneBook.Api.DTOs;

namespace PhoneBook.Api.Services;

public interface IContactService
{
    Task<PagedResultDto<ContactDto>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<ContactDto?> GetByIdAsync(int id);
    Task<PagedResultDto<ContactDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<ContactDto> CreateAsync(CreateContactDto dto);
    Task<ContactDto?> UpdateAsync(int id, UpdateContactDto dto);
    Task<ContactDto?> DeleteAsync(int id);
}
