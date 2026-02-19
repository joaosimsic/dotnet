using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Api.DTOs;

public record UpdateContactDto(
    [Required][MaxLength(200)] string Name,
    [Required][Range(1, 149)] int Age,
    [Required] List<string> PhoneNumbers
);

