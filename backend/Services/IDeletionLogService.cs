using PhoneBook.Api.DTOs;

namespace PhoneBook.Api.Services;

public interface IDeletionLogService
{
    Task LogDeletionAsync(ContactDto contact);
}
