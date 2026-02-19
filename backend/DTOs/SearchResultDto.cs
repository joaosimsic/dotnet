namespace PhoneBook.Api.DTOs;

public record SearchResultDto(
    List<ContactDto> Contacts,
    int TotalCount
);
