using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Api.DTOs;

public record CreateContactDto(
    [Required][MaxLength(200)] string Name,
    [Required][Range(1, 149)] int Age,
    [Required][MinLength(1)] List<string> PhoneNumbers
);
