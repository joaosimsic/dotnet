namespace PhoneBook.Api.DTOs;

public record ContactDto(
    int Id,
    string Name,
    int Age,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<PhoneDto> Phones
);
